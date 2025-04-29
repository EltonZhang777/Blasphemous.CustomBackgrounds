using Newtonsoft.Json;
using System.Collections.Generic;

namespace Blasphemous.CustomBackgrounds.Persistence;

/// <summary>
/// Contains global persistence data that are independent of in-game save slots.
/// </summary>
public class GlobalSaveData
{
    /// <summary>
    /// Currently displayed mod background's name.
    /// </summary>
    public string currentModMainMenuBg = "";

    /// <summary>
    /// List of unlocked mod backgrounds' names.
    /// </summary>
    public List<string> unlockedBackgrounds = new();

    [JsonIgnore]
    internal bool currentIsModMainMenuBg => !string.IsNullOrEmpty(currentModMainMenuBg);
}
