# UnityBuildExtraSetting
整合曾遇過 Android &amp; IOS 輸出時所需要的各種設定

自用，所以一堆不通用的code沒刪除，方便之後挑選

## PostBuildStep

AddPListValues.cs
實作google登入時，IOS需要額外設定GoogleService-Info.plist  
但這個檔案會與firebase的GoogleService-Info.plist衝突  
所以需要額外將IOS的GoogleService-Info改名後重新加入  

``` proj.AddFileToBuild(targetGuid, proj.AddFile("Data/Raw/firebase/GoogleService-Info.plist", "GoogleService-Info.plist")); ```

``` proj.AddFileToBuild(targetGuid, proj.AddFile("Data/Raw/googleSignIn/GoogleService-Info_ios.plist","GoogleService-Info_ios.plist")); ```


## PreBuildStep.cs
BuildProcessHandler
處理建置前與建置後動作
這邊是為了有些檔案在編輯器下才需要，不需要跟著版本一起出去


## AddressableExtraSetup.cs


當執行 Addressables.InitializeAsync() 或 Addressables.CheckForCatalogUpdates() 時，Unity 會向伺服器請求 最新的 catalog.hash 文件。  
這個檔案僅包含 一個 Hash 值，避免直接下載完整的 catalog.json，提升效能。  

Unity 會將 catalog.hash 的內容與 addressables_content_state.bin 內的 Hash 值進行比對：  
如果 Hash 相同 → catalog.json 沒有變更，無需下載。  
如果 Hash 不同 → catalog.json 發生變更，需要下載新的 catalog.json，並更新本地快取。  

這時候會出現一個問題，因為後台那邊的檔案更新速度不一致，可能會出現hash檔案已經更新了，但catalog檔案還沒更新  
變成 Hash不同 -> 判斷catalog.json發生變更，需下載catalog.json ，但此時這個檔案還是舊的(被cache)，下載後就，前端就會判斷是新的，之後就不會再更新了，變成這次熱更實際上是失敗。  


為了解決這個問題，看了catalog.json發現m_BuildResultHash這個字段是沒有被用到使用到，  
所以多加了一個處理，使用CatalogFileFlow的function  
輸出的時候m_BuildResultHash會被強制賦予值，這個值與hash檔案內的id相同  
這樣就可以在更新流程內，再判斷一次目前使用的catalog.json內的m_BuildResultHash是否與hash內的id一致，來判斷是否要重新下載。  

適用於addressable 1.19.19，不知道新版m_BuildResultHash會不會被改掉 (或是其實根本沒這問題，只是官方文檔漏看才以為有這問題)  

另外SetCurrentCatalogNameToResources這個function有將目前catalog版本號額外再寫一個txt檔在專案內  


## ProjectBuilder.cs

AddressableProfileUtility.SetAddressableProfile("Dev_Server");  

原本有用 ExceptFolderBuild.MoveToTemFolder 來將不需要Build到正式版的資料搬移，但後來更新unity版本有BUG，就先註解  

symbolConfigToDefineSymbol 定義每個版本要用到的define  


## Jenkinsfile

定時觸發  
選擇建置Node  
輸出各版本雙平台  
自動化測試(AirTest)  
輸出時ChatGPT整合git Commit內容回報slack  

