using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    public class PhysicsZTest
    {
        public PhysicsZ physics;

        [SetUp]
        public void SetUp() {
            GameObject g = new GameObject();
            g.AddComponent<Rigidbody>();
            physics = g.AddComponent<PhysicsZ>();
        }

        // Is grounded on start
        [Test]
        public void GroundedOnStartTest()
        {
            Assert.True(physics.IsGrounded);
        }

        // yz same on start
        [Test]
        public void YZEqualOnStartTest()
        {
            Assert.AreEqual(physics.transform.position.y, physics.transform.position.z);
        }

        // velocity 0 on start
        [UnityTest]
        public IEnumerator NotMovingOnStartTest()
        {
            Vector3 pos = physics.transform.position;
            yield return new WaitForFixedUpdate();
            Assert.AreEqual(pos, physics.transform.position);
        }

        // horizontal -1 moves left
        [UnityTest]
        public IEnumerator NegativeHorizontalMovesLeftTest()
        {
            Vector3 pos = physics.transform.position;
            physics.Move(-1, 0);
            yield return new WaitForFixedUpdate();
            Assert.Greater(pos.x, physics.transform.position.x);
        }

        // horizontal 1 moves right
        [UnityTest]
        public IEnumerator PositiveHorizontalMovesRightTest()
        {
            Vector3 pos = physics.transform.position;
            physics.Move(1, 0);
            yield return new WaitForFixedUpdate();
            Assert.Less(pos.x, physics.transform.position.x);
        }

        // vertical -1 moves down and in
        [UnityTest]
        public IEnumerator NegativeVerticalMovesDownAndInTest()
        {
            Vector3 pos = physics.transform.position;
            physics.Move(0, -1);
            yield return new WaitForFixedUpdate();
            Assert.Greater(pos.y, physics.transform.position.y);
            Assert.Greater(pos.z, physics.transform.position.z);
            Assert.AreEqual(physics.transform.position.y, physics.transform.position.z);
        }

        // vertical 1 moves up and out
        [UnityTest]
        public IEnumerator PositiveVerticalMoveUpAndOutTest()
        {
            Vector3 pos = physics.transform.position;
            physics.Move(0, 1);
            yield return new WaitForFixedUpdate();
            Assert.Less(pos.y, physics.transform.position.y);
            Assert.Less(pos.z, physics.transform.position.z);
            Assert.AreEqual(physics.transform.position.y, physics.transform.position.z);
        }

        // add force
        // x force adds to x
        [UnityTest]
        public IEnumerator XForceAddsToXTest()
        {
            Vector3 pos = physics.transform.position;
            physics.SetVelocity(new VectorZ(10, 0), 0);
            yield return new WaitForFixedUpdate();
            Assert.Less(pos.x, physics.transform.position.x);
        }

        // x slows down test
        [UnityTest]
        public IEnumerator XSlowsDownTest()
        {
            Vector3 lastPos = physics.transform.position;
            float lastDistance = Mathf.Infinity;
            physics.SetVelocity(new VectorZ(10, 0), 0);
            for (int i = 0; i < 10; i++)
            {
                yield return new WaitForFixedUpdate();
                Vector3 pos = physics.transform.position;
                float distance = Vector3.Distance(lastPos, pos);
                Assert.Less(distance, lastDistance);
                lastPos = pos;
                lastDistance = distance;
            }
        }

        // x eventually stops test
        [UnityTest]
        public IEnumerator XEventuallyStopsTest()
        {
            physics.SetVelocity(new VectorZ(10, 0), 0);
            yield return new WaitForSeconds(2);
            Vector3 pos = physics.transform.position;
            yield return new WaitForFixedUpdate();
            Vector3 posNextFrame = physics.transform.position;
            Assert.AreEqual(pos, posNextFrame);
        }


        // y force and to y and z
        [UnityTest]
        public IEnumerator YZForceAddsToYZTest()
        {
            Vector3 pos = physics.transform.position;
            physics.SetVelocity(new VectorZ(0, 10), 0);
            yield return new WaitForFixedUpdate();
            Assert.Less(pos.y, physics.transform.position.y);
            Assert.Less(pos.z, physics.transform.position.z);
            Assert.AreEqual(physics.transform.position.y, physics.transform.position.z);
        }

        // y and z slow down
        [UnityTest]
        public IEnumerator YZSlowsDownTest()
        {
            Vector3 lastPos = physics.transform.position;
            float lastDistance = Mathf.Infinity;
            physics.SetVelocity(new VectorZ(0, 10), 0);
            for (int i = 0; i < 10; i++)
            {
                yield return new WaitForFixedUpdate();
                Vector3 pos = physics.transform.position;
                float distance = Vector3.Distance(lastPos, pos);
                Assert.Less(distance, lastDistance);
                lastPos = pos;
                lastDistance = distance;
            }
            Assert.AreEqual(physics.transform.position.y, physics.transform.position.z);
        }

        // y and z eventually stop
        [UnityTest]
        public IEnumerator YZEventuallyStopsTest()
        {
            physics.SetVelocity(new VectorZ(0, 10), 0);
            yield return new WaitForSeconds(2);
            Vector3 pos = physics.transform.position;
            yield return new WaitForFixedUpdate();
            Vector3 posNextFrame = physics.transform.position;
            Assert.AreEqual(pos, posNextFrame);

        }

        // v force adds to y
        [UnityTest]
        public IEnumerator VerticalForceAddsToYTest()
        {
            Vector3 start = physics.transform.position;
            physics.SetVelocity(new VectorZ(0, 0), 10);
            yield return new WaitForFixedUpdate();
            Assert.Less(start.y, physics.transform.position.y);
        }

        // y force does not add to z
        [UnityTest]
        public IEnumerator VerticalForceDoesNotAddToZTest()
        {
            Vector3 start = physics.transform.position;
            physics.SetVelocity(new VectorZ(0, 0), 10);
            yield return new WaitForFixedUpdate();
            Assert.AreEqual(start.z, physics.transform.position.z);
        }

        // negative y force while grounded does not decrease y pos
        [UnityTest]
        public IEnumerator NegativeForceGroundedDoesNotDecreaseYPosTest()
        {
            physics.SetVelocity(new VectorZ(0, 0), -10);
            yield return new WaitForFixedUpdate();
            Assert.AreEqual(physics.transform.position.y, physics.transform.position.z);
        }

        // Starts falling if y pos greater than z pos
        [UnityTest]
        public IEnumerator FallsIfAirborneTest()
        {
            physics.transform.position += new Vector3(0, 500, 0);
            Vector3 lastPos = physics.transform.position;
            float lastDistance = 0;
            for (int i = 0; i < 10; i++)
            {
                yield return new WaitForFixedUpdate();
                Vector3 pos = physics.transform.position;
                float distance = Vector3.Distance(lastPos, pos);
                Debug.Log(distance);
                Assert.Greater(distance, lastDistance);
                lastPos = pos;
                lastDistance = distance;
            }
        }

        // does not fall through ground
        [UnityTest]
        public IEnumerator EventuallyHitsGroundTest()
        {
            physics.transform.position += new Vector3(0, 10, 0);
            float timeout = 5;
            while (timeout > 0 && !physics.IsGrounded)
            {
                timeout -= Time.fixedDeltaTime;
                yield return new WaitForFixedUpdate();
            }
            if (!physics.IsGrounded) Assert.Fail("Object never became grounded");
            Assert.AreEqual(physics.transform.position.y, physics.transform.position.z);
        }

        // set x velocity instantly changes velocity
        [UnityTest]
        public IEnumerator SetVelocityXChangesVelocityTest()
        {
            physics.SetVelocity(new VectorZ(100, 0), 0);
            yield return new WaitForSeconds(0.5f);
            physics.SetVelocity(new VectorZ(-10, 0), 0);
            Vector3 oldPos = physics.transform.position;
            yield return new WaitForFixedUpdate();
            Assert.Greater(oldPos.x, physics.transform.position.x);
        }

        // set yz veloicity instantly changes velocity
        [UnityTest]
        public IEnumerator SetVelocityYZChangesVelocityTest()
        {
            physics.SetVelocity(new VectorZ(0, 100), 0);
            yield return new WaitForSeconds(0.5f);
            physics.SetVelocity(new VectorZ(0, -10), 0);
            Vector3 oldPos = physics.transform.position;
            yield return new WaitForFixedUpdate();
            Assert.Greater(oldPos.y, physics.transform.position.y);
            Assert.Greater(oldPos.z, physics.transform.position.z);
            Assert.True(physics.IsGrounded);
        }

        // set vertical velocity instantly changes velocity
        [UnityTest]
        public IEnumerator VerticalForceWhileFallingAddsToYTest()
        {
            physics.transform.position += new Vector3(0, 100, 0);
            // let object fall
            yield return new WaitForSeconds(1);
            physics.SetVelocity(new VectorZ(0, 0), 10);
            Vector3 oldPos = physics.transform.position;
            yield return new WaitForFixedUpdate();
            Assert.Less(oldPos.y, physics.transform.position.y);
        }

        [TearDown]
        public void TearDown()
        {
            Object.Destroy(physics.gameObject);
        }

    }
}
