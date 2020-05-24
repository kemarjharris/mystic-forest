using UnityEngine;
using System.Collections;

public class CursorTestScene : MonoBehaviour
{
    public Cursor cursor;
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey("w"))
        {
            cursor.Up();
        }
        if (Input.GetKey("a"))
        {
            cursor.Left();
        }
        if (Input.GetKey("s"))
        {
            cursor.Down();
        }
        if (Input.GetKey("d"))
        {
            cursor.Right();
        }
    }
}
