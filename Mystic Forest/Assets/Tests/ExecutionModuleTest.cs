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

        [SetUp]
        public void SetUp()
        {
            GameObject go = new GameObject();
            module = go.AddComponent<ExecutionModule>();
            module.onNewChainSelected = delegate (IExecutableChain chain)
            {
                return Substitute.For<IEnumerator<IExecutable>>();
            };
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
            ChainExecutorLinkImpl executor = new ChainExecutorLinkImpl();
            DirectionCommandPicker<IExecutableChain> picker = new DirectionCommandPicker<IExecutableChain>(0);
            module.Initialize(picker, executor);
            Assert.False(executor.IsExecuting());
        }

        // picker chain selected sets linker inactive
        [Test]
        public void ChainSelectedSetsLinkerInactiveTest()
        {
            DirectionCommandPicker<IExecutableChain> picker = new DirectionCommandPicker<IExecutableChain>(0);
            IExecutableChain chain = Substitute.For<IExecutableChain>();
            IChainExecutor executor = Substitute.For<IChainExecutor>();
            module.Initialize(picker, executor);
            picker.onSelected.Invoke(chain);
            Assert.False(module.LinkerIsActive());
        }
        
        // Chain cancellable sets linker active
        [Test]
        public void ChainCancellableSetsLinkerTrueTest()
        {
            // precondition is linker is inactive
            ChainSelectedSetsLinkerInactiveTest();
            ChainExecutorLinkImpl executor = new ChainExecutorLinkImpl();
            IDirectionCommandPicker<IExecutableChain> picker = Substitute.For<IDirectionCommandPicker<IExecutableChain>>();
            module.Initialize(picker, executor);
            executor.onChainCancellable.Invoke();
            Assert.True(module.LinkerIsActive());
        }

        [Test]
        public void StartExecutionSetsLinkerActiveTest()
        {
            module.StartExecution(Substitute.For<IExecutableChainSet>());
            Assert.True(module.LinkerIsActive());
        }

        [Test]
        public void StartExecutionKeepsExecutorInactiveTest()
        {
            ChainExecutorLinkImpl executor = new ChainExecutorLinkImpl();
            DirectionCommandPicker<IExecutableChain> picker = new DirectionCommandPicker<IExecutableChain>(0);
            module.Initialize(picker, executor);
            module.StartExecution(Substitute.For<IExecutableChainSet>());
            Assert.False(executor.IsExecuting());
        }

        // Chain Finished Sets linker inactive
        [Test]
        public void ChainFinishedSetsLinkerInactiveTest()
        {
            ChainExecutorLinkImpl executor = new ChainExecutorLinkImpl();
            IDirectionCommandPicker<IExecutableChain> picker = Substitute.For<IDirectionCommandPicker<IExecutableChain>>();
            module.Initialize(picker, executor);
            executor.onChainFinished.Invoke();
            Assert.False(module.LinkerIsActive());
        }

        [Test]
        public void ChainSelectedStartsExecutionTest()
        {
            ChainExecutorLinkImpl executor = new ChainExecutorLinkImpl();
            DirectionCommandPicker<IExecutableChain> picker = new DirectionCommandPicker<IExecutableChain>(0);
            module.Initialize(picker, executor);
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
            picker.onSelected.Invoke(chain);
            Assert.True(executor.IsExecuting());
        }
    }
}
