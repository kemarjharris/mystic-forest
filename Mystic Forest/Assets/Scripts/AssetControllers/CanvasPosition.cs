using UnityEngine;
using System.Collections;

public class CanvasPosition : MonoBehaviour
{
    Canvas canvas;
    RectTransform canvasRect;
    [Range(0, 1)] public float xPos;
    [Range(0, 1)] public float yPos;
    public bool addToCanvas;
    private Vector3 oldValues;
    Vector3 bottomPoint;

    [ExecuteInEditMode]
    public void Update()
    {
        SetCanvas();


        if (new Vector3(xPos, yPos, canvasRect.rotation.eulerAngles.x) == oldValues) return;
        float tempx = canvas.pixelRect.width * (xPos - canvasRect.pivot.x);
        float tempy = canvas.pixelRect.height * (yPos - canvasRect.pivot.y);
        // calculate point where the z position should be if the canvas is rotated along y axis
        float radians = Mathf.Deg2Rad * canvasRect.rotation.eulerAngles.x;
        float zPosOffset = yPos * Mathf.Tan(radians);
        Vector3 tempPos = canvas.transform.TransformPoint(new Vector3(tempx, tempy, zPosOffset));
        transform.position = tempPos;
        transform.rotation = canvas.transform.rotation;

        oldValues = new Vector3(xPos, yPos, canvasRect.rotation.eulerAngles.x);
    }

    public void SetCanvas()
    {
        if (this.canvas == null)
        {
            Canvas canvas = GetComponentInParent<Canvas>();
            if (canvas != null) this.canvas = canvas;
            else
            {
                canvas = FindObjectOfType<Canvas>();
                if (canvas != null) this.canvas = canvas;
            }
            canvasRect = canvas.GetComponent<RectTransform>();
            if (addToCanvas) transform.SetParent(canvasRect);
        }
    }

}