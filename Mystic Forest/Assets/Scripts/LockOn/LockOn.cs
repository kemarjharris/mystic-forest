using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class LockOn : MonoBehaviour
{
    List<GameObject> objectsInRange;
    private static IHitBox hitBox;
    public System.Predicate<Collider> rule { private get;  set; }

    private void Awake()
    {
        hitBox = GetComponent<IHitBox>();
    }

    // Returns the next closest batter to the one currently on the targeted list
    public GameObject NextToLockOnTo()
    {
        Debug.Log("pa a fekkuto");
        // Create a new list of colliders
        GameObject last = null;
        if (objectsInRange != null && objectsInRange.Count > 0)
        {
            last = objectsInRange[0];
        }
        objectsInRange = new List<GameObject>();
        // Add all colliders to objectsInRange of colliders to be targetted
        void lockOnToBattler(Collider collider)
        {
            if ((rule != null && rule(collider)) || rule == null)
            {
                objectsInRange.Add(collider.gameObject);
            }
        }
        hitBox.CheckCollision(lockOnToBattler);
        // return null if no colliders found
        if (objectsInRange.Count <= 0)
        {
            return null;
        }
        // sort colliders by distance
        int sortByDistanceFromCenter(GameObject x, GameObject y)
        {
            float xDistance = Vector3.Distance(x.transform.position, transform.position);
            float yDistance = Vector3.Distance(y.transform.position, transform.position);
            return xDistance.CompareTo(yDistance);
        }
        objectsInRange.Sort(sortByDistanceFromCenter);
        // return first battler since we cant go over the list circularly
        if (last != null && objectsInRange.Contains(last))
        {
            // move all colliders that were closer to the last battler (including the last battler) to the back of the list
            // + 1 to include last 
            int lastPos = objectsInRange.IndexOf(last) + 1;
            List<GameObject> objectsToLoop = objectsInRange.GetRange(0, lastPos);
            objectsInRange.RemoveRange(0, lastPos);
            objectsInRange.AddRange(objectsToLoop);
        }
        return objectsInRange[0];
    }

}