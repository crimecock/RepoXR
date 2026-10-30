# Compiling

## This fork: R.E.P.O. 0.5.0 · Этот форк

**Why it differs from DaXcess's way below.** A mod is compiled against the game's own DLLs (its code, Unity, Photon,
Steamworks). DaXcess's RepoXR downloads them from NuGet, a package server for .NET, but NuGet only has R.E.P.O. 0.4.4
and Unity 2022. So this fork's `RepoXR.csproj` takes them from **your installed R.E.P.O. 0.5.0**, and the XR libraries
built for Unity 6 from this fork's all-in-one zip unpacked in the game folder. From NuGet it only downloads BepInEx,
CustomDiscoverStateLib (an optional mod RepoXR works with) and the publicizer (lets the mod use the game's private
members, as every R.E.P.O. mod does).

**You need**
- the [.NET SDK 10](https://dotnet.microsoft.com/download/dotnet/10.0) or newer. RepoXR uses C# 14 (`extension`
  blocks); .NET 8 stops with `CS0106`;
- R.E.P.O. 0.5.0 from Steam, with this fork's `…-all-in-one.zip` from [Releases](https://github.com/crimecock/RepoXR/releases)
  unpacked into the game folder (as for playing; its `BepInEx/plugins/RepoXR/RuntimeDeps` has the XR libraries);
- internet the first time (NuGet).

**Steps**

```sh
git clone https://github.com/crimecock/RepoXR.git -b repo-0.5.0
cd RepoXR
dotnet build RepoXR.sln -c Release -p:GamePath="C:\Program Files (x86)\Steam\steamapps\common\REPO"
```

Without git: the code as a [ZIP](https://github.com/crimecock/RepoXR/archive/refs/heads/repo-0.5.0.zip), unpacked (folder `RepoXR-repo-0.5.0`). `GamePath` is the folder with
`REPO.exe` and `REPO_Data` (Steam → R.E.P.O. → Properties → Installed Files → Browse); without it the default Steam
folder is used.

- The all-in-one zip unpacked somewhere else: add `-p:XRDeps=<that folder>/BepInEx/plugins/RepoXR/RuntimeDeps`.
- macOS or Linux work too, with a copy of the Windows game (for example the one repo-quest-vr's build script downloads
  into `~/RepoQuest/game`): `-p:GamePath=$HOME/RepoQuest/game`.

The DLLs land in `bin/Release/netstandard2.1`. To try yours, copy `RepoXR.dll` over `BepInEx/plugins/RepoXR/RepoXR.dll`
in the game folder; the all-in-one zip puts the original back. `RepoXR.Preload.dll` is DaXcess's code unchanged (the
releases ship DaXcess's 1.2.3 build of it).

The releases are built with `-p:DebugSymbols=false`. With it, `RepoXR.dll` has the same code (IL) as the release; the
default Release build also writes a PDB, and then the compiler keeps a few more local variables. A different .NET 10
version may lay out a few methods differently, with the same logic.

| Error | What to do |
|---|---|
| `CS0106` ("modifier is not valid for this item") | The .NET SDK is older than 10 |
| `R.E.P.O. not found in '…'` | Pass `-p:GamePath=<the folder with REPO_Data>` |
| `The Unity 6 XR packages are not in '…'` | Unpack the all-in-one zip into the game folder, or pass `-p:XRDeps=…` |
| `NU1301` / packages not found | No internet, or NuGet is blocked |

**По-русски.** Мод компилируется по DLL самой игры. RepoXR от DaXcess скачивает их из NuGet (сервер пакетов для .NET),
но там только R.E.P.O. 0.4.4 и Unity 2022, поэтому `RepoXR.csproj` этого форка берёт их из **вашей установленной
R.E.P.O. 0.5.0**, а XR-библиотеки под Unity 6 — из архива «всё в одном», распакованного в папку игры. Из NuGet
скачиваются только BepInEx, CustomDiscoverStateLib и publicizer. Нужны [.NET SDK 10](https://dotnet.microsoft.com/download/dotnet/10.0)
или новее (на .NET 8 — ошибки `CS0106`), игра с распакованным архивом «всё в одном» и интернет в первый раз. Команды —
выше; `GamePath` — папка с `REPO.exe`; архив распакован в другое место — `-p:XRDeps=…`. DLL — в
`bin/Release/netstandard2.1`; чтобы попробовать свою, положите `RepoXR.dll` вместо `BepInEx/plugins/RepoXR/RepoXR.dll`.
С `-p:DebugSymbols=false` код совпадает с релизом. Ошибки — в таблице выше. Пошагово по-русски — в
[README.ru.md](README.ru.md#сборка-из-исходников).

### The release zips · Архивы релиза

`tools/package/make_release.py` (Python 3, nothing to install) makes both zips of a release from scratch:

```sh
python3 tools/package/make_release.py alpha.7
```

- `dist/RepoXR-repo0.5.0-alpha.7.zip`: RepoXR alone, as a Thunderstore-style package (manifest, icon, README,
  CHANGELOG) for Gale;
- `dist/RepoXR-repo0.5.0-alpha.7-all-in-one.zip`: everything to unpack into the game folder.

What goes in, and from where (every download is checked by SHA-256 and kept in `work/downloads`):

| Part | From | Why |
|---|---|---|
| `RepoXR.dll` | your build (`bin/Release/netstandard2.1`, or `--dll`) | the mod |
| BepInEx 5.4.23.5 (all-in-one only) | [BepInEx's GitHub release](https://github.com/BepInEx/BepInEx/releases/tag/v5.4.23.5) | the mod loader; 5.4.23.5 fixes its log under Unity 6 |
| FixPluginTypesSerialization 1.1.1 (all-in-one only) | [DaXcess's package on Thunderstore](https://thunderstore.io/c/repo/p/DaXcess/FixPluginTypesSerialization/) | lets Unity see the mod's serializable types; works on Unity 6000.3 |
| `RepoXR.Preload.dll`, `repoxrassets`, `Addressables/` | [DaXcess's RepoXR 1.2.3 on Thunderstore](https://thunderstore.io/c/repo/p/DaXcess/RepoXR/) | unchanged: the preloader, RepoXR's asset bundle and translations |
| Unity 6 XR libraries (`RuntimeDeps`) and `UnityOpenXR.dll`, `openxr_loader.dll` | this fork's alpha 6 release, or built yourself (below) | RepoXR 1.2.3 ships Unity 2022 ones, which do not load in Unity 6 |
| `LICENSE`, `README.md`, `CHANGELOG.md`, `RepoXR-README.txt` | this repository | |

To build the XR libraries yourself too, you need a Unity 6 project with the XR packages that was built once for a
player. repo-quest-vr's Quest project is one: after a Quest build it is `~/RepoQuest/work/unity/REPOQuest`.

```sh
python3 tools/package/make_release.py alpha.7 --unity-project ~/RepoQuest/work/unity/REPOQuest --game ~/RepoQuest/game --dotnet ~/.dotnet10/dotnet
```

[`tools/xr-deps/build.py`](tools/xr-deps/build.py) compiles each XR package again with the arguments Unity used, for
Windows and against the PC game's DLLs; the native OpenXR DLLs come from the project's OpenXR package (1.14.3,
`Runtime/windows/x64` and `RuntimeLoaders/windows/x64`).

Checked: with alpha 6's DLL, the all-in-one zip comes out byte for byte as the alpha 6 release (SHA-256 6063f8fb…).
Keep the alpha 6 release and its zips on GitHub: this script and repo-quest-vr's `scripts/prepare-repoxr.sh` take the
Unity 6 XR libraries from there by URL and SHA-256. New releases, pre-releases or not, change nothing for them.
· **По-русски:** `python3 tools/package/make_release.py alpha.7` собирает оба архива релиза с нуля: BepInEx — с GitHub,
FixPluginTypesSerialization и неизменные части RepoXR 1.2.3 — с Thunderstore, XR-библиотеки Unity 6 — из релиза alpha 6
или сами (`--unity-project` с Quest-проектом repo-quest-vr), ваша `RepoXR.dll`. Каждое скачивание проверяется по
SHA-256. С DLL alpha 6 архив «всё в одном» выходит побайтно как в релизе alpha 6.

### Releasing · Выпуск

A release here: `CHANGELOG.md` `# Unreleased` → `# 1.2.3-repo0.5.0-alpha.N (unofficial fork)`, the README (status,
known issues, the direct link to the new all-in-one zip), `make_release.py alpha.N`, then a GitHub release with tag
`v1.2.3-repo0.5.0-alpha.N` and both zips. The same code then goes to the Quest build as a patch. The whole checklist
for both repositories, where each version string lives and what must not break: [releasing.md](https://github.com/crimecock/repo-quest-vr/blob/repo-0.5.0/docs/releasing.md).
· Выпуск: журнал, README (прямая ссылка на новый архив), `make_release.py alpha.N`, релиз с тегом и двумя архивами;
потом тот же код идёт в сборку Quest патчем. Полный список для обеих реп — [releasing.ru.md](https://github.com/crimecock/repo-quest-vr/blob/repo-0.5.0/docs/releasing.ru.md).

The rest of this file is DaXcess's original guide for RepoXR on R.E.P.O. 0.4.4.

## Initial setup

We recommend using Visual Studio with the .NET development features to compile this project. You can [download Visual Studio here](https://visualstudio.microsoft.com/downloads/). You should use the Community edition as that one is free.

Alternatively you may also manually install the .NET development tools and compile the project from command line. You will need to [download the .NET SDK 8.0](https://dotnet.microsoft.com/en-us/download/dotnet/8.0) or higher, since this project is using C# 12.

You can download it from the link above, or alternatively if you are using Arch Linux you can install dotnet using the following command:

```sh
$ pacman -S dotnet-sdk
```

## Building the assembly

Now that the project is set up, you may compile it by either building it using Visual Studio, or by running the following command in the project root:

```sh
$ dotnet build
```

Alternatively if you're building a finished product run this command:

```sh
$ dotnet build --configuration Release
```

The built plugin assemblies can now be found inside the `bin` folder.

## For R.E.P.O. Testers

The main RepoXR codebase is designed to be compatible with all official R.E.P.O. releases (including some tester builds).  
However, if you want to develop against **private tester builds**, there are a few extra steps to follow.

Since tester builds are private, there are **no public NuGet packages available** for these versions. To let your local build reference the tester assemblies:

Add the following content to `RepoXR.csproj.user` (in the same directory as the `RepoXR.csproj` file), updating the `TesterGamePath` if your game is installed elsewhere:

```xml
<Project>
    <PropertyGroup>
        <USE_TESTER>true</USE_TESTER>
        <TesterGamePath>C:\Program Files (x86)\Steam\steamapps\common\REPO</TesterGamePath>
    </PropertyGroup>
</Project>
```

> **Warning**: Do not share game code or mod code that targets private tester builds until those builds are officially public. Use private repositories or branches, and only merge changes into the main repository once the builds are public.