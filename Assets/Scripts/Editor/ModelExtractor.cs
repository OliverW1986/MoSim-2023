using UnityEditor;
using UnityEngine;
using System.IO;
using System.IO.Compression;

public class ModelExtractor : AssetPostprocessor
{
    // This method is called every time assets are imported, reimported, or deleted
    static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
    {

        // Check if the zip file has been imported
        foreach (string asset in importedAssets)
        {

            if (asset.Contains("Assets/Imports") && asset.Contains(".zip"))
            {
                ExtractZip(asset, asset.Substring(0, asset.LastIndexOf("/")+1));
                AssetDatabase.Refresh(); // Refresh the asset database so Unity sees the new file
                Debug.Log("Model extracted from .zip and imported successfully.");
            }
        }
    }

    // Extract .zip file
    static void ExtractZip(string zipFilePath, string destinationFolder)
    {
        if (File.Exists(zipFilePath))
        {
            // Ensure destination folder exists
            if (!Directory.Exists(destinationFolder))
            {
                Directory.CreateDirectory(destinationFolder);
            }

            // Extract the zip
            ZipFile.ExtractToDirectory(zipFilePath, destinationFolder);
            Debug.Log("Extracted zip file: " + zipFilePath);
        }
        else
        {
            Debug.LogError("Zip file not found: " + zipFilePath);
        }
    }
}
