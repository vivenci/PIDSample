using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PIDSample
{
    public class InnerArgumentException : ApplicationException
    {

        public InnerArgumentException()
        {

        }

        public InnerArgumentException(string message)
            : base(message)
        {

        }

        public InnerArgumentException(string message, Exception inner)
            : base(message, inner)
        {

        }

        public static string GenInfo(string sn, object sv, string dn, object dv)
        {
            string msg = string.Format("内部参数[{0}]的值 {1} 与[{2}]的值 {3} 不匹配.", sn, sv, dn, dv);
            return msg;
        }

    }

}
