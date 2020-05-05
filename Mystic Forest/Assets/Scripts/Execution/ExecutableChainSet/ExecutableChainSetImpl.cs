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

    public IExecutableChainSet Union(IExecutableChainSet other)
    {
        HashSet<IExecutableChain> union = new HashSet<IExecutableChain>(chains);
        union.UnionWith(other);
        return new ExecutableChainSetImpl(union);
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
