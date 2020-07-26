using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class FillColourValueBar : ValueBar
{
    public Image fill;
    Color notFullColor;
    public Color fullColor;
    float lastValue;

    public void Start()
    {
        notFullColor = fill.color;
    }

    protected override void Update()
    {
        lastValue = slider.value;

        base.Update();

        if (slider.value > lastValue)
        {
            fill.color = notFullColor;
        } else
        {
            fill.color = fullColor;
        }

    }
}
