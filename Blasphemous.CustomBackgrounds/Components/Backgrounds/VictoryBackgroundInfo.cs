using Gameplay.UI;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Blasphemous.CustomBackgrounds.Components.Backgrounds;

/// <inheritdoc/>
public class VictoryBackgroundInfo : BaseBackgroundInfo
{
    /// <summary>
    /// The type of victory (regular boss / arena / final boss)
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public UIController.FullMensages victoryType;
}
