# 1.2.3-repo0.5.0-alpha.6 (unofficial fork)

**Changes**:
- Tutorial: RepoXR's VR texts (inventory, map, expressions) show on PC too: 0.5.0's Addressables cannot read
  RepoXR's JSON catalog there, so the same English texts are built in. They exist in English only, so in any other
  language the game's own translated texts stay (with the VR controller icons)
- RepoXR's own text tags (`[MapGrabLeft]`, `[MapGrabRight]`, ...) are replaced with controller icons again: they live
  in `InputManager.tagDictionary`, which alpha 4's tag fix skipped
- Quest: Quest 3 (Touch Plus) controllers, which none of RepoXR's control schemes names, use the Meta Quest scheme, so
  the controls page no longer says DISCONNECTED; *Render Mode* (single pass / multipass) in VR Settings → Rendering
- The shop mirror reads the render mode the game started with (changing it takes a restart)

# 1.2.3-repo0.5.0-alpha.5 (unofficial fork)

**Changes**:
- The shop mirror works in VR: the game's reflection camera rendered as a stereo camera, so the XR eye matrices
  replaced its reflected view. It now renders mono, once for each eye just before that eye renders, from the eye's
  reflected position with the eye's own (mirrored) projection
- *VR Settings → Rendering → Shop Mirror* (on by default) hides the mirror; with single-pass rendering it is always
  hidden, since both eyes would share one reflection texture
- Toggles in RepoXR's own menus (VR Settings, the VR controls' hold/toggle) show the selected option: 0.5.0 only calls
  their fetch event with its new `customFetch` flag, and puts the selection box where the game's own toggles have
  their options
- Tested on PC (SteamVR, Virtual Desktop): the mirror, the expression wheel (with the mirror), the VR settings toggles

# 1.2.3-repo0.5.0-alpha.4 (unofficial fork)

**Changes**:
- The VR controller icons show again (settings → controls, the tutorial, prompts): RepoXR's icon sprite asset was made
  with Unity 2022's TextMeshPro, whose glyph table, material and hash code Unity 6's TextMeshPro does not read; they
  are rebuilt when the asset bundle loads (`Utils.RepairSpriteAsset`)
- Text tags such as `[move]` and `[grab]` are replaced again: 0.5.0 keeps them in `InputGlyphs.tagDictionary`, and the
  old `InputManager.tagDictionary` is empty
- Toggles in the world-space menus highlight the selected option (`MenuTwoOptions.OptionBoxX` compares the buttons in
  canvas space), and RepoXR's own toggles show their labels
- The *VR Settings* button no longer lands on *Back* (the moved buttons keep their place when the localized text
  arrives and `MenuButton.ResizeToText` runs again)
- Tested on PC (SteamVR, Virtual Desktop): settings, VR settings, the controls page, the tutorial, riding the vehicles;
  everything from alpha 3 still works

# 1.2.3-repo0.5.0-alpha.3 (unofficial fork)

**Changes**:
- The whip works in VR: snap the controller back and forward to crack (also from over the shoulder), swing to fling
  what it has caught. Its gestures read the hand's angular velocity while you hold it, with an upright aim frame at the
  hand; VR Settings → Input → *Whip Sideways Sensitivity* (0.5) and *Whip Crack Sensitivity* (1)
- Camera aim targets expire again: RepoXR skips `CameraAim.Update`, and with it the timers that end the aim targets the
  game forces on the camera; `CameraAim.AimTargetActive` stayed true for good and kept the whip's gestures off
- Level → shop no longer hangs on the result screen: in the scene handoff the old VR loading UI cleared the new one's
  instance, and `ResultScreenPatches` threw on every frame
- The main menu's news popup reads normally: its texts are built again once it has grown, and the popups' scale keeps a
  depth of 1
- Docs: how to update an existing install; what was tested and how; the whip in the controls
- Tested on PC (SteamVR, Virtual Desktop): menus, lobby, levels, shop, grabbing, guns, whip, tumble climbing,
  multiplayer with other players (also works on the Quest)
- Known: visual glitches in the settings menus (toggles, sliders; they work) and `?` instead of the controller icons
  on the VR controls page

# 1.2.3-repo0.5.0-alpha.2 (unofficial fork)

**Changes**:
- Unity 6 XR runtime libraries instead of RepoXR 1.2.3's Unity 2022 ones: OpenXR plugin 1.14.3 (native plugin and
  loader for Windows x64), XR Management 4.5.0, XR Interaction Toolkit 2.6.4, XR Core Utils 2.6.0, Legacy Input
  Helpers 2.1.13, compiled for Windows by `tools/xr-deps/build.py`; the game's own Unity.Mathematics is used
- The ways to start in VR are documented (SteamVR, Meta Quest Link, Virtual Desktop, other OpenXR runtimes)
- `InputKey.ChatConfirm` (0.5.0's name for `Confirm`) uses RepoXR's "Confirm" action again
- The gamepad's new grab cursor drift (`InputManager.GrabCursorDriftGet`) is 0 in VR
- PC: R.E.P.O. 0.5.0's Addressables only read binary catalogs, so RepoXR's `catalog_0.1.json` does not load there;
  the VR tutorial texts then keep the game's own instead of failing
- An all-in-one zip for the game folder: BepInEx 5.4.23, FixPluginTypesSerialization 1.1.1 and RepoXR
- First test on PC: alpha 2 (all-in-one zip) starts in VR on Windows; the gameplay is being checked
- Checked on PC: grabbing and tumble climbing work (the climb is pulled where the hand points)
- Docs: how grabbing was fixed for 0.5.0 (README, docs/repo-0.5.0.md "Grabbing: what broke and how it was fixed")
- Known: the main menu's news popup draws its text as solid bars (PC and Quest); fixed in alpha 3

# 1.2.3-repo0.5.0-alpha.1 (unofficial fork)

Port to R.E.P.O. 0.5.0 (Unity 6000.3); details in [docs/repo-0.5.0.md](docs/repo-0.5.0.md).

**Changes**:
- Game API changes of 0.5.0 followed: renamed input keys (`UIClick`), new overloads, the new text prompt page, the
  climb rotation, vehicles and their keys, gun aim and grab release from the hand
- Menus: pointer mode kept, hover areas on the world-space canvas, `XRUIInputModule` on the scene's EventSystem, the
  pointer hits canvases by hand where XRI's UI raycast finds nothing, the first main menu unblocked, the VR Settings
  button moved below the new Language tab, the asset bundle's texts get the game's font
- Grabbing works again with *Item Unequip Auto Hold* (`ForceHoldPatch` follows `GrabInputActiveGet`)
- Scene handoff: RepoXR's root objects move into the loaded scene, the Reload scene's camera is no longer the main
  camera, and shared XR actions stay enabled (the view followed the head only in the tutorial)
- From repo-quest-vr: Head Bob, Post Processing and other players' flashlight shadows settings, an FPS counter; the
  Quest-only Pixel Lights, Lamp Lighting and Level Culling are hidden on PC
- Tested on a Quest 3 (standalone build); the PC build is not tested in VR yet

# 1.2.3

**Changes**:
- The verbose logging option has been renamed and will now also try to load debug symbols

**Fixes**:
- Fixed the map tool rotations not being restored after experiencing a camera override
- Fixed item slots still having a visible battery indicator during camera overrides
- Fixed staff items charge progress based on head rotation instead of hand rotation
- Fixed staff items not properly aligning with world up when not charged

# 1.2.2

**Hotfix**:
- Fixed not being able to unequip melee items without hurt colliders

# 1.2.1

**Changes:**
- Made clipping through the floor more difficult

**Fixes**:
- Fixed the grabber not properly grabbing items at their current position
- Fixed the map player count upgrade not being visible in VR
- Fixed some issues with the on-screen keyboard when renaming a save

# 1.2.0

**Additions**:
- Added support for new valuable interactions
- Added support for new level UI in the map tool
- Added support for the new walkie-talkie item
- Added support for the new scooters
- Added support for the new cosmetics system
- Added VR presence to Steam and Discord (configurable)

**Changes**:
- Left-handed mode now also swaps your controller bindings around
- Left- and right-handed mode now both have separated controller binding overrides
- Restructured mouse input patching so testing with UnityExplorer is easier
- Made some changes to the arms in multiplayer
- Reworked networking internals, should not affect gameplay
- Reworked compatible version checking

**Removals**:
- Removed F8 hotswapping completely
- Removed support for v0.3.x

**Fixes**:
- Smooth turn should feel a bit smoother now when not in the center of your play area
- Fixed opening the VR settings menu causing the screen to reset position

# 1.1.2

**Changes**:
- Disabled movement normalization (configurable in input settings)
- Hotswapping can now only be performed in the main menu, not in lobbies anymore

**Fixes**:
- Fixed LobbyImprovements VR prefix issue

# 1.1.1

**Additions**:
- Added a \[VR\] prefix to VR players in the lobby menu
- Added a warning popup if the host does not have the VR mod

**Fixes**:
- Fixed hotswapping breaking the menu lobby in some circumstances

# 1.1.0

## Detached Arms

You can now detach your arms from your body, giving you more control over your aim, not being constrained by the maximum length of the Semibot's arms anymore. You can change this setting at any time, no restarts or rejoins needed.

## Eye Tracking

RepoXR v1.1.0 adds support for eye tracking (for the three people that have it). 

#### Singleplayer

Players that use eye tracking will have enhanced immersion when it comes to "looking at" things. A few of the enemies in R.E.P.O. behave differently depending on if you're looking at them or not. This detection now factors in where you are looking with your eyes!

Discovering valuables, or your lost friends' heads will also make use of eye tracking. Just look at the items and the game will discover them without you having to move your head.

#### Multiplayer

When playing with other people, other people will see your pupils move based on your real eye movement. Looking down? People will see you look down. Looking straight through your friend's soul because they broke something? Yup, they'll see that!

#### Configuration

Eye tracking can be enabled and disabled mid-game, there's no need to restart. If your headset does not support eye tracking, it will be considered disabled (even when eye tracking is enabled in the config), so no need to change the settings if you don't have eye tracking.

## Hotswap

You can now swap between VR mode and flatscreen mode by pressing the F8 button on your keyboard while you are in the main menu or in the lobby menu. This works even when you are the host (though the lobby will reload for everybody).

**Additions**:
- Added eye tracking support
- Added an option to detach arms from body
- Added support for the new climbing mechanic
- Added support for spectating your death head
- Added support for the monster update
- Added hotswapping in the main menu (F8)

**Changes**:
- The ceiling eye now darkens the world except for where the eye is (no more cheating hihi)
- Removed the performance tab and replaced it with UI in the settings
- Replaced the valuable discover overlay with a new 3D graphic (supports custom colors)
- You now look at the enemy/object that killed you while the death animation plays (if possible)
- Slightly optimized the custom camera by adding a frame rate limiter (disabled by default)
- Optimized framerate by forcibly disabling ambient occlusion (20%-40% less render time)
- Renamed "Dynamic Smooth Speed" to "Analog Smooth Turn"
- Changed the minimum possible HUD height value to account for detached hands
- The map tool can now also be grabbed from behind your head (near the shoulders)
- You can now unbind controls at your leisure

**Removals**:
- Removed support for REPO v0.2.x

# 1.0.3

**Additions**:
- Added a keyboard shortcut (CTRL+C) to the lobby menu for easy copying of a steam lobby link
- Added support for late joining

**Removals**:
- Removed redundant checks during startup

# 1.0.2

**Additions**:
- Added keybind options for turning
- Added a new option for configuring the distance of the camera canvas in the gameplay settings

# 1.0.1

**Additions**:
- Added support for v0.2.1

**Bug fixes**:
- Fixed the small moons in the moon UI rotating weirdly
- Fixed a bug in HarmonyX that would generate invalid code during transpiling

**Removals**:
- Removed support for v0.2.0

# 1.0.0

**The VR mod has been released!!**

It took a bit, but after 3 months <sub><sup>_(started March 21st 2025)_</sup></sub> the R.E.P.O. VR mod has finally released!

No changelogs are necessary for this version, as it is the first version. Subsequent versions will contain a list of changes and new contributors.

### Verifying mod signature

RepoXR comes pre-packaged with a digital signature. You can use tools like GPG to verify the `RepoXR.dll.sig` and `RepoXR.Preload.dll.sig` signatures.

The public key which can be used to verify the file is [9422426F6125277B82CC477DCF78CC72F0FD5EAD (OpenPGP Key Server)](https://keys.openpgp.org/vks/v1/by-fingerprint/9422426F6125277B82CC477DCF78CC72F0FD5EAD).