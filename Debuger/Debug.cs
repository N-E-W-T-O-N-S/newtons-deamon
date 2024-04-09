using System;
using System.Collections.Generic;
using System.Text;

namespace NEWTONS.Debuger
{
    /// <summary>
    /// NEWTONS own Debug class
    /// </summary>
    public static class Debug
    {
        /// <summary>
        /// delegates log events
        /// </summary>
        public static Action<string>? OnLog;
        /// <summary>
        /// delegates warning events
        /// </summary>
        public static Action<string>? OnWarning;
        /// <summary>
        /// delegates error events
        /// </summary>
        public static Action<string>? OnError;

        /// <summary>
        /// raises a log-message event
        /// </summary>
        /// <param name="message">message to be called</param>
        internal static void Log(object message)
        {
            OnLog?.Invoke(message.ToString());
        }

        /// <summary>
        /// raises a warning-message event
        /// </summary>
        /// <param name="message">message to be called</param>
        internal static void LogWarning(object message)
        {
            OnWarning?.Invoke(message.ToString());
        }

        /// <summary>
        /// raises a error-message event
        /// </summary>
        /// <param name="message">message to be called</param>
        internal static void LogError(object message)
        {
            OnError?.Invoke(message.ToString());
        }
    }
}
