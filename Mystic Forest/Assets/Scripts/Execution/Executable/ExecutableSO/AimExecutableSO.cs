using UnityEngine;
using UnityEditor;

public class AimExecutableSO : ExecutableSO
{
    public GameObject cursorPrefab = Resources.Load<GameObject>("Prefabs/Cursor/EnemyCrossHair");
    public float aimDuration = 0;
    public ExecutionEvent onStartAiming = null;
    public ExecutionEvent onTargetSelected = null;

    public override IExecutable CreateExecutable() =>
        new AimExecutable
        {
            aimDuration = aimDuration,
            cursorPrefab = cursorPrefab,
            onStartAiming = Instantiate(onStartAiming),
            onTargetSelected = Instantiate(onTargetSelected)
        };
}