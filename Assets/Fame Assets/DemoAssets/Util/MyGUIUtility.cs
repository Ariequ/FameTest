using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public static class MyGUIUtility
{
    public static Vector2 MousePoint2ScreenPoint(Vector2 mousePoint)
    {
        return new Vector2(mousePoint.x, Screen.height - mousePoint.y - 1);
    }

    public static bool RectContainsMousePoint(Rect[] rect)
    {
        return rect.Any(r => r.Contains(MousePoint2ScreenPoint(Input.mousePosition)));
    }

    public static bool RectContainsMousePoint(Rect[] rect, Vector3 point)
    {
        return rect.Any(r => r.Contains(MousePoint2ScreenPoint(point)));
    }

    public static bool RectContainsMousePoint(Rect rect)
    {
        return rect.Contains(MousePoint2ScreenPoint(Input.mousePosition));
    }

    public static bool RectContainsMousePoint(Rect rect, Vector2 mousePoint)
    {
        return rect.Contains(MousePoint2ScreenPoint(mousePoint));
    }

    public static bool RectContainsMousePoint(Rect rect, Vector2 mousePoint, Rect refRect)
    {
        Vector2 pointOnScreen = MousePoint2ScreenPoint(mousePoint);
        Vector2 adjustedPoint = new Vector2(pointOnScreen.x - refRect.xMin,
            pointOnScreen.y - refRect.yMin);
        return rect.Contains(adjustedPoint);
    }

    public static Vector2 BoundedMousePosition(Vector2 mousePoint)
    {
        // if mousePoint is outside, convert to the edge point
        if (mousePoint.x < 0)
            mousePoint.x = 0;
        if (mousePoint.x > Screen.width - 1)
            mousePoint.x = Screen.width - 1;
        if (mousePoint.y < 0)
            mousePoint.y = 0;
        if (mousePoint.y > Screen.height - 1)
            mousePoint.y = Screen.height - 1;

        return mousePoint;
    }

    public static Rect KeepRectWithinScreen(Rect r)
    {
        Rect result = new Rect(r);
        if (r.x < 0)
        {
            result.x = 0;
        }
        else if (r.xMax > Screen.width)
        {
            result.x = Screen.width - r.width;
        }
        if (r.yMin < 0)
        {
            result.y = 0;
        }
        else if (r.yMax > Screen.height)
        {
            result.y = Screen.height - r.height;
        }

        return result;
    }
}