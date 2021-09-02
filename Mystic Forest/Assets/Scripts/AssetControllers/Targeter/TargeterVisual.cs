using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Targeter))]
public class TargeterVisual : MonoBehaviour
{
    IObjectMarker m;
    Targeter t;

    private void Awake()
    {
        t = GetComponent<Targeter>();
        m = GetComponent<IObjectMarker>();
    }

    private void Start()
    {
        t.lockOn.onLockOn += (g) => m.MarkObject(g.transform);
        t.lockOn.onLockedOnExit += m.UnmarkObject;

        t.targetSet.onTargetMarked += m.MarkObject;
    }

    private void OnDestroy()
    {
        t.lockOn.onLockedOnExit -= m.UnmarkObject;
        t.targetSet.onTargetMarked -= m.MarkObject;
    }

}
