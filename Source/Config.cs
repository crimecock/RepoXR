using System;
using BepInEx.Configuration;
using RepoXR.Assets;
using RepoXR.Managers;
using RepoXR.Player.Camera;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.OpenXR;
using Object = UnityEngine.Object;

namespace RepoXR;

public class Config(string assemblyPath, ConfigFile file)
{
    public string AssemblyPath { get; } = assemblyPath;
    public ConfigFile File { get; } = file;

    // General configuration

    [ConfigDescriptor(customName: "Enable VR", trueText: "Disable", falseText: "Enable")]
    public ConfigEntry<bool> DisableVR { get; } = file.Create("General", nameof(DisableVR), false,
        "Disables the main functionality of this mod, can be used if you want to play without VR while keeping the mod installed.");

    [ConfigDescriptor]
    public ConfigEntry<bool> ExtendedDebugging { get; } = file.Create("General", nameof(ExtendedDebugging), false,
        "Enables verbose debug logging during OpenXR initialization and loads symbols during startup");

    [ConfigDescriptor(customName: "VR Prescence", falseText: "Shown", trueText: "Hidden")]
    public ConfigEntry<bool> DisableVRPresence { get; } = file.Create("General", nameof(DisableVRPresence), false,
        "Disable the addition of VR status to Discord and Steam prescence.");

    // Gameplay configuration

    [ConfigDescriptor]
    public ConfigEntry<bool> ReducedAimImpact { get; } = file.Create("Gameplay", nameof(ReducedAimImpact), false,
        "When enabled, lowers the severity of force-look events (like the ceiling eye), which can be helpful for people with motion sickness");

    [ConfigDescriptor(customName: "Head Bob", stepSize: 5f, suffix: "%")]
    public ConfigEntry<int> HeadBob { get; } = file.Create("Gameplay", nameof(HeadBob), Plugin.IsQuest ? 0 : 25,
        new ConfigDescription(
            "How much the camera bobs and sways while walking, relative to the game's own camera bob. Lower it if walking makes you feel sick, 0% turns it off.",
            new AcceptableValueRange<int>(0, 100)));

    [ConfigDescriptor]
    public ConfigEntry<bool> RoomscaleCrouch { get; } = file.Create("Gameplay", nameof(RoomscaleCrouch), true,
        "When enabled, allows for the player to physically crouch to also crouch in-game");

    [ConfigDescriptor(customName: "Dominant Hand", falseText: "Right", trueText: "Left")]
    public ConfigEntry<bool> LeftHandDominant { get; } = file.Create("Gameplay", nameof(LeftHandDominant), false,
        "Whether to use the left or right hand as dominant hand (the hand used to pick up items)");

    [ConfigDescriptor(customName: "Arms", falseText: "Attached", trueText: "Detached")]
    public ConfigEntry<bool> DetachedArms { get; } = file.Create("Gameplay", nameof(DetachedArms), false,
        "Whether your arms are attached to your body, or if they are separate");

    [ConfigDescriptor]
    public ConfigEntry<HapticFeedbackOption> HapticFeedback { get; } =
        file.Create("Gameplay", nameof(HapticFeedback), HapticFeedbackOption.All,
            new ConfigDescription(
                "Controls how much haptic feedback you will experience while playing with the VR mod.",
                new AcceptableValueEnum<HapticFeedbackOption>()));

    [ConfigDescriptor(customName: "Eye Tracking", trueText: "Enabled", falseText: "Disabled")]
    public ConfigEntry<bool> EnableEyeTracking { get; } = file.Create("Gameplay", nameof(EnableEyeTracking), true,
        "If supported by the headset, use eye tracking to move your characters pupils for other players and for checking line of sight with enemies.");

    // UI configuration

    [ConfigDescriptor(customName: "HUD Height", pointerSize: 0.01f, stepSize: 0.05f)]
    public ConfigEntry<float> HUDPlaneOffset { get; } = file.Create("UI", nameof(HUDPlaneOffset), -0.45f,
        new ConfigDescription("The default height offset for the HUD", new AcceptableValueRange<float>(-1f, 0.5f)));

    [ConfigDescriptor(customName: "HUD Secondary Height", pointerSize: 0.01f, stepSize: 0.05f)]
    public ConfigEntry<float> HUDGazePlaneOffset { get; } = file.Create("UI", nameof(HUDGazePlaneOffset), -0.25f,
        new ConfigDescription("The height offset for the HUD when looking at it",
            new AcceptableValueRange<float>(-1f, 0.5f)));

    [ConfigDescriptor(pointerSize: 0.05f, stepSize: 0.25f)]
    public ConfigEntry<float> SmoothCanvasDistance { get; } = file.Create("UI", nameof(SmoothCanvasDistance), 1.5f,
        new ConfigDescription("The distance that the smooth canvas should be away from the main camera",
            new AcceptableValueRange<float>(1.25f, 3)));

    // Input configuration

    [ConfigDescriptor(enumDisableBar: true)]
    public ConfigEntry<TurnProviderOption> TurnProvider { get; } = file.Create("Input", nameof(TurnProvider),
        TurnProviderOption.Smooth,
        new ConfigDescription("Specify which turning provider your player uses, if any.",
            new AcceptableValueEnum<TurnProviderOption>()));

    [ConfigDescriptor(stepSize: 0.05f, pointerSize: 0.01f, suffix: "x")]
    public ConfigEntry<float> SmoothTurnSpeedModifier { get; } = file.Create("Input", nameof(SmoothTurnSpeedModifier),
        1f,
        new ConfigDescription(
            "A multiplier that is added to the smooth turning speed. Requires turn provider to be set to smooth.",
            new AcceptableValueRange<float>(0.25f, 5)));

    [ConfigDescriptor]
    public ConfigEntry<bool> AnalogSmoothTurn { get; } = file.Create("Input", nameof(AnalogSmoothTurn), true,
        "When enabled, makes the speed of the smooth turning dependent on how far the analog stick is pushed.");

    [ConfigDescriptor(stepSize: 5, suffix: "°")]
    public ConfigEntry<float> SnapTurnSize { get; } = file.Create("Input", nameof(SnapTurnSize), 45f,
        new ConfigDescription(
            "The amount of rotation that is applied when performing a snap turn. Requires turn provider to be set to snap.",
            new AcceptableValueRange<float>(10, 180)));

    [ConfigDescriptor]
    public ConfigEntry<bool> NormalizeMovement { get; } = file.Create("Input", nameof(NormalizeMovement), false,
        "When enabled, any direction you move in will always be at full speed, even when the stick is only pushed slightly.");

    [ConfigDescriptor(customName: "Vehicle direction source", trueText: "Head", falseText: "Hand")]
    public ConfigEntry<bool> VehicleHeadForward { get; } = file.Create("Input", nameof(VehicleHeadForward), false,
        "When enabled, will make the scooters go forward based on head rotation instead of hand rotation.");

    [ConfigDescriptor(customName: "Whip Sideways Sensitivity", pointerSize: 0.05f, stepSize: 0.1f)]
    public ConfigEntry<float> WhipSidewaysSensitivity { get; } = file.Create("Input", nameof(WhipSidewaysSensitivity),
        0.5f, new ConfigDescription(
            "How much a sideways swing of the hand counts for the whip's gestures (R.E.P.O. 0.5.0 made them for a mouse). Lower: the fling needs a wider, faster swing.",
            new AcceptableValueRange<float>(0.1f, 1.5f)));

    [ConfigDescriptor(customName: "Whip Crack Sensitivity", pointerSize: 0.05f, stepSize: 0.1f)]
    public ConfigEntry<float> WhipCrackSensitivity { get; } = file.Create("Input", nameof(WhipCrackSensitivity),
        1f, new ConfigDescription(
            "How much an up and down swing of the hand counts for the whip's gestures. Lower: the crack needs a wider, faster swing.",
            new AcceptableValueRange<float>(0.1f, 1.5f)));

    // Rendering configuration

    [ConfigDescriptor(stepSize: 5f, suffix: "%")]
    public ConfigEntry<int> CameraResolution { get; } = file.Create("Rendering", nameof(CameraResolution), 100,
        new ConfigDescription(
            "This setting configures the resolution scale of the game, lower values are more performant, but will make the game look worse.",
            new AcceptableValueRange<int>(5, 200)));

    [ConfigDescriptor(customName: "Pixel Lights", stepSize: 1f), QuestOnly]
    public ConfigEntry<int> PixelLightCount { get; } = file.Create("Rendering", nameof(PixelLightCount), 1,
        new ConfigDescription(
            "How many lights can light an object at full quality. Too few and lights compete (a surface darkens when the flashlight comes near); more costs performance. The PC game uses 12.",
            new AcceptableValueRange<int>(1, 12)));

    [ConfigDescriptor(customName: "Lamp Lighting"), QuestOnly]
    public ConfigEntry<bool> LampLighting { get; } = file.Create("Rendering", nameof(LampLighting), true,
        "Light the lamps nearest to you per pixel, all in one pass, instead of Unity lighting each object with a few of them in extra passes: no seams between objects and less work for the headset, but lamps cast no shadows. Flashlights are unchanged.");

    [ConfigDescriptor(customName: "Other Flashlight Shadows")]
    public ConfigEntry<bool> OtherFlashlightShadows { get; } = file.Create("Rendering", nameof(OtherFlashlightShadows), !Plugin.IsQuest,
        "Whether other players' flashlights cast shadows. Each one renders the scene again into its own shadow map: with a full lobby this cost 7-10 ms of GPU time per frame on Quest. Their light stays either way.");

    // Quest port: unity-tools/Runtime/LevelCulling.cs reads these three; hiding is rendering only
    [ConfigDescriptor(customName: "Level Culling"), QuestOnly]
    public ConfigEntry<bool> LevelCulling { get; } = file.Create("Rendering", nameof(LevelCulling), true,
        "Do not draw the parts of the level that are too far to matter: small objects beyond the distance below, and everything the fog hides completely. Only drawing changes: objects keep working, and multiplayer is not affected. Saves CPU time on Quest.");

    [ConfigDescriptor(customName: "Small Object Distance", stepSize: 5f, suffix: " m"), QuestOnly]
    public ConfigEntry<int> CullSmallDistance { get; } = file.Create("Rendering", nameof(CullSmallDistance), 20,
        new ConfigDescription(
            "With Level Culling on: props, valuables and items smaller than 1.5 m are not drawn beyond this distance.",
            new AcceptableValueRange<int>(10, 60)));

    [ConfigDescriptor(customName: "Room Distance", stepSize: 5f, suffix: " m"), QuestOnly]
    public ConfigEntry<int> CullFarDistance { get; } = file.Create("Rendering", nameof(CullFarDistance), 0,
        new ConfigDescription(
            "With Level Culling on: nothing is drawn beyond this distance. 0 m: as far as the fog lets you see (no visible difference). Lower values hide far rooms for more performance.",
            new AcceptableValueRange<int>(0, 100)));

    [ConfigDescriptor(customName: "Post Processing")]
    public ConfigEntry<bool> PostProcessing { get; } = file.Create("Rendering", nameof(PostProcessing), !Plugin.IsQuest,
        "The game's post-processing: color grading, vignette, grain and the other screen effects. Off gives a brighter image and saves about 5 ms of GPU time per frame on Quest.");

    // Quest port: Rendering/FpsCounter.cs
    [ConfigDescriptor(customName: "FPS Counter")]
    public ConfigEntry<bool> ShowFPS { get; } = file.Create("Rendering", nameof(ShowFPS), false,
        "Shows the frame rate low in the left of your view: frames per second, the average frame time and the slowest frame of the last half second (and the CPU and GPU times when the headset reports them). Quest 3 runs the game at 72 FPS: green at 68 and above, yellow from 50, red below.");

    [ConfigDescriptor]
    public ConfigEntry<bool> Vignette { get; } = file.Create("Rendering", nameof(Vignette), true,
        "Enables the vignette shader used in certain scenarios and levels in the game.");

    [ConfigDescriptor]
    public ConfigEntry<bool> CustomCamera { get; } =
        file.Create("Rendering", nameof(CustomCamera), false,
            "Adds a second camera mounted on top of the VR camera that will render separately from the VR camera to the display. This requires extra GPU power!");

    [ConfigDescriptor(stepSize: 15, pointerSize: 5)]
    public ConfigEntry<float> CustomCameraFramerate { get; } = file.Create("Rendering", nameof(CustomCameraFramerate),
        144f,
        new ConfigDescription(
            "The maximum frequency that the custom camera can render at. The custom camera framerate is limited to the VR headset's refresh rate, so setting this higher won't have any effect.",
            new AcceptableValueRange<float>(15, 144)));

    [ConfigDescriptor(stepSize: 5)]
    public ConfigEntry<float> CustomCameraFOV { get; } = file.Create("Rendering", nameof(CustomCameraFOV), 75f,
        new ConfigDescription("The field of view that the custom camera should have.",
            new AcceptableValueRange<float>(45, 120)));

    [ConfigDescriptor(percentage: true, stepSize: 0.1f, pointerSize: 0.05f)]
    public ConfigEntry<float> CustomCameraSmoothing { get; } = file.Create("Rendering", nameof(CustomCameraSmoothing),
        0.5f,
        new ConfigDescription("The amount of smoothing that is applied to the custom camera.",
            new AcceptableValueRange<float>(0, 1)));

    // [ConfigDescriptor(customName: "Render Mode", trueText: "Single Pass", falseText: "Multipass")]
    public ConfigEntry<bool> SinglePassRendering { get; } = file.Create("Rendering", nameof(SinglePassRendering), Plugin.IsQuest,
        "Whether to render both eyes in a single pass (multiview) instead of one pass per eye. On Quest this roughly halves the rendering work (measured 54 -> 71 FPS on Manor) and post-processing is fixed for it; turn it off if something only shows in one eye.");

    // Internal configuration

    public ConfigEntry<string> ControllerBindingsOverride { get; } = file.Create("Internal",
        nameof(ControllerBindingsOverride), "", "FOR INTERNAL USE ONLY, DO NOT EDIT");

    public ConfigEntry<string> ControllerBindingsOverrideLeft { get; } = file.Create("Internal",
        nameof(ControllerBindingsOverrideLeft), "", "FOR INTERNAL USE ONLY, DO NOT EDIT");

    public ConfigEntry<string> InputToggleBindings { get; } = file.Create("Internal", nameof(InputToggleBindings), "",
        "FOR INTERNAL USE ONLY, DO NOT EDIT");

    public ConfigEntry<string> OpenXRRuntimeFile { get; } = file.Create("Internal", nameof(OpenXRRuntimeFile), "",
        "FOR INTERNAL USE ONLY, DO NOT EDIT");

    /// <summary>
    /// Create persistent callbacks that persist for the entire duration of the application
    /// </summary>
    public void SetupGlobalCallbacks()
    {
        // Pixel Lights and Lamp Lighting are the Quest build's: on PC the game's own quality settings decide, and the
        // lamps need the Quest build's shaders (without them they would go dark)
        if (Plugin.IsQuest)
        {
            QualitySettings.pixelLightCount = PixelLightCount.Value;
            PixelLightCount.SettingChanged += (_, _) => QualitySettings.pixelLightCount = PixelLightCount.Value;
        }
        PostProcessing.SettingChanged += (_, _) => Rendering.PostProcessingSetting.Update();
        OtherFlashlightShadows.SettingChanged += (_, _) => Rendering.FlashlightShadows.Update();
        if (Plugin.IsQuest)
            Rendering.LampLighting.Create();
        Rendering.FpsCounter.Create();

        CameraResolution.SettingChanged += (_, _) =>
        {
            XRSettings.eyeTextureResolutionScale = CameraResolution.Value / 100f;
        };

        CustomCamera.SettingChanged += (_, _) =>
        {
            if (!VRSession.InVR)
                return;

            if (CustomCamera.Value)
                Object.Instantiate(AssetCollection.CustomCamera, Camera.main!.transform.parent);
            else
                Object.Destroy(VRCustomCamera.instance.gameObject);
        };

        DisableVRPresence.SettingChanged += (_, _) =>
        {
            if (!VRSession.InVR)
                return;

            if (RunManager.instance is {} runManager)
                runManager.UpdateSteamRichPresence();
        };

        SinglePassRendering.SettingChanged += (_, _) =>
        {
            OpenXRSettings.Instance.renderMode = SinglePassRendering.Value
                ?  OpenXRSettings.RenderMode.SinglePassInstanced
                : OpenXRSettings.RenderMode.MultiPass;
        };
    }

    public enum HapticFeedbackOption
    {
        Off,
        Reduced,
        All
    }

    public enum TurnProviderOption
    {
        Snap,
        Smooth,
        Disabled
    }
}

internal class AcceptableValueEnum<T>() : AcceptableValueBase(typeof(T))
    where T: Enum
{
    private readonly string[] names = Enum.GetNames(typeof(T));

    public override object Clamp(object value) => value;
    public override bool IsValid(object value) => true;
    public override string ToDescriptionString() => $"# Acceptable values: {string.Join(", ", names)}";
}

/// <summary>
/// A setting only the standalone Quest build uses: not shown in the VR settings on PC
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public class QuestOnlyAttribute : Attribute;

[AttributeUsage(AttributeTargets.Property)]
public class ConfigDescriptorAttribute(
    string? customName = null,
    bool percentage = false,
    float stepSize = 1.0f,
    float pointerSize = 1.0f,
    string suffix = "",
    string trueText = "On",
    string falseText = "Off",
    bool enumDisableBar = false) : Attribute
{
    public string? CustomName => customName;
    public bool Percentage => percentage;
    public float StepSize => stepSize;
    public float PointerSize => pointerSize;
    public string Suffix => suffix;
    public string TrueText => trueText;
    public string FalseText => falseText;
    public bool EnumDisableBar => enumDisableBar;
}

internal static class ConfigExtensions
{
    extension(ConfigFile file)
    {
        internal ConfigEntry<T> Create<T>(string section, string key, T defaultValue,
            string description)
        {
            return file.Bind(section, key, defaultValue,
                new ConfigDescription(description, null, "HideFromREPOConfig"));
        }

        internal ConfigEntry<T> Create<T>(string section, string key, T defaultValue,
            ConfigDescription description)
        {
            return file.Bind(section, key, defaultValue,
                new ConfigDescription(description.Description, description.AcceptableValues,
                    ["HideFromREPOConfig", ..description.Tags]));
        }
    }
}