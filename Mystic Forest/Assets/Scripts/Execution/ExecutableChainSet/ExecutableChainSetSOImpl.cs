using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Executable/Executable Chain Set SO")]
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

    public IExecutableChainSet Where(System.Predicate<IExecutableChain> predicate) => set.Where(predicate);

    IEnumerator IEnumerable.GetEnumerator() => set.GetEnumerator();
}
