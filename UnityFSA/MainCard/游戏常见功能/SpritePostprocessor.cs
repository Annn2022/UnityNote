

using UnityEditor;
using UnityEngine;

namespace Editor
{
    public class SpritePostprocessor : AssetPostprocessor
    {
        private static int MAXSIZE = 1024;
        
        private void OnPreprocessTexture()
        {
            // 获取当前资产路径
            string assetPath = assetImporter.assetPath;

            // 检查文件后缀，只对png图片进行处理
            if (!assetPath.EndsWith(".png"))
            {
                return;
            }

            TextureImporter textureImporter = (TextureImporter)assetImporter;
            // 只处理Sprite类型的纹理
            if (textureImporter.textureType != TextureImporterType.Sprite)
            {
                return;
            }

            // 读取图片的宽和高
            int imageWidth;
            int imageHeight;
            textureImporter.GetSourceTextureWidthAndHeight(out imageWidth, out imageHeight);
       

            // 计算最接近的较小的2的幂次方
            int maxSize = Mathf.Max(CalculatePowerOfTwo(imageWidth), CalculatePowerOfTwo(imageHeight));

            // 确保maxSize不超过256
            maxSize = Mathf.Min(maxSize, MAXSIZE);

            // 设置纹理的导入设置
            textureImporter.maxTextureSize = maxSize;
            textureImporter.textureCompression = TextureImporterCompression.Compressed;
            textureImporter.crunchedCompression = true;
            textureImporter.compressionQuality = 50;
            textureImporter.mipmapEnabled = false;
            
            // 设置平台特定的导入设置
            TextureImporterPlatformSettings platformSettings = new TextureImporterPlatformSettings
            {
                overridden = true,
                name = "Android",
                maxTextureSize = maxSize,
                format = TextureImporterFormat.ASTC_6x6,
            };
            textureImporter.SetPlatformTextureSettings(platformSettings);

            platformSettings.name = "iPhone";
            textureImporter.SetPlatformTextureSettings(platformSettings);
            platformSettings.name = "WebGL";
            textureImporter.SetPlatformTextureSettings(platformSettings);
            TextureImporterSettings textureImporterSettings = new TextureImporterSettings();
            textureImporter.ReadTextureSettings(textureImporterSettings);
            textureImporterSettings.spriteGenerateFallbackPhysicsShape = false;
            textureImporter.SetTextureSettings(textureImporterSettings);
            
        }
        
        private void OnPostprocessTexture(Texture2D texture)
        {
            // 获取当前资产路径
            string assetPath = assetImporter.assetPath;

            // 检查文件后缀，只对png图片进行处理
            if (!assetPath.EndsWith(".png"))
            {
                return;
            }

            TextureImporter textureImporter = (TextureImporter)assetImporter;

            // 只处理Sprite类型的纹理
            if (textureImporter.textureType != TextureImporterType.Sprite)
            {
                return;
            }
            
            TextureImporterSettings textureImporterSettings = new TextureImporterSettings();
            textureImporter.ReadTextureSettings(textureImporterSettings);
            textureImporterSettings.spriteGenerateFallbackPhysicsShape = false;
            textureImporter.SetTextureSettings(textureImporterSettings);
            // 重新导入资产以应用更改
            AssetDatabase.ImportAsset(assetPath, ImportAssetOptions.ForceUpdate);
        }

        // 计算最接近的较小的2的幂次方
        private int CalculatePowerOfTwo(int value)
        {
            int power = 1;
            while (power * 2 <= value)
            {
                power *= 2;
            }
            return power;
        }
    }
}
