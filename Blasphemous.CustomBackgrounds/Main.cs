using BepInEx;
using Blasphemous.CustomBackgrounds.Components.Animations;
using Blasphemous.CustomBackgrounds.Components.Sprites;
using Blasphemous.ModdingAPI;
using Blasphemous.ModdingAPI.Files;
using System;
using UnityEngine;

namespace Blasphemous.CustomBackgrounds;

[BepInPlugin(ModInfo.MOD_ID, ModInfo.MOD_NAME, ModInfo.MOD_VERSION)]
[BepInDependency("Blasphemous.ModdingAPI", "2.4.1")]
[BepInDependency("Blasphemous.CheatConsole", "1.0.1")]
public class Main : BaseUnityPlugin
{
    public static CustomBackgrounds CustomBackgrounds { get; private set; }

    private void Start()
    {
        CustomBackgrounds = new CustomBackgrounds();
    }

    public static T Validate<T>(T obj, Func<T, bool> validate)
    {
        return validate(obj)
            ? obj
            : throw new Exception($"{obj} is an invalid import argument");
    }

    public static string ColorString(string input, string colorStr)
    {
        return $"<color={colorStr.ToUpper()}>" + input + $"</color>";
    }

    public static string ColorString(string input, Color color)
    {
        return ColorString(input, ColorUtility.ToHtmlStringRGB(color));
    }
    /// <summary>
    /// Try importing animation by passing in AnimationImportInfo
    /// </summary>
    public static bool TryImportAnimation(
        FileHandler fileHandler,
        string fileName,
        AnimationImportInfo importInfo,
        out AnimationInfo animationInfo)
    {
        var options = new SpriteImportOptions()
        {
            Pivot = new Vector2(0.5f, 0)
        };

        if (!fileHandler.LoadDataAsFixedSpritesheet(fileName, new Vector2(importInfo.Width, importInfo.Height), out Sprite[] spritesheet, options))
        {
            ModLog.Error($"Failed to load animation at `{fileName}`!");
            animationInfo = null;
            return false;
        }

        animationInfo = new AnimationInfo(spritesheet, importInfo.SecondsPerFrame);
        return true;
    }

    /// <summary>
    /// Try importing sprite by passing in SpriteImportInfo
    /// </summary>
    public static bool TryImportSprite(
        FileHandler fileHandler,
        string fileName,
        SpriteImportInfo importInfo,
        out Sprite sprite)
    {
        var options = new SpriteImportOptions()
        {
            Pivot = new Vector2(importInfo.Pivot.X, importInfo.Pivot.Y),
            PixelsPerUnit = importInfo.PixelsPerUnit,
        };

        if (!fileHandler.LoadDataAsSprite(fileName, out sprite, options))
        {
            ModLog.Error($"Failed to load sprite at `{fileName}`!");
            return false;
        }

        return true;
    }
}
