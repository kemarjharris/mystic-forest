using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    public class ActionWrapperTest
    {
        // invoke on empty does not throw error
        [Test]
        public void Invoke_EmptyWrapper_DoesNotThrowError()
        {
            ActionWrapper wrapper = new ActionWrapper();
            Assert.DoesNotThrow(() => wrapper.Invoke());
        }
        
        // adding and call calls delgate
        [Test]
        public void AddAction_InvokeCalled_CallsAction()
        {
            bool called = false;
            void call() => called = true;
            ActionWrapper wrapper = new ActionWrapper();
            wrapper.AddAction(call);
            wrapper.Invoke();
            Assert.True(called);
        }

        // removing non existant does not throw error
        [Test]
        public void RemoveAction_NonExistantAction_DoesNotThrowError()
        {
            void call() { }
            ActionWrapper wrapper = new ActionWrapper();
            Assert.DoesNotThrow(() => wrapper.RemoveAction(call));
        }

        // removing delgate stops call
        [Test]
        public void Invoke_RemovedAction_DoesNotCallAction()
        {
            bool called = false;
            void call() => called = true;
            ActionWrapper wrapper = new ActionWrapper();
            wrapper.AddAction(call);
            wrapper.RemoveAction(call);
            wrapper.Invoke();
            Assert.False(called);
        }
    }
}
