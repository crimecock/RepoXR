# RepoXR on R.E.P.O. 0.5.0

**English** · [Русский](repo-0.5.0.ru.md)

Unofficial port of [RepoXR](https://github.com/DaXcess/RepoXR) 1.2.3 (DaXcess) to R.E.P.O. 0.5.0, the Unity 6
update. Branch `repo-0.5.0` of [crimecock/RepoXR](https://github.com/crimecock/RepoXR). Problems with this port go to
[this fork's issues](https://github.com/crimecock/RepoXR/issues), not to DaXcess.

## Status

| Where | State |
|---|---|
| Quest 3, standalone ([repo-quest-vr](https://github.com/crimecock/repo-quest-vr), branch `repo-0.5.0`) | The same RepoXR source, built into the Quest APK. Plays on the headset since 1.0.7-qc21: menus, the singleplayer lobby and the levels (head tracking, pointers, grabbing), the VR settings |
| PC (BepInEx, this fork's release) | **Experimental, not tested in VR yet.** It compiles against the PC game's 0.5.0 assemblies and RepoXR's patches apply to the same game code the Quest build runs, but nobody has started it on Windows with a headset. The risks are listed below |

Multiplayer, vehicles and weapons have not been checked anywhere yet.

## Installing on PC

1. Install [BepInExPack](https://thunderstore.io/c/repo/p/BepInEx/BepInExPack/) and DaXcess's
   [FixPluginTypesSerialization](https://thunderstore.io/c/repo/p/DaXcess/FixPluginTypesSerialization/), the two
   dependencies of RepoXR 1.2.3 (with a mod manager like Gale, install RepoXR 1.2.3 once and they come with it).
2. Remove RepoXR 1.2.3 if it is installed (`BepInEx/plugins/RepoXR` and `BepInEx/patchers/RepoXR`).
3. Unpack `RepoXR-repo0.5.0-*.zip` from this fork's [releases](https://github.com/crimecock/RepoXR/releases) into the
   game folder (next to `REPO.exe`), or import it into Gale as a local mod.
4. Start the game with the headset connected (SteamVR, Oculus/Meta Link, Virtual Desktop...), as with RepoXR 1.2.3.

The BepInEx log (`BepInEx/LogOutput.log`) is what to attach to an issue. The port writes a few diagnostics there:
`[Scene]`, `[Camera]` (first 40 s of a scene), `[Input]`, `[UI]`, `[Grab]`, `[FPS]`.

## Starting in VR

RepoXR uses OpenXR, so it runs with any PC VR runtime; nothing about the game's own launch changes (start R.E.P.O.
from Steam as usual, with the headset connected and its PC software running). It picks the runtime in this order:

1. the runtime chosen in RepoXR's VR settings (the runtime selector; saved as `OpenXRRuntimeFile` in
   `BepInEx/config/io.daxcess.repoxr.cfg`);
2. the system's default OpenXR runtime;
3. any other one it finds: SteamVR, Virtual Desktop, Meta (Oculus).

If the game runs as administrator, only the system default can be used.

| Headset and connection | How |
|---|---|
| **SteamVR** (Valve Index, HTC Vive, Pico through Streaming Assistant, Quest through Steam Link or ALVR, any SteamVR headset) | Start SteamVR, then the game. If another runtime is the default, either pick SteamVR in RepoXR's runtime selector or make it the default: SteamVR → Settings → OpenXR → *Set SteamVR as OpenXR Runtime* |
| **Meta Quest through Link or Air Link** (Meta Quest Link app) | Meta Quest Link app → Settings → General → *OpenXR Runtime*: set Meta Quest Link as active. Connect Link, then start the game from the PC |
| **Virtual Desktop** (Quest, Pico) | Its own runtime (VDXR) is found automatically: start Virtual Desktop, then the game from its desktop. In SteamVR mode it goes through SteamVR instead |
| **Other OpenXR runtimes** (Pico Connect, Windows Mixed Reality, ...) | Make the runtime the system default, or pick it in RepoXR's runtime selector |
| **Without VR** | Launch option `--disable-vr`, or `DisableVR = true` in the config. You still see VR players' hands |

Launch options go in Steam → R.E.P.O. → Properties → *Launch options*, e.g. `-force-d3d11` (see the risks below).

## Risks on PC

| What | Why it may break | If it does |
|---|---|---|
| OpenXR runtime libraries | The release carries RepoXR 1.2.3's `UnityOpenXR.dll`, `openxr_loader.dll` and the managed XR packages, built for Unity 2022. Unity 6 may refuse them or behave differently | The log shows `[RepoXR] Loading VR...` failing. Newer OpenXR plugin binaries (built with Unity 6) would be needed |
| Graphics API | Unity 6 can run the game on Direct3D 12; Unity's OpenXR plugin of RepoXR 1.2.3's time is made for Direct3D 11 | Try the launch option `-force-d3d11` |
| FixPluginTypesSerialization | It hooks Unity's native player to load RepoXR's asset bundle components; it was made for Unity 2022 | Missing VR UI, errors about scripts in `repoxrassets` |
| RepoXR's asset bundle and Addressables | Built with Unity 2022. Fonts are handled in code (below); its shaders and the Addressables 1.x catalog under 0.5.0's Addressables 2 are untested on PC | Pink or missing VR UI, errors loading `catalog_0.1.json` |

## What changed in 0.5.0 and what the port does

Each fix is a commit on `repo-0.5.0`; the patch numbers are those of
[repo-quest-vr's `patches/repoxr`](https://github.com/crimecock/repo-quest-vr/tree/repo-0.5.0/patches/repoxr).

| In R.E.P.O. 0.5.0 | Effect on RepoXR 1.2.3 | Fix |
|---|---|---|
| `InputKey.Confirm` → `UIClick` and other renames, new overloads (`CameraShake.Shake`, `CameraAim.AimTargetSoftSet`, `InputToggleGet`, `UIMouseGetLocalPositionWithinRectTransform`), one text prompt page, the password field as a `MenuTextInput`, the tumble-climb rotation in `ClimbTargetUpdate` | 16 patch classes failed to apply or compile | 0009 |
| Menus click through `InputKey.UIClick` | Buttons did not click | 0009: `UIClick` answered with the pointer's trigger |
| Gamepad support: menus switch to keyboard navigation and back only when the mouse moves | In VR, once in navigation mode, pointing selected nothing | 0010: pointer mode kept while the VR rays exist |
| `SemiFunc.UIMouseHover` sizes the hover area by `lossyScale` | On the world-space menu canvas (scale 0.03) the hover area shrank to a speck | 0011: the scale relative to the menu page |
| The scene's EventSystem runs a `StandaloneInputModule` | XRI's rays only see UI through `XRUIInputModule` on the current EventSystem | 0012: one EventSystem, only `XRUIInputModule` on |
| XRI's UI raycast reports nothing under Unity 6 | The pointer selected nothing | 0014: the pointer is intersected with the world-space canvases by hand; XRI's result wins when it has one. 0013: rays reach 100 m (the main menu canvas is 20-25 m away) |
| Menu pages are no longer at the canvas origin | The pointer and the element positions were compared in different spaces | 0015 |
| The first main menu blocks input until its intro animation | RepoXR skips the intro: nothing could be clicked | 0017 |
| Six settings tabs, a new *Language* tab | RepoXR's VR Settings button lay on top of it | 0018: below *Language*, *Back* below it |
| Unity 6's TextMeshPro | The bundle's texts (Unity 2022 TMP) had no usable font | 0018, 0023, 0025: the game's menu font, also before each scene's VR setup |
| Item Unequip Auto Hold: the toggle-grab release moved into `PhysGrabber.GrabInputActiveGet` | `ForceHoldPatch` lost its anchor and edited the grab cooldown instead: **grabbing stayed disabled** | 0019: the patch targets `GrabInputActiveGet` and throws if its anchor is gone |
| New input keys: `VehicleAccelerate/Brake/DriftLeft/DriftRight/Exit/Grab`, `PushPull`, `ExpressionWheel`, UI navigation | RepoXR's VR action map has none: an exception on every lookup, the keyboard binding as fallback | 0019: looked up once; vehicle keys on the movement stick, `VehicleGrab` on Grab (toggle, as in the game) |
| `InputManager.VehicleInputAllowed` needs the game's own `Player` action map | Vehicles could ignore the stick | 0019 |
| Gun aim from the camera (`ItemGun.AimTargetUpdate`); `ObjectTurningRelease`, `ForceGrabPhysObject` | RepoXR holds objects relative to the hand: guns pointed elsewhere, rotated objects jumped | 0020: from the hand |
| Scene handoff: the next Main scene loads while the Reload scene is still active, which is unloaded afterwards | RepoXR's per-scene root objects (VR overlay canvas, network system) went away with the Reload scene: no VR HUD, no pause menu, no pointers in the lobby | 0024: moved into the loaded scene |
| The Reload scene has its own camera tagged MainCamera | `Camera.main` picked it | 0026: the loaded scene's own UI and camera; the Reload camera untagged |
| RepoXR's trackers (`TrackedPoseDriver`) and XRI controllers share their XR actions and disable them when they go away | In the handoff the Reload scene's camera tracker went away after the new camera's had started: **the view stopped following the head** in the lobby and every level (the tutorial loads without a handoff) | 0028: disabled actions are enabled again before the next input update |

Also in the branch: Dobutsu's settings from repo-quest-vr (0001-0008: Head Bob, Post Processing, other players'
flashlight shadows, focus sphere fix; Pixel Lights and Lamp Lighting for the Quest), an FPS counter (VR Settings →
Rendering), and diagnostics (0013, 0016, 0018, 0027).

## PC and Quest differences

`Plugin.IsQuest` (Android) keeps the Quest-only parts out of the PC build:

- Lamp Lighting is not created on PC: it needs the Quest build's shaders. Pixel Lights is not applied. Both, and the
  three Level Culling settings the Quest build's tools read, are hidden from the VR settings.
- PC uses RepoXR's defaults: Head Bob 25%, post-processing on, other players' flashlight shadows on, single-pass
  rendering off.
- PC keeps RepoXR's controller bindings. The Quest port moves the pause menu to the left menu button and mutes with X.

## Building

As RepoXR 1.2.3 ([COMPILING.md](../COMPILING.md)), against the assemblies of R.E.P.O. 0.5.0 (Unity 6000.3.17f1).

## Credits

RepoXR by [DaXcess](https://github.com/DaXcess/RepoXR) (GPL-3.0). Settings 0001-0008 by
[Dobutsu](https://github.com/Dobutsu/repo-quest-vr). The 0.5.0 port by crimecock, written with Claude.
