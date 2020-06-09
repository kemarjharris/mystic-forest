using UnityEngine;
using System.Collections;

[RequireComponent(typeof(ClosestLockOn))]
public class ClosestLockOnVisual : MonoBehaviour
{

    ClosestLockOn lockOn;
    public SpriteRenderer scan;
    public SpriteRenderer cursor;
    Color scanColour;
    Coroutine scanCoroutine;
    public float cursorHeight;
    Transform followTransform;

    private void Awake()
    {
        lockOn = GetComponent<ClosestLockOn>();
        // Turn off sprites
        scan.enabled = false;
        cursor.enabled = false;
        // prepare data for coroutine animation
        scanColour = scan.color;
        lockOn.onStartScan += ShowScanArea;
        lockOn.onStopScan += PlayScanFadeAnimation;
        lockOn.onLockOn += AttachCursor;
        lockOn.onLockedOnExit += DetachCursor;
    }

    private void LateUpdate()
    {
        if (followTransform == null) return;
        cursor.transform.position = followTransform.position + (Vector3.up * cursorHeight);
    }

    void ShowScanArea()
    {
        scan.color = scanColour;
        scan.enabled = true;
    }

    private void PlayScanFadeAnimation()
    {
        if (scanCoroutine != null)
        {
            StopCoroutine(scanCoroutine);
        }
        scanCoroutine = StartCoroutine(FadeScan());
    }

    private IEnumerator FadeScan()
    {
        float animationTime = 0.1f;
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

    private void OnDestroy()
    {
        lockOn.onStartScan -= ShowScanArea;
        lockOn.onStopScan -= PlayScanFadeAnimation;
        lockOn.onLockOn -= AttachCursor;
        lockOn.onLockedOnExit -= DetachCursor;
    }
}
