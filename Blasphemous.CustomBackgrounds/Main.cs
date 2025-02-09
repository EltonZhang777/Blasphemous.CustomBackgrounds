using BepInEx;
using System;

namespace Blasphemous.CustomBackgrounds;

[BepInPlugin(ModInfo.MOD_ID, ModInfo.MOD_NAME, ModInfo.MOD_VERSION)]
[BepInDependency("Blasphemous.ModdingAPI", "2.4.1")]
public class Main : BaseUnityPlugin
{
    public static CustomBackgrounds CustomBackgrounds { get; private set; }

    private void Start()
    {
        CustomBackgrounds = new CustomBackgrounds();
    }
    public static T Validate<T>(T obj, Func<T, bool> validate)
    {
        return validate(obj)
            ? obj
            : throw new Exception($"{obj} is an invalid import argument");
    }
}
