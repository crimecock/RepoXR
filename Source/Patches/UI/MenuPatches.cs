using HarmonyLib;
using RepoXR.Assets;
using RepoXR.Managers;
using RepoXR.UI;
using RepoXR.UI.Menu;
using UnityEngine;
using UnityEngine.UI;

namespace RepoXR.Patches.UI;

[RepoXRPatch(RepoXRPatchTarget.Universal)]
internal static class MenuPatches
{
    /// <summary>
    /// Register custom menus when the menu managers boots up
    /// </summary>
    [HarmonyPatch(typeof(MenuManager), nameof(MenuManager.Awake))]
    [HarmonyPrefix]
    private static void OnMenuManagerStart(MenuManager __instance)
    {
        if (MenuManager.instance)
            return;
        
        MenuHelper.RegisterCustomMenus(__instance);
    }
    
    /// <summary>
    /// Insert VR settings into the settings menu
    /// </summary>
    [HarmonyPatch(typeof(MenuPageSettings), nameof(MenuPageSettings.Start))]
    [HarmonyPostfix]
    private static void AddVRSettingsPatch(MenuPageSettings __instance)
    {
        // On PC, only add the VR settings menu on the main menu
        if (!SemiFunc.MenuLevel() && !VRSession.InVR)
            return;
        
        GameFonts.FixBundlePrefabs();

        // Quest port: R.E.P.O. 0.5.0's settings page has six tabs (Gameplay 260 ... Gamepad 120, Language 85, 35 apart),
        // and the VR Settings button at (126, 83) sat right on top of Language. It goes below Language, Back below it
        var vrButton = Object.Instantiate(AssetCollection.VRSettingsButton, __instance.transform)
            .GetComponent<Button>();
        var vrRect = vrButton.GetComponent<RectTransform>();
        var language = __instance.transform.Find("Menu Button - Language") as RectTransform;

        // Brute forcing element positions
        vrRect.anchoredPosition = language ? language.anchoredPosition - new Vector2(0, 35) : new Vector2(126, 83);
        vrButton.onClick.AddListener(() =>
        {
            __instance.ButtonEventBack();
            MenuHelper.PageOpenOnTop(MenuHelper.RepoXRMenuPage.VRSettings);
        });

        // Move back button a bit down
        __instance.transform.Find("Menu Button - Back").GetComponent<RectTransform>().anchoredPosition =
            language ? vrRect.anchoredPosition - new Vector2(0, 45) : new Vector2(126, 35);
    }
}