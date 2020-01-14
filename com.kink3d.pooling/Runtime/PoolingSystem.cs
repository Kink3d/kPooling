using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace kTools.Pooling
{
    public static class PoolingSystem
    {
#region Fields
        readonly static List<Pool> s_Pools;
#endregion

#region Constructors
        static PoolingSystem()
        {
            // Create data
            s_Pools = new List<Pool>();
        }
#endregion

#region Pool
        /// <summary>Tests for existing Pool of type T with given key.</summary> 
        /// <param name="key">Unique identifer for Pool.</param>
        /// <returns>True if Pool exists.</returns> 
        public static bool HasPool<T>(object key) where T : Object
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
        /// <param name="baseInstance">The Object to base instances from.</param>
        /// <param name="instanceCount">Amount of instances to create.</param>
        public static void CreatePool<T>(object key, T baseInstance, int instanceCount) where T : Object
        {
            // Test argument validity
            if(key == null)
            {
                Debug.LogWarning("Key cannot be null.");
                return;
            }
            if(baseInstance == null)
            {
                Debug.LogWarning("BaseInstance cannot be null.");
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

            // Create Pool
            var pool = new Pool<T>(key, baseInstance, instanceCount);
            s_Pools.Add(pool);
        }

        /// <summary>Destroys existing Pool of type T with given key.</summary> 
        /// <param name="key">Unique identifer for Pool.</param>
        public static void DestroyPool<T>(object key) where T : Object
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
            }            
        }

        static bool TryGetPool<T>(object key, out Pool<T> value) where T : Object
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
        public static bool TryGetInstance<T>(object key, out T value) where T : Object
        {
            // Test argument validity
            if(key == null)
            {
                Debug.LogWarning("Key cannot be null.");
                value = null;
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
            value = null;
            return false;
        }

        /// <summary>Returns an Instance of type T to Pool with given key.</summary> 
        /// <param name="key">Unique identifer for Pool.</param>
        /// <param name="instance">Instance to return.</param>
        public static void ReturnInstance<T>(object key, T instance) where T : Object
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
    }
}
