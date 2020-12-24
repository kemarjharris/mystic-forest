using UnityEngine;
using UnityEditor;
using System.Collections;

[RequireComponent(typeof(LockOn))]
public class LockOnVisual : MonoBehaviour
{
    LockOn lockOn;
    public GameObject cursor;
    public float cursorHeight;
    Transform followTransform;

    private void Awake()
    {
        lockOn = GetComponent<LockOn>();
        cursor.SetActive(false);
        lockOn.onLockOn += AttachCursor;
        lockOn.onLockedOnExit += DetachCursor;
    }

    private void LateUpdate()
    {
        if (followTransform == null) return;
        cursor.transform.position = followTransform.position + (Vector3.up * cursorHeight);
    }

    private void AttachCursor(GameObject lockedOn)
    {
        if (lockedOn == null) return;
        cursor.SetActive(true);
        followTransform = lockedOn.transform;
    }

    private void DetachCursor()
    {
        followTransform = null;
        cursor.SetActive(false);
        cursor.transform.position = Vector3.zero;
    }

    public void OnDestroy()
    {
        lockOn.onLockOn -= AttachCursor;
        lockOn.onLockedOnExit -= DetachCursor;
    }
}