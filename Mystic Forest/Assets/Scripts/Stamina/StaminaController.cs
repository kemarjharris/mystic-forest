using UnityEngine;
using System.Collections;
using Zenject;
using System.Collections.Generic;

public class StaminaController : MonoBehaviour, IStaminaController
{
    public BoundedValue<float> stamina;
    public float restorationPerSecond;
    public bool restore;
    public bool unlimitedStamina;

    float IStaminaController.stamina => stamina.Value;
    float IStaminaController.maxStamina => stamina.MaxValue;

    [Inject]
    public void Construct(BoundedValue<float> stamina)
    {
        this.stamina = stamina;
    }

    public void Start()
    {
        restore = true;
        stamina.Value = stamina.MaxValue;
    }

    public void Update()
    {
        // for testing
        if (Input.GetKeyDown("f"))
        {
            stamina.Value = stamina.MaxValue;
        }
        if (restore)
        {
            stamina.Value += restorationPerSecond * Time.deltaTime;
        }
    }

    public void DecreaseStamina(float value)
    {
        if (unlimitedStamina) return;
        stamina.Value -= Mathf.Max(0, value);
    }

    public void StopRestoring()
    {
        restore = false;
    }

    public void StartRestoring() => restore = true;
}
