# RepoXR на R.E.P.O. 0.5.0

[English](repo-0.5.0.md) · **Русский**

Неофициальный порт [RepoXR](https://github.com/DaXcess/RepoXR) 1.2.3 (DaXcess) на R.E.P.O. 0.5.0 — обновление игры на
Unity 6. Ветка `repo-0.5.0` в [crimecock/RepoXR](https://github.com/crimecock/RepoXR). О проблемах этого порта пишите в
[issues форка](https://github.com/crimecock/RepoXR/issues), а не DaXcess.

## Состояние

| Где | Что |
|---|---|
| Quest 3, автономно ([repo-quest-vr](https://github.com/crimecock/repo-quest-vr), ветка `repo-0.5.0`) | Тот же исходный код RepoXR, собранный в APK для Quest. С 1.0.7-qc21 играется в шлеме: меню, одиночное лобби и уровни (трекинг головы, лучи, захват), VR-настройки |
| ПК (BepInEx, релиз этого форка) | **Экспериментально, в VR ещё не проверялось.** Собирается под сборки ПК-версии 0.5.0, и патчи RepoXR ложатся на тот же код игры, что работает в сборке для Quest, но на Windows со шлемом его ещё никто не запускал. Риски — ниже |

Мультиплеер, техника и оружие пока нигде не проверены.

## Установка на ПК

1. Поставьте [BepInExPack](https://thunderstore.io/c/repo/p/BepInEx/BepInExPack/) и
   [FixPluginTypesSerialization](https://thunderstore.io/c/repo/p/DaXcess/FixPluginTypesSerialization/) от DaXcess —
   это две зависимости RepoXR 1.2.3 (в менеджере модов вроде Gale проще один раз поставить RepoXR 1.2.3, они придут с
   ним).
2. Удалите RepoXR 1.2.3, если он стоит (`BepInEx/plugins/RepoXR` и `BepInEx/patchers/RepoXR`).
3. Распакуйте `RepoXR-repo0.5.0-*.zip` из [релизов](https://github.com/crimecock/RepoXR/releases) форка в папку игры
   (рядом с `REPO.exe`) или импортируйте его в Gale как локальный мод.
4. Запускайте игру с подключённым шлемом (SteamVR, Oculus/Meta Link, Virtual Desktop...), как с RepoXR 1.2.3.

К issue прикладывайте лог BepInEx (`BepInEx/LogOutput.log`). Порт пишет туда немного диагностики: `[Scene]`,
`[Camera]` (первые 40 с сцены), `[Input]`, `[UI]`, `[Grab]`, `[FPS]`.

## Запуск в VR

RepoXR работает через OpenXR, поэтому подходит любой VR-рантайм для ПК; сам запуск игры не меняется (запускайте
R.E.P.O. из Steam как обычно, со шлемом и запущенной программой шлема на ПК). Рантайм выбирается в таком порядке:

1. выбранный в VR-настройках RepoXR (выбор рантайма; хранится как `OpenXRRuntimeFile` в
   `BepInEx/config/io.daxcess.repoxr.cfg`);
2. рантайм OpenXR по умолчанию в системе;
3. любой другой из найденных: SteamVR, Virtual Desktop, Meta (Oculus).

Если игра запущена от администратора, доступен только рантайм по умолчанию.

| Шлем и подключение | Как |
|---|---|
| **SteamVR** (Valve Index, HTC Vive, Pico через Streaming Assistant, Quest через Steam Link или ALVR, любой шлем SteamVR) | Запустите SteamVR, потом игру. Если по умолчанию стоит другой рантайм, выберите SteamVR в выборе рантайма RepoXR или сделайте его основным: SteamVR → Settings → OpenXR → *Set SteamVR as OpenXR Runtime* |
| **Meta Quest через Link или Air Link** (приложение Meta Quest Link) | Meta Quest Link → Settings → General → *OpenXR Runtime*: сделайте Meta Quest Link активным. Подключите Link, затем запустите игру на ПК |
| **Virtual Desktop** (Quest, Pico) | Его собственный рантайм (VDXR) находится сам: запустите Virtual Desktop, потом игру с его рабочего стола. В режиме SteamVR игра пойдёт через SteamVR |
| **Другие рантаймы OpenXR** (Pico Connect, Windows Mixed Reality...) | Сделайте рантайм основным в системе или выберите его в выборе рантайма RepoXR |
| **Без VR** | Параметр запуска `--disable-vr` или `DisableVR = true` в конфиге. Руки VR-игроков всё равно видно |

Параметры запуска задаются в Steam → R.E.P.O. → Свойства → *Параметры запуска*, например `-force-d3d11` (см. риски
ниже).

## Риски на ПК

| Что | Почему может сломаться | Если сломалось |
|---|---|---|
| Библиотеки OpenXR | С alpha 2 — от Unity 6: OpenXR-плагин 1.14.3 (нативные `UnityOpenXR.dll` и `openxr_loader.dll` для Windows x64 из пакета), XR Management 4.5.0, XR Interaction Toolkit 2.6.4, XR Core Utils 2.6.0, Legacy Input Helpers 2.1.13 — те же версии, что работают в сборке для Quest. Управляемые собраны под Windows вне редактора Unity (ниже); все члены, которые использует RepoXR, в них находятся, но на Windows они ещё не запускались | В логе не проходит `[RepoXR] Loading VR...` или есть `MissingMethodException` / `TypeLoadException` со сборкой `Unity.XR` |
| Графический API | Unity 6 может запускать игру на Direct3D 12 (в игре есть `D3D12Core.dll`), а RepoXR до сих пор работал только на Direct3D 11 | Попробуйте параметр запуска `-force-d3d11` |
| FixPluginTypesSerialization | Подменяет функции нативного плеера Unity, чтобы загрузить компоненты из бандла RepoXR; сделан под Unity 2022 | Нет VR-интерфейса, ошибки про скрипты в `repoxrassets` |
| Бандл ассетов RepoXR и Addressables | Собраны Unity 2022. Шрифты исправлены в коде (ниже); шейдеры и каталог Addressables 1.x под Addressables 2 из 0.5.0 на ПК не проверены | Розовый или пропавший VR-интерфейс, ошибки загрузки `catalog_0.1.json` |

## Что изменилось в 0.5.0 и что делает порт

Каждое исправление — отдельный коммит в `repo-0.5.0`; номера патчей — как в
[`patches/repoxr` у repo-quest-vr](https://github.com/crimecock/repo-quest-vr/tree/repo-0.5.0/patches/repoxr).

| В R.E.P.O. 0.5.0 | Что стало с RepoXR 1.2.3 | Исправление |
|---|---|---|
| `InputKey.Confirm` → `UIClick` и другие переименования, новые перегрузки (`CameraShake.Shake`, `CameraAim.AimTargetSoftSet`, `InputToggleGet`, `UIMouseGetLocalPositionWithinRectTransform`), одна страница текстового ввода, поле пароля как `MenuTextInput`, поворот при лазании в `ClimbTargetUpdate` | 16 классов патчей не применялись или не собирались | 0009 |
| Меню нажимаются через `InputKey.UIClick` | Кнопки не нажимались | 0009: на `UIClick` отвечает курок луча |
| Поддержка геймпада: меню переходят в навигацию с клавиатуры и возвращаются, только когда двигается мышь | В VR после перехода наведение ничего не выбирало | 0010: режим указателя держится, пока есть VR-лучи |
| `SemiFunc.UIMouseHover` считает зону наведения по `lossyScale` | На мировом холсте меню (масштаб 0,03) зона сжималась в точку | 0011: масштаб относительно страницы меню |
| EventSystem сцены работает со `StandaloneInputModule` | Лучи XRI видят UI только через `XRUIInputModule` текущего EventSystem | 0012: один EventSystem, включён только `XRUIInputModule` |
| UI-рейкаст XRI под Unity 6 ничего не находит | Указатель ничего не выбирал | 0014: пересечение луча с мировыми холстами вручную; если XRI что-то нашёл, берётся его результат. 0013: лучи до 100 м (холст главного меню в 20–25 м) |
| Страницы меню больше не в начале координат холста | Указатель и элементы сравнивались в разных пространствах | 0015 |
| Первое главное меню блокирует ввод до конца вступительной анимации | RepoXR её пропускает: ничего не нажималось | 0017 |
| Шесть вкладок настроек, новая *Language* | Кнопка VR Settings лежала поверх неё | 0018: под *Language*, *Back* ниже |
| TextMeshPro в Unity 6 | У текстов бандла (TMP Unity 2022) не было рабочего шрифта | 0018, 0023, 0025: шрифт меню игры, в том числе до VR-настройки каждой сцены |
| Item Unequip Auto Hold: отпускание при захвате-переключателе переехало в `PhysGrabber.GrabInputActiveGet` | `ForceHoldPatch` терял якорь и правил задержку захвата: **захват оставался выключенным** | 0019: патч на `GrabInputActiveGet`, при потере якоря — исключение |
| Новые клавиши: `VehicleAccelerate/Brake/DriftLeft/DriftRight/Exit/Grab`, `PushPull`, `ExpressionWheel`, навигация UI | В VR-действиях RepoXR их нет: исключение на каждый поиск, откат на клавиатуру | 0019: поиск один раз; техника на стике движения, `VehicleGrab` на захвате (переключатель, как в игре) |
| `InputManager.VehicleInputAllowed` требует включённую карту `Player` игры | Техника могла не слушаться стика | 0019 |
| Прицел пушек от камеры (`ItemGun.AimTargetUpdate`); `ObjectTurningRelease`, `ForceGrabPhysObject` | RepoXR держит предметы относительно руки: пушка смотрела не туда, предмет после вращения прыгал | 0020: от руки |
| Передача сцены: следующая сцена Main грузится, пока активна сцена Reload, которая выгружается потом | Корневые объекты RepoXR (холст VR-оверлея, сетевая система) уходили вместе с Reload: в лобби не было VR-HUD, меню паузы и лучей | 0024: переносятся в загруженную сцену |
| У сцены Reload своя камера с тегом MainCamera | `Camera.main` выбирал её | 0026: UI и камера самой загруженной сцены; с камеры Reload снимается тег |
| Трекеры RepoXR (`TrackedPoseDriver`) и контроллеры XRI делят XR-действия и выключают их, когда уходят | При передаче сцены трекер камеры Reload уходил уже после старта трекера новой камеры: **вид переставал следовать за головой** в лобби и всех уровнях (обучение грузится без передачи) | 0028: выключенные действия снова включаются перед следующим обновлением ввода |

Ещё в ветке: настройки Dobutsu из repo-quest-vr (0001–0008: Head Bob, Post Processing, тени фонариков других
игроков, исправление сферы фокуса; Pixel Lights и Lamp Lighting — для Quest), счётчик FPS (VR Settings → Rendering)
и диагностика (0013, 0016, 0018, 0027).

## Чем ПК отличается от Quest

`Plugin.IsQuest` (Android) не пускает в ПК-версию то, что нужно только Quest:

- Lamp Lighting на ПК не создаётся: ему нужны шейдеры сборки для Quest. Pixel Lights не применяется. Обе настройки и
  три настройки Level Culling, которые читают инструменты сборки Quest, скрыты из VR-настроек.
- На ПК — значения RepoXR по умолчанию: Head Bob 25%, пост-обработка включена, тени фонариков других игроков включены,
  однопроходный рендер выключен.
- На ПК остаются привязки RepoXR. В порте для Quest меню паузы переезжает на кнопку ≡ левого контроллера, а X выключает
  микрофон.

## Сборка

Как у RepoXR 1.2.3 ([COMPILING.md](../COMPILING.md)), но со сборками R.E.P.O. 0.5.0 (Unity 6000.3.17f1).

XR-библиотеки релиза: [`tools/xr-deps/build.py`](../tools/xr-deps/build.py) заново компилирует шесть XR-пакетов под
Windows из проекта Unity 6, где они есть и который один раз собирался в плеер (подходит Quest-проект repo-quest-vr).
Скрипт берёт аргументы компилятора, с которыми собирал Unity (`Library/Bee/artifacts/*P.dag/<сборка>.rsp`), меняет
define платформы на `UNITY_STANDALONE_WIN`, а ссылки — на `REPO_Data/Managed` ПК-игры, и обходится без генераторов
кода Unity (этим пакетам они в рантайме не нужны). `Unity.Mathematics` больше не кладётся: у игры есть своя (1.3.2), и
XR-пакеты собраны с ней. Нативные `UnityOpenXR.dll` и `openxr_loader.dll` — файлы пакета из `Runtime/windows/x64` и
`RuntimeLoaders/windows/x64`.

## Авторы

RepoXR — [DaXcess](https://github.com/DaXcess/RepoXR) (GPL-3.0). Настройки 0001–0008 —
[Dobutsu](https://github.com/Dobutsu/repo-quest-vr). Порт на 0.5.0 — crimecock, вместе с Claude.
