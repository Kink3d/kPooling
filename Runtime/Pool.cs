using System;
using UnityEngine;

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

    sealed class Pool<T> : Pool, IDisposable
    {
#region Fields
        readonly T m_Source;
        readonly Instance<T>[] m_Instances;
        readonly Processor<T> m_Processor;
#endregion

#region Constructors
        public Pool(object key, T source, int instanceCount, Processor<T> processor) : base(key)
        {
            // Set data
            m_Source = source;
            m_Instances = new Instance<T>[instanceCount];
            m_Processor = processor;

            // Create instances
            for(int i = 0; i < instanceCount; i++)
            {
                var obj = processor.CreateInstance(key, source);
                var instance = new Instance<T>(obj);
                instances[i] = instance;
            }
        }
#endregion

#region Properties
        public T source => m_Source;
        public Instance<T>[] instances => m_Instances;
        public Processor<T> processor => m_Processor;
#endregion

#region IDisposable
        public void Dispose()
        {
            // Destroy instances
            var instanceCount = instances.Length;
            for(int i = 0; i < instanceCount; i++)
            {
                processor.DestroyInstance(key, instances[i].obj);
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
            processor.OnEnableInstance(key, value.obj);
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
                    processor.OnDisableInstance(key, instance.obj);
                    return;
                }
            }

            // Instance not tracked
            Debug.LogWarning($"Pool ({key}) does not contain object ({value}).");
        }
#endregion
    }
}
