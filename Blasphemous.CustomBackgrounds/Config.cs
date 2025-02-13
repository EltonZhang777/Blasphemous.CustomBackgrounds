using Blasphemous.CustomBackgrounds.Components.Backgrounds;
using Newtonsoft.Json;

namespace Blasphemous.CustomBackgrounds;

/// <summary>
/// Master config for CustomBackgrounds mod
/// </summary>
public class Config
{
    /// <summary>
    /// Only used for saving mod background index at game dispose. Shouldn't be written manually at `.cfg` file
    /// </summary>
    public int savedBackgroundIndex = DEFAULT_BACKGROUND_INDEX;

    [JsonIgnore]
    internal static readonly int DEFAULT_BACKGROUND_INDEX = -9999;

    [JsonIgnore]
    internal bool IsValidBackgroundIndex
    {
        get
        {
            return (savedBackgroundIndex >= 0 && savedBackgroundIndex < 4 + BackgroundRegister.Total);
        }
    }
}
