using Blasphemous.ModdingAPI;
using System.Collections.Generic;
using System.Linq;

namespace Blasphemous.CustomBackgrounds.Components.Backgrounds;

/// <summary>
/// Register handler for new backgrounds
/// </summary>
public static class BackgroundRegister
{
    private static readonly List<BaseBackground> _backgrounds = new();
    internal static IEnumerable<BaseBackground> Backgrounds => _backgrounds;
    internal static IEnumerable<MainMenuBackground> MainMenuBackgrounds => _backgrounds.OfType<MainMenuBackground>();
    internal static IEnumerable<DeathBackground> DeathBackgrounds => _backgrounds.OfType<DeathBackground>();
    internal static IEnumerable<LoadingBackground> LoadingBackgrounds => _backgrounds.OfType<LoadingBackground>();
    internal static IEnumerable<VictoryBackground> VictoryBackgrounds => _backgrounds.OfType<VictoryBackground>();
    internal static IEnumerable<ArcadeDeathBackground> ArcadeDeathBackgrounds => _backgrounds.OfType<ArcadeDeathBackground>();
    internal static IEnumerable<ArcadeLoadingBackground> ArcadeLoadingBackgrounds => _backgrounds.OfType<ArcadeLoadingBackground>();
    internal static IEnumerable<ArcadeIntroBackground> ArcadeIntroBackgrounds => _backgrounds.OfType<ArcadeIntroBackground>();

    internal static int Total => _backgrounds.Count;

    internal static BaseBackground AtIndex(int index) => _backgrounds[index];

    internal static BaseBackground AtName(string name)
    {
        BaseBackground result = Exists(name)
            ? _backgrounds.First(x => x.info.name == name)
            : null;
        if (result == null)
        {
            ModLog.Error($"Failed to access nonexistent background of name `{name}`");
        }
        return result;
    }

    internal static bool Exists(string name) => _backgrounds.Any(x => x.info.name == name);
    internal static bool Exists(string name, bool unlocked) => _backgrounds.Any(x => (x.info.name == name) && (x.isUnlocked == unlocked));
    internal static bool Exists<T>(string name) where T : BaseBackground => _backgrounds.OfType<T>().Any(x => x.info.name == name);
    internal static bool Exists<T>(string name, bool unlocked) where T : BaseBackground => _backgrounds.OfType<T>().Any(x => (x.info.name == name) && (x.isUnlocked == unlocked));
    internal static IEnumerable<T> OfType<T>(this IEnumerable<BaseBackground> collection) where T : BaseBackground
    {
        return collection.Where(x => x is T)?.Select(x => x as T);
    }

    internal static IEnumerable<T> SelectUnlocked<T>(this IEnumerable<T> collection, bool unlocked) where T : BaseBackground
    {
        return collection.Where(x => x.isUnlocked == unlocked);
    }

    /// <summary>
    /// Registers a new background 
    /// </summary>
    public static void RegisterBackground(
        this ModServiceProvider provider,
        BaseBackground background)
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

