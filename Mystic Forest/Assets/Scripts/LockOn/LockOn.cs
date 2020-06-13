using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class LockOn : MonoBehaviour
{
    List<GameObject> objectsInRange;
    GameObject lockedOn;
    public System.Action onScan;
    public System.Action<GameObject> onLockOn;
    public System.Predicate<Collider> rule { private get;  set; }
    public System.Action onLockedOnExit;
    public System.Action onEnable;

    private void Awake()
    {
        objectsInRange = new List<GameObject>();
    }

    public void OnTriggerEnter(Collider collider)
    {
        if ((rule != null && rule (collider)) || rule == null)
        {
            objectsInRange.Add(collider.gameObject);
        }
        
    }

    public void OnTriggerExit(Collider collider)
    {
        if (collider.gameObject == lockedOn)
        {
            lockedOn = null;
            onLockedOnExit?.Invoke();
        }
        objectsInRange.Remove(collider.gameObject);
    }

    public void Update()
    {
        Debug.Log(CollectionUtils.Print(objectsInRange));
    }

    public void OnEnable() => onEnable?.Invoke();

    public void OnDisable()
    {
        onLockedOnExit?.Invoke();
        objectsInRange.Clear();
        lockedOn = null;
    }

    // Returns the next closest batter to the one currently on the targeted list
    public GameObject NextToLockOnTo()
    {
        objectsInRange.RemoveAll(gameObject => gameObject == null);
        onScan?.Invoke();
        // Add all colliders to objectsInRange of colliders to be targetted
        // return null if no colliders found
        if (objectsInRange.Count <= 0)
        {
            onLockOn?.Invoke(null);
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
        if (lockedOn != null && objectsInRange.Contains(lockedOn))
        {
            // move all colliders that were closer to the last battler (including the last battler) to the back of the list
            // + 1 to include last 
            int lastPos = objectsInRange.IndexOf(lockedOn) + 1;
            List<GameObject> objectsToLoop = objectsInRange.GetRange(0, lastPos);
            objectsInRange.RemoveRange(0, lastPos);
            objectsInRange.AddRange(objectsToLoop);
        }
        lockedOn = objectsInRange[0];
        onLockOn?.Invoke(lockedOn);
        return lockedOn;
    }

}