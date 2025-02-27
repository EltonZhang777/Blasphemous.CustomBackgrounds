using Blasphemous.ModdingAPI;
using System.Collections.Generic;
using System.Linq;

namespace Blasphemous.CustomBackgrounds.Components.Backgrounds;

/// <summary>
/// Register handler for new backgrounds
/// </summary>
public static class BackgroundRegister
{

    private static readonly List<Background> _backgrounds = new();
    internal static IEnumerable<Background> Backgrounds => _backgrounds;
    internal static Background AtIndex(int index) => _backgrounds[index];
    internal static Background AtName(string name)
    {
        Background result = Exists(name)
            ? _backgrounds.First(x => x.info.name == name)
            : null;
        if (result == null)
        {
            ModLog.Error($"Failed to access nonexistent background of name `{name}`");
        }
        return result;
    }
    internal static bool Exists(string name) => _backgrounds.Any(x => x.info.name == name);
    internal static int Total => _backgrounds.Count;

    /// <summary>
    /// Registers a new background 
    /// </summary>
    public static void RegisterBackground(
        this ModServiceProvider provider,
        Background background)
    {
        if (provider == null)
            return;

        // prevents repeated registering
        if (_backgrounds.Any(x => x.info.name == background.info.name))
            return;

        background.parentModId = provider.RegisteringMod.Id;
        _backgrounds.Add(background);
        ModLog.Info($"Registered custom Background: {background.info.name}");
    }
}

