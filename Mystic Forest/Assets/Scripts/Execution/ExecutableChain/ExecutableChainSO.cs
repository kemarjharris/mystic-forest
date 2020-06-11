using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Collections;
using System;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "Executable/Executable Chain SO")]
public class ExecutableChainSO : ScriptableObject, IExecutableChain
{
    public ExecutableSO[] attacks;
    public DirectionGroup group;
    public bool aerial;
    public ChainRule rule;


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
        for (int i = 0; i < attacks.Length; i++)
        {
            if (aerial) attacks[i].isAerial = true;
            instances.Add(attacks[i].CreateExecutable());
        }
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

    public IExecutableChainSet NextChains(IExecutableChainSet executables) => rule.Rule(executables);
}