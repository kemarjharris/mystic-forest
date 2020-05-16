using UnityEngine;
using UnityEditor;

public abstract class ExecutableSO : ScriptableObject
{
    public DirectionCommandButton button;

    public abstract IExecutable CreateExecutable();

}