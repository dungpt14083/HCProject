using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class ServerException : Exception
{
    public int ErrorCode;
    public string response;

    public ServerException(int code = 0, string response = null)
    {
        ErrorCode = code;
        this.response = response;
    }
}