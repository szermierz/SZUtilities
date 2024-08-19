using System;
using System.Collections.Generic;
using UnityEngine;

namespace SZUtilities.Extensions
{
    public static class ComponentExtensions
    {
        #region GetComponentsInChildren

        private static void GetComponentsInChildren<Type>(Component component, List<Type> result, bool includeInactive)
            where Type : Component
        {
            var transform = component.transform;
            for(var i = 0; i < transform.childCount; ++i)
            {
                var child = transform.GetChild(i);
                if(!includeInactive && !child.gameObject.activeInHierarchy)
                    continue;

                if (child.GetComponent<Type>() is { } childComponent)
                    result.Add(childComponent);

                GetComponentsInChildren(child, result, includeInactive);
            }
        }

        public static IDisposable GetComponentsInChildren<Type>(this Component component, out List<Type> result, bool includeInactive = false)
            where Type : Component
        {
            var handle = ListRenting.Rent(out result);
            GetComponentsInChildren(component, result, includeInactive);
            return handle;
        }

        private static void GetComponentsInChildren(Component component, Type type, List<Component> result, bool includeInactive)
        {
            var transform = component.transform;
            for (var i = 0; i < transform.childCount; ++i)
            {
                var child = transform.GetChild(i);
                if (!includeInactive && !child.gameObject.activeInHierarchy)
                    continue;

                if (child.GetComponent(type) is { } childComponent)
                    result.Add(childComponent);

                GetComponentsInChildren(child, type, result, includeInactive);
            }
        }

        public static IDisposable GetComponentsInChildren(this Component component, Type type, out List<Component> result, bool includeInactive = false)
        {
            var handle = ListRenting.Rent(out result);
            GetComponentsInChildren(component, type, result, includeInactive);
            return handle;
        }

        #endregion

        #region GetComponents

        public static IDisposable GetComponents<Type>(this Component component, out List<Type> result, bool includeInactive = false)
            where Type : Component
        {
            var handle = ListRenting.Rent(out result);

            var transform = component.transform;
            for (var i = 0; i < transform.childCount; ++i)
            {
                var child = transform.GetChild(i);
                if (!includeInactive && !child.gameObject.activeInHierarchy)
                    continue;

                if (child.GetComponent<Type>() is { } childComponent)
                    result.Add(childComponent);
            }

            return handle;
        }

        public static IDisposable GetComponents(this Component component, Type type, out List<Component> result, bool includeInactive = false)
        {
            var handle = ListRenting.Rent(out result);

            var transform = component.transform;
            for (var i = 0; i < transform.childCount; ++i)
            {
                var child = transform.GetChild(i);
                if (!includeInactive && !child.gameObject.activeInHierarchy)
                    continue;

                if (child.GetComponent(type) is { } childComponent)
                    result.Add(childComponent);
            }

            return handle;
        }

        #endregion
    }
}

