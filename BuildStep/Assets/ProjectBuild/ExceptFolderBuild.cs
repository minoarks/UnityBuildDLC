using System.IO;
using UnityEditor;
using UnityEngine;

namespace Game
{
    public class ExceptFolderBuild
    {

        public static void MoveToTemFolder(string targetName)
        {
            var tempFolder       = $"ExceptFolder";
            var checkFolderExist = Directory.GetParent(Application.dataPath).Parent + $"/{tempFolder}";
            var tempFolderName   = $"ExceptFolder/{targetName}";
            var targetPath       = Directory.GetParent(Application.dataPath).Parent + $"/{tempFolderName}";
            var assetPath        = Application.dataPath                             + $"/{targetName}";

            Debug.Log($"targetPath : {targetPath}");
            Debug.Log($"assetPath : {assetPath}");

            if(!Directory.Exists(checkFolderExist))
            {
                Debug.Log($"Create Dir : {checkFolderExist}");
                Directory.CreateDirectory(checkFolderExist);
            }

            if(Directory.Exists(assetPath))
            {
                Debug.Log($"ReplaceDirectory : {targetPath}");
                FileUtil.ReplaceDirectory(assetPath, targetPath);
                Debug.Log($"DeleteDirector : {assetPath}");
                FileUtil.DeleteFileOrDirectory(assetPath);
            }
            
            AssetDatabase.Refresh();
        }

        public static void MoveBackFromTempFolder(string targetName)
        {
            var tempFolderName = $"ExceptFolder/{targetName}";
            var targetPath     = Directory.GetParent(Application.dataPath).Parent + $"/{tempFolderName}";
            var assetPath      = Application.dataPath                             + $"/{targetName}";

            Debug.Log($"targetPath : {targetPath}");
            Debug.Log($"assetPath : {assetPath}");
            if(Directory.Exists(targetPath))
            {
                FileUtil.ReplaceDirectory(targetPath, assetPath);
                Debug.Log($"DeleteDirector : {targetPath}");
                FileUtil.DeleteFileOrDirectory(targetPath);
            }
            AssetDatabase.Refresh();
        }

    }
}