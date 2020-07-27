using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using NSubstitute;

namespace Tests
{
    public class ExecutionStateTests
    {
        IExecutionState state;
        IBattlerEventSet eventSet;
        bool fired;

        [SetUp]
        public void SetUp()
        {
            eventSet = new BattlerEventSet();
            state = new ExecutionState(eventSet);
            fired = false;
            eventSet.onPlayerBecomeInactive += ()  => fired = true;
        }

        // comboing and executing to not executing dnfe
        [Test]
        public void SetCombatState_ComboingExecutingToNotExecuting_DoesNotFireInactiveEvent()
        {
            state.comboing = true;
            state.combatState = CombatState.ATTACKING;
            // swap to not executing
            state.combatState = CombatState.NOT_ATTACKING;
            // still comboing, so character is not inactive
            Assert.False(fired);
        }

        // not comboing and executing to not executing fe
        [Test]
        public void SetCombatState_NotComboingExecutingToNotExecuting_FiresInactiveEvent()
        {
            state.comboing = false;
            state.combatState = CombatState.ATTACKING;
            // swap to not executing
            state.combatState = CombatState.NOT_ATTACKING;
            // still comboing, so character is not inactive
            Assert.True(fired);
        }

        // executing and comboing to not comboing dnfe
        [Test]
        public void SetComboing_ExecutingComboingToNotComboing_DoesNotFireInactiveEvent()
        {
            state.combatState = CombatState.ATTACKING;
            state.comboing = true;
            // swap to false, character is not inactive
            state.comboing = false;
            Assert.False(fired);
        }

        // not executing and comboing to not combing fe
        [Test]
        public void SetComboing_NotExecutingComboingToNotComboing_FiresInactiveEvent()
        {
            state.combatState = CombatState.NOT_ATTACKING;
            state.comboing = true;
            // swap to false, character is not inactive
            state.comboing = false;
            Assert.True(fired);
        }
    }
}
