using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ClosestLockOn : MonoBehaviour
{
    public GameObject lockedOn { get; private set; }
    List<GameObject> inCollider;
    public System.Action onStartScan;
    public System.Action onStopScan;
    public System.Action<GameObject> onLockOn;
    public System.Predicate<Collider> rule { private get; set; }
    public System.Action onLockedOnExit;

    public void Awake()
    {
        inCollider = new List<GameObject>();
        
    }

    public void OnTriggerEnter(Collider collider)
    {
        if ((rule != null && rule(collider)) || rule == null)
        {
            inCollider.Add(collider.gameObject);
        }
    }

    public void Update()
    {
        Debug.Log(CollectionUtils.Print(inCollider));
        inCollider.RemoveAll(gameObject => gameObject == null);
        int sortByDistanceFromCenter(GameObject x, GameObject y)
        {
            float xDistance = Vector3.Distance(x.transform.position, transform.position);
            float yDistance = Vector3.Distance(y.transform.position, transform.position);
            return xDistance.CompareTo(yDistance);
        }
        inCollider.Sort(sortByDistanceFromCenter);
        if (inCollider.Count > 0 && lockedOn != inCollider[0])
        {
            lockedOn = inCollider[0];
            onLockOn?.Invoke(lockedOn);
        }
    }

    public void OnTriggerExit(Collider collider)
    {
        if (collider.gameObject == lockedOn)
        {
            onLockedOnExit?.Invoke();
            lockedOn = null;
        }
        inCollider.Remove(collider.gameObject);
    }

    public void OnEnable()
    {
        lockedOn = null;
        enabled = true;
        onStartScan?.Invoke();
    }

    public void OnDisable()
    {
        inCollider.Clear();
        enabled = false;
        onStopScan?.Invoke();
        onLockedOnExit();
        lockedOn = null;
    }

}
