using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
// using Facebook.Unity;

namespace SocialPlatform
{
    public class FacebookHandler
    {
        public FacebookHandler()
        {
            // if (!FB.IsInitialized)
            // {
            //     // Initialize the Facebook SDK
            //     FB.Init(InitCallback, OnHideUnity);
            // }
            // else
            // {
            //     // Already initialized, signal an app activation App Event
            //     FB.ActivateApp();
            // }
        }

        private void InitCallback()
        {
            // if (FB.IsInitialized)
            // {
            //     // Signal an app activation App Event
            //     FB.ActivateApp();
            //     // Continue with Facebook SDK
            //     // ...
            // }
            // else
            // {
            //     Debug.Log("Failed to Initialize the Facebook SDK");
            // }
        }
        private void OnHideUnity(bool isGameShown)
        {
            if (!isGameShown)
            {
                // Pause the game - we will need to hide
                Time.timeScale = 0;
            }
            else
            {
                // Resume the game - we're getting focus again
                Time.timeScale = 1;
            }
        }

        public async Task<LoginData> LogIn()
        {
            return new LoginData();
            
            // if(false == FB.IsLoggedIn)
            // {
            //     var perms = new List<string>() { "public_profile", "email" };
            //     FB.LogInWithReadPermissions(perms, AuthCallback);
            //
            //     while (false == FB.IsLoggedIn)
            //     {
            //         await Task.Delay(50);
            //     }
            //     
            //     Debug.Log("User ID : ");
            //     return new LoginData
            //     {
            //         
            //         UserID = Facebook.Unity.AccessToken.CurrentAccessToken.UserId,
            //         AccessToken = Facebook.Unity.AccessToken.CurrentAccessToken.TokenString
            //     };
            // }
            // else
            // {
            //     return new LoginData
            //     {
            //         UserID = Facebook.Unity.AccessToken.CurrentAccessToken.UserId,
            //         AccessToken = Facebook.Unity.AccessToken.CurrentAccessToken.TokenString
            //     };
            // }
        }

        // private void AuthCallback(ILoginResult result)
        // {
        //     if (FB.IsLoggedIn)
        //     {
        //         // AccessToken class will have session details
        //         //var aToken = Facebook.Unity.AccessToken.CurrentAccessToken;
        //         // Print current access token's User ID
        //         //txtLog.text += (aToken.UserId) + "\n";
        //         // Print current access token's granted permissions
        //         //foreach (string perm in aToken.Permissions)
        //         //{
        //         //    txtLog.text += (perm) + "\n";
        //         //}
        //         //LoginCallback?.Invoke(Facebook.Unity.AccessToken.CurrentAccessToken.UserId, Facebook.Unity.AccessToken.CurrentAccessToken.TokenString);
        //     }
        //     else
        //     {
        //         Debug.Log("User cancelled login");
        //     }
        // }
    }
}