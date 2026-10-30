using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using HarmonyLib;
using RepoXR.Assets;
using RepoXR.Managers;
using RepoXR.Networking;
using Steamworks;
using Unity.XR.CoreUtils;
using UnityEngine;

namespace RepoXR;

internal static class Utils
{
    public static string ToHumanReadable(string input)
    {
        var builder = new StringBuilder(input[0].ToString());
        if (builder.Length <= 0)
            return builder.ToString();

        for (var index = 1; index < input.Length; index++)
        {
            var prevChar = input[index - 1];
            var nextChar = index + 1 < input.Length ? input[index + 1] : '\0';

            var isNextLower = char.IsLower(nextChar);
            var isNextUpper = char.IsUpper(nextChar);
            var isPresentUpper = char.IsUpper(input[index]);
            var isPrevLower = char.IsLower(prevChar);
            var isPrevUpper = char.IsUpper(prevChar);

            if (!string.IsNullOrWhiteSpace(prevChar.ToString()) &&
                ((isPrevUpper && isPresentUpper && isNextLower) ||
                 (isPrevLower && isPresentUpper && isNextLower) ||
                 (isPrevLower && isPresentUpper && isNextUpper)))
                builder.Append(' ');

            builder.Append(input[index]);
        }

        return builder.ToString();
    }

    public static string GetControlSpriteString(string controlPath)
    {
        if (string.IsNullOrEmpty(controlPath))
            return "<b><u>NOT BOUND</u></b>";

        var path = Regex.Replace(controlPath.ToLowerInvariant(), @"<[^>]+>([^ ]+)", "$1");
        var hand = path.Split('/')[0].TrimStart('{').TrimEnd('}');
        controlPath = Regex.Replace(string.Join("/", path.Split('/').Skip(1)), @"{(.*)}", "$1");

        var id = (hand, controlPath) switch
        {
            ("lefthand", "primary2daxis" or "thumbstick") => "leftStick",
            ("lefthand", "primary2daxisclick" or "thumbstickclicked") => "leftStickClick",
            ("lefthand", "primary2daxis/up" or "thumbstick/up") => "leftStickUp",
            ("lefthand", "primary2daxis/down" or "thumbstick/down") => "leftStickDown",
            ("lefthand", "primary2daxis/left" or "thumbstick/left") => "leftStickLeft",
            ("lefthand", "primary2daxis/right" or "thumbstick/right") => "leftStickRight",
            ("lefthand", "primarybutton" or "primarypressed") => "leftPrimaryButton",
            ("lefthand", "secondarybutton" or "secondarypressed") => "leftSecondaryButton",
            ("lefthand", "triggerbutton" or "trigger" or "triggerpressed") => "leftTrigger",
            ("lefthand", "gripbutton" or "grip" or "grippressed") => "leftGrip",

            ("righthand", "primary2daxis" or "thumbstick") => "rightStick",
            ("righthand", "primary2daxisclick" or "thumbstickclicked") => "rightStickClick",
            ("righthand", "primary2daxis/up" or "thumbstick/up") => "rightStickUp",
            ("righthand", "primary2daxis/down" or "thumbstick/down") => "rightStickDown",
            ("righthand", "primary2daxis/left" or "thumbstick/left") => "rightStickLeft",
            ("righthand", "primary2daxis/right" or "thumbstick/right") => "rightStickRight",
            ("righthand", "primarybutton" or "primarypressed") => "rightPrimaryButton",
            ("righthand", "secondarybutton" or "secondarypressed") => "rightSecondaryButton",
            ("righthand", "triggerbutton" or "trigger" or "triggerpressed") => "rightTrigger",
            ("righthand", "gripbutton" or "grip" or "grippressed") => "rightGrip",

            (_, "menu" or "menubutton" or "menupressed") => "menuButton",

            _ => "unknown"
        };

        return $"""<sprite name="{id}">""";
    }

    /// <summary>
    /// Makes sure a text can draw <see cref="GetControlSpriteString"/>'s VR controller icons. R.E.P.O. 0.5.0 gives
    /// its keybind texts its own glyph sprite assets (keyboard, gamepads), which have none of them: the names did not
    /// resolve and the controls page showed question marks. RepoXR's input sprite asset becomes the text's sprite asset
    /// when it has none, or a fallback of the one it has
    /// </summary>
    /// <summary>
    /// RepoXR's controller icons (TMPInputsSpriteAsset) were made with the TextMeshPro of Unity 2022, which kept the
    /// sprites' places on the sheet in m_SpriteGlyphTable. Unity 6's TextMeshPro reads m_GlyphTable, the asset came up
    /// with no glyphs, its name lookup stayed empty (a sprite is only listed when its glyph is found) and every
    /// &lt;sprite name=...&gt; was drawn as a "?". The sheet is a plain grid of 100 px cells, glyph 0 at the top left, so
    /// the glyphs are put back from that when they are missing
    /// </summary>
    public static void RepairSpriteAsset(TMPro.TMP_SpriteAsset? asset)
    {
        if (!asset)
            return;

        var characters = asset!.spriteCharacterTable;
        var glyphs = asset.spriteGlyphTable;
        var sheet = asset.spriteSheet;
        Logger.LogInfo($"[UI] Sprite asset {asset.name}: {characters?.Count ?? -1} characters, {glyphs?.Count ?? -1} glyphs, " +
                       $"sheet {(sheet ? $"{sheet.width}x{sheet.height}" : "none")}, material {(asset.material ? asset.material.name : "none")}");

        if (characters is { Count: > 0 } && sheet && (glyphs == null || glyphs.Count < characters.Count))
        {
            const int cell = 100;
            var columns = Mathf.Max(1, sheet.width / cell);
            var rows = Mathf.Max(1, sheet.height / cell);
            var have = new HashSet<uint>((glyphs ?? []).Select(glyph => glyph.index));
            var table = glyphs ?? [];
            foreach (var character in characters)
            {
                var index = character.glyphIndex;
                if (!have.Add(index))
                    continue;

                var rect = new UnityEngine.TextCore.GlyphRect((int)(index % columns) * cell,
                    (rows - 1 - (int)(index / columns)) * cell, cell, cell);
                table.Add(new TMPro.TMP_SpriteGlyph(index, new UnityEngine.TextCore.GlyphMetrics(cell, cell, 0, 80, cell), rect, 1,
                    0));
            }

            if (glyphs == null)
                AccessTools.Field(typeof(TMPro.TMP_SpriteAsset), "m_GlyphTable").SetValue(asset, table);
            Logger.LogInfo($"[UI] Sprite asset {asset.name}: rebuilt {table.Count} glyphs from the {columns}x{rows} sheet");
        }

        asset.UpdateLookupTables();
        Logger.LogInfo($"[UI] Sprite asset {asset.name}: 'leftStick' → {asset.GetSpriteIndexFromName("leftStick")}, " +
                       $"'rightTrigger' → {asset.GetSpriteIndexFromName("rightTrigger")}");
    }

    public static void EnsureVRSprites(TMPro.TMP_Text? text)
    {
        var sprites = Assets.AssetCollection.TMPInputsSpriteAsset;
        if (!text || !sprites)
            return;

        if (!text!.spriteAsset)
        {
            text.spriteAsset = sprites;
            return;
        }

        if (text.spriteAsset == sprites)
            return;

        text.spriteAsset.fallbackSpriteAssets ??= [];
        if (!text.spriteAsset.fallbackSpriteAssets.Contains(sprites))
            text.spriteAsset.fallbackSpriteAssets.Add(sprites);
    }

    public static bool GetControlHand(string controlPath, out HapticManager.Hand hand)
    {
        hand = HapticManager.Hand.Both;

        if (string.IsNullOrEmpty(controlPath))
            return false;

        var path = Regex.Replace(controlPath.ToLowerInvariant(), @"<[^>]+>([^ ]+)", "$1");
        var handText = path.Split('/')[0].TrimStart('{').TrimEnd('}');

        hand = handText == "lefthand" ? HapticManager.Hand.Left : HapticManager.Hand.Right;

        return true;
    }

    public static T? ExecuteWithSteamAPI<T>(Func<T> func)
    {
        try
        {
            var isValid = SteamClient.IsValid;

            if (!isValid)
                SteamClient.Init(3241660, false);

            var result = func();

            if (!isValid)
                SteamClient.Shutdown();

            return result;
        }
        catch
        {
            return default;
        }
    }

    public static bool Collide(Collider lhs, params Collider[] rhs)
    {
        return rhs.Any(collider => Physics.ComputePenetration(lhs, lhs.transform.position, lhs.transform.rotation,
            collider,
            collider.transform.position, collider.transform.rotation, out _, out _));
    }

    public static void DisableScanlines(this SemiUI ui)
    {
        if (ui.GetComponentInChildren<UIScanlines>() is not { } scanlines)
            return;

        scanlines.enabled = false;
        scanlines.image.color = Color.clear;
    }

    public static void SetUIAnchoredPosition(this SemiUI ui, Vector2 anchoredPosition)
    {
        var hidePosition = ui.hidePosition - ui.showPosition;
        var rect = ui.GetComponent<RectTransform>();

        rect.anchoredPosition = anchoredPosition;
        ui.showPosition = Vector2.zero;
        ui.hidePosition = hidePosition;
        ui.Start();
    }

    public static void ReplaceOrInsert<T>(this List<T> list, T item, Predicate<T> match)
    {
        var index = list.FindIndex(match);
        if (index >= 0)
            list[index] = item;
        else
            list.Add(item);
    }

    public static Color GetTextColor(Color baseColor, float minBrightness = 0.6f, float alpha = 1f)
    {
        var brightness = 0.2126f * baseColor.r + 0.7152f * baseColor.g + 0.0722f * baseColor.b;

        if (brightness < minBrightness)
        {
            var blendAmount = Mathf.Clamp01((minBrightness - brightness) / minBrightness);
            baseColor = Color.Lerp(baseColor, Color.white, blendAmount);
        }

        baseColor.a = alpha;
        return baseColor;
    }

    public static string StripSuffix(this string @string, string suffix)
    {
        return @string.EndsWith(suffix) ? @string[..^suffix.Length] : @string;
    }

    /// <summary>
    /// Resolve a translation key to it's localized value
    /// </summary>
    public static LocalizedAsset? L(string name)
    {
        if (!AssetCollection.TranslationsLoaded)
            return null;

        var handle = AssetCollection.GetLocalizedAsset(name);
        handle.WaitForCompletion();
        return handle.Status == UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationStatus.Succeeded
            ? handle.Result
            : null;
    }

    public class WaitUntilTimeout(Func<bool> predicate, float timeout) : CustomYieldInstruction
    {
        private readonly float timeStarted = Time.realtimeSinceStartup;

        public override bool keepWaiting => !predicate() && Time.realtimeSinceStartup - timeStarted < timeout;
    }
}

public static class PlayerLocalCameraExtensions
{
    public static MethodInfo GetHandOverrideTransformMethod =>
        AccessTools.Method(typeof(PlayerLocalCameraExtensions), nameof(GetHandOverrideTransform));

    public static Transform GetHandOverrideTransform(this PlayerLocalCamera camera)
    {
        // If overriden, always return the override transform
        if (camera.GetOverrideActive())
            return camera.playerAvatar.PlayerVisionTarget.VisionTransform;

        // If we are in VR and the camera is local, return our VR hand transform
        if (VRSession.Instance is { } instance && camera.playerAvatar.isLocal)
            return instance.Player.MainHand;

        // If the player is a VR player, return their VR hand transform
        if (NetworkSystem.instance && NetworkSystem.instance.GetNetworkPlayer(camera.playerAvatar, out var player))
            return player.PrimaryHand;

        // Fallback, return the camera transform
        return camera.transform;
    }
}

public static class HashExtensions
{
    private const ulong FNV_OFFSET_BASIS = 0xA953453332BAB083;
    private const ulong FNV_PRIME = 0x1B2534321;
    
    private static Dictionary<Type, ulong> typeHashCache = [];
    private static Dictionary<MethodBase, ulong> methodHashCache = [];
    
    public static ulong GetNetworkHash(this Type type)
    {
        if (typeHashCache.TryGetValue(type, out var hash))
            return hash;
        
        hash = ComputeHash(Encoding.UTF8.GetBytes(type.GetFullNameWithGenericArguments()));
        typeHashCache[type] = hash;
        
        return hash;
    }

    public static ulong GetNetworkHash(this MethodBase method)
    {
        if (methodHashCache.TryGetValue(method, out var hash))
            return hash;
        
        var typeName = method.DeclaringType.GetFullNameWithGenericArguments();
        var methodName =
            $"{method.Name}<{string.Join(", ", method.GetGenericArguments().Select(a => a.DeclaringType.GetFullNameWithGenericArguments()))}>({string.Join(",", method.GetParameters().Select(p => p.ParameterType.GetFullNameWithGenericArguments()))})";
        var fullName = $"{typeName}::{methodName}";
        
        hash = ComputeHash(Encoding.UTF8.GetBytes(fullName));
        methodHashCache[method] = hash;

        return hash;
    }

    private static ulong ComputeHash(byte[] input)
    {
        var hash = FNV_OFFSET_BASIS;
        
        foreach (var b in input)
        {
            hash ^= b;
            hash *= FNV_PRIME;
        }

        return hash;
    }
}