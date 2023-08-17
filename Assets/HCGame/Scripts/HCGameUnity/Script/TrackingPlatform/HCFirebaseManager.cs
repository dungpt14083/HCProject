#if !UNITY_WEBGL
// using Firebase;
// using Firebase.Messaging;
// using GA = Firebase.Analytics;
// using Firebase.Crashlytics;
#endif
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class HCFirebaseManager : SingletonMono<HCFirebaseManager>
{
//     
// #if !UNITY_WEBGL
//     public FirebaseApp firebaseApp;
// #endif
//     public bool ActiveFCM;
//     [HideInInspector]
//     public bool Available = false;
//     // public static LMFirebaseManager Ins;
//     private void Awake()
//     {
//         Init();
//     }
//
//     void Init()
//     {
// #if !UNITY_WEBGL
//
//         Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
//         {
//             var dependencyStatus = task.Result;
//             if (dependencyStatus == Firebase.DependencyStatus.Available)
//             {
//                 // Create and hold a reference to your FirebaseApp,
//                 // where app is a Firebase.FirebaseApp property of your application class.
//                 //var appOptions = AppOptions.LoadFromJsonConfig("{\"project_info\":{\"project_number\":\"160633205721\",\"project_id\":\"lumiworlddev\",\"storage_bucket\":\"lumiworlddev.appspot.com\"},\"client\":[{\"client_info\":{\"mobilesdk_app_id\":\"1:160633205721:android:a8ee016111dad6748b5fc8\",\"android_client_info\":{\"package_name\":\"com.DefaultCompany.LumiUnity\"}},\"oauth_client\":[{\"client_id\":\"160633205721-o3n67dr89m2hl1k4ee6oalp0c0ltuv0g.apps.googleusercontent.com\",\"client_type\":3}],\"api_key\":[{\"current_key\":\"AIzaSyAoBvHVyckeJDA5_jEuZ3AJ09w7j08w4mI\"}],\"services\":{\"appinvite_service\":{\"other_platform_oauth_client\":[{\"client_id\":\"160633205721-o3n67dr89m2hl1k4ee6oalp0c0ltuv0g.apps.googleusercontent.com\",\"client_type\":3}]}}}],\"configuration_version\":\"1\"}");
//                 //firebaseApp = Firebase.FirebaseApp.Create(appOptions);
//                 firebaseApp = FirebaseApp.DefaultInstance;
//                 if (ActiveFCM)
//                 {
//                     Firebase.Messaging.FirebaseMessaging.TokenReceived += OnTokenReceived;
//                     Firebase.Messaging.FirebaseMessaging.MessageReceived += OnMessageReceived;
//
//                 }
//                 //Firebase.FirebaseApp.LogLevel = Firebase.LogLevel.Debug;
//                 Firebase.Crashlytics.Crashlytics.IsCrashlyticsCollectionEnabled = true;
//                 // Set a flag here to indicate whether Firebase is ready to use by your app.
//                 Available = true;
//             }
//             else
//             {
//                 UnityEngine.Debug.LogError(System.String.Format(
//                   "Could not resolve all Firebase dependencies: {0}", dependencyStatus));
//                 // Firebase Unity SDK is not safe to use here.
//             }
//         });
// #endif
//     }
//
//     #region Firebase FCM - Push
// #if !UNITY_WEBGL
//     public Action<string> FCM_OnTokenReceived;
//     public Action<FirebaseMessage> FCM_OnMessageReceived;
//     public void OnTokenReceived(object sender, Firebase.Messaging.TokenReceivedEventArgs token)
//     {
//         UnityEngine.Debug.Log("Received Registration Token: " + token.Token);
//         FCM_OnTokenReceived?.Invoke(token.Token);
//     }
//
//     public void OnMessageReceived(object sender, Firebase.Messaging.MessageReceivedEventArgs e)
//     {
//         UnityEngine.Debug.Log("Received a new message from: " + e.Message.From);
//         FCM_OnMessageReceived?.Invoke(e.Message);
//     }
// #endif
//
//     #endregion
//
//     #region Crashlytics
//
//     public static void CrashlyticsLog(string msg)
//     {
// #if !UNITY_WEBGL
//         Crashlytics.Log($"[Error]: Server response Exception: {msg}");
// #endif
//     }
//
//     public static void CrashlyticsException(Exception ex)
//     {
// #if !UNITY_WEBGL
//         Crashlytics.LogException(ex);
// #endif
//     }
//
//     #endregion
//
//     #region Analytics
//     
//     public void GA_OnLoginByAccount(string userName, bool status)
//     {
// #if !UNITY_WEBGL
//         GA.Parameter[] parameters =
//         {
//             new GA.Parameter(GA.FirebaseAnalytics.UserPropertySignUpMethod, "username_password"),
//             new GA.Parameter("user_name", userName),
//             new GA.Parameter("status", status ? "success" : "fail")
//         };
//         Firebase.Analytics.FirebaseAnalytics.LogEvent(GA.FirebaseAnalytics.EventLogin, parameters);
// #endif
//     }
//     
//     #endregion

}
