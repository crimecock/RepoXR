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
    private static TMP_FontAsset? applied;
    private static bool fromMenu;
    private static bool reportedMissing;

    public static void FixBundlePrefabs()
    {
        if (fromMenu)
            return;

        // The menu's font if there is one, else TMP's default, else any loaded font that works. (1.0.7-qc11 to qc15
        // also wanted an atlas texture: Unity 6's dynamic fonts have none until they render a glyph, so no font ever
        // qualified, the bundle's texts stayed broken, and a level whose RepoXR prefabs show text threw on every
        // instantiation.) A stand-in applied at a scene load, before the menu exists, is replaced by the menu's font
        // once there is one (1.0.7-qc16 kept LiberationSans for good)
        var menuFont = Object.FindObjectsOfType<MenuButton>(true)
            .Select(b => b.buttonText ? b.buttonText.font : null)
            .FirstOrDefault(Usable);
        var reference = menuFont ??
                        (applied ? null :
                            (Usable(TMP_Settings.defaultFontAsset) ? TMP_Settings.defaultFontAsset : null) ??
                            Resources.FindObjectsOfTypeAll<TMP_FontAsset>().FirstOrDefault(Usable));
        if (!reference)
        {
            if (!applied && !reportedMissing)
            {
                reportedMissing = true;
                Logger.LogWarning("[Fonts] no usable font yet; loaded: " + string.Join(", ",
                    Resources.FindObjectsOfTypeAll<TMP_FontAsset>().Select(f =>
                        $"{f.name} (material {(f.material ? f.material.name : "none")}, atlases {f.atlasTextures?.Length ?? 0})")));
            }
            return;
        }

        var fixedTexts = 0;
        foreach (var prefab in new[]
                 {
                     AssetCollection.RebindHeader, AssetCollection.RebindButton, AssetCollection.RebindButtonToggle,
                     AssetCollection.Keyboard, AssetCollection.ExpressionWheel, AssetCollection.ValuableDiscover,
                     AssetCollection.MenuSettings, AssetCollection.MenuSettingsCategory, AssetCollection.MenuShowcase,
                     AssetCollection.RuntimeSetting, AssetCollection.BoolSetting, AssetCollection.SliderSetting,
                     AssetCollection.VRSettingsButton, AssetCollection.VRRig, AssetCollection.VRTumble
                 })
            fixedTexts += Fix(prefab, reference, applied);

        applied = reference;
        fromMenu = menuFont;
        Logger.LogInfo($"[Fonts] {fixedTexts} texts in RepoXR's prefabs given the font {reference.name}" +
                       (fromMenu ? " (the game's menu font)" : " (a stand-in until the menu's font is loaded)"));
    }

    /// <summary>Gives every text under <paramref name="root"/> without a usable font the reference font</summary>
    public static int Fix(GameObject? root, TMP_FontAsset reference, TMP_FontAsset? standIn = null)
    {
        if (!root)
            return 0;

        var count = 0;
        foreach (var text in root!.GetComponentsInChildren<TMP_Text>(true))
        {
            if (Usable(text.font) && text.fontSharedMaterial && (!standIn || text.font != standIn))
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
            // What TMP's MaterialReference needs: the bundle's fonts arrive without a working material
            return font && font!.material && font.material.shader;
        }
        catch
        {
            return false;
        }
    }
}
