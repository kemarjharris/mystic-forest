using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using NSubstitute;
using UnityEngine.TestTools;

namespace ExecutableTest
{
    public class LockOnExecutableTest
    {
        LockOnExecutable lockOn;
        ExecutionEvent onStartLockOnEvent;
        ExecutionEvent onTargetSelectedEvent;
        IBattler battler;
        ITargetSet targets;

        List<GameObject> objects;

        public LockOnExecutable Construct(ExecutionEvent onStartLockOnEvent, ExecutionEvent onTargetSelectedEvent, float duration)
        {
            LockOnExecutableSO so = ScriptableObject.CreateInstance<LockOnExecutableSO>();
            so.onStartLockOn = onStartLockOnEvent;
            so.onTargetSelected = onTargetSelectedEvent;
            so.lockOnDuration = duration;
            LockOnExecutable e = (LockOnExecutable) so.CreateExecutable();
            e.OnStart();
            return e;
        }

        [SetUp]
        public void SetUp()
        {
            onStartLockOnEvent = ScriptableObject.CreateInstance<TestExecutionEvent>();
            onTargetSelectedEvent = ScriptableObject.CreateInstance<TestExecutionEvent>();
            lockOn = Construct(onStartLockOnEvent, onTargetSelectedEvent, 2);
            lockOn.button = DirectionCommandButton.J;
            
            lockOn.timeService = Substitute.For<IUnityTimeService>();
            lockOn.inputService = Substitute.For<IUnityInputService>();
            lockOn.axisService = Substitute.For<IUnityAxisService>();

            lockOn.OnStart();
            GameObject go = Object.Instantiate(Resources.Load<GameObject>("TestPrefabs/MockBattler"));
            go.transform.position = Vector3.zero;
            battler = Substitute.For<IBattler>();
            battler.gameObject.Returns(go);
            targets = new TargetSet();
            objects = new List<GameObject>
            {
                go
            };
        }

        [TearDown]
        public void TearDown()
        {
            foreach (GameObject gameObject in objects)
            {
                Object.Destroy(gameObject);
            }
        }

        public GameObject LoadBattler(int xPos)
        {
            GameObject go = Object.Instantiate(Resources.Load<GameObject>("TestPrefabs/MockBattler"));
            go.transform.position = Vector3.right * xPos;
            go.name = "Mock Battler at " + xPos; 
            objects.Add(go);
            return go;
        }

        public void CallOnInput(string input)
        {
            lockOn.OnInput(input, battler, targets);
        }


        // state is false on start
        [Test]
        public void OnStart_NotTriggered()
        {
            Assert.False(lockOn.IsTriggered());
        }

        [Test]
        public void OnStart_NotCancellable()
        {
            Assert.False(lockOn.IsInCancelTime());
        }

        [Test]
        public void OnStart_NotFinished()
        {
            Assert.False(lockOn.IsFinished());
        }

        [Test]
        public void OnStart_NotFired()
        {
            Assert.False(lockOn.HasFired());
        }

        [Test]
        public void OnStart_NotLockingOn()
        {
            Assert.False(lockOn.isLockingOn());
        }

        // Duration Non negative
        [Test]
        public void OnStart_NegativeDuration_ThrowsError()
        {
            lockOn.lockOnDuration = -1;
            Assert.Throws<System.ArgumentException>(delegate
            {
                lockOn.OnStart();
            });
        }

        // duration greater than 0
        [Test]
        public void OnStart_ZeroDuration_ThrowsError()
        {
            lockOn.lockOnDuration = 0;
            Assert.Throws<System.ArgumentException>(delegate
            {
                lockOn.OnStart();
            });
        }

        // execution events not null
        [Test]
        public void OnStart_StartLockOnEventNull_ThrowsExeception()
        {
            lockOn.onStartLockOn = null;
            Assert.Throws<System.ArgumentException>(delegate
            {
                lockOn.OnStart();
            });
        }

        [Test]
        public void OnStart_TargetSelectedEventNull_ThrowsExeception()
        {
            lockOn.onTargetSelected = null;
            Assert.Throws<System.ArgumentException>(delegate
            {
                lockOn.OnStart();
            });
        }

        [Test]
        public void OnStart_SetsUpOnCancellableEvent()
        {
            onStartLockOnEvent = ScriptableObject.CreateInstance<TestExecutionEvent>();
            onTargetSelectedEvent = ScriptableObject.CreateInstance<CancelTestExecutionEvent>();
            lockOn = Construct(onStartLockOnEvent, onTargetSelectedEvent, 2);
            lockOn.OnTargetSelectedEvent().OnExecute(null, null);
            Assert.True(lockOn.IsInCancelTime());
        }

        [Test]
        public void OnStart_SetsUpOnFinishedEvent()
        {
            onStartLockOnEvent = ScriptableObject.CreateInstance<TestExecutionEvent>();
            onTargetSelectedEvent = ScriptableObject.CreateInstance<FinishTestExecutionEvent>();
            lockOn = Construct(onStartLockOnEvent, onTargetSelectedEvent, 2);
            lockOn.OnTargetSelectedEvent().OnExecute(null, null);
            Assert.True(lockOn.IsFinished());
        }

        // key down sets triggered
        [Test]
        public void OnInput_TriggerKeyDown_TriggersExecutable()
        {
            lockOn.inputService.GetKeyDown("j").Returns(true);
            CallOnInput("j");
            Assert.True(lockOn.IsTriggered());
        }

        // key down calls on state 
        [Test]
        public void OnInput_TriggerKeyDown_OnStartEventFires()
        {
            lockOn.inputService.GetKeyDown("j").Returns(true);
            CallOnInput("j");
            Assert.AreEqual(1, ((TestExecutionEvent) lockOn.onStartLockOn).timesExecuted);
        }

        // time out not triggered does not finish
        [Test]
        public void OnInput_NotTriggeredPastDuration_SetsFinished()
        {
            lockOn.timeService.unscaledTime.Returns(4);
            Assert.False(lockOn.IsFinished());
        }

        // time service timeout triggers finished
        [Test]
        public void OnInput_TriggeredTimeOut_SetsFinished()
        {
            lockOn.inputService.GetKeyDown("j").Returns(true);
            CallOnInput("j");
            // duration defaults to 2
            lockOn.timeService.unscaledTime.Returns(4);
            CallOnInput("j");
            Assert.True(lockOn.IsFinished());
        }

        public void SimulateKeyUp(string input)
        {
            lockOn.inputService.GetKeyDown(input).Returns(true);
            CallOnInput(input);
            lockOn.inputService.GetKeyDown(input).Returns(false);
            lockOn.inputService.GetKeyUp(input).Returns(true);
            CallOnInput(input);
        }

        // keyup while triggered triggeres on execute
        [UnityTest]
        public IEnumerator OnInput_KeyUpTarget_CallsOnTargetSelected()
        {
            LoadBattler(1);
            yield return new WaitForSeconds(1);
            SimulateKeyUp("j");
            Assert.AreEqual(1, ((TestExecutionEvent) lockOn.onTargetSelected).timesExecuted);
        }

        // keyup while not triggered does not finish
        [Test]
        public void OnInput_NotTriggeredKeyUp_DoesNotSetFinished()
        {
            lockOn.inputService.GetKeyUp("j").Returns(true);
            CallOnInput("j");
            Assert.False(lockOn.IsFinished());
        }

        // key up notarget sets finished
        [Test]
        public void OnInput_KeyUpNoTarget_SetsFinished()
        {
            SimulateKeyUp("j");
            Assert.True(lockOn.IsFinished());
        }

        // key up target calls fired
        [UnityTest]
        public IEnumerator OnInput_KeyUpTarget_SetsFired()
        {
            LoadBattler(1);
            yield return new WaitForSeconds(1);
            SimulateKeyUp("j");
            Assert.True(lockOn.HasFired());
        }

        // target is not parent battler
        [UnityTest]
        public IEnumerator OnInput_KeyUpTarget_IsNotParentBattler()
        {
            GameObject expected = LoadBattler(1);
            yield return new WaitForSeconds(1);
            SimulateKeyUp("j");
            Assert.AreEqual(expected.transform, targets.GetTarget());
        }

        // on start lock on selects next target
        // target selected is battler
        [UnityTest]
        public IEnumerator OnInput_KeyDown_SelectsTarget()
        {
            GameObject expected = LoadBattler(1);
            yield return new WaitForSeconds(1);
            lockOn.inputService.GetKeyDown("j").Returns(true);
            CallOnInput("j");
            Assert.AreSame(expected, lockOn.CurrentTarget());
        }

        [Test]
        public void OnInput_KeyDown_StartsLockingOn()
        {
            lockOn.inputService.GetKeyDown("j").Returns(true);
            CallOnInput("j");
            Assert.True(lockOn.isLockingOn());
        }

        // keyup while triggered stops locking on
        [Test]
        public void OnInput_KeyUp_StopsLockingOn()
        {
            SimulateKeyUp("j");
            Assert.False(lockOn.isLockingOn());
        }

        // null target does not fire on target selected
        [Test]
        public void OnInput_KeyUpNoTargets_DoesNotFireOnTargetSelectedEvent()
        {
            SimulateKeyUp("j");
            Assert.AreEqual(0, ((TestExecutionEvent) lockOn.onTargetSelected).timesExecuted);
        }

        // getkey right positive input switches targets
        [UnityTest]
        public IEnumerator OnInput_GetKeyPositiveHorziontalInput_SwitchesTargets()
        {
            GameObject a = LoadBattler(1);
            GameObject b = LoadBattler(-1);
            yield return new WaitForSeconds(1);
            lockOn.inputService.GetKeyDown("j").Returns(true);
            CallOnInput("j");
            yield return null;
            lockOn.inputService.GetKeyDown("j").Returns(false);
            lockOn.inputService.GetKey("j").Returns(true);
            lockOn.axisService.GetAxisDown("Horizontal").Returns(1);
            GameObject expected = lockOn.CurrentTarget().transform == a.transform ? b : a;

            CallOnInput("j");
            Assert.AreSame(expected, lockOn.CurrentTarget().gameObject);
        }

        // ignores non battlers
        [Test]
        public void OnInput_Targeting_IgnoresNonBattlers()
        {
            GameObject obj = new GameObject();
            obj.transform.position = Vector3.right;
            objects.Add(obj);

            lockOn.inputService.GetKeyDown("j").Returns(true);
            CallOnInput("j");

            Assert.Null(lockOn.CurrentTarget());
        }
    }

    public class KeyDownMashExecutableTest
    {
        KeyDownMashExecutable mash;
        ExecutionEvent keyDownEvent;
        ExecutionEvent mashFinishedEvent;
        
        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            mash = new KeyDownMashExecutable();
        }

        [SetUp]
        public void SetUp()
        {
            keyDownEvent = ScriptableObject.CreateInstance<TestExecutionEvent>();
            mashFinishedEvent = ScriptableObject.CreateInstance<TestExecutionEvent>();
            mash = Construct(
               MashInstruction.instance,
               keyDownEvent, // key down event
               mashFinishedEvent, //  mash finished event
               2); // mash duration 
            mash.button =  DirectionCommandButton.J;
            IUnityTimeService service = Substitute.For<IUnityTimeService>();
            service.unscaledTime.Returns(0);
            mash.service = service;
            MashInstruction.instance.service = Substitute.For<IUnityInputService>();
            mash.OnStart();
        }

        public KeyDownMashExecutable Construct(MashInstruction instruction, ExecutionEvent keyDownEvent, ExecutionEvent mashFinishedEvent, float duration)
        {
            KeyDownMashExecutableSO so = ScriptableObject.CreateInstance<KeyDownMashExecutableSO>();
            so.executionEvent = keyDownEvent;
            so.mashTimeEndedEvent = mashFinishedEvent;
            so.mashDuration = duration;
            return (KeyDownMashExecutable)so.CreateExecutable();
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

        [Test]
        public void HasFiredFalseOnStart()
        {
            mash.OnStart();
            Assert.False(mash.HasFired());
        }

        // mash duration non neg
        [Test]
        public void MashDurationNonNegativeOnStartTest()
        {
            mash = Construct(
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
            mash = Construct(
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
            inputService.GetKeyDown("j").Returns(true);
            mash.instruction.service = inputService;
            mash.OnInput("j", null, null);
        }

        // key down fires key down event
        [Test]
        public void KeyDownExecutionEventFiresTest()
        {
            SimulateKeyDown();
            Assert.AreEqual(0, ((TestExecutionEvent) keyDownEvent).timesExecuted); // does not fire
            Assert.AreEqual(1, ((TestExecutionEvent) mash.GetExecutionEvent()).timesExecuted); // fires
        }

        [Test]
        public void KeyDownExecutionEventInstantiatesOnStartTest()
        {
            // Should be equal instances
            Assert.IsInstanceOf<TestExecutionEvent>(mash.GetExecutionEvent());
            // Should be different instances
            Assert.AreNotSame(keyDownEvent, mash.GetExecutionEvent());
        }

        [Test]
        public void MashEndedExecutionEventInstantiatesOnStartTest()
        {
            // Should be equal instances
            Assert.IsInstanceOf<TestExecutionEvent>(mash.GetMashEndedExecutionEvent());
            // Should be different instances
            Assert.AreNotSame(mashFinishedEvent, mash.GetMashEndedExecutionEvent());
        }

        [Test]
        public void MashEndedExecutionEventFiresTest()
        {
            SimulateKeyDown();
            mash.service.unscaledTime.Returns(2.5f);
            mash.OnInput("j", null, null);
            Assert.AreEqual(0, ((TestExecutionEvent) mashFinishedEvent).timesExecuted);
            Assert.AreEqual(1, ((TestExecutionEvent) mash.GetMashEndedExecutionEvent()).timesExecuted);
        }

        [Test]
        public void MashEndedExecutionEventFiresWithNoInputTest()
        {
            SimulateKeyDown();
            mash.service.unscaledTime.Returns(2.5f);
            mash.OnInput("", null, null);
            Assert.AreEqual(0, ((TestExecutionEvent)mashFinishedEvent).timesExecuted);
            Assert.AreEqual(1, ((TestExecutionEvent)mash.GetMashEndedExecutionEvent()).timesExecuted);
        }

        [Test]
        public void MashEndedSetsFiredTest()
        {
            TestExecutionEvent executionEvent = ScriptableObject.CreateInstance<TestExecutionEvent>();
            mash.mashTimeEndedEvent = executionEvent;
            mash.OnStart();
            SimulateKeyDown();
            mash.service.unscaledTime.Returns(2.5f);
            mash.OnInput("", null, null);
            Assert.True(mash.HasFired());
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
            Assert.Throws<System.ArgumentException>(delegate
            {
                mash = Construct(
                MashInstruction.instance,
                null, // key down event
                ScriptableObject.CreateInstance<TestExecutionEvent>(), //  mash finished event
                1); // mash duration 
                mash.OnStart();
            });
        }

        // mash time event not null
        [Test]
        public void MashTimeEventNotNullTest()
        {
            Assert.Throws<System.ArgumentException>(delegate
            {
                mash = Construct(
                MashInstruction.instance,
                null, // key down event
                ScriptableObject.CreateInstance<TestExecutionEvent>(), //  mash finished event
                1); // mash duration 
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

        [Test]
        public void WrongKeyDoesNotTriggerTest()
        {
            IUnityInputService inputService = Substitute.For<IUnityInputService>();
            inputService.GetKeyDown("k").Returns(true);
            mash.instruction.service = inputService;
            mash.button =  DirectionCommandButton.J;
            mash.OnInput("k", null, null);
            Assert.False(mash.IsTriggered());
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
            mash.OnInput("j", null, null);
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
            mash.OnInput("j", null, null);
            Assert.True(mash.IsFinished());
        }

        public void SimulateEndMashTimeWithNoKeyDown()
        {
            mash.service.unscaledTime.Returns(2.51f);
            mash.OnInput("j", null, null);
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

        OnReleaseHoldExecutable hold;
        ExecutionEvent keyDownEvent;
        ExecutionEvent releaseEvent;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            hold = new OnReleaseHoldExecutable();
        }
        
        [SetUp]
        public void SetUp()
        {
            keyDownEvent = ScriptableObject.CreateInstance<TestExecutionEvent>();
            releaseEvent = ScriptableObject.CreateInstance<TestExecutionEvent>();
            hold = Construct(
                HoldInstruction.instance,
                keyDownEvent, // key down event
                releaseEvent, //  release event
                2); // release time
            hold.button =  DirectionCommandButton.J;
            IUnityInputService service = Substitute.For<IUnityInputService>();
            HoldInstruction.instance.service = service;
            HoldInstruction.instance.reset();
            IUnityTimeService timeService = Substitute.For<IUnityTimeService>();
            timeService.unscaledTime.Returns(0);
            hold.service = timeService;
            hold.OnStart();
        }

        public OnReleaseHoldExecutable Construct(HoldInstruction instruction, ExecutionEvent keyDownEvent, ExecutionEvent releaseEvent, float duration)
        {
            OnReleaseHoldExecutableSO so = ScriptableObject.CreateInstance<OnReleaseHoldExecutableSO>();
            so.keyDownExecutionEvent = keyDownEvent;
            so.releaseExecutionEvent = releaseEvent;
            so.releaseTime = duration;
            return (OnReleaseHoldExecutable)so.CreateExecutable();
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

        [Test]
        public void HasFiredFalseOnStart()
        {
            Assert.False(hold.HasFired());
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
            Assert.Throws<System.ArgumentException>(delegate
            {
                hold = Construct(
                HoldInstruction.instance,
                null, // key down event
                ScriptableObject.CreateInstance<TestExecutionEvent>(), //  release event
                2); // release time
            
                hold.OnStart();
            }); 
        } 

        // release execution null throws exeception
        [Test]
        public void ReleaseExecutionNullthrowsExceptionStartTest()
        {
            Assert.Throws<System.ArgumentException>(delegate
            {
                hold = Construct(
                HoldInstruction.instance,
                ScriptableObject.CreateInstance<TestExecutionEvent>(), // key down event
                null, //  release event
                2); // release time
            
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
            hold.OnInput("j", null, null);
            // Since there was no key down executable shouldnt be finished even though time has passed
            Assert.False(hold.IsFinished());
        }

        // Release time negative throws exception
        [Test]
        public void ReleaseTimeNonNegativeThrowsExceptionTest()
        {
            hold = Construct(
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
            hold = Construct(
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
            Assert.AreNotSame(keyDownEvent, hold.GetKeyDownExecutionEvent());
            Assert.IsInstanceOf<TestExecutionEvent>(hold.GetKeyDownExecutionEvent());
        }

        [Test]
        public void ReleaseEventInstantiatedOnStartTest()
        {
            hold.OnStart();
            Assert.AreNotSame(releaseEvent, hold.GetReleaseExecutionEvent());
            Assert.IsInstanceOf<TestExecutionEvent>(hold.GetReleaseExecutionEvent());
        }

        // keydown fires event
        [Test]
        public void KeyDownFiresKeyDownExecutionEventTest()
        {
            TestExecutionEvent testEvent = ScriptableObject.CreateInstance<TestExecutionEvent>();
            hold.keyDownExecutionEvent = testEvent;
            hold.instruction.service.GetKeyDown("j").Returns(true);
            hold.OnInput("j", null, null);
            Assert.AreEqual(1, ((TestExecutionEvent)hold.GetKeyDownExecutionEvent()).timesExecuted);
        }

        // inst triggers
        [Test]
        public void KeyDownTriggersInstTest()
        {
            TestExecutionEvent testEvent = ScriptableObject.CreateInstance<TestExecutionEvent>();
            hold.keyDownExecutionEvent = testEvent;
            hold.instruction.service.GetKeyDown("j").Returns(true);
            hold.OnInput("j", null, null);
            Assert.IsTrue(hold.IsTriggered());
        }

        [Test]
        public void WrongKeyDoesNotTriggerTest()
        {
            TestExecutionEvent testEvent = ScriptableObject.CreateInstance<TestExecutionEvent>();
            hold.keyDownExecutionEvent = testEvent;
            hold.button =  DirectionCommandButton.J;
            hold.instruction.service.GetKeyDown("k").Returns(true);
            hold.OnInput("k", null, null);
            Assert.False(hold.IsTriggered());
        }

        // keydown while triggered does not fire event again
        [Test]
        public void KeyDownTwiceDoesNotFireTwiceTest()
        {
            TestExecutionEvent testEvent = ScriptableObject.CreateInstance<TestExecutionEvent>();
            hold.keyDownExecutionEvent = testEvent;
            hold.instruction.service.GetKeyDown("j").Returns(true);
            hold.OnInput("j", null, null);
            hold.OnInput("j", null, null);
            Assert.AreEqual(1, ((TestExecutionEvent)hold.GetKeyDownExecutionEvent()).timesExecuted);
        }

        // key down while triggered not in cancel time sets finished
        [Test]
        public void KeyDownWhileTriggeredNotInCancelTimeSetsFinished()
        {
            TestExecutionEvent testEvent = ScriptableObject.CreateInstance<TestExecutionEvent>();
            hold.keyDownExecutionEvent = testEvent;
            hold.instruction.service.GetKeyDown("j").Returns(true);
            // trigger first time
            hold.OnInput("j", null, null);
            // check conditions for test
            Assert.IsTrue(hold.IsTriggered());
            Assert.IsFalse(hold.IsInCancelTime());
            // key down again
            hold.OnInput("j", null, null);
            Assert.IsTrue(hold.IsFinished());
        }

        // KeyUp while not triggered does not fire release event
        [Test]
        public void KeyUpNotTriggeredDoesFireReleaseEventTest()
        {
            TestExecutionEvent testEvent = ScriptableObject.CreateInstance<TestExecutionEvent>();
            hold.releaseExecutionEvent = testEvent;
            hold.instruction.service.GetKeyUp("j").Returns(true);
            // pre condition triggered is false
            Assert.False(hold.IsTriggered());
            hold.OnInput("j", null, null);
            // Check key up does not fire release event
            Assert.AreEqual(0, testEvent.timesExecuted);
        }

        // Key Up while triggered fires OnRelease Event
        [Test]
        public void KeyUpTriggeredFiresOnReleaseEventTest()
        {
            TestExecutionEvent testEvent = ScriptableObject.CreateInstance<TestExecutionEvent>();
            hold.releaseExecutionEvent = testEvent;
            SimulateKeyUp();
            Assert.AreEqual(1,((TestExecutionEvent) hold.GetReleaseExecutionEvent()).timesExecuted);
        }

        // Key Up while triggered fires OnRelease Event
        [Test]
        public void KeyUpTriggeredSetsFiredTest()
        {
            TestExecutionEvent testEvent = ScriptableObject.CreateInstance<TestExecutionEvent>();
            hold.releaseExecutionEvent = testEvent;
            SimulateKeyUp();
            Assert.True(hold.HasFired());
        }

        // Release event sets cancellable
        [Test]
        public void TriggeredReleaseEventSetsCancellableTest()
        {
            ExecutionEvent testEvent = ScriptableObject.CreateInstance<CancelTestExecutionEvent>();
            hold.releaseExecutionEvent = testEvent;
            hold.OnStart();
            SimulateKeyUp();
            Assert.True(hold.IsInCancelTime());
        }

        // Release event sets finished
        [Test]
        public void TriggeredReleaseEventSetsFinishedTest()
        {
            ExecutionEvent testEvent = ScriptableObject.CreateInstance<FinishTestExecutionEvent>();
            hold.releaseExecutionEvent = testEvent;
            hold.OnStart();
            hold.instruction.service.GetKeyDown("j").Returns(true);
            hold.OnInput("j", null, null);
            //pre condition
            Assert.True(hold.IsTriggered());
            hold.instruction.service.GetKeyDown("j").Returns(false);
            hold.instruction.service.GetKeyUp("j").Returns(true);

            hold.OnInput("j", null, null);
            Assert.True(hold.IsFinished());
        }

        public void SimulateBadKey()
        {
            hold.instruction.service.GetKeyDown("j").Returns(true);
            hold.OnInput("j", null, null);

            //pre condition is that hold is triggered
            Assert.True(hold.IsTriggered());
            // key up simulation
            hold.instruction.service.GetKeyDown("j").Returns(false);
            hold.instruction.service.GetKeyUp("j").Returns(true);
            // set current time to 1 second passed, event needs to be there for 2 seconds
            IUnityTimeService service = Substitute.For<IUnityTimeService>();
            service.unscaledTime.Returns(1);
            hold.service = service;
            // Fire event again
            hold.OnInput("j", null, null);
        }

        public void SimulateKeyUp()
        {
            hold.instruction.service.GetKeyDown("j").Returns(true);
            hold.OnInput("j", null, null);

            //pre condition is that hold is triggered
            Assert.True(hold.IsTriggered());
            // key up simulation
            hold.instruction.service.GetKeyDown("j").Returns(false);
            hold.instruction.service.GetKeyUp("j").Returns(true);
            // set current time to 1 second passed, event needs to be there for 2 seconds
            IUnityTimeService service = Substitute.For<IUnityTimeService>();
            service.unscaledTime.Returns(2);
            hold.service = service;
            // Fire event again
            hold.OnInput("j", null, null);
        }

        // BadKey triggeres on release event
        // Needs to be set to on misfire event
        /*
        [Test]
        public void BadKeyWhileTriggeredFiresOnReleaseEventTest()
        {
            TestExecutionEvent testEvent = ScriptableObject.CreateInstance<TestExecutionEvent>();
            hold.releaseExecutionEvent = testEvent;
            SimulateBadKey();
            Assert.AreEqual(1, ((TestExecutionEvent) hold.GetReleaseExecutionEvent()).timesExecuted);
        }
        */

        // badkey sets finished
        [Test]
        public void BadKeyWhileTriggeredSetsFinishedTest()
        {
            SimulateBadKey();
            Assert.True(hold.IsFinished());
        }

        [Test]
        public void BadKeyWhileTriggeredSetsFiredTest()
        {
            SimulateBadKey();
            Assert.True(hold.HasFired());
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
        PressExecutable press;
        ExecutionEvent executionEvent;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            press = new PressExecutable();
        }

        [SetUp]
        public void SetUp()
        {
            PressInstruction.instance.reset();
            IUnityInputService service = Substitute.For<IUnityInputService>();
            service.GetKeyDown("j").Returns(true);
            PressInstruction.instance.service = service;
            executionEvent = ScriptableObject.CreateInstance<TestExecutionEvent>();
            PressInstruction instruction = PressInstruction.instance;
            press = Construct(instruction, executionEvent);
            press.button =  DirectionCommandButton.J;
            press.OnStart();
        }

        public PressExecutable Construct(PressInstruction instruction, ExecutionEvent executionEvent)
        {
            PressExecutableSO so = ScriptableObject.CreateInstance<PressExecutableSO>();
            so.instruction = instruction;
            so.executionEvent = executionEvent;
            PressExecutable p = (PressExecutable) so.CreateExecutable();
            p.instruction = instruction;
            p.OnStart();
            return p;
        }

        // public abstract void OnStart();
        // instruction notnull
        [Test]
        public void InstructionNotNullTest()
        {
            ExecutionEvent executionEvent = ScriptableObject.CreateInstance<TestExecutionEvent>();
            press = Construct(null, executionEvent);
            press.OnStart();
            Assert.NotNull(press.instruction);

        }

        // event non null
        [Test]
        public void ExecutionEventNotNullTest()
        {
            Assert.Throws<System.ArgumentException>(delegate
            {
                PressInstruction instruction = PressInstruction.instance;
                press = Construct(instruction, null);
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

        [Test]
        public void HasFiredFalseOnStart()
        {
            press.OnStart();
            Assert.False(press.HasFired());
        }

        //public abstract void OnInput(string input, IBattler battler, ITargetSet targets);
        // test key up does nothing
        [Test]
        public void KeyUpDoesNothingTest()
        {
            PressInstruction instruction = PressInstruction.instance;
            PressInstruction.instance.service.GetKeyDown("j").Returns(false);
            PressInstruction.instance.service.GetKeyUp("j").Returns(true);
            ExecutionEvent executionEvent = ScriptableObject.CreateInstance<TestExecutionEvent>();
            IBattler battler = Substitute.For<IBattler>();
            ITargetSet targets = Substitute.For<ITargetSet>();

            press = Construct(instruction, executionEvent);

            press.OnInput("j", battler, targets);

            Assert.IsFalse(press.IsTriggered());
        }


        // held key does nothing
        [Test]
        public void KeyHeldNothingTest()
        {
            PressInstruction instruction = PressInstruction.instance;
            PressInstruction.instance.service.GetKeyDown("j").Returns(false);
            PressInstruction.instance.service.GetKey("j").Returns(true);
            ExecutionEvent executionEvent = ScriptableObject.CreateInstance<TestExecutionEvent>();
            IBattler battler = Substitute.For<IBattler>();
            ITargetSet targets = Substitute.For<ITargetSet>();

            press = Construct(instruction, executionEvent);

            press.OnInput("j", battler, targets);

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

            press = Construct(instruction, executionEvent);

            press.OnInput("j", battler, targets);

            Assert.IsTrue(press.IsTriggered());
        }

        // Wrong key does not trigger test
        [Test]
        public void WrongKeyDoesNotTriggerTest()
        {
            PressInstruction instruction = PressInstruction.instance;
            ExecutionEvent executionEvent = ScriptableObject.CreateInstance<TestExecutionEvent>();
            IBattler battler = Substitute.For<IBattler>();
            ITargetSet targets = Substitute.For<ITargetSet>();

            press = Construct(instruction, executionEvent);
            press.button =  DirectionCommandButton.J;
            press.OnStart();
            press.OnInput("k", battler, targets);

            Assert.False(press.IsTriggered());
        }

        [Test]
        public void DifferentExecutionInstancesOnStartTest()
        {
            PressInstruction instruction = PressInstruction.instance;
            ExecutionEvent executionEvent = ScriptableObject.CreateInstance<TestExecutionEvent>();
            IBattler battler = Substitute.For<IBattler>();
            ITargetSet targets = Substitute.For<ITargetSet>();
            press = Construct(instruction, executionEvent);
            press.OnStart();
            Assert.AreNotSame(executionEvent, press.GetExecutionEvent());
            Assert.IsInstanceOf<TestExecutionEvent>(press.GetExecutionEvent());
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
            press = Construct(instruction, executionEvent);
            press.OnStart();
            press.OnInput("j", battler, targets);

            Assert.IsTrue(((TestExecutionEvent) press.GetExecutionEvent()).timesExecuted == 1);
        }

        [Test]
        public void HasFiredTrueOnFireTest()
        {
            PressInstruction instruction = PressInstruction.instance;
            TestExecutionEvent executionEvent = ScriptableObject.CreateInstance<TestExecutionEvent>();
            IBattler battler = Substitute.For<IBattler>();
            ITargetSet targets = Substitute.For<ITargetSet>();
            // triggers event
            press = Construct(instruction, executionEvent);
            press.OnStart();
            press.OnInput("j", battler, targets);

            Assert.IsTrue(press.HasFired());
        }

        // test cancellable when event cancellable fires
        [Test]
        public void CancellableWhenCancelFiresTest()
        {
            PressInstruction instruction = PressInstruction.instance;
            CancelTestExecutionEvent executionEvent = ScriptableObject.CreateInstance<CancelTestExecutionEvent>();
            IBattler battler = Substitute.For<IBattler>();
            ITargetSet targets = Substitute.For<ITargetSet>();
            press = Construct(instruction, executionEvent);
            press.OnStart();
            // cancel event fires immediately with this event
            press.OnInput("j", battler, targets);

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

            press = Construct(instruction, executionEvent);
            press.OnStart();
            press.OnInput("j", battler, targets);

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

            press = Construct(instruction, executionEvent);
            press.OnStart();
            // triggers event
            // this should keep cancel time false and set istriggered to true with the given execution event
            press.OnInput("j", battler, targets);
            Assert.IsTrue(press.IsTriggered());
            Assert.IsFalse(press.IsInCancelTime());

            // calling it again with keydown should not trigger event again
            press.OnInput("j", battler, targets);
            TestExecutionEvent test = (TestExecutionEvent)press.GetExecutionEvent();
            Assert.AreEqual(1, test.timesExecuted);
        }
    }
}
