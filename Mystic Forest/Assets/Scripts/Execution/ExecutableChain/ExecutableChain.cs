using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public interface IExecutableChain : IEnumerable<IExecutable>// , DirectionPickable 
{
    // ApproachAnimationData approach { get; }

    //RetreatAnimationData retreat { get; }

    IExecutable head { get; }

    //AttackChainExecutionVisual getVisual(Battler attacker);

}