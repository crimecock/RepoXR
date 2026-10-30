using HarmonyLib;
using TMPro;

namespace RepoXR.Patches.UI;

/// <summary>
/// The main menu's news popup (bottom right) grows from nothing (MenuPageMain.RunMarketingPopupCurves) right after its
/// texts are set, so TextMeshPro builds their meshes while the popup is still tiny. In VR its words were drawn as solid
/// bars; once the popup has finished growing its two texts are built again, at their real size
/// </summary>
[RepoXRPatch]
internal static class MarketingPopupPatches
{
    private static MenuPageMain? rebuiltFor;

    [HarmonyPatch(typeof(MenuPageMain), nameof(MenuPageMain.UpdateMarketingMessage))]
    [HarmonyPostfix]
    private static void OnNewMessage() => rebuiltFor = null;

    [HarmonyPatch(typeof(MenuPageMain), nameof(MenuPageMain.RunMarketingPopupCurves))]
    [HarmonyPostfix]
    private static void RebuildWhenGrown(MenuPageMain __instance)
    {
        if (rebuiltFor == __instance || __instance.marketingPopupScaleLerp < 1)
            return;

        rebuiltFor = __instance;
        Rebuild(__instance.marketingTitleText);
        Rebuild(__instance.marketingMessageText);
    }

    private static void Rebuild(TMP_Text? text)
    {
        if (text)
            text!.ForceMeshUpdate(true, true);
    }
}
