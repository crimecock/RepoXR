RepoXR for R.E.P.O. 0.5.0 - all in one, {version} (unofficial)
https://github.com/crimecock/RepoXR

EN
Unpack everything into the R.E.P.O. folder (next to REPO.exe; Steam -> R.E.P.O. -> Properties -> Installed Files
-> Browse), connect your headset and start the game from Steam. Nothing else to install.
- Contains BepInEx 5.4.23 (BepInExPack), FixPluginTypesSerialization 1.1.1 (DaXcess) and RepoXR for 0.5.0.
- If another BepInEx or RepoXR 1.2.3 is already installed there, delete BepInEx/plugins/RepoXR and
  BepInEx/patchers/RepoXR first. Do not use it together with a mod manager profile of RepoXR.
- Updating an older install: unpack this archive into the game folder again and replace the files; your settings
  (BepInEx/config/io.daxcess.repoxr.cfg) stay.
- If the headset stays black: Steam launch option -force-d3d11.
- Problems: attach BepInEx/LogOutput.log to https://github.com/crimecock/RepoXR/issues (not to DaXcess).
- Uninstall: delete winhttp.dll, doorstop_config.ini, .doorstop_version and the BepInEx folder.
Details, ways to start in VR (SteamVR, Meta Quest Link, Virtual Desktop) and risks:
https://github.com/crimecock/RepoXR/blob/repo-0.5.0/docs/repo-0.5.0.md

RU
Распакуйте всё в папку R.E.P.O. (рядом с REPO.exe; Steam -> R.E.P.O. -> Свойства -> Установленные файлы ->
Обзор), подключите шлем и запустите игру из Steam. Больше ничего ставить не нужно.
- Внутри: BepInEx 5.4.23 (BepInExPack), FixPluginTypesSerialization 1.1.1 (DaXcess) и RepoXR для 0.5.0.
- Если там уже стоят другой BepInEx или RepoXR 1.2.3, сначала удалите BepInEx/plugins/RepoXR и
  BepInEx/patchers/RepoXR. Не используйте вместе с профилем RepoXR в менеджере модов.
- Обновление старой установки: снова распакуйте этот архив в папку игры с заменой файлов; ваши настройки
  (BepInEx/config/io.daxcess.repoxr.cfg) сохранятся.
- Если в шлеме черно: параметр запуска Steam -force-d3d11.
- Проблемы: прикладывайте BepInEx/LogOutput.log в https://github.com/crimecock/RepoXR/issues (не DaXcess).
- Удаление: удалите winhttp.dll, doorstop_config.ini, .doorstop_version и папку BepInEx.
Подробности, способы запуска в VR (SteamVR, Meta Quest Link, Virtual Desktop) и риски:
https://github.com/crimecock/RepoXR/blob/repo-0.5.0/docs/repo-0.5.0.ru.md

Third-party components / Сторонние компоненты
- BepInEx 5.4.23 (with HarmonyX, MonoMod, Mono.Cecil, Unity Doorstop): LGPL-2.1, https://github.com/BepInEx/BepInEx
- FixPluginTypesSerialization 1.1.1 (DaXcess fork of xiaoxiao921's): LGPL-3.0,
  https://github.com/DaXcess/FixPluginTypesSerialization
- RepoXR (DaXcess), with this fork's changes: GPL-3.0, BepInEx/plugins/RepoXR/LICENSE, source at the link above
- Unity OpenXR plugin 1.14.3, XR Management, XR Interaction Toolkit, XR Core Utils, Legacy Input Helpers:
  Unity Companion License, https://unity.com/legal/licenses/unity-companion-license
- OpenXR loader (Khronos): Apache-2.0, https://github.com/KhronosGroup/OpenXR-SDK
