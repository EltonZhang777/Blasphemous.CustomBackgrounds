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
/// Abstract class of background visual elements.
/// </summary>
public abstract class BaseBackground
{
    /// <summary>
    /// The mod that registered the background.
    /// </summary>
    public string parentModId;

    protected GameObject gameObj;
    protected Vector2 spriteSize;
    protected readonly AnimationInfo animationInfo;
    protected readonly Sprite sprite;
    protected static readonly float UI_WIDTH = 640f;
    protected static readonly float UI_HEIGHT = 360f;
    internal bool isUnlocked = false;
    internal readonly BaseBackgroundInfo info;

    /// <summary>
    /// Whether a pop-up should be shown when unlocking this background
    /// </summary>
    protected abstract bool ShouldShowPopup { get; }

    /// <summary>
    /// Accessor of <see cref="gameObj"/> that auto-initializes it when it's null (newly-initialized GameObj is disabled by default)
    /// </summary>
    protected internal GameObject GameObj
    {
        get
        {
            if (gameObj == null)
            {
                InitializeGameObject();
            }
            return gameObj;
        }
        set
        {
            gameObj = value;
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
    /// Basic constructor for custom background object
    /// </summary>
    /// <param name="fileHandler">The registering mod's FileHandler</param>
    /// <param name="backgroundInfo">Deserialized info object</param>
    internal BaseBackground(
        FileHandler fileHandler,
        BaseBackgroundInfo backgroundInfo)
    {
        this.info = backgroundInfo;
        switch (backgroundInfo.spriteType)
        {
            case BaseBackgroundInfo.SpriteType.Static:
                if (!TryImportSprite(fileHandler, backgroundInfo.spriteImportInfo, out sprite))
                {
                    throw new ArgumentException($"Failed loading static background `{backgroundInfo.name}`!");
                }
                spriteSize = sprite.rect.size;
                break;
            case BaseBackgroundInfo.SpriteType.Animated:
                if (!TryImportAnimation(fileHandler, backgroundInfo.animationImportInfo, out animationInfo))
                {
                    throw new ArgumentException($"Failed loading animated background `{backgroundInfo.name}`!");
                }
                spriteSize = new Vector2(backgroundInfo.animationImportInfo.Width, backgroundInfo.animationImportInfo.Height);
                break;
        }

        if (backgroundInfo.acquisitionType == BaseBackgroundInfo.AcquisitionType.OnFlag)
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
    public BaseBackground(
        FileHandler fileHandler,
        string backgroundInfoJsonFileLocation)
        : this(fileHandler, fileHandler.LoadDataAsJson<BaseBackgroundInfo>(backgroundInfoJsonFileLocation))
    { }

    /// <summary>
    /// Initialize the GameObject, then set it to inactive.
    /// </summary>
    internal virtual void InitializeGameObject()
    {
        gameObj = new GameObject($"Background[{info.name}]");
        gameObj.layer = LayerMask.NameToLayer("UI");

        // set RectTransform
        RectTransform rectTransform = gameObj.AddComponent<RectTransform>();
        rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
        rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
        rectTransform.pivot = new Vector2(0.5f, 0.5f);

        // set Image component
        Image image = gameObj.AddComponent<Image>();
        image.raycastTarget = true;
        image.type = Image.Type.Simple;

        // add sprite/animation to Image
        switch (info.spriteType)
        {
            case BaseBackgroundInfo.SpriteType.Static:
                image.sprite = sprite;
                break;
            case BaseBackgroundInfo.SpriteType.Animated:
                ModImageAnimator anim = gameObj.AddComponent<ModImageAnimator>();
                anim.Animation = animationInfo;
                break;
            default:
                throw new NotImplementedException();
        }

        // configure RectTransform to assigned FitType
        switch (info.fitType)
        {
            case BaseBackgroundInfo.FitType.FitScreenRatio:
                image.preserveAspect = false;
                rectTransform.sizeDelta = new Vector2(UI_WIDTH, UI_HEIGHT);
                break;
            case BaseBackgroundInfo.FitType.KeepRatioFillScreen:
                image.preserveAspect = true;
                rectTransform.sizeDelta = (spriteSize.x / spriteSize.y) > (UI_WIDTH / UI_HEIGHT)
                    ? spriteSize / (spriteSize.y / UI_HEIGHT)
                    : spriteSize / (spriteSize.x / UI_WIDTH);
                break;
            case BaseBackgroundInfo.FitType.KeepRatioFitScreen:
                image.preserveAspect = true;
                rectTransform.sizeDelta = (spriteSize.x / spriteSize.y) > (UI_WIDTH / UI_HEIGHT)
                    ? spriteSize / (spriteSize.x / UI_WIDTH)
                    : spriteSize / (spriteSize.y / UI_HEIGHT);
                break;
            case BaseBackgroundInfo.FitType.KeepOriginalResolution:
                image.preserveAspect = true;
                rectTransform.sizeDelta = spriteSize;
                break;
            default:
                throw new NotImplementedException();
        }
        rectTransform.localPosition = Vector3.zero;

        // finalize initialization
        gameObj.SetActive(false);
    }

    /// <summary>
    /// Try importing animation by passing in AnimationImportInfo
    /// </summary>
    protected internal virtual bool TryImportAnimation(
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

    /// <summary>
    /// Try importing sprite by passing in SpriteImportInfo
    /// </summary>
    protected internal virtual bool TryImportSprite(
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
    public virtual void SetUnlocked(bool unlocked, bool showPopUp = true)
    {
        if ((isUnlocked == false) && (unlocked == true))
        {
            if (showPopUp && ShouldShowPopup)
                ShowUnlockPopUp();
        }
        isUnlocked = unlocked;
    }

    /// <summary>
    /// Display the pop-up of unlock information
    /// </summary>
    protected internal virtual void ShowUnlockPopUp()
    {
        PatchController.unlockPopupBackgroundName = info.name;
        UIController.instance.ShowUnlockPopup(PatchController.VANILLA_POPUP_ID);
        PatchController.unlockPopupBackgroundName = "";
    }

    /// <summary>
    /// Properly layer the background's GameObject on Blsphemous UI.
    /// </summary>
    protected internal abstract void SetGameObjectLayer();

    /// <summary>
    /// Activate/Deactivate the background object.
    /// </summary>
    protected internal virtual void SetActive(bool active)
    {
        if (active == true)
        {
            GameObj.SetActive(true);
            SetGameObjectLayer();
        }
        else
        {
            gameObj?.SetActive(false);
        }
    }

    /// <summary>
    /// Unlock the background when the corresponding flag is set to true
    /// </summary>
    protected virtual void OnFlagChange(string flagId)
    {
        if (flagId != info.acquisitionFlag)
            return;
        if (Core.Events.GetFlag(flagId) == false)
            return;

        SetUnlocked(true);
    }
}
