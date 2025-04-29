using Blasphemous.CustomBackgrounds.Persistence;
using Blasphemous.ModdingAPI;
using DG.Tweening;
using Framework.Managers;
using Framework.Map;
using Gameplay.UI;
using Gameplay.UI.Others.MenuLogic;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Blasphemous.CustomBackgrounds.Components.Map;

public static class ModMapManager
{
    internal static List<ModMapMark> modMapMarkInstances = new();
    internal static Dictionary<string, ModMapMark> registeredMarks = new();

    public static NewMapMenuWidget VanillaMapWidget => GameObject.Find($"Game UI/Content/UI_PAUSE/UI_NEWMAP").GetComponent<NewMapMenuWidget>();
    public static MapRenderer VanillaMapRenderer => Traverse.Create(VanillaMapWidget).Field("CurrentRenderer").GetValue<MapRenderer>();
    public static PauseWidget VanillaPauseWidget => Traverse.Create(UIController.instance).Field("pauseWidget").GetValue<PauseWidget>();
    public static MapData VanillaCurrentMap => Traverse.Create(Core.NewMapManager).Field("CurrentMap").GetValue<MapData>();
    public static Vector2 MapCellSize => new(
        Traverse.Create(VanillaMapRenderer).Field("CellSizeX").GetValue<float>(),
        Traverse.Create(VanillaMapRenderer).Field("CellSizeY").GetValue<float>());

    public static void RegisterMapMark(
        this ModServiceProvider provider,
        ModMapMark modMapMark)
    {
        if (provider == null)
            return;

        string id = modMapMark.info.id;
        if (MapMarkTypeExists(id))
        {
            // prevent repeated registering
            ModLog.Error($"A ModMapMark with ID `{id}` has already been registered!");
            return;
        }
        modMapMark.info.registeringModId = provider.RegisteringMod.Id;
        registeredMarks.Add(id, modMapMark);
        ModLog.Info($"Registered ModMapMark: {id}");
    }

    public static void AddMapMark(string id, CellKey cellKey)
    {
        if (!MapMarkTypeExists(id, true) || MapMarkInstanceExistsAtLocation(id, cellKey))
            return;

        modMapMarkInstances.Add(new ModMapMark(registeredMarks[id], cellKey));
    }

    public static void RemoveMapMark(string id, CellKey cellKey)
    {
        if (!MapMarkTypeExists(id, true) || !MapMarkInstanceExistsAtLocation(id, cellKey))
            return;

        try
        {
            modMapMarkInstances.Remove(modMapMarkInstances.FirstOrDefault(x => (x.cellKey.Equals(cellKey)) && (x.info.id == id)));
        }
        catch { }
    }

    public static void ClearAllMapMarks()
    {
        modMapMarkInstances?.Clear();
    }

    public static bool MapMarkTypeExists(string id, bool logError = false, bool throwError = false)
    {
        bool result = registeredMarks.ContainsKey(id);

        string errorMessage = $"ModMapMark of ID `{id}` does not exist!";
        if (result == false)
        {
            if (throwError)
                throw new ArgumentException(errorMessage);

            if (logError)
                ModLog.Error(errorMessage);
        }
        return result;
    }

    public static bool MapMarkInstanceExistsAtLocation(string id, CellKey cellKey, bool logError = false, bool throwError = false)
    {
        if (!MapMarkTypeExists(id))
            return false;

        bool result = modMapMarkInstances.Select(x => x.cellKey).Any(x => x.Equals(cellKey)) && modMapMarkInstances.Any(x => x.info.id == id);

        string errorMessage = $"ModMapMark of type `{id}` already exists at location {cellKey}!";
        if (result == false)
        {
            if (throwError)
                throw new ArgumentException(errorMessage);

            if (logError)
                ModLog.Error(errorMessage);
        }
        return result;
    }

    public static Vector2 GetMapPositionByCellKey(CellKey cellKey)
    {
        return new Vector2(
            MapCellSize.x * (float)cellKey.X,
            MapCellSize.y * (float)cellKey.Y);
    }

    public static Vector2 GetMapPositionByWorldPosition(Vector2 worldPosition)
    {
        CellKey cellKey = Core.NewMapManager.GetCellKeyFromPosition(worldPosition);
        Vector2 mapPosition = GetMapPositionByCellKey(cellKey);
        return mapPosition;
    }

    /// <summary>
    /// Instantly move and center the map widget cursor to the room where a certain world position is.
    /// </summary>
    public static void MoveMapCenterToWorldPosition(Vector2 worldPosition)
    {
        Vector2 mapPosition = -1f * GetMapPositionByWorldPosition(worldPosition);
        VanillaMapRenderer.MoveCenterTo(mapPosition);
    }

    /// <summary>
    /// Smoothly moves the map widget cursor to the room where a certain world position is, with the given duration (in seconds).
    /// </summary>
    public static Tween TweenMapCenterToWorldPosition(Vector2 worldPosition, float duration)
    {
        Vector2 mapPositionEnd = -1f * GetMapPositionByWorldPosition(worldPosition);
        return DOTween.To(
            () => VanillaMapRenderer.Center,
            (x) => VanillaMapRenderer.Center = x,
            mapPositionEnd,
            duration);
    }

    public static void ForceRevealCell(CellKey cellKey)
    {
        CellData cellData = VanillaCurrentMap.CellsByCellKey[cellKey];
        cellData.Revealed = true;
#if DEBUG
        ModLog.Warn($"cellKey: {VanillaCurrentMap.CellsByCellKey[cellKey].CellKey}\nRevealed: {VanillaCurrentMap.CellsByCellKey[cellKey].Revealed}\nBounding: {VanillaCurrentMap.CellsByCellKey[cellKey].Bounding}");
#endif
    }

    /// <summary>
    /// Show/Hide vanilla map widget
    /// </summary>
    public static void ShowMap(bool show)
    {
        if (show && !VanillaPauseWidget.IsActive())
        {
            VanillaPauseWidget.InitialWidget = PauseWidget.ChildWidgets.MAP;
            VanillaPauseWidget.InitialMapMode = PauseWidget.MapModes.SHOW;
            VanillaPauseWidget.FadeShow(false, true, true);
            return;
        }
        else if (!show && VanillaPauseWidget.IsActive())
        {
            VanillaPauseWidget.FadeHide();
            return;
        }
    }

    public static void ForceRenderMap()
    {
        VanillaMapRenderer.Render(
            Core.NewMapManager.GetAllRevealedCells(),
            Core.NewMapManager.GetAllRevealSecretsCells(),
            Core.NewMapManager.GetAllMarks(),
            Core.NewMapManager.GetPlayerCell());
    }

    public static void ShowAllModMapMarks(bool show)
    {
        modMapMarkInstances.ForEach(x => x.SetActive(show));
    }

    internal static List<ModMapMarkSaveData> GetCurrentMapMarksSaveData()
    {
        List<ModMapMarkSaveData> result = new();
        result = modMapMarkInstances.Select(x => new ModMapMarkSaveData() { id = x.info.id, cellKeyLocation = x.cellKey.GetVector2() }).ToList();
        return result;
    }

    internal static void LoadMapMarksSaveData(List<ModMapMarkSaveData> saveData)
    {
        modMapMarkInstances = saveData.Select(item =>
        {
            ModMapMark result = new ModMapMark(registeredMarks[item.id]);
            result.cellKey = new(item.cellKeyLocation.x, item.cellKeyLocation.y);
            return result;
        }).ToList();
    }
    internal static void ConvertSceneNameToDZS(string levelName, out string d, out string z, out string s)
    {
        // DZS-scene name regex pattern
        string pattern = @"^D(\d{2})(?:B?Z(\d{2}))?S(\d{2})$";

        Match match = Regex.Match(levelName, pattern);
        if (match.Success)
        {
            d = $"D{match.Groups[1].Value}";
            z = match.Groups[2].Value;
            s = $"S{match.Groups[3].Value}";
        }
        else
        {
            string errorMessage = $"Invalid level name format: {levelName}";
            ModLog.Error(errorMessage);
            throw new ArgumentException(errorMessage);
        }
    }
}
