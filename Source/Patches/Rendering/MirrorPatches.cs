using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;
using UnityEngine;

using static HarmonyLib.AccessTools;

namespace RepoXR.Patches.Rendering;

/// <summary>
/// R.E.P.O. 0.5.0's shop mirror (MirrorReflection, next to the cosmetic machine) renders one reflection per frame from
/// the camera's position with a symmetric frustum, and its shader samples it at the (flipped) screen position. In a
/// headset each eye has its own position and frustum: the reflection was a mess. In multi-pass rendering the mirror is
/// now rendered for each eye just before that eye renders, from the eye's reflected position with the eye's own
/// (mirrored) projection. In single-pass rendering both eyes share one reflection texture, which cannot be right for
/// either of them: the mirror is hidden there, which also saves rendering the scene again (the Quest's default).
/// VR Settings → Rendering → Shop Mirror (on by default) hides it too
/// </summary>
[RepoXRPatch]
internal static class MirrorPatches
{
    private static readonly HashSet<MirrorReflection> mirrors = [];
    private static Camera.StereoscopicEye? currentEye;
    private static bool subscribed;
    private static readonly HashSet<Camera.MonoOrStereoscopicEye> loggedEyes = [];
    private static bool loggedError;

    // Switches for testing the per-eye reflection (the internal test build has keys for them)
    internal static bool PerEye = true;
    internal static bool FlipSkew = true;
    internal static bool SwapEyes;

    private static bool SinglePass => Plugin.Config.SinglePassRendering.Value;
    private static bool Shown => Plugin.Config.ShopMirror.Value && !SinglePass;

    [HarmonyPatch(typeof(MirrorReflection), nameof(MirrorReflection.OnEnable))]
    [HarmonyPostfix]
    private static void OnMirrorEnable(MirrorReflection __instance)
    {
        mirrors.Add(__instance);
        if (!subscribed)
        {
            Camera.onPreRender += OnCameraPreRender;
            Plugin.Config.ShopMirror.SettingChanged += (_, _) => ShowOrHideAll();
            subscribed = true;
        }

        ShowOrHide(__instance);
        if (!Shown)
            Logger.LogInfo($"[Mirror] {__instance.name}: hidden ({(SinglePass ? "single-pass rendering shares one reflection between both eyes" : "VR Settings → Rendering → Shop Mirror is off")})");
    }

    private static void ShowOrHideAll()
    {
        foreach (var mirror in mirrors)
            if (mirror)
                ShowOrHide(mirror);
    }

    /// <summary>
    /// A hidden mirror's renderer is off; the game renders no reflection for a disabled renderer (ShouldRender)
    /// </summary>
    private static void ShowOrHide(MirrorReflection mirror)
    {
        var renderer = mirror.mirrorRenderer ? mirror.mirrorRenderer : mirror.GetComponent<Renderer>();
        if (renderer)
            renderer.enabled = Shown;
    }

    [HarmonyPatch(typeof(MirrorReflection), nameof(MirrorReflection.OnDisable))]
    [HarmonyPostfix]
    private static void OnMirrorDisable(MirrorReflection __instance) => mirrors.Remove(__instance);

    /// <summary>
    /// The game's own render (once, in the camera's pre-cull) is skipped for a headset camera
    /// </summary>
    [HarmonyPatch(typeof(MirrorReflection), nameof(MirrorReflection.CameraPreCull))]
    [HarmonyPrefix]
    private static bool SkipStereoPreCull(Camera __0) => !(PerEye && __0 && __0.stereoEnabled);

    /// <summary>
    /// Each eye renders its own reflection: no "rendered this frame already" throttle for them
    /// </summary>
    [HarmonyPatch(typeof(MirrorReflection), nameof(MirrorReflection.RenderThrottled))]
    [HarmonyPrefix]
    private static bool NoThrottlePerEye(MirrorReflection __instance, ref bool __result)
    {
        if (currentEye == null)
            return true;

        __result = __instance.distanceFadeCurrent >= 1;
        return false;
    }

    private static void OnCameraPreRender(Camera camera)
    {
        if (!PerEye || !camera || !camera.stereoEnabled || mirrors.Count == 0 || !Shown)
            return;

        var active = camera.stereoActiveEye;
        if (active == Camera.MonoOrStereoscopicEye.Mono)
            return;

        if (loggedEyes.Add(active))
            Logger.LogInfo($"[Mirror] rendering the reflection for the {active} eye ({camera.name}, frame {Time.frameCount})");

        currentEye = (active == Camera.MonoOrStereoscopicEye.Left) != SwapEyes ? Camera.StereoscopicEye.Left : Camera.StereoscopicEye.Right;
        try
        {
            foreach (var mirror in mirrors)
            {
                if (!mirror || !mirror.ShouldRender(camera))
                    continue;

                MirrorReflection.insideRendering = true;
                try
                {
                    mirror.RenderReflection(camera);
                }
                catch (Exception e)
                {
                    if (!loggedError)
                    {
                        loggedError = true;
                        Logger.LogWarning($"[Mirror] per-eye reflection failed: {e}");
                    }
                }
                finally
                {
                    MirrorReflection.insideRendering = false;
                }
            }
        }
        finally
        {
            currentEye = null;
        }
    }

    /// <summary>
    /// Just before the reflection camera renders, its pose and projection are made the current eye's
    /// </summary>
    [HarmonyPatch(typeof(MirrorReflection), nameof(MirrorReflection.RenderReflection))]
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction> RenderForEyePatch(IEnumerable<CodeInstruction> instructions)
    {
        return new CodeMatcher(instructions)
            .MatchForward(false, new CodeMatch(OpCodes.Callvirt, Method(typeof(Camera), nameof(Camera.Render))))
            .ThrowIfInvalid("MirrorReflection.RenderReflection: Camera.Render not found")
            .InsertAndAdvance(new CodeInstruction(OpCodes.Ldarg_0), new CodeInstruction(OpCodes.Ldarg_1))
            .SetInstruction(new CodeInstruction(OpCodes.Call,
                ((Action<Camera, MirrorReflection, Camera>)RenderForEye).Method))
            .InstructionEnumeration();
    }

    private static void RenderForEye(Camera reflection, MirrorReflection mirror, Camera source)
    {
        if (currentEye is { } eye && source)
        {
            var view = source.GetStereoViewMatrix(eye);
            var projection = source.GetStereoProjectionMatrix(eye);
            var eyeToWorld = view.inverse;
            var eyePosition = eyeToWorld.MultiplyPoint(Vector3.zero);
            var eyeForward = -(Vector3)eyeToWorld.GetColumn(2);
            var eyeUp = (Vector3)eyeToWorld.GetColumn(1);

            var normal = mirror.GetSurfaceNormal();
            var position = mirror.transform.position;
            var distance = -Vector3.Dot(normal, position) - mirror.clipPlaneOffset;
            var reflect = MirrorReflection.PlaneReflectionMatrix(new Vector4(normal.x, normal.y, normal.z, distance));

            // As the game does: the camera at the reflected eye, turned (not mirrored); the shader flips the image
            reflection.transform.SetPositionAndRotation(reflect.MultiplyPoint(eyePosition),
                Quaternion.LookRotation(Vector3.Reflect(eyeForward, normal), Vector3.Reflect(eyeUp, normal)));
            reflection.ResetWorldToCameraMatrix();
            var worldToCamera = reflection.worldToCameraMatrix;

            // The flipped image also flips the eye's off-centre frustum
            if (FlipSkew)
                projection.m02 = -projection.m02;
            reflection.projectionMatrix = projection;
            if (mirror.clipBehindSurface)
            {
                var clip = mirror.CameraSpacePlane(worldToCamera, position, normal, 1);
                clip.w = Mathf.Min(clip.w, -reflection.nearClipPlane);
                reflection.projectionMatrix = reflection.CalculateObliqueMatrix(clip);
            }

            reflection.cullingMatrix = reflection.projectionMatrix * worldToCamera;
        }

        reflection.Render();
    }
}
