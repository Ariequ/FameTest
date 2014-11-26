using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(FameObstacle), true)]
public class FameObstacleEditor : Editor {

    bool showCircularObstacleStatsFoldout = true;
	bool showPolygonObstacleStatsFoldout = true;

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        FameObstacle myTarget = (FameObstacle)target;

        if (myTarget.IsInit)
        {
            ShowInitUI(myTarget);
        }
        else
        {
            ShowNotInitUI(myTarget);
        }

        if (GUI.changed)
            EditorUtility.SetDirty(myTarget);
        serializedObject.ApplyModifiedProperties();
    }

    void ShowInitUI(FameObstacle myTarget)
    {
        GUI.enabled = false;
        EditorGUILayout.LabelField("Obstacle ID", myTarget.ObstacleID.ToString());

        EditorGUILayout.EnumPopup("Obstacle Type", myTarget.ObstacleType);

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PrefixLabel("Collision Flag");
        EditorGUILayout.Toggle((myTarget.CollisionFlag & (byte)FameFlag.Bit7) != 0, GUILayout.Width(12));
        EditorGUILayout.Toggle((myTarget.CollisionFlag & (byte)FameFlag.Bit6) != 0, GUILayout.Width(12));
        EditorGUILayout.Toggle((myTarget.CollisionFlag & (byte)FameFlag.Bit5) != 0, GUILayout.Width(12));
        EditorGUILayout.Toggle((myTarget.CollisionFlag & (byte)FameFlag.Bit4) != 0, GUILayout.Width(12));
        EditorGUILayout.Toggle((myTarget.CollisionFlag & (byte)FameFlag.Bit3) != 0, GUILayout.Width(12));
        EditorGUILayout.Toggle((myTarget.CollisionFlag & (byte)FameFlag.Bit2) != 0, GUILayout.Width(12));
        EditorGUILayout.Toggle((myTarget.CollisionFlag & (byte)FameFlag.Bit1) != 0, GUILayout.Width(12));
        EditorGUILayout.Toggle((myTarget.CollisionFlag & (byte)FameFlag.Bit0) != 0, GUILayout.Width(12));
        EditorGUILayout.EndHorizontal();
        GUI.enabled = true;
    }

    void ShowNotInitUI(FameObstacle myTarget)
    {
        myTarget.ObstacleType = (ObstacleType)EditorGUILayout.EnumPopup("Obstacle Type", myTarget.ObstacleType);
        if (myTarget.ObstacleType == ObstacleType.Round)
        {
            showCircularObstacleStatsFoldout = EditorGUILayout.Foldout(showCircularObstacleStatsFoldout, " Circular Obstacle Attributes");
            if (showCircularObstacleStatsFoldout)
            {
                EditorGUI.indentLevel++;
                myTarget.ObstacleRadius = Mathf.Abs(EditorGUILayout.FloatField("Size(Radius)", myTarget.ObstacleRadius));
                Mathf.Abs(EditorGUILayout.FloatField("Position X", myTarget.transform.position.x));
                Mathf.Abs(EditorGUILayout.FloatField("Position Z", myTarget.transform.position.z));
                EditorGUI.indentLevel--;
            }
        }
        else
        {
            showPolygonObstacleStatsFoldout = EditorGUILayout.Foldout(showPolygonObstacleStatsFoldout, "2D Polygon Obstacle Attributes");
            if (showPolygonObstacleStatsFoldout)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.LabelField("Num Point", myTarget.ObstaclePoints.Count.ToString());
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("Add Point"))
                {
                    Vector3 newPos = (myTarget.ObstaclePoints[0] + myTarget.ObstaclePoints[myTarget.ObstaclePoints.Count - 1]) / 2;
                    myTarget.ObstaclePoints.Add(newPos);
                }
                if (GUILayout.Button("Remove Point"))
                {
                    if (myTarget.ObstaclePoints.Count > 3)
                    {
                        myTarget.ObstaclePoints.RemoveAt(myTarget.ObstaclePoints.Count - 1);
                    }
                }
                EditorGUILayout.EndHorizontal();
                for (int i = 0; i < myTarget.ObstaclePoints.Count; i++)
                {
                    Vector3 newPt = EditorGUILayout.Vector3Field("Point " + i.ToString(), myTarget.ObstaclePoints[i]);
                    myTarget.ObstaclePoints[i] = newPt;
                }
                EditorGUI.indentLevel--;
            }

        }

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PrefixLabel("Collision Flag");
        if (EditorGUILayout.Toggle((myTarget.CollisionFlag & (byte)FameFlag.Bit7) != 0, GUILayout.Width(12)))
            myTarget.OnCollisionFlag(FameFlag.Bit7);
        else
            myTarget.OffCollisionFlag(FameFlag.Bit7);
        if (EditorGUILayout.Toggle((myTarget.CollisionFlag & (byte)FameFlag.Bit6) != 0, GUILayout.Width(12)))
            myTarget.OnCollisionFlag(FameFlag.Bit6);
        else
            myTarget.OffCollisionFlag(FameFlag.Bit6);
        if (EditorGUILayout.Toggle((myTarget.CollisionFlag & (byte)FameFlag.Bit5) != 0, GUILayout.Width(12)))
            myTarget.OnCollisionFlag(FameFlag.Bit5);
        else
            myTarget.OffCollisionFlag(FameFlag.Bit5);
        if (EditorGUILayout.Toggle((myTarget.CollisionFlag & (byte)FameFlag.Bit4) != 0, GUILayout.Width(12)))
            myTarget.OnCollisionFlag(FameFlag.Bit4);
        else
            myTarget.OffCollisionFlag(FameFlag.Bit4);
        if (EditorGUILayout.Toggle((myTarget.CollisionFlag & (byte)FameFlag.Bit3) != 0, GUILayout.Width(12)))
            myTarget.OnCollisionFlag(FameFlag.Bit3);
        else
            myTarget.OffCollisionFlag(FameFlag.Bit3);
        if (EditorGUILayout.Toggle((myTarget.CollisionFlag & (byte)FameFlag.Bit2) != 0, GUILayout.Width(12)))
            myTarget.OnCollisionFlag(FameFlag.Bit2);
        else
            myTarget.OffCollisionFlag(FameFlag.Bit2);
        if (EditorGUILayout.Toggle((myTarget.CollisionFlag & (byte)FameFlag.Bit1) != 0, GUILayout.Width(12)))
            myTarget.OnCollisionFlag(FameFlag.Bit1);
        else
            myTarget.OffCollisionFlag(FameFlag.Bit1);
        if (EditorGUILayout.Toggle((myTarget.CollisionFlag & (byte)FameFlag.Bit0) != 0, GUILayout.Width(12)))
            myTarget.OnCollisionFlag(FameFlag.Bit0);
        else
            myTarget.OffCollisionFlag(FameFlag.Bit0);
        EditorGUILayout.EndHorizontal();
        GUI.enabled = true;
    }

    void OnSceneGUI()
    {
        FameObstacle myTarget = (FameObstacle)target;
		Vector3 pos = myTarget.transform.position;	
		
		//Drawing the radius obstacle
		if(myTarget.ObstacleType == ObstacleType.Round){
	        Color oriColor = Handles.color;
	        
	        Handles.color = Color.green;
	        Handles.DrawWireDisc(pos, Vector3.up, myTarget.ObstacleRadius);
	        Handles.color = oriColor;
		}
		//Drawing the points obstacle
		else if(myTarget.ObstacleType == ObstacleType.Polygon){
			for (int i = 0; i < myTarget.ObstaclePoints.Count; i++)
	        {
	            Vector3 pt1 = myTarget.ObstaclePoints[i] + pos;
	            Vector3 pt2 = myTarget.ObstaclePoints[(i + 1) % myTarget.ObstaclePoints.Count] + pos;
	
	            Vector3 newPt = Handles.PositionHandle(pt1, Quaternion.identity);
	            Handles.DrawLine(pt1, pt2);
	            myTarget.ObstaclePoints[i] = new Vector3(newPt.x, 0, newPt.z) - new Vector3(pos.x, 0, pos.z);
	        }	
		}
        Handles.Label(pos, new GUIContent(myTarget.ObstacleType + " Obstacle"));
		if (GUI.changed)
            EditorUtility.SetDirty(target);
    }

}
