using UnityEngine;
using UnityEditor;

[System.Serializable]
public class SuperProperties : ISuper
{
    public float manaCost;

    float ISuper.manaCost => this.manaCost;
}