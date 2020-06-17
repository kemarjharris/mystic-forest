using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DirectionalInput {

    static readonly Direction[] dirs = EnumUtil.toArray<Direction>();
    public static IUnityAxisService service = new UnityAxisService();
    public static IUnityTimeService timeService = new UnityTimeService();
    private static Axis horizontal = new Axis();
    private static Axis vertical = new Axis();
    public static float liveRange = 0.2f;
    

    private struct Axis
    {
        public float value;
        public int lastFramePressed;
        public bool pressed;
    }

    private static float InputOnAxis(string axis, float deadzone) {
        float axisValue = service.GetAxisRaw(axis);
        if (Mathf.Abs(axisValue) > deadzone) return axisValue;
        return 0;
    }

    private static bool AnyInput(float deadzone) => Mathf.Abs(InputOnAxis("Horizontal", deadzone)) > 0 || Mathf.Abs(InputOnAxis("Vertical", deadzone)) > 0;

    private static float Angle()
    {
        if (!AnyInput(0.2f)) return -1;
        float yAxis = InputOnAxis("Vertical", 0.2f);
        float xAxis = InputOnAxis("Horizontal", 0.2f);

        float angle = Vector2.Angle(Vector2.up, new Vector2(xAxis, yAxis));

        if (xAxis < 0)
        {
            angle = 360 - angle;
        }
        return angle;
    }

    public static float GetAxisDown(string axisName)
    {
        float CalculateAxis(ref Axis axis)
        {
            // assume axis hasnt been pressed
            float result = 0;
            // if the axis wasnt already being pressed down, change the value of result
            axis.value = InputOnAxis(axisName, liveRange);
            if (!axis.pressed)
            {
                result = axis.value;
            }

            // trigger flag for frame if on new frame
            if (axis.lastFramePressed != timeService.frameCount)
            {
                // axis.pressed is if the axis was pressed this frame
                axis.pressed = Mathf.Abs(axis.value) > liveRange;
                // save value of last check 
                axis.lastFramePressed = timeService.frameCount;
            }

            return result;
        }

        if (axisName == "Horizontal")
        {
            return CalculateAxis(ref horizontal);
        }
        else if (axisName == "Vertical")
        {
            return CalculateAxis(ref vertical);
        } else
        {
            Debug.LogWarning("Unsupported axis " + axisName);
            return 0;
        }
    }

    public static Direction GetSimpleDirection()
    {
        float angle = Angle();
        int posn;
        Direction dir;
        if (angle < 0)
        {
            posn = -1;
            dir = Direction.NULL;

        }
        else if (angle > 360 - (360 / 8) || angle < (360 / 8))
        {
            posn = 0;
            dir = Direction.N;
        }
        else
        {
            angle -= (360 / 8);
            posn = Mathf.FloorToInt((angle / (360 / 4))) + 1;
            dir = dirs[posn * 2]; 
        }
        return dir;
    }

    /* for testing */
    public static void Reset()
    {
        horizontal = new Axis();
        vertical = new Axis();
    }

}
