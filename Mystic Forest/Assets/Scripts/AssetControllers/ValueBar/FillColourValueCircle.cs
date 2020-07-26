using UnityEngine;
using System.Collections;

public class FillColourValueCircle : ValueCircle
{
    Color notFullColor;
    public Color fullColor;
    float lastValue;

    public void Start()
    {
        notFullColor = circle.color;
    }

    protected override void Update()
    {
        lastValue = circle.fillAmount;

        base.Update();

        if (circle.fillAmount > lastValue)
        {
            circle.color = notFullColor;
        }
        else
        {
            circle.color = fullColor;
        }

    }
}
