using UnityEngine;
using UnityEditor;

[CreateAssetMenu(menuName = "Executable/ExecutableSO/Lock On Executable")]
public class LockOnExecutableSO : ExecutableSO
{
    public GameObject lockOnPrefab;
    public ExecutionEvent onStartLockOn;
    public ExecutionEvent onTargetSelected;
    public float lockOnDuration;

    public override IExecutable CreateExecutable() =>
        new LockOnExecutable
        {
            button = button,
            lockOnPrefab = lockOnPrefab,
            lockOnDuration = lockOnDuration,
            onStartLockOn = Instantiate(onStartLockOn),
            onTargetSelected = Instantiate(onTargetSelected)
        };
}