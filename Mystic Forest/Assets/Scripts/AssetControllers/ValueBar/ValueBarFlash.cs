using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ValueBarFlash : MonoBehaviour
{

    public Image fill;
    public Color colourToFlashTo = Color.white;
    Coroutine flash;
    public int timesToFlash = 3;
    public float flashDuration;
    public Color start;

    public void Start()
    {
        start = fill.color;
    }

    public void Flash()
    {
        if (flash != null)
        {
            StopCoroutine(flash);
        }
        flash = StartCoroutine(FlashRoutine());
    }

    IEnumerator FlashRoutine()
    {
        float dur = flashDuration / 2;
        for (int i = 0; i < timesToFlash; i ++)
        {
            for (float t = 0; t <= dur; t += Time.deltaTime)
            {
                fill.color = Color.Lerp(start, colourToFlashTo, t / dur);
                yield return null;
            }
            for (float t = 0; t <= dur; t += Time.deltaTime)
            {
                fill.color = Color.Lerp(colourToFlashTo, start, t / dur);
                yield return null;
            }
            yield return null;
        }
        fill.color = start;
        yield return null;
        flash = null;
    }
}
