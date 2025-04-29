using Blasphemous.CustomBackgrounds.Components.Map;
using Framework.Map;
using HarmonyLib;

namespace Blasphemous.CustomBackgrounds.Patches;

[HarmonyPatch(typeof(MapRenderer))]
class MapRenderer_ShowModMarks_Patch
{
    [HarmonyPatch("Render")]
    [HarmonyPostfix]
    public static void Postfix_1()
    {
        ModMapManager.ShowAllModMapMarks(true);
    }

    [HarmonyPatch("UpdateMarks")]
    [HarmonyPostfix]
    public static void Postfix_2()
    {
        ModMapManager.ShowAllModMapMarks(true);
    }
}