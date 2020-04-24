using System.Collections;
using UnityEngine.TestTools;
using NUnit.Framework;
using NSubstitute;

namespace Tests
{
    public class PressInstructionTests
    {

        // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
        // `yield return null;` to skip a frame.
        [UnityTest]
        public IEnumerator KeyDownGivesKeyDownEventTest()
        {

            PressInstruction press = new PressInstruction();
            IUnityService service = Substitute.For<IUnityService>();
            service.GetKeyDown("return").Returns(true);
            press.service = service;
            
            // Use the Assert class to test conditions.
            // Use yield to skip a frame.
            yield return null;

            Assert.AreEqual(InstructionKeyEvent.KEYDOWN, press.lookAtTime("return"));
        }

        [Test]
        public void NoKeyDownGivesNothingTest()
        {
            PressInstruction press = new PressInstruction();
            Assert.AreEqual(InstructionKeyEvent.NOKEY, press.lookAtTime("return"));
        }

        [Test]
        public void InvalidInputGivesNothingTest()
        {
            PressInstruction press = new PressInstruction();
            Assert.AreEqual(InstructionKeyEvent.NOKEY, press.lookAtTime(""));
        }

        [UnityTest]
        public IEnumerator KeyUpGivesNothingTest()
        {
            PressInstruction press = new PressInstruction();
            IUnityService service = Substitute.For<IUnityService>();
            service.GetKeyUp("return").Returns(true);
            press.service = service;

            // Use the Assert class to test conditions.
            // Use yield to skip a frame.
            yield return null;

            Assert.AreEqual(InstructionKeyEvent.NOKEY, press.lookAtTime("return"));
        }

    }
}
