using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace ExecutableChainTest
{
    public class ExecutableChainTest
    {
        // Different Enumerators return different things
        [Test]
        public void DifferentEnumeratorsGiveDifferentObjectsTest()
        {
            PressExecutableSO press = ScriptableObject.CreateInstance<PressExecutableSO>();
            ExecutableChainSO chain = ScriptableObject.CreateInstance<ExecutableChainSO>();
            press.executionEvent = ScriptableObject.CreateInstance<TestExecutionEvent>();
            chain.executables = new ExecutableSO[] { press };
            IEnumerator enumOne = chain.GetEnumerator();
            IEnumerator enumTwo = chain.GetEnumerator();
            enumOne.MoveNext();
            enumTwo.MoveNext();
            Assert.AreNotSame(enumOne.Current, enumTwo.Current);
        }

    }
}
