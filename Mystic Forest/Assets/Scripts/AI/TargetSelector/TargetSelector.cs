using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using Zenject;


public abstract class TargetSelector : ScriptableObject
{
    public abstract ITargetSet SelectTarget(List<IPlayer> targets);
}