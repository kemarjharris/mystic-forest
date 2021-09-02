using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Executable/Chain/EX Chain SO")]
public class EXChainSO : BaseChainSO, IEXChain
{
    public SuperProperties superProperties;

    public ExecutableChainSO original;

    public override IDirectionCommand GetDirectionCommand() => new DirectionCommand(executables.Length > 0 ? head.GetButton() : DirectionCommandButton.NULL, original.group.directions);

    public float manaCost => superProperties.manaCost;

    public override bool IsSkill => true;

    public override float staminaCost => Mathf.Infinity;

    IExecutableChain IEXChain.original => original;
}