# RepoXR on R.E.P.O. 0.5.0

**English** · [Русский](repo-0.5.0.ru.md)

Unofficial port of [RepoXR](https://github.com/DaXcess/RepoXR) 1.2.3 (DaXcess) to R.E.P.O. 0.5.0, the Unity 6
update. Branch `repo-0.5.0` of [crimecock/RepoXR](https://github.com/crimecock/RepoXR). Problems with this port go to
[this fork's issues](https://github.com/crimecock/RepoXR/issues), not to DaXcess.

## Status

| Where | State |
|---|---|
| Quest 3, standalone ([repo-quest-vr](https://github.com/crimecock/repo-quest-vr), branch `repo-0.5.0`) | The same RepoXR source, built into the Quest APK. Plays on the headset since 1.0.7-qc21: menus, the singleplayer lobby and the levels (head tracking, pointers, grabbing), the VR settings |
| PC (BepInEx, this fork's release) | **Plays in VR on Windows** (alpha 4, tested 30 October 2026 with SteamVR and Virtual Desktop, Quest controllers): menus, the lobby, levels and the shop through the scene handoff, head tracking, pointers, grabbing (see below), guns, the whip, tumble climbing, the main menu's news popup, the settings menus and the VR controller icons, multiplayer with other players. The risks are listed below |

Multiplayer with other players works on PC and on the Quest; riding the vehicles works on PC. The settings menus and the controller icons work since alpha 4, the shop mirror since alpha 5 (hidden with single-pass rendering). The remaining known issues are in the [README](../README.md#known-issues--известные-проблемы). On the Quest, the same code comes with 1.0.7-qc27 (patches 0030–0032).

### What alpha 3 tested, and how

The internal test builds (not released) had the game's own tester commands on the F keys and in the chat: every
item on the floor, every gun, the snow bike and golf cart, Pony, the shop, god mode. Checked with them on PC: guns
fire from the hand; the whip cracks (73 cracks in one run, `[Whip] crack gesture: Strike`), including from over the
shoulder; level → shop no longer hangs; the news popup reads normally; scene handoffs log no errors.

## Installing on PC

You need R.E.P.O. 0.5.0 from Steam on Windows and a PC VR headset (see [Starting in VR](#starting-in-vr)). Get
`RepoXR-repo0.5.0-*.zip` from this fork's [releases](https://github.com/crimecock/RepoXR/releases).

**All in one (simplest):** `RepoXR-repo0.5.0-*-all-in-one.zip` has BepInEx 5.4.23 (BepInExPack),
FixPluginTypesSerialization 1.1.1 and this RepoXR. Unpack it into the game folder, next to `REPO.exe` (Steam → R.E.P.O.
→ Properties → Installed Files → Browse), and start the game from Steam with the headset connected; nothing else to
install. If another BepInEx or RepoXR 1.2.3 is already there, delete `BepInEx/plugins/RepoXR` and
`BepInEx/patchers/RepoXR` first. To uninstall, delete `winhttp.dll`, `doorstop_config.ini`, `.doorstop_version` and the
`BepInEx` folder. The licenses of what it contains are in its `RepoXR-README.txt`.

**With Gale** (mod manager; official downloads: [GitHub releases](https://github.com/Kesomannen/gale/releases),
[Thunderstore](https://thunderstore.io/c/lethal-company/p/Kesomannen/GaleModManager/),
[Hexium](https://hexium.gg/mod-manager); galemodmanager.com is not official):

1. Pick R.E.P.O. and create a profile.
2. Install **RepoXR** by DaXcess: BepInExPack and FixPluginTypesSerialization come with it.
3. Disable (or remove) DaXcess's RepoXR, keep its two dependencies.
4. Import the zip as a local mod (Gale has local mod imports in its import menu). If you cannot find it, open the
   profile's folder and unpack the zip into it, so that `BepInEx/plugins/RepoXR` and `BepInEx/patchers/RepoXR` appear.
5. Only one RepoXR may be enabled: both use the same plugin ID. Start the game with Gale's *Launch game*.

**By hand:**

1. Unpack [BepInExPack](https://thunderstore.io/c/repo/p/BepInEx/BepInExPack/) and DaXcess's
   [FixPluginTypesSerialization](https://thunderstore.io/c/repo/p/DaXcess/FixPluginTypesSerialization/) into the game
   folder, next to `REPO.exe` (Steam → R.E.P.O. → Properties → Installed Files → Browse).
2. Start the game once without the headset and close it.
3. Remove RepoXR 1.2.3 if it is installed (`BepInEx/plugins/RepoXR` and `BepInEx/patchers/RepoXR`), then unpack the
   zip into the game folder.
4. Start the game from Steam with the headset connected.

**Updating** (RepoXR already installed): unpack the newest all-in-one zip into the game folder again and replace the
files; BepInEx and your settings (`BepInEx/config/io.daxcess.repoxr.cfg`) stay. With only the RepoXR zip, replace
`BepInEx/plugins/RepoXR` and `BepInEx/patchers/RepoXR`. With Gale, import the new zip (or remove RepoXR and add the new
one). The first RepoXR line in `BepInEx/LogOutput.log` names the version.

**What to check:** the view follows your head; there are rays from your hands and menu buttons click; grabbing works
in the lobby; the VR Settings button (below the *Language* tab in Settings) opens.

The BepInEx log (`BepInEx/LogOutput.log`; Gale opens it from the profile) is what to attach to an issue. Look for
`unofficial port to R.E.P.O. v0.5.0` and `OpenXR runtime being used: ...` in it; the port also writes a few
diagnostics: `[Scene]`, `[Camera]` (first 40 s of a scene), `[Input]`, `[UI]`, `[Grab]`, `[FPS]`. To watch the log
while playing, set `Enabled = true` under `[Logging.Console]` in `BepInEx/config/BepInEx.cfg`.

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
| OpenXR runtime libraries | Since alpha 2 they are Unity 6 ones: OpenXR plugin 1.14.3 (native `UnityOpenXR.dll` and `openxr_loader.dll` for Windows x64 from the package), XR Management 4.5.0, XR Interaction Toolkit 2.6.4, XR Core Utils 2.6.0, Legacy Input Helpers 2.1.13 — the versions the Quest build runs, compiled for Windows outside the Unity editor (below) | **Works** in the first PC test: SteamVR/OpenXR 2.18.4, Oculus Touch controllers. If another runtime fails, the log shows `[RepoXR] Loading VR...` failing or a `MissingMethodException` / `TypeLoadException` naming a `Unity.XR` assembly |
| Graphics API | Unity 6 can run the game on Direct3D 12 (the game ships `D3D12Core.dll`); RepoXR was only ever used on Direct3D 11 | Try the launch option `-force-d3d11` |
| FixPluginTypesSerialization | It hooks Unity's native player to load RepoXR's asset bundle components; its README lists Unity up to 2023.2 | **Works on Unity 6000.3.17** in the first PC test: it finds its seven functions in Unity's symbols ("Running under Unity v6000.3.17", "Function offset ..."), and the VR UI loads |
| RepoXR's asset bundle | Built with Unity 2022. Fonts are handled in code (below); its shaders are untested under Unity 6 on PC | Pink or missing VR UI |
| RepoXR's translations (Addressables) | 0.5.0's Addressables 2 only read binary catalogs unless built with `ENABLE_JSON_CATALOG`; the Quest build is, the PC game is not, so `catalog_0.1.json` is refused on PC | Known: the log warns once, and the tutorial keeps the game's own texts for the inventory, map and expression pages instead of RepoXR's VR ones. A binary catalog built with Unity 6 would bring them back |

## What changed in 0.5.0 and what the port does

Each fix is one of the patches of
[repo-quest-vr's `patches/repoxr`](https://github.com/crimecock/repo-quest-vr/tree/repo-0.5.0/patches/repoxr), numbered as there; all of them
are in the code of `repo-0.5.0`.

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
| R.E.P.O. 0.5.0's whip cracks and flings by gestures: `ItemMeleeWhip.InputTick` reads the look delta (`InputManager.GrabCursorLookGet`: mouse or right stick) and the camera's aim frame | In VR the look delta is always zero: the whip could only be dangled | 0029 (`ItemMeleeWhipPatches`): while the whip reads its input, the look delta is the hand's angular velocity in the tracking space (around the vertical, and around the head's left-right axis, so a swing from over the shoulder does not flip), scaled by *Whip Sideways Sensitivity* (0.5) and *Whip Crack Sensitivity* (1); the aim frame is at the hand, along it, upright |
| RepoXR skips `CameraAim.Update`, which also ends the aim targets the game forces on the camera (`AimTargetSet`) | Once anything set one, `CameraAim.AimTargetActive` stayed true for good: the whip's gestures (`ItemMeleeWhip.GestureCanUse`) and the grab cursor look stayed off | 0029: the skipped update still counts the aim target, soft aim target and aim stop timers down; the camera still follows the headset |
| Scene handoff: the next scene's VR loading UI wakes up before the old one is destroyed | The old one's `OnDestroy` cleared the static instance; `ResultScreenPatches` threw on every frame and **level → shop hung on the result screen** | 0029: `LoadingUI` clears only its own instance; the result screen and moon UI patches check for it |
| The main menu's news popup grows from nothing (`MenuPageMain.RunMarketingPopupCurves`, a `Vector2` scale with z = 0) right after its texts are set | Its words were drawn as solid bars in VR | 0029: the popups' scale keeps z = 1 (`FlatScalePatches`), and the news popup's two texts are built again once it has grown (`MarketingPopupPatches`) |
| RepoXR's asset bundle was built with Unity 2022's TextMeshPro: the icon sprite asset keeps its glyphs in `m_SpriteGlyphTable` and its material in `material` | Unity 6's TextMeshPro reads `m_GlyphTable`/`m_Material`: no glyphs, so no sprite name was found (`?`), and no material, so found sprites drew nothing | 0030 (`Utils.RepairSpriteAsset`): the glyphs are rebuilt from the sheet's 100 px grid, the game's own sprite material is copied onto RepoXR's sheet, the lookup tables are refreshed |
| Text tags (`[move]`, `[grab]` …) moved to `InputGlyphs.tagDictionary`; `InputManager.tagDictionary` is never filled | RepoXR's replacement found no tags: `[move]` in the tutorial and prompts | 0030 (`InputPatches.NoUnderlinePatch`): the tags come from `InputGlyphs` |
| `MenuTwoOptions.OptionBoxX` compares the two buttons' world x; new label fields `option1TextMesh`/`option2TextMesh` | On a menu that hangs in 3D the comparison flips (box on the wrong option); RepoXR's toggles (0.4.4 prefabs) kept placeholder labels | 0030 (`MenuTwoOptionsPatches`): positions in canvas space; labels from the buttons' own texts |
| `MenuButton.ResizeToText` puts a button back at the place it had on `Awake`, and runs again when the localized text arrives | The moved *Back* button went back under *VR Settings* | 0030 (`MenuPatches`): the moved buttons' original position is their new place |
| `MenuTwoOptions.StartFetch` calls the toggle's fetch event only with the new `customFetch` flag; `OptionBoxX` returns the x of the options on the game's own toggles (143.5/218.5) | RepoXR's toggles (0.4.4 prefabs) showed one of the game's settings, and their box sat half or wholly outside on the right | 0031 (`MenuTwoOptionsPatches`): `customFetch` is set for toggles with a fetch event; without the game's option buttons the box goes to the centre of the toggle's own buttons |
| New shop mirror (`MirrorReflection`): an extra camera renders the reflection in `Camera.onPreCull`, the shader samples it at the flipped screen position | The reflection camera rendered as a stereo camera, the XR eye matrices replaced its view: a mess in the headset | 0031 (`MirrorPatches`): for a headset camera the reflection camera renders mono, on each eye's pre-render, from the eye's reflected pose with the eye's projection (XR display render pass, skew flipped); hidden in single-pass and with *Shop Mirror* off |
| RepoXR's own text tags (`[MapGrabLeft]`...) live in `InputManager.tagDictionary`; RepoXR's VR tutorial texts exist in English only and do not load on PC | The tutorial showed `[MapGrabLeft]`; on PC the game's texts; on the Quest a non-English player would have got English | 0032 (`InputPatches`, `TutorialPatches`): both tag dictionaries are replaced; the VR texts only in English, built in on PC; the pages are checked by their text |
| Quest 3 Touch Plus controllers come through the XR input subsystem (`XRInputV1::Oculus::MetaQuestTouchPlusControllerOpenXR`), which no RepoXR control scheme names | Input worked, but PlayerInput paired nothing: the controls page said *DISCONNECTED* and could not rebind | 0032 (`VRInputSystem`): on the Quest the Meta Quest scheme also takes any pair of XR controllers, and the controls page falls back to it; *Render Mode* shows in VR Settings on the Quest |

Also in the branch: Dobutsu's settings from repo-quest-vr (0001-0008: Head Bob, Post Processing, other players'
flashlight shadows, focus sphere fix; Pixel Lights and Lamp Lighting for the Quest), an FPS counter (VR Settings →
Rendering), and diagnostics (0013, 0016, 0018, 0027).

## Grabbing: what broke and how it was fixed

Grabbing works on PC and on the Quest. In VR the grab beam comes from the hand, so every place where 0.5.0 aims a
grab from the camera or the mouse had to follow the hand.

| Patch | Where | What was wrong | Fix |
|---|---|---|---|
| 0019 `ForceHoldPatch` | `PhysGrabber.GrabInputActiveGet` | *Item Unequip Auto Hold* (new setting, on by default) moved the toggle-grab release here from `Update`. The patch's code search found nothing and its call landed in the grab cooldown at the end of `Update`: valid IL, but a float where a bool belonged, and **the beam never switched on** | Before `or; brtrue; ldsfld GameplayManager.instance; ldfld itemUnequipAutoHold` the grab-held value is replaced with `ShouldToggleGrabOff(grabHeld, overrideGrabTimer)`; `ThrowIfInvalid` if the anchor is missing. Found by the Quest build's anchor watcher, which reports every code search that finds nothing |
| 0020 | `PhysGrabber.ForceGrabPhysObject`, `ObjectTurningRelease` | Both took the camera's transform | The hand's: no jump after rotating an object |
| 0020 | `ItemGun.AimTargetUpdate` | Guns turned along camera → grab point | From the hand: the gun points along the grab beam |
| 0009 `HandBasedTumbleClimbPatch` | `PhysGrabber.ClimbTargetUpdate` | 0.5.0 moved the climb rotation here from `GrabStateClimb`; the old transpiler broke the end of `GrabStateClimb` | The climb stick follows the hand's rotation. Checked on PC |
| 0019 | `InputKey.VehicleGrab` | No VR action | RepoXR's Grab, a toggle as in the game |
| alpha 2 `NoGrabCursorDrift` | `InputManager.GrabCursorDriftGet` | 0.5.0's gamepad grab cursor drift (`PhysGrabber.GamepadGrabTargetGet`) moves the grab point after the view | 0 in VR: the grab point stays on the beam |
| 0018 | `[Grab]` in the log | — | What the VR input did for grabbing, for reports |

## PC and Quest differences

`Plugin.IsQuest` (Android) keeps the Quest-only parts out of the PC build:

- Lamp Lighting is not created on PC: it needs the Quest build's shaders. Pixel Lights is not applied. Both, and the
  three Level Culling settings the Quest build's tools read, are hidden from the VR settings.
- PC uses RepoXR's defaults: Head Bob 25%, post-processing on, other players' flashlight shadows on, single-pass
  rendering off.
- PC keeps RepoXR's controller bindings. The Quest port moves the pause menu to the left menu button and mutes with X.

## Building

As RepoXR 1.2.3 ([COMPILING.md](../COMPILING.md)), against the assemblies of R.E.P.O. 0.5.0 (Unity 6000.3.17f1).

The XR runtime libraries of the release: [`tools/xr-deps/build.py`](../tools/xr-deps/build.py) compiles the six XR
packages again for Windows from a Unity 6 project that has them and was built once for a player (repo-quest-vr's
Quest project is one). It takes the compiler arguments Unity used (`Library/Bee/artifacts/*P.dag/<assembly>.rsp`),
swaps the platform defines for `UNITY_STANDALONE_WIN` and the references for the PC game's `REPO_Data/Managed`, and
leaves out Unity's source generators (none of these packages needs them at runtime). `Unity.Mathematics` is not
shipped any more: the game has its own (1.3.2), and the XR packages are compiled against it. The native
`UnityOpenXR.dll` and `openxr_loader.dll` are the package's `Runtime/windows/x64` and `RuntimeLoaders/windows/x64`
files.

## Credits

RepoXR by [DaXcess](https://github.com/DaXcess/RepoXR) (GPL-3.0). Settings 0001-0008 by
[Dobutsu](https://github.com/Dobutsu/repo-quest-vr). The 0.5.0 port by crimecock, written with Claude.
