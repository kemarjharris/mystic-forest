using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    public class LockOnTest
    {
        LockOn lockOn;
        List<GameObject> objects;

        [SetUp]
        public void SetUp()
        {
            // a sphere
            lockOn = Object.Instantiate(Resources.Load<GameObject>("Test Lock On GameObject")).GetComponent<LockOn>();
            lockOn.gameObject.transform.localScale = new Vector3(10, 10, 10);
            lockOn.gameObject.transform.position = Vector3.zero;
            objects = new List<GameObject>();
        }

        [TearDown]
        public void TearDown()
        {
            Object.Destroy(lockOn.gameObject);
            foreach (GameObject obj in objects)
            {
                if (obj != null) Object.Destroy(obj);
            }
        }

        public GameObject SpawnGOWithXPos(float xPos)
        {
            
            GameObject gO = Object.Instantiate(Resources.Load<GameObject>("Test Object"));
            gO.transform.position = Vector3.right * xPos;
            objects.Add(gO);
            return gO;
        }

        // No GameObjects in view null GameObject
        [Test]
        public void LockOn_NoGameObjects_ReturnsNull()
        {
            Assert.Null(lockOn.NextToLockOnTo());
        }

        // one GameObject in view gives that GameObject
        [UnityTest]
        public IEnumerator LockOn_OneGameObject_ReturnsGameObject()
        {
            GameObject expected = SpawnGOWithXPos(1);
            yield return null;
            GameObject result = lockOn.NextToLockOnTo();
            Assert.AreSame(expected, result);
        }

        // one battler locked on twice gives same battler
        [UnityTest]
        public IEnumerator LockOnMultipleTimes_OneGameObjectSameGameObject()
        {
            GameObject expected = SpawnGOWithXPos(1);
            yield return null;
            lockOn.NextToLockOnTo();
            GameObject result = lockOn.NextToLockOnTo();
            Assert.AreSame(expected, result);
        }

        // one GameObject targeted new GameObject enters new GameObject targeted
        [UnityTest]
        public IEnumerator LockOnMultipleTimes_GameObjectTargeted_NewGameObjectEnters_NewGameObjectTargeted()
        {
            // GameObject locked on should be current
            SpawnGOWithXPos(1);
            yield return null;
            lockOn.NextToLockOnTo();
            // Next GameObject Enters
            GameObject expected = SpawnGOWithXPos(2);
            GameObject result = lockOn.NextToLockOnTo();
        }        

        // one GameObject targeted leaves, new appears, new GameObject targetted
        [UnityTest]
        public IEnumerator LockOn_TargetedLeaves_NewAppears_NewGameObjectTargeted()
        {
            // GameObject locked on should be current
            GameObject GameObject = SpawnGOWithXPos(1);
            yield return null;
            lockOn.NextToLockOnTo();
            // targeted leaves 
            Object.Destroy(GameObject.gameObject);
            // New Target appears
            GameObject expected = SpawnGOWithXPos(1);
            yield return null;
            GameObject result = lockOn.NextToLockOnTo();
            Assert.AreSame(expected, result);
        }

        [UnityTest]
        public IEnumerator LockOn_TargetedLeaves_ReturnsNull()
        {
            GameObject toDestory = SpawnGOWithXPos(1);
            yield return null;
            lockOn.NextToLockOnTo();
            Object.Destroy(toDestory);
            yield return null;
            GameObject result = lockOn.NextToLockOnTo();
            Assert.IsNull(result);
        }

        // more than one GameObject gives closest GameObject
        [UnityTest]
        public IEnumerator LockOnMultiple_TwoGameObjectsFirstCall_ClosestGameObject()
        {
            GameObject expected = SpawnGOWithXPos(1);
            SpawnGOWithXPos(2);
            yield return null;
            GameObject result = lockOn.NextToLockOnTo();
            Assert.AreSame(expected, result);
        }

        // two GameObjects two calls gives second GameObject
        [UnityTest]
        public IEnumerator LockOnMultiple_TwoGameObjectsTwoCalls_SecondGameObject()
        {
            SpawnGOWithXPos(1);
            GameObject expected = SpawnGOWithXPos(2);
            yield return null;
            lockOn.NextToLockOnTo();
            GameObject result = lockOn.NextToLockOnTo();
            Assert.AreSame(expected, result);
        }

        [UnityTest]
        public IEnumerator LockOn_TwoGameObjectsThreeCalls_LoopsAround()
        {
            GameObject expected = SpawnGOWithXPos(1);
            SpawnGOWithXPos(2);
            yield return null;

            // lock on to closest
            lockOn.NextToLockOnTo();
            // lock on to second closest
            lockOn.NextToLockOnTo();
            // lock onto first again
            GameObject result = lockOn.NextToLockOnTo();
            Assert.AreSame(expected, result);
        }

        // two GameObjects same distance second call gives second GameObject
        [UnityTest]
        public IEnumerator LockOnMultiple_TwoGameObjectsSameDistanceSecondCall_SecondGameObject()
        {
            GameObject first = SpawnGOWithXPos(1);
            GameObject second = SpawnGOWithXPos(-1);
            yield return null;
            GameObject lockedOn = lockOn.NextToLockOnTo();
            GameObject result = lockOn.NextToLockOnTo();
            GameObject expected = lockedOn == first ? second : first;
            Assert.AreSame(expected, result);
        }

        [UnityTest]
        public IEnumerator LockOnMutiple_TwoGameObjectsClosestTargetedSwapDistance_SecondGameObject()
        {
            GameObject toSwap = SpawnGOWithXPos(1);
            GameObject expected = SpawnGOWithXPos(2);
            yield return null;
            // lock on to closest
            lockOn.NextToLockOnTo();
            // GameObjects swap distances, expected is now closest
            toSwap.gameObject.transform.position = Vector3.left * -2;
            expected.gameObject.transform.position = Vector3.left * -1;
            yield return null;
            GameObject result = lockOn.NextToLockOnTo();
            Assert.AreSame(expected, result);
        }

        // three GameObjects, closest locked on, closest leaves, new closest GameObject targeted
        [UnityTest]
        public IEnumerator LockOnMultiple_ThreeGameObjectsClosestLockedOnClosestLeaves_NewClosestTargeted()
        {
            GameObject GameObjectToLeave = SpawnGOWithXPos(1);
            GameObject expected = SpawnGOWithXPos(2);
            SpawnGOWithXPos(3);
            yield return null;
            // lock on to GameObject to leave
            lockOn.NextToLockOnTo();
            // targeted leaves 
            Object.Destroy(GameObjectToLeave.gameObject);
            yield return null;
            // should lock on the second furthest
            GameObject result = lockOn.NextToLockOnTo();
            Assert.AreSame(expected, result);
        }

        // three GameObjects, furthest leaves, second closest GameObject gets targetted
        [UnityTest]
        public IEnumerator LockOnMultiple_ThreeGameObjectsClosestLockedOnFurthestLeaves_SecondClosestTargeted()
        {
            SpawnGOWithXPos(1);
            GameObject expected = SpawnGOWithXPos(2);
            GameObject GameObjectToLeave = SpawnGOWithXPos(3);
            yield return null;
            // lock on to closest
            lockOn.NextToLockOnTo();
            // furthest leaves 
            Object.Destroy(GameObjectToLeave.gameObject);
            yield return null;
            // should lock on the second furthest
            GameObject result = lockOn.NextToLockOnTo();
            Assert.AreSame(expected, result);
        }

        [UnityTest]
        public IEnumerator LockOnMultiple_ThreeGameObjectsThreeCalls_FurthestTargeted()
        {
            SpawnGOWithXPos(1);
            SpawnGOWithXPos(2);
            GameObject expected = SpawnGOWithXPos(3);
            yield return null;
            // lock on to closest
            lockOn.NextToLockOnTo();
            // lock on to second closest
            lockOn.NextToLockOnTo();
            // should lock on the furthest
            GameObject result = lockOn.NextToLockOnTo();
            Assert.AreSame(expected, result);
        }

        [UnityTest]
        public IEnumerator LockOnMultiple_ThreeGameObjectsFourCalls_LoopsBackToFirst()
        {
            GameObject expected = SpawnGOWithXPos(1);
            SpawnGOWithXPos(2);
            SpawnGOWithXPos(3);
            yield return null;

            // lock on to closest
            lockOn.NextToLockOnTo();
            // lock on to second closest
            lockOn.NextToLockOnTo();
            // lock on the furthest
            lockOn.NextToLockOnTo();
            // lock onto first again
            GameObject result = lockOn.NextToLockOnTo();
            Assert.AreSame(expected, result);
        }

        // three GameObjects, middle GameObject leaves, third GameObject gets targeted
        [UnityTest]
        public IEnumerator LockOnMultiple_ThreeGameObjectsClosestLockedOnMiddleLeaves_SecondClosestTargeted()
        {
            SpawnGOWithXPos(1);
            GameObject GameObjectToLeave = SpawnGOWithXPos(2);
            GameObject expected = SpawnGOWithXPos(3);
            yield return null;
            // lock on to closest
            lockOn.NextToLockOnTo();
            // furthest leaves 
            Object.Destroy(GameObjectToLeave.gameObject);
            yield return null;
            // should lock on to the furthest
            GameObject result = lockOn.NextToLockOnTo();
            Assert.AreSame(expected, result);
        }
    }
}
