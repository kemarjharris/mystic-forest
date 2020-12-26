using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Wall : MonoBehaviour
{
    ISet<GameObject> touching = new HashSet<GameObject>();
    new public BoxCollider collider;

    private void Start()
    {
        collider = GetComponent<BoxCollider>();
    }

    public void OnCollisionEnter(Collision collision) => OnTriggerEnter(collision.collider);

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.tag == "Battler")
        {
            touching.Add(collider.gameObject);
        }
    }

    public void OnCollisionExit(Collision collision) => OnTriggerExit(collision.collider);

    public void OnTriggerExit(Collider collider)
    {
        touching.Remove(collider.gameObject);
    }

    

    public bool IsTouching(GameObject gameObject)
    {
        return touching.Contains(gameObject);
    }
}
