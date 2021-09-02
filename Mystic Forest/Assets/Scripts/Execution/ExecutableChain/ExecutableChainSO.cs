using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Collections;
using System;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "Executable/Chain/Executable Chain SO")]
public class ExecutableChainSO : BaseChainSO, IExecutableChain
{
    public DirectionGroup group;
    public bool skill;
    public float StaminaCost;

    public override bool IsSkill => skill;

    public override float staminaCost => StaminaCost;

    float IExecutableChain.staminaCost => StaminaCost;

    public override IDirectionCommand GetDirectionCommand() => new DirectionCommand(executables.Length > 0 ? head.GetButton() : DirectionCommandButton.NULL, group.directions);
}