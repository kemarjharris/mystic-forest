using UnityEngine;
using UnityEditor;

public interface IExecutionState
{
    CombatState combatState { get; set; }
    bool comboing { get; set; }
    bool selectingSkill { get; set; }
}

public class ExecutionState : IExecutionState
{
    public CombatState combatState { get; set; } = CombatState.NOT_ATTACKING;
    public bool comboing { get; set; }
    public bool selectingSkill { get; set; } 
}