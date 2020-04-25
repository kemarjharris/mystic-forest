

// From https://pastebin.com/j2inUHEU

//****** Donations are greatly appreciated.  ******
//****** You can donate directly to Jesse through paypal at  https://www.paypal.me/JEtzler   ******

using UnityEngine;
using UnityEngine.UI;

class HoverText : MonoBehaviour
{

    public Text text = null;
    GameObject target;		// Object that this label should follow
    public Vector3 offset = Vector3.up;    // Units in world space to offset; 1 unit above object by default
    public bool clampToScreen = false;  // If true, label will be visible even if object is off screen
    float clampBorderSize = .05f;  // How much viewport space to leave at the borders when a label is being clamped
    bool useMainCamera = true;   // Use the camera tagged MainCamera
                                 //var cameraToUse : Camera;	// Only use this if useMainCamera is false
    Camera cam;              //private var cam : Camera;
    Transform thisTransform;
    Transform camTransform;
 
public void Start()
    {
        thisTransform = transform;
        if (useMainCamera)
            cam = Camera.main;
        else
            //cam = cameraToUse;
        camTransform = cam.transform;
    }

    public void Update()
    {
        if (clampToScreen)
        {
            var relativePosition = camTransform.InverseTransformPoint(target.transform.position);
            relativePosition.z = Mathf.Max(relativePosition.z, 1.0f);
            thisTransform.position = cam.WorldToViewportPoint(camTransform.TransformPoint(relativePosition + offset));
            thisTransform.position =  new Vector3(Mathf.Clamp(thisTransform.position.x, clampBorderSize, 1.0f - clampBorderSize),
                                             Mathf.Clamp(thisTransform.position.y, clampBorderSize, 1.0f - clampBorderSize),
                                             thisTransform.position.z);
        }
        else if (target != null)
        {
            //transform.position = target.transform.position + new Vector3(0, 2, 0);
        }
    }

    public void setText(string text)
    {
        this.text.text = text;
    }

    public void setObjectToFollow(GameObject gameObject)
    {
        target = gameObject;
    }
}