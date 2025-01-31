using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Game
{
    public class GameFileSetup
    {

        public static void ChangeFileVersion(BuildSetting.DefineSymbolConfig symbolConfig)
        {
            AssetDatabase.Refresh();
            switch(symbolConfig)
            {
                case BuildSetting.DefineSymbolConfig.Dev:
                    CopyFileToDevVersion();
                    break;
                case BuildSetting.DefineSymbolConfig.Release:
                    CopyFileToProductionVersion();
                    break;
                case BuildSetting.DefineSymbolConfig.Production:
                    CopyFileToProductionVersion();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(symbolConfig), symbolConfig, null);
            }

            AssetDatabase.Refresh();
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            ReplaceHotfix();
            AssetDatabase.Refresh();

            void ReplaceHotfix()
            {
                var originPath = $"{Application.dataPath}/Game/Resources/Json/Game/Item.json";
                var targetPath = $"{Application.dataPath}/Game/Resources_moved/Json/Game/ItemNew.json";
                FileUtil.ReplaceFile(originPath, targetPath);

                // Assets /Game /Sprite /ItemPicture /ItemPicture.png 
                // Assets /Game /Sprite /HotUpdateTest /ItemPicture.png

                originPath = $"{Application.dataPath}/Game/Sprite/ItemPicture/ItemPicture.png";
                targetPath = $"{Application.dataPath}/Game/Sprite/HotUpdateTest/ItemPicture.png";
                FileUtil.ReplaceFile(originPath, targetPath);
                originPath = $"{Application.dataPath}/Game/Sprite/ItemPicture/ItemPicture.tpsheet";
                targetPath = $"{Application.dataPath}/Game/Sprite/HotUpdateTest/ItemPicture.tpsheet";
                FileUtil.ReplaceFile(originPath, targetPath);

                //章節頭像
                originPath = $"{Application.dataPath}/Game/Sprite/ChapterIcon/ChapterIcon.png";
                targetPath = $"{Application.dataPath}/Game/Sprite/HotUpdateTest/ChapterIcon.png";
                FileUtil.ReplaceFile(originPath, targetPath);
                originPath = $"{Application.dataPath}/Game/Sprite/ChapterIcon/ChapterIcon.tpsheet";
                targetPath = $"{Application.dataPath}/Game/Sprite/HotUpdateTest/ChapterIcon.tpsheet";
                FileUtil.ReplaceFile(originPath, targetPath);
                
                //ItemKeepsake
                originPath = $"{Application.dataPath}/Game/Sprite/ItemKeepsake/ItemKeepsake.png";
                targetPath = $"{Application.dataPath}/Game/Sprite/HotUpdateTest/ItemKeepsake.png";
                FileUtil.ReplaceFile(originPath, targetPath);
                originPath = $"{Application.dataPath}/Game/Sprite/ItemKeepsake/ItemKeepsake.tpsheet";
                targetPath = $"{Application.dataPath}/Game/Sprite/HotUpdateTest/ItemKeepsake.tpsheet";
                FileUtil.ReplaceFile(originPath, targetPath);
                

                // Assets /Game /Sprite /ItemCharacter /ItemCharacter.png
                //Assets/Game/Sprite/HotUpdateTest/ItemCharacter.png
                // originPath = $"{Application.dataPath}/Game/Sprite/ItemCharacter/ItemCharacter.png";
                // targetPath = $"{Application.dataPath}/Game/Sprite/HotUpdateTest/ItemCharacter.png";
                // FileUtil.ReplaceFile(originPath, targetPath);
                //
                // originPath = $"{Application.dataPath}/Game/Sprite/ItemCharacter/ItemCharacter.tpsheet";
                // targetPath = $"{Application.dataPath}/Game/Sprite/HotUpdateTest/ItemCharacter.tpsheet";
                // FileUtil.ReplaceFile(originPath, targetPath);
            }
        }

        private static void CopyFileToProductionVersion()
        {
            var originPath = FileHelper.GetUpLevelDirectory(Application.dataPath, 2);
            var targetPath = $"{Application.dataPath}/Game/Resources/Json";
            var path       = Path.Combine(originPath, $"GameData/Production");

            FileUtil.ReplaceDirectory($"{path}", $"{targetPath}");
            
            //move  story folder to target
            originPath = $"{Application.dataPath}/Game/Resources/Json/Story";
            targetPath = $"{Application.dataPath}/Game/Resources_moved/Json/Story";
            FileUtil.ReplaceDirectory(originPath, targetPath);
        }

        private static void CopyFileToDevVersion()
        {
            var originPath = FileHelper.GetUpLevelDirectory(Application.dataPath, 2);
            var targetPath = $"{Application.dataPath}/Game/Resources/Json";
            var path       = Path.Combine(originPath, $"GameData/Dev");

            FileUtil.ReplaceDirectory($"{path}", $"{targetPath}");
            //move  story folder to target
            originPath = $"{Application.dataPath}/Game/Resources/Json/Story";
            targetPath = $"{Application.dataPath}/Game/Resources_moved/Json/Story";
            FileUtil.ReplaceDirectory(originPath, targetPath);
            
        }
        
       


    }
}