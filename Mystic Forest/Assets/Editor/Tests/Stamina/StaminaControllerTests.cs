using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    public abstract class StaminaControllerTests
    {
        protected IStaminaController controller;
        protected float restorationPerSecond = 1;

        // max on start
        [UnityTest]
        public virtual IEnumerator Start_Stamina_MaxOnStart()
        {
            yield return null;
            Assert.AreEqual(controller.stamina, controller.maxStamina);
        }

        // does not decrease past 0
        [Test]
        public void DecreaseStamina_DecreasedByMoreThanCurrent_DoesNotFallBelowZero()
        {
            controller.DecreaseStamina(controller.maxStamina + 1);
            Assert.AreEqual(controller.stamina, 0);
        }

        // negative decrease does not decrease stamina
        [Test]
        public void DecreaseStamina_DecreasedByNegativeValue_DoesNotIncreaseStamina()
        {
            controller.DecreaseStamina(1);
            float currStamina = controller.stamina;
            controller.DecreaseStamina(-1);
            Assert.AreEqual(currStamina, controller.stamina);
        }

        [UnityTest]
        public IEnumerator Update_Restoring_RestoresMana()
        {
            controller.DecreaseStamina(controller.maxStamina);
            // Assert no stamina to start
            Assert.AreEqual(controller.stamina, 0);
            // restoring so value should increase
            yield return null;
            Assert.Greater(controller.stamina, 0);
        }

        // Max stamina nonzero
        [UnityTest]
        public IEnumerator Start_MaxValue_Nonzero()
        {
            yield return null;
            Assert.Greater(controller.stamina, 0);
        }

        // stop restoring does not restore
        [UnityTest]
        public IEnumerator StopRestoring_StopsResoringStamina()
        {
            controller.DecreaseStamina(controller.maxStamina);
            // Assert no stamina to start
            Assert.AreEqual(controller.stamina, 0);
            controller.StopRestoring();
            yield return null;
            Assert.AreEqual(controller.stamina, 0);
        }

        // Does not restore past max
        [UnityTest]
        public IEnumerator Update_Restoring_DoesNotRestorePastUpperBound()
        {
            controller.DecreaseStamina(controller.maxStamina);
            // stamoina at 0
            float timeUntilFull = controller.maxStamina / restorationPerSecond;
            yield return new WaitForSecondsRealtime(timeUntilFull + 0.1f);
            Assert.AreEqual(controller.stamina, controller.maxStamina);
        }


    }

    public class StaminaControllerImplementationTest : StaminaControllerTests
    {
        GameObject go;
        readonly float min = 0;
        readonly float max = 1;
        readonly float start = 0; 

        [SetUp]
        public void SetUp()
        {
            go = new GameObject();
            StaminaController controller = go.AddComponent<StaminaController>();

            controller.stamina = new BoundedFloat(start, min, max);
            controller.restorationPerSecond = restorationPerSecond;
            controller.Start();
            this.controller = controller;
        }

        [TearDown]
        public void TearDown()
        {
            Object.Destroy(go);
        }
    }


}
