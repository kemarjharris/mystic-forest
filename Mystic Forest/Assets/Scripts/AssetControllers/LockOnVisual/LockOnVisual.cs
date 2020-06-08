using UnityEngine;
using UnityEditor;
using System.Collections;

[RequireComponent(typeof(LockOn))]
public class LockOnVisual : MonoBehaviour
{
    LockOn lockOn;
    public SpriteRenderer scan;
    public SpriteRenderer cursor;
    public float cursorHeight;

    Coroutine scanCoroutine;
    Vector3 scanLocalScale;
    Color scanColour;

    Transform followTransform;

    private void Awake()
    {
        lockOn = GetComponent<LockOn>();
        // Turn off sprites
        scan.enabled = false;
        cursor.enabled = false;
        // prepare data for coroutine animation
        scanColour = scan.color;
        // make cursor face camera
        //cursor.transform.rotation = Camera.main.transform.rotation * transform.lossyScale;
        // Add scan events to lock on object
        lockOn.onScan += PlayScanAnimation;
        lockOn.onLockOn += AttachCursor;
        lockOn.onLockedOnExit += DetachCursor;
    }

    private void PlayScanAnimation()
    {
        if (scanCoroutine != null)
        {
            StopCoroutine(scanCoroutine);
        }
        scanCoroutine = StartCoroutine(VisualizeScan());
    }

    private void LateUpdate()
    {
        if (followTransform == null) return;
        cursor.transform.position = followTransform.position + (Vector3.up * cursorHeight);
    }

    private void AttachCursor(GameObject lockedOn)
    {
        if (lockedOn == null) return;
        cursor.enabled = true;
        followTransform = lockedOn.transform;
    }

    private void DetachCursor()
    {
        followTransform = null;
        cursor.enabled = false;
        cursor.transform.position = Vector3.zero;
    }

    private IEnumerator VisualizeScan() {
        float animationTime = 0.2f;
        float timePassed = 0;
        scan.enabled = true;

        scan.color = scanColour;
        Color faded = new Color(scanColour.r, scanColour.g, scanColour.b, 0);
        // expand to scale
        while (timePassed < animationTime)
        {
            timePassed += Time.deltaTime;
            scan.color = Color.Lerp(scanColour, faded, timePassed / animationTime);
            yield return null;
        }

        scan.enabled = false;
    }

    public void OnDestroy()
    {
        lockOn.onScan -= PlayScanAnimation;
        lockOn.onLockOn -= AttachCursor;
        lockOn.onLockedOnExit -= DetachCursor;
    }
}