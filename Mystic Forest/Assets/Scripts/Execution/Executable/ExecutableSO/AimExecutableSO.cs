using UnityEngine;
using UnityEditor;

[CreateAssetMenu(menuName = "Executable/ExecutableSO/Aim Executable")]
public class AimExecutableSO : ExecutableSO
{
    private void OnEnable()
    {
        cursorPrefab = Resources.Load<GameObject>("Prefabs/Cursor/EnemyCrossHair");
    }

    public GameObject cursorPrefab = null;
    public float aimDuration;
    public ExecutionEvent onStartAiming = null;
    public ExecutionEvent onTargetSelected = null;

    public override IExecutable CreateExecutable() =>
        new AimExecutable
        {
            button = button,
            aimDuration = aimDuration,
            cursorPrefab = cursorPrefab,
            onStartAiming = Instantiate(onStartAiming),
            onTargetSelected = Instantiate(onTargetSelected)
        };
}