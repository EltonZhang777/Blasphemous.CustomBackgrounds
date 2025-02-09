using Blasphemous.CustomBackgrounds.Components.Animations;
using Blasphemous.CustomBackgrounds.Components.Sprites;
using Blasphemous.ModdingAPI;
using Blasphemous.ModdingAPI.Files;
using System;
using UnityEngine;

namespace Blasphemous.CustomBackgrounds.Components.Backgrounds;

/// <summary>
/// Custom main menu background object
/// </summary>
public class Background
{
    /// <summary>
    /// The mod that registered the background.
    /// </summary>
    public string parentModId;

    internal BackgroundInfo backgroundInfo;
    internal bool isApplied = false;
    private GameObject _gameObj;
    private readonly FileHandler _fileHandler;
    private readonly AnimationInfo _animationInfo;
    private readonly Sprite _sprite;

    internal GameObject GameObj
    {
        get
        {
            if (_gameObj == null)
            {
                InitializeGameObject();
            }
            return _gameObj;
        }
        set
        {
            _gameObj = value;
        }
    }

    public Background(
        FileHandler fileHandler,
        BackgroundInfo backgroundInfo)
    {
        this._fileHandler = fileHandler;
        this.backgroundInfo = backgroundInfo;
        switch (backgroundInfo.spriteType)
        {
            case BackgroundInfo.SpriteType.Static:
                if (!TryImportSprite(fileHandler, backgroundInfo.spriteImportInfo, out _sprite))
                {
                    throw new ArgumentException($"Failed loading static background `{backgroundInfo.name}`!");
                }
                break;
            case BackgroundInfo.SpriteType.Animated:
                if (!TryImportAnimation(fileHandler, backgroundInfo.animationImportInfo, out _animationInfo))
                {
                    throw new ArgumentException($"Failed loading animated background `{backgroundInfo.name}`!");
                }
                break;
        }
    }

    internal void InitializeGameObject()
    {
        _gameObj = new GameObject($"Background[{backgroundInfo.name}]");
        switch (backgroundInfo.spriteType)
        {
            case BackgroundInfo.SpriteType.Static:
                SpriteRenderer sr = _gameObj.AddComponent<SpriteRenderer>();
                sr.sortingOrder = -1;
                sr.sprite = _sprite;
                break;
            case BackgroundInfo.SpriteType.Animated:
                ModAnimator anim = _gameObj.AddComponent<ModAnimator>();
                anim.Animation = _animationInfo;
                break;
        }
    }

    internal bool TryImportAnimation(
        FileHandler fileHandler,
        AnimationImportInfo importInfo,
        out AnimationInfo animationInfo)
    {
        var options = new SpriteImportOptions()
        {
            Pivot = new Vector2(0.5f, 0)
        };

        if (!fileHandler.LoadDataAsFixedSpritesheet(importInfo.FilePath, new Vector2(importInfo.Width, importInfo.Height), out Sprite[] spritesheet, options))
        {
            ModLog.Error($"Failed to load {importInfo.Name} from {importInfo.FilePath}");
            animationInfo = null;
            return false;
        }

        animationInfo = new AnimationInfo(importInfo.Name, spritesheet, importInfo.SecondsPerFrame);
        return true;
    }

    internal bool TryImportSprite(
        FileHandler fileHandler,
        SpriteImportInfo importInfo,
        out Sprite sprite)
    {
        var options = new SpriteImportOptions()
        {
            Pivot = new Vector2(importInfo.Pivot.x, importInfo.Pivot.y),
            PixelsPerUnit = importInfo.PixelsPerUnit,
        };

        if (!fileHandler.LoadDataAsSprite($"{importInfo.Name}.png", out sprite, options))
        {
            ModLog.Error($"Failed to load sprite {importInfo.Name}");
            return false;
        }

        return true;
    }
}
