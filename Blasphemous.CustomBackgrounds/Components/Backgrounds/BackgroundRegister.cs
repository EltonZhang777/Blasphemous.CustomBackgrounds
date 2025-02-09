using Blasphemous.ModdingAPI;
using System.Collections.Generic;
using System.Linq;

namespace Blasphemous.CustomBackgrounds.Components.Backgrounds;

/// <summary>
/// Register handler for new language patches
/// </summary>
public static class BackgroundRegister
{

    private static readonly List<Background> _backgrounds = new();
    internal static IEnumerable<Background> Backgrounds => _backgrounds;
    internal static Background AtIndex(int index) => _backgrounds[index];
    internal static int Total => _backgrounds.Count;

    /// <summary>
    /// Registers a new Background 
    /// </summary>
    public static void RegisterBackground(
        this ModServiceProvider provider,
        Background background)
    {
        if (provider == null)
            return;

        // prevents repeated registering
        if (_backgrounds.Any(x => x.backgroundInfo.name == background.backgroundInfo.name))
            return;

        background.parentModId = provider.RegisteringMod.Id;
        _backgrounds.Add(background);
        ModLog.Info($"Registered custom Background: {background.backgroundInfo.name}");
    }
}

