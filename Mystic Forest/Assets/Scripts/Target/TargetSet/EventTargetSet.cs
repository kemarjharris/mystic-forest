using UnityEngine;
using System.Collections;

public class EventTargetSet : ITargetSet
{
    Transform target;
    System.Action<Transform> onTargetChanged;

    public EventTargetSet(System.Action<Transform> onTargetChanged = null)
    {
        this.onTargetChanged = onTargetChanged;
    }

    public Transform GetTarget() => target;

    public void SetTarget(Transform target)
    {
        this.target = target;
        onTargetChanged?.Invoke(target);
    }
}
