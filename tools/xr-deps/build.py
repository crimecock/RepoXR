"""XR runtime libraries for RepoXR on R.E.P.O. 0.5.0 (Unity 6000.3, Windows, Mono), built from Unity 6 packages."""
import os,re,subprocess,sys
# Usage: build.py <Unity 6 project> <REPO_Data/Managed of the PC game> <out folder> <csc.dll> [assembly...]
# <Unity 6 project>: a project with com.unity.xr.openxr 1.14.3, com.unity.xr.management 4.5.0,
# com.unity.xr.interaction.toolkit 2.6.4, com.unity.xr.core-utils 2.6.0 and com.unity.xr.legacyinputhelpers 2.1.13
# that has been built once for a player (its Library/Bee/artifacts/*P.dag/<assembly>.rsp are the compiler's own
# arguments). Each assembly is compiled again with those arguments for Windows: the target platform's defines are
# swapped for UNITY_STANDALONE_WIN's, Unity's engine and .NET references for the game's own, and the XR packages for
# each other's new builds. Order: SpatialTracking, LegacyInputHelpers, CoreUtils, Management, OpenXR, Interaction.Toolkit
import glob
SRC=sys.argv[1]; GAME=sys.argv[2]; OUT=sys.argv[3]; CSC=['dotnet',sys.argv[4]]
DAG=glob.glob(f'{SRC}/Library/Bee/artifacts/*P.dag')[0]
DROP=re.compile(r'^(PLATFORM_ANDROID|UNITY_ANDROID.*|ENABLE_EGL|ENABLE_IL2CPP|ENABLE_ANDROID_.*|PLATFORM_EXTENDS_VULKAN_.*|UNITY_HAS_GOOGLEVR|UNITY_HAS_TANGO|ENABLE_ETC_COMPRESSION|ENABLE_FIREBASE_IDENTIFIERS|PLATFORM_HAS_BUGGY_MSAA_RESOLVE|PLATFORM_REQUIRES_TETHERED_VULKAN_COMMAND_POOL|PLATFORM_IMPLEMENTS_INSIGHTS_ANR|PLATFORM_SUPPORTS_INSIGHTS_DEVICE_INFO|ENABLE_INSIGHTS_PLATFORM_SPECIFIC_RESOURCES|PLATFORM_HAS_MULTIPLE_SWAPCHAINS|PLATFORM_HAS_ADDITIONAL_API_CHECKS|PLATFORM_HAS_GRAPHICS_JOBS_SUPPORT_CHECK_OVERRIDE|PLATFORM_SUPPORTS_SPLIT_GRAPHICS_JOBS|ENABLE_UNITYADS_RUNTIME|UNITY_UNITYADS_API)$')
ADD=['UNITY_STANDALONE_WIN','UNITY_STANDALONE','PLATFORM_STANDALONE_WIN','PLATFORM_STANDALONE','ENABLE_MONO','UNITY_64','ENABLE_CLUSTER_SYNC','ENABLE_CLUSTERINPUT','ENABLE_NVIDIA','ENABLE_AMD','ENABLE_OUT_OF_PROCESS_CRASH_HANDLER','ENABLE_WEBSOCKET_HOST','ENABLE_MONO_BDWGC','ENABLE_PROFILER']
XR={'Unity.XR.CoreUtils','Unity.XR.Management','Unity.XR.OpenXR','UnityEngine.SpatialTracking','Unity.XR.Interaction.Toolkit','UnityEngine.XR.LegacyInputHelpers'}
def build(name):
    rsp=open(f'{DAG}/{name}.rsp').read().splitlines()
    args=['-nostdlib+','-noconfig','-target:library',f'-out:{OUT}/{name}.dll','/optimize+','/deterministic','/debug:portable','-warn:0','-nologo','/utf8output','-langversion:9.0']
    refs=set(); srcs=[]; missing=[]
    for l in rsp:
        if l.startswith('-define:'):
            d=l[8:]
            if not DROP.match(d): args.append('-define:'+d)
        elif l.startswith('-r:'):
            p=l[3:].strip('"'); b=os.path.basename(p).replace('.ref.dll','.dll'); n=b[:-4]
            if n in XR:
                refs.add(f'{OUT}/{b}')
            elif os.path.exists(f'{GAME}/{b}'):
                refs.add(f'{GAME}/{b}')
            else: missing.append(b)
        elif l.endswith('.cs') or l.endswith('.cs"'):
            srcs.append(f'{SRC}/'+l.strip('"'))
        elif l in ('/unsafe+',) or l.startswith('/nowarn'):
            args.append(l)
    for d in ADD: args.append('-define:'+d)
    for f in ['mscorlib.dll','netstandard.dll','System.dll','System.Core.dll','System.Xml.dll','System.Runtime.dll']:
        if os.path.exists(f'{GAME}/{f}'): refs.add(f'{GAME}/{f}')
    args += ['-r:'+r for r in sorted(refs)] + srcs
    open(f'{OUT}/{name}.win.rsp','w').write('\n'.join(args))
    r=subprocess.run(CSC+[f'@{OUT}/{name}.win.rsp'],capture_output=True,text=True)
    errs=[x for x in r.stdout.splitlines() if ' error ' in x]
    print(f'{name}: rc={r.returncode} errors={len(errs)} missing refs={sorted(set(missing))[:12]}')
    for e in errs[:15]: print('   ',e[-260:])
    return r.returncode==0
os.makedirs(OUT,exist_ok=True)
for n in sys.argv[5:] or ['UnityEngine.SpatialTracking','UnityEngine.XR.LegacyInputHelpers','Unity.XR.CoreUtils','Unity.XR.Management','Unity.XR.OpenXR','Unity.XR.Interaction.Toolkit']:
    if not build(n): sys.exit(1)
