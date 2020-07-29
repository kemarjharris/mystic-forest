using UnityEngine;
using UnityEditor;

public interface IApproach
{
    void ApproachTransform(Transform transform, Transform destination);

    System.Action OnTransformReached { get; set; }
}