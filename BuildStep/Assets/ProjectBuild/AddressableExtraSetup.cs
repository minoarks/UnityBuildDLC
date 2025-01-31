using System;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEngine;
using UnityEngine.AddressableAssets.Initialization;
using UnityEngine.AddressableAssets.ResourceLocators;

namespace Game
{
    public class AddressableExtraSetup
    {

        public enum Platform
        {

            iOS,
            Android,

        }

        [MenuItem("Test/Addressable/Force Set Catalog Hash")]
        public static void CatalogFileFlow()
        {
            SetCatalogWithHash(Platform.iOS); 
            SetCurrentCatalogNameToResources(Platform.iOS);
            
            SetCatalogWithHash(Platform.Android);
            SetCurrentCatalogNameToResources(Platform.Android);
        }

        private static void SetCatalogWithHash(Platform bs)
        {
            var (catalogPath, hashPath) = GetCatalogSettingPath(bs);

            // var hashPath = Path.GetDirectoryName(catalogPath) + "/" + Path.GetFileNameWithoutExtension(catalogPath) + ".hash";
            // if(File.Exists(hashPath) == false)
            //     return;
            Debug.Log($"SetCatalogWithHash Use Platform : {bs}");
            Debug.Log($"Catalog Path : {catalogPath}");
            Debug.Log($"Hash Path : {hashPath}");
            if(!File.Exists(catalogPath) || !File.Exists(hashPath))
                return;

            var hash = File.ReadAllText(hashPath);
            Debug.Log($"current hash {hash}");
            var json = File.ReadAllText(catalogPath);
            var d    = JsonUtility.FromJson<ContentCatalogData>(json);
            ModifyBuildResultHash(d, hash);

            // d.m_BuildResultHash = hash;

            var t = JsonUtility.ToJson(d);
            File.WriteAllText(catalogPath, t);

        }

        public static (string json, string hash) GetCatalogSettingPath(Platform platform)
        {
            var settingsPath = GetSettingsJsonPath(platform);

            // 檢查設定路徑是否存在
            if(!File.Exists(settingsPath))
            {
                // 如果設定檔案不存在，就略過
                Debug.LogError($"{platform} Settings file not found.");
                return (null, null);
            }
            var text            = File.ReadAllText(settingsPath);
            var rtdRuntime      = JsonUtility.FromJson<ResourceManagerRuntimeData>(text);
            var assetLocation   = rtdRuntime.CatalogLocations[0].InternalId;
            var catalogFileName = Path.GetFileNameWithoutExtension(assetLocation);
            var remoteBuildPath = GetAddressableRemoteBuildPath();
            remoteBuildPath = remoteBuildPath.Replace("[BuildTarget]", platform.ToString());
            var catalogPath = Path.Combine(Path.GetDirectoryName(Application.dataPath) ?? string.Empty, remoteBuildPath, catalogFileName + ".json");
            var hashPath    = Path.Combine(Path.GetDirectoryName(Application.dataPath) ?? string.Empty, remoteBuildPath, catalogFileName + ".hash");
            
            Debug.Log($"GetCatalogSettingPath Use Platform : {platform} - {catalogPath} - {hashPath}"); 

            return (catalogPath, hashPath);
        }

        public static ContentCatalogData GetJsonData(Platform bs)
        {
            var (catalogPath, hashPath) = GetCatalogSettingPath(bs);

            Debug.Log($"GetJsonData Use Platform : {bs}"); 
            Debug.Log($"Catalog Path : {catalogPath}");
            Debug.Log($"Hash Path : {hashPath}");
            if(!File.Exists(catalogPath) || !File.Exists(hashPath))
                return null;

            var hash = File.ReadAllText(hashPath);
            var json = File.ReadAllText(catalogPath);
            var d    = JsonUtility.FromJson<ContentCatalogData>(json);
            ModifyBuildResultHash(d, hash);

            var t = JsonUtility.ToJson(d);
            File.WriteAllText(catalogPath, t);

            return d;
        }

        private static void ModifyBuildResultHash(ContentCatalogData catalogData, string newHash)
        {
            Type catalogDataType = typeof(ContentCatalogData);

            var buildResultHashField = catalogDataType.GetField("m_BuildResultHash", BindingFlags.NonPublic | BindingFlags.Instance);

            if(buildResultHashField != null)
            {
                Debug.Log($"--- ModifyBuildResultHash {buildResultHashField.GetValue(catalogData)} -> {newHash}");
                buildResultHashField.SetValue(catalogData, newHash);
            }
            else
            {
                Debug.LogError("m_BuildResultHash field not found in ContentCatalogData.");
            }
        }

        public static void SetCurrentCatalogNameToResources(Platform bs)
        {
            //主要作用是將目前的catalog name寫入到Resources資料夾下，讓APP可以知道目前讀取的catalog版本號是什麼
            // var bs              = GetCurrentBuildTargetString(); 
            var settingsPath = GetSettingsJsonPath(bs);
            if(!File.Exists(settingsPath))
            {
                // 如果設定檔案不存在，就略過
                Debug.LogError($"{bs} Settings file not found.");
                return;
            }
            var text            = File.ReadAllText(settingsPath);
            var rtdRuntime      = JsonUtility.FromJson<ResourceManagerRuntimeData>(text);
            var assetLocation   = rtdRuntime.CatalogLocations[0].InternalId;
            var catalogFileName = Path.GetFileNameWithoutExtension(assetLocation);

            // 設定要寫入的文件路徑
            var platform        = bs.ToString().ToLower();
            var resourcesPath   = Path.Combine(Application.dataPath, "Resources");
            var versionInfoPath = Path.Combine(Application.dataPath, "VersionInfo", $"{platform}_catalog_version.txt");
            var filePath        = Path.Combine(resourcesPath,        $"cata/{platform}_catalog_version.txt");

            // 寫入文件
            File.WriteAllText(filePath,        catalogFileName);
            File.WriteAllText(versionInfoPath, catalogFileName);

            // 確保在 Unity 編輯器中刷新資源
            #if UNITY_EDITOR
            AssetDatabase.Refresh();
            #endif
        }


        public static string GetAddressableRemoteBuildPath()
        {
            #if UNITY_EDITOR

            // 獲取 Addressable 資產設置
            var settings = AddressableAssetSettingsDefaultObject.Settings;

            // 獲取當前選擇的 profile
            var profileId       = settings.activeProfileId;
            var profileSettings = settings.profileSettings;

            // 獲取特定的 profile 參數值
            var buildPath = profileSettings.GetValueByName(profileId, "RemoteBuildPath");

            // 替換 [BuildTarget] 標籤
            // buildPath = settings.profileSettings.EvaluateString(profileId, buildPath);

            return buildPath;
            #else
    Debug.LogError("GetAddressableRemoteBuildPath is only available in the Unity Editor.");
    return null;
            #endif
        }

        public static string GetSettingsJsonPath(Platform platform)
        {
            var projectRootPath = Path.GetDirectoryName(Application.dataPath);

            var settingsPath = Path.Combine(projectRootPath, $"Library/com.unity.addressables/aa/{platform}/settings.json");
            return settingsPath;
        }

        private static string GetCurrentBuildTargetString()
        {
            var buildTarget = EditorUserBuildSettings.activeBuildTarget;
            var bs          = buildTarget.ToString();
            return bs;
        }

    }
}