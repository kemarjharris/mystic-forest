using UnityEngine;
using UnityEditor;

public interface IEXChain : IExecutableChain, ISuper 
{
    IExecutableChain original { get; }
}