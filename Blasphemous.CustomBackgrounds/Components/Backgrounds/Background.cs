using Blasphemous.CustomBackgrounds.Components.Animations;
using Blasphemous.CustomBackgrounds.Components.Sprites;
using Blasphemous.CustomBackgrounds.Extensions;
using Blasphemous.ModdingAPI;
using Blasphemous.ModdingAPI.Files;
using Framework.Managers;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

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

    internal string LocalizedName
    {
        get
        {
            string currentLanguage = Core.Localization.GetCurrentLanguageCode();

            // The language exists and localization string is not null
            if (backgroundInfo.localization.ContainsKey(currentLanguage) && !string.IsNullOrEmpty(backgroundInfo.localization[currentLanguage]))
            {
                return backgroundInfo.localization[currentLanguage];
            }

            // The language doesn't exist
            ModLog.Warn($"Localization string of {currentLanguage} for {backgroundInfo.name} doesn't exist!");
            if (backgroundInfo.localization.ContainsKey("en"))
            {
                // use English if it exists
                return backgroundInfo.localization["en"];
            }
            else if (backgroundInfo.localization.Count > 0)
            {
                // use first language with a non-null localization string if it exists
                return backgroundInfo.localization.Values.First(x => !string.IsNullOrEmpty(x));
            }

            ModLog.Error($"Failed to localize background `{backgroundInfo.name}` to any language!");
            return "#LOC_ERROR";
        }
    }

    /// <summary>
    /// Constructor for custom background object
    /// </summary>
    /// <param name="fileHandler">The registering mod's FileHandler</param>
    /// <param name="backgroundInfo">Deserialized info object</param>
    public Background(
        FileHandler fileHandler,
        BackgroundInfo backgroundInfo)
    {
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

    /// <summary>
    /// Recommended constructor for custom background object. Handles JSON deserialization on its own.
    /// </summary>
    /// <param name="fileHandler">The registering mod's FileHandler</param>
    /// <param name="backgroundInfoJsonFileLocation">JSON file location of `backgroundInfo`</param>
    public Background(
        FileHandler fileHandler,
        string backgroundInfoJsonFileLocation)
        : this(fileHandler, fileHandler.LoadDataAsJson<BackgroundInfo>(backgroundInfoJsonFileLocation))
    { }

    internal void InitializeGameObject()
    {
        _gameObj = new GameObject($"Background[{backgroundInfo.name}]");
        _gameObj.transform.position = new Vector3(0f, 0f, 99f);
        _gameObj.layer = LayerMask.NameToLayer("UI");

        // set RectTransform to fit screen
        RectTransform rectTransform = _gameObj.AddComponent<RectTransform>();
        rectTransform.anchorMin = new Vector2(0, 0);
        rectTransform.anchorMax = new Vector2(0, 0);
        rectTransform.pivot = new Vector2(0, 0);
        rectTransform.position = new Vector3(0f, 0f, 0f);
        rectTransform.sizeDelta = new Vector2(640f, 360f);

        // set Image component
        Image image = _gameObj.AddComponent<Image>();
        image.raycastTarget = true;
        image.preserveAspect = true;
        image.type = Image.Type.Simple;

        // add sprite/animation to Image
        switch (backgroundInfo.spriteType)
        {
            case BackgroundInfo.SpriteType.Static:
                image.sprite = _sprite;
                break;
            case BackgroundInfo.SpriteType.Animated:
                ModImageAnimator anim = _gameObj.AddComponent<ModImageAnimator>();
                anim.Animation = _animationInfo;
                break;
        }
    }

    internal void SetGameObjectLayer()
    {
        Transform targetTransform = GameObject.Find($"Game UI/Content/UI_MAINMENU/Menu").transform;
        _gameObj.transform.SetParent(targetTransform, false);
        int index = GameObject.Find($"Game UI/Content/UI_MAINMENU/Menu/StaticBackground").transform.GetSiblingIndex();
        _gameObj.transform.SetSiblingIndex(index + 1);
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

        if (!fileHandler.LoadDataAsFixedSpritesheet(backgroundInfo.fileName, new Vector2(importInfo.Width, importInfo.Height), out Sprite[] spritesheet, options))
        {
            ModLog.Error($"Failed to load {backgroundInfo.name} from {backgroundInfo.fileName}");
            animationInfo = null;
            return false;
        }

        animationInfo = new AnimationInfo(spritesheet, importInfo.SecondsPerFrame);
        return true;
    }

    internal bool TryImportSprite(
        FileHandler fileHandler,
        SpriteImportInfo importInfo,
        out Sprite sprite)
    {
        var options = new SpriteImportOptions()
        {
            Pivot = new Vector2(importInfo.Pivot.X, importInfo.Pivot.Y),
            PixelsPerUnit = importInfo.PixelsPerUnit,
        };

        if (!fileHandler.LoadDataAsSprite(backgroundInfo.fileName, out sprite, options))
        {
            ModLog.Error($"Failed to load sprite at `{backgroundInfo.fileName}`!");
            return false;
        }

        return true;
    }
}
