using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

    public class BaseResponse<T>
    {
        public int status;
        public T data;
    }

