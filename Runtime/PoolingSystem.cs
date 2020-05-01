using System;
using System.Collections.Generic;
using UnityEngine;

namespace kTools.Pooling
{
    public static class PoolingSystem
    {
#region Fields
        readonly static List<Pool> s_Pools;
        readonly static List<Processor> s_Processors;
#endregion

#region Constructors
        static PoolingSystem()
        {
            // Create data
            s_Pools = new List<Pool>();
            s_Processors = new List<Processor>();
            GatherProcessors();
        }
#endregion

#region Pool
        [RuntimeInitializeOnLoadMethod]
        private static void ClearPoolsOnLoad()
        {
            s_Pools.Clear();
        }
    
        /// <summary>Tests for existing Pool of type T with given key.</summary> 
        /// <param name="key">Unique identifer for Pool.</param>
        /// <returns>True if Pool exists.</returns> 
        public static bool HasPool<T>(object key)
        {
            // Test argument validity
            if(key == null)
            {
                Debug.LogWarning("Key cannot be null.");
                return false;
            }

            // Find Pool
            var hasPool = TryGetPool<T>(key, out _);
            return hasPool;
        }
        
        /// <summary>Creates a new Pool of type T.</summary> 
        /// <param name="key">Unique identifer for Pool.</param>
        /// <param name="source">The object to base instances from.</param>
        /// <param name="instanceCount">Amount of instances to create.</param>
        public static void CreatePool<T>(object key, T source, int instanceCount)
        {
            // Test argument validity
            if(key == null)
            {
                Debug.LogWarning("Key cannot be null.");
                return;
            }
            if(source == null)
            {
                Debug.LogWarning("Source cannot be null.");
                return;
            }
            if(instanceCount < 1)
            {
                Debug.LogWarning("InstanceCount cannot less than one.");
                return;
            }

            // Test if Pool already exists
            if(HasPool<T>(key))
            {
                Debug.LogWarning($"Pool already exists with key ({key}).");
                return;
            }

            // Get Processor for T
            Processor<T> processor;
            if(!TryGetProcessor(out processor))
            {
                Debug.LogWarning($"No processor found for Type ({typeof(T)}).");
                return;
            }

            // Create Pool
            var pool = new Pool<T>(key, source, instanceCount, processor);
            s_Pools.Add(pool);
        }

        /// <summary>Destroys existing Pool of type T with given key.</summary> 
        /// <param name="key">Unique identifer for Pool.</param>
        public static void DestroyPool<T>(object key)
        {
            // Test argument validity
            if(key == null)
            {
                Debug.LogWarning("Key cannot be null.");
                return;
            }

            // Find Pool
            Pool<T> pool;
            if(TryGetPool<T>(key, out pool))
            {
                // Remove Pool
                s_Pools.Remove(pool);
                pool.Dispose();
                return;
            }

            // No Pool found
            Debug.LogWarning($"Pool does not exist with key ({key}).");      
        }

        static bool TryGetPool<T>(object key, out Pool<T> value)
        {
            foreach(var pool in s_Pools)
            {
                // Test for Non-matching key
                if(!pool.key.Equals(key))
                    continue;

                // Test for mismatching Type
                if(!(pool is Pool<T> typedPool))
                {
                    Debug.LogWarning($"Pool with key ({key}) is not of Type ({typeof(T).FullName}).");
                    break;
                }

                // Pool found
                value = typedPool;
                return true;
            }

            // Pool not found
            value = null;
            return false;
        }
#endregion

#region Instance
        /// <summary>Trys to get an Instance of type T from Pool with given key.</summary> 
        /// <param name="key">Unique identifer for Pool.</param>
        /// <param name="value">Returned instance.</param>
        /// <returns>True if returned instance is not null.</returns> 
        public static bool TryGetInstance<T>(object key, out T value)
        {
            // Test argument validity
            if(key == null)
            {
                Debug.LogWarning("Key cannot be null.");
                value = default(T);
                return false;
            }

            // Find Pool
            Pool<T> pool;
            if(TryGetPool<T>(key, out pool))
            {
                // Get Instance
                value = pool.GetInstance();
                return true;
            }
            
            // Pool not found
            value = default(T);
            return false;
        }

        /// <summary>Returns an Instance of type T to Pool with given key.</summary> 
        /// <param name="key">Unique identifer for Pool.</param>
        /// <param name="instance">Instance to return.</param>
        public static void ReturnInstance<T>(object key, T instance)
        {
            // Test argument validity
            if(key == null)
            {
                Debug.LogWarning("Key cannot be null.");
                return;
            }
            if(instance == null)
            {
                Debug.LogWarning("Instance cannot be null.");
                return;
            }

            // Find Pool
            Pool<T> pool;
            if(TryGetPool<T>(key, out pool))
            {
                // Return instance
                pool.ReturnInstance(instance);
            }
        }
#endregion

#region Processors
        static void GatherProcessors()
        {
            // Avoid redundant execution
            if(s_Processors.Count != 0)
                return;

            // Search all Assemblies for Processors
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (var type in assembly.GetTypes())
                {
                    if (type.IsClass && !type.IsAbstract && type.IsSubclassOf(typeof(Processor)))
                    {
                        // Add to Processor list
                        var processor = (Processor)Activator.CreateInstance(type);
                        s_Processors.Add(processor);
                    }
                }
            }
        }

        static bool TryGetProcessor<T>(out Processor<T> value)
        {
            // Find Processor of matching type T
            foreach(var processor in s_Processors)
            {
                if(processor is Processor<T> typedProcessor)
                {
                    // Processor found
                    value = typedProcessor;
                    return true;
                }
            }

            // No Processor found
            value = null;
            return false;
        }
#endregion
    }
}
