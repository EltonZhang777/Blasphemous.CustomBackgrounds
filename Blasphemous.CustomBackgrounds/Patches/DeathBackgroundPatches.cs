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
/// For main game, show mod death background if any is activated
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
        List<DeathBackground> activeBackgrounds = BackgroundRegister.DeathBackgrounds.SelectUnlocked(true).ToList().Where(x => Core.Events.GetFlag(x.info.activeFlag)).ToList();
        if (activeBackgrounds.Count == 0)
        {
            // execute vanilla IEnumerator coroutine
            yield return result;
            yield break;
        }

        if (activeBackgrounds.Count > 1)
        {
            ModLog.Warn($"More than one DeathBackgrounds are active simultaneously!");
        }
        if (activeBackgrounds.Any(x => x.info.disablesVanillaCounterpart == true))
        {
            PatchController.SetVanillaCounterpartActive<DeathBackground>(false);
        }
        activeBackgrounds.ForEach(x => x.SetActive(true));

        // execute vanilla IEnumerator coroutine
        yield return result;

        // Postfix
        yield return new WaitForSeconds(1.5f);
        activeBackgrounds.ForEach(x => x.SetActive(false));
        PatchController.SetVanillaCounterpartActive<DeathBackground>(true);
    }
}

/// <summary>
/// For mini-game, show mod death background if any is activated
/// </summary>
[HarmonyPatch(typeof(DeadScreenWidget), "OnDeadDemake")]
class DeadScreenWidget_OnDeadDemake_ShowModDeathBackground_Patch
{
    [HarmonyPostfix]
    public static IEnumerator Postfix(
        IEnumerator result,
        DeadScreenWidget __instance)
    {
        // Prefix
        List<ArcadeDeathBackground> activeBackgrounds = BackgroundRegister.ArcadeDeathBackgrounds.SelectUnlocked(true).ToList().Where(x => Core.Events.GetFlag(x.info.activeFlag)).ToList();
        if (activeBackgrounds.Count == 0)
        {
            // execute vanilla IEnumerator coroutine
            yield return result;
            yield break;
        }

        if (activeBackgrounds.Count > 1)
        {
            ModLog.Warn($"More than one DeathBackgrounds are active simultaneously!");
        }
        if (activeBackgrounds.Any(x => x.info.disablesVanillaCounterpart == true))
        {
            PatchController.SetVanillaCounterpartActive<ArcadeDeathBackground>(false);
        }
        activeBackgrounds.ForEach(x => x.SetActive(true));

        // execute vanilla IEnumerator coroutine
        yield return result;

        // Postfix
        yield return new WaitForSeconds(1.5f);
        activeBackgrounds.ForEach(x => x.SetActive(false));
        PatchController.SetVanillaCounterpartActive<ArcadeDeathBackground>(true);
    }
}