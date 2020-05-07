using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Collections;
using System;

[InitializeOnLoad]
[CreateAssetMenu()]
public class ExecutableChainSO : ScriptableObject, IExecutableChain //, ExecutableAttackChain
{
    public ExecutableSO[] attacks;
    public DirectionGroup group;
   
    public IExecutable head => attacks[0];

    public IEnumerator<IExecutable> GetEnumerator()
    {
        return GetCustomizableEnumerator();
    }

    private List<ExecutableSO> instances;
    IEnumerator<IExecutable> LoopEnumerator()
    {
        instances = new List<ExecutableSO>();
        for (int i = 0; i < attacks.Length; i++) instances.Add(Instantiate(attacks[i]));
        return instances.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public IDirectionCommand GetDirectionCommand() => new DirectionCommand(attacks.Length > 0 ? head.GetButton() : DirectionCommandButton.NULL, group.directions);

    public ICustomizableEnumerator<IExecutable> GetCustomizableEnumerator() => new CustomizableEnumerator<IExecutable>(LoopEnumerator());
}