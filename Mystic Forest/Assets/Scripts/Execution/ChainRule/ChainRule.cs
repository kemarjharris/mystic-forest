using UnityEngine;
using UnityEditor;
using UnityEngine.Events;

public abstract class ChainRule : ScriptableObject
{
    public abstract IExecutableChainSet Rule(IExecutableChainSet set);
}

