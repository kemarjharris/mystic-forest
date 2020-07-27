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

    public class StateExecutableChainSetTest
    {

        StateExecutableChainSetImpl CreateSet(IExecutableChainSet set, bool grounded, bool comboing, bool selectingSkill)
        {
            IExecutionState state = Substitute.For<IExecutionState>();
            IBattlerPhysics physics = Substitute.For<IBattlerPhysics>();
            physics.IsGrounded.Returns(grounded);
            state.comboing.Returns(comboing);
            state.selectingSkill = selectingSkill;
            return new StateExecutableChainSetImpl(physics, state, set);
        }

        IExecutableChain CreateChain(bool groundChain, bool skillChain)
        {
            IExecutableChain chain = Substitute.For<IExecutableChain>();
            chain.IsAerial.Returns(!groundChain);
            chain.IsSkill.Returns(skillChain);
            return chain;
            
        }

        void TestStateSet(bool inGroundedState, bool inComboingState, bool isSelectingSkill, bool groundChain, bool skillChain, bool assertType)
        {
            IExecutableChain chain = CreateChain(groundChain, skillChain);
            IExecutableChainSet testSet = new ExecutableChainSetImpl(new IExecutableChain[] { chain });
            StateExecutableChainSetImpl set = CreateSet(testSet, inGroundedState, inComboingState, isSelectingSkill);
            if (assertType)
            {
                Assert.True(set.CorrectState(chain));
            }
            else
            {
                Assert.False(set.CorrectState(chain));
            }
        }


        // - grounded, not comboing, not selecting skill
        // gives ground chain
        [Test]
        public void CorrectState_GroundedNotComboingNotSelectingSkill_GivesNormals()
        {
            TestStateSet(true, false, false, true, false, true);
        }

        // does not give air chain
        [Test]
        public void CorrectState_GroundedNotComboingNotSelectingSkill_DoesNotGiveAerials()
        {
            TestStateSet(true, false, false, false, false, false);
        }

        // does not give skill
        [Test]
        public void CorrectState_GroundedNotComboingNotSelectingSkill_DoesNotGiveSkill()
        {
            TestStateSet(true, false, false, true, true, false);
        }

        // does not give ground chain
        [Test]
        public void CorrectState_AirborneNotComboingNotSelectingSkill_DoesNotGiveGroundNormal()
        {
            TestStateSet(false, false, false, true, false, false);
        }

        // gives air chain
        [Test]
        public void CorrectState_AirborneNotComboingNotSelectingSkill_GivesAerial()
        {
            TestStateSet(false, false, false, false, false, true);
        }

        // does not give air skill
        [Test]
        public void CorrectState_AirborneNotComboingNotSelectingSkill_DoesNotGiveAerialSkill()
        {
            TestStateSet(false, false, false, false, true, false);
        }

        // comboing 
        // gives normal chain
        [Test]
        public void CorrectState_GroundedComboingNotSelectingSkill_GivesNormals()
        {
            TestStateSet(true, true, false, true, false, true);
        }

        // gives skills
        [Test]
        public void CorrectState_GroundedComboingNotSelectingSkill_GivesSkills()
        {
            TestStateSet(true, true, false, true, true, true);
        }

        // selecting skill
        // gives normal chain
        [Test]
        public void CorrectState_GroundedNotComboingSelectingSkill_GivesNormals()
        {
            TestStateSet(true, false, true, true, false, true);
        }

        // gives skills
        [Test]
        public void CorrectState_GroundedNotComboingSelectingSkill_GivesSkills()
        {
            TestStateSet(true, false, true, true, true, true);
        }

        [Test]
        public void Where_GivesBackStateSet()
        {
            Assert.IsInstanceOf(typeof(StateExecutableChainSetImpl), 
                new StateExecutableChainSetImpl(
                    Substitute.For<IBattlerPhysics>(), 
                    Substitute.For<IExecutionState>(),
                    Substitute.For<IExecutableChainSet>()
                ).Where((chain) => true));
        }

        [Test]
        public void Union_GivesBackStateSet()
        {
            Assert.IsInstanceOf(typeof(StateExecutableChainSetImpl), 
                new StateExecutableChainSetImpl(Substitute.For<IBattlerPhysics>(),
                Substitute.For<IExecutionState>(),
                Substitute.For<IExecutableChainSet>())
            .Union(Substitute.For<IExecutableChainSet>()));
        }
    }
}
