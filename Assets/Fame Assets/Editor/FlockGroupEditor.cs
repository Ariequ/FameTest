using UnityEditor;
using UnityEngine;
using System.Collections;
using FameCSharp;
using System;

[CustomEditor(typeof(FlockGroup),true)]
public class FlockGroupEditor : Editor {
    bool showSteeringBehaviorFoldout = false;
    bool showAgentAttributeFoldout = false;
    bool showShapeConstraintFoldout = false;
    //int numShapeCtrlPoint = 3;
    //bool numShapeCtrlPointDirty = true;

    MonoScript scriptType;

    public override void OnInspectorGUI()
    {
        FlockGroup myTarget = (FlockGroup)target;
        if (myTarget.IsInit())
        {
            ShowInitUI();
        }
        else
        {
            ShowNotInitUI();
        }
    }

    private void ShowInitUI()
    {
        serializedObject.Update();
        FlockGroup myTarget = (FlockGroup)target;
        EditorGUILayout.LabelField("Flock ID", myTarget.FlockID.ToString());
        EditorGUILayout.LabelField("Flock Type", myTarget.FlockType.ToString());
        EditorGUILayout.LabelField("Num Agent", myTarget.GetNumAgentInFlock().ToString());
    }

    private void ShowNotInitUI()
    {
        serializedObject.Update();
        FlockGroup myTarget = (FlockGroup)target;

        myTarget.FlockType = (FlockType)EditorGUILayout.EnumPopup("Flock Type", myTarget.FlockType);
        myTarget.NumAgents = Mathf.Abs(EditorGUILayout.IntField("Num Agent", myTarget.NumAgents));

        myTarget.AgentTemplate = (GameObject)EditorGUILayout.ObjectField(new GUIContent("Avatar"), myTarget.AgentTemplate, typeof(GameObject), false);
        MonoScript inputScript = (MonoScript)EditorGUILayout.ObjectField(new GUIContent("FlockMember Script"), scriptType, typeof(MonoScript), false);
        if (inputScript != null)
        {
            if (typeof(FlockMember).IsAssignableFrom(inputScript.GetClass()))
            {
                scriptType = inputScript;
            }
        }
        else
        {
            scriptType = null;
        }
        if (!myTarget.AgentCreated)
        {
            if (GUILayout.Button("Create Avatars"))
            {
                Undo.RegisterFullObjectHierarchyUndo(myTarget, " ");
                int numAgent = myTarget.NumAgents;
                GameObject avatarTemplate = myTarget.AgentTemplate;
                if (numAgent != 0 && myTarget.AgentTemplate != null)
                {
                    FameCore.Util.FVec3[] points = FameUnityUtil.Vec3ToFVec3(myTarget.GetCtrlPointWorld());
                    FameCore.Util.FVec3[] mSamplePoints = FAME.Singleton.Sample(points, numAgent, 1, 3);
                    for (int i = 0; i < mSamplePoints.Length; i++)
                    {
                        //create prefabs
                        Vector3 position = new Vector3(mSamplePoints[i].x, mSamplePoints[i].y, mSamplePoints[i].z);
                        if (Terrain.activeTerrain != null && myTarget.FlockType == FlockType.Ground)
                        {
                            position = new Vector3(position.x, Terrain.activeTerrain.SampleHeight(position) + position.y, position.z);
                        }
                        GameObject agentAvatar = GameObject.Instantiate(avatarTemplate, position, Quaternion.identity) as GameObject;
                        agentAvatar.transform.parent = myTarget.gameObject.transform;
                        FlockMember flockMember;
                        if (scriptType == null)
                        {
                            flockMember = agentAvatar.AddComponent<FlockMember>();
                        }
                        else
                        {
                            flockMember = agentAvatar.AddComponent(scriptType.name) as FlockMember;
                        }
                        flockMember.BehaviorList = CloneBehaviorList(myTarget.behaviorList);
                        flockMember.AgentRadius = myTarget.AgentRadius;
                        flockMember.AgentMinSpeed = myTarget.AgentMinSpeed;
                        flockMember.AgentMaxSpeed = myTarget.AgentMaxSpeed;
                        flockMember.AgentMaxForce = myTarget.AgentMaxForce;
                        flockMember.AgentMass = myTarget.AgentMass;
                        flockMember.CollisionFlag = myTarget.CollisionFlag;

                    }
                    myTarget.AgentCreated = true;
                }
            }
        }
        else
        {
            if (GUILayout.Button("Delete Avatars"))
            {
                Undo.RecordObject(myTarget, "Delete Avatars");
                while (myTarget.transform.childCount != 0)
                {
                    Undo.DestroyObjectImmediate(myTarget.transform.GetChild(0).gameObject);
                }
                myTarget.AgentCreated = false;
            }
        }

        //Formation Shape
        showShapeConstraintFoldout = EditorGUILayout.Foldout(showShapeConstraintFoldout, "Formation Shape");
        if (showShapeConstraintFoldout)
        {
            EditorGUILayout.LabelField("Num Point", myTarget.ShapeConstraint.Count.ToString());
            //updates the shape when ui lost focus
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Add Point"))
            {
                Vector3 newPos = (myTarget.ShapeConstraint[0] + myTarget.ShapeConstraint[myTarget.ShapeConstraint.Count - 1]) / 2;
                myTarget.ShapeConstraint.Add(newPos);
            }
            if (GUILayout.Button("Remove Point"))
            {
                if(myTarget.ShapeConstraint.Count >3){
                    myTarget.ShapeConstraint.RemoveAt(myTarget.ShapeConstraint.Count - 1);
                }
            }
            EditorGUILayout.EndHorizontal();
            for (int i = 0; i < myTarget.ShapeConstraint.Count; i++)
            {
                Vector3 newPt = EditorGUILayout.Vector3Field("Point " + i.ToString(), myTarget.ShapeConstraint[i]);
                //                myTarget.template.ShapeConstraint[i].Set(newPt.x, newPt.y, newPt.z);
                myTarget.ShapeConstraint[i] = newPt;
            }
        }

        //Steering Behavior
        showSteeringBehaviorFoldout = EditorGUILayout.Foldout(showSteeringBehaviorFoldout, "Steering Behavior");
        if (showSteeringBehaviorFoldout)
        {
            for (int i = 0; i < myTarget.behaviorList.Length; i++)
            {
                myTarget.behaviorList[i] = BehaviorFieldEx(myTarget.behaviorList[i]);
            }
            if (GUILayout.Button("Apply to All"))
            {
                Component[] memberComponent = myTarget.gameObject.GetComponentsInChildren(typeof(FlockMember));
                Undo.RecordObjects(memberComponent, "SetFlockMemberBehavior");
                foreach (Component c in memberComponent)
                {
                    FlockMember flockMember = c as FlockMember;
                    for (int i = 0; i < flockMember.BehaviorList.Length; i++)
                    {
                        flockMember.BehaviorList[i].Radius = myTarget.behaviorList[i].Radius;
                        flockMember.BehaviorList[i].Weight = myTarget.behaviorList[i].Weight;
                        flockMember.BehaviorList[i].Active = myTarget.behaviorList[i].Active;
                    }
                }
            }
        }

        showAgentAttributeFoldout = EditorGUILayout.Foldout(showAgentAttributeFoldout, "Agent Attributes");
        if (showAgentAttributeFoldout)
        {
            myTarget.AgentRadius = Mathf.Abs(EditorGUILayout.FloatField("Size(Radius)", myTarget.AgentRadius));
            myTarget.AgentMinSpeed = Mathf.Abs(EditorGUILayout.FloatField("MinSpeed", myTarget.AgentMinSpeed));
            myTarget.AgentMaxSpeed = Mathf.Abs(EditorGUILayout.FloatField("MaxSpeed", myTarget.AgentMaxSpeed));
            if (myTarget.AgentMinSpeed > myTarget.AgentMaxSpeed)
            {
                myTarget.AgentMaxSpeed = myTarget.AgentMinSpeed;
            }

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

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel("Field Flag");
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

            if (GUILayout.Button("Apply to All"))
            {
                Component[] memberComponent = myTarget.gameObject.GetComponentsInChildren(typeof(FlockMember));
                Undo.RecordObjects(memberComponent, "SetFlockMemberProperty");
                foreach (Component c in memberComponent)
                {
                    FlockMember flockMember = c as FlockMember;
                    flockMember.AgentRadius = myTarget.AgentRadius;
                    flockMember.AgentMinSpeed = myTarget.AgentMinSpeed;
                    flockMember.AgentMaxSpeed = myTarget.AgentMaxSpeed;
                    flockMember.AgentMaxForce = myTarget.AgentMaxForce;
                    flockMember.AgentMass = myTarget.AgentMass;
                    flockMember.CollisionFlag = myTarget.CollisionFlag;
                    flockMember.FieldFlag = myTarget.FieldFlag;
                }
            }
        }

        if (GUI.changed)
            EditorUtility.SetDirty(myTarget);
        serializedObject.ApplyModifiedProperties();
    }

    public static BehaviorParam BehaviorFieldEx(BehaviorParam param)
    {
        EditorGUILayout.BeginHorizontal();
        param.Active = GUILayout.Toggle(param.Active, "", GUILayout.Width(18));
        GUILayout.Label(System.Enum.GetName(typeof(SteeringBehaviors), param.Behavior));
        EditorGUILayout.EndHorizontal();
        if (!param.Active)
            GUI.enabled = false;
        EditorGUILayout.BeginHorizontal();
        ++EditorGUI.indentLevel;
        EditorGUILayout.LabelField("Weight", GUILayout.Width(60));
        param.Weight = Mathf.Abs(EditorGUILayout.FloatField(param.Weight, GUILayout.Width(55)));
        GUILayout.Space(5);
        --EditorGUI.indentLevel;
        if (param.HasRadius())
        {
            EditorGUILayout.LabelField("Radius", GUILayout.Width(60));
            param.Radius = Mathf.Abs(EditorGUILayout.FloatField(param.Radius, GUILayout.Width(55)));
        }
        EditorGUILayout.EndHorizontal();

        GUI.enabled = true;
        return param;
    }

    BehaviorParam[] CloneBehaviorList(BehaviorParam[] param)
    {
        BehaviorParam[] result = new BehaviorParam[param.Length];
        for (int i = 0; i < param.Length; i++)
        {
            result[i] = (BehaviorParam)param[i].Clone();
        }
        return result;
    }

    void OnSceneGUI()
    {
        FlockGroup myTarget = (FlockGroup)target;
        Vector3 shapeGameObjectPos = myTarget.gameObject.transform.position;
        for (int i = 0; i < myTarget.ShapeConstraint.Count; i++)
        {
            Vector3 pt1 = myTarget.ShapeConstraint[i] + shapeGameObjectPos;
            Vector3 pt2 = myTarget.ShapeConstraint[(i + 1) % myTarget.ShapeConstraint.Count] + shapeGameObjectPos;

            Vector3 newPt = Handles.PositionHandle(pt1, Quaternion.identity);
            Handles.DrawLine(pt1, pt2);
            myTarget.ShapeConstraint[i] = new Vector3(newPt.x, 0, newPt.z) - new Vector3(shapeGameObjectPos.x, 0, shapeGameObjectPos.z);
        }

        if (GUI.changed)
            EditorUtility.SetDirty(target);

        Component[] members = myTarget.gameObject.GetComponentsInChildren(typeof(FlockMember));
        for (int i = 0; i < members.Length; i++)
        {
            FlockMemberEditor.DrawFlockMemberGizmo((FlockMember)members[i], showAgentAttributeFoldout, showSteeringBehaviorFoldout);
        }

    }
}
