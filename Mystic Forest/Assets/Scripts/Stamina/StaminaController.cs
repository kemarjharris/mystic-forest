using UnityEngine;
using System.Collections;
using Zenject;
using System.Collections.Generic;

public class StaminaController : MonoBehaviour, IStaminaController
{
    public List<GameObject> observers;
    List<BoundedFloatObserver> floatObservers;
    public BoundedFloat stamina;
    public float restorationPerSecond;
    public bool restore;

    float IStaminaController.stamina => stamina.Value;

    public void Start()
    {
        restore = true;
        if (stamina == null)
        {
            stamina = new BoundedFloat(0, 0, 0);
        }
        floatObservers = new List<BoundedFloatObserver>();
        for (int i = 0; i < observers.Count; i++)
        {
            BoundedFloatObserver bfo = observers[i].GetComponent<BoundedFloatObserver>();
            if (bfo != null)
            {
                floatObservers.Add(bfo);
                bfo.Observe(stamina);
            }
        }
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
        stamina.Value -= value;
    }

    public void StopRestoring()
    {
        restore = false;
    }

    public void StartRestoring() => restore = true;
}
