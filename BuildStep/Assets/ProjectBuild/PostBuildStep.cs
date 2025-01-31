
namespace Game
{
    using UnityEngine;
    using UnityEditor;
    using UnityEditor.Callbacks;
    #if UNITY_IOS 
using UnityEditor.iOS.Xcode;
    #endif
    using System.IO;

    public class PostBuildStep
    {

        const string k_TrackingDescription = "您的數據將用於為您提供更好的個性化廣告體驗。";

        [PostProcessBuild(0)]
        public static void OnPostProcessBuild(BuildTarget buildTarget, string pathToXcode)
        {
            if(buildTarget == BuildTarget.iOS)
            {
                AddPListValues(pathToXcode);
                AddToEntitlements(buildTarget, pathToXcode);
                ChangeXcodeMinimumOSVersion(buildTarget, pathToXcode);
            }
        }
        
        /// <summary>
        /// 所需要的權限，這邊用到的是推播、Sign in with Apple、In-App Purchase、Background Modes
        /// </summary>
        /// <param name="buildTarget"></param>
        /// <param name="buildPath"></param>
        public static void AddToEntitlements(BuildTarget buildTarget, string buildPath)
        {
            if (buildTarget != BuildTarget.iOS) return;
            #if UNITY_IOS 
            string projPath = PBXProject.GetPBXProjectPath(buildPath);

            PBXProject proj = new PBXProject();
            proj.ReadFromFile(projPath);

            ProjectCapabilityManager manager = new ProjectCapabilityManager(
                projPath,
                "Entitlements.entitlements",
                targetGuid: proj.GetUnityMainTargetGuid()
            );

            manager.AddPushNotifications(false);
            manager.AddSignInWithApple();
            manager.AddInAppPurchase();
            manager.AddBackgroundModes(BackgroundModesOptions.RemoteNotifications);
            // manager.AddAccessWiFiInformation();

            manager.WriteToFile();
            
            
            #endif
        }

        public static void AddPListValues(string pathToXcode)
        {
            #if UNITY_IOS 
            // Retrieve the plist file from the Xcode project directory:
            string        plistPath = pathToXcode + "/Info.plist";
            PlistDocument plistObj = new PlistDocument();


            // Read the values from the plist file:
            plistObj.ReadFromString(File.ReadAllText(plistPath));

            // Set values from the root object:
            PlistElementDict plistRoot = plistObj.root;

            // Set the description key-value in the plist:
            plistRoot.SetString("NSUserTrackingUsageDescription", k_TrackingDescription);

            var atsKey = "NSAppTransportSecurity";

            PlistElementDict atsDict = plistRoot.CreateDict(atsKey);
            
           
            #if Dev
            // 為特定域名設置例外
            PlistElementDict exceptionDomains = atsDict.CreateDict("NSExceptionDomains");
            PlistElementDict domainDict       = exceptionDomains.CreateDict("yourdomain.com");
            domainDict.SetBoolean("NSTemporaryExceptionAllowsInsecureHTTPLoads", true);
            #endif
            atsDict.SetBoolean("NSAllowsArbitraryLoads",             true);
            atsDict.SetBoolean("NSAllowsArbitraryLoadsInWebContent", true);

            // URL Type
            PlistElementArray urlTypes = plistRoot.CreateArray("CFBundleURLTypes");
            PlistElementDict  dict = urlTypes.AddDict();
            dict.SetString("CFBundleURLName", "googleLoginIn");
            PlistElementArray urlSchemes = dict.CreateArray("CFBundleURLSchemes");
            urlSchemes.AddString("com.googleusercontent.apps.0000000000"); //change it to your own google client id
            
            //出口合規
            plistRoot.SetBoolean("ITSAppUsesNonExemptEncryption", false); 

            //Appsflyer
            plistRoot.SetString("NSAdvertisingAttributionReportEndpoint", "https://appsflyer-skadnetwork.com/");
            //AppsFlyer衝突
            plistRoot.SetBoolean("AppsFlyerShouldSwizzle", true);
            
            // Save changes to the plist:
            File.WriteAllText(plistPath, plistObj.WriteToString());

            // GoogleService-info.plist
            var projPath = pathToXcode + "/Unity-iPhone.xcodeproj/project.pbxproj";
            var proj = new PBXProject();
            proj.ReadFromFile(projPath);
            var targetGuid = proj.GetUnityMainTargetGuid();
            proj.AddFileToBuild(targetGuid, proj.AddFile("Data/Raw/firebase/GoogleService-Info.plist", "GoogleService-Info.plist"));
            proj.AddFileToBuild(targetGuid, proj.AddFile("Data/Raw/googleSignIn/GoogleService-Info_ios.plist", "GoogleService-Info_ios.plist"));
            proj.WriteToFile(projPath);
            
            #endif
        }

        public static void ChangeXcodeMinimumOSVersion(BuildTarget buildTarget, string pathToBuiltProject)
        {
#if UNITY_IOS 
            string plistPath = pathToBuiltProject + "/Info.plist";
            var    plist     = new PlistDocument();
            plist.ReadFromString(File.ReadAllText(plistPath));
 
            // Get root
            var rootDict = plist.root;
 
            // Set minimum OS to 13
            var buildKeyMinOS = "MinimumOSVersion";
            rootDict.SetString(buildKeyMinOS, "13.0");
 
            // Write to file
            File.WriteAllText(plistPath, plist.WriteToString());
#endif
        }

    }
}