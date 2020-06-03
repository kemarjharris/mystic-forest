using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    public abstract class BasePhysicsZTest
    {
        
        public IPhysicsZ physics;

        [SetUp]
        public abstract void SetUp();

        // velocity 0 on start
        [UnityTest]
        public IEnumerator NotMovingOnStartTest()
        {
            yield return new WaitUntil(() => physics.IsGrounded);
            yield return new WaitForSeconds(1f);
            Vector3 pos = physics.transform.position;
            yield return new WaitForFixedUpdate();
            Assert.AreEqual(pos, physics.transform.position);
        }

        // add force
        // x force adds to x
        [UnityTest]
        public IEnumerator XForceAddsToXTest()
        {
            yield return new WaitUntil(() => physics.IsGrounded);
            Vector3 pos = physics.transform.position;
            physics.SetVelocity(new VectorZ(10, 0), 0);
            yield return new WaitForFixedUpdate();
            Assert.Less(pos.x, physics.transform.position.x);
        }

        // x slows down test
        [UnityTest]
        public IEnumerator XSlowsDownTest()
        {
            yield return new WaitUntil(() => physics.IsGrounded);
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
            yield return new WaitUntil(() => physics.IsGrounded);
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
            yield return new WaitUntil(() => physics.IsGrounded);
            Vector3 pos = physics.transform.position;
            physics.SetVelocity(new VectorZ(0, 10), 0);
            yield return new WaitForFixedUpdate();
            Assert.Less(pos.y, physics.transform.position.y);
            Assert.Less(pos.z, physics.transform.position.z);
        }

        // y and z slow down
        [UnityTest]
        public IEnumerator YZSlowsDownTest()
        {
            yield return new WaitUntil(() => physics.IsGrounded);
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
        }

        // y and z eventually stop
        [UnityTest]
        public IEnumerator YZEventuallyStopsTest()
        {
            yield return new WaitUntil(() => physics.IsGrounded);
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
            yield return new WaitUntil(() => physics.IsGrounded);
            Vector3 start = physics.transform.position;
            physics.SetVelocity(new VectorZ(0, 0), 10);
            yield return new WaitForFixedUpdate();
            Assert.AreEqual(start.z, physics.transform.position.z);
        }

        // negative y force while grounded does not decrease y pos
        [UnityTest]
        public IEnumerator NegativeForceGroundedDoesNotDecreaseYPosTest()
        {
            yield return new WaitUntil(() => physics.IsGrounded);
            yield return new WaitForSeconds(1);
            Vector3 start = physics.transform.position;
            physics.SetVelocity(new VectorZ(0, 0), -10);
            yield return new WaitForFixedUpdate();
            Assert.AreEqual(start.y, physics.transform.position.y);
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
            physics.transform.position += new Vector3(0, 50, 0);
            float timeout = 10;
            while (timeout > 0 && !physics.IsGrounded)
            {
                timeout -= Time.fixedDeltaTime;
                yield return new WaitForFixedUpdate();
            }
            if (!physics.IsGrounded) Assert.Fail("Object never became grounded");
        }

        // set x velocity instantly changes velocity
        [UnityTest]
        public IEnumerator SetVelocityXChangesVelocityTest()
        {
            yield return new WaitUntil(() => physics.IsGrounded);
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
            yield return new WaitUntil(() => physics.IsGrounded);
            physics.SetVelocity(new VectorZ(0, 100), 0);
            yield return new WaitForSeconds(0.5f);
            physics.SetVelocity(new VectorZ(0, -10), 0);
            Vector3 oldPos = physics.transform.position;
            yield return new WaitForFixedUpdate();
            Assert.Greater(oldPos.y, physics.transform.position.y);
            Assert.Greater(oldPos.z, physics.transform.position.z);
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
        public abstract void TearDown();

    }

    public class PhysicsZTest : BasePhysicsZTest
    {
        [SetUp]
        public override void SetUp()
        {
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


        [TearDown]
        public override void TearDown()
        {
            Object.Destroy(physics.transform.gameObject);
        }
    }

    public class BattlerPhysicsZTest : BasePhysicsZTest
    {

        BattlerPhysicsZ bPhysics;

        [SetUp]
        public override void SetUp()
        {
            GameObject g = new GameObject();
            g.AddComponent<Rigidbody>();
            g.AddComponent<BoxCollider>();
            bPhysics = g.AddComponent<BattlerPhysicsZ>();
            physics = bPhysics;
        }

        public IEnumerator WaitUntilGrounded()
        {
            Vector3 oldPos;
            do
            {
                oldPos = bPhysics.transform.position;
                yield return new WaitForFixedUpdate();

            } while (oldPos != bPhysics.transform.position);
        }

        // freeze stops rb movement
        [UnityTest]
        public IEnumerator Freeze_MovingObject_StopsMoving()
        {
            yield return WaitUntilGrounded();
            Vector3 start = physics.transform.position;
            bPhysics.SetVelocity(new VectorZ(10, 0), 0);
            yield return new WaitForFixedUpdate();
            Assert.AreNotEqual(start, bPhysics.transform.position);
            start = bPhysics.transform.position;
            bPhysics.freeze = true;
            yield return new WaitForFixedUpdate();
            Assert.AreEqual(start, bPhysics.transform.position);
        }
         
        // freeze does not allow rb movement
        [UnityTest]
        public IEnumerator Freeze_MoveFrozenObject_DoesNotMove()
        {
            yield return WaitUntilGrounded();
            Vector3 start = bPhysics.transform.position;
            bPhysics.freeze = true;
            bPhysics.SetVelocity(new VectorZ(10, 0), 0);
            yield return new WaitForFixedUpdate();
            Assert.AreEqual(start, bPhysics.transform.position);
        }

        // freeze keeps contact with ground
        [UnityTest]
        public IEnumerator Freeze_Grounded_KeepsContactWithGround()
        {
            yield return WaitUntilGrounded();
            Assert.True(bPhysics.IsGrounded);
            bPhysics.freeze = true;
            Assert.True(bPhysics.IsGrounded);
        }

        // unfreeze restores velocity
        [UnityTest]
        public IEnumerator Unfreeze_WasMoving_RestoresVelocity()
        {
            
            yield return WaitUntilGrounded();
            // Start moving
            bPhysics.SetVelocity(new VectorZ(10, 0), 0);
            yield return new WaitForFixedUpdate();
            // freeze to stop movement
            bPhysics.freeze = true;
            Vector3 pos = bPhysics.transform.position;
            //move to next frame
            yield return new WaitForFixedUpdate();
            // Assert the object is frozen
            Assert.AreEqual(pos, bPhysics.transform.position);
            // track and unfreeze position
            pos = bPhysics.transform.position;
            bPhysics.freeze = false;
            // move to next frame
            yield return new WaitForFixedUpdate();
            // assert the pbject is moving again
            Assert.AreNotEqual(pos, bPhysics.transform.position);
        }

        // lockZ ground vector does not move on Z axis
        [UnityTest]
        public IEnumerator LockZ_ZLocked_NoZMovement()
        {
            yield return WaitUntilGrounded();
            Vector3 start = bPhysics.transform.position;
            bPhysics.lockZ = true;
            bPhysics.SetVelocity(new VectorZ(0, 10), 0);
            yield return new WaitForFixedUpdate();
            Assert.AreEqual(start.z, bPhysics.transform.position.z);
        }

        // spawn on top of battler pushes battler away
        // fall on top of battler pushes battler away
        // high negative does not fall through ground

        [TearDown]
        public override void TearDown()
        {
            Object.Destroy(physics.transform.gameObject);
        }

    }


}
