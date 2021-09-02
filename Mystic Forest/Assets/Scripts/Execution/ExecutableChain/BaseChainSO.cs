using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

public abstract class BaseChainSO : ScriptableObject, IExecutableChain
{
    public abstract bool IsSkill { get; }
    public abstract float staminaCost { get; }
    public bool aerial;
    public ChainRule rule;

    public bool IsAerial => aerial;

    public IExecutableChainSet NextChains(IExecutableChainSet executables) => rule.Rule(executables);

    public abstract IDirectionCommand GetDirectionCommand();


    public ExecutableSO[] executables;


    private List<IExecutable> instances;

    public virtual IEnumerator<IExecutable> GetEnumerator()
    {
        return GetCustomizableEnumerator();
    }

    IEnumerator<IExecutable> LoopEnumerator()
    {
        instances = new List<IExecutable>();
        for (int i = 0; i < executables.Length; i++)
        {
            if (IsAerial) executables[i].isAerial = true;
            instances.Add(executables[i].CreateExecutable());
        }
        return instances.GetEnumerator();
    }

    public IExecutable head => executables[0].CreateExecutable();

    public virtual ICustomizableEnumerator<IExecutable> GetCustomizableEnumerator()
    {
        return new CustomizableEnumerator<IExecutable>(LoopEnumerator());
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public override string ToString()
    {
        return name.Split('(')[0];
    }
}