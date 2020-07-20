using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class CanvasPosition : MonoBehaviour
{
    public Canvas canvas;
    public RectTransform canvasRect;
    [Range(0, 1)] public float xPos;
    [Range(0, 1)] public float yPos;
    private Vector3 oldValues;
    Vector3 bottomPoint;

    private void Awake()
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
        }
        canvasRect = canvas.GetComponent<RectTransform>();
    }

    public void Update()
    {

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

}