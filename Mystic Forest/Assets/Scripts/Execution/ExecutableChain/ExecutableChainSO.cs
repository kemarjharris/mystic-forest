using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Collections;
using System;

[CreateAssetMenu(menuName = "Executable/Executable Chain SO")]
public class ExecutableChainSO : ScriptableObject, IExecutableChain //, ExecutableAttackChain
{
    public ExecutableSO[] attacks;
    public DirectionGroup group;
    public bool aerial;


    private List<IExecutable> instances;

    public IExecutable head => attacks[0].CreateExecutable();

    public bool IsAerial => aerial;

    public IEnumerator<IExecutable> GetEnumerator()
    {
        return GetCustomizableEnumerator();
    }
    
    IEnumerator<IExecutable> LoopEnumerator()
    {
        instances = new List<IExecutable>();
        for (int i = 0; i < attacks.Length; i++) instances.Add(attacks[i].CreateExecutable());
        return instances.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public IDirectionCommand GetDirectionCommand() => new DirectionCommand(attacks.Length > 0 ? head.GetButton() : DirectionCommandButton.NULL, group.directions);

    public ICustomizableEnumerator<IExecutable> GetCustomizableEnumerator()
    {
        return new CustomizableEnumerator<IExecutable>(LoopEnumerator());
    }
}