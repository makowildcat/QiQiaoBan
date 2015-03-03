using System;
using System.Collections.Generic;
using System.Text;

namespace QiQiaoBan.Helpers
{
    static public class HelpConvert
    {
        /// <summary>
        /// Just to transform int to string formatted in time this 00:00
        /// 1 unit = 1 second
        /// </summary>
        /// <param name="n"></param>
        /// <returns></returns>
        static public string intToStringTime(int n)
        {
            return (n / 60).ToString("00") + ":" + (n % 60).ToString("00");
        }
    }
}
