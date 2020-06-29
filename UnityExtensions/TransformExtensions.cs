using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SZ.UnityExtensions
{
    public static class TransformExtensions
    {
        public static IEnumerable<Transform> GetChildren(this Transform transform)
        {
            foreach (Transform child in transform)
                yield return child;
        }

        public static IEnumerable<GameObject> GetChildrenGameObjects(this Transform transform)
        {
            return transform.GetChildren().Select(_child => _child.gameObject);
        }
    }
}