using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using NSubstitute;
using UnityEngine.TestTools;

namespace ExecutableTest
{
    public class KeyDownMashExecutableTest
    {
        KeyDownMashExecutableSO mash;
        
        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            mash = ScriptableObject.CreateInstance<KeyDownMashExecutableSO>();
        }

        [SetUp]
        public void SetUp()
        {
            mash.Construct(
               MashInstruction.instance,
               ScriptableObject.CreateInstance<TestExecutionEvent>(), // key down event
               ScriptableObject.CreateInstance<TestExecutionEvent>(), //  mash finished event
               2); // mash duration 
            IUnityTimeService service = Substitute.For<IUnityTimeService>();
            service.unscaledTime.Returns(0);
            mash.service = service;
            mash.OnStart();
        }

        // state is false on start
        [Test]
        public void IsTriggeredFalseOnStartTest()
        {
            mash.OnStart();
            Assert.False(mash.IsTriggered());
        }

        [Test]
        public void IsCancellableFalseOnStartTest()
        {
            mash.OnStart();
            Assert.False(mash.IsInCancelTime());
        }

        [Test]
        public void IsFinishedFalseOnStartTest()
        {
            mash.OnStart();
            Assert.False(mash.IsFinished());
        }

        // mash duration non neg
        [Test]
        public void MashDurationNonNegativeOnStartTest()
        {
            mash.Construct(
                MashInstruction.instance,
                ScriptableObject.CreateInstance<TestExecutionEvent>(), // key down event
                ScriptableObject.CreateInstance<TestExecutionEvent>(), //  mash finished event
                -1); // mash duration 
            Assert.Throws<System.ArgumentException>(delegate
            {
                mash.OnStart();
            });
        }

        // mash duration non zero
        [Test]
        public void MashDurationNonZeroTest()
        {
            mash.Construct(
                MashInstruction.instance,
                ScriptableObject.CreateInstance<TestExecutionEvent>(), // key down event
                ScriptableObject.CreateInstance<TestExecutionEvent>(), //  mash finished event
                0); // mash duration 
            Assert.Throws<System.ArgumentException>(delegate
            {
                mash.OnStart();
            });
        }

        public void SimulateKeyDown()
        {
            IUnityInputService inputService = Substitute.For<IUnityInputService>();
            inputService.GetKeyDown("return").Returns(true);
            mash.instruction.service = inputService;
            mash.OnInput("return", null, null);
        }

        // key down fires key down event
        [Test]
        public void KeyDownExecutionEventFiresTest()
        {
            TestExecutionEvent executionEvent = ScriptableObject.CreateInstance<TestExecutionEvent>();
            mash.executionEvent = executionEvent;
            SimulateKeyDown();
            Assert.AreEqual(0, executionEvent.timesExecuted); // does not fire
            Assert.AreEqual(1, ((TestExecutionEvent) mash.GetExecutionEvent()).timesExecuted); // fires
        }

        [Test]
        public void KeyDownExecutionEventInstantiatesOnStartTest()
        {
            TestExecutionEvent executionEvent = ScriptableObject.CreateInstance<TestExecutionEvent>();
            mash.executionEvent = executionEvent;
            mash.OnStart();
            // Should be equal instances
            Assert.IsInstanceOf<TestExecutionEvent>(mash.GetExecutionEvent());
            // Should be different instances
            Assert.AreNotSame(executionEvent, mash.GetExecutionEvent());
        }

        [Test]
        public void MashEndedExecutionEventInstantiatesOnStartTest()
        {
            TestExecutionEvent executionEvent = ScriptableObject.CreateInstance<TestExecutionEvent>();
            mash.mashTimeEndedEvent = executionEvent;
            mash.OnStart();
            // Should be equal instances
            Assert.IsInstanceOf<TestExecutionEvent>(mash.GetMashEndedExecutionEvent());
            // Should be different instances
            Assert.AreNotSame(executionEvent, mash.GetMashEndedExecutionEvent());
        }

        [Test]
        public void MashEndedExecutionEventFiresTest()
        {
            TestExecutionEvent executionEvent = ScriptableObject.CreateInstance<TestExecutionEvent>();
            mash.mashTimeEndedEvent = executionEvent;
            mash.OnStart();
            SimulateKeyDown();
            mash.service.unscaledTime.Returns(2.5f);
            mash.OnInput("return", null, null);
            Assert.AreEqual(0, executionEvent.timesExecuted);
            Assert.AreEqual(1, ((TestExecutionEvent) mash.GetMashEndedExecutionEvent()).timesExecuted);
        }

        [Test]
        public void KeyDownEventFiresTest()
        {
            bool fired = false;
            mash.onKeyDown = () => fired = true;
            SimulateKeyDown();
            Assert.True(fired);
        }

        [Test]
        public void InstructionNotNullAfterOnStartTest()
        {
            mash.OnStart();
            Assert.NotNull(mash.instruction);
        }

        // key down event not null
        [Test]
        public void KeyDownEventNotNullTest()
        {
            mash.Construct(
                MashInstruction.instance,
                null, // key down event
                ScriptableObject.CreateInstance<TestExecutionEvent>(), //  mash finished event
                1); // mash duration 
            Assert.Throws<System.ArgumentException>(delegate
            {
                mash.OnStart();
            });
        }

        // mash time event not null
        [Test]
        public void MashTimeEventNotNullTest()
        {
            mash.Construct(
                MashInstruction.instance,
                null, // key down event
                ScriptableObject.CreateInstance<TestExecutionEvent>(), //  mash finished event
                1); // mash duration 
            Assert.Throws<System.ArgumentException>(delegate
            {
                mash.OnStart();
            });
        }

        // keydown sets triggered
        [Test]
        public void KeyDownSetsTriggeredTest()
        {
            SimulateKeyDown();
            Assert.True(mash.IsTriggered());
        }

        // cancellable fires when mash finishes
        [Test]
        public void MashFinishedFiresCancellableTest()
        {
            ExecutionEvent executionEvent = ScriptableObject.CreateInstance<CancelTestExecutionEvent>();
            mash.mashTimeEndedEvent = executionEvent;
            mash.OnStart();
            SimulateKeyDown();
            mash.service.unscaledTime.Returns(2.5f);
            mash.OnInput("return", null, null);
            Assert.True(mash.IsInCancelTime());
        }

        // finish fires when mash finishes 
        [Test]
        public void MashFinishedFiresFinishedTest()
        {
            ExecutionEvent executionEvent = ScriptableObject.CreateInstance<FinishTestExecutionEvent>();
            mash.mashTimeEndedEvent = executionEvent;
            mash.OnStart();
            SimulateKeyDown();
            mash.service.unscaledTime.Returns(2.5f);
            mash.OnInput("return", null, null);
            Assert.True(mash.IsFinished());
        }

        public void SimulateEndMashTimeWithNoKeyDown()
        {
            mash.service.unscaledTime.Returns(2.51f);
            mash.OnInput("return", null, null);
        }

        // no key down at end time triggered is false
        [Test]
        public void NoKeyDownAtEndSetTriggeredToFalseTest()
        {
            SimulateEndMashTimeWithNoKeyDown();
            Assert.False(mash.IsTriggered());
        }

        // no key down at end time sets cancellable to false
        [Test]
        public void NoKeyDownAtEndSetCancelTimeToFalseTest()
        {
            SimulateEndMashTimeWithNoKeyDown();
            Assert.False(mash.IsInCancelTime());
        }

        [Test]
        public void WaitsForKeyDownBeforeStartingTest()
        {
            SimulateEndMashTimeWithNoKeyDown();
            Assert.False(mash.IsFinished());
        }

        // no key down at end does not fire mashfinished event
        [Test]
        public void NoKeyDownDoesNotFireMashFinishedEvent()
        {
            TestExecutionEvent executionEvent = ScriptableObject.CreateInstance<TestExecutionEvent>();
            mash.mashTimeEndedEvent = executionEvent;
            SimulateEndMashTimeWithNoKeyDown();
            Assert.AreEqual(0, executionEvent.timesExecuted);
        }
    }

    public class OnReleaseHoldExecutionTest
    {

        OnReleaseHoldExecutableSO hold;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            hold = ScriptableObject.CreateInstance<OnReleaseHoldExecutableSO>();
        }
        
        [SetUp]
        public void SetUp()
        {

            hold.Construct(
                HoldInstruction.instance,
                ScriptableObject.CreateInstance<TestExecutionEvent>(), // key down event
                ScriptableObject.CreateInstance<TestExecutionEvent>(), //  release event
                2); // release time
            IUnityInputService service = Substitute.For<IUnityInputService>();
            HoldInstruction.instance.service = service;
            HoldInstruction.instance.reset();
            IUnityTimeService timeService = Substitute.For<IUnityTimeService>();
            timeService.unscaledTime.Returns(0);
            hold.service = timeService;
            hold.OnStart();
        }

        // Is finished is false
        [Test]
        public void IsFinishedFalseOnStartTest()
        {
            Assert.False(hold.IsFinished());
        }

        // in cancel time is false
        [Test]
        public void CancelTimeIsFalseOnStartTest()
        {
            Assert.False(hold.IsInCancelTime());
        }

        // is triggered false
        [Test]
        public void IsTriggeredFalseOnStartTest()
        {
            Assert.False(hold.IsTriggered());
        }

        // Instruction not null
        [Test]
        public void InstructionNotNullOnStartTest()
        {
            hold.OnStart();
            Assert.NotNull(hold.instruction);
        }

        // keyDownExecution null throws exception
        [Test]
        public void KeyDownExecutionNullThrowsExceptionOnStartTest()
        {
            hold.Construct(
                HoldInstruction.instance,
                null, // key down event
                ScriptableObject.CreateInstance<TestExecutionEvent>(), //  release event
                2); // release time
            Assert.Throws<System.ArgumentException>(delegate
            {
                hold.OnStart();
            }); 
        } 

        // release execution null throws exeception
        [Test]
        public void ReleaseExecutionNullthrowsExceptionStartTest()
        {
            hold.Construct(
                HoldInstruction.instance,
                ScriptableObject.CreateInstance<TestExecutionEvent>(), // key down event
                null, //  release event
                2); // release time
            Assert.Throws<System.ArgumentException>(delegate
            {
                hold.OnStart();
            });
        }

        // Test service not null
        [Test]
        public void ServiceNotNullAfterStartTest()
        {
            hold.OnStart();
            Assert.NotNull(hold.service);
        }

        [Test]
        public void WaitsForInputBeforeFinishingTest()
        {
            // test instruction used is 2 seconds long
            hold.service.unscaledTime.Returns(3);
            hold.OnInput("return", null, null);
            // Since there was no key down executable shouldnt be finished even though time has passed
            Assert.False(hold.IsFinished());
        }

        // Release time negative throws exception
        [Test]
        public void ReleaseTimeNonNegativeThrowsExceptionTest()
        {
            hold.Construct(
                HoldInstruction.instance,
                ScriptableObject.CreateInstance<TestExecutionEvent>(), // key down event
                ScriptableObject.CreateInstance<TestExecutionEvent>(), //  release event
                -1); // release time
            Assert.Throws<System.ArgumentException>(delegate
            {
                hold.OnStart();
            });
        }

        // release time 0 throws exception
        [Test]
        public void ReleaseTimeNonZeroThrowsExceptionTest()
        {
            hold.Construct(
               HoldInstruction.instance,
               ScriptableObject.CreateInstance<TestExecutionEvent>(), // key down event
               ScriptableObject.CreateInstance<TestExecutionEvent>(), //  release event
               0); // release time
            Assert.Throws<System.ArgumentException>(delegate
            {
                hold.OnStart();
            });
        }

        [Test]
        public void KeyDownEventInstantiateOnStartTest()
        {
            TestExecutionEvent @event = ScriptableObject.CreateInstance<TestExecutionEvent>();
            hold.keyDownExecutionEvent = @event;
            hold.OnStart();
            Assert.AreNotSame(@event, hold.GetKeyDownExecutionEvent());
            Assert.IsInstanceOf<TestExecutionEvent>(hold.GetKeyDownExecutionEvent());
        }

        [Test]
        public void ReleaseEventInstantiatedOnStartTest()
        {
            TestExecutionEvent @event = ScriptableObject.CreateInstance<TestExecutionEvent>();
            hold.releaseExecutionEvent = @event;
            hold.OnStart();
            Assert.AreNotSame(@event, hold.GetReleaseExecutionEvent());
            Assert.IsInstanceOf<TestExecutionEvent>(hold.GetReleaseExecutionEvent());
        }

        // keydown fires event
        [Test]
        public void KeyDownFiresKeyDownExecutionEventTest()
        {
            TestExecutionEvent testEvent = ScriptableObject.CreateInstance<TestExecutionEvent>();
            hold.keyDownExecutionEvent = testEvent;
            hold.instruction.service.GetKeyDown("return").Returns(true);
            hold.OnInput("return", null, null);
            Assert.AreEqual(1, ((TestExecutionEvent)hold.GetKeyDownExecutionEvent()).timesExecuted);
        }

        // inst triggers
        [Test]
        public void KeyDownTriggersInstTest()
        {
            TestExecutionEvent testEvent = ScriptableObject.CreateInstance<TestExecutionEvent>();
            hold.keyDownExecutionEvent = testEvent;
            hold.instruction.service.GetKeyDown("return").Returns(true);
            hold.OnInput("return", null, null);
            Assert.IsTrue(hold.IsTriggered());
        }

        // keydown while triggered does not fire event again
        [Test]
        public void KeyDownTwiceDoesNotFireTwiceTest()
        {
            TestExecutionEvent testEvent = ScriptableObject.CreateInstance<TestExecutionEvent>();
            hold.keyDownExecutionEvent = testEvent;
            hold.instruction.service.GetKeyDown("return").Returns(true);
            hold.OnInput("return", null, null);
            hold.OnInput("return", null, null);
            Assert.AreEqual(1, ((TestExecutionEvent)hold.GetKeyDownExecutionEvent()).timesExecuted);
        }

        // key down while triggered not in cancel time sets finished
        [Test]
        public void KeyDownWhileTriggeredNotInCancelTimeSetsFinished()
        {
            TestExecutionEvent testEvent = ScriptableObject.CreateInstance<TestExecutionEvent>();
            hold.keyDownExecutionEvent = testEvent;
            hold.instruction.service.GetKeyDown("return").Returns(true);
            // trigger first time
            hold.OnInput("return", null, null);
            // check conditions for test
            Assert.IsTrue(hold.IsTriggered());
            Assert.IsFalse(hold.IsInCancelTime());
            // key down again
            hold.OnInput("return", null, null);
            Assert.IsTrue(hold.IsFinished());
        }

        // KeyUp while not triggered does not fire release event
        [Test]
        public void KeyUpNotTriggeredDoesFireReleaseEventTest()
        {
            TestExecutionEvent testEvent = ScriptableObject.CreateInstance<TestExecutionEvent>();
            hold.releaseExecutionEvent = testEvent;
            hold.instruction.service.GetKeyUp("return").Returns(true);
            // pre condition triggered is false
            Assert.False(hold.IsTriggered());
            hold.OnInput("return", null, null);
            // Check key up does not fire release event
            Assert.AreEqual(0, testEvent.timesExecuted);
        }

        // Key Up while triggered fires OnRelease Event
        [Test]
        public void KeyUpTriggeredFiresOnReleaseEventTest()
        {
            TestExecutionEvent testEvent = ScriptableObject.CreateInstance<TestExecutionEvent>();
            hold.releaseExecutionEvent = testEvent;
            hold.instruction.service.GetKeyDown("return").Returns(true);
            hold.OnInput("return", null, null);
            //pre condition
            Assert.True(hold.IsTriggered());
            hold.instruction.service.GetKeyDown("return").Returns(false);
            hold.instruction.service.GetKeyUp("return").Returns(true);

            hold.OnInput("return", null, null);
            Assert.AreEqual(1,((TestExecutionEvent) hold.GetReleaseExecutionEvent()).timesExecuted);
        }

        // Release event sets cancellable
        [Test]
        public void TriggeredReleaseEventSetsCancellableTest()
        {
            ExecutionEvent testEvent = ScriptableObject.CreateInstance<CancelTestExecutionEvent>();
            hold.releaseExecutionEvent = testEvent;
            hold.OnStart();
            hold.instruction.service.GetKeyDown("return").Returns(true);
            hold.OnInput("return", null, null);
            //pre condition
            Assert.True(hold.IsTriggered());
            hold.instruction.service.GetKeyDown("return").Returns(false);
            hold.instruction.service.GetKeyUp("return").Returns(true);

            hold.OnInput("return", null, null);
            Assert.True(hold.IsInCancelTime());
        }

        // Release event sets finished
        [Test]
        public void TriggeredReleaseEventSetsFinishedTest()
        {
            ExecutionEvent testEvent = ScriptableObject.CreateInstance<FinishTestExecutionEvent>();
            hold.releaseExecutionEvent = testEvent;
            hold.OnStart();
            hold.instruction.service.GetKeyDown("return").Returns(true);
            hold.OnInput("return", null, null);
            //pre condition
            Assert.True(hold.IsTriggered());
            hold.instruction.service.GetKeyDown("return").Returns(false);
            hold.instruction.service.GetKeyUp("return").Returns(true);

            hold.OnInput("return", null, null);
            Assert.True(hold.IsFinished());
        }

        public void SimulateBadKey()
        {
            hold.instruction.service.GetKeyDown("return").Returns(true);
            hold.OnInput("return", null, null);

            //pre condition is that hold is triggered
            Assert.True(hold.IsTriggered());
            // key up simulation
            hold.instruction.service.GetKeyDown("return").Returns(false);
            hold.instruction.service.GetKeyUp("return").Returns(true);
            // set current time to 1 second passed, event needs to be there for 2 seconds
            IUnityTimeService service = Substitute.For<IUnityTimeService>();
            service.unscaledTime.Returns(1);
            hold.service = service;
            // Fire event again
            hold.OnInput("return", null, null);
        }

        // BadKey triggeres on release event
        [Test]
        public void BadKeyWhileTriggeredFiresOnReleaseEventTest()
        {
            TestExecutionEvent testEvent = ScriptableObject.CreateInstance<TestExecutionEvent>();
            hold.releaseExecutionEvent = testEvent;
            SimulateBadKey();
            Assert.AreEqual(1, ((TestExecutionEvent) hold.GetReleaseExecutionEvent()).timesExecuted);
        }

        // badkey sets finished
        [Test]
        public void BadKeyWhileTriggeredSetsFinishedTest()
        {
            SimulateBadKey();
            Assert.True(hold.IsFinished());
        }

        // bad key sets cancellable false
        [Test]
        public void BadKeyWhileTriggeredSetsCancellableFalseTest()
        {
            SimulateBadKey();
            Assert.False(hold.IsInCancelTime());

        }
    }

    public class PressExecutionTest
    {
        PressExecutableSO press;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            press = ScriptableObject.CreateInstance<PressExecutableSO>();
        }

        [SetUp]
        public void SetUp()
        {
            PressInstruction.instance.reset();
            IUnityInputService service = Substitute.For<IUnityInputService>();
            service.GetKeyDown("return").Returns(true);
            PressInstruction.instance.service = service;
            ExecutionEvent executionEvent = ScriptableObject.CreateInstance<TestExecutionEvent>();
            PressInstruction instruction = PressInstruction.instance;
            press.Construct(instruction, executionEvent);
            press.OnStart();
        }

        // public abstract void OnStart();
        // instruction notnull
        [Test]
        public void InstructionNotNullTest()
        {
            ExecutionEvent executionEvent = ScriptableObject.CreateInstance<TestExecutionEvent>();
            press.Construct(null, executionEvent);
            press.OnStart();
            Assert.NotNull(press.instruction);

        }

        // event non null
        [Test]
        public void ExecutionEventNotNullTest()
        {
            PressInstruction instruction = PressInstruction.instance;
            press.Construct(instruction, null);
            Assert.Throws<System.ArgumentException>(delegate
            {
                press.OnStart();
            });
        }

        // Is triggered is false
        [Test]
        public void IsTriggeredFalseOnStartTest()
        {
            press.OnStart();
            Assert.False(press.IsTriggered());
        }
        
        // Is cancel is false on start test
        [Test]
        public void InCancelTimeFalseOnStartTest()
        {
            press.OnStart();
            Assert.False(press.IsInCancelTime());
        }

        // Is not finished on start
        [Test]
        public void IsNotFinishedOnStartTest()
        {
            press.OnStart();
            Assert.False(press.IsFinished());
        }

        //public abstract void OnInput(string input, IBattler battler, ITargetSet targets);
        // test key up does nothing
        [Test]
        public void KeyUpDoesNothingTest()
        {
            PressInstruction instruction = PressInstruction.instance;
            PressInstruction.instance.service.GetKeyDown("return").Returns(false);
            PressInstruction.instance.service.GetKeyUp("return").Returns(true);
            ExecutionEvent executionEvent = ScriptableObject.CreateInstance<TestExecutionEvent>();
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
            PressInstruction instruction = PressInstruction.instance;
            PressInstruction.instance.service.GetKeyDown("return").Returns(false);
            PressInstruction.instance.service.GetKey("return").Returns(true);
            ExecutionEvent executionEvent = ScriptableObject.CreateInstance<TestExecutionEvent>();
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
            PressInstruction instruction = PressInstruction.instance;
            ExecutionEvent executionEvent = ScriptableObject.CreateInstance<TestExecutionEvent>();
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
            PressInstruction instruction = PressInstruction.instance;
            CancelTestExecutionEvent executionEvent = ScriptableObject.CreateInstance<CancelTestExecutionEvent>();
            IBattler battler = Substitute.For<IBattler>();
            ITargetSet targets = Substitute.For<ITargetSet>();
            press.Construct(instruction, executionEvent);
            press.OnStart();
            // cancel event fires immediately with this event
            press.OnInput("return", battler, targets);

            Assert.IsTrue(press.IsInCancelTime());
        }

        // test finished when event finished fires
        [Test]
        public void FinishedWhenFinishEventFiresTest()
        {
            PressInstruction instruction = PressInstruction.instance;
            FinishTestExecutionEvent executionEvent = ScriptableObject.CreateInstance<FinishTestExecutionEvent>();
            IBattler battler = Substitute.For<IBattler>();
            ITargetSet targets = Substitute.For<ITargetSet>();

            press.Construct(instruction, executionEvent);
            press.OnStart();
            press.OnInput("return", battler, targets);

            Assert.IsTrue(press.IsFinished());
        }

        // test keydown while triggered not in canceltime does not fire event twice
        [Test]
        public void KeyDownWhileTriggeredDoesNotFireExecutionEventTest()
        {
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
