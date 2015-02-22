using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace QiQiaoBan.Common
{
    /// <summary>The Logger class provides a quick and simple way of outputting debug information to the Output console in Visual Studio</summary>
    public static class Logger
    {
        /// <summary>Output a debug message to the console</summary>
        /// <param name="message">Text message</param>
        public static void Log(string message) { LogInfo(null, message); }

        /// <summary>Output a debug message to the console</summary>
        /// <param name="ex">An exception</param>
        public static void Log(Exception ex) { LogInfo(ex, null); }

        /// <summary>Output a debug message to the console</summary>
        /// <param name="ex">An exception</param>
        /// <param name="message">Text message</param>
        public static void Log(Exception ex, string message) { LogInfo(ex, message); }

        /// <summary>Output a debug message to the console</summary>
        /// <param name="ex">An exception</param>
        /// <param name="message">Text message</param>
        private static void LogInfo(Exception ex, string message)
        {
#if DEBUG
            if (!Debugger.IsAttached) return;
            if (ex == null && string.IsNullOrEmpty(message)) return;

            var tmp = DateTime.Now.TimeOfDay;
            var now = String.Format("{0}:{1}:{2}", tmp.Hours, tmp.Minutes, tmp.Seconds);
            if (ex != null && !string.IsNullOrEmpty(message)) Debug.WriteLine("{0} : {1} ({2})", now, ex.Message, message);
            else if (ex != null) Debug.WriteLine("{0} : {1}", now, ex.Message);
            else Debug.WriteLine("{0} : {1}", now, message);
#endif
        }
    }
}