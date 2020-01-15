namespace kTools.Pooling
{
    public abstract class Processor
    {
    }

    public abstract class Processor<T> : Processor
    {
#region Abstract
        /// <summary>Create new Instance of Type T using source object.</summary> 
        /// <param name="key">Unique identifer for Pool.</param>
        /// <param name="source">Source object.</param>
        /// <returns>New instance.</returns> 
        public abstract T CreateInstance(object key, T source);

        /// <summary>Destroy Instance of Type T.</summary> 
        /// <param name="key">Unique identifer for Pool.</param>
        /// <param name="instance">Instance to destroy.</param>
        public abstract void DestroyInstance(object key, T instance);
#endregion

#region Virtual
        /// <summary>Called when instance is enabled.</summary> 
        /// <param name="key">Unique identifer for Pool.</param>
        /// <param name="instance">Instance to enable.</param>
        public virtual void OnEnableInstance(object key, T instance) {}

        /// <summary>Called when instance is disabled.</summary> 
        /// <param name="key">Unique identifer for Pool.</param>
        /// <param name="instance">Instance to disable.</param>
        public virtual void OnDisableInstance(object key, T instance) {}
#endregion
    }
}
