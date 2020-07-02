using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class StateExecutableChainSetImpl : IExecutableChainSet
{
    IExecutableChainSet set;
    IBattlerPhysics physics;
    bool usingAerials;

    public StateExecutableChainSetImpl(IBattlerPhysics physics, IExecutableChainSet set)
    {
        this.physics = physics;
        this.set = set;
    }

    IExecutableChainSet Aerials()
    {
        // Aerial sets
        bool IsAerial(IExecutableChain chain) => chain.IsAerial && !chain.IsSkill;
        return set.Where(IsAerial);
    }
    IExecutableChainSet Normals()
    {
        bool isNormal(IExecutableChain chain) => !chain.IsAerial && !chain.IsSkill;
        return set.Where(isNormal);
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
        IExecutableChainSet tempSet = physics.IsGrounded ? Normals() : Aerials();
        return tempSet.Where(predicate);
    }

    public IEnumerator<IExecutableChain> GetEnumerator()
    {
        bool grounded = physics.IsGrounded;
        foreach (IExecutableChain chain in set)
        {
            if (CorrectState(chain)) yield return chain;
        }
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    bool CorrectState(IExecutableChain chain) => (chain.IsAerial && !physics.IsGrounded) || (!chain.IsAerial && physics.IsGrounded);

    
}
