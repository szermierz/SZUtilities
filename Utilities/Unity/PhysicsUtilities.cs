using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.WSA;

namespace SZUtilities.Physics
{
    [UnityEditor.InitializeOnLoad]
    public static class PhysicsUtilities
    {
        #region Debug
        
        public static DebugEx Debug = new DebugEx(nameof(PhysicsUtilities));

        #endregion
        
        #region Initialization
        
        static PhysicsUtilities()
        {
#if UNITY_EDITOR
            _UnityAssetProcessorCallbacks.onWillSaveAssets += _paths => _ResetCollisionLayers();
            _ResetCollisionLayers();
#endif
        }

        #endregion

        #region Collision layers

#if UNITY_EDITOR

        private const int c_maxLayersCount = 32;
        
        private static Dictionary<Type, ICollisionLayersSetup> s_collisionSetups = new Dictionary<Type, ICollisionLayersSetup>();
        public static IDictionary<Type, ICollisionLayersSetup> _CollisionSetups => s_collisionSetups;

        private static void _ResetCollisionLayers()
        {
            if (null == s_collisionSetups)
                s_collisionSetups = new Dictionary<Type, ICollisionLayersSetup>();

            var setupTypes = new HashSet<Type>(Reflection.FindDerivedTypes<ICollisionLayersSetup>());
            foreach (var type in setupTypes)
            {
                if (type.IsAbstract || type.IsInterface)
                    continue;

                if (s_collisionSetups.ContainsKey(type))
                    continue;
                
                var setup = Reflection.CreateObject<ICollisionLayersSetup>(type);
                s_collisionSetups.Add(type, setup);
            }

            var toDelete = new List<Type>();
            foreach (var setup in s_collisionSetups)
            {
                var setupType = setup.Value.GetType();
                if (!setupTypes.Contains(setupType))
                    toDelete.Add(setupType);
            }

            foreach (var setupType in toDelete)
                s_collisionSetups.Remove(setupType);
            
            if(_SetupCollisionLayers())
                UnityEditor.AssetDatabase.SaveAssets();
        }
        
        private static bool _SetupCollisionLayers()
        {
            if (!s_collisionSetups.Any())
                return false;
            
            var anyChanged = false;

            // Gather layers
            var layers = new List<string>(c_maxLayersCount);
            for (var i = 0; i < c_maxLayersCount; ++i)
            {
                var layerName = UnityEngine.LayerMask.LayerToName(i);
                if (string.IsNullOrEmpty(layerName))
                    continue;
                
                layers.Add(layerName);
            }
            
            // Gather collisions
            var collisions = new HashSet<string>();
            foreach (var collisionSetup in s_collisionSetups.Values)
            {
                var firstLayer = collisionSetup.LayerName;

                if (null == collisionSetup.CollisionLayers)
                    continue;
                
                foreach (var secondLayer in collisionSetup.CollisionLayers)
                {
                    var key = BuildKey(firstLayer, secondLayer);
                    collisions.Add(key);
                }
            }
            for (var i = 0; i < c_maxLayersCount; ++i)
            {
                var layerName = UnityEngine.LayerMask.LayerToName(i);
                
                if (s_collisionSetups.Any(_collisionSetup =>
                                              _collisionSetup.Value.LayerName == layerName
                                              && _collisionSetup.Value.NoCollisionWithSelf))
                    continue;
                
                var key = BuildKey(layerName, layerName);
                collisions.Add(key);
            }
            
            // Setup collisions
            for (var firstLayerIndex = 0; firstLayerIndex < c_maxLayersCount; ++firstLayerIndex)
            {
                var firstLayer = UnityEngine.LayerMask.LayerToName(firstLayerIndex);
                for (var secondLayerIndex = 0; secondLayerIndex < c_maxLayersCount; ++secondLayerIndex)
                {
                    var secondLayer = UnityEngine.LayerMask.LayerToName(secondLayerIndex);

                    var key = BuildKey(firstLayer, secondLayer);

                    var currentCollision = !UnityEngine.Physics.GetIgnoreLayerCollision(firstLayerIndex, secondLayerIndex);
                    var requiredCollision = collisions.Contains(key);

                    if (currentCollision != requiredCollision)
                    {
                        Debug.Log($"Setting layer collision between {firstLayer} and {secondLayer} to {requiredCollision}");
                        UnityEngine.Physics.IgnoreLayerCollision(firstLayerIndex, secondLayerIndex, !requiredCollision);
                        anyChanged = true;
                    }
                }
            }

            return anyChanged;

            string BuildKey(string firstLayerName, string secondLayerName)
            {
                const string c_keyDelimiter = "|";
                return string.Compare(firstLayerName, secondLayerName, StringComparison.Ordinal) > 0 
                    ? $"{firstLayerName}{c_keyDelimiter}{secondLayerName}" 
                    : $"{secondLayerName}{c_keyDelimiter}{firstLayerName}";
            }
        }
        
#endif
        
        #endregion
    }

    public interface ICollisionLayersSetup
    {
        string LayerName { get; }
        bool NoCollisionWithSelf { get; }
        IReadOnlyList<string> CollisionLayers { get; }
    }
}