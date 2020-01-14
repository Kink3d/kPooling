using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace kTools.Pooling
{
    abstract class Pool
    {
#region Fields
        readonly object m_Key;
#endregion

#region Constructors
        public Pool(object key)
        {
            m_Key = key;
        }
#endregion

#region Properties
        public object key => m_Key;
#endregion
    }

    sealed class Pool<T> : Pool, IDisposable where T : Object
    {
#region Fields
        readonly T m_BaseInstance;
        readonly Instance<T>[] m_Instances;
#endregion

#region Constructors
        public Pool(object key, T baseInstance, int instanceCount) : base(key)
        {
            // Set data
            m_BaseInstance = baseInstance;
            m_Instances = new Instance<T>[instanceCount];

            // Create instances
            for(int i = 0; i < instanceCount; i++)
            {
                var obj = Activator.CreateInstance<T>();
                var instance = new Instance<T>(obj);
                instances[i] = instance;
            }
        }
#endregion

#region Properties
        public T baseInstance => m_BaseInstance;
        public Instance<T>[] instances => m_Instances;
#endregion

#region IDisposable
        public void Dispose()
        {
            // Destroy instances
            var instanceCount = instances.Length;
            for(int i = 0; i < instanceCount; i++)
            {
                #if UNITY_EDITOR
                Object.DestroyImmediate(instances[i].obj);
                #else
                Object.Destroy(instances[i].obj);
                #endif
            }
        }
#endregion

#region Instance
        public T GetInstance()
        {
            Instance<T> value = null;

            void GetInactiveInstance()
            {
                if(value != null)
                    return;
                
                foreach(var instance in instances)
                {
                    if(!instance.activeSelf)
                        value = instance;
                }
                value = null;
            }

            void GetOldestInstance()
            {
                if(value != null)
                    return;
                
                var oldestTime = Mathf.Infinity;
                var oldestIndex = 0;
                for(int i = 0; i < instances.Length; i++)
                {
                    if(instances[i].activeTime < oldestTime)
                    {
                        oldestTime = instances[i].activeTime;
                        oldestIndex = i;
                    }
                }
                value = instances[oldestIndex];
            }

            // Get instance
            GetInactiveInstance();
            GetOldestInstance();
            
            // Enable instance
            value.SetActive(true);
            return value.obj;
        }

        public void ReturnInstance(T value)
        {
            // Find instance 
            foreach(var instance in instances)
            {
                if(instance.obj.Equals(value))
                {
                    // Disable instance
                    instance.SetActive(false);
                    return;
                }
            }

            // Instance not tracked
            Debug.LogWarning($"Pool ({key}) does not contain object ({value}).");
        }
#endregion
    } 
}
