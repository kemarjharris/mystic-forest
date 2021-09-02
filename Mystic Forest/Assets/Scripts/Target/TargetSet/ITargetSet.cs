using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITargetSet 
{
    void SetTarget(Transform target);

    Transform GetTarget();

    void MarkTarget(Transform target);

    System.Action<Transform> onTargetChanged { get; set; }
    System.Action<Transform> onTargetMarked { get; set; }

}
