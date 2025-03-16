using Blasphemous.CustomBackgrounds.Components.Backgrounds;
using Blasphemous.ModdingAPI;
using Framework.Managers;
using Gameplay.UI.Others.Buttons;
using HarmonyLib;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Blasphemous.CustomBackgrounds.Patches;

/// <summary>
/// Show mod death background if any is activated
/// </summary>
[HarmonyPatch(typeof(DeadScreenWidget), "OnDeadAction")]
class DeadScreenWidget_OnDeadAction_ShowModDeathBackground_Patch
{
    [HarmonyPostfix]
    public static IEnumerator Postfix(
        IEnumerator result,
        DeadScreenWidget __instance)
    {
        // Prefix
        List<DeathBackground> activeBackgrounds = BackgroundRegister.DeathBackgrounds.SelectUnlocked(true).ToList().Where(x => Core.Events.GetFlag(x.activeFlag)).ToList();
        if (activeBackgrounds.Count == 0)
            yield break;

        if (activeBackgrounds.Count > 1)
        {
            ModLog.Warn($"More than one DeathBackgrounds are active simultaneously!");
        }
        PatchController.SetVanillaCounterpartActive<DeathBackground>(false);
        activeBackgrounds.ForEach(x => x.SetActive(true));

        // original IEnumerator coroutine
        yield return result;

        // Postfix
        yield return new WaitForSeconds(1.5f);
        activeBackgrounds.ForEach(x => x.SetActive(false));
        PatchController.SetVanillaCounterpartActive<DeathBackground>(true);
    }
}