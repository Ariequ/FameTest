using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;
[CustomEditor(typeof(FameManager), true)]
public class FameManagerEditor : Editor
{
    bool settingsFoldout = true;
    bool terrainFoldout = true;
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        FameManager myTarget = (FameManager)target;

        EditorGUILayout.LabelField("Num Flock Groups", FameManager.NumFlockGroup().ToString());
        EditorGUILayout.LabelField("Num Agents", FameManager.NumAgents().ToString());
        EditorGUILayout.LabelField("Num Obstacles", FameManager.NumObstacles().ToString());
        EditorGUILayout.LabelField("Num Paths", FameManager.NumPaths().ToString());
        EditorGUILayout.LabelField("Num Fields", FameManager.NumFields().ToString());

        serializedObject.ApplyModifiedProperties();
        if (!myTarget.IsInit())
        {
            EditorGUILayout.Space();
            settingsFoldout = EditorGUILayout.Foldout(settingsFoldout, "FAME Settings");
            if (settingsFoldout)
            {
                ++EditorGUI.indentLevel;
                myTarget.IgnoreMinSpeed = Math.Abs(FameEditorUtil.CustomFloatField("Ignored Speed", myTarget.IgnoreMinSpeed, GUILayout.MinWidth(150)));
                myTarget.AccelerationThreshold = Math.Abs(FameEditorUtil.CustomFloatField("Acceleration To Ignore", myTarget.AccelerationThreshold, GUILayout.MinWidth(150)));
                myTarget.SlowingDistance = Math.Abs(FameEditorUtil.CustomFloatField("Dist. to slowing down", myTarget.SlowingDistance, GUILayout.MinWidth(150)));
                myTarget.SwitchAgentPosition = FameEditorUtil.CustomToggle("Self-Organize in Formation", myTarget.SwitchAgentPosition, GUILayout.MinWidth(150));
                myTarget.EnableTerrainEffect = FameEditorUtil.CustomToggle("Enable Terrain Effect", myTarget.EnableTerrainEffect, GUILayout.MinWidth(150));
                --EditorGUI.indentLevel;
            }
            EditorGUILayout.Space();
            terrainFoldout = EditorGUILayout.Foldout(terrainFoldout, "FAME Terrain");
            if (terrainFoldout)
            {
                ++EditorGUI.indentLevel;
                EditorGUILayout.BeginHorizontal();
                myTarget.GetInfoFromUnityTerrain = EditorGUILayout.Toggle(myTarget.GetInfoFromUnityTerrain, GUILayout.Width(22));
                EditorGUILayout.LabelField("Get Info From Terrain GameObject?");
                EditorGUILayout.EndHorizontal();

                if (!myTarget.GetInfoFromUnityTerrain)
                {
                    EditorGUILayout.LabelField("Terrain Bound");
                    myTarget.Resolution = EditorGUILayout.IntSlider("Resolution", myTarget.Resolution, 64, 1025);
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("W", GUILayout.Width(24));
                    myTarget.TerrainWidth = Mathf.Abs(EditorGUILayout.FloatField(myTarget.TerrainWidth));
                    EditorGUILayout.LabelField("H", GUILayout.Width(24));
                    myTarget.TerrainHeight = Mathf.Abs(EditorGUILayout.FloatField(myTarget.TerrainHeight));
                    EditorGUILayout.LabelField("L", GUILayout.Width(24));
                    myTarget.TerrainLength = Mathf.Abs(EditorGUILayout.FloatField(myTarget.TerrainLength));

                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.Space();
                    EditorGUILayout.LabelField("Height Map");
                    myTarget.TerrainHeightMap = EditorGUILayout.ObjectField(myTarget.TerrainHeightMap, typeof(Texture2D), false) as Texture2D;
                    //myTarget.TerrainWidth = Math.Abs(EditorGUILayout.FloatField("Width", myTarget.TerrainWidth));
                    //myTarget.TerrainLength = Math.Abs(EditorGUILayout.FloatField("Height", myTarget.TerrainLength));
                    //myTarget.TerrainHeight = Math.Abs(EditorGUILayout.FloatField("Height", myTarget.TerrainLength));

                }
                else
                {
                    if (Terrain.activeTerrain == null)
                    {
                        EditorGUILayout.LabelField("Terrain GameObject not found");
                    }
                    else
                    {
                        Bounds b = Terrain.activeTerrain.collider.bounds;
                        EditorGUILayout.LabelField("Terrain Bound");
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField("W", GUILayout.Width(24));
                        EditorGUILayout.FloatField(b.extents.x);
                        EditorGUILayout.LabelField("H", GUILayout.Width(24));
                        EditorGUILayout.FloatField(b.extents.y);
                        EditorGUILayout.LabelField("L", GUILayout.Width(24));
                        EditorGUILayout.FloatField(b.extents.z);
                        EditorGUILayout.EndHorizontal();
                    }
                }
                EditorGUILayout.Space();
                myTarget.WarpTerrain = EditorGUILayout.Toggle("Warp Terrain", myTarget.WarpTerrain);
                if (!myTarget.WarpTerrain)
                {
                    GUI.enabled = false;
                }
                myTarget.WarpTerrainSize = EditorGUILayout.RectField(myTarget.WarpTerrainSize);
                GUI.enabled = true;
                --EditorGUI.indentLevel;
            }
        }

        if (GUI.changed)
            EditorUtility.SetDirty(myTarget);
        serializedObject.ApplyModifiedProperties();
    }


    static int Clamp(int num, int min, int max)
    {
        int result = num;
        if (result < min) result = min;
        if (result > max) result = max;
        return result;
    }

    void OnSceneGUI()
    {
        FameManager myTarget = (FameManager)target;
        if (GUI.changed)
            EditorUtility.SetDirty(target);

        if (!myTarget.GetInfoFromUnityTerrain)
        {
            Rect r = new Rect(myTarget.transform.position.x, myTarget.transform.position.z, myTarget.TerrainWidth, myTarget.TerrainLength);
            Color oriColor = Handles.color;
            Handles.color = Color.green;
            FameEditorUtil.CustomDrawWireBox(r, myTarget.transform.position.y, myTarget.TerrainHeight);
            Handles.color = oriColor;
        }

        if (myTarget.WarpTerrain)
        {
            FameEditorUtil.CustomDrawRect(myTarget.WarpTerrainSize, myTarget.transform.position.y);
        }

    }


}
