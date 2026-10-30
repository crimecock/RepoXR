> [!IMPORTANT]
> **This is an unofficial fork: RepoXR 1.2.3 ported to R.E.P.O. 0.5.0 (Unity 6).** Branch `repo-0.5.0`.
> The same source plays on a Quest 3 in the standalone [repo-quest-vr](https://github.com/crimecock/repo-quest-vr)
> build; the PC (BepInEx) release **plays in VR on Windows** (SteamVR, Virtual Desktop; Quest controllers): menus, levels,
> the shop and its mirror, grabbing, guns, the whip, tumble climbing, vehicles, the settings menus, multiplayer. What
> changed, how to install it and the risks: [docs/repo-0.5.0.md](docs/repo-0.5.0.md) · [по-русски](README.ru.md).
> Report problems with this fork [here](https://github.com/crimecock/RepoXR/issues), not to DaXcess.

**Start here · С чего начать**

| I want to… · Хочу… | Read · Читать |
|---|---|
| play on PC · играть на ПК | [Install](#install-on-pc-repo-050--установка-на-пк) (below), then [controls](docs/controls.md) · [управление](docs/controls.ru.md) |
| update · обновиться | [Updating](#install-on-pc-repo-050--установка-на-пк) (in Install) |
| play on a Quest without a PC · играть на Quest без ПК | [repo-quest-vr](https://github.com/crimecock/repo-quest-vr) |
| change the mod · поменять мод | [Build from source](#build-from-source--сборка-из-исходников), [COMPILING.md](COMPILING.md) |
| make the release zips, release a version · собрать архивы, выпустить версию | [COMPILING.md](COMPILING.md#the-release-zips--архивы-релиза), [releasing.md](https://github.com/crimecock/repo-quest-vr/blob/repo-0.5.0/docs/releasing.md) |
| know what the port changed and why · что поменял порт и почему | [Status](#status-and-what-made-it-work--состояние-и-что-пришлось-починить), [docs/repo-0.5.0.md](docs/repo-0.5.0.md) |

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

**You need:** R.E.P.O. 0.5.0 on [Steam](https://store.steampowered.com/app/3241660/REPO/) (Windows), a PC VR headset
and its PC software — for a Quest: [Meta Horizon Link](https://www.meta.com/help/quest/1517439565442928/) (Link / Air Link) or [Virtual Desktop](https://www.vrdesktop.net/);
other headsets: [SteamVR](https://store.steampowered.com/app/250820/SteamVR/). Nothing else: BepInEx comes in the zip.

1. Download the newest all-in-one zip: now **alpha 6** — [RepoXR-repo0.5.0-alpha.6-all-in-one.zip](https://github.com/crimecock/RepoXR/releases/download/v1.2.3-repo0.5.0-alpha.6/RepoXR-repo0.5.0-alpha.6-all-in-one.zip)
   (all versions: [Releases](https://github.com/crimecock/RepoXR/releases)).
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

**Нужно:** R.E.P.O. 0.5.0 в [Steam](https://store.steampowered.com/app/3241660/REPO/) (Windows), VR-шлем для ПК и
его программа на ПК — для Quest: [Meta Horizon Link](https://www.meta.com/help/quest/1517439565442928/) (Link / Air Link) или [Virtual Desktop](https://www.vrdesktop.net/);
другие шлемы: [SteamVR](https://store.steampowered.com/app/250820/SteamVR/). Больше ничего: BepInEx — в архиве.

1. Скачайте самый новый архив «всё в одном»: сейчас **alpha 6** — [RepoXR-repo0.5.0-alpha.6-all-in-one.zip](https://github.com/crimecock/RepoXR/releases/download/v1.2.3-repo0.5.0-alpha.6/RepoXR-repo0.5.0-alpha.6-all-in-one.zip)
   (все версии — [релизы](https://github.com/crimecock/RepoXR/releases)).
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

## Build from source · Сборка из исходников

**To play you build nothing:** the all-in-one zip has RepoXR.dll ready-made. Build it yourself only to change the mod.
The Quest build ([repo-quest-vr](https://github.com/crimecock/repo-quest-vr)) bakes the same DLL into the game
([how it builds it](https://github.com/crimecock/repo-quest-vr/blob/repo-0.5.0/docs/build-repoxr.md)).

A mod is compiled against the game's own DLLs. DaXcess's RepoXR downloads them from NuGet (a package server for .NET),
but NuGet only has R.E.P.O. 0.4.4 and Unity 2022, so this fork takes them from **your installed game** instead.

1. **Install the [.NET SDK 10](https://dotnet.microsoft.com/download/dotnet/10.0).** Not 8: RepoXR uses C# 14, and
   .NET 8 stops with `CS0106` errors.
2. **Install the mod the normal way first** ([above](#install-on-pc-repo-050--установка-на-пк)): the build also needs
   the Unity 6 XR libraries from the all-in-one zip (`BepInEx/plugins/RepoXR/RuntimeDeps` in the game folder).
3. **Get the code** with git, or as a [ZIP](https://github.com/crimecock/RepoXR/archive/refs/heads/repo-0.5.0.zip) (unpacks to `RepoXR-repo-0.5.0`):
   ```sh
   git clone https://github.com/crimecock/RepoXR.git -b repo-0.5.0
   cd RepoXR
   ```
4. **Build.** `GamePath` is the game folder (the one with `REPO.exe` and `REPO_Data`); the internet is needed once, for
   BepInEx and two small build tools from NuGet:
   ```sh
   dotnet build RepoXR.sln -c Release -p:GamePath="C:\Program Files (x86)\Steam\steamapps\common\REPO"
   ```
5. **Try it:** copy `bin/Release/netstandard2.1/RepoXR.dll` over `BepInEx/plugins/RepoXR/RepoXR.dll` in the game folder.
   Unpacking the all-in-one zip again puts the release's DLL back.

With `-p:DebugSymbols=false` the DLL has exactly the release's code. Other folders, macOS / Linux, troubleshooting:
[COMPILING.md](COMPILING.md).

**The release zips themselves** (the all-in-one with BepInEx and FixPluginTypesSerialization, and the RepoXR zip for
Gale): `python3 tools/package/make_release.py alpha.7` builds both from scratch, every download checked by SHA-256;
what goes in and from where: [COMPILING.md](COMPILING.md#the-release-zips--архивы-релиза). Releasing a new version
(here and for the Quest): [releasing.md](https://github.com/crimecock/repo-quest-vr/blob/repo-0.5.0/docs/releasing.md).

**По-русски.** Чтобы играть, собирать ничего не нужно. Мод компилируется по DLL самой игры; RepoXR от DaXcess берёт их
из NuGet, но там только R.E.P.O. 0.4.4, поэтому форк берёт их из **вашей установленной игры**. Шаги: поставить
[.NET SDK 10](https://dotnet.microsoft.com/download/dotnet/10.0) (на .NET 8 — ошибки `CS0106`), сначала установить мод
обычным способом (сборке нужны XR-библиотеки из архива), скачать код (git или [ZIP](https://github.com/crimecock/RepoXR/archive/refs/heads/repo-0.5.0.zip)), собрать командой выше
(`GamePath` — папка с `REPO.exe`), положить `RepoXR.dll` из `bin/Release/netstandard2.1` вместо
`BepInEx/plugins/RepoXR/RepoXR.dll`. Подробно — [README.ru.md](README.ru.md#сборка-из-исходников) и [COMPILING.md](COMPILING.md).

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
| Settings menus, VR controls page · меню настроек, страница управления VR | ✅ alpha 4–6 | ✅ same code; the Quest 3's controls page since 1.0.7-qc27 · тот же код; страница управления на Quest 3 — с 1.0.7-qc27 |
| Shop mirror · зеркало в магазине | ✅ alpha 5 (*VR Settings → Rendering → Shop Mirror*, on · вкл.) | ✅ hidden by default (single-pass shares one reflection between the eyes); shown with *VR Settings → Rendering → Render Mode = Multipass* · по умолчанию скрыто (в single-pass одно отражение на оба глаза); видно с *Render Mode = Multipass* |
| Multiplayer with other players · мультиплеер с другими | ✅ | ✅ |

## Known issues · Известные проблемы

Everything works on PC, and the Quest builds the same code · на ПК всё работает, а Quest собирает тот же код. One
small issue · одна мелочь:

- **The language page** (*Settings → Language*) shows only Text to Speech in VR: the language buttons land outside the
  menu. The fix comes with the next version; until then start the game without VR (headset off, or the launch option
  `--disable-vr`), pick the language on the monitor and start again in VR. · **Страница «Язык»** (*Настройки → Язык*)
  в VR показывает только Text to Speech: кнопки языков оказываются за пределами меню. Исправление — в следующей
  версии; а пока запустите игру без VR (шлем выключен или параметр запуска `--disable-vr`), выберите язык на мониторе
  и запустите снова в VR.

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
[по-русски](docs/controls.ru.md). Rebind in *Settings → Controls*, where free actions (Inventory 1–3, Expression 1–6,
Push To Talk) can get a button too · переназначение — *Настройки → Управление*. On the Quest the menu is on ≡ and X
mutes · на Quest меню на ≡, X — микрофон ([Quest controls](https://github.com/crimecock/repo-quest-vr/blob/repo-0.5.0/docs/controls.md)).

## Related · Связанные проекты

- **[crimecock/repo-quest-vr](https://github.com/crimecock/repo-quest-vr)** (branch `repo-0.5.0`): the standalone
  Quest 3 build of R.E.P.O. 0.5.0 with this RepoXR, built from your own copy of the game · сборка R.E.P.O. 0.5.0 для
  Quest 3 без ПК с этим RepoXR, из вашей копии игры. [Controls on Quest](https://github.com/crimecock/repo-quest-vr/blob/repo-0.5.0/docs/controls.md) ·
  [управление на Quest](https://github.com/crimecock/repo-quest-vr/blob/repo-0.5.0/docs/controls.ru.md).
- The code of this branch (`Source/`) is repo-quest-vr's [`patches/repoxr`](https://github.com/crimecock/repo-quest-vr/tree/repo-0.5.0/patches/repoxr)
  0001–0032 applied to RepoXR 1.2.3, and 0033 is this branch's `RepoXR.csproj` (build against the game folder) · код
  этой ветки — патчи 0001–0032 из repo-quest-vr поверх RepoXR 1.2.3, а 0033 — `RepoXR.csproj` этой ветки (сборка по
  папке игры).
- Upstream: [DaXcess/RepoXR](https://github.com/DaXcess/RepoXR) (RepoXR 1.2.3, R.E.P.O. 0.4.4),
  [Dobutsu/repo-quest-vr](https://github.com/Dobutsu/repo-quest-vr) (the original Quest port).

---

The original RepoXR for R.E.P.O. 0.4.4, with its own README, Discord and Thunderstore page:
**[DaXcess/RepoXR](https://github.com/DaXcess/RepoXR)** · оригинальный RepoXR для R.E.P.O. 0.4.4 и его README — у
DaXcess.
