using System.Collections.Generic;
using UnityEngine;

namespace RepoXR.Rendering;

/// <summary>
/// Other players' flashlights cast shadows only if Rendering.OtherFlashlightShadows is on. Every shadow-casting
/// flashlight renders the scene once more into its own shadow map: with a full lobby of six that cost 7-10 ms of GPU
/// time per frame on Quest. Their light stays either way; your own flashlight is left alone.
/// </summary>
internal static class FlashlightShadows
{
    private static readonly Dictionary<Light, LightShadows> Original = new();

    public static void Register(FlashlightController flashlight)
    {
        if (flashlight.PlayerAvatar.isLocal || !flashlight.spotlight)
            return;

        Original[flashlight.spotlight] = flashlight.spotlight.shadows;
        Apply(flashlight.spotlight);
    }

    public static void Update()
    {
        var gone = new List<Light>();
        foreach (var light in Original.Keys)
        {
            if (light)
                Apply(light);
            else
                gone.Add(light);
        }

        foreach (var light in gone)
            Original.Remove(light);
    }

    private static void Apply(Light light)
    {
        light.shadows = Plugin.Config.OtherFlashlightShadows.Value ? Original[light] : LightShadows.None;
    }
}
