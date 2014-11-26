using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(FameField), true)]
public class FameFieldEditor : Editor {
	bool showFieldStatsFoldout = true;
	bool showCircularFieldStatsFoldout = true;
	
	public override void OnInspectorGUI(){
        serializedObject.Update();
        FameField myTarget = (FameField)target;

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
	
	void OnSceneGUI(){
        FameField myTarget = (FameField)target;
		Vector3 pos = myTarget.transform.position;	
		
		if(myTarget.FieldType == FameFieldType.Circular){
		    Color oriColor = Handles.color;
		        
		    Handles.color = Color.cyan;
		    Handles.DrawWireDisc(pos, Vector3.up, myTarget.FieldRadius);
		    Handles.color = oriColor;
			
			switch(myTarget.CircularFieldDirection){
                case FameCircularDirection.Clockwise:
					Handles.Label(new Vector3(pos.x, pos.y, pos.z+myTarget.FieldRadius), "Clockwise");
						break;
                case FameCircularDirection.AntiClockwise:
					Handles.Label(new Vector3(pos.x, pos.y, pos.z+myTarget.FieldRadius), "Anti-Clockwise");
					break;
                case FameCircularDirection.Attraction:
                    Handles.Label(new Vector3(pos.x, pos.y, pos.z + myTarget.FieldRadius), "Attract");
                    break;
                case FameCircularDirection.Repulsion:
                    Handles.Label(new Vector3(pos.x, pos.y, pos.z + myTarget.FieldRadius), "Repel");
                    break;
			}
		}
		else{
			float lengthx = myTarget.FieldWidthX / 2;
			float lengthz = myTarget.FieldWidthZ / 2;
			
			Vector3[] fieldarea = {
				new Vector3(pos.x-lengthx, pos.y, pos.z-lengthz), 
				new Vector3(pos.x-lengthx, pos.y, pos.z+lengthz), 
				new Vector3(pos.x+lengthx, pos.y, pos.z+lengthz), 
				new Vector3(pos.x+lengthx, pos.y, pos.z-lengthz)
			};

			Handles.DrawSolidRectangleWithOutline(fieldarea, Color.clear, Color.cyan);
            float angle = -(myTarget.FieldAngleDeg - 90);
            Handles.ArrowCap(0, pos, Quaternion.Euler(new Vector3(0, angle, 0)), Mathf.Min(lengthx, lengthz));
			
		}
		
		if (GUI.changed)
            EditorUtility.SetDirty(target);
	}

    void ShowInitUI(FameField myTarget)
    {
        GUI.enabled = false;
        EditorGUILayout.LabelField("Field ID", myTarget.FieldID.ToString());
        myTarget.FieldType = (FameFieldType)EditorGUILayout.EnumPopup("Field Type", myTarget.FieldType);

        if (myTarget.FieldType == FameFieldType.Circular)
        {
            showCircularFieldStatsFoldout = EditorGUILayout.Foldout(showCircularFieldStatsFoldout, "Field Attributes");
            if (showCircularFieldStatsFoldout)
            {
                EditorGUILayout.EnumPopup("Field Direction", myTarget.CircularFieldDirection);
                EditorGUILayout.FloatField("Size(Radius)", myTarget.FieldRadius);
                EditorGUILayout.FloatField("Magnitude", myTarget.FieldMagnitude);
            }
        }
        else
        {
            showFieldStatsFoldout = EditorGUILayout.Foldout(showFieldStatsFoldout, "Field Attributes");
            if (showFieldStatsFoldout)
            {
                EditorGUILayout.FloatField("X-Width", myTarget.FieldWidthX);
                EditorGUILayout.FloatField("Z-Width", myTarget.FieldWidthZ);
                EditorGUILayout.FloatField("Angle", myTarget.FieldAngleDeg);
                EditorGUILayout.FloatField("Magnitude", myTarget.FieldMagnitude);
            }
        }

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PrefixLabel("Collision Flag");
        EditorGUILayout.Toggle((myTarget.FieldFlag & (byte)FameFlag.Bit7) != 0, GUILayout.Width(12));
        EditorGUILayout.Toggle((myTarget.FieldFlag & (byte)FameFlag.Bit6) != 0, GUILayout.Width(12));
        EditorGUILayout.Toggle((myTarget.FieldFlag & (byte)FameFlag.Bit5) != 0, GUILayout.Width(12));
        EditorGUILayout.Toggle((myTarget.FieldFlag & (byte)FameFlag.Bit4) != 0, GUILayout.Width(12));
        EditorGUILayout.Toggle((myTarget.FieldFlag & (byte)FameFlag.Bit3) != 0, GUILayout.Width(12));
        EditorGUILayout.Toggle((myTarget.FieldFlag & (byte)FameFlag.Bit2) != 0, GUILayout.Width(12));
        EditorGUILayout.Toggle((myTarget.FieldFlag & (byte)FameFlag.Bit1) != 0, GUILayout.Width(12));
        EditorGUILayout.Toggle((myTarget.FieldFlag & (byte)FameFlag.Bit0) != 0, GUILayout.Width(12));
        EditorGUILayout.EndHorizontal();
        GUI.enabled = true;
    }

    void ShowNotInitUI(FameField myTarget)
    {
        myTarget.FieldType = (FameFieldType)EditorGUILayout.EnumPopup("Field Type", myTarget.FieldType);

        if (myTarget.FieldType == FameFieldType.Circular)
        {
            showCircularFieldStatsFoldout = EditorGUILayout.Foldout(showCircularFieldStatsFoldout, "Field Attributes");
            if (showCircularFieldStatsFoldout)
            {
                myTarget.CircularFieldDirection = (FameCircularDirection)EditorGUILayout.EnumPopup("Field Direction", myTarget.CircularFieldDirection);
                myTarget.FieldRadius = Mathf.Abs(EditorGUILayout.FloatField("Size(Radius)", myTarget.FieldRadius));
                myTarget.FieldMagnitude = Mathf.Abs(EditorGUILayout.FloatField("Magnitude", myTarget.FieldMagnitude));
            }
        }
        else
        {
            showFieldStatsFoldout = EditorGUILayout.Foldout(showFieldStatsFoldout, "Field Attributes");
            if (showFieldStatsFoldout)
            {
                myTarget.FieldWidthX = Mathf.Abs(EditorGUILayout.FloatField("X-Width", myTarget.FieldWidthX));
                myTarget.FieldWidthZ = Mathf.Abs(EditorGUILayout.FloatField("Z-Width", myTarget.FieldWidthZ));
                myTarget.FieldAngleDeg = Mathf.Abs(EditorGUILayout.FloatField("Angle", myTarget.FieldAngleDeg));
                myTarget.FieldMagnitude = Mathf.Abs(EditorGUILayout.FloatField("Magnitude", myTarget.FieldMagnitude));
            }
        }

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PrefixLabel("Collision Flag");
        if (EditorGUILayout.Toggle((myTarget.FieldFlag & (byte)FameFlag.Bit7) != 0, GUILayout.Width(12)))
            myTarget.OnFieldFlag(FameFlag.Bit7);
        else
            myTarget.OffFieldFlag(FameFlag.Bit7);
        if (EditorGUILayout.Toggle((myTarget.FieldFlag & (byte)FameFlag.Bit6) != 0, GUILayout.Width(12)))
            myTarget.OnFieldFlag(FameFlag.Bit6);
        else
            myTarget.OffFieldFlag(FameFlag.Bit6);
        if (EditorGUILayout.Toggle((myTarget.FieldFlag & (byte)FameFlag.Bit5) != 0, GUILayout.Width(12)))
            myTarget.OnFieldFlag(FameFlag.Bit5);
        else
            myTarget.OffFieldFlag(FameFlag.Bit5);
        if (EditorGUILayout.Toggle((myTarget.FieldFlag & (byte)FameFlag.Bit4) != 0, GUILayout.Width(12)))
            myTarget.OnFieldFlag(FameFlag.Bit4);
        else
            myTarget.OffFieldFlag(FameFlag.Bit4);
        if (EditorGUILayout.Toggle((myTarget.FieldFlag & (byte)FameFlag.Bit3) != 0, GUILayout.Width(12)))
            myTarget.OnFieldFlag(FameFlag.Bit3);
        else
            myTarget.OffFieldFlag(FameFlag.Bit3);
        if (EditorGUILayout.Toggle((myTarget.FieldFlag & (byte)FameFlag.Bit2) != 0, GUILayout.Width(12)))
            myTarget.OnFieldFlag(FameFlag.Bit2);
        else
            myTarget.OffFieldFlag(FameFlag.Bit2);
        if (EditorGUILayout.Toggle((myTarget.FieldFlag & (byte)FameFlag.Bit1) != 0, GUILayout.Width(12)))
            myTarget.OnFieldFlag(FameFlag.Bit1);
        else
            myTarget.OffFieldFlag(FameFlag.Bit1);
        if (EditorGUILayout.Toggle((myTarget.FieldFlag & (byte)FameFlag.Bit0) != 0, GUILayout.Width(12)))
            myTarget.OnFieldFlag(FameFlag.Bit0);
        else
            myTarget.OffFieldFlag(FameFlag.Bit0);
        EditorGUILayout.EndHorizontal();
    }
}