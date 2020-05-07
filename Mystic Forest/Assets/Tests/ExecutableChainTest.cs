using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    public class ExecutableChainTest
    {
        
        [Test]
        public void DifferentInstancesPerEnumeratorTest()
        {
            ExecutableChainSO executables = ScriptableObject.CreateInstance<ExecutableChainSO>();
            executables.attacks = new ExecutableSO[] { ScriptableObject.CreateInstance<PressExecutableSO>() };
            IEnumerator enumeratorOne = executables.GetEnumerator();
            enumeratorOne.MoveNext();
            object first = enumeratorOne.Current;
            IEnumerator enumeratorTwo = executables.GetEnumerator();
            enumeratorTwo.MoveNext();
            object second = enumeratorTwo.Current;
            Assert.AreNotSame(first, second);
        }
    }
}
