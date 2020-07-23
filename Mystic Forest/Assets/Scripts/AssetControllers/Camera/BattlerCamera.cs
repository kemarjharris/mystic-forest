using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Zenject;

public class BattlerCamera : MonoBehaviour
{
    public Vector3 offset = new Vector3(0, 0, -10);
    public Transform followTransform;
    PlayerSwitcher switcher;
    Vector3 velocity = Vector3.zero;
    public float smoothTime = 0.1f;

    [Inject]
    public void Construct(PlayerSwitcher switcher)
    {
        this.switcher = switcher;
    }

    public void Awake()
    {
        switcher.onPlayerSwitched += SwapFollowTransform;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        Vector3 followTransformPosition = new Vector3(followTransform.position.x, transform.position.y, followTransform.position.z) + offset;
        if (Vector3.Distance(transform.position, followTransformPosition) < 1)
        {
            transform.position = followTransformPosition;
        } else
        {
            transform.position = Vector3.SmoothDamp(transform.position, followTransformPosition, ref velocity, smoothTime);
        }
    }

    public void OnDestroy()
    {
        switcher.onPlayerSwitched -= SwapFollowTransform;
    }

    public void SwapFollowTransform(IPlayer player)
    {
        this.followTransform = player.transform;
    }
}
