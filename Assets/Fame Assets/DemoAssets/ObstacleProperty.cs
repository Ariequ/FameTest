using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class ObstacleProperty : MonoBehaviour
{
    bool show = false;
    FameObstacle taskObstacle;
    byte collisionFlag;
    private static ObstacleProperty mSingleton = null;
    public static ObstacleProperty Singleton
    {
        get
        {
            if (mSingleton == null)
            {
                GameObject go = new GameObject("ObstacleProperty");
                go.AddComponent<ObstacleProperty>();
            }
            return mSingleton;
        }
    }

    void Awake()
    {
        mSingleton = this;
    }

    public void Show(int obstacleID)
    {
        taskObstacle = FameManager.GetObstacle(obstacleID);
        collisionFlag = taskObstacle.CollisionFlag;
        show = true;
    }
    Rect rectWindowScreen = new Rect((Screen.width - 270) / 2, (Screen.height - 180) / 2, 270, 170);
    void OnGUI()
    {
        if (!show) return;
        string windowName = "Obstacle Property -  ID: " + taskObstacle.ObstacleID.ToString();
        rectWindowScreen = MyGUIUtility.KeepRectWithinScreen(GUILayout.Window((int)WindowID.obstacleProperty, rectWindowScreen, DrawWindow, new GUIContent(windowName)));

    }
    void DrawWindow(int id)
    {
        GUILayout.Label("Obstacle Type: " + taskObstacle.ObstacleType.ToString());
        GUILayout.BeginHorizontal();
        GUILayout.Label("Obstacle Flag");
        if (GUILayout.Toggle((taskObstacle.CollisionFlag & (byte)FameFlag.Bit7) != 0, "", GUILayout.Width(12)))
            taskObstacle.OnCollisionFlag(FameFlag.Bit7);
        else
            taskObstacle.OffCollisionFlag(FameFlag.Bit7);
        if (GUILayout.Toggle((taskObstacle.CollisionFlag & (byte)FameFlag.Bit6) != 0, "", GUILayout.Width(12)))
            taskObstacle.OnCollisionFlag(FameFlag.Bit6);
        else
            taskObstacle.OffCollisionFlag(FameFlag.Bit6);
        if (GUILayout.Toggle((taskObstacle.CollisionFlag & (byte)FameFlag.Bit5) != 0, "", GUILayout.Width(12)))
            taskObstacle.OnCollisionFlag(FameFlag.Bit5);
        else
            taskObstacle.OffCollisionFlag(FameFlag.Bit5);
        if (GUILayout.Toggle((taskObstacle.CollisionFlag & (byte)FameFlag.Bit4) != 0, "", GUILayout.Width(12)))
            taskObstacle.OnCollisionFlag(FameFlag.Bit4);
        else
            taskObstacle.OffCollisionFlag(FameFlag.Bit4);
        if (GUILayout.Toggle((taskObstacle.CollisionFlag & (byte)FameFlag.Bit3) != 0, "", GUILayout.Width(12)))
            taskObstacle.OnCollisionFlag(FameFlag.Bit3);
        else
            taskObstacle.OffCollisionFlag(FameFlag.Bit3);
        if (GUILayout.Toggle((taskObstacle.CollisionFlag & (byte)FameFlag.Bit2) != 0, "", GUILayout.Width(12)))
            taskObstacle.OnCollisionFlag(FameFlag.Bit2);
        else
            taskObstacle.OffCollisionFlag(FameFlag.Bit2);
        if (GUILayout.Toggle((taskObstacle.CollisionFlag & (byte)FameFlag.Bit1) != 0, "", GUILayout.Width(12)))
            taskObstacle.OnCollisionFlag(FameFlag.Bit1);
        else
            taskObstacle.OffCollisionFlag(FameFlag.Bit1);
        if (GUILayout.Toggle((taskObstacle.CollisionFlag & (byte)FameFlag.Bit0) != 0, "", GUILayout.Width(12)))
            taskObstacle.OnCollisionFlag(FameFlag.Bit0);
        else
            taskObstacle.OffCollisionFlag(FameFlag.Bit0);
        GUILayout.EndHorizontal();
        GUILayout.Space(20);
        GUILayout.BeginHorizontal();
        GUILayout.Space(20);

        if (GUILayout.Button("OK"))
        {
            taskObstacle.ApplyCollisionFlag();
            show = false;
        }
        if (GUILayout.Button("Cancel"))
        {
            taskObstacle.CollisionFlag = collisionFlag;
            show = false;
        }
        GUILayout.Space(20);
        GUILayout.EndHorizontal();
        GUI.DragWindow();
    }

    public void Hide()
    {
        this.show = false;
    }

}
