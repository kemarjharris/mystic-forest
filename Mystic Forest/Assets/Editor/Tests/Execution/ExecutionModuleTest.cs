﻿using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using NSubstitute;

namespace ExecutionModuleTest
{
    public class ExecutionModuleTest
    {
        protected ExecutionModule module;
        protected IDirectionCommandPicker<IExecutableChain> picker;
        protected ChainExecutorLinkImpl executor;


        [SetUp]
        public virtual void SetUp()
        {
            GameObject go = new GameObject();
            module = go.AddComponent<ExecutionModule>();
            DirectionCommandPicker<IExecutableChain> picker = new DirectionCommandPicker<IExecutableChain>(0);
            picker.Construct(Substitute.For<IUnityTimeService>(), Substitute.For<IUnityInputService>());
            this.picker = picker;
            ChainExecutorLinkImpl executor = new ChainExecutorLinkImpl();
            this.executor = executor;
            module.Construct(this.picker, this.executor);
        }

        [TearDown]
        public void TearDown()
        {
            Object.Destroy(module.gameObject);
        }

        [Test]
        // linking active starts false
        public void LinkerInActiveOnStartTest()
        {
            Assert.False(module.LinkerIsActive());
        }

        [Test]
        public void ExecutorNotExecutingOnStartTest()
        {
            Assert.False(executor.IsExecuting());
        }

        // picker chain selected sets linker inactive
        [Test]
        public void ChainSelectedSetsLinkerInactiveTest()
        {

            IExecutableChain chain = Substitute.For<IExecutableChain>();
            picker.OnSelected.Invoke(chain);
            Assert.False(module.LinkerIsActive());
        }
        
        // Chain cancellable sets linker active
        [Test]
        public void ChainFiredSetsLinkerTrueTest()
        {
            // precondition is linker is inactive
            ChainSelectedSetsLinkerInactiveTest();
            executor.OnChainFired.Invoke();
            Assert.True(module.LinkerIsActive());
        }

        [Test]
        public void StartExecutionSetsLinkerActiveTest()
        {
            module.StartExecution(Substitute.For<IExecutableChainSet>(), Substitute.For<IBattler>());
            Assert.True(module.LinkerIsActive());
        }

        [Test]
        public void StartExecutionKeepsExecutorInactiveTest()
        {
            module.StartExecution(Substitute.For<IExecutableChainSet>(), Substitute.For<IBattler>());
            Assert.False(executor.IsExecuting());
        }

        // Chain Finished Sets linker inactive
        [Test]
        public void ChainFinishedSetsLinkerInactiveTest()
        {
            executor.OnChainFinished.Invoke();
            Assert.False(module.LinkerIsActive());
        }

        [Test]
        public void ChainSelectedStartsExecutionTest()
        {
            IExecutableChain chain = Substitute.For<IExecutableChain>();
            chain.GetEnumerator().Returns(
                new List<IExecutable>
                (
                    new IExecutable[]
                    {
                        Substitute.For<IExecutable>()
                    }
                 ).GetEnumerator()
            );
            picker.OnSelected.Invoke(chain);
            Assert.True(executor.IsExecuting());
        }
    }

    public class StaminaExecutionModuleTest : ExecutionModuleTest
    {
        IStaminaController controller;
        IBattlerEventSet eventSet;


        [SetUp]
        public override void SetUp()
        {
            GameObject go = new GameObject();
            StaminaExecutionModule sModule = go.AddComponent<StaminaExecutionModule>();
            controller = Substitute.For<IStaminaController>();
            controller.stamina.Returns(1);
            eventSet = new BattlerEventSet();

            sModule.Construct(controller, eventSet);
            DirectionCommandPicker<IExecutableChain> picker = new DirectionCommandPicker<IExecutableChain>(0);
            picker.Construct(Substitute.For<IUnityTimeService>(), Substitute.For<IUnityInputService>());
            picker.Set(Substitute.For<IExecutableChainSet>());
            this.picker = picker;
            ChainExecutorLinkImpl executor = new ChainExecutorLinkImpl();
            this.executor = executor;
            module = sModule;
            module.Construct(this.picker, this.executor);
        }

        IExecutableChain SubChain()
        {
            IExecutableChain chain = Substitute.For<IExecutableChain>();
            chain.staminaCost.Returns(1);
            return chain;

        }

        // does not execute when stamina is at 0
        [Test]
        public void ExecuteChain_StaminaIsAtZero_DoesNotExecuteChain()
        {
            controller.stamina.Returns(0);
            IExecutableChain chain = SubChain();
            IChainExecutor executor = Substitute.For<IChainExecutor>();
            module.Construct(picker, executor);
            module.StartExecution(chain, Substitute.For<IBattler>());

            // stamina is 0 so chain shouldnt get executed
            executor.DidNotReceive().ExecuteChain(Arg.Any<IBattler>(), Arg.Any<ITargetSet>(), Arg.Any<IEnumerator<IExecutable>>());
        }

        // deducts chain cost when event executed
        [Test]
        public void ExecutingChain_EventExecuted_CallsDecreaseStamina()
        {
            module.StartExecution(SubChain(), Substitute.For<IBattler>());
            eventSet.onEventExecuted();
            controller.Received().DecreaseStamina(1);
        }

        // stops restoring when event executed
        [Test]
        public void ExecutingChain_EventExecuted_StopsRestoringStamina()
        {
            eventSet.onEventExecuted();
            controller.Received().StopRestoring();
        }

        // starts restoring when becomes inactive
        [Test]
        public void ExecutingChain_BecomeInactive_StartsRestoringStamina()
        {
            eventSet.onPlayerBecomeInactive();
            controller.Received().StartRestoring();
        }
    }
}
