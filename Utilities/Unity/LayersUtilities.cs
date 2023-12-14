using System;
using System.Collections.Generic;

namespace SZUtilities.Layers
{
#if UNITY_EDITOR
    [UnityEditor.InitializeOnLoad]
#endif
    public static class LayersUtilities
    {
        public static DebugEx Debug = new DebugEx(nameof(LayersUtilities));
        
        static LayersUtilities()
        {
#if UNITY_EDITOR
            _UnityAssetProcessorCallbacks.onWillSaveAssets += _paths => _ResetLayers();
            _ResetLayers();
#endif
        }
        
#if UNITY_EDITOR

        private static Dictionary<string, ILayerSetup> s_layerSetups = new Dictionary<string, ILayerSetup>();
        public static IDictionary<string, ILayerSetup> _LayerSetups => s_layerSetups;
        
        [UnityEditor.MenuItem("Tools/SZUtilities/ResetLayers")]
        public static void _ResetLayers()
        {
            if (null == s_layerSetups)
                s_layerSetups = new Dictionary<string, ILayerSetup>();

            var layersSetupTypes = new HashSet<Type>(Reflection.FindDerived<ILayerSetup>());
            foreach (var type in layersSetupTypes)
            {
                if (type.IsAbstract || type.IsInterface)
                    continue;
                
                var setup = Reflection.CreateObject<ILayerSetup>(type);
                if(!s_layerSetups.ContainsKey(setup.LayerName))
                    s_layerSetups.Add(setup.LayerName, setup);
            }

            var toDelete = new List<ILayerSetup>();
            foreach (var setup in s_layerSetups)
            {
                var setupType = setup.Value.GetType();
                if (layersSetupTypes.Contains(setupType))
                    continue;
                
                toDelete.Add(setup.Value);
            }

            foreach (var setup in toDelete)
            {
                s_layerSetups.Remove(setup.LayerName);
            }
            
            if(_EnsureLayers())
                UnityEditor.AssetDatabase.SaveAssets();
        }

        private static bool _EnsureLayers()
        {
            if (0 == s_layerSetups.Count)
                return false;
            
            var tagManager = new UnityEditor.SerializedObject(UnityEditor.AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
            var layerProps = tagManager.FindProperty("layers");
            var propCount = layerProps.arraySize;

            var anyChanged = false;
            
            for (var propIndex = 8; propIndex < propCount; propIndex++)
            {
                var layerProp = layerProps.GetArrayElementAtIndex(propIndex);
                var layerName = layerProp.stringValue;

                if (s_layerSetups.ContainsKey(layerName))
                    continue;
                if(string.IsNullOrEmpty(layerName))
                    continue;
                
                Debug.Log($"Removed layer {layerProp.stringValue} at {propIndex}");
                layerProp.stringValue = string.Empty;
                anyChanged = true;
            }
            
            var i = 8;
            foreach (var setupPair in s_layerSetups)
            {
                if (UnityEngine.LayerMask.NameToLayer(setupPair.Key) >= 0)
                    continue;
                
                for (; i < propCount; ++i)
                {
                    var layerProp = layerProps.GetArrayElementAtIndex(i);
                    
                    if (string.IsNullOrEmpty(layerProp.stringValue))
                    {
                        layerProp.stringValue = setupPair.Key;
                        anyChanged = true;
                        Debug.Log($"Added layer {setupPair.Key} at {i}");
                        break;
                    }
                }
            }
 
            if(anyChanged)
                tagManager.ApplyModifiedProperties();

            return anyChanged;
        }
        
#endif
        
    }
}