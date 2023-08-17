using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Assets.Network;
using UnityEngine;
using Cysharp.Threading.Tasks;



class HCNetworkManager : Singleton<HCNetworkManager>
{

    private API api;
    private SocialPlatform.SocialPlatformModule socialPlatformHandler;
    public NetworkConfig Config;
    
    private IDictionary<string, object> currentUser = null;
    public HCNetworkManager()
    {
        api = new API();
        Config = new NetworkConfig();
        socialPlatformHandler = new SocialPlatform.SocialPlatformModule();
    }
    public async Task Login(IDictionary<string, object> user)
    {
        //TODO: current only using 1
        currentUser = user;
        //Login to Get Token
        //var token = await gameSession.Login(currentUser["PlayerName"].ToString(), currentUser["PlayerPass"].ToString());
        var token = "TEMP_TOKEN";

        Debug.Log("get token : " + token);
        api.SetToken(token);
    }

    public void SetToken(string token)
    {
        api.SetToken(token);
    }

    public void SetURL(string url)
    {
        api.SetURL(url);
    }

    #region LOGIN
    public async Task<bool> UpdateDeviceID()
    {
        IDictionary<string, object> requestParams = new Dictionary<string, object>();
        requestParams.Add("deviceId", SystemInfo.deviceUniqueIdentifier);

        BaseResponse<object> response = await postAndParseResponse<object>("/user/verify/device", requestParams);

        if ((int)LStatusCode.SUCCESS == response.status)
        {
            return true;
        }
        return false;
    }

    public async Task<bool> SendEmailConfirmationCode(string code)
    {
        IDictionary<string, object> requestParams = new Dictionary<string, object>();
        requestParams.Add("confirmationCode", code);
        requestParams.Add("deviceId", SystemInfo.deviceUniqueIdentifier);

        BaseResponse<object> response = await postAndParseResponse<object>("/user/verify/email", requestParams);

        if ((int)LStatusCode.SUCCESS == response.status)
        {
            return true;
        }
        return false;
    }

    public async Task<bool> SendEmailAccount(string email)
    {
        IDictionary<string, object> requestParams = new Dictionary<string, object>();
        requestParams.Add("email", email);

        BaseResponse<object> response = await postAndParseResponse<object>("/user/send/email", requestParams);

        if ((int)LStatusCode.SUCCESS == response.status)
        {
            return true;
        }
        return false;
    }

    public async Task<UserDataResponse> RegisterAccount(string userName, string password)
    {
        IDictionary<string, object> requestParams = new Dictionary<string, object>();
        requestParams.Add("userName", userName);
        requestParams.Add("password", password);

        var registerResponse = await postAndParseResponse<UserDataResponse>("/user/v2/register", requestParams);
        return registerResponse.data;
    }

    public async Task<UserDataResponse> Login(string userName, string password)
    {
        IDictionary<string, object> requestParams = new Dictionary<string, object>();
        requestParams.Add("userName", userName);
        requestParams.Add("password", password);
        requestParams.Add("version", Application.version);
        requestParams.Add("deviceId", SystemInfo.deviceUniqueIdentifier);
        var loginResponse = await postAndParseResponse<UserDataResponse>("/user/v2/login", requestParams);
        return loginResponse.data;
    }

    public async Task<UserDataResponse> Login_Facebook()
    {
        SocialPlatform.LoginData loginData = await socialPlatformHandler.LoginFacebook();
        Debug.Log("Login_Facebook: " + loginData.UserID + " === " + loginData.AccessToken);
        IDictionary<string, object> requestParams = new Dictionary<string, object>();
        requestParams.Add("facebookId", loginData.UserID);
        requestParams.Add("accessToken", loginData.AccessToken);

        BaseResponse<UserDataResponse> loginResponse = await postAndParseResponse<UserDataResponse>("/user/auth/facebook", requestParams);
        return loginResponse.data;
    }
    public async UniTask<UserDataResponse> Login_Google()
    {
        SocialPlatform.LoginData loginData = await socialPlatformHandler.LoginGoogle();
        Debug.Log("Login_Google: " + loginData.UserID + " === " + loginData.AccessToken);
        IDictionary<string, object> requestParams = new Dictionary<string, object>();
        requestParams.Add("googleId", loginData.UserID);
        requestParams.Add("accessToken", loginData.AccessToken);

        BaseResponse<UserDataResponse> loginResponse = await postAndParseResponse<UserDataResponse>("/user/auth/google", requestParams);
        return loginResponse.data;
    }

    #endregion
    //===== LAND BLOCK

   
    #region PRIVATE REGION
    private async Task<BaseResponse<T>> getAndParseResponse<T>(string path, IDictionary<string, object> requestParams)
    {
        return await sendRequestAndParseResponse<T>(async () =>
        {
            return await api.GetAsync(path, requestParams);
        });
    }
    private async Task<BaseResponse<T>> postAndParseResponse<T>(string path, IDictionary<string, object> requestParams)
    {
        return await sendRequestAndParseResponse<T>(() =>
        {
            return api.PostAsync(path, requestParams);
        });
    }

    private async Task<BaseResponse<T>> putAndParseResponse<T>(string path, IDictionary<string, object> requestParams)
    {
        return await sendRequestAndParseResponse<T>(() =>
        {
            return api.PutAsync(path, requestParams);
        });
    }

    private async Task<BaseResponse<T>> sendRequestAndParseResponse<T>(Func<Task<string>> requestAction)
    {
        BaseResponse<T> result = null;
        var response = string.Empty;
        try
        {
            response = await requestAction.Invoke();

            Debug.Log("response : " + response);
            result = ParseResponse<T>(response);
        }
        catch (Exception e)
        {
            // if (e is ServerException)
            // {
            //     var sex = e as ServerException;
            //     if (sex.ErrorCode == (int) LStatusCode.ERR_INVALID_TOKEN)
            //     {
            //         // call to get UserToken
            //         try
            //         {
            //             var refreshToken = LGameData.Instance.CachedToken;
            //             if (!string.IsNullOrEmpty(refreshToken))
            //             {
            //                 Debug.LogWarning("call refresh token ");
            //                 var tokenResponse = await GetUserToken(refreshToken);
            //                 api.SetToken(tokenResponse.token);
            //                 Debug.LogWarning("after call refresh token ");
            //
            //                 response = await requestAction.Invoke();
            //                 result = ParseResponse<T>(response);
            //                 return result;
            //             }
            //         }
            //         catch (Exception exception)
            //         {
            //             throw new ServerException((int)LStatusCode.ERR_INVALID_TOKEN);
            //         }
            //     }
            // }

            throw;
        }

        return result;
    }

    private BaseResponse<T> ParseResponse<T>(string response)
    {

        BaseResponse<T> result = JsonConvert.DeserializeObject<BaseResponse<T>>(response);
        if (result.status != (int)LStatusCode.SUCCESS)
        {
            throw new ServerException(result.status, response);
        }
        return result;
    }

    #endregion

    public void OnQuit()
    {
        api.OnQuit();
    }
}

