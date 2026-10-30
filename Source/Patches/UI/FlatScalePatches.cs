using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using UnityEngine;

using static HarmonyLib.AccessTools;

namespace RepoXR.Patches.UI;

/// <summary>
/// R.E.P.O. 0.5.0 animates a few menu popups with <c>transform.localScale = Vector2.Lerp(...)</c>: the Vector2 turns
/// into a Vector3 with z = 0. Flat, on a screen-space canvas, that is harmless. On RepoXR's world-space canvases
/// TextMeshPro's SDF shader takes the text's normal through that scale for its perspective filter, divides by the zero
/// and every letter came out as a solid bar (the main menu's news popup, the cosmetics popup). The scale keeps z = 1
/// </summary>
[RepoXRPatch]
internal static class FlatScalePatches
{
    private static readonly MethodInfo Vector2ToVector3 =
        Method(typeof(Vector2), "op_Implicit", [typeof(Vector2)]);

    private static readonly MethodInfo SetLocalScale =
        PropertySetter(typeof(Transform), nameof(Transform.localScale));

    private static IEnumerable<MethodBase> TargetMethods() =>
    [
        Method(typeof(MenuPageMain), nameof(MenuPageMain.RunMarketingPopupCurves)),
        Method(typeof(MenuElementCosmeticButtonPopup), "Update"),
        Method(typeof(CosmeticTokenUIElementDestroy), "Update")
    ];

    private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, MethodBase original)
    {
        var code = instructions.ToList();
        var replaced = 0;

        for (var i = 0; i < code.Count - 1; i++)
        {
            if (!code[i].Calls(Vector2ToVector3) || !code[i + 1].Calls(SetLocalScale))
                continue;

            code[i] = new CodeInstruction(OpCodes.Call, ((System.Func<Vector2, Vector3>)Scale).Method)
                .WithLabels(code[i].labels).WithBlocks(code[i].blocks);
            replaced++;
        }

        if (replaced == 0)
            Logger.LogWarning($"[UI] {original.DeclaringType?.Name}.{original.Name}: no Vector2 scale found");

        return code;
    }

    private static Vector3 Scale(Vector2 scale) => new(scale.x, scale.y, 1);
}
