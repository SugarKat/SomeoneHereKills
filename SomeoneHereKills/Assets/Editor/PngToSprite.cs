using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

public static class PngToSpriteFolderTool
{
    // Folder *names* to search for inside Assets (not full paths).
    // Example: if you have Assets/Art/UI and the last folder is "UI", then "UI" matches.
    private static readonly string[] TargetFolderNames =
    {
        "Base",
        "Cheek",
        "Eyes",
        "Horns"
    };

    // Optional: change defaults if you want.
    private const int DefaultPixelsPerUnit = 100;
    private const bool SetMultipleSpriteMode = false; // true => SpriteMode.Multiple, false => Single

    [MenuItem("Tools/Sprites/Convert PNGs to Sprites in 4 Folders")]
    public static void ConvertPngsToSprites()
    {
        try
        {
            var targetFolders = FindAssetFoldersByName(TargetFolderNames);
            if (targetFolders.Length == 0)
            {
                Debug.LogWarning($"No matching folders found for: {string.Join(", ", TargetFolderNames)}");
                return;
            }

            int changedCount = 0;
            int totalPngCount = 0;

            AssetDatabase.StartAssetEditing();

            foreach (var folderPath in targetFolders)
            {
                // Find all png assets under this folder (Unity GUID search).
                string[] guids = AssetDatabase.FindAssets("t:Texture2D", new[] { folderPath });

                foreach (string guid in guids)
                {
                    string assetPath = AssetDatabase.GUIDToAssetPath(guid);

                    // Filter to .png only (Texture2D could be other formats).
                    if (!assetPath.EndsWith(".png", StringComparison.OrdinalIgnoreCase))
                        continue;

                    totalPngCount++;

                    var importer = AssetImporter.GetAtPath(assetPath) as TextureImporter;
                    if (importer == null) continue;

                    bool needsReimport = false;

                    // Ensure it's a sprite.
                    if (importer.textureType != TextureImporterType.Sprite)
                    {
                        importer.textureType = TextureImporterType.Sprite;
                        needsReimport = true;
                    }

                    // Single vs Multiple (Multiple is useful for spritesheets; doesn’t auto-slice).
                    var desiredMode = SetMultipleSpriteMode ? SpriteImportMode.Multiple : SpriteImportMode.Single;
                    if (importer.spriteImportMode != desiredMode)
                    {
                        importer.spriteImportMode = desiredMode;
                        needsReimport = true;
                    }

                    // Common good defaults for 2D sprites.
                    if (importer.spritePixelsPerUnit != DefaultPixelsPerUnit)
                    {
                        importer.spritePixelsPerUnit = DefaultPixelsPerUnit;
                        needsReimport = true;
                    }

                    if (!importer.mipmapEnabled)
                    {
                        // already false? great. If true, disable for sprites.
                    }
                    else
                    {
                        importer.mipmapEnabled = false;
                        needsReimport = true;
                    }

                    if (importer.alphaIsTransparency == false)
                    {
                        importer.alphaIsTransparency = true;
                        needsReimport = true;
                    }

                    // Make sure it’s readable only if you need runtime access to pixels (usually false).
                    // importer.isReadable = false;

                    if (needsReimport)
                    {
                        importer.SaveAndReimport();
                        changedCount++;
                    }
                }
            }

            Debug.Log($"PNG?Sprite done. Folders matched: {targetFolders.Length}. PNGs found: {totalPngCount}. Reimported: {changedCount}.");
        }
        finally
        {
            AssetDatabase.StopAssetEditing();
            AssetDatabase.Refresh();
        }
    }

    private static string[] FindAssetFoldersByName(string[] folderNames)
    {
        // Find all folders under Assets. We match by the last folder name.
        // Returns Unity project relative paths like "Assets/Art/UI".
        var allFolderGuids = AssetDatabase.FindAssets("t:Folder", new[] { "Assets" });
        var allFolders = allFolderGuids
            .Select(AssetDatabase.GUIDToAssetPath)
            .Where(p => !string.IsNullOrEmpty(p))
            .ToArray();

        bool NameMatches(string fullPath)
        {
            string last = Path.GetFileName(fullPath.TrimEnd('/', '\\'));
            return folderNames.Any(n => string.Equals(n, last, StringComparison.OrdinalIgnoreCase));
        }

        // Distinct in case of duplicates; order for stable logs.
        return allFolders
            .Where(NameMatches)
            .Distinct()
            .OrderBy(p => p)
            .ToArray();
    }
}
