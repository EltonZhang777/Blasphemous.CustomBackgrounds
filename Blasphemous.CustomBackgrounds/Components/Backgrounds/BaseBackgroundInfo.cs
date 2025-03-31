using Blasphemous.CustomBackgrounds.Components.Animations;
using Blasphemous.CustomBackgrounds.Components.Sprites;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Collections.Generic;

namespace Blasphemous.CustomBackgrounds.Components.Backgrounds;

/// <summary>
/// Serializable class containing information of background import settings
/// </summary>
public class BaseBackgroundInfo
{
    /// <summary>
    /// The code name of the background.
    /// </summary>
    public string name;

    /// <summary>
    /// The color of the name text when displayed in pop-up or selection tab, in HTML color string format `#RRGGBB`
    /// </summary>
    public string textColor = "#9C4E4E";

    /// <summary>
    /// File name of the sprite picture in `data` folder
    /// </summary>
    public string fileName;

    /// <summary>
    /// Contains localization strings
    /// </summary>
    public Dictionary<string, string> localization;

    /// <summary>
    /// Whether this mod background is shown in front of its vanilla counterpart (i.e. blocks vanilla's visuals)
    /// </summary>
    public bool blocksVanillaCounterpart = true;

    /// <summary>
    /// Background will be granted when this flag turns true
    /// </summary>
    public string acquisitionFlag = "";

    /// <summary>
    /// Conditionally-displayed background will be activated when this flag is true
    /// </summary>
    public string activeFlag = "";

    /// <summary>
    /// See <see cref="SpriteType"/>
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public SpriteType spriteType;

    /// <summary>
    /// See <see cref="FitType"/>
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public FitType fitType;

    /// <summary>
    /// See <see cref="AcquisitionType"/>
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public AcquisitionType acquisitionType = AcquisitionType.OnInitialize;

    /// <summary>
    /// Information for importing static sprite
    /// </summary>
    public SpriteImportInfo spriteImportInfo = null;

    /// <summary>
    /// Information for importing animated sprites
    /// </summary>
    public AnimationImportInfo animationImportInfo = null;

    /// <summary>
    /// Enum object describing when the background should be acquired
    /// </summary>
    public enum AcquisitionType
    {
        /// <summary>
        /// Patched on game start
        /// </summary>
        OnInitialize,

        /// <summary>
        /// Patched when a certain flag's value is `true`
        /// </summary>
        OnFlag
    }

    /// <summary>
    /// Type of sprite for the background
    /// </summary>
    public enum SpriteType
    {
        /// <summary>
        /// Static image background
        /// </summary>
        Static,

        /// <summary>
        /// Animated background
        /// </summary>
        Animated
    }

    /// <summary>
    /// How the imported image is fitted to screen size
    /// </summary>
    public enum FitType
    {
        /// <summary>
        /// Stretch image to fit screen ratio and size
        /// </summary>
        FitScreenRatio,

        /// <summary>
        /// Keep image original ratio, scale it down to fit screen.
        /// </summary>
        KeepRatioFitScreen,

        /// <summary>
        /// Keep image original ratio, scale it up to fill screen.
        /// </summary>
        KeepRatioFillScreen,

        /// <summary>
        /// Keep image original resolution and ratio (does not scale). 
        /// </summary>
        KeepOriginalResolution
    }
}
