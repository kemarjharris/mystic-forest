using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using NSubstitute;

namespace Tests
{

    public class NeutralControllerTest
    {

        NeutralController neutral;
        IUnityAxisService service;
        GameObject go;

        public IEnumerator LoadScene()
        {
            yield return null;
            UnityEngine.SceneManagement.SceneManager.LoadScene("Combat Test Scene");
            yield return null;
            PlayerController controller = Object.FindObjectOfType<PlayerController>();

            go = controller.gameObject;
            neutral = (NeutralController)controller.neutral;
            yield return null;
        }

        public void SetDirectionalService(float horizontal, float vertical)
        {
           
            IUnityAxisService service = Substitute.For<IUnityAxisService>();
            service.GetAxis("Horizontal").Returns(horizontal);
            service.GetAxis("Vertical").Returns(vertical);
            neutral.service = service;
        }

        public IEnumerator WaitUntilGrounded()
        {
            IBattlerPhysicsZ p = go.GetComponent<IBattlerPhysicsZ>();
            Vector3 oldPos;
            do
            {
                oldPos = go.transform.position;
                yield return new WaitForFixedUpdate();

            } while (!p.IsGrounded && oldPos != go.transform.position);
        }

        IEnumerator Move(float horizontal, float vertical, System.Action<Vector3> assert)
        {
            yield return LoadScene();
            yield return WaitUntilGrounded();
            float seconds = 2;
            SetDirectionalService(horizontal, vertical);
            do
            {
                Vector3 start = go.transform.position;
                neutral.Update();
                neutral.FixedUpdate();
                yield return new WaitForFixedUpdate();
                seconds -= Time.deltaTime;
                assert(start);

            } while (seconds > 0);
        }


        [UnityTest]
        public IEnumerator Move_LeftInput_MovesLeft()
        {
            yield return Move(-1, 0, (pos) => Assert.Greater(pos.x, go.transform.position.x));
        }

        [UnityTest]
        public IEnumerator Move_RightInput_MovesRight()
        {
            yield return Move(1, 0, (pos) => Assert.Less(pos.x, go.transform.position.x));
        }

        [UnityTest]
        public IEnumerator Move_UpInput_MovesUp()
        {
            yield return Move(0, 1, delegate(Vector3 pos) {
                    Assert.Less(pos.y, go.transform.position.y);
                    Assert.Less(pos.z, go.transform.position.z);
                }
            );
        }

    }
}
