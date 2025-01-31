using System.IO;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace Game
{
    public class BuildProcessHandler : IPreprocessBuildWithReport, IPostprocessBuildWithReport
    {
        public int callbackOrder => 0;

        // 資料的備份目錄（請確保此路徑在 Assets 資料夾之外）
        private static readonly string BackupFolder = "Temp/Backup/StreamingAssets_BattleSys";
        // 資料的目標路徑
        private static readonly string TargetPath = "Assets/StreamingAssets/BattleSys";

        // 建置前處理
        public void OnPreprocessBuild(BuildReport report)
        {
            Debug.Log("建置前處理：備份並刪除 BattleSys 資料夾");

            if (Directory.Exists(TargetPath))
            {
                // 確保備份目錄的父目錄存在
                string backupParentDir = Path.GetDirectoryName(BackupFolder);
                if (!Directory.Exists(backupParentDir))
                    Directory.CreateDirectory(backupParentDir);

                // 如果之前有備份，先刪除
                if (Directory.Exists(BackupFolder))
                    Directory.Delete(BackupFolder, true);

                // 將資料夾移動到備份目錄
                Directory.Move(TargetPath, BackupFolder);

                Debug.Log($"已備份並移除目錄：{TargetPath}");
            }
        }

        // 建置後處理
        public void OnPostprocessBuild(BuildReport report)
        {
            Debug.Log("建置後處理：還原 BattleSys 資料夾");

            if (Directory.Exists(BackupFolder))
            {
                // 如果目標路徑已存在（不應該發生，但以防萬一）
                if (Directory.Exists(TargetPath))
                    Directory.Delete(TargetPath, true);

                // 將備份的資料夾移回原始位置
                Directory.Move(BackupFolder, TargetPath);

                Debug.Log($"已成功還原目錄：{TargetPath}");
            }
        }
    }

}