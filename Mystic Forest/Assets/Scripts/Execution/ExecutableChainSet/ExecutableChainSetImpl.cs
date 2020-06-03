using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ExecutableChainSetImpl : IExecutableChainSet
{
    HashSet<IExecutableChain> chains;

    public ExecutableChainSetImpl(IEnumerable<IExecutableChain> chains)
    {
        this.chains = new HashSet<IExecutableChain>(chains);
    }

    public ExecutableChainSetImpl(HashSet<IExecutableChain> chains)
    {
        this.chains = chains;
    }

    public bool Contains(IExecutableChain chain)
    {
        return chains.Contains(chain);
    }

    public IEnumerator<IExecutableChain> GetEnumerator() => chains.GetEnumerator();

    public IExecutableChainSet Where(System.Predicate<IExecutableChain> predicate)
    {
        HashSet<IExecutableChain> copy = new HashSet<IExecutableChain>(chains);
        bool notPredicate(IExecutableChain chain) => !predicate(chain);
        copy.RemoveWhere(notPredicate);
        return new ExecutableChainSetImpl(copy);
    }


    public IExecutableChainSet Union(IExecutableChainSet other)
    {
        HashSet<IExecutableChain> union = new HashSet<IExecutableChain>(chains);
        if (other == null) return this;
        union.UnionWith(other);
        return new ExecutableChainSetImpl(union);
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
