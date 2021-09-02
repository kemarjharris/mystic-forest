using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

[RequireComponent(typeof(ILockOnObjectFinder))]
public class LockOn : MonoBehaviour
{
    Transform lockedOn;
    public System.Action onScan;
    public System.Action<GameObject> onLockOn;
    public System.Predicate<Collider> rule { private get;  set; }
    public System.Action onLockedOnExit;
    public System.Action onEnable;
    public ILockOnObjectFinder finder;

    private void Awake()
    {
        finder = GetComponent<ILockOnObjectFinder>();
    }

    public void OnEnable() => onEnable?.Invoke();

    public void OnDisable()
    {
        RemoveTarget();
    }

    public void RemoveTarget()
    {
        onLockedOnExit?.Invoke();
        lockedOn = null;
    }

    public void SetTarget(Transform target)
    {
        lockedOn = target;
        onLockOn?.Invoke(target.gameObject);
    }

    public Transform GetTarget() => lockedOn;

    // Returns the next closest batter to the one currently on the targeted list
    public GameObject NextToLockOnTo()
    {
        List<Collider> objectsInRange = finder.ObjectsInRange();

        // Debug.Log(CollectionUtils.Print(objectsInRange));

        if (rule != null)
        {
            for (int i = objectsInRange.Count - 1; i >= 0; i--)
            {
                if (!rule(objectsInRange[i]))
                {
                    objectsInRange.RemoveAt(i);
                }
            }
        }
        onScan?.Invoke();
        // Add all colliders to objectsInRange of colliders to be targetted
        // return null if no colliders found
        if (objectsInRange.Count <= 0)
        {
            onLockOn?.Invoke(null);
            return null;
        }
        // sort colliders by distance
        int sortByDistanceFromCenter(Collider x, Collider y)
        {

            float xDistance = Vector3.Distance(new Vector3(x.transform.position.x, 0, x.transform.position.z), transform.position);
            float yDistance = Vector3.Distance(new Vector3(y.transform.position.x, 0, y.transform.position.z), transform.position);
            return xDistance.CompareTo(yDistance);
        }
        objectsInRange.Sort(sortByDistanceFromCenter);
        // return first battler since we cant go over the list circularly

        if (lockedOn != null)
        {
            Collider lockedOnCollider = lockedOn.GetComponent<Collider>();
            if (objectsInRange.Contains(lockedOnCollider)) { 
                // move all colliders that were closer to the last battler (including the last battler) to the back of the list
                // + 1 to include last 
                int lastPos = objectsInRange.IndexOf(lockedOnCollider) + 1;
            List<Collider> objectsToLoop = objectsInRange.GetRange(0, lastPos);
            objectsInRange.RemoveRange(0, lastPos);
            objectsInRange.AddRange(objectsToLoop);
            }
        }
        lockedOn = objectsInRange[0].transform;
        onLockOn?.Invoke(lockedOn.gameObject);
        return lockedOn.gameObject;
    }
}