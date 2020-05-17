using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Executable/Executable Chain Set SO")]
[UnityEditor.InitializeOnLoad]
public class ExecutableChainSetSOImpl : ScriptableObject, IExecutableChainSet
{
    public List<ExecutableChainSO> chainList;
    IExecutableChainSet set;

    public void OnEnable()
    {
        set = new ExecutableChainSetImpl(chainList);
    }

    public bool Contains(IExecutableChain chain) => set.Contains(chain);

    public IEnumerator<IExecutableChain> GetEnumerator() => set.GetEnumerator();

    public IExecutableChainSet Union(IExecutableChainSet other) => set.Union(other);

    IEnumerator IEnumerable.GetEnumerator() => set.GetEnumerator();
}
