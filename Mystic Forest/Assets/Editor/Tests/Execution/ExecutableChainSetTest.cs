using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using NSubstitute;

namespace Tests
{
    public class ExecutableChainSetTest
    {
        IExecutableChainSet set;

        // contains chain
        [Test]
        public void Contains_ChainsExists_ReturnsTrue()
        {
            IExecutableChain chain = Substitute.For<IExecutableChain>();
            IExecutableChain[] chains = new IExecutableChain[] { chain };
            set = new ExecutableChainSetImpl(chains);
            Assert.True(set.Contains(chain));
        }

        // empty does not contain chain
        [Test]
        public void Contains_ChainDoesntExist_ReturnsFalse()
        {
            IExecutableChain chain = Substitute.For<IExecutableChain>();
            IExecutableChain[] chains = new IExecutableChain[] { };
            set = new ExecutableChainSetImpl(chains);
            Assert.False(set.Contains(chain));
        }

        [Test]
        public void Union_MergedSets_ContainsBoth()
        {
            IExecutableChain chainA = Substitute.For<IExecutableChain>();
            IExecutableChainSet setA = new ExecutableChainSetImpl(new IExecutableChain[] { chainA });
            IExecutableChain chainB = Substitute.For<IExecutableChain>();
            IExecutableChainSet setB = new ExecutableChainSetImpl(new IExecutableChain[] { chainB });
            set = setA.Union(setB);
            Assert.True(set.Contains(chainA));
            Assert.True(set.Contains(chainB));
        }

        [Test]
        public void Union_NullSet_DoesNotThrow()
        {
            IExecutableChain chain = Substitute.For<IExecutableChain>();
            IExecutableChain[] chains = new IExecutableChain[] { };
            set = new ExecutableChainSetImpl(chains);
            Assert.DoesNotThrow(() => set.Union(null));
        }

        [Test]
        public void Where_FiltersSet()
        {
            IExecutableChain chainA = Substitute.For<IExecutableChain>();
            IExecutableChain chainB = Substitute.For<IExecutableChain>();
            IExecutableChain[] chains = new IExecutableChain[] { chainA, chainB };
            set = new ExecutableChainSetImpl(chains);
            IExecutableChainSet filteredSet = set.Where((chain) => chain == chainA);
            Assert.True(filteredSet.Contains(chainA));
            Assert.False(filteredSet.Contains(chainB));
        }
    }
}
