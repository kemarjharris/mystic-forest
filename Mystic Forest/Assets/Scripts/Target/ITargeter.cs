using UnityEngine;
using System.Collections;

public interface ITargeter
{
    ITargetSet targetSet { get; }

    System.Action<Transform> onLockOn { get; set; }
}
