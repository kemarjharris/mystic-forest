using UnityEngine;
using UnityEditor;
using Zenject;

public interface IExecutionState
{
    CombatState combatState { get; set; }
    bool comboing { get; set; }
    bool selectingSkill { get; set; }
}

public class ExecutionState : IExecutionState
{
    IBattlerEventSet eventSet;
    CombatState combatState;
    bool comboing;

    //[Inject]
    public ExecutionState(IBattlerEventSet eventSet)
    {
        this.eventSet = eventSet;
    }

    CombatState IExecutionState.combatState { get =>combatState;
        set
        {
            bool wasAttacking = combatState != CombatState.NOT_ATTACKING;
            // change to not attacking while not comboing
            if (wasAttacking && value == CombatState.NOT_ATTACKING && comboing == false)
            {
               eventSet.onPlayerBecomeInactive?.Invoke();
            }
            combatState = value;
        }
    } 

    bool IExecutionState.comboing { get => comboing;
        set
        {
            bool wasComboing = comboing;
            // finish a combo and youre not attacking
            if (wasComboing && !value && combatState == CombatState.NOT_ATTACKING)
            {
                eventSet.onPlayerBecomeInactive?.Invoke();
            }
            comboing = value;
        }
    }
    public bool selectingSkill { get; set; } 
}