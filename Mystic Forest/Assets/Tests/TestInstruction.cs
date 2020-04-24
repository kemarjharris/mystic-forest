using System.Collections;
using UnityEngine.TestTools;
using NUnit.Framework;
using NSubstitute;

namespace Tests
{
    public class MashInstructionTests
    {
        [SetUp]
        public void SetUp()
        {
            MashInstruction.instance.reset();
        }

        // bad input gives no key
        [Test]
        public void BadInputGivesNoKeyTest()
        {
            MashInstruction mash = MashInstruction.instance;
            Assert.AreEqual(InstructionKeyEvent.NOKEY, mash.lookAtTime("", 0, 2));
        }

        // negative time pressed gives noKey
        [Test]
        public void NegativeTimeGivesNoKeyTest()
        {
            MashInstruction mash = MashInstruction.instance;
            Assert.AreEqual(InstructionKeyEvent.NOKEY, mash.lookAtTime("return", -1, 2));
        }

        // nothing pressed gives no key
        [Test]
        public void NoInputGivesNoKeyTest()
        {
            MashInstruction mash = MashInstruction.instance;
            Assert.AreEqual(InstructionKeyEvent.NOKEY, mash.lookAtTime("return", 0, 2));
        }

        // Key down in accepted times gives keydown
        [UnityTest]
        public IEnumerator AcceptedTimeGivesKeyDownTest()
        {
            MashInstruction mash = MashInstruction.instance;
            IUnityService service = Substitute.For<IUnityService>();
            service.GetKeyDown("return").Returns(true);
            mash.service = service;

            yield return null;

            Assert.AreEqual(InstructionKeyEvent.KEYDOWN, mash.lookAtTime("return", 1, 2));
        }

        // key down outside of accepted time gives badkey
        [UnityTest]
        public IEnumerator KeyDownOutsideOfAcceptedTimeGivesBadKeyTest()
        {
            MashInstruction mash = MashInstruction.instance;
            IUnityService service = Substitute.For<IUnityService>();
            service.GetKeyDown("return").Returns(true);
            mash.service = service;

            yield return null;

            Assert.AreEqual(InstructionKeyEvent.BADKEY, mash.lookAtTime("return", 2.5f, 2));
        }


        // key up with key down in accepted time gives key up
        [UnityTest]
        public IEnumerator AcceptedTimeGivesKeyUpTest()
        {
            MashInstruction mash = MashInstruction.instance;
            IUnityService service = Substitute.For<IUnityService>();
            mash.service = service;
            service.GetKeyDown("return").Returns(true);
            mash.lookAtTime("return", 1, 2);
            yield return null;
            service.GetKeyDown("return").Returns(false);
            service.GetKeyUp("return").Returns(true);
            yield return null;

            Assert.AreEqual(InstructionKeyEvent.KEYUP, mash.lookAtTime("return", 1, 2));
        }

        // no key down but key up gives no key
        [UnityTest]
        public IEnumerator NoKeyDownButKeyUpGivesNoKeyTest()
        {
            MashInstruction mash = MashInstruction.instance;
            IUnityService service = Substitute.For<IUnityService>();
            service.GetKeyUp("return").Returns(true);
            mash.service = service;

            yield return null;

            Assert.AreEqual(InstructionKeyEvent.NOKEY, mash.lookAtTime("return", 1, 2));
        }

        // key up outside of accepted time gives badkey
        [UnityTest]
        public IEnumerator KeyUpOutsideOfAcceptedTimeGivesBadKeyTest()
        {
            MashInstruction mash = MashInstruction.instance;
            IUnityService service = Substitute.For<IUnityService>();
            service.GetKeyUp("return").Returns(true);
            mash.service = service;

            yield return null;

            Assert.AreEqual(InstructionKeyEvent.BADKEY, mash.lookAtTime("return", 2.5f, 2));
        }

        // no button pressed gives bad key
        [UnityTest]
        public IEnumerator NoButtonPressedGivesBadKeyTest()
        {
            MashInstruction mash = MashInstruction.instance;
            IUnityService service = Substitute.For<IUnityService>();
            service.GetKeyDown("return").Returns(false);
            mash.service = service;

            yield return null;

            Assert.AreEqual(InstructionKeyEvent.BADKEY, mash.lookAtTime("return", 2.5f, 2));
        }
    }

    public class PressInstructionTests
    {

        [SetUp]
        public void SetUp()
        {
            PressInstruction.instance.reset();
        }

        // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
        // `yield return null;` to skip a frame.
        [UnityTest]
        public IEnumerator KeyDownGivesKeyDownEventTest()
        {

            PressInstruction press = PressInstruction.instance;
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
            PressInstruction press = PressInstruction.instance;
            Assert.AreEqual(InstructionKeyEvent.NOKEY, press.lookAtTime("return"));
        }

        [Test]
        public void InvalidInputGivesNothingTest()
        {
            PressInstruction press = PressInstruction.instance;
            Assert.AreEqual(InstructionKeyEvent.NOKEY, press.lookAtTime(""));
        }

        [UnityTest]
        public IEnumerator KeyUpGivesNothingTest()
        {
            PressInstruction press = PressInstruction.instance;
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
