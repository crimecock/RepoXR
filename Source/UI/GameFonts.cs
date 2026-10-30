using System.Linq;
using RepoXR.Assets;
using TMPro;
using UnityEngine;

namespace RepoXR.UI;

/// <summary>
/// Quest port: RepoXR's own UI comes from an asset bundle built with Unity 2022 and TextMeshPro 3. Under Unity 6's
/// TextMeshPro (part of uGUI 2.0) the bundle's text arrives without a usable font: MenuButton.Awake threw in
/// TMP's MaterialReference while instantiating the VR Settings button, and the bundle's texts would not render.
/// Once the game's own menu is up, every text in the bundle's prefabs gets the game's menu font
/// </summary>
internal static class GameFonts
{
    private static bool done;

    public static void FixBundlePrefabs()
    {
        if (done)
            return;

        var reference = Object.FindObjectsOfType<MenuButton>(true)
            .Select(b => b.buttonText ? b.buttonText.font : null)
            .FirstOrDefault(Usable);
        if (!reference)
        {
            Logger.LogWarning("[Fonts] no game menu font found yet");
            return;
        }

        done = true;
        var fixedTexts = 0;
        foreach (var prefab in new[]
                 {
                     AssetCollection.RebindHeader, AssetCollection.RebindButton, AssetCollection.RebindButtonToggle,
                     AssetCollection.Keyboard, AssetCollection.ExpressionWheel, AssetCollection.ValuableDiscover,
                     AssetCollection.MenuSettings, AssetCollection.MenuSettingsCategory, AssetCollection.MenuShowcase,
                     AssetCollection.RuntimeSetting, AssetCollection.BoolSetting, AssetCollection.SliderSetting,
                     AssetCollection.VRSettingsButton, AssetCollection.VRRig, AssetCollection.VRTumble
                 })
            fixedTexts += Fix(prefab, reference);

        Logger.LogInfo($"[Fonts] {fixedTexts} texts in RepoXR's prefabs given the game's font {reference.name}");
    }

    /// <summary>Gives every text under <paramref name="root"/> without a usable font the reference font</summary>
    public static int Fix(GameObject? root, TMP_FontAsset reference)
    {
        if (!root)
            return 0;

        var count = 0;
        foreach (var text in root!.GetComponentsInChildren<TMP_Text>(true))
        {
            if (Usable(text.font) && text.fontSharedMaterial)
                continue;

            text.font = reference;
            text.fontSharedMaterial = reference.material;
            count++;
        }

        return count;
    }

    private static bool Usable(TMP_FontAsset? font)
    {
        try
        {
            return font && font!.material && font.atlasTexture;
        }
        catch
        {
            return false;
        }
    }
}
