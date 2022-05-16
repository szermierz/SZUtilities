using System;

#if UNITY_EDITOR

namespace SZUtilities
{
    public class _UnityAssetProcessorCallbacks : UnityEditor.SaveAssetsProcessor
    {
        public static event Action<string> onWillCreateAsset;
        static void OnWillCreateAsset(string assetName)
        {
            onWillCreateAsset?.Invoke(assetName);
        }

        public static event Action<string, UnityEditor.RemoveAssetOptions> onWillDeleteAsset;
        static void OnWillDeleteAsset(string assetName, UnityEditor.RemoveAssetOptions options)
        {
            onWillDeleteAsset?.Invoke(assetName, options);
        }
        
        public static event Action<string, string> onWillMoveAsset;
        private static UnityEditor.AssetMoveResult OnWillMoveAsset(string sourcePath, string destinationPath)
        {
            onWillMoveAsset?.Invoke(sourcePath, destinationPath);
            return UnityEditor.AssetMoveResult.DidNotMove;
        }
        
        public static event Action<string[]> onWillSaveAssets;
        static string[] OnWillSaveAssets(string[] paths)
        {
            onWillSaveAssets?.Invoke(paths);
            return paths;
        }
    }
}

#endif