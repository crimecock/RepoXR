using HarmonyLib;
using RepoXR.Assets;

namespace RepoXR.Patches.UI;

[RepoXRPatch]
internal static class ResultScreenPatches
{
    /// <summary>
    /// Reset the loading screen position and fix skip font on the first frame of the result screen
    /// </summary>
    [HarmonyPatch(typeof(ResultScreenUI), nameof(ResultScreenUI.StateFadeIn))]
    [HarmonyPrefix]
    private static void InitializeScreenPatch(ResultScreenUI __instance)
    {
        if (!__instance.stateImpulse) return;

        // An exception here skips StateFadeIn on every frame and the game never leaves the result screen (level → shop
        // on 0.5.0, when the scene handoff had cleared LoadingUI.instance), so nothing here may throw
        if (__instance.skipText)
            __instance.skipText.spriteAsset = AssetCollection.TMPInputsSpriteAsset;

        if (RepoXR.UI.LoadingUI.instance)
            RepoXR.UI.LoadingUI.instance.ResetPosition();
        else if (!loadingUIMissingLogged)
        {
            loadingUIMissingLogged = true;
            Logger.LogWarning("[Scene] result screen: no VR loading UI to reposition");
        }
    }

    private static bool loadingUIMissingLogged;

    /// <summary>
    /// Disable player movement and rotation while the result animation plays (fixes some particle system funkyness)
    /// </summary>
    [HarmonyPatch(typeof(ResultScreenUI), nameof(ResultScreenUI.Update))]
    [HarmonyPostfix]
    private static void DisableTurningPatch(ResultScreenUI __instance)
    {
        if (__instance.fadeLerp < 1 || !PlayerController.instance) return;

        PlayerController.instance.InputDisable(0.1f);
    }
}