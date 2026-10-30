using HarmonyLib;
using RepoXR.UI.Menu;

namespace RepoXR.Patches.UI;

[RepoXRPatch]
internal static class MenuPageSavesPatches
{
    /// <summary>
    /// Make sure to close the keyboard after renaming (R.E.P.O. 0.5.0: every text prompt, the save rename among them, is
    /// the shared MenuPagePopUpTextInput)
    /// </summary>
    [HarmonyPatch(typeof(MenuPagePopUpTextInput), nameof(MenuPagePopUpTextInput.ButtonEventConfirm))]
    [HarmonyPostfix]
    private static void OnConfirm()
    {
        if (InputKeyboard.instance is {} instance && instance != null)
            instance.Close();
    }

    /// <summary>
    /// Make sure to close the keyboard after canceling the renaming
    /// </summary>
    [HarmonyPatch(typeof(MenuPagePopUpTextInput), nameof(MenuPagePopUpTextInput.ButtonEventCancel))]
    [HarmonyPostfix]
    private static void OnExitPage()
    {
        if (InputKeyboard.instance is {} instance && instance != null)
            instance.Close();
    }
}