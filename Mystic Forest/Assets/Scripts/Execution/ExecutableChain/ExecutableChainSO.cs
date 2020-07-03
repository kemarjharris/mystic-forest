using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Collections;
using System;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "Executable/Chain/Executable Chain SO")]
public class ExecutableChainSO : ScriptableObject, IExecutableChain
{
    public ExecutableSO[] attacks;
    public DirectionGroup group;
    public bool aerial;
    public bool skill;
    public ChainRule rule;


    private List<IExecutable> instances;

    public virtual IExecutable head => attacks[0].CreateExecutable();

    public virtual bool IsAerial => aerial;

    public virtual bool IsSkill => skill;

    public virtual IEnumerator<IExecutable> GetEnumerator()
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

    public virtual IDirectionCommand GetDirectionCommand() => new DirectionCommand(attacks.Length > 0 ? head.GetButton() : DirectionCommandButton.NULL, group.directions);

    public virtual ICustomizableEnumerator<IExecutable> GetCustomizableEnumerator()
    {
        return new CustomizableEnumerator<IExecutable>(LoopEnumerator());
    }

    public virtual IExecutableChainSet NextChains(IExecutableChainSet executables) => rule.Rule(executables);

    public override string ToString()
    {
        return name.Split('(')[0];
    }

    IEnumerator IEnumerable.GetEnumerator() => LoopEnumerator();
}