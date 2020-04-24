using System.Collections;
using UnityEngine.TestTools;
using NUnit.Framework;
using NSubstitute;

namespace Tests
{

    public class HoldInstuctionTest
    {
        [SetUp]
        public void Setup()
        {
            HoldInstruction.instance.reset();
        }

        // negative time
        [Test]
        public void NegativeInputTimeNoKeyTest()
        {
            Assert.AreEqual(InstructionKeyEvent.NOKEY, HoldInstruction.instance.lookAtTime("return", -1, 2));
        }

        // bad string input gives no key
        [Test]
        public void BadStringGivesNoKeyTest()
        {
            Assert.AreEqual(InstructionKeyEvent.NOKEY, HoldInstruction.instance.lookAtTime("", 0, 2));
        }

        // Negative release time throws value error
        [Test]
        public void NegativeReleaseTimeArgumentExceptionTest()
        {
            Assert.Throws<System.ArgumentException>(delegate
            {
                HoldInstruction.instance.lookAtTime("return", 0, -1);
            });
        }

        // key down at start gives key down
        [UnityTest]
        public IEnumerator KeyDownAtStartGivesKeyDownTest()
        {
            HoldInstruction hold = HoldInstruction.instance;
            IUnityService service = Substitute.For<IUnityService>();
            service.GetKeyDown("return").Returns(true);
            hold.service = service;

            yield return null;

            Assert.AreEqual(InstructionKeyEvent.KEYDOWN, HoldInstruction.instance.lookAtTime("return", 0, 2));

        }

        // key down outside of accepted range gives bad key
        [UnityTest]
        public IEnumerator KeyDownOutsideOfAcceptedRangeGivesBadKeyTest()
        {
            HoldInstruction hold = HoldInstruction.instance;
            IUnityService service = Substitute.For<IUnityService>();
            service.GetKeyDown("return").Returns(true);
            hold.service = service;

            yield return null;

            Assert.AreEqual(InstructionKeyEvent.BADKEY, HoldInstruction.instance.lookAtTime("return", 2.5f, 2));

        }

        // no key down gives badkey
        [Test]
        public void NoKeyDownOutsideOfRangeGivesBadKeyTest()
        {
            Assert.AreEqual(InstructionKeyEvent.BADKEY, HoldInstruction.instance.lookAtTime("return", 2.5f, 2));
        }

        // no key down key held gives noKey
        [Test]
        public void NoKeyDownKeyHeldGivesNoKeyTest()
        {
            IUnityService service = Substitute.For<IUnityService>();
            service.GetKeyDown("return").Returns(false);
            service.GetKey("return").Returns(true);
            HoldInstruction.instance.service = service;
            Assert.AreEqual(InstructionKeyEvent.NOKEY, HoldInstruction.instance.lookAtTime("return", 1f, 2));
        }

        // get point at random time gives key held
        [UnityTest]
        public IEnumerator KeyHeldAfterKeyDownGivesKeyHeldTest()
        {
            IUnityService service = Substitute.For<IUnityService>();
            HoldInstruction.instance.service = service;

            // user presses key
            service.GetKeyDown("return").Returns(true);
            service.GetKey("return").Returns(true);

            HoldInstruction.instance.lookAtTime("return", 0, 2);
            yield return null;

            //next frame, key is still down
            service.GetKeyDown("return").Returns(false);

            Assert.AreEqual(InstructionKeyEvent.KEYHELD, HoldInstruction.instance.lookAtTime("return", 0.1f, 2));
        }

        // key held outside of accepted range gives bad key
        [UnityTest]
        public IEnumerator KeyHeldOutsideOfRangegivesBadKey()
        {
            IUnityService service = Substitute.For<IUnityService>();
            HoldInstruction.instance.service = service;

            // user presses key
            service.GetKeyDown("return").Returns(true);
            service.GetKey("return").Returns(true);

            HoldInstruction.instance.lookAtTime("return", 0, 2);
            yield return null;

            //next frame, key is still down
            service.GetKeyDown("return").Returns(false);

            Assert.AreEqual(InstructionKeyEvent.BADKEY, HoldInstruction.instance.lookAtTime("return", 2.5f, 2));
        }

        // key down then key up in accepted range gives key up
        [UnityTest]
        public IEnumerator KeyUpInRangeGivesKeyUpTest()
        {

            IUnityService service = Substitute.For<IUnityService>();
            HoldInstruction.instance.service = service;

            // user presses key
            service.GetKeyDown("return").Returns(true);

            HoldInstruction.instance.lookAtTime("return", 0, 2);
            yield return null;

            //next frame, key is still down
            service.GetKeyDown("return").Returns(false);
            service.GetKeyUp("return").Returns(true);

            Assert.AreEqual(InstructionKeyEvent.KEYUP, HoldInstruction.instance.lookAtTime("return", 2f, 2));
        }


        // key up without key down gives no key 
        [Test]
        public void KeyUpNoKeyDownGivesNoKeyTest()
        {
            IUnityService service = Substitute.For<IUnityService>();
            HoldInstruction.instance.service = service;
            service.GetKeyUp("return").Returns(true);
            Assert.AreEqual(InstructionKeyEvent.NOKEY, HoldInstruction.instance.lookAtTime("return", 2, 2));
        }

        // key up outside of accepted range gives bad key
        [UnityTest]
        public IEnumerator KeyUpOutsideOfRangeGivesBadKeyTest()
        {
            IUnityService service = Substitute.For<IUnityService>();
            HoldInstruction.instance.service = service;

            // user presses key
            service.GetKeyDown("return").Returns(true);

            HoldInstruction.instance.lookAtTime("return", 0, 2);
            yield return null;

            //next frame, key up outside of range
            service.GetKeyDown("return").Returns(false);
            service.GetKeyUp("return").Returns(true);

            Assert.AreEqual(InstructionKeyEvent.BADKEY, HoldInstruction.instance.lookAtTime("return", 2.5f, 2));
        }

    }

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

        // Negative release time throws value error
        public void NegativeEndTimeValueErrorTest()
        {
            Assert.Throws<System.ArgumentException>(delegate
            {
                MashInstruction.instance.lookAtTime("return", 0, -1);
            });
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
        [Test]
        public void KeyDownGivesKeyDownEventTest()
        {

            PressInstruction press = PressInstruction.instance;
            IUnityService service = Substitute.For<IUnityService>();
            service.GetKeyDown("return").Returns(true);
            press.service = service;
            
            // Use the Assert class to test conditions.
            // Use yield to skip a frame.
            //yield return null;

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

        [Test]
        public void KeyUpGivesNothingTest()
        {
            PressInstruction press = PressInstruction.instance;
            IUnityService service = Substitute.For<IUnityService>();
            service.GetKeyUp("return").Returns(true);
            press.service = service;

            Assert.AreEqual(InstructionKeyEvent.NOKEY, press.lookAtTime("return"));
        }

    }
}
