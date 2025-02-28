using Blasphemous.CustomBackgrounds.Components.Animations;
using Blasphemous.CustomBackgrounds.Components.Sprites;
using Blasphemous.CustomBackgrounds.Extensions;
using Blasphemous.CustomBackgrounds.Patches;
using Blasphemous.ModdingAPI;
using Blasphemous.ModdingAPI.Files;
using Framework.Managers;
using Gameplay.UI;
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

    internal BackgroundInfo info;
    internal bool isUnlocked = false;
    private GameObject _gameObj;
    private Vector2 _spriteSize;
    private readonly AnimationInfo _animationInfo;
    private readonly Sprite _sprite;
    private static readonly float UI_WIDTH = 640f;
    private static readonly float UI_HEIGHT = 360f;

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
            if (info.localization.ContainsKey(currentLanguage) && !string.IsNullOrEmpty(info.localization[currentLanguage]))
            {
                return info.localization[currentLanguage];
            }

            // The language doesn't exist
            ModLog.Warn($"Localization string of {currentLanguage} for {info.name} doesn't exist!");
            if (info.localization.ContainsKey("en"))
            {
                // use English if it exists
                return info.localization["en"];
            }
            else if (info.localization.Count > 0)
            {
                // use first language with a non-null localization string if it exists
                return info.localization.Values.First(x => !string.IsNullOrEmpty(x));
            }

            ModLog.Error($"Failed to localize background `{info.name}` to any language!");
            return "#LOC_ERROR";
        }
    }
    internal string ColoredLocalizedName => Main.ColorString(LocalizedName, info.textColor);

    /// <summary>
    /// Constructor for custom background object
    /// </summary>
    /// <param name="fileHandler">The registering mod's FileHandler</param>
    /// <param name="backgroundInfo">Deserialized info object</param>
    public Background(
        FileHandler fileHandler,
        BackgroundInfo backgroundInfo)
    {
        this.info = backgroundInfo;
        switch (backgroundInfo.spriteType)
        {
            case BackgroundInfo.SpriteType.Static:
                if (!TryImportSprite(fileHandler, backgroundInfo.spriteImportInfo, out _sprite))
                {
                    throw new ArgumentException($"Failed loading static background `{backgroundInfo.name}`!");
                }
                _spriteSize = _sprite.rect.size;
                break;
            case BackgroundInfo.SpriteType.Animated:
                if (!TryImportAnimation(fileHandler, backgroundInfo.animationImportInfo, out _animationInfo))
                {
                    throw new ArgumentException($"Failed loading animated background `{backgroundInfo.name}`!");
                }
                _spriteSize = new Vector2(backgroundInfo.animationImportInfo.Width, backgroundInfo.animationImportInfo.Height);
                break;
        }

        if (backgroundInfo.acquisitionType == BackgroundInfo.AcquisitionType.OnFlag)
        {
            if (string.IsNullOrEmpty(backgroundInfo.acquisitionFlag))
            {
                throw new ArgumentException($"Failed initializing background `{backgroundInfo.name}`: no flag designated for flag-acquired background!");
            }
            else
            {
                Main.CustomBackgrounds.EventHandler.OnFlagChange += OnFlagChange;
            }
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
        _gameObj = new GameObject($"Background[{info.name}]");
        _gameObj.transform.position = new Vector3(0f, 0f, 99f);
        _gameObj.layer = LayerMask.NameToLayer("UI");

        // set RectTransform
        RectTransform rectTransform = _gameObj.AddComponent<RectTransform>();
        rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
        rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
        rectTransform.pivot = new Vector2(0, 0);

        // set Image component
        Image image = _gameObj.AddComponent<Image>();
        image.raycastTarget = true;
        image.preserveAspect = true;
        image.type = Image.Type.Simple;

        // add sprite/animation to Image
        switch (info.spriteType)
        {
            case BackgroundInfo.SpriteType.Static:
                image.sprite = _sprite;
                break;
            case BackgroundInfo.SpriteType.Animated:
                ModImageAnimator anim = _gameObj.AddComponent<ModImageAnimator>();
                anim.Animation = _animationInfo;
                break;
            default:
                throw new NotImplementedException();
        }

        // configure RectTransform to assigned FitType
        switch (info.fitType)
        {
            case BackgroundInfo.FitType.FitScreenRatio:
                image.preserveAspect = false;
                rectTransform.sizeDelta = new Vector2(UI_WIDTH, UI_HEIGHT);
                break;
            case BackgroundInfo.FitType.KeepRatioFillScreen:
                image.preserveAspect = true;
                rectTransform.sizeDelta = (_spriteSize.x / _spriteSize.y) > (UI_WIDTH / UI_HEIGHT)
                    ? _spriteSize / (_spriteSize.y / UI_HEIGHT)
                    : _spriteSize / (_spriteSize.x / UI_WIDTH);
                break;
            case BackgroundInfo.FitType.KeepRatioFitScreen:
                image.preserveAspect = true;
                rectTransform.sizeDelta = (_spriteSize.x / _spriteSize.y) > (UI_WIDTH / UI_HEIGHT)
                    ? _spriteSize / (_spriteSize.x / UI_WIDTH)
                    : _spriteSize / (_spriteSize.y / UI_HEIGHT);
                break;
            default:
                throw new NotImplementedException();
        }
        rectTransform.localPosition = rectTransform.sizeDelta / -2f;
        _gameObj.SetActive(false);
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

        if (!fileHandler.LoadDataAsFixedSpritesheet(info.fileName, new Vector2(importInfo.Width, importInfo.Height), out Sprite[] spritesheet, options))
        {
            ModLog.Error($"Failed to load {info.name} from {info.fileName}");
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

        if (!fileHandler.LoadDataAsSprite(info.fileName, out sprite, options))
        {
            ModLog.Error($"Failed to load sprite at `{info.fileName}`!");
            return false;
        }

        return true;
    }

    /// <summary>
    /// Set background unlocked state. If unlocking from locked state, trigger a pop-up window.
    /// </summary>
    public void SetUnlocked(bool unlocked, bool showPopUp = true)
    {
        if ((isUnlocked == false) && (unlocked == true))
        {
            if (showPopUp)
                ShowUnlockPopUp();
        }
        isUnlocked = unlocked;
    }

    internal void ShowUnlockPopUp()
    {
        PatchController.unlockPopupBackgroundName = info.name;
        UIController.instance.ShowUnlockPopup(PatchController.VANILLA_POPUP_ID);
        PatchController.unlockPopupBackgroundName = "";
    }

    /// <summary>
    /// Unlock the background when the corresponding flag is set to true
    /// </summary>
    protected void OnFlagChange(string flagId)
    {
        if (flagId != info.acquisitionFlag)
            return;
        if (Core.Events.GetFlag(flagId) == false)
            return;

        SetUnlocked(true);
    }
}
