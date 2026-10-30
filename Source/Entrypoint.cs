using HarmonyLib;
using RepoXR.Assets;
using RepoXR.Input;
using RepoXR.Managers;
using RepoXR.Networking;
using RepoXR.Patches;
using RepoXR.Player.Camera;
using RepoXR.UI;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.UI;

namespace RepoXR;

[RepoXRPatch]
internal static class Entrypoint
{
    /// <summary>
    /// The default setup for VR for every scene
    /// </summary>
    internal static void SetupDefaultSceneVR(Scene scene)
    {
        // Quest port, R.E.P.O. 0.5.0 scene handoff: the next Main scene loads while the Reload scene is still loaded,
        // and the Reload scene has a camera tagged MainCamera of its own. Camera.main and GameObject.Find picked the
        // Reload scene's camera: the head tracking went onto a camera about to be unloaded, while the new scene's
        // camera kept RepoXR's counter-rotation without the head pose, and the view turned opposite to the head (the
        // singleplayer lobby and every level reached through it; the tutorial loads without a handoff). Everything
        // is now looked up in the scene that was loaded
        var ui = SceneRoot(scene, "UI");
        if (!ui)
        {
            Logger.LogInfo($"[Scene] {scene.name}: no game UI, no VR scene setup");
            return;
        }

        // Quest port: before any of RepoXR's prefabs is instantiated in this scene
        GameFonts.FixBundlePrefabs();

        // We grab all these references manually as most of the instances aren't set yet
        // Since most of them run in the "Start" lifetime function

        // Disable blocking UI
        ui!.transform.Find("UI/Canvas").GetComponent<Canvas>().enabled = false;

        var canvas = ui.transform.Find("HUD/HUD Canvas");
        var fade = canvas.Find("Fade");
        var video = canvas.Find("Render Texture Video");
        var loading = canvas.Find("Loading");
        var moon = canvas.Find("Moon UI");
        var splash = canvas.Find("Splash Screen");
        var result = canvas.Find("Result Screen");

        // The overlay camera is always in the same position in the hierarchy, in every scene
        var overlayCamera = canvas.parent.Find("Camera Overlay").GetComponent<Camera>();
        var mainCamera = MainCameraIn(scene);
        Logger.LogInfo($"[Scene] {scene.name}: VR camera {mainCamera.name} ({mainCamera.gameObject.scene.name})");

        // Add tracking to camera
        mainCamera.gameObject.AddComponent<VRCameraTracker>();

        // Parent overlay to main camera
        overlayCamera.transform.SetParent(mainCamera.transform, false);
        overlayCamera.transform.localPosition = Vector3.zero;

        overlayCamera.depth = 2;
        overlayCamera.farClipPlane = 1000;
        overlayCamera.orthographic = false;
        overlayCamera.clearFlags = CameraClearFlags.Depth;
        overlayCamera.targetTexture = null;
        overlayCamera.nearClipPlane = 0.01f;

        // Disable post-processing layer on UI camera (it's sort of broken)
        Object.Destroy(overlayCamera.GetComponent<PostProcessLayer>());

        // Make sure main camera renders to VR
        mainCamera.targetTexture = null;

        // Create blocking overlay (fade + static video)
        var overlayCanvas = new GameObject("VR Overlay Canvas") { layer = 5 }.AddComponent<Canvas>();
        overlayCanvas.renderMode = RenderMode.ScreenSpaceCamera;
        overlayCanvas.worldCamera = overlayCamera;
        overlayCanvas.sortingOrder = 5; // Put a little higher up the order so it renders on top

        fade.SetParent(overlayCanvas.transform, false);
        video.SetParent(overlayCanvas.transform, false);

        // Replace original material since that one has some transparency issues
        video.GetComponent<RawImage>().material = AssetCollection.VideoOverlay;

        // Make sure the components on the overlay fill the entire screen
        overlayCanvas.transform.GetComponentsInChildren<RectTransform>(true).Do(rect =>
        {
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
        });

        // Create loading canvas
        var loadingCanvas = new GameObject("VR Loading Canvas") { layer = 5 }.AddComponent<Canvas>();
        loadingCanvas.renderMode = RenderMode.WorldSpace;
        loadingCanvas.sortingOrder = 6; // Loading canvas needs to render on top of literally everything
        loadingCanvas.transform.localPosition = Vector3.zero;
        loadingCanvas.transform.localEulerAngles = Vector3.zero;
        loadingCanvas.transform.localScale = Vector3.one * 0.01f;
        loadingCanvas.transform.SetParent(mainCamera.transform.parent, false);
        loadingCanvas.gameObject.AddComponent<UI.LoadingUI>();
        loadingCanvas.GetComponent<RectTransform>().sizeDelta = new Vector2(720, 400); // For masking

        loading.SetParent(loadingCanvas.transform, false);

        // Moon UI stuff
        var moonMask = new GameObject("Moon Mask")
        {
            transform =
            {
                parent = loadingCanvas.transform,
                localScale = Vector3.one,
                localPosition = Vector3.zero,
                localRotation = Quaternion.identity
            }
        }.AddComponent<RectMask2D>().GetComponent<RectTransform>();
        moonMask.sizeDelta = new Vector2(800, 550);

        moon.SetParent(moonMask.transform, false);
        moon.localScale = Vector3.one * 0.8f;

        // Result screen stuff (Cosmetic tokens)
        var resultMask = new GameObject("Result Mask")
        {
            transform =
            {
                parent = loadingCanvas.transform,
                localScale = Vector3.one,
                localPosition = Vector3.zero,
                localRotation = Quaternion.identity
            }
        }.AddComponent<RectMask2D>().GetComponent<RectTransform>();
        resultMask.sizeDelta = new Vector2(720, 405);

        result.SetParent(resultMask.transform, false);

        splash.SetParent(loadingCanvas.transform, false);
        splash.SetAsFirstSibling(); // Prevent obscuring the loading UI

        // Create custom camera (if enabled)
        if (Plugin.Config.CustomCamera.Value)
            Object.Instantiate(AssetCollection.CustomCamera, mainCamera.transform.parent);

        // Create haptic feedback manager
        new GameObject("Haptic Manager").AddComponent<HapticManager>();

        // Create persistent data manager
        new GameObject("Data Manager").AddComponent<DataManager>();
    }

    internal static bool SceneRootExists(Scene scene, string name) => SceneRoot(scene, name);

    private static GameObject? SceneRoot(Scene scene, string name)
    {
        foreach (var root in scene.GetRootGameObjects())
            if (root.name == name)
                return root;
        return null;
    }

    /// <summary>The scene's own camera tagged MainCamera (an enabled one first), else Camera.main</summary>
    private static Camera MainCameraIn(Scene scene)
    {
        Camera? any = null;
        foreach (var root in scene.GetRootGameObjects())
        foreach (var camera in root.GetComponentsInChildren<Camera>(true))
        {
            if (!camera.CompareTag("MainCamera"))
                continue;
            if (camera.isActiveAndEnabled)
                return camera;
            any ??= camera;
        }

        return any ? any! : Camera.main!;
    }

    /// <summary>
    /// <see cref="GameDirector"/> is always present in the `Main` scene, so we use it as a base entrypoint
    /// </summary>
    [HarmonyPatch(typeof(GameDirector), nameof(GameDirector.Start))]
    [HarmonyPostfix]
    private static void OnStartup(GameDirector __instance)
    {
        VRInputSystem.Instance.ActivateInput();

        if (RunManager.instance.levelCurrent == RunManager.instance.levelMainMenu ||
            RunManager.instance.levelCurrent == RunManager.instance.levelSplashScreen)
            OnStartupMainMenu();

        // We have to do some magic for the Lobby Menu level because of ✨late join✨
    }

    /// <summary>
    /// Special custom entrypoint since we might swap levels shortly after GameDirector startup (Late join)
    /// </summary>
    [HarmonyPatch(typeof(LobbyMenuOpen), nameof(LobbyMenuOpen.Awake))]
    [HarmonyPostfix]
    private static void OnStartLobbyMenu()
    {
        if (RunManager.instance.levelCurrent == RunManager.instance.levelLobbyMenu)
            OnStartupMainMenu();
    }

    /// <summary>
    /// Use the Truck Start Room module to detect if we are in the actual game
    /// </summary>
    [HarmonyPatch(typeof(StartRoom), nameof(StartRoom.Start))]
    [HarmonyPostfix]
    private static void OnStartTruck(StartRoom __instance)
    {
        // The menu levels also have the truck so we should just ignore them
        if (__instance.name is "Start Room - Main Menu(Clone)" or "Start Room - Lobby Menu(Clone)"
            or "Start Room - Splash Screen(Clone)")
            return;

        OnStartupInGame();
    }

    [HarmonyPatch(typeof(SplashScreen), nameof(SplashScreen.Start))]
    [HarmonyPostfix]
    private static void OnStartupSplashScreen()
    {
        UI.LoadingUI.instance.ResetPosition();
    }

    private static void OnStartupMainMenu()
    {
        HUDCanvas.instance.gameObject.AddComponent<MainMenu>();
        DataManager.instance.ResetData();
    }

    private static void OnStartupInGame()
    {
        GameFonts.FixBundlePrefabs();
        GameDirector.instance.gameObject.AddComponent<VRSession>();
    }
}

[RepoXRPatch(RepoXRPatchTarget.Universal)]
internal static class UniversalEntrypoint
{
    private static bool hasShownErrorMessage;
    
    public static void OnSceneLoad(Scene scene, LoadSceneMode mode)
    {
        // R.E.P.O. 0.5.0 hands scenes over: the next Main scene loads while the Reload scene is still the active one
        // (ReloadScene.Handoff*), and the Reload scene is unloaded afterwards. The objects created here at the root
        // (the VR overlay canvas, the network system...) land in the active scene and went away with it: in the
        // singleplayer lobby GameHud found no overlay canvas and stopped, the game's HUD canvas stayed a giant screen
        // around the player (the "mirrored" view), with no pause menu and no pointers. They are moved into the scene
        // that was loaded
        var active = SceneManager.GetActiveScene();
        Logger.LogInfo($"[Scene] {scene.name} loaded ({mode}), active scene {active.name}");

        // The Reload scene's own camera is tagged MainCamera too and is still loaded while the next Main scene starts:
        // much of RepoXR takes Camera.main in Awake/Start (camera position, loading UI, pause menu, crosshair)
        if (Entrypoint.SceneRootExists(scene, "UI"))
            UntagOtherMainCameras(scene);
        var before = active == scene ? null : new HashSet<GameObject>(active.GetRootGameObjects());

        try
        {
            if (Plugin.Flags.HasFlag(Flags.VR))
                Entrypoint.SetupDefaultSceneVR(scene);

            SetupDefaultSceneUniversal();
        }
        finally
        {
            if (before != null)
                MoveNewRoots(active, before, scene, mode);
        }
    }

    private static void UntagOtherMainCameras(Scene scene)
    {
        for (var i = 0; i < SceneManager.sceneCount; i++)
        {
            var other = SceneManager.GetSceneAt(i);
            if (other == scene || !other.isLoaded)
                continue;

            foreach (var root in other.GetRootGameObjects())
            foreach (var camera in root.GetComponentsInChildren<Camera>(true))
            {
                if (!camera.CompareTag("MainCamera"))
                    continue;

                camera.tag = "Untagged";
                Logger.LogInfo($"[Scene] {camera.name} in {other.name} no longer counts as the main camera");
            }
        }
    }

    private static void MoveNewRoots(Scene active, HashSet<GameObject> before, Scene scene, LoadSceneMode mode)
    {
        var moved = 0;
        foreach (var root in active.GetRootGameObjects())
        {
            if (before.Contains(root))
                continue;

            SceneManager.MoveGameObjectToScene(root, scene);
            moved++;
        }

        Logger.LogInfo($"[Scene] {scene.name} loaded ({mode}) while {active.name} was active: {moved} RepoXR objects moved into it");
    }

    /// <summary>
    /// The default setup for every scene (including for non-vr players)
    /// </summary>
    private static void SetupDefaultSceneUniversal()
    {
        new GameObject("RepoXR Network System").AddComponent<NetworkSystem>();

        ShowVRFailedWarning();

#if DEBUG
        ShowEarlyAccessWarning();
#endif
    }
    
    private static void ShowVRFailedWarning()
    {
        if (!Plugin.Flags.HasFlag(Flags.StartupFailed) ||
            hasShownErrorMessage || RunManager.instance.levelCurrent != RunManager.instance.levelMainMenu)
            return;

        hasShownErrorMessage = true;
        MenuManager.instance.PagePopUpScheduled("VR Startup Failed", Color.red,
            "RepoXR tried to launch the game in VR, however an error occured during initialization.\n\nYou will still be able to play the game and make use of this mod's features, however you will not be in VR.\n\nYou can disable VR in the settings if you want to suppress this error in the future.",
            "Alright fam",
            true);
    }

#if DEBUG
    private static bool earlyAccessWarningShown;

    private static void ShowEarlyAccessWarning()
    {
        if (earlyAccessWarningShown || !VRSession.InVR)
            return;

        earlyAccessWarningShown = true;
        MenuManager.instance.PagePopUpScheduled("VR Development Build", Color.red,
            "You are using a development build of RepoXR. This build is not finished and might contain bugs or other unforeseen \"features\".\n\nEnter with caution.",
            "Let me play!",
            true);
    }
#endif
}