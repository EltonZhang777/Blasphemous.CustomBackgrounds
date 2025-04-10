using Blasphemous.CustomBackgrounds.Components.Backgrounds;
using Blasphemous.ModdingAPI;
using Framework.Managers;
using Gameplay.UI;
using HarmonyLib;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Blasphemous.CustomBackgrounds.Patches;

[HarmonyPatch(typeof(UIController), "ShowFullMessageCourrutine")]
class UIController_ShowFullMessageCourrutine_ShowModVictoryScreen_Patch
{
    [HarmonyPostfix]
    public static IEnumerator Postfix(
        IEnumerator result,
        UIController.FullMensages message)
    {
        // Prefix
        List<VictoryBackground> activeBackgrounds = BackgroundRegister.VictoryBackgrounds.SelectUnlocked(true).Where(x => Core.Events.GetFlag(x.info.activeFlag) && (x.victoryType == message)).ToList();
        if (activeBackgrounds.Count == 0)
        {
            // execute vanilla IEnumerator coroutine
            yield return result;
            yield break;
        }

        if (activeBackgrounds.Count > 1)
        {
            ModLog.Warn($"More than one VictoryBackgrounds are active simultaneously!");
        }
        if (activeBackgrounds.Any(x => x.info.disablesVanillaCounterpart))
        {
            PatchController.SetVanillaCounterpartActive<VictoryBackground>(false);
        }
        activeBackgrounds.ForEach(x => x.SetActive(true));

        // execute vanilla IEnumerator coroutine
        yield return result;

        // Postfix
        yield return new WaitForEndOfFrame();
        PatchController.SetVanillaCounterpartActive<VictoryBackground>(true);
        activeBackgrounds.ForEach(x => x.SetActive(false));
    }
}