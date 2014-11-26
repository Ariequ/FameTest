using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public static class TerrainUtil
{
    public static bool GetMouseTerrainPt(Camera camera, out Vector3 point)
    {
        Vector3 screenPosition = Input.mousePosition;
        RaycastHit hit = new RaycastHit();
        if (camera != null)
        {
            Ray ray = camera.ScreenPointToRay(screenPosition);
            if (Physics.Raycast(ray, out hit, 3000f, 1 << 8))
            {
                point = hit.point;
                //Debug.Log(point.ToString());
                return true;
            }
        }
        point = Vector3.zero;
        return false;
    }

    public static bool GetMouse2TerrainPt(Camera camera, out Vector3 point, Vector2 screenPosition)
    {
        RaycastHit hit = new RaycastHit();
        Ray ray = camera.ScreenPointToRay(screenPosition);
        if (Physics.Raycast(ray, out hit, 3000f, 1 << 8))
        {
            point = hit.point;
            return true;
        }
        point = Vector3.zero;
        return false;
    }
}
