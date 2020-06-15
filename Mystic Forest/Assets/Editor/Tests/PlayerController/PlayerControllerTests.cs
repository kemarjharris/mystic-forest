using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using NSubstitute;

namespace Tests
{
    public abstract class PlayerControllerTest
    {
        protected JointController controller;
        protected GameObject go;

        public IEnumerator WaitUntilGrounded()
        {
            IBattlerPhysics p = go.GetComponent<IBattlerPhysics>();
            Vector3 oldPos;
            do
            {
                oldPos = go.transform.position;
                yield return new WaitForFixedUpdate();

            } while (!p.IsGrounded && oldPos != go.transform.position);
        }

        public IEnumerator Move(IPlayerController controller, float horizontal, float vertical, System.Action<Vector3> assert)
        {
            float seconds = 1;
            SetDirectionalService(horizontal, vertical);
            do
            {
                Vector3 start = go.transform.position;
                controller.Update();
                controller.FixedUpdate();
                yield return new WaitForFixedUpdate();
                seconds -= Time.deltaTime;
                assert(start);

            } while (seconds > 0);
        }

        public virtual IEnumerator LoadScene()
        {
            yield return null;
            UnityEngine.SceneManagement.SceneManager.LoadScene("2.5D Test Scene");
            yield return null;
            controller = Object.FindObjectOfType<JointController>();
            controller.enabled = false;
            go = controller.gameObject;
            yield return WaitUntilGrounded();
        }

        public abstract void SetDirectionalService(float horizontal, float vertical);
    }

    public class NeutralControllerTest : PlayerControllerTest
    {
        JointController neutral;
        IUnityAxisService service;
        

        public override IEnumerator LoadScene()
        {
            yield return base.LoadScene();
            neutral = controller;
        }

        public override void SetDirectionalService(float horizontal, float vertical)
        {
           
            IUnityAxisService service = Substitute.For<IUnityAxisService>();
            service.GetAxis("Horizontal").Returns(horizontal);
            service.GetAxis("Vertical").Returns(vertical);
            neutral.service = service;
        }

        [UnityTest]
        public IEnumerator Move_LeftInput_MovesLeft()
        {
            yield return LoadScene();
            yield return Move(neutral, -1, 0, (pos) => Assert.Greater(pos.x, go.transform.position.x));
        }

        [UnityTest]
        public IEnumerator Move_RightInput_MovesRight()
        {
            yield return LoadScene();
            yield return Move(neutral, 1, 0, (pos) => Assert.Less(pos.x, go.transform.position.x));
        }

        [UnityTest]
        public IEnumerator Move_UpInput_MovesUp()
        {
            yield return LoadScene();
            yield return Move(neutral, 0, 1, (pos) => Assert.Less(pos.z, go.transform.position.z));
        }

        [UnityTest]
        public IEnumerator Move_DownInput_MovesDown()
        {
            yield return LoadScene();
            yield return Move(neutral, 0, -1, (pos) => Assert.Greater(pos.z, go.transform.position.z));
        }
    }

    public class CombatControllerTest : PlayerControllerTest
    {
        JointController combat;
        IUnityAxisService service;
        IExecutionModule module;


        public override IEnumerator LoadScene()
        {
            yield return base.LoadScene();
            combat = controller;
        }

        public override void SetDirectionalService(float horizontal, float vertical)
        {
            IUnityAxisService service = Substitute.For<IUnityAxisService>();
            service.GetAxis("Horizontal").Returns(horizontal);
            service.GetAxis("Vertical").Returns(vertical);
            combat.service = service;
        }

        [UnityTest]
        public IEnumerator Move_LeftInput_MovesLeft()
        {

            yield return LoadScene();
            yield return WaitUntilGrounded();
            yield return Move(combat, -1, 0, (pos) => Assert.Greater(pos.x, go.transform.position.x));
        }

        [UnityTest]
        public IEnumerator Move_RightInput_MovesRight()
        {

            yield return LoadScene();
            yield return WaitUntilGrounded();
            yield return Move(combat, 1, 0, (pos) => Assert.Less(pos.x, go.transform.position.x));
        }

        // jumping vertical increases Y and not X
        [UnityTest]
        public IEnumerator Jump_PositiveVerticalInput_IncreasesYPosition()
        {
            yield return LoadScene();
            SetDirectionalService(0, 1);
            Vector3 start = go.transform.position;
            combat.Update();
            combat.FixedUpdate();
            Assert.Less(start.y, go.transform.position.y);
        }

        // jump up forward increases X and Y
        [UnityTest]
        public IEnumerator Jump_PositiveVerticalAndHorizontalInput_JumpsForwards()
        {
            yield return LoadScene();
            SetDirectionalService(1, 1);
            Vector3 start = go.transform.position;
            float seconds = 0.5f;
            do
            {
                seconds -= Time.deltaTime;
                combat.Update();
                combat.FixedUpdate();
                yield return null;
            } while (seconds > 0);
            Assert.Less(start.x, go.transform.position.x);
            Assert.Less(start.y, go.transform.position.y);
        }

        [UnityTest]
        public IEnumerator Jump_PositiveVerticalNegativeHorizontal_JumpsBackward()
        {
            yield return LoadScene();
            SetDirectionalService(-1, 1);
            Vector3 start = go.transform.position;
            float seconds = 0.5f;
            do
            {
                seconds -= Time.deltaTime;
                combat.Update();
                combat.FixedUpdate();
                yield return null;
            } while (seconds > 0);
            Assert.Greater(start.x, go.transform.position.x);
            Assert.Less(start.y, go.transform.position.y);
        }

        [UnityTest]
        public IEnumerator Jump_Airborne_DoesNotJumpAgain()
        {
            yield return LoadScene();
            SetDirectionalService(0, 1);
            Vector3 pos = go.transform.position;
            combat.Update();
            combat.FixedUpdate();
            yield return null;
            SetDirectionalService(0, 0);
            // loop until falling
            do
            {
                pos = go.transform.position;
                combat.Update();
                combat.FixedUpdate();
                yield return null;
            } while (pos.y <= go.transform.position.y);
            // upwards input again
            pos = go.transform.position;
            yield return null;
            combat.Update();
            combat.FixedUpdate();
            yield return null;
            Assert.Greater(pos.y, go.transform.position.y);
        }

        [UnityTest]
        public IEnumerator Jump_HorizontalInput_DoesNotChangeMovement()
        {
            
            yield return LoadScene();
            // jump forward
            SetDirectionalService(1, 1);
            combat.Update();
            combat.FixedUpdate();
            Vector3 pos = go.transform.position;
            // input backwards next frame
            SetDirectionalService(-1, 0);
            // no response
            float seconds = 0.5f;
            do
            {
                seconds -= Time.deltaTime;
                pos = go.transform.position;
                yield return null;
                combat.Update();
                combat.FixedUpdate();
                Assert.LessOrEqual(pos.x, go.transform.position.x);
            } while (seconds > 0);
        }

        [UnityTest]
        public IEnumerator Jump_Horizontally_MaintainsVelocity()
        {
            yield return LoadScene();
            // jump forward
            SetDirectionalService(1, 1);
            combat.Update();
            combat.FixedUpdate();
            Vector3 pos = go.transform.position;
            // input backwards next frame
            SetDirectionalService(0, 0);
            float lastdistance = float.NegativeInfinity;
            // no response
            float seconds = 1;
            do
            {
                seconds -= Time.deltaTime;
                pos = go.transform.position;
                yield return null;
                combat.Update();
                combat.FixedUpdate();

                float distance = Mathf.Abs(go.transform.position.x - pos.x);
                Assert.That(lastdistance <= distance + 0.5f);
                lastdistance = distance;
            } while (seconds > 0);
        }

        public IEnumerator Attack_NoMoving(System.Action setState, float horizontal, float vertical)
        {
            yield return LoadScene();
            yield return WaitUntilGrounded();
            setState();
            SetDirectionalService(horizontal, vertical);
            Vector3 pos = go.transform.position;
            yield return null;
            combat.Update();
            combat.FixedUpdate();
            Assert.Less(0.002f, Vector3.Distance(pos, go.transform.position));
        }

        // no horizontal movement
        [UnityTest]
        public IEnumerator Attack_Attacking_NoForwardMovement()
        {
            yield return Attack_NoMoving(() => combat.SetStateAttacking(), 1, 0);
        }

        [UnityTest]
        public IEnumerator Attack_Attacking_NoBackwardMovement()
        {
            yield return Attack_NoMoving(() => combat.SetStateAttacking(), -1, 0);
        }

        // no jumping
        [UnityTest]
        public IEnumerator Attack_Attacking_NoJumping()
        {
            yield return Attack_NoMoving(() => combat.SetStateAttacking(), 0, 1);
        }

        // STATE able to cancel attack
        // no horizontal movement
        [UnityTest]
        public IEnumerator Attack_AbleToCancelAttack_NoPositiveHorizontalMovement()
        {
            yield return Attack_NoMoving(() => combat.SetStateAbleToCancelAttack(), 1, 0);
        }

        [UnityTest]
        public IEnumerator Attack_AbleToCancelAttack_NoNegativeHorizontalMovement()
        {
            yield return Attack_NoMoving(() => combat.SetStateAbleToCancelAttack(), -1, 0);
        }

        // able to jump
        [UnityTest]
        public IEnumerator Attack_AbleToCancelAttack_AbleToJump()
        {
            yield return LoadScene();
            yield return WaitUntilGrounded();
            combat.SetStateAbleToCancelAttack();
            SetDirectionalService(0, 1);
            Vector3 start = go.transform.position;
            yield return null;
            combat.Update();
            combat.FixedUpdate();
            yield return null;
            Assert.Less(start.y, go.transform.position.y);
        }
    }
}
