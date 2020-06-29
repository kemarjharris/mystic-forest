using UnityEngine;
using System.Collections;

[RequireComponent(typeof(LockOn))]
public class LockOnScanVisual : MonoBehaviour
{
    LockOn lockOn;
    public SpriteRenderer scan;

    Coroutine scanCoroutine;
    Vector3 scanLocalScale;
    Color scanColour;

    private void Awake()
    {
        lockOn = GetComponent<LockOn>();
        // Turn off sprites
        scan.enabled = false;
        // prepare data for coroutine animation
        scanColour = scan.color;
        // Add scan events to lock on object
        lockOn.onScan += PlayScanAnimation;
        lockOn.onEnable += RecolourCursor;
    }

    private void PlayScanAnimation()
    {
        if (scanCoroutine != null)
        {
            StopCoroutine(scanCoroutine);
        }
        scanCoroutine = StartCoroutine(VisualizeScan());
    }

    private IEnumerator VisualizeScan()
    {
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

    public void RecolourCursor() => scan.enabled = false;

    public void OnDestroy()
    {
        lockOn.onScan -= PlayScanAnimation;
        lockOn.onEnable -= RecolourCursor;
    }
}
