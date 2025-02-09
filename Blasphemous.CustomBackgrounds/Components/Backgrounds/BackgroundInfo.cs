using Blasphemous.CustomBackgrounds.Components.Animations;
using Blasphemous.CustomBackgrounds.Components.Sprites;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Blasphemous.CustomBackgrounds.Components.Backgrounds;

public class BackgroundInfo
{

    /// <summary>
    /// The display name of the background.
    /// </summary>
    public string name;

    /// <summary>
    /// File name of the sprite picture in `data` folder
    /// </summary>
    public string fileName;

    /// <summary>
    /// Background will be granted when this flag turns true
    /// </summary>
    public string acquisitionFlag;

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
    public AcquisitionType acquisitionType;

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
        /// Extend image size to fit screen size
        /// </summary>
        FitScreen,

        /// <summary>
        /// Keep image original resolution and imported pixel per unit setting
        /// </summary>
        KeepOriginalResolution,

        /// <summary>
        /// Only stretch image horizontally
        /// </summary>
        FitHorizontal,

        /// <summary>
        /// Only stretch image vertically
        /// </summary>
        FitVertical,
    }
}
