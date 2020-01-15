using UnityEngine;

namespace kTools.Pooling
{
    sealed class Instance<T>
    {
#region Fields
        readonly T m_Obj;
        bool m_ActiveSelf;
        float m_ActiveTime;
#endregion

#region Constructors
        public Instance(T obj)
        {
            m_Obj = obj;
            SetActive(false);
        }
#endregion

#region Properties
        public T obj => m_Obj;
        public bool activeSelf => m_ActiveSelf;
        public float activeTime => m_ActiveTime;
#endregion

#region State
        public void SetActive(bool value)
        {
            m_ActiveSelf = value;
            m_ActiveTime = value ? Time.realtimeSinceStartup : 0.0f;
        }
#endregion
    }    
}
