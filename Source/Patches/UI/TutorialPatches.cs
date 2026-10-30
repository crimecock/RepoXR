using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;
using RepoXR.Assets;
using RepoXR.Player.Camera;
using TMPro;
using UnityEngine;
using static HarmonyLib.AccessTools;

namespace RepoXR.Patches.UI;

[RepoXRPatch]
internal static class TutorialPatches
{
    private static TMP_SpriteAsset originalEmojis;

    [HarmonyPatch(typeof(TutorialUI), nameof(TutorialUI.Start))]
    [HarmonyPostfix]
    private static void OnTutorialStart(TutorialUI __instance)
    {
        // Copy a reference to the original sprite asset
        originalEmojis = __instance.Text.spriteAsset;

        // Update the dummy text to use our input icons
        __instance.dummyText.spriteAsset = AssetCollection.TMPInputsSpriteAsset;
    }
    
    /// <summary>
    /// Update the tutorial UI to use our input icons
    /// </summary>
    [HarmonyPatch(typeof(TutorialUI), nameof(TutorialUI.SetPage))]
    [HarmonyPrefix]
    private static void UpdateTextSpriteAtlas(TutorialUI __instance, ref string dummyTextString, bool transition)
    {
        __instance.Text.spriteAsset = transition ? originalEmojis : AssetCollection.TMPInputsSpriteAsset;

        dummyTextString = dummyTextString.Replace("keyboard", "controller");
    }

    /// <summary>
    /// Update the tips UI to use our input icons
    /// </summary>
    [HarmonyPatch(typeof(TutorialUI), nameof(TutorialUI.SetTipPage))]
    [HarmonyPrefix]
    private static void UpdateTipTextSpriteAtlas(TutorialUI __instance, ref string text)
    {
        __instance.Text.spriteAsset = AssetCollection.TMPInputsSpriteAsset;
        
        text = text.Replace("keyboard", "controller");
    }

    /// <summary>
    /// Make sure the sprite asset is reverted back to ours after the "good job" message
    /// </summary>
    [HarmonyPatch(typeof(TutorialUI), nameof(TutorialUI.SwitchPage), MethodType.Enumerator)]
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction> SwitchPagePatch(IEnumerable<CodeInstruction> instructions)
    {
        // In release: instance is `ldloc.1`
        // In debug: instance is `this.<>4__this`

        CodeInstruction[] ldThis = Plugin.GameDebugBuild
            ?
            [
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Ldfld,
                    Field(Inner(typeof(TutorialUI), "<SwitchPage>d__34"), "<>4__this"))
            ]
            : [new CodeInstruction(OpCodes.Ldloc_1)];

        return new CodeMatcher(instructions)
            .MatchForward(false,
                new CodeMatch(OpCodes.Callvirt, PropertySetter(typeof(TMP_Text), nameof(TMP_Text.text))))
            .InsertAndAdvance([
                ..ldThis,
                new CodeInstruction(OpCodes.Call, ((Action<TutorialUI>)SetSpriteAtlas).Method)
            ])
            .InstructionEnumeration();

        static void SetSpriteAtlas(TutorialUI ui)
        {
            ui.Text.spriteAsset = AssetCollection.TMPInputsSpriteAsset;
        }
    }

    /// <summary>
    /// Replace some of the tutorial text to make more sense in VR
    /// </summary>
    [HarmonyPatch(typeof(TutorialDirector), nameof(TutorialDirector.Awake))]
    [HarmonyPostfix]
    private static void OnTutorialDirectorCreate(TutorialDirector __instance)
    {
        // RepoXR's VR texts exist in English only: in any other language the game's own (translated) texts stay, which
        // also show the VR controller icons
        var locale = UnityEngine.Localization.Settings.LocalizationSettings.SelectedLocale;
        if (locale && !locale.Identifier.Code.StartsWith("en", StringComparison.OrdinalIgnoreCase))
            return;

        // Replace some tutorial text with VR equivalents (the pages are checked: their indices are those of 0.4.4)
        var inventoryFill = Page(__instance, 10, "inventory");
        var inventoryEmpty = Page(__instance, 11, "inventory");
        var map = Page(__instance, 12, "map");
        var expressions = Page(__instance, 25, "express");

        Replace(inventoryFill, "TUTORIAL.INVENTORY_FILL.TEXT", "TUTORIAL.INVENTORY_FILL.DUMMY_TEXT");
        Replace(inventoryEmpty, "TUTORIAL.INVENTORY_EMPTY.TEXT", "TUTORIAL.INVENTORY_EMPTY.DUMMY_TEXT");
        Replace(map, "TUTORIAL.MAP.TEXT", "TUTORIAL.MAP.DUMMY_TEXT");
        Replace(expressions, "TUTORIAL.EXPRESSIONS.TEXT", null);
    }

    private static TutorialDirector.TutorialPage? Page(TutorialDirector director, int index, string word)
    {
        if (index >= director.tutorialPages.Count)
            return null;

        var page = director.tutorialPages[index];
        var text = (page.localizedText ? page.localizedText.GetLocalizedString(null) : null) ?? page.text ?? "";
        if (text.IndexOf(word, StringComparison.OrdinalIgnoreCase) >= 0 ||
            (page.text ?? "").IndexOf(word, StringComparison.OrdinalIgnoreCase) >= 0 ||
            (page.pageName ?? "").IndexOf(word, StringComparison.OrdinalIgnoreCase) >= 0)
            return page;

        Logger.LogWarning($"[Tutorial] page {index} ('{page.pageName}') is not about '{word}' any more: its text stays");
        return null;
    }

    private static void Replace(TutorialDirector.TutorialPage? page, string textKey, string? dummyKey)
    {
        if (page == null)
            return;

        // Through RepoXR's translations when they load (the Quest); on PC 0.5.0's Addressables cannot read RepoXR's
        // JSON catalog (see AssetCollection.LoadAssets), and the same English texts are built in
        if (AssetCollection.TranslationsLoaded)
        {
            page.localizedText = Utils.L(textKey) ?? page.localizedText;
            if (dummyKey != null)
                page.localizedDummyText = Utils.L(dummyKey) ?? page.localizedDummyText;
            return;
        }

        if (BuiltInTexts.TryGetValue(textKey, out var text))
        {
            page.localizedText = null!;
            page.text = text;
        }

        if (dummyKey != null && BuiltInTexts.TryGetValue(dummyKey, out var dummy))
        {
            page.localizedDummyText = null!;
            page.dummyText = dummy;
        }
    }

    /// <summary>
    /// RepoXR's English VR tutorial texts (its string table "HUD RepoXR", en-US)
    /// </summary>
    private static readonly Dictionary<string, string> BuiltInTexts = new()
    {
        ["TUTORIAL.INVENTORY_FILL.TEXT"] = "Grab an Item and deposit it in one of the slots in front of you to put it in your inventory.",
        ["TUTORIAL.INVENTORY_FILL.DUMMY_TEXT"] = "Grab ([grab]) an item, hold it and deposit it in one of the three slots in front of you. Fill all spots in your inventory to complete the progress bar!",
        ["TUTORIAL.INVENTORY_EMPTY.TEXT"] = "Use [grab] with your right hand to take an item out of your inventory.",
        ["TUTORIAL.INVENTORY_EMPTY.DUMMY_TEXT"] = "Use [grab] while holding your right hand over an item in your inventory, and empty all spots to fill the progress bar!",
        ["TUTORIAL.MAP.TEXT"] = "Use [MapGrabLeft] or [MapGrabRight] to grab and view the map. The map is <b><u>important</b></u> for navigation.",
        ["TUTORIAL.MAP.DUMMY_TEXT"] = "Use your left ([MapGrabLeft]) or right ([MapGrabRight]) hand and press the respective button to pick up and view the map until you have filled the progress bar!",
        ["TUTORIAL.EXPRESSIONS.TEXT"] = "Press and hold the [chat] button open the expressions wheel. You can even mix!"
    };

    /// <summary>
    /// Use a less impactful force rotate to look at the truck screen
    /// </summary>
    [HarmonyPatch(typeof(TutorialTruckTrigger), nameof(TutorialTruckTrigger.Update))]
    [HarmonyPostfix]
    private static void TruckForceRotate(TutorialTruckTrigger __instance)
    {
        if (__instance.lockLookTimer <= 0)
            return;

        VRCameraAim.instance.SetAimTarget(__instance.lookTarget.position + Vector3.down, 0.1f, 5, __instance.gameObject,
            90, true);
    }

    /// <summary>
    /// Make the spectate head prompt UI have the correct control sprites
    /// </summary>
    [HarmonyPatch(typeof(SpectateHeadUI), nameof(SpectateHeadUI.Awake))]
    [HarmonyPostfix]
    private static void OnSpectateHeadUICreate(SpectateHeadUI __instance)
    {
        __instance.promptText.spriteAsset = AssetCollection.TMPInputsSpriteAsset;
    }
}