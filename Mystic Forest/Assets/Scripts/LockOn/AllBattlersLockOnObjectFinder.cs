using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AllBattlersLockOnObjectFinder : MonoBehaviour, ILockOnObjectFinder
{

    List<Collider> battlerColliders;

    private void Start()
    {
        GameObject[] battlers = GameObject.FindGameObjectsWithTag("Battler");
        battlerColliders = new List<Collider>();
        for (int i = 0; i < battlers.Length; i++)
        {
            Collider collider = battlers[i].gameObject.GetComponent<Collider>();
            if (collider != null)
            {
                battlerColliders.Add(collider);
            }
        }
    }


    public List<Collider> ObjectsInRange()
    {
        battlerColliders.RemoveAll((collider) => collider.gameObject == null);
        return battlerColliders;
    }
}
