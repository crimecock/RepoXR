# RepoXR на R.E.P.O. 0.5.0

[English](repo-0.5.0.md) · **Русский**

Неофициальный порт [RepoXR](https://github.com/DaXcess/RepoXR) 1.2.3 (DaXcess) на R.E.P.O. 0.5.0 — обновление игры на
Unity 6. Ветка `repo-0.5.0` в [crimecock/RepoXR](https://github.com/crimecock/RepoXR). О проблемах этого порта пишите в
[issues форка](https://github.com/crimecock/RepoXR/issues), а не DaXcess.

## Состояние

| Где | Что |
|---|---|
| Quest 3, автономно ([repo-quest-vr](https://github.com/crimecock/repo-quest-vr), ветка `repo-0.5.0`) | Тот же исходный код RepoXR, собранный в APK для Quest. С 1.0.7-qc21 играется в шлеме: меню, одиночное лобби и уровни (трекинг головы, лучи, захват), VR-настройки |
| ПК (BepInEx, релиз этого форка) | **Играется в VR на Windows** (alpha 6; проверено 30 октября 2026 через SteamVR и Virtual Desktop, контроллеры Quest): меню, лобби, уровни и магазин через передачу сцены, трекинг головы, лучи, захват (см. ниже), пушки, хлыст, лазание в кувырке, новость в главном меню, меню настроек и значки контроллера, зеркало в магазине, техника, колесо эмоций, обучение, мультиплеер с другими игроками. Риски — ниже |

Мультиплеер с другими игроками работает на ПК и на Quest; езда на технике на ПК работает. Меню настроек и значки контроллера работают с alpha 4, зеркало в магазине — с alpha 5 (при single-pass скрыто). Оставшиеся известные проблемы — в [README](../README.ru.md#известные-проблемы). На Quest тот же код приходит с 1.0.7-qc27 (патчи 0030–0032).

### Что проверено в alpha 3 и как

Во внутренних тестовых сборках (не выпускались) на F-клавишах и в чате были тестовые команды самой игры: все
предметы на пол, все пушки, снегоход и гольф-кар, Pony, магазин, бессмертие. С ними на ПК проверено: пушки стреляют
от руки; хлыст щёлкает (73 щелчка за один заход, `[Whip] crack gesture: Strike`), в том числе из-за плеча; переход
уровень → магазин больше не зависает; новость в меню читается; передача сцен проходит без ошибок в логе.

## Установка на ПК

Нужны R.E.P.O. 0.5.0 из Steam на Windows и VR-шлем для ПК (см. [Запуск в VR](#запуск-в-vr)). Скачайте
`RepoXR-repo0.5.0-*.zip` из [релизов](https://github.com/crimecock/RepoXR/releases) форка.

**Всё в одном (проще всего):** в `RepoXR-repo0.5.0-*-all-in-one.zip` лежат BepInEx 5.4.23 (BepInExPack),
FixPluginTypesSerialization 1.1.1 и этот RepoXR. Распакуйте его в папку игры, рядом с `REPO.exe` (Steam → R.E.P.O. →
Свойства → Установленные файлы → Обзор), и запускайте игру из Steam с подключённым шлемом; больше ничего ставить не
нужно. Если там уже стоят другой BepInEx или RepoXR 1.2.3, сначала удалите `BepInEx/plugins/RepoXR` и
`BepInEx/patchers/RepoXR`. Удаление: удалите `winhttp.dll`, `doorstop_config.ini`, `.doorstop_version` и папку
`BepInEx`. Лицензии того, что внутри, — в его `RepoXR-README.txt`.

**Через Gale** (менеджер модов; официально скачивается с [GitHub](https://github.com/Kesomannen/gale/releases),
[Thunderstore](https://thunderstore.io/c/lethal-company/p/Kesomannen/GaleModManager/) и
[Hexium](https://hexium.gg/mod-manager); galemodmanager.com — неофициальный):

1. Выберите R.E.P.O. и создайте профиль.
2. Поставьте **RepoXR** от DaXcess: вместе с ним придут BepInExPack и FixPluginTypesSerialization.
3. Выключите (или удалите) RepoXR от DaXcess, две его зависимости оставьте.
4. Импортируйте архив как локальный мод (импорт локальных модов есть в меню импорта Gale). Если не нашли — откройте
   папку профиля и распакуйте архив в неё, чтобы появились `BepInEx/plugins/RepoXR` и `BepInEx/patchers/RepoXR`.
5. Включён должен быть только один RepoXR: у обоих одинаковый ID плагина. Запускайте игру кнопкой *Launch game* в Gale.

**Вручную:**

1. Распакуйте [BepInExPack](https://thunderstore.io/c/repo/p/BepInEx/BepInExPack/) и
   [FixPluginTypesSerialization](https://thunderstore.io/c/repo/p/DaXcess/FixPluginTypesSerialization/) от DaXcess в
   папку игры, рядом с `REPO.exe` (Steam → R.E.P.O. → Свойства → Установленные файлы → Обзор).
2. Запустите игру один раз без шлема и закройте.
3. Удалите RepoXR 1.2.3, если он стоит (`BepInEx/plugins/RepoXR` и `BepInEx/patchers/RepoXR`), и распакуйте архив в
   папку игры.
4. Запускайте игру из Steam с подключённым шлемом.

**Обновление** (RepoXR уже стоит): снова распакуйте самый новый архив «всё в одном» в папку игры с заменой файлов;
BepInEx и ваши настройки (`BepInEx/config/io.daxcess.repoxr.cfg`) сохранятся. Если ставили только архив RepoXR —
замените `BepInEx/plugins/RepoXR` и `BepInEx/patchers/RepoXR`. В Gale — импортируйте новый архив (или удалите RepoXR и
добавьте новый). Версия — в первой строке RepoXR в `BepInEx/LogOutput.log`.

**Что проверить:** вид следует за головой; из рук идут лучи, кнопки меню нажимаются; захват работает в лобби;
открывается VR Settings (кнопка под вкладкой *Language* в настройках).

К issue прикладывайте лог BepInEx (`BepInEx/LogOutput.log`; в Gale он открывается из профиля). В нём ищите
`unofficial port to R.E.P.O. v0.5.0` и `OpenXR runtime being used: ...`; ещё порт пишет немного диагностики:
`[Scene]`, `[Camera]` (первые 40 с сцены), `[Input]`, `[UI]`, `[Grab]`, `[FPS]`. Чтобы видеть лог во время игры,
поставьте `Enabled = true` в разделе `[Logging.Console]` файла `BepInEx/config/BepInEx.cfg`.

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
| **[SteamVR](https://store.steampowered.com/app/250820/SteamVR/)** (Valve Index, HTC Vive, Pico через Streaming Assistant, Quest через Steam Link или ALVR, любой шлем SteamVR) | Запустите SteamVR, потом игру. Если по умолчанию стоит другой рантайм, выберите SteamVR в выборе рантайма RepoXR или сделайте его основным: SteamVR → Settings → OpenXR → *Set SteamVR as OpenXR Runtime* |
| **Meta Quest через Link или Air Link** ([приложение Meta Horizon Link](https://www.meta.com/help/quest/1517439565442928/), раньше Meta Quest Link) | Meta Quest Link → Settings → General → *OpenXR Runtime*: сделайте Meta Quest Link активным. Подключите Link, затем запустите игру на ПК |
| **[Virtual Desktop](https://www.vrdesktop.net/)** (Quest, Pico; на ПК — приложение Streamer) | Его собственный рантайм (VDXR) находится сам: запустите Virtual Desktop, потом игру с его рабочего стола. В режиме SteamVR игра пойдёт через SteamVR |
| **Другие рантаймы OpenXR** (Pico Connect, Windows Mixed Reality...) | Сделайте рантайм основным в системе или выберите его в выборе рантайма RepoXR |
| **Без VR** | Параметр запуска `--disable-vr` или `DisableVR = true` в конфиге. Руки VR-игроков всё равно видно |

Параметры запуска задаются в Steam → R.E.P.O. → Свойства → *Параметры запуска*, например `-force-d3d11` (см. риски
ниже).

## Риски на ПК

| Что | Почему может сломаться | Если сломалось |
|---|---|---|
| Библиотеки OpenXR | С alpha 2 — от Unity 6: OpenXR-плагин 1.14.3 (нативные `UnityOpenXR.dll` и `openxr_loader.dll` для Windows x64 из пакета), XR Management 4.5.0, XR Interaction Toolkit 2.6.4, XR Core Utils 2.6.0, Legacy Input Helpers 2.1.13 — те же версии, что в сборке для Quest, собранные под Windows вне редактора Unity (ниже) | **Работают** в первой проверке на ПК: SteamVR/OpenXR 2.18.4, контроллеры Oculus Touch. Если с другим рантаймом не заработает, в логе не пройдёт `[RepoXR] Loading VR...` или будет `MissingMethodException` / `TypeLoadException` со сборкой `Unity.XR` |
| Графический API | Unity 6 может запускать игру на Direct3D 12 (в игре есть `D3D12Core.dll`), а RepoXR до сих пор работал только на Direct3D 11 | Попробуйте параметр запуска `-force-d3d11` |
| FixPluginTypesSerialization | Подменяет функции нативного плеера Unity, чтобы загрузить компоненты из бандла RepoXR; в его README поддержка до Unity 2023.2 | **Работает на Unity 6000.3.17** в первой проверке на ПК: находит свои семь функций по символам Unity («Running under Unity v6000.3.17», «Function offset ...»), VR-интерфейс загружается |
| Бандл ассетов RepoXR | Собран Unity 2022. Шрифты исправлены в коде (ниже); шейдеры под Unity 6 на ПК не проверены | Розовый или пропавший VR-интерфейс |
| Переводы RepoXR (Addressables) | Addressables 2 из 0.5.0 читают только бинарные каталоги, если не собраны с `ENABLE_JSON_CATALOG`; сборка для Quest собрана с ним, ПК-игра — нет, поэтому `catalog_0.1.json` на ПК не грузится | Известно: в логе одно предупреждение, а в обучении на страницах инвентаря, карты и эмоций остаются тексты игры вместо VR-текстов RepoXR. Вернуть их можно бинарным каталогом, собранным Unity 6 |

## Что изменилось в 0.5.0 и что делает порт

Каждое исправление — один из патчей
[`patches/repoxr` у repo-quest-vr](https://github.com/crimecock/repo-quest-vr/tree/repo-0.5.0/patches/repoxr), с теми же номерами;
все они в коде `repo-0.5.0`.

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
| Хлыст в R.E.P.O. 0.5.0 щёлкает и бросает жестами: `ItemMeleeWhip.InputTick` читает смещение взгляда (`InputManager.GrabCursorLookGet`: мышь или правый стик) и систему координат камеры | В VR смещение всегда 0: хлыстом можно было только болтать | 0029 (`ItemMeleeWhipPatches`): пока хлыст читает ввод, смещение — угловая скорость руки в пространстве трекинга (вокруг вертикали и вокруг оси «влево-вправо» головы, поэтому замах из-за плеча не переворачивается), с множителями *Whip Sideways Sensitivity* (0,5) и *Whip Crack Sensitivity* (1); система координат — у руки, вдоль неё, без крена |
| RepoXR пропускает `CameraAim.Update`, а он ещё и снимает принудительные цели взгляда камеры (`AimTargetSet`) | Когда что-то ставило цель, `CameraAim.AimTargetActive` оставался true навсегда: жесты хлыста (`ItemMeleeWhip.GestureCanUse`) и взгляд курсора захвата не работали | 0029: пропущенное обновление по-прежнему считает таймеры цели взгляда, мягкой цели и остановки прицела; камера по-прежнему следует за шлемом |
| Передача сцены: VR-экран загрузки следующей сцены просыпается раньше, чем уничтожается старый | `OnDestroy` старого обнулял статический `instance`; `ResultScreenPatches` падал каждый кадр, и **переход уровень → магазин зависал на экране итогов** | 0029: `LoadingUI` обнуляет только свой `instance`; патчи экрана итогов и луны проверяют его |
| Попап новостей в главном меню вырастает из нуля (`MenuPageMain.RunMarketingPopupCurves`, масштаб из `Vector2`, z = 0) сразу после того, как ему задали тексты | В VR его слова рисовались сплошными плашками | 0029: масштаб попапов сохраняет z = 1 (`FlatScalePatches`), а оба текста новости перестраиваются, когда попап вырос (`MarketingPopupPatches`) |
| Бандл RepoXR собран TextMeshPro от Unity 2022: у набора значков глифы в `m_SpriteGlyphTable`, материал в `material` | TextMeshPro от Unity 6 читает `m_GlyphTable`/`m_Material`: без глифов имя спрайта не находилось (`?`), без материала найденные спрайты не рисовались | 0030 (`Utils.RepairSpriteAsset`): глифы восстанавливаются по сетке листа (100 px), на лист RepoXR копируется спрайтовый материал игры, таблицы поиска обновляются |
| Теги текста (`[move]`, `[grab]` …) переехали в `InputGlyphs.tagDictionary`; `InputManager.tagDictionary` никто не заполняет | Замена RepoXR не находила тегов: `[move]` в обучении и подсказках | 0030 (`InputPatches.NoUnderlinePatch`): теги из `InputGlyphs` |
| `MenuTwoOptions.OptionBoxX` сравнивает мировые x кнопок; новые поля подписей `option1TextMesh`/`option2TextMesh` | У меню в 3D сравнение переворачивается (рамка не на том варианте); у переключателей RepoXR (префабы 0.4.4) подписи-заглушки | 0030 (`MenuTwoOptionsPatches`): позиции в пространстве холста; подписи из текстов кнопок |
| `MenuButton.ResizeToText` возвращает кнопку на место, где она была в `Awake`, и вызывается снова, когда приходит перевод | Переставленная кнопка *Назад* возвращалась под *VR Settings* | 0030 (`MenuPatches`): исходное место переставленных кнопок — новое |
| `MenuTwoOptions.StartFetch` вызывает событие получения значения только с новым флагом `customFetch`; `OptionBoxX` отдаёт x опций переключателей самой игры (143.5/218.5) | Переключатели RepoXR (префабы 0.4.4) показывали одну из настроек игры, рамка стояла наполовину или целиком справа снаружи | 0031 (`MenuTwoOptionsPatches`): `customFetch` ставится для переключателей с событием получения; без кнопок опций игры рамка встаёт в центр собственных кнопок переключателя |
| Новое зеркало в магазине (`MirrorReflection`): отдельная камера рисует отражение в `Camera.onPreCull`, шейдер берёт его по отражённой экранной позиции | Камера отражения рисовала как стерео-камера, матрицы глаз XR подменяли её вид: каша в шлеме | 0031 (`MirrorPatches`): для камеры шлема камера отражения рисует моно, в pre-render каждого глаза, из отражённой позиции глаза с его проекцией (render pass XR-дисплея, сдвиг отражён); скрыто в single-pass и при выключенном *Shop Mirror* |
| Собственные теги RepoXR (`[MapGrabLeft]`…) лежат в `InputManager.tagDictionary`; VR-тексты обучения RepoXR есть только на английском и на ПК не грузятся | В обучении было `[MapGrabLeft]`; на ПК — тексты игры; на Quest неанглоязычный игрок получал бы английский | 0032 (`InputPatches`, `TutorialPatches`): заменяются оба словаря тегов; VR-тексты только для английского, на ПК встроены; страницы проверяются по тексту |
| Контроллеры Touch Plus у Quest 3 приходят через XR input subsystem (`XRInputV1::Oculus::MetaQuestTouchPlusControllerOpenXR`), их не называет ни одна схема RepoXR | Ввод работал, но PlayerInput ничего не спаривал: страница управления писала *DISCONNECTED*, переназначать было нельзя | 0032 (`VRInputSystem`): на Quest схема Meta Quest принимает любую пару XR-контроллеров, страница управления откатывается на неё; *Render Mode* виден в VR Settings на Quest |
| Проект RepoXR берёт R.E.P.O. и Unity из NuGet, а там только R.E.P.O. 0.4.4 и Unity 2022; код RepoXR написан на C# 14 | Форк не собирался из клона (только своим закрытым файлом проекта) | `RepoXR.csproj` (патч 0033 в repo-quest-vr): папка игры (`GamePath`) и XR-пакеты Unity 6 из архива «всё в одном» (`XRDeps`), с publicize там, где RepoXR лезет во внутренности; .NET 10 SDK. С `-p:DebugSymbols=false` код DLL (IL) — как в релизе: [COMPILING.md](../COMPILING.md) |

Ещё в ветке: настройки Dobutsu из repo-quest-vr (0001–0008: Head Bob, Post Processing, тени фонариков других
игроков, исправление сферы фокуса; Pixel Lights и Lamp Lighting — для Quest), счётчик FPS (VR Settings → Rendering)
и диагностика (0013, 0016, 0018, 0027).

## Захват: что сломалось и как починили

Захват работает на ПК и на Quest. В VR луч захвата идёт из руки, поэтому всё, где 0.5.0 целит захват от камеры или
мыши, пришлось перевести на руку.

| Патч | Где | Что было не так | Что сделали |
|---|---|---|---|
| 0019 `ForceHoldPatch` | `PhysGrabber.GrabInputActiveGet` | *Item Unequip Auto Hold* (новая настройка, по умолчанию включена) перенесла сюда из `Update` отпускание захвата-переключателя. Поиск по коду в патче ничего не нашёл, и вызов попал в задержку захвата в конце `Update`: IL правильный, но float там, где нужен bool, и **луч захвата не включался** | Перед `or; brtrue; ldsfld GameplayManager.instance; ldfld itemUnequipAutoHold` значение «захват зажат» заменяется на `ShouldToggleGrabOff(grabHeld, overrideGrabTimer)`; без якоря — `ThrowIfInvalid`. Нашёл это сторож якорей сборки для Quest: он сообщает о каждом поиске по коду, который ничего не нашёл |
| 0020 | `PhysGrabber.ForceGrabPhysObject`, `ObjectTurningRelease` | Оба брали трансформ камеры | Руку: предмет не прыгает после вращения |
| 0020 | `ItemGun.AimTargetUpdate` | Пушка поворачивалась по линии «камера → точка захвата» | От руки: пушка смотрит вдоль луча |
| 0009 `HandBasedTumbleClimbPatch` | `PhysGrabber.ClimbTargetUpdate` | 0.5.0 перенесла сюда поворот лазания из `GrabStateClimb`; старый транспайлер ломал конец `GrabStateClimb` | Палка лазания следует повороту руки. Проверено на ПК |
| 0019 | `InputKey.VehicleGrab` | Нет VR-действия | Grab RepoXR, переключателем, как в игре |
| alpha 2 `NoGrabCursorDrift` | `InputManager.GrabCursorDriftGet` | Дрейф курсора захвата для геймпада 0.5.0 (`PhysGrabber.GamepadGrabTargetGet`) сдвигает точку захвата вслед за взглядом | В VR 0: точка захвата остаётся на луче |
| 0018 | `[Grab]` в логе | — | Что VR-ввод сделал для захвата, для отчётов |

## Чем ПК отличается от Quest

`Plugin.IsQuest` (Android) не пускает в ПК-версию то, что нужно только Quest:

- Lamp Lighting на ПК не создаётся: ему нужны шейдеры сборки для Quest. Pixel Lights не применяется. Обе настройки и
  три настройки Level Culling, которые читают инструменты сборки Quest, скрыты из VR-настроек.
- На ПК — значения RepoXR по умолчанию: Head Bob 25%, пост-обработка включена, тени фонариков других игроков включены,
  однопроходный рендер выключен.
- На ПК остаются привязки RepoXR. В порте для Quest меню паузы переезжает на кнопку ≡ левого контроллера, а X выключает
  микрофон.

## Сборка

По папке игры: `dotnet build RepoXR.sln -c Release -p:GamePath=<папка с REPO_Data>`, в папку игры должен быть распакован архив «всё в одном» (XR-пакеты под Unity 6). По шагам: [COMPILING.md](../COMPILING.md#this-fork-repo-050--этот-форк).

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
