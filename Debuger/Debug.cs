using System;
using System.Collections.Generic;
using System.Text;

namespace NEWTONS.Debuger
{
    public static class Debug
    {
        public static Action<string>? OnLog;
        public static Action<string>? OnWarning;
        public static Action<string>? OnError;

        internal static void Log(object message)
        {
            OnLog?.Invoke(message.ToString());
        }

        internal static void LogWarning(object message)
        {
            OnWarning?.Invoke(message.ToString());
        }

        internal static void LogError(object message)
        {
            OnError?.Invoke(message.ToString());
        }
    }
}
