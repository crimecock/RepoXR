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