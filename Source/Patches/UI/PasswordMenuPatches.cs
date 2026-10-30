using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;
using RepoXR.UI;
using UnityEngine;
using static HarmonyLib.AccessTools;

namespace RepoXR.Patches.UI;

[RepoXRPatch]
internal static class PasswordMenuPatches
{
    /// <summary>
    /// Make the password menu work in VR
    /// </summary>
    [HarmonyPatch(typeof(MenuPagePassword), nameof(MenuPagePassword.Start))]
    [HarmonyPostfix]
    private static void OnMenuPagePasswordShown(MenuPagePassword __instance)
    {
        __instance.gameObject.AddComponent<PasswordUI>();
    }

    /// <summary>
    /// Detect when the password has been submitted
    /// </summary>
    [HarmonyPatch(typeof(MenuPagePassword), nameof(MenuPagePassword.ConfirmButton))]
    [HarmonyPostfix]
    private static void OnMenuPagePasswordConfirm(MenuPagePassword __instance)
    {
        __instance.GetComponent<PasswordUI>().OnConfirm();
    }

    // R.E.P.O. 0.5.0: the password page has no PasswordTextSet of its own any more, its field is a MenuTextInput, whose
    // cursor MenuTextInputPatches.TextCursorPatch fixes
}
