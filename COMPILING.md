# Compiling

## This fork: R.E.P.O. 0.5.0 · Этот форк

NuGet only has R.E.P.O. 0.4.4 and Unity 2022, so this fork builds against **your game folder**: the game's code,
Unity 6, Photon, Steamworks and the XR packages built for Unity 6 come from there (see `RepoXR.csproj`). NuGet only
provides BepInEx, CustomDiscoverStateLib and the publicizer.

You need:
- the [.NET SDK 10](https://dotnet.microsoft.com/download/dotnet/10.0) or newer: RepoXR uses C# 14 (`extension` blocks), which older SDKs reject with `CS0106`;
- R.E.P.O. 0.5.0 from Steam, with this fork's `…-all-in-one.zip` from [Releases](https://github.com/crimecock/RepoXR/releases)
  unpacked into the game folder (its `BepInEx/plugins/RepoXR/RuntimeDeps` has the XR packages);
- internet for NuGet.

```sh
git clone https://github.com/crimecock/RepoXR.git -b repo-0.5.0
cd RepoXR
dotnet build RepoXR.sln -c Release
```

- The game somewhere else than `C:\Program Files (x86)\Steam\steamapps\common\REPO`: add
  `-p:GamePath="D:\SteamLibrary\steamapps\common\REPO"` (the folder with `REPO_Data`).
- The all-in-one zip unpacked elsewhere: `-p:XRDeps=<that folder>/BepInEx/plugins/RepoXR/RuntimeDeps`.
- macOS or Linux work too, with a copy of the Windows game (for example the one repo-quest-vr's build script downloads
  into `~/RepoQuest/game`).

The DLLs land in `bin/Release/netstandard2.1`. To try yours, copy `RepoXR.dll` over `BepInEx/plugins/RepoXR/RepoXR.dll`
in the game folder; the all-in-one zip puts the original back. `RepoXR.Preload.dll` is DaXcess's code unchanged (the
releases ship DaXcess's 1.2.3 build of it).

The releases are built with `-p:DebugSymbols=false`. With it, `RepoXR.dll` has the same code (IL) as the release; the
default Release build also writes a PDB, and then the compiler keeps a few more local variables.

**По-русски.** В NuGet есть только R.E.P.O. 0.4.4 и Unity 2022, поэтому форк собирается **по папке игры**: код игры,
Unity 6, Photon, Steamworks и XR-пакеты под Unity 6 берутся оттуда. Нужны .NET SDK 10 или новее (RepoXR написан на C# 14, старые SDK падают с `CS0106`), R.E.P.O. 0.5.0 из Steam
с распакованным в папку игры архивом «всё в одном» из [релизов](https://github.com/crimecock/RepoXR/releases) и
интернет для NuGet. Команды — выше; игра в другой папке — `-p:GamePath=…` (папка с `REPO_Data`), архив распакован в
другое место — `-p:XRDeps=<папка>/BepInEx/plugins/RepoXR/RuntimeDeps`. DLL — в `bin/Release/netstandard2.1`; чтобы
попробовать свою, положите `RepoXR.dll` вместо `BepInEx/plugins/RepoXR/RepoXR.dll` в папке игры. Релизы собираются с
`-p:DebugSymbols=false`: так код `RepoXR.dll` совпадает с релизом.

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