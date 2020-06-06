using Sirenix.Utilities;
using UnityEditor;
using UnityEngine;

public class SpriteProcessor : AssetPostprocessor
{
     
    private void OnPostprocessTexture(Texture2D texture)
    {
        if (assetPath.Contains("Texture"))
        {
            var strArr = assetPath.Split('/');
            TextureImporter import = assetImporter as TextureImporter;
            import.spritePackingTag = strArr[strArr.Length -2];
            import.textureType = TextureImporterType.Sprite;
        }
    }
}
