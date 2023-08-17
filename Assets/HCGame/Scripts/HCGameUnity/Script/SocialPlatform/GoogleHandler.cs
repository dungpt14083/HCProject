using System;
using System.Threading.Tasks;
using UnityEngine;
using Google;
using Cysharp.Threading.Tasks;

namespace SocialPlatform
{
    [Serializable]
    public class GoogleJsonData
    {
        public string googleID;
        public string accessToken;
    }

    public class GoogleHandler
    {
        private string webClientID = "650939381653-h79bcoe2v3ti7e61pl952kav013rr5cj.apps.googleusercontent.com";

        private bool hasLogIn = false;
        private LoginData loginData;
        public GoogleHandler()
        {
            hasLogIn = false;

#if UNITY_WEBGL
            Debug.Log("JSHandler.Instance.googleLoginHandler!!!");
            JSHandler.Instance.googleLoginHandler += (input) =>
            {
                GoogleJsonData data = JsonUtility.FromJson<GoogleJsonData>(input);

                loginData = new LoginData();
                loginData.UserID = data.googleID;
                loginData.AccessToken = data.accessToken;
                hasLogIn = true;
                Debug.Log("GoogleHandler!!! hasLogIn " + hasLogIn);
            };
#endif
        }

        public async UniTask<LoginData> Login()
        {
            hasLogIn = false;

// #if UNITY_WEBGL && !UNITY_EDITOR
//             JSPlugin.LoginGoogleJS(webClientID);
// #else
//             GoogleSignIn.Configuration = new GoogleSignInConfiguration();
//             GoogleSignIn.Configuration.WebClientId = webClientID;
//             GoogleSignIn.Configuration.UseGameSignIn = false;
//             GoogleSignIn.Configuration.RequestIdToken = true;
//             GoogleSignIn.Configuration.RequestAuthCode = true;
//
//             await GoogleSignIn.DefaultInstance.SignIn().ContinueWith(OnAuthenticationFinished);
// #endif
//
//             while(false == hasLogIn)
//             {
//                 await UniTask.Delay(50, false);
//             }
            return loginData;
        }

        // private void OnAuthenticationFinished(Task<GoogleSignInUser> task)
        // {
        //     if(task.IsFaulted)
        //     {
        //         Debug.Log("OnAuthenticationFinished!!! " + task.Exception.InnerException.ToString());
        //         throw task.Exception.InnerException;
        //     }
        //     else
        //     {
        //         Debug.Log("OnAuthenticationFinished!!! " + task.Result.DisplayName + " === " + task.Result.Email + "===" + task.Result.AuthCode + "===" + task.Result.IdToken + "===" + task.Result.UserId);
        //         loginData = new LoginData();
        //         loginData.UserID = task.Result.UserId;
        //         loginData.AccessToken = task.Result.IdToken;
        //         hasLogIn = true;
        //     }
        // }
    }
}