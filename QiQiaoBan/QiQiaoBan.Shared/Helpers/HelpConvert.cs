using System;
using System.Collections.Generic;
using System.Text;

namespace QiQiaoBan.Helpers
{
    static public class HelpConvert
    {
        static public string intToStringTime(int n)
        {
            return (n / 60).ToString("00") + ":" + (n % 60).ToString("00");
        }
    }
}
