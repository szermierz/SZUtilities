using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace DebugUtilities
{
    public struct DebugEx
    {
        #region Construction

        private readonly string m_sourceName;

        public DebugEx(UnityEngine.Object source)
            : this(source.GetType().Name)
        { }

        public DebugEx(string sourceName)
        {
            m_sourceName = sourceName;
        }

        #endregion

        #region Logging

        [DebuggerHidden]
        private string PrepareMessage(string message, UnityEngine.Object context)
        {
            return FormatText($"[{m_sourceName}] <b>{GetCallerMethodName(3)}</b>: {message}");
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

        [DebuggerHidden]
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static string GetCallerMethodName(int skipFrames = 1)
        {
            var stackTrace = new StackTrace(new StackFrame(skipFrames));
            return stackTrace.GetFrame(0).GetMethod().Name;
        }

        #endregion

        #region Debug utilities

        public string GetDebugName(UnityEngine.Object target)
        {
            return $"({target.GetType().Name}){target.name}{(target is UnityEngine.Component component ? TransformPath(component.transform.parent) : string.Empty)}";
        }

        private static string TransformPath(UnityEngine.Transform transform) => transform ? $"|{transform.name}{TransformPath(transform.parent)}" : string.Empty;

        #endregion
    }
}