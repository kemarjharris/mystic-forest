using UnityEngine;
using System.Collections;
using Zenject;

public class StaminaWheel : ValueCircle
{
    public Vector3 offset;
    RectTransform canvasTransform;
    new Camera camera;
    CanvasPosition cp;
    Transform followTransform;

    private void Start()
    {
        Canvas canvas = GameObject.FindGameObjectWithTag("Main Canvas").GetComponent<Canvas>();
        canvasTransform = canvas.GetComponent<RectTransform>();
        cp = gameObject.AddComponent<CanvasPosition>();
        cp.SetCanvas(canvas);
        cp.hardUpdate = true;
        camera = Camera.main;
    }

    protected void LateUpdate()
    {
        base.Update();
        transform.rotation = canvasTransform.rotation;
        Vector3 viewportPoint = camera.WorldToViewportPoint(followTransform.position + offset);
        cp.xPos = viewportPoint.x;
        cp.yPos = viewportPoint.y;
        Debug.Log(viewportPoint.x);
    }


    [Inject]
    public void Construct(BoundedValue<float> stamina, IPlayer player)
    {
        base.Construct(stamina);
        followTransform = player.transform;
    }
}
