using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AttackChainLinker
{
    IDirectionCommandPicker<IExecutableChain> picker;
    IExecutableChainSet set;
    //AttackChainExecutionVisual visual;

    public AttackChainLinker(MonoBehaviour runner)
    {
        picker = new DirectionCommandPicker<IExecutableChain>(0.2f);
    }

    public IExecutableChain Update()
    {
        IExecutableChain chain = picker.InputSelect();
        if (chain != null)
        {
            return chain;
        }
        return null;
    }

    public void Load(IExecutableChainSet set, IBattler attacker)
    {
        this.set = set;
        picker.Set(set);
        //visual = new AttackChainExecutionVisual(set, attacker.getPrefab().transform);
    }
}
