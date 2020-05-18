using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using NSubstitute;

namespace ExecutorTest
{
    public class ChainExecutorLinkImplTest
    {
        [Test]
        public void NoChainDoesNotThrowExceptionTest()
        {
            ChainExecutorLinkImpl executor = new ChainExecutorLinkImpl();
            Assert.DoesNotThrow(delegate {
                executor.Update();
            });
        }

        // ContinueExecution sets executables
        [Test]
        public void ExecuteChainSetsExecutablesTest()
        {
            IExecutable executable = Substitute.For<IExecutable>();
            IEnumerator<IExecutable> executables = new List<IExecutable>( new IExecutable[] { executable }).GetEnumerator();
            ChainExecutorLinkImpl executor = new ChainExecutorLinkImpl();
            executor.ExecuteChain(null, null, executables);
            Assert.AreSame(executables, executor.GetExecutables());
        }

        // Load curr to first executable
        [Test]
        public void ExecuteChainSetsCurrTest()
        {
            IExecutable executable = Substitute.For<IExecutable>();
            IEnumerator<IExecutable> executables = new List<IExecutable>(new IExecutable[] { executable }).GetEnumerator();
            ChainExecutorLinkImpl executor = new ChainExecutorLinkImpl();
            executor.ExecuteChain(null, null, executables);
            Assert.AreSame(executable, executor.GetCurr());
        }

        // Load prev cancellable does not set prev to null
        [Test]
        public void ExecuteChainPrevCancellableDoesNotSetPrevTest()
        {
            IExecutable prev = SetUpExecutableState(true, true, false);
            IExecutable executable = Substitute.For<IExecutable>();
            IEnumerator<IExecutable> executables = new List<IExecutable>(new IExecutable[] { executable }).GetEnumerator();
            ChainExecutorLinkImpl executor = new ChainExecutorLinkImpl();
            executor.Construct(executables, prev, null);
            executor.ExecuteChain(null, null, executables);
            Assert.AreSame(prev, executor.GetPrev());    
        }

        // prev finished sets prev to null
        [Test]
        public void ExecuteChainPrevFinishedSetsPrevToNullTest()
        {
            IExecutable prev = SetUpExecutableState(true, true, true);
            IExecutable executable = Substitute.For<IExecutable>();
            IEnumerator<IExecutable> executables = new List<IExecutable>(new IExecutable[] { executable }).GetEnumerator();
            ChainExecutorLinkImpl executor = new ChainExecutorLinkImpl();
            executor.Construct(executables, prev, null);
            executor.ExecuteChain(null, null, executables);
            Assert.Null(executor.GetPrev());
        }

        // curr on start gets called on load
        [Test]
        public void ExecuteChainCurrOnStartGetsCalledTest()
        {
            bool onStartCalled = false;
            IExecutable executable = Substitute.For<IExecutable>();
            IEnumerator<IExecutable> executables = new List<IExecutable>(new IExecutable[] { executable }).GetEnumerator();
            ChainExecutorLinkImpl executor = new ChainExecutorLinkImpl();
            executable.When(x => x.OnStart()).
                Do(delegate { onStartCalled = true; });
            executor.ExecuteChain(null, null, executables);
            Assert.True(onStartCalled);

        }

        public IExecutable SetUpExecutableState(
            bool isTriggered,
            bool isCancellable,
            bool isFinished)
        {
            IExecutable executable = Substitute.For<IExecutable>();
            executable.IsTriggered().Returns(isTriggered);
            executable.IsInCancelTime().Returns(isCancellable);
            executable.IsFinished().Returns(isFinished);
            return executable;
        }

        public ChainExecutorLinkImpl SetUpExecutorState(
            bool prevIsTriggered,
            bool prevIsCancellable,
            bool prevIsFinished,
            bool currIsTriggered,
            bool currIsCancellable,
            bool currIsFinished)
        {
            IExecutable prev = SetUpExecutableState(prevIsTriggered, prevIsCancellable, prevIsFinished);
            IExecutable curr = SetUpExecutableState(currIsTriggered, currIsCancellable, currIsFinished);
            IEnumerator<IExecutable> executables = new List<IExecutable>(new IExecutable[] { prev, curr }).GetEnumerator();

            ChainExecutorLinkImpl executor = new ChainExecutorLinkImpl();
            executor.Construct(executables, prev, curr);
            return executor;
        }

        public ChainExecutorLinkImpl SetUpExecutorStateForCheckingOnInput(
            bool prevIsTriggered,
            bool prevIsCancellable,
            bool prevIsFinished,
            bool currIsTriggered,
            bool currIsCancellable,
            bool currIsFinished,
            System.Action<NSubstitute.Core.CallInfo> @event)
        {
            ChainExecutorLinkImpl executor =
                SetUpExecutorState(
                    prevIsTriggered,
                    prevIsCancellable,
                    prevIsFinished,
                    currIsTriggered,
                    currIsCancellable,
                    currIsFinished
                );
            executor.GetCurr().WhenForAnyArgs(x => x.OnInput("return", null, null)).
                Do(@event);
            return executor;
        }

        public void AssertMovedNext(ChainExecutorLinkImpl executor, IExecutable oldCurr)
        {
            Assert.AreSame(oldCurr, executor.GetPrev());
        }

        /* prev null curr not triggered */
        // reads input
        [Test]
        public void FirstExecutableCurrNotTriggeredReadsInputTest()
        {
            bool onInputCalled = false;
            IExecutable curr = SetUpExecutableState(false, false, false);
            curr.WhenForAnyArgs(x => x.OnInput("return", null, null)).
                Do(delegate { onInputCalled = true; });
            ChainExecutorLinkImpl executor = new ChainExecutorLinkImpl();
            executor.Construct(
                new List<IExecutable>(new IExecutable[] { curr }).GetEnumerator(),
                null,
                curr);
            executor.Update();
            Assert.True(onInputCalled);
        }

        /* prev null curr triggered */
        // reads input
        [Test]
        public void FirstExecutableCurrTriggeredReadsInputTest()
        {
            bool onInputCalled = false;
            IExecutable curr = SetUpExecutableState(true, false, false);
            curr.WhenForAnyArgs(x => x.OnInput("return", null, null)).
                Do(delegate { onInputCalled = true; });
            ChainExecutorLinkImpl executor = new ChainExecutorLinkImpl();
            executor.Construct(
                new List<IExecutable>(new IExecutable[] { curr }).GetEnumerator(),
                null,
                curr);
            executor.Update();
            Assert.True(onInputCalled);
        }

        /* prev null curr cancellable */
        // Moves to next executable
        [Test]
        public void FirstExecutableCurrCancellableMovesNextTest()
        {
            IExecutable curr = SetUpExecutableState(true, true, false);
            ChainExecutorLinkImpl executor = new ChainExecutorLinkImpl();
            executor.Construct(
                new List<IExecutable>(new IExecutable[] { curr }).GetEnumerator(),
                null,
                curr);
            executor.Update();
            AssertMovedNext(executor, curr);
        }

        /* prev cancellable curr not triggered */
        // reads input 
        [Test]
        public void PrevCancellableCurrNotTriggeredReadsInputTest()
        {
            bool onInputCalled = false;
            ChainExecutorLinkImpl executor = 
                SetUpExecutorStateForCheckingOnInput(true, true, false, false, false, false,
                delegate { onInputCalled = true; });
            executor.Update();
            Assert.True(onInputCalled);
        }

        /* prev cancellable curr triggered curr not cancellable */
        // reads input
        [Test]
        public void PrevCancellableCurrTriggeredReadsInputTest()
        {
            bool onInputCalled = false;
            ChainExecutorLinkImpl executor =
                SetUpExecutorStateForCheckingOnInput(true, true, false, true, false, false,
                delegate { onInputCalled = true; });
            executor.Update();
            Assert.True(onInputCalled);
        }

        /* prev cancellable curr triggered curr cancellable */
        // Moves to next executable
        [Test]
        public void PrevCancellableCurrCancellableCallsMoveNextTest()
        {
            ChainExecutorLinkImpl executor = SetUpExecutorState(true, true, false, true, true, false);
            IExecutable current = executor.GetCurr();
            executor.Update();
            // curent should have moved
            AssertMovedNext(executor, current);
        }

        /* prev finished curr not triggered */
        // stops execution
        [Test]
        public void PrevFinishedCurrNotTriggeredStopsExecutionTest()
        {
            ChainExecutorLinkImpl executor = SetUpExecutorState(true, true, true, false, false, false);
            executor.Update();
            Assert.False(executor.IsExecuting());
        }

        // fires onFinish
        [Test]
        public void PrevFinishedCurrNotTriggeredFiresOnFinishTest()
        {
            bool eventFires = false;
            ChainExecutorLinkImpl executor = SetUpExecutorState(true, true, true, false, false, false);
            executor.onChainFinished = delegate { eventFires = true; };
            executor.Update();
            Assert.True(eventFires);
        }

        [Test]
        public void PrevFinishedCurrNullFiresOnFinishTest()
        {
            bool eventFires = false;
            ChainExecutorLinkImpl executor = new ChainExecutorLinkImpl();
            IExecutable prev = SetUpExecutableState(true, true, true);
            IEnumerator<IExecutable> executables = new List<IExecutable>(new IExecutable[] { prev }).GetEnumerator();
            executor.Construct(executables, prev, null);
            executor.onChainFinished = delegate { eventFires = true; };
            executor.Update();
            Assert.True(eventFires);
        }

        [Test]
        public void PrevFinishedCurrNotTriggeredDoesNotReadInputTest()
        {
            bool onInputCalled = false;
            ChainExecutorLinkImpl executor =
                SetUpExecutorStateForCheckingOnInput(true, true, true, false, false, false,
                delegate { onInputCalled = true; });
            executor.Update();
            Assert.False(onInputCalled);
        }

        [Test]
        public void PrevFinishedCurrNotTriggeredDoesNotCallMoveNextTest()
        {
            ChainExecutorLinkImpl executor = SetUpExecutorState(true, true, true, false, false, false);
            IExecutable current = executor.GetCurr();
            executor.Update();
            // curent should have moved
            Assert.AreNotSame(executor.GetPrev(), current);
        }

        /* prev finished curr triggered */
        // reads input
        [Test]
        public void PrevFinishedCurrTriggeredReadsInput()
        {
            bool onInputCalled = false;
            ChainExecutorLinkImpl executor =
                SetUpExecutorStateForCheckingOnInput(true, true, true, true, false, false,
                delegate { onInputCalled = true; });
            executor.Update();
            Assert.True(onInputCalled);
        }

        /* Next Executable */
        // prev gets set to curr
        [Test]
        public void NextExecutableSetsPrevToCurr()
        {
            ChainExecutorLinkImpl executor = SetUpExecutorState(true, true, false, true, true, false);
            IExecutable curr = executor.GetCurr();
            executor.NextExecutable();
            AssertMovedNext(executor, curr);
        }

        /* NextExecutable -> middle of chain */
        // curr gets set to next
        [Test]
        public void NextExecutableSetsCurrToNextTest()
        {
            IExecutable first = SetUpExecutableState(true, true, false);
            IExecutable next = SetUpExecutableState(true, true, false);
            IEnumerator<IExecutable> executables = new List<IExecutable>(new IExecutable[] { first, next }).GetEnumerator();
            ChainExecutorLinkImpl executor = new ChainExecutorLinkImpl();
            executor.Construct(executables, first, next);
            executor.GetExecutables().MoveNext();
            executor.NextExecutable();
            Assert.AreSame(next, executor.GetCurr());
        }

        // move next with another executable calls on start for that executable
        [Test]
        public void NextExecutableCallsOnStartTest()
        {
            bool onStartCalled = false;
            IExecutable first = SetUpExecutableState(true, true, false);
            IExecutable next = SetUpExecutableState(true, true, false);
            next.When(x => x.OnStart())
                .Do(delegate { onStartCalled = true; });
            IEnumerator<IExecutable> executables = new List<IExecutable>(new IExecutable[] { first, next }).GetEnumerator();
            ChainExecutorLinkImpl executor = new ChainExecutorLinkImpl();
            executor.Construct(executables, first, next);
            executor.GetExecutables().MoveNext();
            executor.NextExecutable();
            Assert.True(onStartCalled);
        }

        /* NextExecutable -> end of chain */
        // cancellable event fires
        [Test]
        public void CurrCancellableEndOfChainFiresOnCancellableTest()
        {
            bool onCancellableFired = false;
            IExecutable curr = SetUpExecutableState(true, true, false);
            IEnumerator<IExecutable> executables = new List<IExecutable>(new IExecutable[] { curr }).GetEnumerator();
            ChainExecutorLinkImpl executor = new ChainExecutorLinkImpl();
            executor.Construct(executables, null, curr);
            executor.GetExecutables().MoveNext();
            executor.onChainCancellable = delegate { onCancellableFired = true; };
            executor.Update();
            Assert.True(onCancellableFired);
        }

        [Test]
        public void CurrFiredNotCancellableDoesNotFireOnCancelTest()
        {
            bool onCancellableFired = false;
            IExecutable curr = SetUpExecutableState(true, false, false);
            curr.HasFired().Returns(true);
            IEnumerator<IExecutable> executables = new List<IExecutable>(new IExecutable[] { curr }).GetEnumerator();
            ChainExecutorLinkImpl executor = new ChainExecutorLinkImpl();
            executor.Construct(executables, null, curr);
            executor.GetExecutables().MoveNext();
            executor.onChainCancellable = delegate { onCancellableFired = true; };
            executor.Update();
            Assert.False(onCancellableFired);

        }
    }
}