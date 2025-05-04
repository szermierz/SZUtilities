using System;
using System.Collections.Generic;
using SZUtilities.Extensions;
using UnityEngine;

namespace SZUtilities
{
    public class PrefabPool 
        : MonoBehaviourEx
    {
        private static PrefabPool s_instance;
        public static PrefabPool Instance
        {
            get
            {
                if(s_instance)
                    return s_instance;

                var gameObject = new GameObject();
                DontDestroyOnLoad(gameObject);
                s_instance = gameObject.AddComponent<PrefabPool>();
                return s_instance;
            }
        }

        private readonly Dictionary<GameObject, List<GameObject>> m_pools = new();

        private static readonly List<SpawnHandle> s_freeHandles = new();

        public IDisposable Rent<ComponentType>(ComponentType prefab, out ComponentType result)
            where ComponentType : Component
        {
            var handle = Rent(prefab.gameObject, out var spawn);
            result = spawn.GetComponent<ComponentType>();
            return handle;
        }

        public IDisposable Rent(GameObject prefab, out GameObject result)
        {
            if(!m_pools.ContainsKey(prefab))
                m_pools.Add(prefab, new());

            var pool = m_pools[prefab];
            if(0 == pool.Count)
                pool.Add(Instantiate(prefab));

            result = pool.PopUnorderedAt(^1);

            if (0 == s_freeHandles.Count)
                s_freeHandles.Add(new());

            var handle = s_freeHandles.PopUnorderedAt(^1);

            handle.Setup(this, pool, result);

            return handle;
        }

        private class SpawnHandle
            : IDisposable
        {
            private PrefabPool m_prefabPool;
            private List<GameObject> m_pool;
            private GameObject m_spawn;

            public void Setup(PrefabPool prefabPool, List<GameObject> pool, GameObject spawn)
            {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
                if (m_prefabPool || m_spawn || null != m_pool)
                    throw new Exception();
                if (null == pool || !prefabPool || !spawn)
                    throw new ArgumentNullException();
#endif

                m_prefabPool = prefabPool;
                m_pool = pool;
                m_spawn = spawn;
            }

            public void Dispose()
            {
                if (!m_prefabPool || !m_spawn)
                    return;
#if UNITY_EDITOR || DEVELOPMENT_BUILD
                if (null == m_pool)
                    throw new Exception();
#endif
                m_spawn.transform.SetParent(m_prefabPool.transform);
                m_spawn.SetActive(false);
                m_pool.Add(m_spawn);

                m_spawn = null;
                m_prefabPool = null;
                m_pool = null;

                s_freeHandles.Add(this);
            }
        }
    }
}