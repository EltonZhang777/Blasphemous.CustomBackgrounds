using Framework.Managers;
using HarmonyLib;

namespace Blasphemous.CustomBackgrounds.Patches;


[HarmonyPatch(typeof(EventManager), "SetFlag")]
internal class EventManager_SetFlag_FlagChangeEvent_Patch
{
    public static void Postfix(string id)
    {
        Main.CustomBackgrounds.EventHandler.FlagChange(id);
    }
}