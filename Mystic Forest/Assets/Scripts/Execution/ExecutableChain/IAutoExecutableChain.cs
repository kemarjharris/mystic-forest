using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public interface IAutoExecutableChain : IEnumerable<ExecutionEvent>
{
    TravelMethodSO travelMethod { get; }
    Vector3 startPositionOffset { get; }
    float travelSpeed { get; }
}