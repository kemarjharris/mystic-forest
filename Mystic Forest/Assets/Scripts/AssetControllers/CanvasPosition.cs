using UnityEngine;
using System.Collections;

public class CanvasPosition : MonoBehaviour
{
    Canvas canvas;
    RectTransform canvasRect;
    [Range(0, 1)] public float xPos;
    [Range(0, 1)] public float yPos;
    public bool addToCanvas;
    public bool changeScale = true;
    public bool hardUpdate;
    private Vector3 oldValues;
    Vector3 bottomPoint;
    public CanvasMode canvasMode;


    [ExecuteInEditMode]
    public void LateUpdate()
    {
        SetCanvas();


        if (hardUpdate || new Vector3(xPos, yPos, canvasRect.rotation.eulerAngles.x) != oldValues)
        {
            float tempx = canvasRect.rect.width * (xPos - canvasRect.pivot.x);
            float tempy = canvasRect.rect.height * (yPos - canvasRect.pivot.y);
            // calculate point where the z position should be if the canvas is rotated along y axis
            float radians = Mathf.Deg2Rad * canvasRect.rotation.eulerAngles.x;
            float zPosOffset = yPos * Mathf.Tan(radians);
            Vector3 tempPos = canvas.transform.TransformPoint(new Vector3(tempx, tempy, zPosOffset));
            transform.position = tempPos;
            transform.rotation = canvas.transform.rotation;
            oldValues = new Vector3(xPos, yPos, canvasRect.rotation.eulerAngles.x);
        }
         
    }

    void SetCanvas()
    {
        if (this.canvas == null)
        {
            Canvas canvas = null;
            switch(canvasMode)
            {
                case CanvasMode.MAIN_CANVAS:
                    canvas = GameObject.FindGameObjectWithTag("Main Canvas").GetComponent<Canvas>();
                    break;
                case CanvasMode.CANVAS_IN_PARENT:
                    canvas = GetComponentInParent<Canvas>();
                    break;
                case CanvasMode.FIND_OBJECT_OF_TYPE:
                    canvas = FindObjectOfType<Canvas>();
                    break;
            }
            SetCanvas(canvas);
        }
        
    }

    public void SetCanvas(Canvas canvas)
    {
        this.canvas = canvas;
        canvasRect = canvas.GetComponent<RectTransform>();
        if (addToCanvas) transform.SetParent(canvasRect, changeScale);
    }

    public enum CanvasMode
    {
        MAIN_CANVAS, CANVAS_IN_PARENT, FIND_OBJECT_OF_TYPE
    }

}