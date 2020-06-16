using UnityEngine;
using System.Collections;

public class LockOnTestScene : MonoBehaviour
{
    public GameObject go;
    public LockOn lockOn;

    // Update is called once per frame
    void Update()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        go.transform.position += new Vector3(h, 0, v);

        GameObject t = lockOn.NextToLockOnTo();
        if (t == go)
        {
            Debug.Log("in lock on range");
        } else
        {
            Debug.Log("not in lock on range");
        }
    }
}
