using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Zenject;

public class ValueCircle : MonoBehaviour
{
    public Image circle;
    BoundedValue<float> value;


    public virtual void Construct(BoundedValue<float> value)
    {
        this.value = value;
    }

    private void Start()
    {
        circle.type = Image.Type.Filled;
        circle.fillMethod = Image.FillMethod.Radial360;
        circle.fillOrigin = (int) Image.OriginVertical.Top;
        circle.fillClockwise = false;
    }

    protected virtual void Update()
    {
            UpdateValue();   
    }

    private void UpdateValue()
    {
        circle.fillAmount = CalculateValue();
    }

    float CalculateValue()
    {
        return value.Value / value.MaxValue;
    }

}
