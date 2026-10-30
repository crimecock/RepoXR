using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;

using static HarmonyLib.AccessTools;

namespace RepoXR.Patches.Item;

[RepoXRPatch(RepoXRPatchTarget.Universal)]
internal static class ItemGunPatches
{
    /// <summary>
    /// R.E.P.O. 0.5.0 turns a held gun so that its muzzle points along the line from the camera through the grab
    /// point, and stores that as the grabber's camera-relative rotation. RepoXR applies the held rotation relative to
    /// the hand (PhysGrabObject.PhysicsGrabbingManipulation), so measured from the head the gun pointed somewhere else
    /// whenever head and hand did; from the hand, the gun points where the grab beam goes
    /// </summary>
    [HarmonyPatch(typeof(ItemGun), nameof(ItemGun.AimTargetUpdate))]
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction> AimFromHandPatch(IEnumerable<CodeInstruction> instructions)
    {
        return new CodeMatcher(instructions)
            .MatchForward(false,
                new CodeMatch(OpCodes.Callvirt,
                    Method(typeof(PlayerLocalCamera), nameof(PlayerLocalCamera.GetOverrideTransform))))
            .ThrowIfInvalid("ItemGun.AimTargetUpdate: GetOverrideTransform not found")
            .Set(OpCodes.Call, PlayerLocalCameraExtensions.GetHandOverrideTransformMethod)
            .InstructionEnumeration();
    }
}
