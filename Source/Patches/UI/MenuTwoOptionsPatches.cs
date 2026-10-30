using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using UnityEngine;

using static HarmonyLib.AccessTools;

namespace RepoXR.Patches.UI;

/// <summary>
/// R.E.P.O. 0.5.0's two-option toggles (MenuTwoOptions: On/Off, Head/Hand...) in a world-space VR menu
/// </summary>
[RepoXRPatch]
internal static class MenuTwoOptionsPatches
{
    private static readonly MethodInfo GetPosition = PropertyGetter(typeof(Transform), nameof(Transform.position));

    /// <summary>
    /// OptionBoxX decides which option is on the left by comparing the two buttons' world x. On a screen-space canvas
    /// that is the screen's x; RepoXR's menus hang in the world, turned whichever way the player faced, and when the
    /// canvas faces the other way round the comparison flips: the selection box went to the option that is not set.
    /// The buttons are compared in their canvas' own space instead
    /// </summary>
    [HarmonyPatch(typeof(MenuTwoOptions), nameof(MenuTwoOptions.OptionBoxX))]
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction> OptionBoxInCanvasSpace(IEnumerable<CodeInstruction> instructions)
    {
        var replaced = 0;
        foreach (var instruction in instructions)
        {
            if (instruction.Calls(GetPosition))
            {
                replaced++;
                yield return new CodeInstruction(OpCodes.Call, ((System.Func<Transform, Vector3>)CanvasPosition).Method)
                    .WithLabels(instruction.labels).WithBlocks(instruction.blocks);
                continue;
            }

            yield return instruction;
        }

        if (replaced != 2)
            Logger.LogWarning($"[UI] MenuTwoOptions.OptionBoxX: {replaced} positions in canvas space (expected 2)");
    }

    private static Vector3 CanvasPosition(Transform transform)
    {
        var canvas = transform.GetComponentInParent<Canvas>();
        var root = canvas ? canvas.rootCanvas.transform : null;
        return root ? root.InverseTransformPoint(transform.position) : transform.position;
    }

    /// <summary>
    /// 0.5.0 writes a toggle's two labels through option1TextMesh/option2TextMesh, fields RepoXR's own toggles (built
    /// for 0.4.4: VR Settings, the VR controls' hold/toggle) do not have: their labels kept the prefab's placeholder
    /// text. The buttons' own texts are used when those fields are empty
    /// </summary>
    [HarmonyPatch(typeof(MenuTwoOptions), nameof(MenuTwoOptions.Start))]
    [HarmonyPrefix]
    private static void LabelsFromButtons(MenuTwoOptions __instance)
    {
        if (!__instance.option1TextMesh && __instance.option1Button)
            __instance.option1TextMesh = __instance.option1Button.buttonText
                ? __instance.option1Button.buttonText
                : __instance.option1Button.GetComponentInChildren<TMPro.TextMeshProUGUI>(true);

        if (!__instance.option2TextMesh && __instance.option2Button)
            __instance.option2TextMesh = __instance.option2Button.buttonText
                ? __instance.option2Button.buttonText
                : __instance.option2Button.GetComponentInChildren<TMPro.TextMeshProUGUI>(true);
    }
}

/// <summary>
/// The game's keybind buttons show RepoXR's VR controller icons (InputManager.InputDisplayGet answers with sprite
/// tags); their texts need RepoXR's sprite asset for them (see <see cref="Utils.EnsureVRSprites"/>)
/// </summary>
[RepoXRPatch]
internal static class KeybindSpritePatches
{
    [HarmonyPatch(typeof(MenuKeybind), nameof(MenuKeybind.UpdateBindingDisplay))]
    [HarmonyPrefix]
    private static void VRSprites(MenuKeybind __instance)
    {
        if (__instance.menuBigButton && __instance.menuBigButton.menuButton)
            Utils.EnsureVRSprites(__instance.menuBigButton.menuButton.buttonText);
    }
}
