using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    public class CursorTest
    {

        Cursor cursor = null;
        Vector3 start;

        [SetUp]
        public void SetUp()
        {
            cursor = new GameObject().AddComponent<Cursor>();
            start = cursor.transform.position;
        }

        [TearDown]
        public void TearDown()
        {
            Object.Destroy(cursor.gameObject);
        }

        public IEnumerator WaitHalfASecond()
        {
            yield return new WaitForSeconds(1.5f);
        }

        public void CheckMovedUp() => Assert.Less(start.z, cursor.transform.position.z);
        public void CheckMovedDown() => Assert.Greater(start.z, cursor.transform.position.z);
        public void CheckMovedLeft() => Assert.Greater(start.x, cursor.transform.position.x);
        public void CheckMovedRight() => Assert.Less(start.x, cursor.transform.position.x);

        // no input does not move
        [UnityTest]
        public IEnumerator NoInputDoesNotMoveTest()
        {
            // Use yield to skip a frame.
            yield return WaitHalfASecond();
            Assert.AreEqual(start, cursor.transform.position);
        }

        // Moves up
        [UnityTest]
        public IEnumerator MoveUpTest()
        {
            cursor.Up();
            // Use yield to skip a frame.
            yield return WaitHalfASecond();
            CheckMovedUp();
        }

        // moves down
        [UnityTest]
        public IEnumerator MoveDownTest()
        {
            cursor.Down();
            // Use yield to skip a frame.
            yield return WaitHalfASecond();
            CheckMovedDown();
        }

        // moves left
        [UnityTest]
        public IEnumerator MoveLeftTest()
        {
            cursor.Left();
            // Use yield to skip a frame.
            yield return WaitHalfASecond();
            CheckMovedLeft();
        }

        // moves right
        [UnityTest]
        public IEnumerator MoveRightTest()
        {
            cursor.Right();
            // Use yield to skip a frame.
            yield return WaitHalfASecond();
            CheckMovedRight();
        }

        // up down does not move
        [UnityTest]
        public IEnumerator UpDownNoMovementTest()
        {
            cursor.Up();
            cursor.Down();

            // Use yield to skip a frame.
            yield return WaitHalfASecond();
            Assert.AreEqual(start, cursor.transform.position);
        }

        // left right does not move
        [UnityTest]
        public IEnumerator LeftRightNoMovementTest()
        {
            cursor.Left();
            cursor.Right();
            // Use yield to skip a frame.
            yield return WaitHalfASecond();
            Assert.AreEqual(start, cursor.transform.position);
        }

        // up right moves up right
        [UnityTest]
        public IEnumerator UpRightMovementTest()
        {
            cursor.Up();
            cursor.Right();
            // Use yield to skip a frame.
            yield return WaitHalfASecond();
            CheckMovedUp();
            CheckMovedRight();
        }

        // down right moves down right
        [UnityTest]
        public IEnumerator DownRightMovementTest()
        {
            cursor.Down();
            cursor.Right();
            // Use yield to skip a frame.
            yield return WaitHalfASecond();
            CheckMovedDown();
            CheckMovedRight();
        }

        // up left moves up left
        [UnityTest]
        public IEnumerator UpLeftMovementTest()
        {
            cursor.Up();
            cursor.Left();
            // Use yield to skip a frame.
            yield return WaitHalfASecond();
            CheckMovedUp();
            CheckMovedLeft();
        }

        // down left moves down left
        [UnityTest]
        public IEnumerator DownLeftMovementTest()
        {
            cursor.Down();
            cursor.Left();
            // Use yield to skip a frame.
            yield return WaitHalfASecond();
            CheckMovedDown();
            CheckMovedLeft();
        }
    }
}
