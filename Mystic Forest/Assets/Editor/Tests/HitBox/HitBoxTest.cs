using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    public class HitBoxTest
    {
        HitBox a;
        GameObject b;

        [SetUp]
        public void SetUp()
        {
            a = Object.Instantiate(Resources.Load<GameObject>("TestPrefabs/Test Hit Collider GameObject")).GetComponent<HitBox>();
            b = Object.Instantiate(Resources.Load<GameObject>("TestPrefabs/Test Hit Collider GameObject"));
        }

        // A Test behaves as an ordinary method
        [UnityTest]
        public IEnumerator ObjectsNotOverlappingTest()
        {
            bool fired = false;
            a.transform.position = Vector3.zero;
            b.transform.position = new Vector3(10, 10);
            yield return null;
            a.CheckCollision(delegate { fired = true; });
            Assert.False(fired);
        }

        [UnityTest]
        public IEnumerator ObjectsOverlappingTest()
        {
            bool fired = false;
            a.transform.position = Vector3.zero;
            b.transform.position = Vector3.zero;
            yield return null;
            a.CheckCollision(delegate { fired = true; });
            Assert.True(fired);
        }

        [TearDown]
        public void TearDown()
        {
            Object.Destroy(a.gameObject);
            Object.Destroy(b.gameObject);
        }


    }
}
