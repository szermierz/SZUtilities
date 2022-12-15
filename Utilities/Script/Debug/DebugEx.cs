using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace SZUtilities
{
    public struct DebugEx
    {
        #region Construction

        private readonly string m_sourceName;
        private string SourceName =>
            string.IsNullOrEmpty(m_sourceName)
            ? "Global"
            : m_sourceName;

        public DebugEx(string sourceName)
        {
            m_sourceName = sourceName;
        }

#if UNITY_2017_1_OR_NEWER
        public DebugEx(UnityEngine.Object source)
            : this(source.GetType().Name)
        { }
#endif

        #endregion

        #region Utilities

        [DebuggerHidden]
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static string GetCallerMethodName(int skipFrames = 1)
        {
#if UNITY_EDITOR
            var stackTrace = new StackTrace(new StackFrame(skipFrames));
            return stackTrace.GetFrame(0).GetMethod().Name;
#else
            return nameof(DebugEx);
#endif
        }

        #endregion

#if UNITY_2017_1_OR_NEWER

        #region Unity logging

        [DebuggerHidden]
        private string PrepareMessage(string message, UnityEngine.Object context)
        {
            return FormatText($"[{SourceName}] <b>{GetCallerMethodName(3)}</b>: {message}");
        }

        [DebuggerHidden]
        public void Log(object message, UnityEngine.Object context = null)
        {
            UnityEngine.Debug.Log(PrepareMessage(message.ToString(), context));
        }

        [DebuggerHidden]
        public void Log(string message, UnityEngine.Object context = null)
        {
            UnityEngine.Debug.Log(PrepareMessage(message, context));
        }

        [DebuggerHidden]
        public void LogError(object message, UnityEngine.Object context = null)
        {
            UnityEngine.Debug.LogError(PrepareMessage(message.ToString(), context));
        }

        [DebuggerHidden]
        public void LogError(string message, UnityEngine.Object context = null)
        {
            UnityEngine.Debug.LogError(PrepareMessage(message, context));
        }

        [DebuggerHidden]
        public void LogWarning(object message, UnityEngine.Object context = null)
        {
            UnityEngine.Debug.LogWarning(PrepareMessage(message.ToString(), context));
        }

        [DebuggerHidden]
        public void LogWarning(string message, UnityEngine.Object context = null)
        {
            UnityEngine.Debug.LogWarning(PrepareMessage(message, context));
        }

        [DebuggerHidden]
        private static string FormatText(string _input)
        {
            return Regex.Replace(_input, @"\[(\w+)\]", m =>
            {
                return $"[<b><color=#{Color(m.Groups[1].Value)}>{m.Groups[1].Value}</color></b>]";
            });
        }

        private static System.Security.Cryptography.SHA1 s_Hash = System.Security.Cryptography.SHA1.Create();

        [DebuggerHidden]
        private static string Color(string _input)
        {
            var bytes = s_Hash.ComputeHash(System.Text.Encoding.UTF8.GetBytes(_input));
            return string.Join("", bytes.Select(b => b.ToString("x2")).ToArray()).Substring(0, 6);
        }

#endregion

#region Unity debug utilities

        public string GetDebugName(UnityEngine.Object target)
        {
            return $"({target.GetType().Name}){target.name}{(target is UnityEngine.Component component ? TransformPath(component.transform.parent) : string.Empty)}";
        }

        private static string TransformPath(UnityEngine.Transform transform) => transform ? $"|{transform.name}{TransformPath(transform.parent)}" : string.Empty;

#endregion

#else

#region Plain cs logging

        private enum LogType
        {
            Log,
            Warning,
            Error
        }

        [DebuggerHidden]
        private void LogMessage(LogType logType, string message)
        {
            switch(logType)
            {
                case LogType.Error:
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write("Error: ");
                    break;
                case LogType.Warning:
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.Write("Warning: ");
                    break;
            }

            Console.ForegroundColor = (ConsoleColor)Color(SourceName);
            Console.Write($"[{SourceName}] ");

            Console.ResetColor();
            Console.WriteLine($"[{GetCallerMethodName(3)}] {message}");
        }

        [DebuggerHidden]
        public void Log(object message)
        {
            LogMessage(LogType.Log, message?.ToString());
        }

        [DebuggerHidden]
        public void Log(string message)
        {
            LogMessage(LogType.Log, message);
        }

        [DebuggerHidden]
        public void LogError(object message)
        {
            LogMessage(LogType.Error, message?.ToString());
        }

        [DebuggerHidden]
        public void LogError(string message)
        {
            LogMessage(LogType.Error, message);
        }

        [DebuggerHidden]
        public void LogWarning(object message)
        {
            LogMessage(LogType.Warning, message?.ToString());
        }

        [DebuggerHidden]
        public void LogWarning(string message)
        {
            LogMessage(LogType.Warning, message);
        }

        private static System.Security.Cryptography.SHA1 s_Hash = System.Security.Cryptography.SHA1.Create();

        [DebuggerHidden]
        private static int Color(string _input)
        {
            var bytes = s_Hash.ComputeHash(System.Text.Encoding.UTF8.GetBytes(_input));
        
            byte result = 0;
            foreach (var b in bytes)
                result ^= b;

            return (result % 14) + 1;
        }

#endregion

#endif
    }
}