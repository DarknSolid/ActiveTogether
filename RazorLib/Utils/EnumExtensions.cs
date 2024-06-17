using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RazorLib.Utils
{
    public static class EnumUtils
    {
        public static TEnum TryParse<TEnum>(string str) where TEnum : struct
        {
            TEnum temp;
            if (Enum.TryParse(str, out temp))
            {
                return temp;
            }
            else
            {
                return default;
            }
        }
    }
}
