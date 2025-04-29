using Blasphemous.CustomBackgrounds.Components.Animations;
using Blasphemous.CustomBackgrounds.Components.Sprites;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Blasphemous.CustomBackgrounds.Components.Map;

/// <summary>
/// Serializable class containing information of mod map mark import settings
/// </summary>
public class ModMapMarkInfo
{
    /// <summary>
    /// The id of the map mark.
    /// </summary>
    public string id;

    /// <summary>
    /// The id of the mod that registered the map mark
    /// </summary>
    [JsonIgnore]
    internal string registeringModId;

    /// <summary>
    /// File name of the sprite picture in `data` folder
    /// </summary>
    public string fileName;

    /// <summary>
    /// See <see cref="SpriteType"/>
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public SpriteType spriteType;

    /// <summary>
    /// Information for importing static sprite
    /// </summary>
    public SpriteImportInfo spriteImportInfo = null;

    /// <summary>
    /// Information for importing animated sprites
    /// </summary>
    public AnimationImportInfo animationImportInfo = null;

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
}

