using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using NSubstitute;
using UnityEngine.TestTools;

namespace Tests
{
    public class PressExecutionTest
    {
        [SetUp]
        public void SetUp()
        {
            PressInstruction.instance.reset();
            IUnityService service = Substitute.For<IUnityService>();
            service.GetKeyDown("return").Returns(true);
            PressInstruction.instance.service = service;
        }

        // public abstract void OnStart();
        // instruction notnull
        [Test]
        public void InstructionNotNullTest()
        {
            PressExecutableSO press = ScriptableObject.CreateInstance<PressExecutableSO>();
            ExecutionEvent executionEvent = Substitute.For<ExecutionEvent>();
            press.Construct(null, executionEvent);
            press.OnStart();

            Assert.NotNull(press.instruction);

        }

        // event non null
        [Test]
        public void ExecutionEventNotNullTest()
        {
            PressExecutableSO press = ScriptableObject.CreateInstance<PressExecutableSO>();
            PressInstruction instruction = PressInstruction.instance;

            Assert.Throws<System.ArgumentException>(delegate
            {
                press.OnStart();
            });
        }

        // Is triggered is false
        // is not in cancel time
        // is not finished
        [Test]
        public void StartValuesAreCorrectTest()
        {
            PressExecutableSO press = ScriptableObject.CreateInstance<PressExecutableSO>();
            Assert.False(press.IsTriggered());
            Assert.False(press.IsInCancelTime());
            Assert.False(press.IsFinished());
        }
        

        //public abstract void OnInput(string input, IBattler battler, ITargetSet targets);
        // test key up does nothing
        [Test]
        public void KeyUpDoesNothingTest()
        {
            PressExecutableSO press = ScriptableObject.CreateInstance<PressExecutableSO>();
            PressInstruction instruction = PressInstruction.instance;
            PressInstruction.instance.service.GetKeyDown("return").Returns(false);
            PressInstruction.instance.service.GetKeyUp("return").Returns(true);
            ExecutionEvent executionEvent = Substitute.For<ExecutionEvent>();
            IBattler battler = Substitute.For<IBattler>();
            ITargetSet targets = Substitute.For<ITargetSet>();

            press.Construct(instruction, executionEvent);

            press.OnInput("return", battler, targets);

            Assert.IsFalse(press.IsTriggered());
        }


        // held key does nothing
        [Test]
        public void KeyHeldNothingTest()
        {
            PressExecutableSO press = ScriptableObject.CreateInstance<PressExecutableSO>();
            PressInstruction instruction = PressInstruction.instance;
            PressInstruction.instance.service.GetKeyDown("return").Returns(false);
            PressInstruction.instance.service.GetKey("return").Returns(true);
            ExecutionEvent executionEvent = Substitute.For<ExecutionEvent>();
            IBattler battler = Substitute.For<IBattler>();
            ITargetSet targets = Substitute.For<ITargetSet>();

            press.Construct(instruction, executionEvent);

            press.OnInput("return", battler, targets);

            Assert.IsFalse(press.IsTriggered());
        }

        // test key down set is triggerd to true
        [Test]
        public void KeyDownTriggersTest()
        {
            PressExecutableSO press = ScriptableObject.CreateInstance<PressExecutableSO>();
            PressInstruction instruction = PressInstruction.instance;
            ExecutionEvent executionEvent = Substitute.For<ExecutionEvent>();
            IBattler battler = Substitute.For<IBattler>();
            ITargetSet targets = Substitute.For<ITargetSet>();

            press.Construct(instruction, executionEvent);

            press.OnInput("return", battler, targets);

            Assert.IsTrue(press.IsTriggered());
        }

        // test keydown executes event
        [Test]
        public void KeyDownTriggersEventTest()
        {
            PressExecutableSO press = ScriptableObject.CreateInstance<PressExecutableSO>();
            PressInstruction instruction = PressInstruction.instance;
            TestExecutionEvent executionEvent = ScriptableObject.CreateInstance<TestExecutionEvent>();
            IBattler battler = Substitute.For<IBattler>();
            ITargetSet targets = Substitute.For<ITargetSet>();
            // triggers event
            press.Construct(instruction, executionEvent);
            press.OnInput("return", battler, targets);

            Assert.IsTrue(executionEvent.timesExecuted == 1);
        }

        // test cancellable when event cancellable fires
        [Test]
        public void CancellableWhenCancelFiresTest()
        {
            PressExecutableSO press = ScriptableObject.CreateInstance<PressExecutableSO>();
            PressInstruction instruction = PressInstruction.instance;
            CancelTestExecutionEvent executionEvent = ScriptableObject.CreateInstance<CancelTestExecutionEvent>();
            IBattler battler = Substitute.For<IBattler>();
            ITargetSet targets = Substitute.For<ITargetSet>();
            press.Construct(instruction, executionEvent);

            // cancel event fires immediately with this event
            press.OnInput("return", battler, targets);

            Assert.IsTrue(press.IsInCancelTime());
        }

        // test finished when event finished fires
        [Test]
        public void FinishedWhenFinishEventFiresTest()
        {

            PressExecutableSO press = ScriptableObject.CreateInstance<PressExecutableSO>();
            PressInstruction instruction = PressInstruction.instance;
            FinishTestExecutionEvent executionEvent = ScriptableObject.CreateInstance<FinishTestExecutionEvent>();
            IBattler battler = Substitute.For<IBattler>();
            ITargetSet targets = Substitute.For<ITargetSet>();

            press.Construct(instruction, executionEvent);
            press.OnInput("return", battler, targets);

            Assert.IsTrue(press.IsFinished());
        }

        // test keydown while triggered not in canceltime does not fire event twice
        [Test]
        public void KeyDownWhileTriggeredDoesNotFireExecutionEventTest()
        {
            PressExecutableSO press = ScriptableObject.CreateInstance<PressExecutableSO>();
            PressInstruction instruction = PressInstruction.instance;
            TestExecutionEvent executionEvent = ScriptableObject.CreateInstance<TestExecutionEvent>();
            IBattler battler = Substitute.For<IBattler>();
            ITargetSet targets = Substitute.For<ITargetSet>();

            press.Construct(instruction, executionEvent);

            // triggers event
            // this should keep cancel time false and set istriggered to true with the given execution event
            press.OnInput("return", battler, targets);
            Assert.IsTrue(press.IsTriggered());
            Assert.IsFalse(press.IsInCancelTime());

            // calling it again with keydown should not trigger event again
            press.OnInput("return", battler, targets);
            Assert.AreEqual(1, executionEvent.timesExecuted);
        }


        // test keydown while triggered not in cancel time sets finished
        [Test]
        public void KeyDownWhileNotInCancelTimeSetsFinishedTest()
        {
            PressExecutableSO press = ScriptableObject.CreateInstance<PressExecutableSO>();
            PressInstruction instruction = PressInstruction.instance;
            TestExecutionEvent executionEvent = ScriptableObject.CreateInstance<TestExecutionEvent>();
            IBattler battler = Substitute.For<IBattler>();
            ITargetSet targets = Substitute.For<ITargetSet>();

            press.Construct(instruction, executionEvent);
            // set to triggered
            press.OnInput("return", battler, targets);
            Assert.IsTrue(press.IsTriggered());
            Assert.IsFalse(press.IsInCancelTime());
            // should set to finished
            press.OnInput("return", battler, targets);
            Assert.IsTrue(press.IsFinished());
        }
    }
}
