using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.Build.Pipeline.Utilities;
using UnityEditor.Build.Reporting;
using UnityEditorInternal;
using UnityEngine;

namespace Game
{
    /// <summary>
    /// Build project with menu item and provide method for CI auto build.
    /// </summary>
    public class ProjectBuilder
    {

        public enum ServerConfig
        {

            Dev_Server,
            Production_Server

        }

        /// <summary>
        /// 額外處理需要輸出的場景
        /// </summary>
        [MenuItem("ARK_Tools/Build/ExtraStory")]
        public static void BuildStory()
        {
            BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();
            buildPlayerOptions.scenes           = new[] { "Assets/Game/Scenes/StoryTest.unity", "Assets/Game/Scenes/UIscenes/StoryUI.unity" };
            buildPlayerOptions.locationPathName = "Build/Game.exe";
            buildPlayerOptions.target           = BuildTarget.StandaloneWindows64;
            buildPlayerOptions.options          = BuildOptions.None;

            BuildPipeline.BuildPlayer(buildPlayerOptions);
        }

        [MenuItem("ARK_Tools/Build/ExtraBattle")]
        public static void BuildBattle()
        {
            BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();
            buildPlayerOptions.scenes           = new[] { "Assets/Game/Scenes/BattleTest.unity", "Assets/Game/Scenes/Game.unity" };
            buildPlayerOptions.locationPathName = "BattleTest/Game.exe";
            buildPlayerOptions.target           = BuildTarget.StandaloneWindows64;
            buildPlayerOptions.options          = BuildOptions.None;

            // GameFileSetup.ChangeFileVersion(BuildSetting.DefineSymbolConfig.Dev);
            BuildPipeline.BuildPlayer(buildPlayerOptions);
        }


        [MenuItem("ARK_Tools/Build/BuildTest")]
        public static void BuildProject()
        {
            BuildTarget  target       = EditorUserBuildSettings.activeBuildTarget;
            BuildSetting buildSetting = new BuildSetting("", "", BuildSetting.DefineSymbolConfig.Release);

            // Handle command line arguments.
            if(InternalEditorUtility.inBatchMode)
            {
                Dictionary<string, string> commandLineArgumentToValue = ParseCommandLineArgument();

                if(commandLineArgumentToValue.ContainsKey("-outputPath"))
                    buildSetting.outputPath = commandLineArgumentToValue["-outputPath"];
                if(commandLineArgumentToValue.ContainsKey("-defineSymbolConfig"))
                    buildSetting.symbolConfig = (BuildSetting.DefineSymbolConfig)Enum.Parse(typeof(BuildSetting.DefineSymbolConfig), commandLineArgumentToValue["-defineSymbolConfig"]);
                if(commandLineArgumentToValue.ContainsKey("-logFile"))
                    buildSetting.logFile = commandLineArgumentToValue["-logFile"];
                if(commandLineArgumentToValue.ContainsKey("-androidBuildType"))
                    buildSetting.androidBuildType = (BuildSetting.AndroidBuildType)Enum.Parse(typeof(BuildSetting.AndroidBuildType), commandLineArgumentToValue["-androidBuildType"]);
                if(commandLineArgumentToValue.ContainsKey("-whiteList"))
                    buildSetting.whiteList = true;
                if(commandLineArgumentToValue.ContainsKey("-local"))
                    buildSetting.local = bool.Parse(commandLineArgumentToValue["-local"]);

                //Todo 之後轉成command build的時候再用
                // if(commandLineArgumentToValue.ContainsKey("-buildOptions"))
                // {
                //     var buildOption = commandLineArgumentToValue["-buildOptions"].Split(',');
                //     foreach(var option in buildOption)
                //     {
                //         if(option.Equals("development"))
                //             buildSetting.buildOptions |= BuildOptions.Development;
                //         if(option.Equals("cleanCache"))
                //             buildSetting.buildOptions |= BuildOptions.CleanBuildCache;
                //     }
                // }


                if(!buildSetting.local)
                {
                    AddressableProfileUtility.SetAddressableProfile(buildSetting.symbolConfig == BuildSetting.DefineSymbolConfig.Dev ? "Dev_Server" : "Production_Server");
                }
                else
                {
                    AddressableProfileUtility.SetAddressableProfile("Local_Server");
                }
            }

            GameFileSetup.ChangeFileVersion(buildSetting.symbolConfig);

            BuildProject(target, buildSetting);
        }

        public static void SetProfileDevServer()
        {
            AddressableProfileUtility.SetAddressableProfile("Dev_Server");
        }

        public static void SetProfileProductionServer()
        {
            AddressableProfileUtility.SetAddressableProfile("Production_Server");
        }

        public static void UpdateAddressable()
        {
            BuildTarget target       = EditorUserBuildSettings.activeBuildTarget;
            var         buildSetting = new BuildSetting("", "", BuildSetting.DefineSymbolConfig.Release);

            if(InternalEditorUtility.inBatchMode)
            {
                var commandLineArgumentToValue = ParseCommandLineArgument();

                if(commandLineArgumentToValue.ContainsKey("-defineSymbolConfig"))
                    buildSetting.symbolConfig = (BuildSetting.DefineSymbolConfig)Enum.Parse(typeof(BuildSetting.DefineSymbolConfig), commandLineArgumentToValue["-defineSymbolConfig"]);

                if(commandLineArgumentToValue.ContainsKey("-local"))
                    buildSetting.local = bool.Parse(commandLineArgumentToValue["-local"]);
            }

            var server = buildSetting.symbolConfig == BuildSetting.DefineSymbolConfig.Dev ? ServerConfig.Dev_Server : ServerConfig.Production_Server;
            Debug.Log($"[UpdateAddressable] use Profile {server}");

            if(!buildSetting.local)
            {
                AddressableProfileUtility.SetAddressableProfile(server.ToString());
            }
            else
            {
                AddressableProfileUtility.SetAddressableProfile("Local_Server");
            }

            // GameFileSetup.ChangeFileVersion(buildSetting.symbolConfig);
            UpdateAddressableFlow();
            AssetDatabase.Refresh();
            AssetDatabase.SaveAssets();
            if(!buildSetting.local)
                AddressableExtraSetup.CatalogFileFlow();
            AssetDatabase.Refresh();
            AssetDatabase.SaveAssets();
        }

        public static void UpdateAddressableUseEditor(BuildSetting.DefineSymbolConfig buildSetting)
        {
            var server = buildSetting == BuildSetting.DefineSymbolConfig.Dev ? ServerConfig.Dev_Server : ServerConfig.Production_Server;
            Debug.Log($"[UpdateAddressable] use Profile {server}");
            AddressableProfileUtility.SetAddressableProfile(server.ToString());

            // GameFileSetup.ChangeFileVersion(buildSetting.symbolConfig);
            UpdateAddressableFlow();
            AssetDatabase.Refresh();
            AssetDatabase.SaveAssets();
            AddressableExtraSetup.CatalogFileFlow();
            AssetDatabase.Refresh();
            AssetDatabase.SaveAssets();
        }

        private static void UpdateAddressableFlow()
        {
            AssetDatabase.Refresh();
            Debug.Log("Clean Addressable Library");

            //Clean Addressable Library
            AddressableAssetSettings.CleanPlayerContent(null);
            BuildCache.PurgeCache(false);

            //
            AssetDatabase.Refresh();
            AssetDatabase.SaveAssets();
            Debug.Log("Build Addressable Library");

            //Default Build
            // AddressableAssetSettingsDefaultObject.Settings.DataBuilders.FirstOrDefault();
            // AddressableAssetSettingsDefaultObject.Settings.ActivePlayerDataBuilderIndex = 1;
            AddressableAssetSettings.BuildPlayerContent();
            Debug.Log("Finished Addressable Library");
        }

        public static void MoveExceptFolder()
        {
            ExceptFolderBuild.MoveToTemFolder("StompyRobot");
            ExceptFolderBuild.MoveToTemFolder("AirTest");
        }

        public static void MoveBackExceptFolder()
        {
            ExceptFolderBuild.MoveBackFromTempFolder("StompyRobot");
            ExceptFolderBuild.MoveBackFromTempFolder("AirTest");
        }

        static void BuildProject(BuildTarget buildTarget, BuildSetting buildSetting)
        {
            BuildReport buildReport;
            string      defineSymbolBeforeBuild = PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildPipeline.GetBuildTargetGroup(buildTarget));

            //原本是為了srdebug可以開啟
            //buildSetting.buildOptions;
            var optionsList = buildSetting.symbolConfig == BuildSetting.DefineSymbolConfig.Production
                ? BuildOptions.None        | BuildOptions.CleanBuildCache
                : BuildOptions.Development | BuildOptions.CleanBuildCache;

            EnvironmentSetting(buildTarget, buildSetting);

            if(!buildSetting.local)
                AddressableExtraSetup.CatalogFileFlow();

            BuildPlayerOptions buildPlayerOption = new BuildPlayerOptions
            {
                scenes           = EditorBuildSettings.scenes.Where((s) => s.enabled).Select((s) => s.path).ToArray(),
                locationPathName = GetBuildPath(buildTarget, buildSetting.symbolConfig, buildSetting.outputPath),
                target           = buildTarget,
                options          = optionsList,
            };

            var defines = BuildSetting.symbolConfigToDefineSymbol[buildSetting.symbolConfig];
            if(buildSetting.whiteList)
            {
                defines += ";WhiteList";
            }
            PlayerSettings.SetScriptingDefineSymbolsForGroup(
                BuildPipeline.GetBuildTargetGroup(buildTarget),
                defines
            );

            AssetDatabase.Refresh();


            // if(buildSetting.symbolConfig == BuildSetting.DefineSymbolConfig.Production)
            // {
            //     ExceptFolderBuild.MoveToTemFolder("StompyRobot");
            //     ExceptFolderBuild.MoveToTemFolder("AirTest");
            // }

            buildReport = BuildPipeline.BuildPlayer(buildPlayerOption);

            if(buildSetting.logFile != "")
            {
                // ReportGenerator.CreateReport(buildSetting.logFile);
            }

            var backUpThisFolderPath
                = $"{buildSetting.outputPath}_BackUpThisFolder_ButDontShipItWithYourGame";

            Debug.Log($"[BackUpThisFolderPath] {backUpThisFolderPath}");
            if(!Directory.Exists(backUpThisFolderPath))
                return;

            Directory.Delete(backUpThisFolderPath, true);


            AssetDatabase.Refresh();
            AssetDatabase.SaveAssets();

            PlayerSettings.SetScriptingDefineSymbolsForGroup(
                BuildPipeline.GetBuildTargetGroup(buildTarget), defineSymbolBeforeBuild);


            if(buildReport.summary.result == BuildResult.Succeeded)
            {
                Debug.Log("[ProjectBuilder] Build Success: Time:" + buildReport.summary.totalTime + " Size:" + buildReport.summary.totalSize + " bytes");
                if(InternalEditorUtility.inBatchMode)
                    EditorApplication.Exit(0);
            }
            else
            {
                if(InternalEditorUtility.inBatchMode)
                    EditorApplication.Exit(1);
                throw new Exception("[ProjectBuilder] Build Failed: Time:" + buildReport.summary.totalTime + " Total Errors:" + buildReport.summary.totalErrors);
            }

            // if(buildSetting.symbolConfig == BuildSetting.DefineSymbolConfig.Production)
            // {
            //     ExceptFolderBuild.MoveBackFromTempFolder("StompyRobot");
            //     ExceptFolderBuild.MoveBackFromTempFolder("AirTest");
            // }
        }

        private static void EnvironmentSetting(BuildTarget buildTarget, BuildSetting buildSetting)
        {
            switch(buildTarget)
            {
                case BuildTarget.iOS:
                    Setting(buildSetting);
                    break;

                case BuildTarget.Android:
                    Setting(buildSetting);
                    break;
            }
        }

        static string GetBuildPath(BuildTarget buildTarget, BuildSetting.DefineSymbolConfig defineSymbolConfig, string outputPath = "")
        {
            string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
            string timeStamp   = DateTime.Now.ToString("yyyyMMdd_HHmm");
            string fileName    = $"{PlayerSettings.productName}_{defineSymbolConfig}_{timeStamp}{GetFileExtension(buildTarget)}";
            string buildPath;
            string version = PlayerSettings.bundleVersion;


            outputPath = (outputPath == "") ? desktopPath : outputPath;

            // buildPath = Path.Combine(outputPath, PlayerSettings.productName, $"{buildTarget}_{timeStamp}", fileName);

            buildPath = outputPath + GetFileExtension(buildTarget);

            buildPath = buildPath.Replace(@"\", @"\\");

            return buildPath;
        }

        private static void Setting(BuildSetting buildSetting)
        {
            #if UNITY_EDITOR_OSX
            PlayerSettings.Android.keystoreName = "keystore path";
            PlayerSettings.Android.keyaliasName = "com.yourcompany.yourgame";
            PlayerSettings.Android.keyaliasPass = "password";
            PlayerSettings.Android.keystorePass = "password";
            #endif

            #if UNITY_EDITOR_WIN
            PlayerSettings.Android.keystoreName = "keystore path";
            PlayerSettings.Android.keyaliasName = "com.yourcompany.yourgame";
            PlayerSettings.Android.keyaliasPass = "password";
            PlayerSettings.Android.keystorePass = "password";
            #endif

            EditorUserBuildSettings.androidBuildSystem = AndroidBuildSystem.Gradle;

            if(buildSetting.androidBuildType == BuildSetting.AndroidBuildType.aab)
            {
                PlayerSettings.Android.useAPKExpansionFiles = true;
                EditorUserBuildSettings.buildAppBundle      = true;
            }
            else
            {
                PlayerSettings.Android.useAPKExpansionFiles = false;
                EditorUserBuildSettings.buildAppBundle      = false;
            }

            if(buildSetting.symbolConfig is BuildSetting.DefineSymbolConfig.Release or BuildSetting.DefineSymbolConfig.Production)
                EditorUserBuildSettings.androidCreateSymbols = AndroidCreateSymbols.Public;
            else
                EditorUserBuildSettings.androidCreateSymbols = AndroidCreateSymbols.Disabled;

            if(buildSetting.symbolConfig == BuildSetting.DefineSymbolConfig.Production)
            {
                PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.Android, "com.yourcompany.yourgame");
                PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.iOS,     "com.yourcompany.yourgame");
            }
            else
            {
                PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.Android, "com.yourcompany.yourgame");
                PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.iOS,     "com.yourcompany.yourgame");
            }
        }

        /// <summary>
        /// Parse command line argument and extract custom command line arguments.
        /// </summary>
        /// -outputPath <pathName>: Set output path (directory) for executables.
        /// -defineSymbolConfig <configName>: Set config for define symbol.
        /// <returns>Dictionary about custom command line arguments.</returns>
        private static Dictionary<string, string> ParseCommandLineArgument()
        {
            Dictionary<string, string> commandLineArgToValue = new Dictionary<string, string>();
            string[] customCommandLineArg =
            {
                "-outputPath", "-defineSymbolConfig",
                "-logFile", "-androidBuildType", "-whiteList", "-local"
            };
            string[] commandLineArg = Environment.GetCommandLineArgs();

            for(var i = 0; i < commandLineArg.Length; i++)
            {
                for(var j = 0; j < customCommandLineArg.Length; j++)
                    if(commandLineArg[i] == customCommandLineArg[j])
                        commandLineArgToValue.Add(customCommandLineArg[j], commandLineArg[(i + 1) % commandLineArg.Length]);
            }

            return commandLineArgToValue;
        }

        /// <summary>
        /// Return file extension according to build target.
        /// </summary>
        /// <param name="target">The build target.</param>
        /// <returns>file extension string.</returns>
        static string GetFileExtension(BuildTarget target)
        {
            switch(target)
            {
                case BuildTarget.StandaloneWindows:
                case BuildTarget.StandaloneWindows64:
                    return ".exe";
                case BuildTarget.StandaloneOSX:
                    return ".app";
                case BuildTarget.StandaloneLinux64:
                    return ".x86_64";
                case BuildTarget.WebGL:
                    return ".webgl";
                case BuildTarget.Android:
                    if(EditorUserBuildSettings.buildAppBundle)
                    {
                        return ".aab";
                    }
                    else
                        return ".apk";

                    return EditorUserBuildSettings.buildAppBundle ? ".aab" : ".apk";
                case BuildTarget.iOS:
                    return "";
                default:
                    Debug.LogError("No corresponding extension!");
                    return "";
            }
        }

        public static void SwitchPlatform(BuildTarget targetPlatform)
        {
            EditorUserBuildSettings.SwitchActiveBuildTarget(BuildPipeline.GetBuildTargetGroup(targetPlatform), targetPlatform);
        }

    }

    public class BuildSetting
    {

        public enum DefineSymbolConfig
        {

            Dev        = 0,
            Release    = 1,
            Production = 2

        }

        public enum AndroidBuildType
        {

            apk,
            aab,

        }

        public static readonly Dictionary<DefineSymbolConfig, string> symbolConfigToDefineSymbol = new Dictionary<DefineSymbolConfig, string>
        {
            [DefineSymbolConfig.Dev]        = "ODIN_INSPECTOR_3;dUI_MANAGER;dUI_TextMeshPro;UNITY_POST_PROCESSING_STACK_V2;ODIN_VALIDATOR;Dev;",
            [DefineSymbolConfig.Release]    = "ODIN_INSPECTOR_3;dUI_MANAGER;dUI_TextMeshPro;UNITY_POST_PROCESSING_STACK_V2;ODIN_VALIDATOR;Release;",
            [DefineSymbolConfig.Production] = "DISABLE_SRDEBUGGER,ODIN_INSPECTOR_3;dUI_MANAGER;dUI_TextMeshPro;UNITY_POST_PROCESSING_STACK_V2;ODIN_VALIDATOR;Production;"
        };
        public string             outputPath       = "";
        public DefineSymbolConfig symbolConfig     = DefineSymbolConfig.Dev;
        public string             logFile          = "";
        public AndroidBuildType   androidBuildType = AndroidBuildType.apk;
        public bool               whiteList        = false;
        public bool               local            = false;


        public BuildSetting(string outputPath, string logFile, DefineSymbolConfig symbolConfig)
        {
            this.outputPath   = outputPath;
            this.symbolConfig = symbolConfig;
            this.logFile      = logFile;
        }

        public override string ToString()
        {
            return $"{nameof(BuildSetting)}: symbolConfig={symbolConfig}, outputPath={outputPath}";
        }

    }
}