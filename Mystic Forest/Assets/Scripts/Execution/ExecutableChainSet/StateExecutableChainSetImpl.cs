using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class StateExecutableChainSetImpl : IExecutableChainSet
{
    IExecutableChainSet set;
    IBattlerPhysics physics;
    ExecutionState state;
    // ISkillCaster skillCaster;

    public StateExecutableChainSetImpl(IBattlerPhysics physics, ExecutionState state, IExecutableChainSet set)
    {
        this.physics = physics;
        this.state = state;
        this.set = set;
        
        // this.skillCaster = skillCaster;
    }

    public bool Contains(IExecutableChain chain)
    {
        if (CorrectState(chain))
        {
            return set.Contains(chain);
        }
        return false;
    }

    public IExecutableChainSet Union(IExecutableChainSet other) => set.Union(other);

    public IExecutableChainSet Where(Predicate<IExecutableChain> predicate)
    {
        return new StateExecutableChainSetImpl(physics, state, set.Where(predicate));
    }

    public IEnumerator<IExecutableChain> GetEnumerator()
    {
        foreach (IExecutableChain chain in set)
        {
            if (CorrectState(chain)) yield return chain;
        }
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    bool CorrectState(IExecutableChain chain)
    {
        // correct aerial state
        if ((chain.IsAerial && !physics.IsGrounded) || (!chain.IsAerial && physics.IsGrounded)) {
            // correct skill state
            if (!chain.IsSkill || state.comboing || (chain.IsSkill && state.selectingSkill))
            //{

                
                    return true;
                
            //}
        }
        return false;
    }

    
}
