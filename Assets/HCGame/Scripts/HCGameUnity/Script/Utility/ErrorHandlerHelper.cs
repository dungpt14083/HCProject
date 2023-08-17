
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

class ErrorHandlerHelper
{

    // public static Action InvalidTokenCallback { get; set; }
    public static bool HandleException(Exception ex)
    {
        if (ex is ServerException)
        {
            var sex = (ServerException)ex;
            switch ((LStatusCode)sex.ErrorCode)
            {
                case LStatusCode.ERR_INVALID_TOKEN:
                    HCGameManager.Instance.SignoutAndShowLogin();
                    // BGameData.Instance.CachedToken = string.Empty;
                    // BGUIManager.ShowLoginDialog();
                    HCGUIManager.ShowMessageDialog(new MessageBoxData(MessageConstant.ERROR, MessageConstant.ERR_INVALID_TOKEN + ": " + sex.ErrorCode));
                    break;
                case LStatusCode.ERR_USER_NOT_EXIST:
                    HCGUIManager.ShowMessageDialog(new MessageBoxData(MessageConstant.ERROR, MessageConstant.ERR_USER_NOT_EXIST + ": " + sex.ErrorCode));
                    break;
                case LStatusCode.ERR_LOGIN_FAIL:
                    HCGUIManager.ShowMessageDialog(new MessageBoxData(MessageConstant.ERROR, MessageConstant.LOGIN_ERROR + ": " + sex.ErrorCode));
                    break;
                case LStatusCode.ERR_USER_EXIST:
                    HCGUIManager.ShowMessageDialog(new MessageBoxData(MessageConstant.ERROR, MessageConstant.ERR_USER_EXIST + ": " + sex.ErrorCode));
                    break;
                case LStatusCode.ERR_REPASSWORD_NOT_SAME:
                    HCGUIManager.ShowMessageDialog(new MessageBoxData(MessageConstant.ERROR, $"Password not same: {sex.ErrorCode}"));
                    break;
                case LStatusCode.ERR_REGISTER_FAIL:
                    HCGUIManager.ShowMessageDialog(new MessageBoxData(MessageConstant.ERROR, MessageConstant.ERR_REGISTER_FAIL + ": " + sex.ErrorCode));
                    break;

                default:
                    HCGUIManager.ShowMessageDialog(new MessageBoxData(MessageConstant.ERROR, $"Unknow Server Exception: {sex.ErrorCode}"));

                    break;
            }
            Debug.LogError("Server Exception : " + sex.ErrorCode);
            return true;
        }
        Debug.LogError("Unknow Exception : " + ex.ToString() + "=\n" + ex.StackTrace);
        HCGUIManager.ShowMessageDialog(new MessageBoxData(MessageConstant.ERROR, MessageConstant.UNKNOW_ERROR));
        return false;
    }
}
