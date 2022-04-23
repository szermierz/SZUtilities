using UnityEngine;

namespace SZUtilities
{
    public static class ApplicationStatus
    {
        public static bool IsQuitting { get; private set; }
        
        [RuntimeInitializeOnLoadMethod]
        static void RegisterQuitting()
        {
            IsQuitting = false;
            Application.quitting -= OnQuit;
            Application.quitting += OnQuit;
        }

        private static void OnQuit()
        {
            IsQuitting = true;
        }
    }
}
