using UnityEngine;
using System.Collections;

public class EventTargetSet : ITargetSet
{
    Transform target;
    public System.Action<Transform> onTargetChanged { get; set; }
    public System.Action<Transform> onTargetMarked { get; set; }

    public Transform GetTarget() => target;

    public void SetTarget(Transform target)
    {
        this.target = target;
        onTargetChanged?.Invoke(target);
    }

    public void MarkTarget(Transform target) => onTargetMarked?.Invoke(target);
 
}
