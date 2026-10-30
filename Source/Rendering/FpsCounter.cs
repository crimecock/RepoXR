using RepoXR.Managers;
using UnityEngine;
using UnityEngine.UI;

namespace RepoXR.Rendering;

/// <summary>
/// Quest port: frame rate readout (VR Settings → Rendering → FPS Counter). A small panel low in the left of the view,
/// half a second averages: frames per second, the average frame time and the slowest frame. Frame times are read
/// with Time.unscaledDeltaTime, so they include waiting for the compositor; when Unity's frame timing stats are
/// available, the CPU and GPU times are shown as well.
/// </summary>
internal class FpsCounter : MonoBehaviour
{
    private const float Interval = 0.5f;

    private Canvas canvas = null!;
    private Text text = null!;
    private Image background = null!;

    private int frames;
    private float elapsed, worst;
    private readonly FrameTiming[] timings = new FrameTiming[1];
    private Camera? reportedCamera;
    private bool reportedNoCamera;

    public static void Create()
    {
        var host = new GameObject("RepoXR FPS Counter");
        DontDestroyOnLoad(host);
        host.AddComponent<FpsCounter>();
    }

    private void Awake()
    {
        canvas = gameObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.WorldSpace;
        canvas.sortingOrder = short.MaxValue;

        var rect = (RectTransform)transform;
        rect.sizeDelta = new Vector2(300, 48);
        rect.localScale = Vector3.one * 0.001f; // 1 unit = 1 mm

        var panel = new GameObject("Background", typeof(RectTransform));
        panel.transform.SetParent(transform, false);
        background = panel.AddComponent<Image>();
        background.color = new Color(0, 0, 0, 0.55f);
        background.raycastTarget = false;
        Stretch((RectTransform)panel.transform);

        var label = new GameObject("Text", typeof(RectTransform));
        label.transform.SetParent(transform, false);
        text = label.AddComponent<Text>();
        text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        text.fontSize = 22;
        text.alignment = TextAnchor.MiddleCenter;
        text.horizontalOverflow = HorizontalWrapMode.Overflow;
        text.verticalOverflow = VerticalWrapMode.Overflow;
        text.raycastTarget = false;
        text.text = "-- FPS";
        Stretch((RectTransform)label.transform);

        Show(false);
    }

    private static void Stretch(RectTransform rect)
    {
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
    }

    private void OnEnable() => Application.onBeforeRender += Follow;

    private void OnDisable() => Application.onBeforeRender -= Follow;

    private void Show(bool show)
    {
        if (canvas.enabled != show)
            canvas.enabled = show;
    }

    private bool? lastSetting;

    private void Update()
    {
        if (lastSetting != Plugin.Config.ShowFPS.Value)
        {
            lastSetting = Plugin.Config.ShowFPS.Value;
            Logger.LogInfo($"[FPS] counter {(lastSetting.Value ? "on" : "off")}");
        }

        if (!Plugin.Config.ShowFPS.Value)
        {
            Show(false);
            frames = 0;
            elapsed = worst = 0;
            return;
        }

        var dt = Time.unscaledDeltaTime;
        frames++;
        elapsed += dt;
        if (dt > worst)
            worst = dt;

        FrameTimingManager.CaptureFrameTimings();

        if (elapsed < Interval)
            return;

        var fps = frames / elapsed;
        var line = $"{fps:0} FPS  {elapsed * 1000 / frames:0.0} ms  max {worst * 1000:0}";
        if (FrameTimingManager.GetLatestTimings(1, timings) > 0 && timings[0].cpuFrameTime > 0)
            line += $"\nCPU {timings[0].cpuFrameTime:0.0}  GPU {(timings[0].gpuFrameTime > 0 ? timings[0].gpuFrameTime.ToString("0.0") : "-")} ms";

        text.text = line;
        text.color = fps >= 68 ? new Color(0.5f, 1, 0.5f) : fps >= 50 ? new Color(1, 0.9f, 0.4f) : new Color(1, 0.45f, 0.4f);
        ((RectTransform)transform).sizeDelta = new Vector2(300, line.Contains('\n') ? 76 : 48);

        frames = 0;
        elapsed = worst = 0;
    }

    /// <summary>Right before rendering, with this frame's head pose: below and left of the center of the view</summary>
    private void Follow()
    {
        if (!Plugin.Config.ShowFPS.Value)
            return;

        // RepoXR's VR camera in a level; the main menu sets up its own, tagged as the main camera
        var camera = VRSession.Instance is { MainCamera: { } vr } && vr.isActiveAndEnabled ? vr : Camera.main;
        if (camera == null)
        {
            if (reportedCamera != null || !reportedNoCamera)
                Logger.LogInfo("[FPS] no camera to follow");
            reportedNoCamera = true;
            reportedCamera = null;
            Show(false);
            return;
        }

        var head = camera.transform;
        transform.SetPositionAndRotation(head.position + head.rotation * new Vector3(-0.17f, -0.2f, 0.8f),
            head.rotation);
        var layer = LayerFor(camera);
        if (gameObject.layer != layer)
            SetLayer(transform, layer);
        Show(true);

        if (camera != reportedCamera)
        {
            reportedCamera = camera;
            reportedNoCamera = false;
            Logger.LogInfo($"[FPS] following {camera.name} (layer {LayerMask.LayerToName(layer)}, culling mask {camera.cullingMask:X}, " +
                           $"stereo {camera.stereoEnabled}), font {(text.font ? text.font.name : "none")}");
        }
    }

    /// <summary>A layer the camera draws: its own, else UI, else Default</summary>
    private static int LayerFor(Camera camera)
    {
        foreach (var layer in new[] { camera.gameObject.layer, 5, 0 })
            if ((camera.cullingMask & (1 << layer)) != 0)
                return layer;

        return camera.gameObject.layer;
    }

    private static void SetLayer(Transform root, int layer)
    {
        root.gameObject.layer = layer;
        foreach (Transform child in root)
            SetLayer(child, layer);
    }
}
