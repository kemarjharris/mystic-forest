using UnityEngine;
using UnityEditor;

public class AllChainRule : ChainRule
{
    public override IExecutableChainSet Rule(IExecutableChainSet set)
    {
        return set.Where(chain => true);
    }
}