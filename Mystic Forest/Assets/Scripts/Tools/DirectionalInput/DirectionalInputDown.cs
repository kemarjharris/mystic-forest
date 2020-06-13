using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DirectionalInputDown
{

    static float horizontal;
    static float vertical;
    static float jhorizontal;
    static float jvertical;
    static float liveRange = 0.5f;
    static int hframe;
    static int vframe;
    static bool vAxisPressed;
    static bool hAxisPressed;

    public void setLiveRange(float range)
    {
        liveRange = range;
    }

    public static bool AnyInput(float deadzone)
    {
        return InputOnAxis(true, deadzone) != 0 || InputOnAxis(false, deadzone) != 0;
    }

    public static int InputOnAxisDown(bool horizontally)
    {
        int result = 0;
        if (horizontally)
        {
            if (!hAxisPressed)
            {
                result = InputOnAxis(horizontally);
            }
            triggerHorizontalPlessFlag();
        }
        else
        {
            if (!vAxisPressed)
            {
                result = InputOnAxis(horizontally);
            }
            triggerVerticalPressFlag();
        }

        return result;
    }

    public static int InputOnAxis(bool horizontally)
    {
        return InputOnAxis(horizontally, liveRange);
    }

    public static int InputOnAxis(bool horizontally, float liveRange)
    {
        SetAllAxes();
        if (horizontally)
        {
            if (horizontal > 0 || jhorizontal >= liveRange)
            {
                return 1;
            }
            else if (horizontal < 0 || jhorizontal <= -liveRange)
            {
                return -1;
            }
            else
            {
                return 0;
            }
        }
        else
        {
            if (vertical > 0 || jvertical >= liveRange)
            {
                return 1;
            }
            else if (vertical < 0 || jvertical <= -liveRange)
            {
                return -1;
            }
            else
            {
                return 0;
            }
        }
    }

    public static float Angle()
    {
        SetAllAxes();

        if (AnyInput(0.2f))
        {
            float angle = -1;

            if (InputOn(true, 0.2f))
            {
                angle = Vector2.Angle(Vector2.up, new Vector2(horizontal, vertical));
            }
            else if (InputOn(false, 0.2f))
            {
                angle = Vector2.Angle(Vector2.up, new Vector2(jhorizontal, jvertical));
            }

            if (InputOnAxis(true, 0.2f) < 0)
            {
                angle = 360 - angle;
            }
            return angle;
        }
        else
        {
            return -1;
        }
    }

    public static bool InputOn(bool keys, float deadzone)
    {

        if (keys)
        {
            SetKeyAxes();
            return (Mathf.Abs(horizontal) > deadzone || Mathf.Abs(vertical) > deadzone);
        }
        else
        {
            SetJoystickAxes();
            return (Mathf.Abs(jhorizontal) > deadzone || Mathf.Abs(jvertical) > deadzone);
        }
    }

    private static void SetHorizontalAxis()
    {
        horizontal = Input.GetAxisRaw("Horizontal");
        //jhorizontal = Input.GetAxis("JoystickHorizontalLeft");

    }

    private static void SetVerticalAxis()
    {
        vertical = Input.GetAxisRaw("Vertical");
        //jvertical = Input.GetAxis("JoystickVerticalLeft");
    }

    private static void SetKeyAxes()
    {
        horizontal = Input.GetAxisRaw("Horizontal");
        vertical = Input.GetAxisRaw("Vertical");
    }

    private static void SetJoystickAxes()
    {
        //jhorizontal = Input.GetAxis("JoystickHorizontalLeft");
        //jvertical = Input.GetAxis("JoystickVerticalLeft");
    }

    private static void SetAllAxes()
    {
        SetHorizontalAxis();
        SetVerticalAxis();
    }

    private static void triggerHorizontalPlessFlag()
    {
        if (hframe != Time.frameCount)
        {
            hAxisPressed = Mathf.Abs(horizontal) > liveRange || Mathf.Abs(jhorizontal) > liveRange;
            hframe = Time.frameCount;
            SetHorizontalAxis();
        }
    }

    private static void triggerVerticalPressFlag()
    {
        if (vframe != Time.frameCount)
        {
            vAxisPressed = Mathf.Abs(vertical) > liveRange || Mathf.Abs(jvertical) > liveRange;
            vframe = Time.frameCount;
            SetVerticalAxis();
        }
    }
}
