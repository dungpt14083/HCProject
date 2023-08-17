using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.ResourceManagement.AsyncOperations;

public class BUtil : MonoBehaviour
{
    #if UNITY_WEBGL

    // [DllImport("__Internal")]
    // private static extern string GetToken();
    //
    // [DllImport("__Internal")]
    // private static extern string GetAPIUrl();
    //
    // [DllImport("__Internal")]
    // private static extern void GoOfferingLand();
    //
    // [DllImport("__Internal")]
    // private static extern void GoMaintenance();
    //
    // [DllImport("__Internal")]
    // private static extern void CopyToClipboard(string text);
    
    #endif

    public static BUtil Ins;
    private void Awake()
    {
        Ins = GetComponent<BUtil>();
    }

    public Coroutine WaitSec(float sec, Action act, bool ignoreTimeScale = false)
    {
        return StartCoroutine(Sec(sec, act, ignoreTimeScale));
    }

    IEnumerator Sec(float sec, Action act, bool ignoreTimescale = false)
    {
        if (ignoreTimescale) yield return new WaitForSecondsRealtime(sec);
        else yield return new WaitForSeconds(sec);
        act?.Invoke();
    }

    public static async UniTask<string> GetJWTToken()
    {
//         try
//         {
//             //TODO: replace JWT token to test on Unity Editor
// #if UNITY_WEBGL && !UNITY_EDITOR
//         var token = GetToken();
// #elif (UNITY_ANDROID|| UNITY_IOS)
//
//             //temp disable deeplink
//             // var token = ProcessDeepLinkMngr.GetJWTFromDeepLink(Application.absoluteURL);
//             // if (string.IsNullOrEmpty(token))
//             // {
//             //     token = BGameData.Instance.CachedToken;
//             // }
//             
//             // var token = "Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJwdWJsaWNBZGRyZXNzIjoiMHhlM2E5YzI1N2VhNjYxM2RiMDQ3ZDRkNWFjNTQ0YmMyYjMzNTMwN2Q2IiwibG9naW5UaW1lIjoxNjQ2ODEyNTMwNzkxLCJpYXQiOjE2NDY4MTI1MzAsImV4cCI6MTY0Njg5ODkzMH0.oklaLlfurdSPVZVkPYkxknGk-2W7BieCsQWytGQdI_0";
//             var token = string.Empty;
//             var refreshToken = BGameData.Instance.CachedToken;
//             if (!string.IsNullOrEmpty(refreshToken))
//             {
//                 token = (await BNetworkManager.Instance.GetUserToken(refreshToken)).token;
//             }
//
// #else
//             var token = "Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJwdWJsaWNBZGRyZXNzIjoiMHhlM2E5YzI1N2VhNjYxM2RiMDQ3ZDRkNWFjNTQ0YmMyYjMzNTMwN2Q2IiwibG9naW5UaW1lIjoxNjUxMDQ1NDU3MzI3LCJpYXQiOjE2NTEwNDU0NTcsImV4cCI6MTY1MTEzMTg1N30.I6Qrh9uwrWHQwtqU-66tVbRFYo_D8baSjT6PttmzPgg";
// #endif
//             return token;
//         }
//         catch (Exception ex)
//         {
//             Debug.LogError("Get Token Error : " + ex.ToString() + "\n=TRACE:==" + ex.StackTrace);
//         }
         return string.Empty;
    }


    public static string GetRefreshToken()
    {
        return string.Empty;
    }
        

    public static string GetUrl()
    {
        try
        {
            var url = "";
   
            
        #if PROD
                    url = "https://abc.com"; 
        #elif STAG
                    url = "https://abc.com"; 
        #else
                    url = "https://abc.com";
        #endif
            
            return url;
        }
        catch (Exception ex)
        {
            Debug.LogError("Get API URL Error : " + ex.ToString() + "\n=TRACE:==" + ex.StackTrace);
        }
        return string.Empty;
    }

    public static string GetLoginDeeplink()
    {
        var deeplink = "";
        try
        {
        #if PROD
                    deeplink = "https://metamask.app.link/dapp/abc.com"; 
        #else
            deeplink = "https://metamask.app.link/dapp/abc.com";  
        #endif
        return deeplink;
        }
        catch (Exception ex)
        {
            Debug.LogError("Get API URL Error : " + ex.ToString() + "\n=TRACE:==" + ex.StackTrace);
        }
        return string.Empty;
    }
    
    public static void SetTextCopyToClipBoard(string text)
    {
#if UNITY_WEBGL && UNITY_EDITOR == false
            // CopyToClipboard(text);
#else
        GUIUtility.systemCopyBuffer = text;
#endif
        
        
    }

    public static bool IsPointerOverUIObject()
    {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        return results.Count > 0;
    }

    public static async UniTask ClearAllChildItem(Transform parent)
    {
        parent.gameObject.SetActive(false);
        var allChilds = new GameObject[parent.childCount];
        for (int i = 0; i < parent.childCount; i++)
        {
            var child = parent.GetChild(i).gameObject;
            allChilds[i] = child;
        }

        for (int j = 0; j < allChilds.Length; j++)
        {
            GameObject.Destroy(allChilds[j]);
            await UniTask.DelayFrame(1);
        }
        
        parent.gameObject.SetActive(true);
    }

    //Check if game object A is child of game object B, recrusive
    public static bool IsChildrenOfObject(Transform transA, Transform transB)
    {
        Transform currentParent = transA.parent;
        while(null != currentParent)
        {
            if(currentParent == transB)
            {
                return true;
            }
            currentParent = currentParent.parent;
        }
        return false;
    }

    public static string[] SplitString(string inputString, int stringLength = 500)
    {
        List<string> returnString = new List<string>();
        int length = Mathf.FloorToInt(inputString.Length / stringLength) + 1;
        for (int i = 0; i < length; i++)
        {
            int stringStartPosition = i * stringLength;
            if (stringStartPosition > inputString.Length)
            {
                break;
            }

            if (stringStartPosition + stringLength < inputString.Length)
            {
                returnString.Add(inputString.Substring(stringStartPosition, stringLength));

            }
            else
            {
                returnString.Add(inputString.Substring(stringStartPosition, inputString.Length - stringStartPosition));
            }
        }
        return returnString.ToArray();
    }

    public static bool IsPointerValid()
    {
        if (Input.GetMouseButton(0) == true || Input.touchCount == 1)
        {
            return true;
        }
        return false;
    }

    public static Vector3 GetPointerPosition()
    {
        if (Input.GetMouseButton(0) == true)
        {
            return Input.mousePosition;
        }
        if (Input.touchCount == 1)
        {
            return Input.GetTouch(0).position;
        }
        return Vector3.zero;
    }

    public static void SetGlobalScale(Transform transform, Vector3 globalScale)
    {
        transform.localScale = Vector3.one;
        transform.localScale = new Vector3(globalScale.x / transform.lossyScale.x, globalScale.y / transform.lossyScale.y, globalScale.z / transform.lossyScale.z);
    }

    public static void SetChildsLayer(Transform root, int layer)
    {
        var children = root.GetComponentsInChildren<Transform>(includeInactive: true);
        foreach (var child in children)
        {
            child.gameObject.layer = layer;
        }
    }
}
