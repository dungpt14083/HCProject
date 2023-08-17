using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public enum LStatusCode
{
    SUCCESS = 0,

    // 1 -> 300 // critical game error, relogin game
    ERR_INVALID_TOKEN = 1,
    ERR_USER_NOT_EXIST = 2,
    ERR_LOGIN_FAIL = 3,
    ERR_USER_EXIST = 4,
    ERR_REPASSWORD_NOT_SAME = 5,
    ERR_REGISTER_FAIL = 6,
}
