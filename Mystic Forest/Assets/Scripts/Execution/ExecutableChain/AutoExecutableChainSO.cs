using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Executable/Chain/Auto Executable Chain SO")]
public class AutoExecutableChainSO : ScriptableObject, IAutoExecutableChain
{
    public ExecutionEvent[] attacks;
    public TravelMethodSO travelMethodSO;
    public Vector3 startPositionOffset;
    public float travelSpeed;

    TravelMethodSO IAutoExecutableChain.travelMethod { get => travelMethodSO; }
    Vector3 IAutoExecutableChain.startPositionOffset { get => startPositionOffset; }
    float IAutoExecutableChain.travelSpeed { get => travelSpeed; }

    private List<ExecutionEvent> instances;

    public IEnumerator<ExecutionEvent> GetEnumerator() => LoopEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => LoopEnumerator();

    IEnumerator<ExecutionEvent> LoopEnumerator()
    {
        instances = new List<ExecutionEvent>();
        for (int i = 0; i < attacks.Length; i++)
        {
            instances.Add(attacks[i]);
        }
        return instances.GetEnumerator();
    }
}