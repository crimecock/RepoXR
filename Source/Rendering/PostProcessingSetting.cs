using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

namespace RepoXR.Rendering;

/// <summary>
/// The game's post-processing (color grading, vignette, grain, ...) as a setting. The game renders its scene with the
/// main camera and then the "top" camera (held items) on top; the top camera's layer applies the game's profile to the
/// whole frame. The main camera's layer has no volume to read (its volume mask is Nothing), yet still runs its full
/// stack every frame, so it is always switched off. On Quest, with both eyes processed separately, the stack costs
/// about 5 ms of GPU time.
/// </summary>
internal static class PostProcessingSetting
{
    private static Camera mainCamera;
    private static Camera topCamera;

    public static void Apply(Camera main, Camera top)
    {
        mainCamera = main;
        topCamera = top;
        Update();
    }

    public static void Update()
    {
        if (mainCamera && mainCamera.GetComponent<PostProcessLayer>() is { } mainLayer && mainLayer.volumeLayer.value == 0)
            mainLayer.enabled = false;

        if (topCamera && topCamera.GetComponent<PostProcessLayer>() is { } topLayer)
            topLayer.enabled = Plugin.Config.PostProcessing.Value;
    }
}
