using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public interface IExecutableChain : IEnumerable<IExecutable>, IDirectionPickable 
{

    IExecutable head { get; }

    ICustomizableEnumerator<IExecutable> GetCustomizableEnumerator();

    bool IsAerial { get; }

    bool IsSkill { get; }

    IExecutableChainSet NextChains(IExecutableChainSet executables);
}