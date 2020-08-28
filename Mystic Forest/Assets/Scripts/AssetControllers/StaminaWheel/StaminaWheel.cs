using UnityEngine;
using System.Collections;
using Zenject;

public class StaminaWheel : MonoBehaviour
{
    public Vector3 offset;
    Transform followTransform;
    public ValueCircle circle;

    protected void LateUpdate()
    {
        transform.rotation = Camera.main.transform.rotation;
        Vector3 viewportPoint = Camera.main.WorldToViewportPoint(followTransform.position + offset);
        transform.position = Camera.main.ViewportToWorldPoint(new Vector3(viewportPoint.x, viewportPoint.y, Camera.main.farClipPlane));
    }


    [Inject]
    public void Construct(BoundedValue<float> stamina, IPlayer player)
    {
        circle.Construct(stamina);
        followTransform = player.transform;
    }
}
