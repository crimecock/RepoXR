using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;
using RepoXR.Managers;
using UnityEngine;

using static HarmonyLib.AccessTools;

namespace RepoXR.Patches.Item;

/// <summary>
/// R.E.P.O. 0.5.0's whip cracks and flings by gestures: ItemMeleeWhip.InputTick reads the look delta
/// (InputManager.GrabCursorLookGet: the mouse, or the gamepad's right stick) and the aim frame of the camera
/// (forward/right/up), and its gestures look for a backswing and a return of that aim. In VR the look delta is always
/// zero, so the whip could only be dangled. Here, while the whip reads its input, the aim frame is the hand and the
/// look delta is how far the hand turned this frame, in the units the game expects (degrees / (180 * deltaTime)):
/// a quick flick of the controller back and forward cracks, a sweep to the side flings
/// </summary>
[RepoXRPatch]
internal static class ItemMeleeWhipPatches
{
    private static bool readingWhipInput;
    private static Quaternion previousHand;
    private static int previousFrame = -10;

    [HarmonyPatch(typeof(ItemMeleeWhip), nameof(ItemMeleeWhip.InputTick))]
    [HarmonyPrefix]
    private static void BeforeInputTick() => readingWhipInput = true;

    [HarmonyPatch(typeof(ItemMeleeWhip), nameof(ItemMeleeWhip.InputTick))]
    [HarmonyFinalizer]
    private static void AfterInputTick() => readingWhipInput = false;

    /// <summary>
    /// The whip's aim frame: the hand instead of the camera (GetOverrideTransform, and the CameraAim fallback)
    /// </summary>
    [HarmonyPatch(typeof(ItemMeleeWhip), nameof(ItemMeleeWhip.InputTick))]
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction> AimFromHandPatch(IEnumerable<CodeInstruction> instructions)
    {
        return new CodeMatcher(instructions)
            .MatchForward(false,
                new CodeMatch(OpCodes.Callvirt,
                    Method(typeof(PlayerLocalCamera), nameof(PlayerLocalCamera.GetOverrideTransform))))
            .ThrowIfInvalid("ItemMeleeWhip.InputTick: GetOverrideTransform not found")
            .Set(OpCodes.Call, PlayerLocalCameraExtensions.GetHandOverrideTransformMethod)
            .MatchForward(false,
                new CodeMatch(OpCodes.Ldsfld, Field(typeof(CameraAim), nameof(CameraAim.Instance))),
                new CodeMatch(OpCodes.Callvirt, PropertyGetter(typeof(Component), nameof(Component.transform))))
            .ThrowIfInvalid("ItemMeleeWhip.InputTick: CameraAim.Instance.transform not found")
            .Advance(1)
            .Set(OpCodes.Call, ((System.Func<CameraAim, Transform>)AimTransform).Method)
            .InstructionEnumeration();
    }

    private static Transform? aimFrame;

    /// <summary>
    /// The whip's aim frame in VR: at the hand, pointing where the hand points, but upright (no wrist roll), like the
    /// mouse-driven camera the gestures were made for
    /// </summary>
    private static Transform AimTransform(CameraAim cameraAim)
    {
        if (VRSession.Instance is not { } session)
            return cameraAim.transform;

        if (!aimFrame)
        {
            aimFrame = new GameObject("RepoXR Whip Aim") { hideFlags = HideFlags.HideAndDontSave }.transform;
            Object.DontDestroyOnLoad(aimFrame.gameObject);
        }

        var hand = session.Player.MainHand;
        var space = session.MainCamera.transform.parent;
        var up = space ? space.up : Vector3.up;
        aimFrame!.SetPositionAndRotation(hand.position,
            Mathf.Abs(Vector3.Dot(hand.forward, up)) < 0.99f ? Quaternion.LookRotation(hand.forward, up) : hand.rotation);

        return aimFrame;
    }

    /// <summary>
    /// The look delta while the whip reads its input: how fast the hand turns, split like a mouse into turning around
    /// the vertical (x) and tipping up or down around the head's horizontal right (y). Measured in the space the hands
    /// are tracked in, so turning with the stick does not count; an angular velocity rather than angles, so a swing
    /// from over the shoulder (the hand pointing straight up and past it) does not flip
    /// </summary>
    [HarmonyPatch(typeof(InputManager), nameof(InputManager.GrabCursorLookGet))]
    [HarmonyPostfix]
    private static void HandLookDelta(bool __0, ref Vector2 __result)
    {
        if (!readingWhipInput || VRSession.Instance is not { } session)
            return;

        var hand = session.Player.MainHand;
        var camera = session.MainCamera.transform;
        var space = camera.parent;
        var rotation = space ? Quaternion.Inverse(space.rotation) * hand.rotation : hand.rotation;

        var dt = Time.deltaTime;
        var consecutive = Time.frameCount - previousFrame == 1;
        var delta = rotation * Quaternion.Inverse(previousHand);
        previousHand = rotation;
        previousFrame = Time.frameCount;

        if (!consecutive || dt <= 0)
        {
            __result = Vector2.zero;
            return;
        }

        delta.ToAngleAxis(out var angle, out var axis);
        if (angle > 180)
            angle -= 360;
        if (!float.IsFinite(axis.x) || !float.IsFinite(angle))
        {
            __result = Vector2.zero;
            return;
        }

        // Degrees turned this frame around each axis of the tracking space
        var turn = axis * angle;
        var headRight = Quaternion.Euler(0, (space ? camera.localEulerAngles : camera.eulerAngles).y, 0) * Vector3.right;
        // A hand moves much faster than a mouse is pushed; the sideways part (the fling) went off too easily
        var yaw = Vector3.Dot(turn, Vector3.up) * Plugin.Config.WhipSidewaysSensitivity.Value;
        var pitch = Vector3.Dot(turn, headRight) * Plugin.Config.WhipCrackSensitivity.Value;

        // Mouse: x turns right, y looks up (a negative pitch around the right axis)
        var value = new Vector2(yaw, -pitch) / (180 * dt);
        __result = __0 ? Vector2.ClampMagnitude(value, 1) : value;
    }
}
