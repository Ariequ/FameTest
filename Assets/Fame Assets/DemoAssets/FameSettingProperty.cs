using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class FameSettingProperty : MonoBehaviour
{
    bool show = false;

    private static FameSettingProperty mSingleton = null;
    public static FameSettingProperty Singleton
    {
        get
        {
            if (mSingleton == null)
            {
                GameObject go = new GameObject("FameSettingProperty");
                go.AddComponent<FameSettingProperty>();
            }
            return mSingleton;
        }
    }

    void Awake()
    {
        mSingleton = this;
    }

    public void Show()
    {

        show = true;
        EnableTerrainEffect = FameCSharp.FameSettings.ApplyTerrainEffect;
        IgnoreMinSpeed = FameCSharp.FameSettings.ignoreMinSpeed;
        AccelerationThreshold = FameCSharp.FameSettings.accelerationThreshold;
        SlowingDistance = FameCSharp.FameSettings.slowingDistance;
        SwitchAgentPosition = FameCSharp.FameSettings.SwitchAgentPosition;
        WrapTerrain = FameCSharp.FameSettings.WrapTerrain;
    }
    Rect rectWindowScreen = new Rect((Screen.width - 350) / 2, (Screen.height - 250) / 2, 350, 250);
    void OnGUI()
    {
        if (!show) return;
        string windowName = "Fame Settings";
        rectWindowScreen = MyGUIUtility.KeepRectWithinScreen(GUILayout.Window((int)WindowID.setting, rectWindowScreen, DrawWindow, new GUIContent(windowName)));

    }

    float IgnoreMinSpeed;
    float AccelerationThreshold;
    float SlowingDistance;
    bool SwitchAgentPosition;
    bool EnableTerrainEffect;
    bool WrapTerrain;
    private static float CustomFloatField(String content, float value, params GUILayoutOption[] options)
    {
        float result = value;
        GUILayout.BeginHorizontal();
        GUILayout.Label(content);
        string tempResult = GUILayout.TextField(value.ToString(), options);
        float tempResultf = 0;
        if (float.TryParse(tempResult, out tempResultf))
        {
            result = tempResultf;
        }
        GUILayout.EndHorizontal();
        return result;
    }


    void DrawWindow(int id)
    {
        IgnoreMinSpeed = Math.Abs(CustomFloatField("Ignored Speed", IgnoreMinSpeed, GUILayout.Width(150)));
        AccelerationThreshold = Math.Abs(CustomFloatField("Acceleration To Ignore", AccelerationThreshold, GUILayout.Width(150)));
        SlowingDistance = Math.Abs(CustomFloatField("Dist. to slowing down", SlowingDistance, GUILayout.Width(150)));
        SwitchAgentPosition = GUILayout.Toggle(SwitchAgentPosition, "Self-Organize in Formation");
        EnableTerrainEffect = GUILayout.Toggle(EnableTerrainEffect, "Enable Terrain Effect");
        WrapTerrain = GUILayout.Toggle(WrapTerrain, "Wrap Terrain");

        GUILayout.Space(20);
        GUILayout.BeginHorizontal();
        GUILayout.Space(20);

        if (GUILayout.Button("OK"))
        {
            FameManager.Singleton.IgnoreMinSpeed = IgnoreMinSpeed;
            FameManager.Singleton.AccelerationThreshold = AccelerationThreshold;
            FameManager.Singleton.SlowingDistance = SlowingDistance;
            FameManager.Singleton.SwitchAgentPosition = SwitchAgentPosition;
            FameManager.Singleton.EnableTerrainEffect = EnableTerrainEffect;
            FameManager.Singleton.WarpTerrain = WrapTerrain;
            FameManager.Singleton.ApplyFameSettings();
            show = false;
        }
        if (GUILayout.Button("Cancel"))
        {
            show = false;
        }
        if (GUILayout.Button("Reset Scene"))
        {
            UnitSelection.DeselectUnit();
            FameManager.ResetScene();
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
