"""The release zips of this fork from scratch: RepoXR-repo0.5.0-<version>.zip (RepoXR alone, a Thunderstore-style
package for Gale) and RepoXR-repo0.5.0-<version>-all-in-one.zip (to unpack into the game folder).

    python3 tools/package/make_release.py <version> [--dll <RepoXR.dll>] [--unity-project <dir> --game <dir>]

<version>         e.g. alpha.7 (file names; "alpha 7" in the all-in-one README)
--dll             RepoXR.dll to ship (default bin/Release/netstandard2.1/RepoXR.dll: build it first, COMPILING.md)
--unity-project   build the Unity 6 XR libraries yourself (tools/xr-deps/build.py) from a Unity 6 project with the XR
--game            packages that was built once for a player (repo-quest-vr's Quest project: ~/RepoQuest/work/unity/
--dotnet          REPOQuest), against the PC game (the folder with REPO_Data); dotnet with a C# compiler (default:
                  dotnet on PATH). The native OpenXR DLLs come from the project's OpenXR package then.
                  Without these, the XR libraries are taken from this fork's alpha 6 release (checked by SHA-256).
--downloads       where downloads are kept (default work/downloads)
--out             where the zips go (default dist)

What goes in, and from where (each download is checked by SHA-256):
  BepInEx 5.4.23.5 (all-in-one only)                 github.com/BepInEx/BepInEx release
  FixPluginTypesSerialization 1.1.1 (all-in-one)     thunderstore.io, DaXcess's package (FPTS makes Unity see the
                                                     mod's serializable types; works on Unity 6000.3)
  RepoXR.Preload.dll, repoxrassets, Addressables     thunderstore.io, DaXcess's RepoXR 1.2.3 (unchanged: the preloader,
                                                     RepoXR's asset bundle and translations)
  Unity 6 XR libraries, UnityOpenXR.dll,             this fork's alpha 6 release, or --unity-project (see above)
  openxr_loader.dll
  RepoXR.dll                                         --dll
  LICENSE, README.md, CHANGELOG.md                   this repository; icon.png from DaXcess's package
"""
import argparse, hashlib, io, json, os, shutil, subprocess, sys, tarfile, urllib.request, zipfile, glob

ROOT = os.path.abspath(os.path.join(os.path.dirname(__file__), "..", ".."))
DOWNLOADS = {
    "bepinex": ("https://github.com/BepInEx/BepInEx/releases/download/v5.4.23.5/BepInEx_win_x64_5.4.23.5.zip",
                "82f9878551030f54657792c0740d9d51a09500eeae1fba21106b0c441e6732c4"),
    "fpts": ("https://thunderstore.io/package/download/DaXcess/FixPluginTypesSerialization/1.1.1/",
             "aca18b08f7f092538fe6fb22755c6ca00514c50380875ced0c1e8e3d9b0f4ff7"),
    "repoxr123": ("https://thunderstore.io/package/download/DaXcess/RepoXR/1.2.3/",
                  "bc6459b97a7663d25059fc7d8c362bbd42d5159b12c5969ad1448a498f77f35e"),
    "alpha6": ("https://github.com/crimecock/RepoXR/releases/download/v1.2.3-repo0.5.0-alpha.6/RepoXR-repo0.5.0-alpha.6.zip",
               "6ddc79a00e3035ab715461763d5eefc65c3d0ba5ac7b61a704fc09319c6291be"),
}
XR_MANAGED = ["Unity.XR.CoreUtils.dll", "Unity.XR.Interaction.Toolkit.dll", "Unity.XR.Management.dll",
              "Unity.XR.OpenXR.dll", "UnityEngine.SpatialTracking.dll", "UnityEngine.XR.LegacyInputHelpers.dll"]
XR_NATIVE = ["UnityOpenXR.dll", "openxr_loader.dll"]
STAMP = (2026, 10, 30, 0, 0, 0)
MANIFEST = {
    "name": "RepoXR",
    "description": "Unofficial port of RepoXR 1.2.3 to R.E.P.O. 0.5.0 (Unity 6): plays in VR on PC",
    "version_number": "1.2.3",
    "dependencies": ["BepInEx-BepInExPack-5.4.2305", "DaXcess-FixPluginTypesSerialization-1.1.1"],
    "website_url": "https://github.com/crimecock/RepoXR",
}


def die(msg):
    sys.exit(f"ERROR: {msg}")


def sha256(data):
    return hashlib.sha256(data).hexdigest()


def fetch(name, folder):
    url, digest = DOWNLOADS[name]
    path = os.path.join(folder, f"{name}.zip")
    if not os.path.exists(path) or sha256(open(path, "rb").read()) != digest:
        print(f"downloading {url}")
        os.makedirs(folder, exist_ok=True)
        request = urllib.request.Request(url, headers={"User-Agent": "RepoXR-make-release"})
        data = urllib.request.urlopen(request).read()
        if sha256(data) != digest:
            die(f"{url}: SHA-256 {sha256(data)}, expected {digest}")
        open(path, "wb").write(data)
    return zipfile.ZipFile(path)


def read(z, name):
    try:
        return z.read(name)
    except KeyError:
        die(f"{name} is not in {z.filename}")


def xr_from_project(project, game, dotnet, work):
    """Builds the managed XR libraries with tools/xr-deps/build.py; the natives come from the OpenXR package"""
    sdks = subprocess.run([dotnet, "--list-sdks"], capture_output=True, text=True).stdout.split("\n")
    sdk = [line.split(" [")[0] for line in sdks if line.strip()]
    base = [line.split("[")[1].rstrip("]") for line in sdks if line.strip()]
    if not sdk:
        die(f"no .NET SDK in {dotnet}")
    csc = os.path.join(base[-1], sdk[-1], "Roslyn", "bincore", "csc.dll")
    out = os.path.join(work, "xr-deps")
    env = dict(os.environ, PATH=os.path.dirname(os.path.abspath(dotnet)) + os.pathsep + os.environ.get("PATH", "")) \
        if os.path.sep in dotnet else None
    managed = os.path.join(game, "REPO_Data", "Managed")
    subprocess.run([sys.executable, os.path.join(ROOT, "tools", "xr-deps", "build.py"), project, managed, out, csc],
                   check=True, env=env)
    files = {name: open(os.path.join(out, name), "rb").read() for name in XR_MANAGED}
    openxr = glob.glob(os.path.join(project, "Library", "PackageCache", "com.unity.xr.openxr@*"))
    if not openxr:
        die(f"no com.unity.xr.openxr package in {project}/Library/PackageCache")
    natives = {"UnityOpenXR.dll": os.path.join(openxr[0], "Runtime", "windows", "x64", "UnityOpenXR.dll"),
               "openxr_loader.dll": os.path.join(openxr[0], "RuntimeLoaders", "windows", "x64", "openxr_loader.dll")}
    return files, {name: open(path, "rb").read() for name, path in natives.items()}


def write_zip(path, entries):
    with zipfile.ZipFile(path, "w", zipfile.ZIP_DEFLATED, compresslevel=9) as z:
        for name, data in entries:
            info = zipfile.ZipInfo(name, STAMP)
            info.compress_type = zipfile.ZIP_DEFLATED
            info.external_attr = 0o644 << 16
            z.writestr(info, data)
    print(f"{os.path.relpath(path)}  sha256 {sha256(open(path, 'rb').read())}")


def main():
    p = argparse.ArgumentParser(description=__doc__, formatter_class=argparse.RawDescriptionHelpFormatter)
    p.add_argument("version")
    p.add_argument("--dll", default=os.path.join(ROOT, "bin", "Release", "netstandard2.1", "RepoXR.dll"))
    p.add_argument("--unity-project")
    p.add_argument("--game")
    p.add_argument("--dotnet", default="dotnet")
    p.add_argument("--downloads", default=os.path.join(ROOT, "work", "downloads"))
    p.add_argument("--out", default=os.path.join(ROOT, "dist"))
    a = p.parse_args()
    if not os.path.exists(a.dll):
        die(f"{a.dll} not found: build RepoXR first (COMPILING.md) or pass --dll")
    if bool(a.unity_project) != bool(a.game):
        die("--unity-project needs --game (and the other way round)")

    bepinex = fetch("bepinex", a.downloads)
    fpts = fetch("fpts", a.downloads)
    upstream = fetch("repoxr123", a.downloads)
    if a.unity_project:
        managed, natives = xr_from_project(a.unity_project, a.game, a.dotnet, os.path.join(ROOT, "work"))
        print("XR libraries: built from", a.unity_project)
    else:
        alpha6 = fetch("alpha6", a.downloads)
        managed = {n: read(alpha6, f"BepInEx/plugins/RepoXR/RuntimeDeps/{n}") for n in XR_MANAGED}
        natives = {n: read(alpha6, f"BepInEx/patchers/RepoXR/RuntimeDeps/{n}") for n in XR_NATIVE}
        print("XR libraries: from this fork's alpha 6 release")

    addressables = sorted(n for n in upstream.namelist()
                          if n.startswith("BepInEx/plugins/RepoXR/Addressables/") and not n.endswith("/"))
    repoxr = [
        ("BepInEx/patchers/RepoXR/RepoXR.Preload.dll", read(upstream, "BepInEx/patchers/RepoXR/RepoXR.Preload.dll")),
        *[(f"BepInEx/patchers/RepoXR/RuntimeDeps/{n}", natives[n]) for n in XR_NATIVE],
        ("BepInEx/plugins/RepoXR/RepoXR.dll", open(a.dll, "rb").read()),
        ("BepInEx/plugins/RepoXR/repoxrassets", read(upstream, "BepInEx/plugins/RepoXR/repoxrassets")),
        *[(n, read(upstream, n)) for n in addressables],
        *[(f"BepInEx/plugins/RepoXR/RuntimeDeps/{n}", managed[n]) for n in XR_MANAGED],
    ]
    license_ = open(os.path.join(ROOT, "LICENSE"), "rb").read()
    package = [
        ("CHANGELOG.md", open(os.path.join(ROOT, "CHANGELOG.md"), "rb").read()),
        ("LICENSE", license_),
        ("README.md", open(os.path.join(ROOT, "README.md"), "rb").read()),
        ("icon.png", read(upstream, "icon.png")),
        ("manifest.json", (json.dumps(MANIFEST, indent=2) + "\n").encode()),
    ] + repoxr

    readme = open(os.path.join(ROOT, "tools", "package", "RepoXR-README.txt"), encoding="utf-8").read()
    readme = readme.replace("{version}", a.version.replace(".", " "))
    core = sorted(n for n in bepinex.namelist() if n.startswith("BepInEx/core/") and not n.endswith("/"))
    all_in_one = [
        (".doorstop_version", read(bepinex, ".doorstop_version")),
        ("RepoXR-README.txt", readme.encode("utf-8")),
        ("doorstop_config.ini", read(bepinex, "doorstop_config.ini")),
        ("winhttp.dll", read(bepinex, "winhttp.dll")),
        ("BepInEx/changelog.txt", read(bepinex, "changelog.txt")),
        *[(n, read(bepinex, n)) for n in core],
        ("BepInEx/patchers/FixPluginTypesSerialization/FixPluginTypesSerialization.dll",
         read(fpts, "BepInEx/patchers/FixPluginTypesSerialization/FixPluginTypesSerialization.dll")),
        *repoxr[:3],
        ("BepInEx/plugins/RepoXR/LICENSE", license_),
        *repoxr[3:],
    ]
    os.makedirs(a.out, exist_ok=True)
    write_zip(os.path.join(a.out, f"RepoXR-repo0.5.0-{a.version}.zip"), package)
    write_zip(os.path.join(a.out, f"RepoXR-repo0.5.0-{a.version}-all-in-one.zip"), all_in_one)


if __name__ == "__main__":
    main()
