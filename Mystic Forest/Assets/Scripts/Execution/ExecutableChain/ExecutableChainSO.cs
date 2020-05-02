using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Collections;
using System;

[CreateAssetMenu()]
public class ExecutableChainSO : ScriptableObject, IExecutableChain //, ExecutableAttackChain
{
    // Amount of time in seconds the chain takes to execute
    public ExecutableSO[] attacks;

    public IExecutable head => attacks[0];

    public IEnumerator<IExecutable> GetEnumerator()
    {
        return new ExecutableChainEnumerator(LoopEnumerator());
    }

    IEnumerator<IExecutable> LoopEnumerator()
    {
        for (int i = 0; i < attacks.Length; i++) yield return Instantiate(attacks[i]);
    }


    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}