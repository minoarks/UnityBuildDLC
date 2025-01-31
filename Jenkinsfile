def Android_Stage_Result = false
def IOS_Stage_Result = false
def APK_PATH = [:]

// def IOS_Build_Path_Setting() {
//   map = [:]
//   if(params.Dev){
//     map["Dev"] = "asdaz/dev"
//   }

//   if(params.Release){
//     map["Release"] = "asdaz/Release"
//   }

//   if(params.Production){
//     map["Production"] = "asdaz/Production"
//   }
//    Dev_Path = "${ANDROID_OUTPUT_WORKSPACE}/${PROJECT_NAME}/Dev/${env.APP_VERSION}/${ParentPath}/${PROJECT_NAME}_Dev_${CUR_TIMESTAMP}"
//         Release_Path = "${ANDROID_OUTPUT_WORKSPACE}/${PROJECT_NAME}/Release/${env.APP_VERSION}/${ParentPath}/${PROJECT_NAME}_Release_${CUR_TIMESTAMP}"
//         Production_Path = "${ANDROID_OUTPUT_WORKSPACE}/${PROJECT_NAME}/Production/${env.APP_VERSION}/${ParentPath}/${PROJECT_NAME}_Production_${CUR_TIMESTAMP}"
      
//   return map
// }



pipeline {
  // triggers {
    // cron(env.BRANCH_NAME == 'master_2_0' ? '0 20 * * *' : '')
    // parameterizedCron(env.BRANCH_NAME == "master_2_0" ? "30 22 * * 1-5 % UpdateAddressable=false;Dev=true;Release=true;android=true;apk=true" : "")
  // }
  parameters {

      booleanParam(name: 'UpdateAddressable', defaultValue: false, description: 'Update Addressable')

      booleanParam(name: 'Local', defaultValue: false, description: '是否禁止熱更新')

      booleanParam(name: 'Dev', defaultValue: true, description: 'Dev版本')

      booleanParam(name: 'Release', defaultValue: false, description: 'Release版本')

      booleanParam(name: 'Production', defaultValue: false, description: 'Production版本')


      booleanParam(name: 'ios', defaultValue: false, description: '建置 ios')

      booleanParam(name: 'android', defaultValue: true, description: '建置 android')

      booleanParam(name: 'apk', defaultValue: true, description: '輸出apk檔案')

      booleanParam(name: 'aab', defaultValue: false, description: '輸出aab檔案(送審用)')
      
      booleanParam(name: 'uploadTestFlight', defaultValue: false, description: '(IOS)是否上傳TestFlight')

      booleanParam(name: 'line', defaultValue: true, description: 'line Notifty')


      choice(name: 'build_agent',choices: ['MBP', 'MacAir'], description: '選擇機器')

      choice(name: 'build_version',choices: ['2', '1'], description: '版本')


      booleanParam(defaultValue: false, name: 'TestNewPlayer', description: '測試新遊客帳號')

      booleanParam(defaultValue: false, name: 'TestTourist', description: '測試新遊客帳號')

      booleanParam(defaultValue: false, name: 'TestGoogle', description: '測試已有的Google帳號')

      choice(name: 'testmode', choices: ["none",'all', 'normaltest', 'advancetest'], description: 'Run on specific platform')

      text(name: 'eventIndex', defaultValue: '-1' , description: '活動章節起始章節')

      text(name: 'mainIndex', defaultValue: '-1' , description: '主章節起始章節')
      
      booleanParam(name: 'useDefaultDefineSymbol', defaultValue: false, description: '使用默認symbol')


  }

  agent {
    node{
          label "${build_agent}"
          customWorkspace "${getCustomworkspace()}"
      }
  }

  options {
      skipDefaultCheckout true
      timeout(time: 2, unit: 'HOURS') // Build timeout set to 2 hour
  }
  environment 
  {
        WORK_SPACE = "${WORKSPACE}".replace("\\", "/").replace("@2", "")
        
        // WORK_DIR = "/Volumes/user/project/projectName"
        // UNITY_WINDOWS_PATH = "C:/Program Files/Unity/Hub/Editor/${UNITY_VERSION}/Editor/Unity.exe"
        // UNITY_MAC_PATH = "/Applications/Unity/Hub/Editor/${UNITY_VERSION}/Unity.app/Contents/MacOS/Unity"
        UNITY_PATH = "/Applications/Unity/Hub/Editor/${UNITY_VERSION}/Unity.app/Contents/MacOS/Unity"
        UNITY_PROJECT_DIR = "${WORK_SPACE}/${PROJECT_NAME}" //專案 -> unity專案
        // UNITY_BUILDOPTION_PATH = "${WORK_SPACE}/BuildOptions.txt"
        UNITY_BUILD_METHOD = 'Game.ProjectBuilder.BuildProject'
        UNITY_UPDATE_METHOD = 'Game.ProjectBuilder.UpdateAddressable'
        UNITY_VERSION = "2021.3.43f1"
        PROJECT_NAME = "PROJECT_NAME"
        PACKAGE_NAME = "PACKAGE_NAME"
        LICENSE_PATH = "/Users/user/workspace/Unity_lic.alf"
        UNITY_ACCOUNT_PATH = "/Users/user/workspace/UnityServiceAccount.txt"
        PROFILE_NAME = "User Name"
        CUR_TIMESTAMP = curTimeStamp()
  }


  
      
  stages {

    stage('清除環境') {
      
      steps {
        
        echo "Clean workspace. (${WORK_SPACE})"

        script {

          
          echo "檢查工作空間中的.git目錄 (${WORK_SPACE})"
          // 檢查.git目錄是否存在
          if (fileExists("${WORK_SPACE}/.git")) {
            echo "清除工作空間中的未追蹤檔案。"
            dir(path: "${WORK_SPACE}") {
                sh 'git clean -f -d -x -e "[Ll]ibrary"'
            }
          } else {
            echo "工作空間中沒有.git目錄，跳過清除步驟。"
          }
          echo "版本時間戳 ${CUR_TIMESTAMP}"
        }
       
        // def start = new Date().format('dd/MM/yyyy HH:mm:ss')
      }
    }
    stage('下載資料') {
      steps {
        dir("${WORK_SPACE}"){
          timeout(time: 60, unit: 'MINUTES') {
            checkout scm
          }
        }
      }
    }


    stage('環境設定'){

      steps{
        // script { env.GIT_COMMIT_MSG = sh (script: "git log -1 --pretty=%B ${GIT_COMMIT}", returnStdout: true).trim() }
        // echo "Commit Msg : (${env.GIT_COMMIT_MSG})"
        script { env.APP_VERSION = sh(script: "cat ${UNITY_PROJECT_DIR}/ProjectSettings/ProjectSettings.asset | grep 'bundleVersion' | cut -c 18-", returnStdout: true).trim()}
        echo "Set App Version : (${env.APP_VERSION})"
        script { env.UNITY_USERNAME = sh(script: "cat ${UNITY_ACCOUNT_PATH} | grep 'username' | cut -d '=' -f 2 | tr -d '[:space:]'", returnStdout: true).trim()}
        script { env.UNITY_PASSWORD = sh(script: "cat ${UNITY_ACCOUNT_PATH} | grep 'password' | cut -d '=' -f 2 | tr -d '[:space:]'", returnStdout: true).trim()}

        // script { env.BuildAndroid = Boolean.valueOf(sh(script: "cat ${UNITY_BUILDOPTION_PATH} | grep 'Android'    | cut -d '=' -f 2 | tr -d '[:space:]'", returnStdout: true).trim())}
        // script { env.BuildIOS     = Boolean.valueOf(sh(script: "cat ${UNITY_BUILDOPTION_PATH} | grep 'IOS'        | cut -d '=' -f 2 | tr -d '[:space:]'", returnStdout: true).trim())}
        // script { env.BuildBundle  = Boolean.valueOf(sh(script: "cat ${UNITY_BUILDOPTION_PATH} | grep 'Bundle'     | cut -d '=' -f 2 | tr -d '[:space:]'", returnStdout: true).trim())}
        // script { env.    = Boolean.valueOf(sh(script: "cat ${UNITY_BUILDOPTION_PATH} | grep ''  | cut -d '=' -f 2 | tr -d '[:space:]'", returnStdout: true).trim())}
        // script { env.LineNotify   = Boolean.valueOf(sh(script: "cat ${UNITY_BUILDOPTION_PATH} | grep 'LineNotify' | cut -d '=' -f 2 | tr -d '[:space:]'", returnStdout: true).trim())}
        
        echo "unity username     : ${env.UNITY_USERNAME} "
        echo "unity password     : ${env.UNITY_PASSWORD} "

        echo "unity BuildAndroid : ${params.android} "
        echo "unity BuildIOS     : ${params.ios} "
        echo "unity Line         : ${params.line} "

        // script {
        //   if(params.line){
        //     firstNotifyLINE('RKHdeVi5U0To8aoUt9cbU7RQJ46XgvzV0j3XYkO9yta',"開始建置流程 ${PROJECT_NAME} 版本 ${env.APP_VERSION} .${BUILD_NUMBER}\nAndroid : ${params.android}\nIOS        : ${params.ios}")
        //   }
        // }

        

      }
    }

    stage('更新素材'){
      steps 
      {
         script{

            is_auto_build = isTimeTrigger()
            changeCount = get_changes_count(currentBuild)

            println "ChangeCount : ${changeCount}"

            if (changeCount < 4 && is_auto_build) {
                println "No changes detected, skipping build."
                currentBuild.result = 'ABORTED'
                return
            }


            if(params.UpdateAddressable){

            platformsToBuild = []

            // 添加勾選的平台到 platformsToBuild 清單
            if(params.ios){
                platformsToBuild.add('iOS')
            }
            if(params.android){
                platformsToBuild.add('Android')
            }
            for (int i = 0; i < 3; i++) {
              for(platform in platformsToBuild){
                  BUILD_TARGET = platform.toUpperCase()
                  
                  if(params.Dev){
                      echo "更新素材 -dev- (${BUILD_TARGET}) start"
                      sh "${UNITY_PATH} -projectPath ${UNITY_PROJECT_DIR} -executeMethod ${UNITY_UPDATE_METHOD} -buildTarget ${BUILD_TARGET} -quit -batchmode -nographics -defineSymbolConfig Dev -local ${params.Local}"
                      echo "更新素材 -- (${BUILD_TARGET}) end"
                  }

                  if(params.Release){
                      echo "更新素材 -release- (${BUILD_TARGET}) start"
                      sh "${UNITY_PATH} -projectPath ${UNITY_PROJECT_DIR} -executeMethod ${UNITY_UPDATE_METHOD} -buildTarget ${BUILD_TARGET} -quit -batchmode -nographics -defineSymbolConfig Release -local ${params.Local}"
                      echo "更新素材 -- (${BUILD_TARGET}) end"
                  }

                  if(params.Production){
                      echo "更新素材 -production- (${BUILD_TARGET}) start"
                      sh "${UNITY_PATH} -projectPath ${UNITY_PROJECT_DIR} -executeMethod ${UNITY_UPDATE_METHOD} -buildTarget ${BUILD_TARGET} -quit -batchmode -nographics -defineSymbolConfig Production"
                      echo "更新素材 -- (${BUILD_TARGET}) end"
                  }

                  CATALOG_TARGET = BUILD_TARGET
                  echo "检查 catalog 版本和 hash"
                  
                  catalog_platform = BUILD_TARGET.toLowerCase()
                  catalogVersionPath = "${UNITY_PROJECT_DIR}/Assets/Resources/cata/${catalog_platform}_catalog_version.txt"

                  if (!fileExists(catalogVersionPath)) {
                      error "Catalog version file not found: ${catalogVersionPath}"
                  }

                  catalogContent = readFile(catalogVersionPath).trim()
                  echo " ${BUILD_TARGET} Catalog Version: ${catalogContent}"

                  catalogHashPath = "${UNITY_PROJECT_DIR}/ServerData/${CATALOG_TARGET}/${catalogContent}.hash"
                  
                  if (!fileExists(catalogHashPath)) {
                      error "Catalog hash file not found: ${catalogHashPath}"
                  }

                  catalogHash = readFile(catalogHashPath).trim()
                  echo "Catalog Hash: ${catalogHash}"
              }
            }






            }else{
                echo "不更新addressable"
            }

          }
        
        sleep(2)
      
      }
    }
    

    stage('Build Android') 
    {
      
      environment 
      {
        BUILD_TARGET = 'Android'
        ANDROID_OUTPUT_WORKSPACE = "/Users/user/Google"
       
        // Dev_Path = "${ANDROID_OUTPUT_WORKSPACE}/${PROJECT_NAME}/Dev/${env.APP_VERSION}/${ParentPath}/${PROJECT_NAME}_Dev_${CUR_TIMESTAMP}"
        // Release_Path = "${ANDROID_OUTPUT_WORKSPACE}/${PROJECT_NAME}/Release/${env.APP_VERSION}/${ParentPath}/${PROJECT_NAME}_Release_${CUR_TIMESTAMP}"
        // Production_Path = "${ANDROID_OUTPUT_WORKSPACE}/${PROJECT_NAME}/Production/${env.APP_VERSION}/${ParentPath}/${PROJECT_NAME}_Production_${CUR_TIMESTAMP}"
        
      }

      steps 
      {
      
        catchError(buildResult: 'SUCCESS', stageResult: 'FAILURE') {
          
          echo "Build Debug"
          
          script{
            BUILD_TARGET = 'Android'

            echo "BUILD_TARGET ${BUILD_TARGET}"
            
            AndroidPathMap = [:]
            buildType = []
            if(params.apk){
                buildType.add("apk")
            }
            if(params.aab){
                buildType.add("aab")
            }

            if(params.android && !params.UpdateAddressable){

                echo "建置Android -- start"

                if(params.Dev){
                  AndroidPathMap["Dev"] = "${ANDROID_OUTPUT_WORKSPACE}/${PROJECT_NAME}/${env.BRANCH_NAME}/Dev/${env.APP_VERSION}/${PROJECT_NAME}_Dev_${CUR_TIMESTAMP}"
                }
                if(params.Release){
                    AndroidPathMap["Release"] = "${ANDROID_OUTPUT_WORKSPACE}/${PROJECT_NAME}/${env.BRANCH_NAME}/Release/${env.APP_VERSION}/${PROJECT_NAME}_Release_${CUR_TIMESTAMP}"
                }
                if(params.Production){
                    AndroidPathMap["Production"] = "${ANDROID_OUTPUT_WORKSPACE}/${PROJECT_NAME}/${env.BRANCH_NAME}/Production/${env.APP_VERSION}/${PROJECT_NAME}_Production_${CUR_TIMESTAMP}"
                }
            
                AndroidPathMap.each { entry ->
                    // //-logFile ${LOG_PATH} 
                  
                    echo "Android -- Build ${entry.key}"

                    // if(entry.key == "Production"){
                    //   sh "${UNITY_PATH} -projectPath ${UNITY_PROJECT_DIR} -buildTarget ${BUILD_TARGET} \
                    //     -username ${env.UNITY_USERNAME} -password ${env.UNITY_PASSWORD} -executeMethod Game.ProjectBuilder.MoveExceptFolder \
                    //     -quit -batchmode -nographics"
                    // }
                    buildIndex = 0
                    for (build in buildType) {
                      
                        echo "-- BuildType ${build}"
                        if(build == "apk"){
                          if(params.useDefaultDefineSymbol){
                              sh "${UNITY_PATH} -projectPath ${UNITY_PROJECT_DIR} -buildTarget ${BUILD_TARGET} \
                                -username ${env.UNITY_USERNAME} -password ${env.UNITY_PASSWORD} -executeMethod ${UNITY_BUILD_METHOD} \
                                -quit -batchmode -nographics -outputPath ${entry.value}_${buildIndex} -defineSymbolConfig ${entry.key} -useDefaultDefineSymbol -androidBuildType ${build} -local ${params.Local}"
                                  
                                APK_PATH["${entry.key}"] = "${env.BRANCH_NAME}/${entry.key}/${env.APP_VERSION}/${PROJECT_NAME}_${entry.key}_${CUR_TIMESTAMP}_${buildIndex}"
                          }else{
                              sh "${UNITY_PATH} -projectPath ${UNITY_PROJECT_DIR} -buildTarget ${BUILD_TARGET} \
                              -username ${env.UNITY_USERNAME} -password ${env.UNITY_PASSWORD} -executeMethod ${UNITY_BUILD_METHOD} \
                              -quit -batchmode -nographics -outputPath ${entry.value}_${buildIndex} -defineSymbolConfig ${entry.key} -androidBuildType ${build} -local ${params.Local}"
                          }
                          APK_PATH["${entry.key}"] = "${env.BRANCH_NAME}/${entry.key}/${env.APP_VERSION}/${PROJECT_NAME}_${entry.key}_${CUR_TIMESTAMP}_${buildIndex}"
                            // APK_PATH.add("${env.BRANCH_NAME}/${entry.key}/${env.APP_VERSION}/${PROJECT_NAME}_${entry.key}_${CUR_TIMESTAMP}_${buildIndex}")
                            
                        }else{

                          sh "${UNITY_PATH} -projectPath ${UNITY_PROJECT_DIR} -buildTarget ${BUILD_TARGET} \
                          -username ${env.UNITY_USERNAME} -password ${env.UNITY_PASSWORD} -executeMethod ${UNITY_BUILD_METHOD} \
                          -quit -batchmode -nographics -outputPath ${entry.value}_${buildIndex} -defineSymbolConfig ${entry.key} -androidBuildType ${build}"

                        }

                        buildIndex = buildIndex +1
                    }

                    // if(entry.key == "Production"){
                    //   sh "${UNITY_PATH} -projectPath ${UNITY_PROJECT_DIR} -buildTarget ${BUILD_TARGET} \
                    //     -username ${env.UNITY_USERNAME} -password ${env.UNITY_PASSWORD} -executeMethod Game.ProjectBuilder.MoveBackExceptFolder \
                    //     -quit -batchmode -nographics"
                    // }
                    
                    

                    echo "Android -- Finished ${entry.key}"
                }
              
               

                echo "完成batchmode"
                sleep(3)

              // script {  
              //     def exitCode = Integer.valueOf(sh(script: "find ${SEARCH_REPORT_PATH} -name '*.xml' -type f | wc -l", returnStdout: true).trim())
              //     boolean exists = (exitCode > 0)
              //     if(exists == true){
              //         sh "mv ${BUILD_REPORT_ORIGIN} ${BUILD_REPORT_DEST}"
              //         echo "move report file to ${BUILD_REPORT_DEST}"
              //     }else{
              //         echo "no file exist"
              //     }
              //  }

              // sleep(1)

              echo "建置Android -- end"
              Android_Stage_Result = true
              // echo "Build Release"
              // sh "${UNITY_PATH} -projectPath ${UNITY_PROJECT_DIR} -buildTarget ${BUILD_TARGET} -username ${env.UNITY_USERNAME} -password ${env.UNITY_PASSWORD} -executeMethod ${UNITY_BUILD_METHOD} -logFile -quit -batchmode -nographics -outputPath ${FINAL_PATH} -defineSymbolConfig ${RELEASE_SYMBOL}"
            }
          }
        }

        // script{
        //   if(params.android){
        //     sh "cat ${LOG_PATH}"
        //   }
        // }
       
      }
    }

    stage('Build IOS')
    {
      
     environment 
      {
        LC_ALL = 'en_US.UTF-8'
        LANG    = 'en_US.UTF-8'
        LANGUAGE = 'en_US.UTF-8'
        
        BUILD_TARGET = 'IOS'
        KEYCHAIN_LOCATION= "$HOME/Library/Keychains/login.keychain"
        KEYCHAIN_PASSWARD= "password"

        //build出檔案放在哪裡
        IOS_PROJECT_WORKSPACE = "/Users/user/workspace/${PROJECT_NAME}_Export" 
        // ARCHIVE_PATH = "${IOS_PROJECT_WORKSPACE}/ArchiveExport/${PROJECT_NAME}.xcarchive"
        // IPA_PATH= "${IOS_PROJECT_WORKSPACE}/IPAExport"
      

        //設定檔案，這些放Unity專案外，拿來切換用的
        exportOptionsPlist_appstore_Path = "${WORK_SPACE}/appstore_manual.plist"
        exportOptionsPlist_develop_manual_Path = "${WORK_SPACE}/develop_manual.plist"
        exportOptionsPlist_develop_auto_Path = "${WORK_SPACE}/develop_auto.plist"
        exportOptionsPlist_adhoc_manual_Path = "${WORK_SPACE}/adhoc_manual.plist"
        exportOptionsPlist_adhoc_auto_Path = "${WORK_SPACE}/adhoc_auto.plist"
        UPLOAD_ACCOUNT_PATH = "/Users/user/workspace/TestFlightKey.txt" 
        ENABLE_BITCODE = "No"
        // UPovisioningProfile = ""
        

      }

      steps 
      {


        script { env.UPLOADUSER = sh(script: "cat ${UPLOAD_ACCOUNT_PATH} | grep 'User' | cut -d '=' -f 2 | tr -d '[:space:]'", returnStdout: true).trim()}
        script { env.UPLOADPSW  = sh(script: "cat ${UPLOAD_ACCOUNT_PATH} | grep 'PSW'  | cut -d '=' -f 2 | tr -d '[:space:]'", returnStdout: true).trim()}
        // echo "Build ${SYMBOL_CONFIG} ${BUILD_TARGET} with Unity (${UNITY_PATH})"
        // echo "UNITY_PROJECT_DIR : ${UNITY_PROJECT_DIR}"
        // echo "PROJECT_DIR : ${PROJECT_DIR}"
        // // echo "Output path: ${OUTPUT_PATH}"
        catchError(buildResult: 'SUCCESS', stageResult: 'FAILURE') {
          
          
          script{

            BUILD_TARGET = 'IOS'
            ExportProjectPathMap = [:]
            Archive_Path_Map = [:]
            IPA_Path_Map = [:]  

            echo "BUILD_TARGET ${BUILD_TARGET}"


            if(params.ios && !params.UpdateAddressable){


              if(params.Dev){
                  ExportProjectPathMap["Dev"] = "${IOS_PROJECT_WORKSPACE}/ProjectExport/Dev"
                  Archive_Path_Map["Dev"] = "${IOS_PROJECT_WORKSPACE}/ArchiveExport/Dev/${PROJECT_NAME}.xcarchive"
                  IPA_Path_Map["Dev"] =  "${IOS_PROJECT_WORKSPACE}/IPAExport/Dev"
              }
              if(params.Release){
                  ExportProjectPathMap["Release"] = "${IOS_PROJECT_WORKSPACE}/ProjectExport/Release"
                  Archive_Path_Map["Release"] = "${IOS_PROJECT_WORKSPACE}/ArchiveExport/Release/${PROJECT_NAME}.xcarchive"
                  IPA_Path_Map["Release"] =  "${IOS_PROJECT_WORKSPACE}/IPAExport/Release"
              }
              if(params.Production){
                  ExportProjectPathMap["Production"] = "${IOS_PROJECT_WORKSPACE}/ProjectExport/Production"
                  Archive_Path_Map["Production"] = "${IOS_PROJECT_WORKSPACE}/ArchiveExport/Production/${PROJECT_NAME}.xcarchive"
                  IPA_Path_Map["Production"] =  "${IOS_PROJECT_WORKSPACE}/IPAExport/Production"
              }
              

              

              // script { 
              //   // env.PROFILE_DATA = sh(script: "$(security cms -D -i ${PROFILE}")
              //   env.PROVISIONING_PROFILE_NAME = "RelfeworldPP_develop"
              //   env.UUID = "9582f838-abda-4a56-bd8b-9a1bc580297e"
              //   env.APP_ID_PREFIX = "W2W56UBF2U"
              //   env.CODE_SIGN_IDENTITY = "Apple Development: Tzu-Chin Liu (P4UU652Z7K)"
              // }

              ExportProjectPathMap.each { entry ->

                //build新的project
                  
                  sh "rm -f -r ${entry.value}"
                  
                  sleep(2)
                  sh "${UNITY_PATH} -projectPath ${UNITY_PROJECT_DIR} \
                      -buildTarget ${BUILD_TARGET} \
                      -logFile \
                      -username ${env.UNITY_USERNAME} \
                      -password ${env.UNITY_PASSWORD} \
                      -executeMethod ${UNITY_BUILD_METHOD} \
                      -quit -batchmode \
                      -nographics -outputPath ${entry.value} \
                      -defineSymbolConfig ${entry.key} -local ${params.Local}"


                  // 輸出專案才需要重新安裝
                  echo "Pod Install thirdPart Plugins"
                  

                  dir("${entry.value}") {

                    sh "/usr/local/bin/pod install"

                  }  
                    
                  
                  sleep(3)

                  echo "Unlock keychains"
                  sh "security list-keychains -s ${KEYCHAIN_LOCATION}"
                  sh "security unlock-keychain -p ${KEYCHAIN_PASSWARD} ${KEYCHAIN_LOCATION}"
            

                  if(entry.key == 'Release'){

                    echo "Clean"
                    sh "rm -f -r ${Archive_Path_Map['Release']}"
                    sleep(2)
                    // clean
                    sh "xcodebuild clean -workspace ${entry.value}/Unity-iPhone.xcworkspace -scheme Unity-iPhone"
                    
                    echo "Archieve config - Release"
                    sh "xcodebuild -workspace  ${entry.value}/Unity-iPhone.xcworkspace \
                    -scheme Unity-iPhone \
                    -configuration Debug archive \
                    -archivePath ${Archive_Path_Map['Release']} \
                    -useModernBuildSystem=Yes \
                    -destination 'generic/platform=iOS' \
                    GCC_PREPROCESSOR_DEFINITIONS='\$GCC_PREPROCESSOR_DEFINITIONS IL2CPP_LARGE_EXECUTABLE_ARM_WORKAROUND=1' \
                    ENABLE_BITCODE=NO"

                    echo "Build Ipa"

                    sh "rm -f -r ${IPA_Path_Map['Release']}"
                    sleep(2)

                    echo "Build Ipa - appstore manual" 
                    sh "xcodebuild -exportArchive \
                    -archivePath ${Archive_Path_Map['Release']} \
                    -exportPath ${IPA_Path_Map['Release']} \
                    -exportOptionsPlist ${exportOptionsPlist_appstore_Path}" //改appstore避免下次build再跑錯
                    //exportOptionsPlist_adhoc_manual_Path
                    sh "xcrun altool --validate-app --file '${IPA_Path_Map['Release']}/${PACKAGE_NAME}.ipa' --type ios --username ${env.UPLOADUSER} --password ${env.UPLOADPSW}"

                    

                  }

                  if(entry.key == 'Production'){

                    echo "Clean"
                    sh "rm -f -r ${Archive_Path_Map['Production']}"
                    sleep(2)
                    // clean
                    sh "xcodebuild clean -workspace ${entry.value}/Unity-iPhone.xcworkspace -scheme Unity-iPhone"
                    
                    echo "Archieve config - Production"

                    sh "xcodebuild -workspace  ${entry.value}/Unity-iPhone.xcworkspace \
                    -scheme Unity-iPhone \
                    -configuration Release archive \
                    -archivePath ${Archive_Path_Map['Production']} \
                    -useModernBuildSystem=Yes \
                    -destination 'generic/platform=iOS' \
                    ENABLE_BITCODE=NO"


                    echo "Build Ipa"

                    sh "rm -f -r ${IPA_Path_Map['Production']}"
                    sleep(2)

                    echo "Build Ipa - appstore manual" 
                    sh "xcodebuild -exportArchive \
                    -archivePath ${Archive_Path_Map['Production']} \
                    -exportPath ${IPA_Path_Map['Production']} \
                    -exportOptionsPlist ${exportOptionsPlist_appstore_Path}"
                    //exportOptionsPlist_adhoc_manual_Path
                    sh "xcrun altool --validate-app --file '${IPA_Path_Map['Production']}/${PACKAGE_NAME}.ipa' --type ios --username ${env.UPLOADUSER} --password ${env.UPLOADPSW}"

                    if(params.uploadTestFlight){

                      sh "xcrun altool --upload-app --file '${IPA_Path_Map['Production']}/${PACKAGE_NAME}.ipa' --type ios --username ${env.UPLOADUSER} --password ${env.UPLOADPSW}"

                    }
                  }
                  
                  IOS_Stage_Result = true
              }
              

            }
          }
         

        }
      }

  
    }

    stage('FinalClean') {
      
      steps {
      
        sleep(2)
        // dir(path: "${WORK_SPACE}") {
        //     sh 'git clean -f -d -x -e "[Ll]ibrary"'
        // }
        // sleep(10)
      }
    }

    // stage('Anadroid模擬器測試') {
    //         when { expression { checkStatus("UserPC") == true }}
    //         agent { 
    //           node{
    //               label "UserPC"
    //                customWorkspace "C:/gitProject/"
    //           }
    //         }
    //         options { skipDefaultCheckout() }
    //         steps {

    //           script { dir("D:/GoogleDrive/ProjectName/"){ env.apkfiles = bat (script: "dir D:\\GoogleDrive\\ProjectName\\master\\*.apk /b /s /O:D /A:-D-H-S", returnStdout: true).trim()}}
              
    //           catchError(buildResult: 'SUCCESS', stageResult: 'FAILURE') {
    //             script{


    //               if(params.testmode == "none")
    //                 return

    //                 sleep(200)  

    //                 //整理路徑
    //                 fullPath = [:] 
    //                 apkPaths = env.apkfiles.split('\\n')
    //                 devPath = []
    //                 releasePath = []
    //                 productionPath = []

    //                 for (path in apkPaths)
    //                 {
    //                   if(path.contains("\\Dev\\")){
    //                     devPath.add(path.replace('\\','/').replaceAll("[\\\r\\\n]+", ""))
    //                   }

    //                   if(path.contains("\\Release\\")){
    //                     releasePath.add(path.replace('\\','/').replaceAll("[\\\r\\\n]+", ""))
    //                   }

    //                   if(path.contains("\\Production\\")){
    //                     productionPath.add(path.replace('\\','/').replaceAll("[\\\r\\\n]+", ""))
    //                   }
    //                 }
    //                 fullPath["Dev"] = devPath
    //                 fullPath["Release"] = releasePath
    //                 fullPath["Production"] = productionPath


    //                 APK_PATH.each { entry ->
                      
    //                   install_apkPath = fullPath[entry.key].last()
    //                   dirpath = "C:/gitProject/airtest_projectName/${env.BRANCH_NAME}/${BUILD_NUMBER}/${entry.key}/"
                  
    //                   testParams = ""
    //                   if(params.TestGoogle)
    //                     testParams = testParams + "--login_google True "
                      
    //                   if(params.TestTourist)
    //                     testParams = testParams + "--login_tourist True "

    //                   if(params.NewPlayer)
    //                     testParams = testParams + "--login_newplayer True "

    //                   if(params.TestGoogle & params.TestTourist & params.NewPlayer)
    //                     testParams = "--login_all True "

    //                   dir("${dirpath}"){
    //                     sleep(2)
    //                     bat "adb kill-server"
    //                     sleep(2)
    //                     bat "adb start-server"
    //                     sleep(2)
    //                     bat "adb devices"
    //                     sleep(2)

    //                   }
                      

    //                   try{

    //                       //舊的，因為python3.10不支援一些模組，改成用airtest編輯器啟動
    //                       // bat "python C:/gitProject/airtest_projectName/launcher.py \
    //                       // C:/gitProject/airtest_projectName/projectName.air \
    //                       echo "run airtest"
    //                       bat "C:/Users/abcde/Downloads/AirtestIDE-win-1.2.14/AirtestIDE/AirtestIDE launcher \
    //                       C:/gitProject/airtest_projectName/launcher.py \
    //                       C:/gitProject/airtest_projectName/projectName.air \
    //                       --log ${dirpath} \
    //                       --retry 3 \
    //                       --env Android \
    //                       --testmode ${params.testmode} \
    //                       --apkInstallPath ${install_apkPath}\
    //                       --mainIndex ${params.mainIndex}\
    //                       --eventIndex ${params.eventIndex}\
    //                       --version ${entry.key}\
    //                       ${testParams} \
    //                       --device Android://127.0.0.1:5037/emulator-5554?cap_method=JAVACAP^&^&ori_method=ADBORI"
                          
    //                   } finally {

    //                       sleep(5)

    //                       echo "create report"
    //                       //產生報告
    //                       bat "python -m airtest report C:/gitProject/airtest_projectName/projectName.air \
    //                       --log_root ${dirpath} \
    //                       --lang zh  \
    //                       --export D:/airtestLog/${env.BRANCH_NAME}/${BUILD_NUMBER}/${entry.key}/ \
    //                       --plugin airtest_selenium.report poco.utils.airtest.report"
    //                       env.run_test_report = true
                        
    //                   }

    //                   // }
                 

    //                 } 
                  

    //               echo "完成android模擬器測試"
                  
    //             }
    //           }
    //         }
    // }

    stage('Line Notify'){

      agent {
      node{
            label "${build_agent}"
        }
      }
      options { skipDefaultCheckout() }

      steps{

        catchError(buildResult: 'SUCCESS', stageResult: 'FAILURE') {
          script{
              msg = ""
              
              if(!params.line){
                return
              }
              if(env.run_test_report){
                
                APK_PATH.each { entry ->
                  msg += "測試報告 : http://localhost:8000/${env.BRANCH_NAME}/${BUILD_NUMBER}/${entry.key}/game.log/log.html\n"
                }
              }
              
              release_note = ""
              def apkLinks = [] // 用於存儲 APK 的版本和 URL

              APK_PATH.each { entry ->
                  sleep(90)
                  
                  echo "entry.key : ${entry.key}"
                  echo "entry.value : ${entry.value}"

                  def fullName = entry.value.split('/')[-1]
                  echo "Full name: ${fullName}"

                  // 執行 Python 腳本，傳遞檔案名稱
                  def result = sh(script: "/Users/user/anaconda3/bin/python /Users/user/Google/DriveTask/get_drive_link.py ${fullName}.apk", returnStdout: true).trim()
                  
                  release_note += "\n${entry.key} : <${result}|${env.APP_VERSION} -[${BUILD_NUMBER}]> "
                  apkLinks << [
                            version: env.APP_VERSION,
                            buildNumber: BUILD_NUMBER,
                            key: entry.key,
                            url: result
                        ]
              }
              echo "release_note : ${release_note}"

              if(release_note != "") {

                def commit_Messages = getChangesSinceLastSuccessfulBuildApk();
                def app_update_message = getChangesFromLastAppVersion()

                def commit_header = commit_Messages.header
                def commit_messageLines = commit_Messages.messageLines

                def app_header = app_update_message.header
                def app_messageLines = app_update_message.messageLines

                def commit_message = ""
                def app_version_summary = ""

                if(commit_messageLines != "")
                {
                  commit_message = summarizeGitCommitsWithCurl(commit_messageLines)
                  sleep(3)
                }

                if(app_messageLines != "")
                {
                  app_version_summary = summarizeGitCommitsWithCurl(app_messageLines)
                  sleep(3)
                }


                // commit_divide = "-------- 普通更新 -> ${commit_header} --------"
                // version_divide = "-------- 版本更新 -> ${env.APP_VERSION} ${app_header} --------"
                // apk_divide = "------------------下載------------------"

                // def summary = "${commit_divide}\n${commit_message}\n${version_divide}\n${app_version_summary}\n${apk_divide}\n${release_note}"

                finalNotifySlack(env.APP_VERSION,commit_header,commit_message,app_header,app_version_summary,apkLinks);
              }else
              {
                echo "No release note"
              }

          }
        }
      }

    }

  }
}



def getChangesFromLastAppVersion() {
    def commits = []
    def build = currentBuild
    def vars = build.getBuildVariables()
    // 獲取當前的 APP_VERSION
    def currentVersion = vars['APP_VERSION']

    // 從目前 Build 開始回溯，直到找到上一個符合條件的 Build
    while (build != null) {
        if (isBuildSuccessAnd_android_aab_production(build) && hasAppVersionChanged(build, currentVersion)) {
            // 如果找到符合條件的 Build，停止回溯
            break
        }
        // 累積該 Build 的變更訊息
        def changeSet = build.getChangeSets()
        changeSet.each { cs ->
            cs.items.each { item ->
                def commitTime = new Date(item.timestamp)
                def commitTimeStr = commitTime.format('yyyy-MM-dd HH:mm:ss')
                
                // 在訊息前加入時間戳記
                commits << [time: commitTime, msg: "${commitTimeStr} ${item.msg}"]
            }
        }
        // 回溯到前一個 Build
        build = build.getPreviousBuild()
    }

    // 如果沒有任何 commit，返回空字串
    if (commits.isEmpty()) {
        return [header: "", messageLines: ""]
    }

    // 找出最早與最晚的 commit 時間
    def earliestCommit = commits.min { it.time }
    def latestCommit = commits.max { it.time }

    // 不顯示年份，只顯示月、日
    // 使用 format('M') 和 format('d') 來避免前導零，如 1月5號 而非 01月05號
    def earliestMonth = earliestCommit.time.format('M')
    def earliestDay = earliestCommit.time.format('d')
    def latestMonth = latestCommit.time.format('M')
    def latestDay = latestCommit.time.format('d')

    // 組合範圍資訊
    def header = "${earliestMonth}/${earliestDay} ~ ${latestMonth}/${latestDay}"
    // 組合全部 commit 訊息
    def messageLines = commits.collect { commit -> commit.msg }.join("\n")

    return [header: header, messageLines: messageLines]
}

def getChangesSinceLastSuccessfulBuildApk() { 
    def commits = []
    def build = currentBuild

    // 從目前 Build 開始回溯，直到找到上一個符合條件的 Build
    while (build != null) {
        if (isBuildParamsAndSuccess(build)) {
            // 如果找到符合條件的 Build，停止回溯
            break
        }
        // 累積該 Build 的變更訊息
        def changeSet = build.getChangeSets()
        changeSet.each { cs ->
            cs.items.each { item ->
                def commitTime = new Date(item.timestamp)
                def commitTimeStr = commitTime.format('yyyy-MM-dd HH:mm:ss')
                
                // 在訊息前加入時間戳記
                commits << [time: commitTime, msg: "${commitTimeStr} ${item.msg}"]
            }
        }
        // 回溯到前一個 Build
        build = build.getPreviousBuild()
    }

    // 如果沒有任何 commit，返回空字串
    if (commits.isEmpty()) {
        return [header: "", messageLines: ""]
    }

    // 找出最早與最晚的 commit 時間
    def earliestCommit = commits.min { it.time }
    def latestCommit = commits.max { it.time }

    // 不顯示年份，只顯示月、日
    // 使用 format('M') 和 format('d') 來避免前導零，如 1月5號 而非 01月05號
    def earliestMonth = earliestCommit.time.format('M')
    def earliestDay = earliestCommit.time.format('d')
    def latestMonth = latestCommit.time.format('M')
    def latestDay = latestCommit.time.format('d')

    // 組合範圍資訊
    def header = "${earliestMonth}/${earliestDay} ~ ${latestMonth}/${latestDay}"

    // 組合全部 commit 訊息
    def messageLines = commits.collect { commit -> commit.msg }.join("\n")

    return [header: header, messageLines: messageLines]
}

// 判斷 Build 是否符合參數條件且結果成功
def isBuildParamsAndSuccess(build) {
    echo "Current Build Number: ${build.number}"
    // 檢查建置結果
    if (build.result != 'SUCCESS') {
        echo "Build result is not SUCCESS: ${build.result}"
        return false
    }
    def history = []
    def buildParams = build.rawBuild.getAction(hudson.model.ParametersAction)
    if (buildParams) {
        def paramMap = [:]
        buildParams.each { param ->
          paramMap[param.name] = param.value
        }
        println "Build #${build.number} params: ${paramMap}"
        history.add([build: build.number, params: paramMap])
        // 檢查參數是否存在並轉換為布林值
        boolean androidParam = paramMap['android']?.toString()?.toBoolean() ?: false
        boolean apkParam = paramMap['apk']?.toString()?.toBoolean() ?: false
        // 檢查是否存在 Dev/Release/Production 參數
        boolean hasEnvParam = ['Dev', 'Release', 'Production'].any { env ->
            paramMap[env]?.toString()?.toBoolean() ?: false
        }

        return androidParam && apkParam && hasEnvParam
    } else {
        echo "No parameters found for build #${build.number}"
        return false
    }

    // 從 Build 中取得參數變數 有 APP_VERSION:1.1.X 可以用
    // def vars = build.getBuildVariables()
    // println "Build Variables: ${vars}"
    // boolean androidParam = vars['android']?.toBoolean() ?: false
    // boolean apkParam = vars['apk']?.toBoolean() ?: false

    return false
}

// 判斷 Build 是否符合參數條件且結果成功
def isBuildSuccessAnd_android_aab_production(build) {
    echo "Current Build Number: ${build.number}"
    // 檢查建置結果
    if (build.result != 'SUCCESS') {
        echo "Build result is not SUCCESS: ${build.result}"
        return false
    }

    def buildParams = build.rawBuild.getAction(hudson.model.ParametersAction)
    if (buildParams) {
        def paramMap = [:]
        buildParams.each { param ->
            paramMap[param.name] = param.value
        }

        boolean androidParam = paramMap['android']?.toString()?.toBoolean() ?: false
        boolean aabParam = paramMap['aab']?.toString()?.toBoolean() ?: false
        boolean isProduction = paramMap['Production']?.toString()?.toBoolean() ?: false

        // 條件：Production 且 android 且 aab 必須為 true
        return isProduction && androidParam && aabParam
    } else {
        echo "No parameters found for build #${build.number}"
        return false
    }

    return false
}

def get_changes_count(build) {

    // 檢查變更數量
    changeCount = 0
    def changeSet = build.getChangeSets()
    changeSet.each { cs ->
        cs.items.each { item ->
          echo "Commit 2 Message: ${item.msg}"
          changeCount +=1
        }
    }

    return changeCount
}

def hasAppVersionChanged(build, inputVersion) {
    // 從 build 取得變數
    def vars = build.getBuildVariables()
    echo "Build Variables: ${vars}"
    
    // 獲取當前的 APP_VERSION
    def currentVersion = vars['APP_VERSION']
    println "Input Version: ${inputVersion}, Current Version: ${currentVersion}"
    
    // 比對 APP_VERSION
    if (currentVersion == inputVersion) {
        return false // 版號相同，未改變
    } else {
        return true // 版號不同，已改變
    }
}




def summarizeGitCommitsWithCurl(message) {
    def apiKey = "open ai key" // 確保環境變數已設置
    def payload = [
        model: "gpt-4o-mini",
        messages: [
            [
                role: "system",
                content: "你是一位精通 Git 的管理者，擅長總結零散的 commit 訊息，使用繁體中文語意回應，我會給資料，幫我移除jenkinsfile，版本號，addr，小於6個中文字相關的訊息後將相關內容合併成清晰的分類，並避免重複描述。對於重要的專有名詞或特定內容，出現英文的commit以非技術人員能理解的方式呈現,盡量保持其原始描述，並按以下分組方式進行整理：修正（Bug 修復）、更新（功能或資源更新）、優化（性能或介面改進）、新增（新功能或內容）、其他（不屬於以上分類的變更）。內容不要加入時間戳。"
            ],
            [
                role: "user",
                content: "以下是 Git commit 訊息：\n" + message + 
                    "\n請根據上述指引，移除重複或無意義的資訊。回應的內容不要過多的贅字，除了[其他]分類以外，例如優化，更新，修正，新增等。這些屬於大標題中分類的詞彙。"
            ]
        ]
    ]

    // 將 payload 轉為 JSON 字符串
    def jsonPayload = groovy.json.JsonOutput.toJson(payload)

    // 使用 curl 發送請求
    def response = sh(
        script: """
        curl -X POST "https://api.openai.com/v1/chat/completions" \
            -H "Authorization: Bearer ${apiKey}" \
            -H "Content-Type: application/json" \
            -d '${jsonPayload}'
        """,
        returnStdout: true
    ).trim()
    
    println "Response: ${response}"
    // 解析返回結果
    def jsonResponse = new groovy.json.JsonSlurperClassic().parseText(response)
    return jsonResponse.choices[0].message.content.trim()
}

def finalNotifySlack(app_version,commit_header,commit_msg,app_header,app_msg,apkLinks) {

    // def isFailure = result == 'FAILURE'
    def slackWebhookUrl = 'slack_webhook_url'

    def buttons = []

    apkLinks.each { apk ->
        buttons += [
            [
                "type": "button",
                "text": [
                    "type": "plain_text",
                    "emoji": true,
                    "text": "下載 ${apk.key} ${apk.version}"
                ],
                "url": "${apk.url}",
                // "action_id": "download_${apk.key}_${apk.buildNumber}"
            ]
        ]
    }


    // def blocks = [
    //     [
    //     "type": "section",
    //     "text": [
    //       "type": "mrkdwn",
    //       "text": "*定期更新\n${commit_header}*\n${commit_msg}"
    //       ],
    //       "accessory": [
    //         "type": "image",
    //         "image_url": "https://api.slack.com/img/blocks/bkb_template_images/approvalsNewDevice.png",
    //         "alt_text": "computer thumbnail"
    //         ]
    //     ],
    //     [
    //       "type": "divider"
    //     ],
    //     [
    //     "type": "section",
    //     "text": [
    //       "type": "mrkdwn",
    //       "text": "*${app_version}更新\n$${app_header}*\n${app_msg}"
    //       ],
    //       "accessory": [
    //         "type": "image",
    //         "image_url": "https://api.slack.com/img/blocks/bkb_template_images/approvalsNewDevice.png",
    //         "alt_text": "computer thumbnail"
    //         ]
    //     ],
    //     [
    //       "type": "divider"
    //     ]
    // ]

    
    def blocks = [
        [
          "type": "section",
           "text": [
            "type": "mrkdwn",
            "text": "*定期更新*\n${commit_msg} "
            ]
        ],
        [
          "type": "divider"
        ],
        [
          "type": "section",
           "text": [
              "type": "mrkdwn",
              "text": "*[${app_version}] 累計更新* - ${app_header}\n${app_msg}"
            ]
        ],
        [
          "type": "divider"
        ]
    ]

    // 如果有按鈕，則添加 actions 區塊
    if (buttons.size() > 0) {
        blocks += [
            [
                "type": "actions",
                "elements": buttons
            ]
        ]
    }

    def attachments = [["blocks": blocks]]
    def payload = ["attachments": attachments]
    def payloadJson = groovy.json.JsonOutput.toJson(payload)


    // 訊息中添加結果標籤 (成功或失敗)
    // def statusMessage = isFailure ? "[FAILURE] " : "[SUCCESS] "
    // def finalMessage = "${message}"
    // def payload = groovy.json.JsonOutput.toJson([text: finalMessage])

    sh """
    curl -X POST "${slackWebhookUrl}" \
        -H "Content-type: application/json" \
        --data '${payloadJson}'
    """
}

def firstNotifyLINE(token,message) {

    def url = 'https://notify-api.line.me/api/notify'
    // def imageThumbnail = 'https://i.imgur.com/h8Ypmva.jpeg'
    // def imageFullsize = 'https://i.imgur.com/h8Ypmva.jpeg'
  
    // sh "curl ${url} -H 'Authorization: Bearer ${token}' -F 'message=${message}' -F 'imageThumbnail=${imageThumbnail}' -F 'imageFullsize=${imageFullsize}'"
    sh "curl ${url} -H 'Authorization: Bearer ${token}' -F 'message=${message}'"

}

// 時區轉換
// def curTimeStamp() 
// {
//     return new Date((Calendar.getInstance()).getTimeInMillis() + (480 * 60000)).format('yyyyMMdd_HHmm')
// }
def curTimeStamp() 
{
    return new Date(Calendar.getInstance().getTimeInMillis()).format('yyyyMMdd_HHmm')
}

//_version = sh(script: "echo `date '+%Y%m%d%H%M%S'`", returnStdout: true).trim() 也可以

def isTimeTrigger() {
    boolean startedByTimer = false

    def buildCauses = currentBuild.getBuildCauses()
    // println "Build causes: ${buildCauses}"

    timerCause = currentBuild.rawBuild.getCause(org.jenkinsci.plugins.parameterizedscheduler.ParameterizedTimerTriggerCause)
    if (timerCause) {
        println "Build reason: Build was started by parameterized timer"
        startedByTimer = true
    }
    println "Build started by manual"

    return startedByTimer
}

@NonCPS
def getAllChangeResults() {
	def result = getChangeStringForBuild(currentBuild)

	def buildToCheck = currentBuild.getPreviousBuild()
	while (buildToCheck != null && buildToCheck.result != 'SUCCESS') {
		result += "\nBuild #${buildToCheck.number} [${buildToCheck.result}]\n"
		result += getChangeStringForBuild(buildToCheck)

		buildToCheck = buildToCheck.previousBuild
	}

	return result
}

@NonCPS
def isChangeSetIsNull() {
	
    return currentBuild.changeSets == null;
  
}


@NonCPS
def getCurrentBuildAuthor() {
	
    if(currentBuild.changeSets != null){
      return currentBuild.changeSets.last().items.last().author.getFullName();
    }else{
      return "xxxxxx"
    }
  
}

@NonCPS
def getChangeStringForBuild(build) {
    MAX_MSG_LEN = 60

    echo "Gathering SCM changes"
    def changeLogSets = build.changeSets
	  def changeString = ""

    for (int i = 0; i < changeLogSets.size(); i++) {
        def entries = changeLogSets[i].items
        for (int j = entries.length-1; j >= 0; j--) 
        {
            def entry = entries[j]
            def truncated_msg = entry.msg.take(MAX_MSG_LEN)
			      if (entry.msg.length() > MAX_MSG_LEN) {
				      truncated_msg += "..."
			      }

            changeString += "[${entry.author.getFullName().take(3)}]: ${truncated_msg}\n"
        }
    }

    if (!changeString) {
        changeString = " - No new changes"
    }

    return "${changeString}"
}

def getCustomworkspace() {
  if (params.build_agent == 'MacAir') {
    return "/Volumes/SSD/project/projectName"
  }
  else if (params.build_agent == 'MBP') {
    if(params.build_version == '1'){
        return "/Users/user/Desktop/GitProject/projectName"
    }else{
        return "/Users/user/Desktop/GitProject/projectName_2_0"
    }
  }
}

@NonCPS
def checkStatus(String nodeName) {

    Node node = hudson.model.Hudson.instance.getNode(nodeName)
    
    if (node == null) {
        println("ERROR: Node ${nodeName} doesn't exist")
        return false
    }
    
    Computer computer = node.toComputer()
 
      // TODO skip this; the node is always busy at this point
    if (computer.countBusy()) {
        println("WARNING: Ignore ${nodeName} as it is busy")
        return true
    }

    if (computer.isOffline()) {
        println "Error! Node ${nodeName} is offline."
        return false
     }
    return true
}