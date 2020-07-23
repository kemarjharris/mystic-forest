using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class StaminaTestScene : MonoBehaviour
{
    public List<BoundedFloatObserver> o;
    StaminaController c;

    private void Start()
    {
        c = GetComponent<StaminaController>();
        for (int i = 0; i < o.Count; i++)
        {
            o[i].Observe(c.stamina);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown("space")) {
            c.StopRestoring();
        } else if (Input.GetKeyDown("r"))
        {
            c.StartRestoring();
        }
    }
}