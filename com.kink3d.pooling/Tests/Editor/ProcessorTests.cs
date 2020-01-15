using NUnit.Framework;
using UnityEngine.TestTools;

namespace kTools.Pooling.Editor.Tests
{
    public static class ProcessorTestResults
    {
#region Properies
        public static bool instanceCreated;
        public static bool instanceDestroyed;
        public static bool instanceEnabled;
        public static bool instanceDisabled;
#endregion
    }

    public sealed class TestObject
    {
    }

    public sealed class TestProcessor : Processor<TestObject>
    {
#region Overrides
        public override TestObject CreateInstance(object key, TestObject source)
        {
            ProcessorTestResults.instanceCreated = true;
            var obj = new TestObject();
            return obj;
        }

        public override void DestroyInstance(object key, TestObject instance)
        {
            ProcessorTestResults.instanceDestroyed = true;
            instance = null;
        }

        public override void OnEnableInstance(object key, TestObject instance)
        {
            ProcessorTestResults.instanceEnabled = true;
        }

        public override void OnDisableInstance(object key, TestObject instance)
        {
            ProcessorTestResults.instanceDisabled = true;
        }
#endregion
    }

    public sealed class ProcessorTests
    {
#region Fields
        readonly TestObject m_Key;
        readonly TestObject m_Obj;
        readonly int m_InstanceCount;
#endregion

#region Constructors
        public ProcessorTests()
        {
            m_Key = new TestObject();
            m_Obj = new TestObject();
            m_InstanceCount = 1;
        }
#endregion

#region Setup
        [SetUp]
        public void SetUp()
        {
            // Ensure Pools are set up
            if(!PoolingSystem.HasPool<TestObject>(m_Key))
            {
                PoolingSystem.CreatePool(m_Key, m_Obj, m_InstanceCount);
            }
        }
#endregion

#region Tests
        [Test]
        public void CanCreateInstance()
        {
            // Result
            Assert.IsTrue(ProcessorTestResults.instanceCreated);
            LogAssert.NoUnexpectedReceived();
        }
        
        [Test]
        public void CanDestroyInstance()
        {
            // Execution
            PoolingSystem.DestroyPool<TestObject>(m_Key);

            // Result
            Assert.IsTrue(ProcessorTestResults.instanceDestroyed);
            LogAssert.NoUnexpectedReceived();
        }

        [Test]
        public void CanEnableInstance()
        {
            // Setup
            TestObject instance;
            
            // Execution
            var getInstance = PoolingSystem.TryGetInstance(m_Key, out instance);

            // Result
            Assert.IsTrue(ProcessorTestResults.instanceEnabled);
            LogAssert.NoUnexpectedReceived();
        }

        [Test]
        public void CanDisableInstance()
        {
            // Setup
            TestObject instance;
            
            // Execution
            PoolingSystem.TryGetInstance(m_Key, out instance);
            PoolingSystem.ReturnInstance(m_Key, instance);

            // Result
            Assert.IsTrue(ProcessorTestResults.instanceDisabled);
            LogAssert.NoUnexpectedReceived();
        }
#endregion
    }
}
