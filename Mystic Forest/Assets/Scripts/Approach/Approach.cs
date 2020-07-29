using UnityEngine;
using UnityEditor;
using System;

public class ApproachSO : IApproach
{

    public TravelMethodSO travelMethod;
    public Vector3 offset;
    public float speed;

    public Action OnTransformReached { get; set; }
     

    public void ApproachTransform(Transform transform, Transform destination)
    {
        Vector3 destinationPosition = destination.position + offset;
        travelMethod.Travel(transform, destinationPosition, speed, OnTransformReached);
    }
}