using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public static class WorldStateManager
{
    static bool unitSelect = true;
    static bool showGUI = true;
    static bool canMoveCtrlPoint = true;
    static bool canMoveUnit = true;
    static bool isRecording = false;

    public static bool UnitSelect { get { return unitSelect; } }
    public static bool ShowGUI { get { return showGUI; } }
    public static bool CanMoveCtrlPoint { get { return canMoveCtrlPoint; } }
    public static bool CanMoveUnit { get { return canMoveUnit; } }
    public static bool IsRecording { get { return isRecording; } }
    public enum WorldState
    {
        SettingUp,
        SettingUpFreeze,
        Recording,
        RecordingEnd,
    }

    private static WorldState currentState = WorldState.SettingUp;

    public static WorldState CurrentState
    {
        get
        {
            return currentState;
        }
        set
        {
            Debug.Log(value);
            switch (value)
            {
                case WorldState.SettingUp:
                    unitSelect = true;
                    showGUI = true;
                    canMoveCtrlPoint = true;
                    canMoveUnit = true;
                    isRecording = false;
                    break;
                case WorldState.SettingUpFreeze:
                    unitSelect = false;
                    showGUI = true;
                    canMoveCtrlPoint = false;
                    canMoveUnit = false;
                    isRecording = false;
                    break;
                case WorldState.Recording:
                    unitSelect = false;
                    showGUI = false;
                    canMoveCtrlPoint = false;
                    canMoveUnit = false;
                    isRecording = true;
                    break;
                case WorldState.RecordingEnd:
                    unitSelect = false;
                    showGUI = true;
                    canMoveCtrlPoint = true;
                    canMoveUnit = true;
                    isRecording = false;
                    break;
            }
            currentState = value;
        }
    }
}
