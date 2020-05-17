using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using NSubstitute;

namespace Tests
{
    public class LoopExecutionEvent
    {
        LoopEvent loop;
        IBattler battler;
        ITargetSet targets;
        CancelTestExecutionEvent cancelEvent;
        TestExecutionEvent testEvent;
        ManualCancelExecutionEvent manualEvent;


        [SetUp]
        public void SetUp()
        {
            loop = ScriptableObject.CreateInstance<LoopEvent>();
            battler = Substitute.For<IBattler>();
            targets = Substitute.For<ITargetSet>();
            cancelEvent = ScriptableObject.CreateInstance<CancelTestExecutionEvent>();
            testEvent = ScriptableObject.CreateInstance<TestExecutionEvent>();
            manualEvent = ScriptableObject.CreateInstance<ManualCancelExecutionEvent>();
            loop.events = new ExecutionEvent[] { cancelEvent, testEvent };
        }


        // not executiong on start
        [Test]
        public void NotExecutingOnStartTest()
        {
            Assert.False(loop.IsExecuting);
        }

        // no fire next on start
        [Test]
        public void NoFireNextOnStartTest()
        {
            Assert.False(loop.FireNext);
        }

        // execution true on first input
        [Test]
        public void ExecutingTrueOnInputTest()
        {
            loop.events = new ExecutionEvent[] { testEvent };
            loop.OnExecute(battler, targets);
            Assert.True(loop.IsExecuting);
        }

        // executing false when cancel fires
        [Test]
        public void ExecutingFalseWhenCancelFiresTest()
        {
            // cancel event set in SetUp
            loop.OnExecute(battler, targets);
            bool result = loop.IsExecuting;
            Assert.False(result);
        }

        // on input if executing fire next set to true
        [Test]
        public void OnInputIfExecutingFireNextSetToTrueTest()
        {
            Assert.False(loop.FireNext);
            loop.IsExecuting = true;
            loop.OnExecute(battler, targets);
            Assert.True(loop.FireNext);
        }

        // event does not fire if fire next is true
        [Test]
        public void DoesNotFireIfFireNextTest()
        {
            loop.events = new ExecutionEvent[] { testEvent };
            loop.FireNext = true;
            loop.OnExecute(battler, targets);
            Assert.AreEqual(0, testEvent.timesExecuted);
        }

        // pos increases when cancel event fires
        [Test]
        public void PosIncreasesWhenCancelEventFiresTest()
        {
            int pos = loop.pos;
            loop.OnExecute(battler,targets);
            Assert.AreEqual(pos + 1, loop.pos);
        }

        // fire next false after execution event fires
        [Test]
        public void FireNextFalseAfterCancelEventFiresTest()
        {
            loop.OnExecute(battler, targets);
            Assert.False(loop.FireNext);
        }

        [Test]
        public void OnInputTwiceSetsFireNextTest()
        {
            loop.events = new ExecutionEvent[] { testEvent };
            loop.OnExecute(battler, targets);
            loop.OnExecute(battler, targets);
            Assert.True(loop.FireNext);
        }

        // when cancellable next fires agian if fire again
        [Test]
        public void FireNextTrueNextFiresAgainWhenCurrCancellableTest()
        {

            loop.events = new ExecutionEvent[] { manualEvent, testEvent };
            loop.OnExecute(battler, targets);
            loop.OnExecute(battler, targets);
            Assert.True(loop.FireNext);
            manualEvent.FireOnCancelEvent();
            Assert.AreEqual(1, testEvent.timesExecuted);
        }

        // when cancellable does not fire again if fire next false
        [Test]
        public void FireNextFalseDoesNotFireNextTest()
        {
            loop.events = new ExecutionEvent[] { manualEvent, testEvent };
            loop.OnExecute(battler, targets);
            Assert.False(loop.FireNext);
            manualEvent.FireOnCancelEvent();
            Assert.AreEqual(0, testEvent.timesExecuted);
        }

        [Test]
        public void FireNextTrueCancelEventFiresSetsExecutingTrueTest()
        {
            loop.events = new ExecutionEvent[] { manualEvent, testEvent };
            loop.OnExecute(battler, targets);
            loop.OnExecute(battler, targets);
            Assert.True(loop.FireNext);
            manualEvent.FireOnCancelEvent();
            Assert.True(loop.IsExecuting);
        }

        [Test]
        public void FiresNextNextTest()
        {
            ManualCancelExecutionEvent @event = ScriptableObject.CreateInstance<ManualCancelExecutionEvent>();
            loop.events = new ExecutionEvent[] { manualEvent, @event, testEvent };
            // set first event to fire
            loop.OnExecute(battler, targets);
            // set next event to fire
            loop.OnExecute(battler, targets);
            // fire first event, next event will fire
            manualEvent.FireOnCancelEvent();
            // next event isnt finished, set to fire again
            loop.OnExecute(battler, targets);
            // fire next event, test event should fire too
            manualEvent.FireOnCancelEvent();
            Assert.AreEqual(1, testEvent.timesExecuted);
        }

        [Test]
        public void DoesNotFireAgainIfInterruptedTest()
        {
            loop.events = new ExecutionEvent[] { manualEvent, testEvent };
            loop.OnExecute(battler, targets);
            loop.OnExecute(battler, targets);
            loop.Interrupt();
            manualEvent.FireOnCancelEvent();
            Assert.AreEqual(1, testEvent.timesExecuted);
            
        }
    }
    
}
