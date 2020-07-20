using UnityEngine;
using UnityEditor;
using System.Collections;

public class HoldVisual : ExecutionVisual
{
    public SpriteRenderer fullCircle;
    public SpriteRenderer increasingCircle;
    public SpriteRenderer outerCircle;
    public IRoutine fillRoutine;
    public const float fillScale = 1/ 0.65f;

    public void Initialize(OnReleaseHoldExecutable executable)
    {
        executable.onStartHolding = delegate
        {
            FullSize();
            Fill(executable.releaseTime);
        };
        executable.onRelease = delegate
        {
            StopFill();
        };
    }

    public override void MarkFinished()
    {
        base.MarkFinished();
        fullCircle.gameObject.SetActive(false);
        increasingCircle.gameObject.SetActive(false);
        outerCircle.gameObject.SetActive(false);
    }

    public void FullSize()
    {
        StartCoroutine(fullSize());
    }

    IEnumerator fullSize()
    {
        Vector3 outerStartScale = outerCircle.transform.localScale;
        Vector3 fullStartScale = fullCircle.transform.localScale;
        float routineLength = 0.05f;
        float start = Time.unscaledTime;
        float t = 0;
        start = Time.unscaledTime;

        do
        {
            t = (Time.unscaledTime - start) / routineLength;
            fullCircle.transform.localScale = Vector3.Lerp(fullStartScale, fullStartScale * fillScale, t);
            outerCircle.transform.localScale = Vector3.Lerp(outerStartScale, outerStartScale * fillScale, t);
            yield return null;
        } while (t < 1);
    }

    public override void SetColor(Color color)
    {
        base.SetColor(color);
        fullCircle.color = new Color(color.r, color.g, color.b, (float) 45/255);
        increasingCircle.color = new Color(color.r, color.g, color.b, (float) 155 / 255);
        outerCircle.color = color;
    }

    public void Fill(float totalSeconds)
    {
        fillRoutine = new RoutineImpl(fill(totalSeconds), this);
        fillRoutine.Start();

    }

    public void StopFill()
    {
        if (fillRoutine != null)
        {
            fillRoutine.Stop();
            fillRoutine = null;
        }

    }

    IEnumerator fill(float totalSeconds)
    {
        float startTime = Time.unscaledTime;
        for (float time = 0; time < totalSeconds + .2f; time += Time.unscaledDeltaTime)
        {
            float t = (Time.unscaledTime - startTime) / totalSeconds;
            increasingCircle.transform.localScale = 
                Vector3.LerpUnclamped(circle.transform.localScale, fullCircle.transform.localScale, t);
            yield return null;
        }
    }
}