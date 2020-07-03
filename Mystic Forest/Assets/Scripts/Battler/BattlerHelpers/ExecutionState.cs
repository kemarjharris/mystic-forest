using UnityEngine;
using UnityEditor;

public class ExecutionState
{
    public CombatState combatState = CombatState.NOT_ATTACKING;
    public bool comboing;
    public bool selectingSkill;
}