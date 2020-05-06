using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Collections;
using System;

[CreateAssetMenu()]
public class ExecutableChainSO : ScriptableObject, IExecutableChain //, ExecutableAttackChain
{
    public ExecutableSO[] attacks;
    public DirectionGroup group;
    public DirectionCommandButton button;
   

    public IExecutable head => attacks[0];

    public IEnumerator<IExecutable> GetEnumerator()
    {
        return new ExecutableChainEnumerator(LoopEnumerator());
    }

    private List<ExecutableSO> instances;
    IEnumerator<IExecutable> LoopEnumerator()
    {
        if (instances == null)
        {
            instances = new List<ExecutableSO>();
            for (int i = 0; i < attacks.Length; i ++) instances.Add(Instantiate(attacks[i]));
        }
        return instances.GetEnumerator();
    }


    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public IDirectionCommand GetDirectionCommand() => new DirectionCommand(button, group.directions);
}