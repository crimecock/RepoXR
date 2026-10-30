using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using HarmonyLib;
using RepoXR.Assets;
using RepoXR.Input;
using RepoXR.Managers;
using RepoXR.Networking;
using RepoXR.Player;
using UnityEngine;

using static HarmonyLib.AccessTools;

namespace RepoXR.Patches;

[RepoXRPatch]
internal static class PhysGrabberPatches
{
    /// <summary>
    /// Make certain phys grabber operations operate from the hand transform instead of the camera transform
    /// </summary>
    [HarmonyPatch(typeof(PhysGrabber), nameof(PhysGrabber.Update))]
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction> UpdatePatches(IEnumerable<CodeInstruction> instructions)
    {
        return new CodeMatcher(instructions)
            .MatchForward(false,
                new CodeMatch(OpCodes.Callvirt, Method(typeof(PlayerLocalCamera), nameof(PlayerLocalCamera.GetOverrideTransform))))
            .Repeat(matcher => matcher.Set(OpCodes.Call, PlayerLocalCameraExtensions.GetHandOverrideTransformMethod))
            .InstructionEnumeration();
    }

    /// <summary>
    /// Slow down the push/pull logic since it's way too fast in VR
    /// </summary>
    [HarmonyPatch(typeof(PhysGrabber), nameof(PhysGrabber.Update))]
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction> PushPullSlowdownPatch(IEnumerable<CodeInstruction> instructions)
    {
        return new CodeMatcher(instructions)
            .MatchForward(false, new CodeMatch(OpCodes.Ldc_R4, 0.2f))
            .SetOperandAndAdvance(0.1f)
            .Insert(
                new CodeInstruction(OpCodes.Ldsfld, Field(typeof(InputManager), nameof(InputManager.instance))),
                new CodeInstruction(OpCodes.Callvirt,
                    Method(typeof(InputManager), nameof(InputManager.KeyPullAndPush))),
                new CodeInstruction(OpCodes.Mul)
            )
            .MatchForward(false, new CodeMatch(OpCodes.Ldc_R4, 0.2f))
            .SetOperandAndAdvance(-0.1f)
            .Insert(
                new CodeInstruction(OpCodes.Ldsfld, Field(typeof(InputManager), nameof(InputManager.instance))),
                new CodeInstruction(OpCodes.Callvirt,
                    Method(typeof(InputManager), nameof(InputManager.KeyPullAndPush))),
                new CodeInstruction(OpCodes.Mul)
            )
            .InstructionEnumeration();
    }

    /// <summary>
    /// Provide haptic feedback while something is grabbed
    /// </summary>
    [HarmonyPatch(typeof(PhysGrabber), nameof(PhysGrabber.Update))]
    [HarmonyPostfix]
    private static void HapticFeedbackPatch(PhysGrabber __instance)
    {
        if (!__instance.isLocal)
            return;

        // Do not provide haptic feedback if we're being forced to hold something
        if (__instance.overrideGrabTimer > 0)
            return;

        var grabbed = __instance.grabbed
            ? AssetCollection.GrabberHapticCurve.EvaluateTimed(__instance.loopSound.Source.pitch * 1.12667f) * 0.1f
            : 0;
        var overcharge = __instance.physGrabBeamOverChargeFloat * 0.4f *
                         AssetCollection.OverchargeHapticCurve.EvaluateTimed(__instance.physGrabBeamOverChargeFloat *
                                                                             3);

        if (grabbed + overcharge <= 0)
            return;

        HapticManager.Impulse(HapticManager.Hand.Dominant, HapticManager.Type.Continuous, grabbed + overcharge);
    }

    /// <summary>
    /// When grabbing items, shoot rays out of the hand, instead of the camera
    /// </summary>
    [HarmonyPatch(typeof(PhysGrabber), nameof(PhysGrabber.RayCheck))]
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction> RayCheckPatches(IEnumerable<CodeInstruction> instructions)
    {
        return new CodeMatcher(instructions)
            // Replace grabber checks from camera with checks from hand
            .MatchForward(false,
                new CodeMatch(OpCodes.Callvirt,
                    Method(typeof(PlayerLocalCamera), nameof(PlayerLocalCamera.GetOverrideTransform))))
            .Set(OpCodes.Call, PlayerLocalCameraExtensions.GetHandOverrideTransformMethod)
            .InstructionEnumeration();
    }

    /// <summary>
    /// Make the object turning input use the controller inputs instead of mouse inputs
    /// </summary>
    [HarmonyPatch(typeof(PhysGrabber), nameof(PhysGrabber.ObjectTurning))]
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction> ObjectTurningPatches(IEnumerable<CodeInstruction> instructions)
    {
        return new CodeMatcher(instructions)
            // Use VR controls for rotation instead of mouse inputs
            // R.E.P.O. 0.5.0: x = SemiFunc.InputMouseX() * sensitivity; y = SemiFunc.InputMouseY() * sensitivity (8 instructions)
            .MatchForward(false, new CodeMatch(OpCodes.Call, Method(typeof(SemiFunc), nameof(SemiFunc.InputMouseX))))
            .RemoveInstructions(8)
            .Insert(
                // TESTER: Check these operands in Debug vs Release builds
                new CodeInstruction(OpCodes.Ldloca_S, Plugin.GameDebugBuild ? 13 : 5), // Mouse X
                new CodeInstruction(OpCodes.Ldloca_S, Plugin.GameDebugBuild ? 14 : 6), // Mouse Y
                new CodeInstruction(OpCodes.Call, Method(typeof(PhysGrabberPatches), nameof(GetRotationInput)))
            )
            .InstructionEnumeration();
    }

    private static void GetRotationInput(out float x, out float y)
    {
        var input = Actions.Instance["Rotation"].ReadValue<Vector2>();

        x = input.x;
        y = input.y;
    }

    /// <summary>
    /// Allow a custom override to disable object turning
    /// </summary>
    [HarmonyPatch(typeof(PhysGrabber), nameof(PhysGrabber.ObjectTurning))]
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction> DisableTurningPatch(IEnumerable<CodeInstruction> instructions)
    {
        var matcher = new CodeMatcher(instructions)
            .MatchForward(false, new CodeMatch(OpCodes.Call, Method(typeof(SemiFunc), nameof(SemiFunc.InputHold))))
            .Advance(1);

        var jmp = matcher.Instruction;

        matcher.Advance(1).InsertAndAdvance(
            new CodeInstruction(OpCodes.Call, PropertyGetter(typeof(VRSession), nameof(VRSession.Instance))),
            new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(VRSession), nameof(VRSession.Player))),
            new CodeInstruction(OpCodes.Ldfld, Field(typeof(VRPlayer), nameof(VRPlayer.disableRotateTimer))),
            new CodeInstruction(OpCodes.Ldc_R4, 0f),
            new CodeInstruction(OpCodes.Bgt_Un_S, jmp.operand)
        );

        return matcher
            .InstructionEnumeration();
    }

    /// <summary>
    /// Detect item release and try to equip item if possible
    /// </summary>
    [HarmonyPatch(typeof(PhysGrabber), nameof(PhysGrabber.ReleaseObject))]
    [HarmonyPrefix]
    private static void OnReleaseObject(PhysGrabber __instance)
    {
        if (!__instance.grabbed || !__instance.isLocal || !__instance.grabbedObject ||
            !__instance.grabbedObject.TryGetComponent<ItemEquippable>(out var item))
            return;

        if (VRSession.Instance is not { } session)
            return;

        session.Player.Rig.inventoryController.TryEquipItem(item);
    }

    /// <summary>
    /// Make sure the force grab timer also works in VR, where the grab key is the same as the "take from inventory" key
    /// </summary>
    [HarmonyPatch(typeof(PhysGrabber), nameof(PhysGrabber.Update))]
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction> ForceHoldPatch(IEnumerable<CodeInstruction> instructions)
    {
        return new CodeMatcher(instructions)
            .MatchForward(false,
                new CodeMatch(OpCodes.Ldfld,
                    Field(typeof(GameplayManager), nameof(GameplayManager.itemUnequipAutoHold))))
            .Advance(-2)
            .InsertAndAdvance(
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Ldfld, Field(typeof(PhysGrabber), nameof(PhysGrabber.overrideGrabTimer))),
                new CodeInstruction(OpCodes.Call, ((Func<bool, float, bool>)ShouldToggleGrabOff).Method)
            )
            .InstructionEnumeration();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static bool ShouldToggleGrabOff(bool grabHeld, float grabTimer) => grabHeld && grabTimer <= 0;
    }
}

[RepoXRPatch(RepoXRPatchTarget.Universal)]
internal static class PhysGrabberUniversalPatches
{
    [HarmonyPatch(typeof(PhysGrabber), nameof(PhysGrabber.ObjectTurning))]
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction> ObjectTurningPatches(IEnumerable<CodeInstruction> instructions)
    {
        return new CodeMatcher(instructions)
            // Replace camera transform with hand transform (local player)
            .MatchForward(false,
                new CodeMatch(OpCodes.Callvirt,
                    Method(typeof(PlayerLocalCamera), nameof(PlayerLocalCamera.GetOverrideTransform))))
            .Set(OpCodes.Call, PlayerLocalCameraExtensions.GetHandOverrideTransformMethod)
            // Replace camera transform with hand transform (remote player)
            .MatchForward(false,
                new CodeMatch(OpCodes.Callvirt,
                    Method(typeof(PlayerLocalCamera), nameof(PlayerLocalCamera.GetOverrideTransform))))
            .Set(OpCodes.Call, PlayerLocalCameraExtensions.GetHandOverrideTransformMethod)
            .InstructionEnumeration();
    }

    /// <summary>
    /// Force the grabbing of objects to start from the hand
    /// </summary>
    [HarmonyPatch(typeof(PhysGrabber), nameof(PhysGrabber.StartGrabbingPhysObject))]
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction> GrabFromHandPatch(IEnumerable<CodeInstruction> instructions)
    {
        return new CodeMatcher(instructions)
            // Replace camera transform with hand transform
            .MatchForward(false,
                new CodeMatch(OpCodes.Callvirt,
                    Method(typeof(PlayerLocalCamera), nameof(PlayerLocalCamera.GetOverrideTransform))))
            .Set(OpCodes.Call, PlayerLocalCameraExtensions.GetHandOverrideTransformMethod)
            .InstructionEnumeration();
    }

    /// <summary>
    /// Make our tumble climb follow our hand rotation instead of the camera rotation
    /// </summary>
    // R.E.P.O. 0.5.0: the climb rotation moved out of GrabStateClimb into ClimbTargetUpdate, which starts from the
    // climb stick's rotation (`var rotation = climbStickTransform.rotation`); that read becomes the hand's rotation.
    // (Matched against GrabStateClimb, which no longer calls Quaternion.Angle, the old Advance(-5) cut into its end and
    // IL2CPP refused the method)
    [HarmonyPatch(typeof(PhysGrabber), nameof(PhysGrabber.ClimbTargetUpdate))]
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction> HandBasedTumbleClimbPatch(IEnumerable<CodeInstruction> instructions)
    {
        return new CodeMatcher(instructions)
            .MatchForward(false,
                new CodeMatch(OpCodes.Ldfld, Field(typeof(PhysGrabber), nameof(PhysGrabber.climbStickTransform))),
                new CodeMatch(OpCodes.Callvirt, PropertyGetter(typeof(Transform), nameof(Transform.rotation))),
                new CodeMatch(OpCodes.Stloc_0))
            .ThrowIfInvalid("PhysGrabber.ClimbTargetUpdate: climbStickTransform.rotation not found")
            .SetAndAdvance(OpCodes.Call, ((Func<PhysGrabber, Quaternion>)GetRotation).Method)
            .RemoveInstruction()
            .InstructionEnumeration();

        static Quaternion GetRotation(PhysGrabber grabber)
        {
            if (grabber.playerAvatar.isLocal)
                return VRSession.InVR
                    ? VRSession.Instance.Player.MainHand.rotation * Quaternion.Euler(0, 180, 0)
                    : grabber.climbStickTransform.rotation;

            if (!NetworkSystem.instance)
                return grabber.climbStickTransform.rotation;

            return NetworkSystem.instance.GetNetworkPlayer(grabber.playerAvatar, out var networkPlayer)
                ? networkPlayer.PrimaryHand.rotation * Quaternion.Euler(0, 180, 0)
                : grabber.climbStickTransform.rotation;
        }
    }

    private static float nextGrabReport;
    private static int grabReports;

    /// <summary>
    /// Quest port, diagnostics for R.E.P.O. 0.5.0: the VR input the grabber sees. A few reports after a level loads
    /// (control scheme, devices, the Grab/Movement/Interact actions), and one line per Grab press or release with the
    /// grabber's state ("[Grab] ...")
    /// </summary>
    [HarmonyPatch(typeof(PhysGrabber), nameof(PhysGrabber.Update))]
    [HarmonyPostfix]
    private static void GrabInputReport(PhysGrabber __instance)
    {
        if (!__instance.isLocal)
            return;

        try
        {
            var input = VRInputSystem.Instance;
            var grab = Actions.Instance["Grab"];
            if (grabReports < 6 && Time.unscaledTime >= nextGrabReport)
            {
                nextGrabReport = Time.unscaledTime + 5f;
                grabReports++;
                var player = input.GetPlayerInput();
                Logger.LogInfo($"[Grab] input: scheme {input.CurrentControlScheme}, active {player.inputIsActive}, map {player.currentActionMap?.name}, " +
                               $"devices [{string.Join(", ", System.Linq.Enumerable.Select(player.devices, d => d.layout))}]");
                foreach (var name in new[] { "Grab", "Movement", "Interact", "Menu", "Jump" })
                {
                    var action = Actions.Instance[name];
                    Logger.LogInfo(action == null
                        ? $"[Grab] action {name}: missing"
                        : $"[Grab] action {name}: enabled {action.enabled}, bindings [{string.Join("; ", System.Linq.Enumerable.Select(action.bindings, b => b.effectivePath))}], " +
                          $"controls [{string.Join("; ", System.Linq.Enumerable.Select(action.controls, c => c.path))}], phase {action.phase}");
                }
            }

            if (grab != null && (grab.WasPressedThisFrame() || grab.WasReleasedThisFrame()))
                Logger.LogInfo($"[Grab] {(grab.WasPressedThisFrame() ? "pressed" : "released")}: hold {SemiFunc.InputHold(InputKey.Grab)}, down {SemiFunc.InputDown(InputKey.Grab)}, " +
                               $"toggle {InputManager.instance.InputToggleGet(InputKey.Grab)}, grabbed {__instance.grabbed}, beam {__instance.physGrabBeamActive}, " +
                               $"disable timer {__instance.grabDisableTimer:F2}, looking at {(__instance.currentlyLookingAtPhysGrabObject ? __instance.currentlyLookingAtPhysGrabObject.name : "-")}, " +
                               $"input disabled {(PlayerController.instance ? PlayerController.instance.InputDisableTimer : -1):F2}, avatar disabled {__instance.playerAvatar.isDisabled}");
        }
        catch (System.Exception e)
        {
            if (grabReports < 7)
            {
                grabReports++;
                Logger.LogWarning($"[Grab] report failed: {e}");
            }
        }
    }
}