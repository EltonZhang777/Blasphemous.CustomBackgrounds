using Newtonsoft.Json;
using System.Collections.Generic;

namespace Blasphemous.CustomBackgrounds;

/// <summary>
/// Contains data that should be saved across game sessions.
/// </summary>
public class BackgroundSaveData
{
    /// <summary>
    /// Currently displayed mod background's name.
    /// </summary>
    public string currentModBackground = "";

    /// <summary>
    /// List of unlocked mod backgrounds' names.
    /// </summary>
    public List<string> unlockedBackgrounds = new();

    [JsonIgnore]
    internal bool currentIsModBackground => !string.IsNullOrEmpty(currentModBackground);
}
