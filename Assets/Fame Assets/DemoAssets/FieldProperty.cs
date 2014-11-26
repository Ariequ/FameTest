using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class FieldProperty : MonoBehaviour
{
    bool show = false;
    FameField taskfield;
    FameFieldParam taskFieldParamBackup = new FameFieldParam(FameCircularDirection.AntiClockwise, 10, 2, 10, 10, FameFieldType.Circular, 0);

    private static FieldProperty mSingleton = null;
    public static FieldProperty Singleton
    {
        get
        {
            if (mSingleton == null)
            {
                GameObject go = new GameObject("FieldProperty");
                go.AddComponent<FieldProperty>();
            }
            return mSingleton;
        }
    }

    void Awake()
    {
        mSingleton = this;
    }

    public void Show(FameFieldParam param)
    {
        show = true;
    }

    public void Show(int fieldID)
    {
        taskfield = FameManager.GetField(fieldID);
        taskFieldParamBackup.AngleDeg = taskfield.FieldAngleDeg;
        taskFieldParamBackup.Magnitude = taskfield.FieldMagnitude;
        taskFieldParamBackup.FieldDirection = taskfield.CircularFieldDirection;
        taskFieldParamBackup.Width = taskfield.FieldWidthX;
        taskFieldParamBackup.Height = taskfield.FieldWidthZ;
        taskFieldParamBackup.Radius = taskfield.FieldRadius;
        taskFieldParamBackup.FieldType = taskfield.FieldType;
        show = true;
    }
    Rect rectWindowScreen = new Rect((Screen.width - 270) /2 , (Screen.height - 180) / 2, 270, 170);

    void OnGUI()
    {
        if (!show) return;
        string windowName = "Field Property -  ID: " + taskfield.FieldID.ToString();
        rectWindowScreen = MyGUIUtility.KeepRectWithinScreen(GUILayout.Window((int)WindowID.fieldProperty, rectWindowScreen, DrawWindow, new GUIContent(windowName)));

    }
    void DrawWindow(int id){
        GUIContent[] fieldTypeGUIContent = new GUIContent[]{
            new GUIContent(FameFieldType.Uniform.ToString()),
            new GUIContent(FameFieldType.Circular.ToString())
        };
        taskfield.FieldType = (FameFieldType)GUILayout.SelectionGrid((int)taskfield.FieldType, fieldTypeGUIContent, 2);
        if (taskfield.FieldType == FameFieldType.Circular)
        {
            GUIContent[] fieldDir = new GUIContent[]{
                new GUIContent("cw"),
                new GUIContent("ccw"),
                new GUIContent("attract"),
                new GUIContent("repel")
            };
            taskfield.CircularFieldDirection = (FameCircularDirection)GUILayout.SelectionGrid((int)taskfield.CircularFieldDirection, fieldDir, 4);
            GUILayout.BeginHorizontal();
            GUILayout.Label("Magnitude: " + taskfield.FieldMagnitude.ToString("N1"), GUILayout.Width(100));
            taskfield.FieldMagnitude = GUILayout.HorizontalSlider(taskfield.FieldMagnitude, 0, 100);
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.Label("Radius: " + taskfield.FieldRadius.ToString("N1"), GUILayout.Width(100));
            taskfield.FieldRadius = GUILayout.HorizontalSlider(taskfield.FieldRadius, 5, 100);
            GUILayout.EndHorizontal();

        }
        else if (taskfield.FieldType == FameFieldType.Uniform)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("Magnitude: " + taskfield.FieldMagnitude.ToString("N1"), GUILayout.Width(100));
            taskfield.FieldMagnitude = GUILayout.HorizontalSlider(taskfield.FieldMagnitude, 0, 100);
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.Label("Width: " + taskfield.FieldWidthX.ToString("N1"), GUILayout.Width(60));
            taskfield.FieldWidthX = GUILayout.HorizontalSlider(taskfield.FieldWidthX, 5, 100);
            GUILayout.Label("Height: " + taskfield.FieldWidthZ.ToString("N1"), GUILayout.Width(60));
            taskfield.FieldWidthZ = GUILayout.HorizontalSlider(taskfield.FieldWidthZ, 5, 100);
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.Label("Direction: " + taskfield.FieldAngleDeg.ToString("N1"), GUILayout.Width(60));
            taskfield.FieldAngleDeg = GUILayout.HorizontalSlider(taskfield.FieldAngleDeg, 0, 360);
            GUILayout.EndHorizontal();

        }

        GUILayout.BeginHorizontal();
        GUILayout.Label("Field Flag");
        if (GUILayout.Toggle((taskfield.FieldFlag & (byte)FameFlag.Bit7) != 0, "", GUILayout.Width(12)))
            taskfield.OnFieldFlag(FameFlag.Bit7);
        else
            taskfield.OffFieldFlag(FameFlag.Bit7);
        if (GUILayout.Toggle((taskfield.FieldFlag & (byte)FameFlag.Bit6) != 0, "", GUILayout.Width(12)))
            taskfield.OnFieldFlag(FameFlag.Bit6);
        else
            taskfield.OffFieldFlag(FameFlag.Bit6);
        if (GUILayout.Toggle((taskfield.FieldFlag & (byte)FameFlag.Bit5) != 0, "", GUILayout.Width(12)))
            taskfield.OnFieldFlag(FameFlag.Bit5);
        else
            taskfield.OffFieldFlag(FameFlag.Bit5);
        if (GUILayout.Toggle((taskfield.FieldFlag & (byte)FameFlag.Bit4) != 0, "", GUILayout.Width(12)))
            taskfield.OnFieldFlag(FameFlag.Bit4);
        else
            taskfield.OffFieldFlag(FameFlag.Bit4);
        if (GUILayout.Toggle((taskfield.FieldFlag & (byte)FameFlag.Bit3) != 0, "", GUILayout.Width(12)))
            taskfield.OnFieldFlag(FameFlag.Bit3);
        else
            taskfield.OffFieldFlag(FameFlag.Bit3);
        if (GUILayout.Toggle((taskfield.FieldFlag & (byte)FameFlag.Bit2) != 0, "", GUILayout.Width(12)))
            taskfield.OnFieldFlag(FameFlag.Bit2);
        else
            taskfield.OffFieldFlag(FameFlag.Bit2);
        if (GUILayout.Toggle((taskfield.FieldFlag & (byte)FameFlag.Bit1) != 0, "", GUILayout.Width(12)))
            taskfield.OnFieldFlag(FameFlag.Bit1);
        else
            taskfield.OffFieldFlag(FameFlag.Bit1);
        if (GUILayout.Toggle((taskfield.FieldFlag & (byte)FameFlag.Bit0) != 0, "", GUILayout.Width(12)))
            taskfield.OnFieldFlag(FameFlag.Bit0);
        else
            taskfield.OffFieldFlag(FameFlag.Bit0);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Space(20);

        if (GUILayout.Button("OK"))
        {
            taskfield.ApplyFameFieldSetting();
            taskfield.ApplyFieldFlag();
            show = false;
        }
        if (GUILayout.Button("Cancel"))
        {
            taskfield.FieldAngleDeg = taskFieldParamBackup.AngleDeg;
            taskfield.FieldMagnitude = taskFieldParamBackup.Magnitude;
            taskfield.CircularFieldDirection = taskFieldParamBackup.FieldDirection;

            taskfield.FieldWidthX = taskFieldParamBackup.Width;
            taskfield.FieldWidthZ = taskFieldParamBackup.Height;
            taskfield.FieldRadius = taskFieldParamBackup.Radius;
            taskfield.FieldType = taskFieldParamBackup.FieldType;
            taskfield.FieldFlag = taskFieldParamBackup.Flag;
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
    //private static Rect hubRect = new Rect(20, Screen.height - 290, 270, 210);
    public static void DrawPlaceFieldHUD(FameFieldParam taskFieldParam)
    {
        Rect fieldHUDRect = new Rect(20, Screen.height - 290, 270, 210);
        GUILayout.BeginArea(fieldHUDRect);
        GUIContent[] fieldTypeGUIContent = new GUIContent[]{
            new GUIContent(FameFieldType.Uniform.ToString()),
            new GUIContent(FameFieldType.Circular.ToString())
        };
        taskFieldParam.FieldType = (FameFieldType)GUILayout.SelectionGrid((int)taskFieldParam.FieldType, fieldTypeGUIContent, 2);
        if (taskFieldParam.FieldType == FameFieldType.Circular)
        {
            GUIContent[] fieldDir = new GUIContent[]{
                new GUIContent("cw"),
                new GUIContent("ccw"),
                new GUIContent("attract"),
                new GUIContent("repel")
            };
            taskFieldParam.FieldDirection = (FameCircularDirection)GUILayout.SelectionGrid((int)taskFieldParam.FieldDirection, fieldDir, 4);
            GUILayout.BeginHorizontal();
            GUILayout.Label("Magnitude: " + taskFieldParam.Magnitude.ToString("N1"), GUILayout.Width(100));
            taskFieldParam.Magnitude = GUILayout.HorizontalSlider(taskFieldParam.Magnitude, 0, 100);
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.Label("Radius: " + taskFieldParam.Radius.ToString("N1"), GUILayout.Width(100));
            taskFieldParam.Radius = GUILayout.HorizontalSlider(taskFieldParam.Radius, 5, 100);
            GUILayout.EndHorizontal();

        }
        else if (taskFieldParam.FieldType == FameFieldType.Uniform)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("Magnitude: " + taskFieldParam.Magnitude.ToString("N1"), GUILayout.Width(100));
            taskFieldParam.Magnitude = GUILayout.HorizontalSlider(taskFieldParam.Magnitude, 0, 100);
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.Label("Width: " + taskFieldParam.Width.ToString("N1"), GUILayout.Width(60));
            taskFieldParam.Width = GUILayout.HorizontalSlider(taskFieldParam.Width, 5, 100);
            GUILayout.Label("Height: " + taskFieldParam.Height.ToString("N1"), GUILayout.Width(60));
            taskFieldParam.Height = GUILayout.HorizontalSlider(taskFieldParam.Height, 5, 100);
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.Label("Direction: " + taskFieldParam.AngleDeg.ToString("N1"), GUILayout.Width(60));
            taskFieldParam.AngleDeg = GUILayout.HorizontalSlider(taskFieldParam.AngleDeg, 0, 360);
            GUILayout.EndHorizontal();

        }

        GUILayout.BeginHorizontal();
        GUILayout.Label("Field Flag");
        if (GUILayout.Toggle((taskFieldParam.Flag & (byte)FameFlag.Bit7) != 0, "", GUILayout.Width(12)))
            taskFieldParam.OnFlag(FameFlag.Bit7);
        else
            taskFieldParam.OffFlag(FameFlag.Bit7);
        if (GUILayout.Toggle((taskFieldParam.Flag & (byte)FameFlag.Bit6) != 0, "", GUILayout.Width(12)))
            taskFieldParam.OnFlag(FameFlag.Bit6);
        else
            taskFieldParam.OffFlag(FameFlag.Bit6);
        if (GUILayout.Toggle((taskFieldParam.Flag & (byte)FameFlag.Bit5) != 0, "", GUILayout.Width(12)))
            taskFieldParam.OnFlag(FameFlag.Bit5);
        else
            taskFieldParam.OffFlag(FameFlag.Bit5);
        if (GUILayout.Toggle((taskFieldParam.Flag & (byte)FameFlag.Bit4) != 0, "", GUILayout.Width(12)))
            taskFieldParam.OnFlag(FameFlag.Bit4);
        else
            taskFieldParam.OffFlag(FameFlag.Bit4);
        if (GUILayout.Toggle((taskFieldParam.Flag & (byte)FameFlag.Bit3) != 0, "", GUILayout.Width(12)))
            taskFieldParam.OnFlag(FameFlag.Bit3);
        else
            taskFieldParam.OffFlag(FameFlag.Bit3);
        if (GUILayout.Toggle((taskFieldParam.Flag & (byte)FameFlag.Bit2) != 0, "", GUILayout.Width(12)))
            taskFieldParam.OnFlag(FameFlag.Bit2);
        else
            taskFieldParam.OffFlag(FameFlag.Bit2);
        if (GUILayout.Toggle((taskFieldParam.Flag & (byte)FameFlag.Bit1) != 0, "", GUILayout.Width(12)))
            taskFieldParam.OnFlag(FameFlag.Bit1);
        else
            taskFieldParam.OffFlag(FameFlag.Bit1);
        if (GUILayout.Toggle((taskFieldParam.Flag & (byte)FameFlag.Bit0) != 0, "", GUILayout.Width(12)))
            taskFieldParam.OnFlag(FameFlag.Bit0);
        else
            taskFieldParam.OffFlag(FameFlag.Bit0);
        GUILayout.Space(5);
        GUILayout.EndHorizontal();
        GUILayout.EndArea();
    }

}
