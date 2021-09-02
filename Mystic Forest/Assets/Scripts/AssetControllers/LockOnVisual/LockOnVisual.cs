using UnityEngine;
using UnityEditor;
using System.Collections;

public class LockOnVisual : MonoBehaviour
{
    public LockOn lockOn;
    public GameObject cursor;
    private GameObject cursorGO;
    public float cursorHeight;
    Transform followTransform;

    private void Awake()
    {
        cursorGO = Instantiate(cursor);
        lockOn.onLockOn += AttachCursor;
        lockOn.onLockedOnExit += DetachCursor;
        cursorGO.transform.SetParent(transform);
        cursorGO.SetActive(false);
    }

    private void LateUpdate()
    {
        if (followTransform == null) return;
        cursorGO.transform.position = followTransform.position + (Vector3.up * cursorHeight);
    }

    private void AttachCursor(GameObject lockedOn)
    {
        if (lockedOn == null) return;
        cursorGO.SetActive(true);
        followTransform = lockedOn.transform;
    }

    private void DetachCursor()
    {
        followTransform = null;
        cursorGO.SetActive(false);
        cursorGO.transform.position = Vector3.zero;
    }

    public void OnDestroy()
    {
        lockOn.onLockOn -= AttachCursor;
        lockOn.onLockedOnExit -= DetachCursor;
    }
}