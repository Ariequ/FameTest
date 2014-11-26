using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEditor;


public static class FameEditorUtil
{
    public static float CustomFloatField(String content, float value, params GUILayoutOption[] options)
    {
        float result;
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField(content, options);
        result = EditorGUILayout.FloatField(value);
        EditorGUILayout.EndHorizontal();
        return result;
    }

    public static bool CustomToggle(String content, bool value, params GUILayoutOption[] options)
    {
        bool result;
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField(content, options);
        result = EditorGUILayout.Toggle(value);
        EditorGUILayout.EndHorizontal();
        return result;
    }
    public static void CustomDrawRect(Rect r, float yPos)
    {
        Vector3 pt000 = new Vector3(r.xMin, yPos, r.yMin);
        Vector3 pt001 = new Vector3(r.xMax, yPos, r.yMin);
        Vector3 pt010 = new Vector3(r.xMin, yPos, r.yMax);
        Vector3 pt011 = new Vector3(r.xMax, yPos, r.yMax);

        Handles.DrawLine(pt000, pt001);
        Handles.DrawLine(pt000, pt010);
        Handles.DrawLine(pt011, pt001);
        Handles.DrawLine(pt011, pt010);
    }

    public static void CustomDrawWireBox(Rect r, float yMin, float height)
    {
        Vector3 pt000 = new Vector3(r.xMin, yMin, r.yMin);
        Vector3 pt001 = new Vector3(r.xMax, yMin, r.yMin);
        Vector3 pt010 = new Vector3(r.xMin, yMin, r.yMax);
        Vector3 pt011 = new Vector3(r.xMax, yMin, r.yMax);
        Vector3 pt100 = new Vector3(r.xMin, yMin + height, r.yMin);
        Vector3 pt101 = new Vector3(r.xMax, yMin + height, r.yMin);
        Vector3 pt110 = new Vector3(r.xMin, yMin + height, r.yMax);
        Vector3 pt111 = new Vector3(r.xMax, yMin + height, r.yMax);
        Handles.DrawLine(pt000, pt001);
        Handles.DrawLine(pt000, pt010);
        Handles.DrawLine(pt011, pt001);
        Handles.DrawLine(pt011, pt010);

        Handles.DrawLine(pt100, pt101);
        Handles.DrawLine(pt100, pt110);
        Handles.DrawLine(pt111, pt101);
        Handles.DrawLine(pt111, pt110);

        Handles.DrawLine(pt000, pt100);
        Handles.DrawLine(pt001, pt101);
        Handles.DrawLine(pt010, pt110);
        Handles.DrawLine(pt011, pt111);

    }
}

