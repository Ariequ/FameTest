using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;
using FameCSharp;

[CustomEditor(typeof(FlockMember), true)]
public class FlockMemberEditor : Editor
{
    bool showAgentStatsFoldout = true;
    bool showSteeringBehaviorFoldout = true;
    void ShowInitUI()
    {
        FlockMember myTarget = (FlockMember)target;
        EditorGUILayout.LabelField("Agent ID", myTarget.AgentID.ToString());
        EditorGUILayout.LabelField("Current Speed", myTarget.MovementSpeed.ToString());
    }

    void ShowNotInitUI()
    {
        FlockMember myTarget = (FlockMember)target;
        myTarget.FlockType = (FlockType)EditorGUILayout.EnumPopup("Flock Type", myTarget.FlockType);

        showSteeringBehaviorFoldout = EditorGUILayout.Foldout(showSteeringBehaviorFoldout, "Steering Behavior");
        if (showSteeringBehaviorFoldout)
        {
            for (int i = 0; i < myTarget.BehaviorList.Length; i++)
            {
                myTarget.BehaviorList[i] = FlockGroupEditor.BehaviorFieldEx(myTarget.BehaviorList[i]);

            }
        }
        showAgentStatsFoldout = EditorGUILayout.Foldout(showAgentStatsFoldout, "Agent Attributes");
        if (showAgentStatsFoldout)
        {
            myTarget.AgentRadius = Mathf.Abs(EditorGUILayout.FloatField("Size(Radius)", myTarget.AgentRadius));
            myTarget.AgentMinSpeed = Mathf.Abs(EditorGUILayout.FloatField("MinSpeed", myTarget.AgentMinSpeed));
            myTarget.AgentMaxSpeed = Mathf.Abs(EditorGUILayout.FloatField("MaxSpeed", myTarget.AgentMaxSpeed));
            myTarget.AgentMaxForce = Mathf.Abs(EditorGUILayout.FloatField("MaxForce", myTarget.AgentMaxForce));
            myTarget.AgentMass = Mathf.Abs(EditorGUILayout.FloatField("Mass", myTarget.AgentMass));
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
        }

        if (GUI.changed)
            EditorUtility.SetDirty(myTarget);
    }
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        FlockMember myTarget = (FlockMember)target;
        if (myTarget.IsInit)
        {
            ShowInitUI();
        }
        else
        {
            ShowNotInitUI();
        }
        serializedObject.ApplyModifiedProperties();
    }

    void OnSceneGUI()
    {
        if (GUI.changed)
            EditorUtility.SetDirty(target);
        DrawFlockMemberGizmo((FlockMember)target, showAgentStatsFoldout, showSteeringBehaviorFoldout);
    }
    
    public static void DrawFlockMemberGizmo(FlockMember myTarget, bool showAgentAttribute, bool showBehavior)
    {

        Color oriColor = Handles.color;
        Vector3 pos = myTarget.transform.position;
        Handles.color = Color.green;
        Handles.DrawWireDisc(pos, Vector3.up, myTarget.AgentRadius);
        Handles.color = oriColor;
        if (showAgentAttribute) {
            //Drawing the radius of the Agent
            Vector3 orientation = new Vector3(0, 0, 1);
            Vector3 direction = FameUnityUtil.Rotate(orientation, -Mathf.Deg2Rad * myTarget.transform.localRotation.eulerAngles.y);

            Vector3 sideComp = new Vector3(direction.z, 0, -direction.x);


            Handles.color = Color.yellow;
            Vector3 maxSpeedPos = pos + direction * myTarget.AgentMaxSpeed;
            Vector3 minSpeedPos = pos + direction * myTarget.AgentMinSpeed;
            Handles.DrawLine(maxSpeedPos + sideComp, maxSpeedPos - sideComp);
            Handles.DrawLine(minSpeedPos + sideComp, minSpeedPos - sideComp);
            Handles.DrawLine(minSpeedPos, maxSpeedPos);
            Handles.Label(minSpeedPos, "Min Speed");
            Handles.Label(maxSpeedPos, "Max Speed");
            Handles.color = oriColor;
        }

        int i = 0;
        const float teeta = 0.1745f;
        if (showBehavior)
        {
            foreach (BehaviorParam bp in myTarget.BehaviorList)
            {
                if (bp.Active && bp.HasRadius())
                {
                    float r = bp.Radius + myTarget.AgentRadius;
                    Handles.DrawWireDisc(pos, Vector3.up, r);
                    Handles.Label(pos + FameUnityUtil.Rotate(new Vector3(0, 0, -r), teeta * i), bp.Behavior.ToString());
                    i++;
                }
            }
        }


    }
}
