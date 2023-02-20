using System.Collections.Generic;
using UnityEngine;

namespace kTools.Pooling
{
    public sealed class GameObjectProcessor : Processor<GameObject>
    {
#region Fields
        static Dictionary<object, Transform> m_Containers;
#endregion

#region Constructors
        public GameObjectProcessor()
        {
            // Set data
            m_Containers = new Dictionary<object, Transform>();
        }
#endregion

#region Overrides

        [RuntimeInitializeOnLoadMethod]
        private static void ClearContainers()
        {
            m_Containers?.Clear();
        }

        public override GameObject CreateInstance(object key, GameObject source)
        {
            // Find container Transform matching key
            if(!m_Containers.TryGetValue(key, out _))
            {
                // No matching container Transform so create one
                Transform container = new GameObject($"Pool - {key}").transform;
                m_Containers.Add(key, container);
            }

            // Create Instance
            var obj = GameObject.Instantiate(source, Vector3.zero, Quaternion.identity);
            obj.name = $"{source.name}(Clone)";
            OnDisableInstance(key, obj);
            return obj;
        }

        public override void DestroyInstance(object key, GameObject instance)
        {
            // Destroy instance
            DestroyGameObject(instance);

            // Find container Transform matching key
            Transform container;
            if(m_Containers.TryGetValue(key, out container))
            {
                // Last instance so destroy container Transform
                if(container.childCount == 0)
                {
                    DestroyGameObject(container.gameObject);
                    m_Containers.Remove(key);
                }
            }
        }

        public override void OnEnableInstance(object key, GameObject instance)
        {
            // Parenting
            instance.transform.SetParent(null);

            // Active
            instance.SetActive(true);
        }

        public override void OnDisableInstance(object key, GameObject instance)
        {
            // Parenting
            Transform container;
            if(m_Containers.TryGetValue(key, out container))
            {
                instance.transform.SetParent(container);
            }

            // Active
            instance.SetActive(false);

            // Transform
            instance.transform.localPosition = Vector3.zero;
            instance.transform.localRotation = Quaternion.identity;
            instance.transform.localScale = Vector3.one;
        }
#endregion

#region GameObject
        void DestroyGameObject(GameObject gameObject)
        {
            #if UNITY_EDITOR
            Object.DestroyImmediate(gameObject);
            #else
            Object.Destroy(gameObject);
            #endif
        }
#endregion
    }
}
