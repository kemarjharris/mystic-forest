using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public interface IExecutableChainSet : IEnumerable<IExecutableChain>
{
    bool Contains(IExecutableChain chain);

    IExecutableChainSet Union(IExecutableChainSet other);

    IExecutableChainSet Where(System.Predicate<IExecutableChain> predicate);
}
