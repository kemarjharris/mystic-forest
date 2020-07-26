using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ValueBar : MonoBehaviour {

    BoundedValue<float> value;
    public Text text;
    public bool showText;
    public Slider slider;

    public void Construct(BoundedValue<float> value)
    {
        this.value = value;
    }

    private void Start()
    {
        if (showText) text.transform.position = new Vector2(transform.position.x, transform.position.y);
        slider.interactable = false;
        text.transform.localPosition = Vector3.zero;
        if (!showText) text.gameObject.SetActive(false);
    }

    private void UpdateValue()
    {
        slider.value = CalculateValue();
    }

    protected virtual void Update()
    {
        UpdateValue();
        
        if (showText) text.text = (int) value.Value + "/" + (int) value.MaxValue;
    }

    float CalculateValue()
    {
        return value.Value / value.MaxValue;
    }
}
