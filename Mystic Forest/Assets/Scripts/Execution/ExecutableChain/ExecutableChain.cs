using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public interface IExecutableChain : IEnumerable<IExecutable>, IDirectionPickable 
{
    // ApproachAnimationData approach { get; }

    //RetreatAnimationData retreat { get; }

    IExecutable head { get; }

    //AttackChainExecutionVisual getVisual(Battler attacker);

}