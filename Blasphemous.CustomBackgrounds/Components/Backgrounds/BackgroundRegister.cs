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
        Background result = null;
        try
        {
            result = _backgrounds.First(x => x.info.name == name);
        }
        catch
        {
            ModLog.Error($"Failed to access nonexistent background of name `{name}`");
        }
        return result;
    }
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

