using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using NSubstitute;

namespace ExecutionModuleTest
{
    public class ExecutionModuleTest
    {
        ExecutionModule module;
        IDirectionCommandPicker<IExecutableChain> picker;
        ChainExecutorLinkImpl executor;


        [SetUp]
        public void SetUp()
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
}
