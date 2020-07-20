using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ExpandingButtonMashVisual: ExecutionVisual
{
    public Image timer;

    private IRoutine routine;
    private Vector3 startScale;

    private void Awake()
    {
        startScale = circle.transform.localScale;
    }

    public void Initialize(KeyDownMashExecutable executable)
    {
        executable.onKeyDown = delegate { 
            if (timer.fillAmount >= 1)
            {
                StartTimer(executable.mashDuration);
            }
            ExpandButton();
        };
    }

    public override void SetColor(Color color)
    {
        base.SetColor(color);
        timer.color = new Color(color.r, color.g, color.b, (float)180 / 255);
    }

    public void StartTimer(float seconds)
    {
        StartCoroutine(startTimer(seconds));
    }

    IEnumerator startTimer(float seconds)
    {
        timer.transform.localScale *= 1.65f;
        float start = Time.unscaledTime;
        float t = 1;
        do
        {
            t = (Time.unscaledTime - start) / seconds;
            timer.fillAmount = Mathf.Lerp(0, 1, 1- t);
            yield return null;
        } while (t < 1);
        
    }

    public void ExpandButton()
    {
        if (routine != null)
        {
            routine.Stop();
        }
        circle.transform.localScale = startScale;
        routine = new RoutineImpl(expandButton(circle.gameObject), this);
        routine.Start();
    }

    IEnumerator expandButton(GameObject circle)
    {
        Vector3 newScale = startScale * 1.5f;
        float routineLength = 0.2f;
        float start = Time.unscaledTime;
        float t = 0;
        while (t < 1)
        {

            t = (Time.unscaledTime - start) / routineLength;
            circle.transform.localScale = Vector3.LerpUnclamped(startScale, newScale, 1-t);

            yield return null;
        }

        circle.transform.localScale = startScale;

    }
}
