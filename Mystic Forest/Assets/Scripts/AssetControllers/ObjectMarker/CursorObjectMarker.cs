using UnityEngine;
using System.Collections;

public class CursorObjectMarker : MonoBehaviour, IObjectMarker
{

    // public LockOn lockOn;
    public GameObject cursor;
    private GameObject cursorGO;
    public float cursorHeight;
    Transform followTransform;

    private void Awake()
    {
        cursorGO = Instantiate(cursor);
        cursorGO.transform.SetParent(transform);
        cursorGO.SetActive(false);
    }

    private void LateUpdate()
    {
        if (followTransform == null) return;
        cursorGO.transform.position = followTransform.position + (Vector3.up * cursorHeight);
    }

    public void MarkObject(Transform marked)
    {
        if (marked == null) return;
        cursorGO.SetActive(true);
        followTransform = marked.transform;
    }

    public void UnmarkObject()
    {
        followTransform = null;
        cursorGO.SetActive(false);
        cursorGO.transform.position = Vector3.zero;
    }
}
