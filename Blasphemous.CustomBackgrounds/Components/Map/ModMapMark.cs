using Blasphemous.CustomBackgrounds.Components.Animations;
using Blasphemous.CustomBackgrounds.Extensions;
using Blasphemous.ModdingAPI.Files;
using Framework.Map;
using HarmonyLib;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Blasphemous.CustomBackgrounds.Components.Map;

/// <summary>
/// Class for custom map marker
/// </summary>
public class ModMapMark
{
    protected internal readonly AnimationInfo animationInfo;
    protected internal readonly Sprite sprite;
    internal ModMapMarkInfo info;
    internal CellKey cellKey;
    private GameObject _gameObj;

    internal GameObject GameObj
    {
        get
        {
            if (_gameObj == null)
                InitializeGameObject();

            return _gameObj;
        }

        set
        {
            _gameObj = value;
        }
    }

    internal Image ObjImage => GameObj.GetComponent<Image>();

    internal ModMapMark(FileHandler fileHandler, ModMapMarkInfo mapMarkInfo)
    {
        cellKey = new(-999, -999);
        this.info = mapMarkInfo;
        switch (mapMarkInfo.spriteType)
        {
            case ModMapMarkInfo.SpriteType.Static:
                if (!Main.TryImportSprite(fileHandler, info.fileName, mapMarkInfo.spriteImportInfo, out sprite))
                {
                    throw new ArgumentException($"Failed loading static map mark `{mapMarkInfo.id}`!");
                }
                break;
            case ModMapMarkInfo.SpriteType.Animated:
                if (!Main.TryImportAnimation(fileHandler, info.fileName, mapMarkInfo.animationImportInfo, out animationInfo))
                {
                    throw new ArgumentException($"Failed loading animated map mark `{mapMarkInfo.id}`!");
                }
                break;
        }
    }

    public ModMapMark(FileHandler fileHandler, string mapMarkInfoFileLocation)
        : this(fileHandler, fileHandler.LoadDataAsJson<ModMapMarkInfo>(mapMarkInfoFileLocation))
    { }

    /// <summary>
    /// Copy constructor
    /// </summary>
    public ModMapMark(ModMapMark other)
    {
        animationInfo = other.animationInfo;
        sprite = other.sprite;
        info = other.info;
        cellKey = new(-999, -999);
    }

    /// <summary>
    /// Copy to a new location
    /// </summary>
    public ModMapMark(ModMapMark other, CellKey cellKey)
    {
        animationInfo = other.animationInfo;
        sprite = other.sprite;
        info = other.info;
        this.cellKey = cellKey;
    }

    internal void InitializeGameObject()
    {
        // initialize GameObject with a RectTransform
        _gameObj = new GameObject($"ModMapMark_{info.id}_{cellKey}", new System.Type[] { typeof(RectTransform) });

        // initialize image
        _gameObj.AddComponent<Image>();
        switch (info.spriteType)
        {
            case ModMapMarkInfo.SpriteType.Static:
                ObjImage.sprite = sprite;
                break;
            case ModMapMarkInfo.SpriteType.Animated:
                ModImageAnimator anim = _gameObj.AddComponent<ModImageAnimator>();
                anim.Animation = animationInfo;
                break;
            default:
                throw new NotImplementedException();
        }

        // adjust RectTransform
        RectTransform rectTransform = _gameObj.GetComponent<RectTransform>();
        rectTransform.SetParent(Traverse.Create(ModMapManager.VanillaMapRenderer).Field("markRoot").GetValue<RectTransform>());
        rectTransform.localRotation = Quaternion.identity;
        rectTransform.localScale = Vector3.one;
        rectTransform.localPosition = ModMapManager.GetMapPositionByCellKey(cellKey);
        rectTransform.sizeDelta = new Vector2(ObjImage.sprite.rect.width, ObjImage.sprite.rect.height);

        // finalize initialization
        _gameObj.SetActive(false);
    }

    internal void SetActive(bool active)
    {
        if (active)
        {
            GameObj.SetActive(true);
        }
        else
        {
            _gameObj?.SetActive(false);
        }
    }
}
