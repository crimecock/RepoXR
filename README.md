> [!IMPORTANT]
> **This is an unofficial fork: RepoXR 1.2.3 ported to R.E.P.O. 0.5.0 (Unity 6).** Branch `repo-0.5.0`.
> The same source plays on a Quest 3 in the standalone [repo-quest-vr](https://github.com/crimecock/repo-quest-vr)
> build; the PC (BepInEx) release **plays in VR on Windows** (SteamVR, Virtual Desktop; Quest controllers): menus, levels,
> the shop and its mirror, grabbing, guns, the whip, tumble climbing, vehicles, the settings menus, multiplayer. What
> changed, how to install it and the risks: [docs/repo-0.5.0.md](docs/repo-0.5.0.md) · [по-русски](README.ru.md).
> Report problems with this fork [here](https://github.com/crimecock/RepoXR/issues), not to DaXcess.

## Credits · Благодарности

Thanks to the people whose work this port stands on · спасибо авторам, на чьей работе стоит этот порт:

- **[DaXcess](https://github.com/DaXcess)**: [RepoXR](https://github.com/DaXcess/RepoXR) and
  [RepoXR-Unity](https://github.com/DaXcess/RepoXR-Unity), the VR mod itself · сам VR-мод
  ([Thunderstore](https://thunderstore.io/c/repo/p/DaXcess/RepoXR)).
- **[Dobutsu](https://github.com/Dobutsu)**: [repo-quest-vr](https://github.com/Dobutsu/repo-quest-vr), the original
  standalone Quest port · оригинальный порт на Quest без ПК ([ko-fi](https://ko-fi.com/dobutsu)).
- **[BepInEx](https://github.com/BepInEx)**: [BepInEx](https://github.com/BepInEx/BepInEx) and
  [HarmonyX](https://github.com/BepInEx/HarmonyX).
- **[xiaoxiao921](https://github.com/xiaoxiao921)**: [FixPluginTypesSerialization](https://github.com/xiaoxiao921/FixPluginTypesSerialization)
  ([DaXcess's fork](https://github.com/DaXcess/FixPluginTypesSerialization) is in the all-in-one zip · в архиве
  «всё в одном» — форк DaXcess).
- Unity's XR packages (OpenXR, XR Interaction Toolkit) through the [needle-mirror](https://github.com/needle-mirror)
  package mirrors · XR-пакеты Unity через зеркала needle-mirror.
- 0.5.0 port by **[crimecock](https://github.com/crimecock)**, made with [Claude Code](https://claude.com/claude-code) ·
  перенос на 0.5.0 — crimecock.

## Install on PC (R.E.P.O. 0.5.0) · Установка на ПК

**English**

1. Download the newest `RepoXR-repo0.5.0-…-all-in-one.zip` from [Releases](https://github.com/crimecock/RepoXR/releases).
2. Open the game folder: Steam → R.E.P.O. → Properties → Installed Files → Browse (the folder with `REPO.exe`).
3. Unpack the whole archive there. It contains BepInEx, FixPluginTypesSerialization and RepoXR: nothing else to
   install. If RepoXR 1.2.3 was there before, delete `BepInEx/plugins/RepoXR` and `BepInEx/patchers/RepoXR` first.
4. Turn on the headset and its PC software: Meta Quest Link / Air Link, Virtual Desktop or SteamVR.
5. Start R.E.P.O. from Steam. If the headset stays black, add the launch option `-force-d3d11`.
6. Something wrong? Attach `BepInEx/LogOutput.log` to an [issue](https://github.com/crimecock/RepoXR/issues).

Other ways to install (Gale), how to start in VR and the risks:
[docs/repo-0.5.0.md](docs/repo-0.5.0.md).

**Updating (RepoXR already installed).** Download the newest `…-all-in-one.zip` and unpack it into the game folder
again, replacing the files. That is all: BepInEx, its config and your RepoXR settings
(`BepInEx/config/io.daxcess.repoxr.cfg`) stay. With only the RepoXR zip, replace the folders
`BepInEx/plugins/RepoXR` and `BepInEx/patchers/RepoXR` with the ones from the zip. With Gale, import the new zip into
your profile (or remove RepoXR there and add the new one). The first line of RepoXR in `BepInEx/LogOutput.log` names
the version you are running.

**Русский**

1. Скачайте самый новый `RepoXR-repo0.5.0-…-all-in-one.zip` из [релизов](https://github.com/crimecock/RepoXR/releases).
2. Откройте папку игры: Steam → R.E.P.O. → Свойства → Установленные файлы → Обзор (папка с `REPO.exe`).
3. Распакуйте туда весь архив. В нём BepInEx, FixPluginTypesSerialization и RepoXR — больше ничего ставить не нужно.
   Если раньше там стоял RepoXR 1.2.3, сначала удалите `BepInEx/plugins/RepoXR` и `BepInEx/patchers/RepoXR`.
4. Включите шлем и его программу на ПК: Meta Quest Link / Air Link, Virtual Desktop или SteamVR.
5. Запустите R.E.P.O. из Steam. Если в шлеме черно, добавьте параметр запуска `-force-d3d11`.
6. Что-то не так? Приложите `BepInEx/LogOutput.log` к [issue](https://github.com/crimecock/RepoXR/issues).

Другие способы установки (Gale), запуск в VR и риски:
[docs/repo-0.5.0.ru.md](docs/repo-0.5.0.ru.md).

**Обновление (RepoXR уже стоит).** Скачайте самый новый `…-all-in-one.zip` и снова распакуйте его в папку игры с
заменой файлов. Это всё: BepInEx, его конфиг и ваши настройки RepoXR (`BepInEx/config/io.daxcess.repoxr.cfg`)
сохранятся. Если ставили только архив RepoXR — замените папки `BepInEx/plugins/RepoXR` и `BepInEx/patchers/RepoXR`
на папки из архива. В Gale — импортируйте новый архив в профиль (или удалите там RepoXR и добавьте новый). Какая версия
запущена, видно по первой строке RepoXR в `BepInEx/LogOutput.log`.

## Status and what made it work · Состояние и что пришлось починить

| | PC (this fork) · ПК | Quest 3 standalone ([repo-quest-vr](https://github.com/crimecock/repo-quest-vr)) |
|---|---|---|
| Starts in VR · запуск в VR | ✅ SteamVR, Virtual Desktop; Quest controllers | ✅ |
| Menus, pointers · меню, лучи | ✅ | ✅ |
| Lobby, levels, shop, head tracking · лобби, уровни, магазин, трекинг головы | ✅ | ✅ |
| Grabbing · захват | ✅ | ✅ |
| Guns · пушки | ✅ | ✅ same code · тот же код |
| Whip: crack and fling by swinging the controller · хлыст: щелчок и бросок взмахом контроллера | ✅ alpha 3 | ✅ same code · тот же код |
| Tumble climb · лазание в кувырке | ✅ | ✅ same code · тот же код |
| Main menu news popup (bottom right) · новость в главном меню | ✅ alpha 3 | ✅ same code · тот же код |
| Vehicles · техника | ✅ | ✅ same code · тот же код |
| Settings menus, VR controls page · меню настроек, страница управления VR | ✅ alpha 4–5 | ✅ same code (1.0.7-qc24) · тот же код |
| Shop mirror · зеркало в магазине | ✅ alpha 5 (*VR Settings → Rendering → Shop Mirror*, on · вкл.) | ✅ hidden by default (single-pass shares one reflection between the eyes); works with single-pass off · по умолчанию скрыто (в single-pass одно отражение на оба глаза); работает с выключенным single-pass |
| Multiplayer with other players · мультиплеер с другими | ✅ | ✅ |

## Known issues · Известные проблемы

None right now: everything works on PC, and the Quest builds the same code. · Сейчас нет: на ПК всё работает, а
Quest собирает тот же код.

Tip · совет: the whip flings only once it has caught something; its sensitivity is in *VR Settings → Input → Whip
Sideways / Crack Sensitivity* · хлыст бросает, только когда что-то поймал; чувствительность — в *VR Settings → Input*.

Found something? Attach `BepInEx/LogOutput.log` to an [issue](https://github.com/crimecock/RepoXR/issues). ·
Нашли что-то? Приложите `BepInEx/LogOutput.log` к [issue](https://github.com/crimecock/RepoXR/issues).

**The fixes that mattered · Главные исправления**

1. **The view stopped following the head after a level load.** 0.5.0 loads levels by a *scene handoff*: the new scene
   starts before the temporary Reload scene goes away. RepoXR's trackers share their XR actions, and the Reload scene's
   tracker switched the head tracking off on its way out. Disabled actions are now switched back on, RepoXR's objects
   move into the loaded scene, and the Reload camera is no longer the main camera.
   **Вид переставал следовать за головой после загрузки уровня.** 0.5.0 грузит уровни «передачей сцены»: новая сцена
   стартует раньше, чем уходит временная Reload. Трекеры RepoXR делят XR-действия, и трекер Reload, уходя, выключал
   трекинг головы. Теперь выключенные действия включаются обратно, объекты RepoXR переносятся в загруженную сцену, а
   камера Reload больше не считается главной.
2. **Grabbing stayed disabled — now it works (PC and Quest).** 0.5.0's *Item Unequip Auto Hold* moved the toggle-grab
   release from `PhysGrabber.Update` into `PhysGrabber.GrabInputActiveGet`. RepoXR's `ForceHoldPatch` searched for its
   old spot, did not find it, and wrote its call into the grab cooldown at the end of `Update`: the grab beam never
   switched on. Now the patch targets `GrabInputActiveGet` and fails loudly if its anchor is gone; the Quest build's
   bake check catches lost anchors like this one. The rest of grabbing was moved to the hand too: grabbing and the
   release after rotating an object (`ForceGrabPhysObject`, `ObjectTurningRelease` — rotated objects used to jump),
   gun aim (`ItemGun.AimTargetUpdate`), the tumble climb (`ClimbTargetUpdate`, pulled where your hand points), vehicle
   grabbing on the grab trigger, and 0.5.0's gamepad grab cursor drift is switched off so the grab point stays on the
   beam. [Details](docs/repo-0.5.0.md#grabbing-what-broke-and-how-it-was-fixed).
   **Захват не работал — теперь работает (ПК и Quest).** *Item Unequip Auto Hold* из 0.5.0 перенёс отпускание
   захвата-переключателя из `PhysGrabber.Update` в `PhysGrabber.GrabInputActiveGet`. `ForceHoldPatch` RepoXR не нашёл
   своё старое место и вписал вызов в задержку захвата в конце `Update` — луч захвата не включался никогда. Теперь
   патч стоит на `GrabInputActiveGet` и падает с ошибкой, если якоря нет. Остальной захват тоже от руки: взять и
   отпустить после вращения (предмет больше не прыгает), прицел пушек, лазание в кувырке, захват техники; дрейф
   курсора захвата 0.5.0 выключен, точка захвата остаётся на луче.
   [Подробно](docs/repo-0.5.0.ru.md#захват-что-сломалось-и-как-починили).
3. **Menus did not click.** New `UIClick` key, gamepad navigation mode, hover areas by `lossyScale`, a
   `StandaloneInputModule` on the EventSystem, XRI's UI raycast finding nothing under Unity 6, a blocked first main
   menu: each one fixed. **Меню не нажимались** — шесть разных причин, все исправлены.
4. **Unity 6 on PC.** The release ships Unity 6 XR libraries (OpenXR 1.14.3 and friends, compiled for Windows by
   `tools/xr-deps/build.py`); FixPluginTypesSerialization 1.1.1 turned out to work on Unity 6000.3.
   **Unity 6 на ПК**: XR-библиотеки от Unity 6, FixPluginTypesSerialization 1.1.1 на Unity 6000.3 работает.
5. **The whip (alpha 3).** 0.5.0's whip cracks and flings by mouse gestures, which a controller never made. Two causes:
   RepoXR skips the game's camera aim update, and with it the timer that ends the aim targets the game forces on the
   camera, so one of them stayed active for good and switched the whip's gestures off; and the gestures read the mouse.
   Now the timers run again, and while you hold the whip the gestures read how fast your hand turns (around the
   vertical and around your head's left-right axis, so a swing from over the shoulder works). Snap the controller back
   and forward to crack; once the whip has caught something, swing to fling it. VR Settings → Input → *Whip Sideways
   Sensitivity* (0.5) and *Whip Crack Sensitivity* (1).
   **Хлыст (alpha 3).** Щелчок и бросок в 0.5.0 — жесты мыши, контроллер их не делал. Две причины: RepoXR пропускает
   обновление прицела камеры игры, а с ним и таймер, который снимает принудительный поворот камеры, — один такой
   оставался навсегда и выключал жесты хлыста; и жесты читают мышь. Теперь таймеры снова идут, а пока держишь хлыст,
   жесты читают скорость поворота руки (вокруг вертикали и вокруг оси «влево-вправо» головы — замах из-за плеча тоже
   работает). Резко назад и вперёд — щелчок; когда хлыст что-то поймал, мах — бросок. VR Settings → Input → *Whip
   Sideways Sensitivity* (0,5) и *Whip Crack Sensitivity* (1).
6. **Level → shop hung on the result screen (alpha 3).** In the scene handoff the old VR loading screen, on its way
   out, cleared the new one's reference; the result screen then threw on every frame and never finished. Fixed, and
   the result screen and moon patches can no longer throw. **Переход уровень → магазин зависал на экране итогов** —
   старый экран загрузки затирал ссылку на новый; исправлено.
7. **The news popup's text was solid bars (alpha 3).** The popup grows from nothing; its texts are built again once
   it has grown, and its scale keeps a depth of 1. **Новость в главном меню рисовалась плашками** — исправлено.
8. **Settings menus and controller icons (alpha 4).** RepoXR's asset bundle was made with Unity 2022's TextMeshPro;
   Unity 6's reads the controller icon sheet without its glyphs and material, so every icon was a `?` (or nothing).
   They are rebuilt when the bundle loads. 0.5.0 also moved the text tags (`[move]`, `[grab]` …) to `InputGlyphs`, so
   the tutorial and prompts showed `[move]` instead of an icon. Toggles: the selection box compared world positions,
   which flip on a menu that hangs in 3D, and RepoXR's own toggles lacked 0.5.0's label fields; the *VR Settings*
   button no longer lands on *Back*. **Меню настроек и значки контроллера (alpha 4)**: значки вместо `?`, `[move]` в
   обучении, рамка переключателей, подписи, кнопка VR Settings — исправлено.
9. **The shop mirror (alpha 5).** The game's reflection camera is an ordinary camera: with XR on it rendered as a
   stereo camera, the eye matrices replaced its reflected view, and the mirror showed a mess. It now renders mono, once
   for each eye, from that eye's reflected position with its own projection. With single-pass rendering both eyes
   share one reflection texture, so the mirror is hidden there; *VR Settings → Rendering → Shop Mirror* (on) turns it
   off for frame rate. The toggles in RepoXR's own menus show the right option too (alpha 5). **Зеркало в магазине
   (alpha 5)** работает: камера отражения рисовала в стерео, теперь — по разу на глаз; в single-pass скрыто; настройка
   *Shop Mirror*. Переключатели в меню RepoXR показывают выбранный вариант.
10. Plus: renamed and new game APIs (16 patch classes), vehicles and their new keys on the left stick, guns aimed from
   the hand, the VR Settings button moved off the new *Language* tab, fonts of RepoXR's 2022 asset bundle.
   Ещё: переименованные API игры, техника на левом стике, прицел пушек от руки, кнопка VR Settings, шрифты.

Every change, patch by patch: [docs/repo-0.5.0.md](docs/repo-0.5.0.md) · [по-русски](docs/repo-0.5.0.ru.md).

## Controls · Управление

| Quest / Touch controller | In game | В игре |
|---|---|---|
| Left stick · click · hold | Move · chat · expression wheel | Ходьба · чат · колесо эмоций |
| Left trigger · left grip | Sprint · rotate held object | Бег · вращать предмет |
| **X** · Y | **Menu** · reset height | **Меню** · сбросить рост |
| Right stick left/right · up/down · click | Turn · push/pull, scroll · crouch | Поворот · оттолкнуть/притянуть, прокрутка · присесть |
| **Right trigger** | **Grab** (beam), click in menus, get on/off vehicles | **Схватить** (луч), нажать в меню, сесть/слезть с техники |
| Right grip | Interact, fire a gun, horn | Взаимодействовать, выстрел, гудок |
| A · B | Jump · tumble | Прыжок · кувырок |

Map at the shoulder, flashlight at the head, inventory, vehicles, weapons, tumble climbing: [docs/controls.md](docs/controls.md) ·
[по-русски](docs/controls.ru.md). Rebind in *Settings → Controls*.

## Related · Связанные проекты

- **[crimecock/repo-quest-vr](https://github.com/crimecock/repo-quest-vr)** (branch `repo-0.5.0`): the standalone
  Quest 3 build of R.E.P.O. 0.5.0 with this RepoXR, built from your own copy of the game · сборка R.E.P.O. 0.5.0 для
  Quest 3 без ПК с этим RepoXR, из вашей копии игры. [Controls on Quest](https://github.com/crimecock/repo-quest-vr/blob/repo-0.5.0/docs/controls.md) ·
  [управление на Quest](https://github.com/crimecock/repo-quest-vr/blob/repo-0.5.0/docs/controls.ru.md).
- The code of this branch (`Source/`) is repo-quest-vr's [`patches/repoxr`](https://github.com/crimecock/repo-quest-vr/tree/repo-0.5.0/patches/repoxr)
  0001–0029 applied to RepoXR 1.2.3 · код этой ветки — патчи 0001–0029 из repo-quest-vr поверх RepoXR 1.2.3.
- Upstream: [DaXcess/RepoXR](https://github.com/DaXcess/RepoXR) (RepoXR 1.2.3, R.E.P.O. 0.4.4),
  [Dobutsu/repo-quest-vr](https://github.com/Dobutsu/repo-quest-vr) (the original Quest port).

---

# R.E.P.O. VR Mod

[![Thunderstore Version](https://img.shields.io/thunderstore/v/DaXcess/RepoXR?style=for-the-badge&logo=thunderstore&logoColor=white)](https://thunderstore.io/c/repo/p/DaXcess/RepoXR)
[![GitHub Version](https://img.shields.io/github/v/release/DaXcess/RepoXR?style=for-the-badge&logo=github)](https://github.com/DaXcess/RepoXR/releases/latest)
[![Thunderstore Downloads](https://img.shields.io/thunderstore/dt/DaXcess/RepoXR?style=for-the-badge&logo=thunderstore&logoColor=white)](https://thunderstore.io/c/repo/p/DaXcess/RepoXR)
[![GitHub Downloads](https://img.shields.io/github/downloads/DaXcess/RepoXR/total?style=for-the-badge&logo=github)](https://github.com/DaXcess/RepoXR/releases/latest)
<br />
[![Release Build](https://img.shields.io/github/actions/workflow/status/DaXcess/RepoXR/build-release.yaml?branch=main&style=for-the-badge&label=RELEASE)](https://github.com/DaXcess/RepoXR/actions/workflows/build-release.yaml)
[![Debug Build](https://img.shields.io/github/actions/workflow/status/DaXcess/RepoXR/build-debug.yaml?branch=dev&style=for-the-badge&label=DEBUG)](https://github.com/DaXcess/RepoXR/actions/workflows/build-debug.yaml)

**RepoXR** is a fully fledged R.E.P.O. VR mod that adds full 6-DoF motion controlled VR support to R.E.P.O.

The mod is powered by Unity's OpenXR plugin and is thereby compatible with a wide range of headsets, controllers and runtimes, like Oculus, Virtual Desktop, SteamVR and many more!

RepoXR is compatible with multiplayer and works in a lobby comprised of both flatscreen **and** VR players. Running this mod without having a VR headset will allow you to see the arm movements of any VR semibots in the same lobby, all while still being compatible with vanilla clients (even if the host is using no mods, though there is a small catch with that).

### Discord Server

Facing issues, have some mod (in)compatibility to report or just want to hang out?

You can join the [Discord Server](https://discord.gg/2DxNgpPZUF)!

# Compatibility

At the time of writing, no explicit compatibility exists yet for RepoXR, so you will have to manually figure out what mod works in VR and which ones don't.

# Installing and using the mod

It is recommended to use a mod launcher like Gale to easily download and install the mod. You can download Gale from its [GitHub releases](https://github.com/Kesomannen/gale/releases). This mod can be found on thunderstore under the name [RepoXR](https://thunderstore.io/c/repo/p/DaXcess/RepoXR). You can also install the mod by manually downloading it in combination with BepInEx.

Running the mod using Gale can be done simply by clicking "Launch game", which will automagically launch the game with the installed mods.

For more information on using the mod, check out the [RepoXR Thunderstore page](https://thunderstore.io/c/repo/p/DaXcess/RepoXR).

# Versions

Here is a list of RepoXR versions and which version(s) of R.E.P.O. it supports

| RepoXR | R.E.P.O. Version |
|--------|------------------|
| v1.2.3 | v0.4.4           |
| v1.2.2 | v0.4.3           |
| v1.2.1 | v0.4.3           |
| v1.2.0 | v0.4.3           |
| v1.1.2 | v0.3.0 - v0.3.2  |
| v1.1.1 | v0.3.0 - v0.3.1  |
| v1.1.0 | v0.3.0 - v0.3.1  |
| v1.0.3 | v0.2.1           |
| v1.0.2 | v0.2.1           |
| v1.0.1 | v0.2.1           |
| v1.0.0 | v0.2.0           |

> RepoXR dynamically measures compatibility with the game during startup, meaning newer R.E.P.O. versions might be supported even though they aren't listed here.

# Install from source

> The easiest way to install the mod is by downloading it from Thunderstore. You only need to follow these steps if you are planning on installing the mod by building the source code and without a mod manager.

To install the mod from the source code, you will first have to compile the mod. Instructions for this are available in [COMPILING.md](COMPILING.md).

Next up you'll need to grab a copy of a previous release of RepoXR either from [Releases](https://github.com/DaXcess/RepoXR/releases) or from [Thunderstore](https://thunderstore.io/c/repo/p/DaXcess/RepoXR) (manual download). Extract this zip file and replace the `LCVR.dll` and/or the `LCVR.Preload.dll` files with the new files that you obtained after following the guide in [COMPILING.md](COMPILING.md).

You can *also* manually build the required plugins, asset bundles and addressables by cloning the [RepoXR-Unity](https://github.com/DaXcess/RepoXR-Unity) and performing the necessary steps (which I am not planning on documenting).