using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using FameCSharp;
using FameCore.Util;

public enum SteeringBehaviors
{
    Cohesion,
    Alignment,
    Separation,
    SeparationFromOtherGroup,
    Guide,
    VectorField,
}

[System.Serializable]
public class FlockGroup : MonoBehaviour, CtrlPointInterface
{
	int flockID = -1;
    /// <summary>
    /// return the unique id to identify the flock
    /// </summary>
    public int FlockID
    {
        get
        {
            return flockID;
        }
    }

    /// <summary>
    /// return the unique id to identify the flock
    /// </summary>
    public int ID{
        get
        {
            return FlockID;
        }
    }
    /// <summary>
    /// Gets the number agent in flock.
    /// </summary>
    /// <returns>
    /// The number agent in flock.
    /// </returns>
    public int GetNumAgentInFlock()
    {
        return FAME.Singleton.AgentCount(flockID);
    }
    #region flockParam

    [SerializeField]
    public BehaviorParam[] behaviorList = {
        new BehaviorParam(SteeringBehaviors.Cohesion,true, FlockMember.Default_CohRad, FlockMember.Default_CohW),
        new BehaviorParam(SteeringBehaviors.Alignment,true, FlockMember.Default_AliRad, FlockMember.Default_AliW),
        new BehaviorParam(SteeringBehaviors.Separation,true, FlockMember.Default_SepRad, FlockMember.Default_SepW),
        new BehaviorParam(SteeringBehaviors.SeparationFromOtherGroup,true, FlockMember.Default_Sep2Rad, FlockMember.Default_Sep2W),
        new BehaviorParam(SteeringBehaviors.Guide,true, FlockMember.Default_GuiW),
        new BehaviorParam(SteeringBehaviors.VectorField, true, FlockMember.Default_PFW)
    };

    [SerializeField]
    bool agentCreated = false;
    /// <summary>
    /// a flag to check if the agents have already been created
    /// </summary>
    public bool AgentCreated
    {
        get { return agentCreated; }
        set
        {
            if (!init)
            {
                agentCreated = value;
            }
            else
            {
                Debug.LogError("Not allowed to changed this value at runtime");
            }
        }
    }

    [SerializeField]
    private FlockType flockType = FlockType.Ground;
    /// <summary>
    /// the flockType: ground or air
    /// </summary>
    public FlockType FlockType
    {
        get { return flockType; }
        set
        {
            if (!init)
            {
                flockType = value;
            }
            else
            {
                Debug.LogError("Not allowed to changed this value at runtime");
            }
        }
    }

    [SerializeField]
    private float agentRadius = 5;
    /// <summary>
    /// the radius of each agent. Will apply value to all its flockMember when called during runtime
    /// </summary>
    public float AgentRadius
    {
        get { return agentRadius; }
        set
        {
            agentRadius = value;
            if (init) SetAgentAttributeGroup(AgentAttribute.mRadius, value);
        }
    }

    [SerializeField]
    private float agentMinSpeed = 0;
    /// <summary>
    /// the minimum speed that the agent should travel. Will apply value to all its flockMember when called during runtime
    /// </summary>
    public float AgentMinSpeed
    {
        get { return agentMinSpeed; }
        set
        {
            agentMinSpeed = value;
            if (init) SetAgentAttributeGroup(AgentAttribute.mMinSpeed, value);
        }
    }

    [SerializeField]
    private float agentMaxSpeed = 40;
    /// <summary>
    /// the maximum speed that the agent should travel. Will apply value to all its flockMember when called during runtime
    /// </summary>
    public float AgentMaxSpeed
    {
        get { return agentMaxSpeed; }
        set
        {
            agentMaxSpeed = value;
            if (init) SetAgentAttributeGroup(AgentAttribute.mMaxSpeed, value);
        }
    }

    [SerializeField]
    private float agentMass = 1;

    public float AgentMass
    {    /// <summary>
    /// the mass of the agent. Will apply value to all its flockMember when called during runtime
    /// </summary>
        get { return agentMass; }
        set
        {
            agentMass = value;
            if (init) SetAgentAttributeGroup(AgentAttribute.mMass, value);
        }
    }

    [SerializeField]
    private bool updateAgentOrientation;
    /// <summary>
    /// Whether to update the agent's heading every timeframe
    /// </summary>
    public bool UpdateAgentOrientation
    {
        get { return updateAgentOrientation; }
        set { updateAgentOrientation = value; }
    }

    [SerializeField]
    private float mMaxForce = 20;
    /// <summary>
    /// the maximum force that is applied to the agents during each update. Will apply value to all its flockMember when called during runtime
    /// </summary>
    public float AgentMaxForce
    {
        get { return mMaxForce; }
        set
        {
            mMaxForce = value;
            if (init) SetAgentAttributeGroup(AgentAttribute.mMaxForce, value);
        }
    }
    [SerializeField]
    private float mTurnSpeed = 180;
    public float TurnSpeed
    {
        get { return mTurnSpeed; }
        set
        {
            mTurnSpeed = value;
            if (init) SetAgentAttributeGroup(AgentAttribute.mTurnSpeed, value);
        }
    }

    [SerializeField]
    private List<Vector3> shapeConstraint = new List<Vector3>(){
       new Vector3(0,0,0),
       new Vector3(100,0,0),
       new Vector3(0,0,100),
    };

    [SerializeField]
    float meshHeight = 10;

    /// <summary>
    /// the height of the formation shape constrant. For Flying flockTypes only
    /// </summary>
    public float MeshHeight
    {
        get
        {
            return meshHeight;
        }
        set
        {
            meshHeight = value;
        }
    }


    /// <summary>
    /// get the control points defining the formation shape
    /// </summary>
    /// <returns>array of vector3 points defining the formation shape</returns>
    public Vector3[] GetCtrlPointWorld()
    {
        if (init)
        {
            return FameManager.GetFlockWorldCtrlPoint(flockID);
        }
        else
        {
            Vector3[] result = new Vector3[ShapeConstraint.Count];
            for (int i = 0; i < result.Length; i++)
            {
                result[i] = ShapeConstraint[i] + gameObject.transform.position;
            }
            return result;            
        }
    }

    /// <summary>
    /// Set the position of the i-th control where i is the index specified  
    /// </summary>
    /// <param name="index">index of the control point</param>
    /// <param name="worldPos">position of the controlpoint in the world coordinate</param>
    public void SetCtrlPointPos(int index, Vector3 worldPos)
    {
        //Vector3 rootPos = gameObject.transform.position;
        //shapeConstraint[index] = new Vector3(worldPos.x, worldPos.y, worldPos.z) - rootPos;
        FAME.Singleton.SetFlockCtrlPoint(flockID, index, worldPos.x, worldPos.y, worldPos.z);
    }

    /// <summary>
    /// the formation shape to used during initialization. Do not use this during runtime, instead use GetCtrlPointWorld()
    /// </summary>
    public List<Vector3> ShapeConstraint
    {
        get { return shapeConstraint; }
        set { shapeConstraint = value; }
    }

    [SerializeField]
    private int numAgents;
    /// <summary>
    /// Number of agents belonging to the flock
    /// </summary>
    public int NumAgents
    {
        get {
            if (!init)
            {
                return numAgents;
            } else {
                return FameManager.GetAgentIDinFlock(flockID).Length;
            }
        }
        set
        {
            if (!init)
            {
                numAgents = value;
            }
            else
            {
                Debug.LogError("Not allowed to changed this value at runtime");
            }
        }
    }

    [SerializeField]
    private byte collisionFlag = 0;

    public byte CollisionFlag
    {    /// <summary>
    /// The collision flag of the agent. Note that you have to call ApplyCollisionFlag after setting the value.
    /// The collision flag is checked against the obstacleFlags in each obstacles to determine if the agents can
    /// go through the obstacles.
    /// The agent can pass this obstacle only when the “and (&)” operator of the collision flag and obstacle flag is a non-zero value.
    /// Example:
    /// Obstacle Flag -  10100001
    /// Collision Flag - 01011110 = 00000000 (cannot pass)
    /// Collision Flag - 10000000 = 10000000 (can pass)
    /// </summary>
        get
        {
            return collisionFlag;
        }
        set
        {
            collisionFlag = value;
        }
    }

    /// <summary>
    /// Turn on the flag for certain bits specified. (bitwise OR operator is performed)
    /// </summary>
    /// <param name="flag">bits to turn on</param>
    public void OnCollisionFlag(byte flag)
    {
        collisionFlag |= flag;
    }

    /// <summary>
    /// Turn off the flag for certain bits specified. (bitwise AND operator is performed)
    /// </summary>
    /// <param name="flag">bits to turn off</param>
    public void OffCollisionFlag(byte flag)
    {
        collisionFlag &= (byte)~flag;
    }
    /// <summary>
    /// Apply the collision flag settings to FAME and all the flockMembers belonging to the group
    /// </summary>
    public void ApplyCollisionFlag()
    {
        FAME.Singleton.SetFlockCollisionFlag(flockID, collisionFlag);
        int[] agentIDs = FameManager.GetAgentIDinFlock(flockID);
        for (int i = 0; i < agentIDs.Length; i++)
        {
            FlockMember member = FameManager.GetFlockMember(agentIDs[i]);
            member.CollisionFlag = collisionFlag;
        }
    }

    [SerializeField]
    private byte fieldFlag = 0;
    /// <summary>
    /// The field flag of the agent. Note that you have to call ApplyFieldFlag after setting the value.
    /// The field flag is checked against the fieldFlag in each FameField to determine if the agents can
    /// go through the Field.
    /// The FameField will not have an effect on the agent's movement only when the “and (&)” operator of this value and FieldFlag of each FameField is a non-zero value.
    /// Example:
    /// Agent FieldFlag - 10100001
    /// FameField Flag  - 01011110 = 00000000 (will affect agent movement)
    /// FameField Flag  - 10000000 = 10000000 (will not affect agent movement)
    /// </summary>
    public byte FieldFlag
    {
        get
        {
            return fieldFlag;
        }
        set
        {
            fieldFlag = value;
        }
    }

    /// <summary>
    /// Turn on the flag for certain bits specified. (bitwise OR operator is performed)
    /// </summary>
    /// <param name="flag">bits to turn on</param>
    public void OnFieldFlag(byte flag)
    {
        fieldFlag |= flag;
    }
    /// <summary>
    /// Turn off the flag for certain bits specified. (bitwise AND operator is performed)
    /// </summary>
    /// <param name="flag">bits to turn off</param>
    public void OffFieldFlag(byte flag)
    {
        fieldFlag &= (byte)~flag;
    }
    /// <summary>
    /// Apply the field flag settings to FAME and all the flockMembers belonging to the group
    /// </summary>
    public void ApplyFieldFlag()
    {
        if (init)
        {
            FAME.Singleton.SetFlockFieldFlag(flockID, fieldFlag);
            int[] agentIDs = FameManager.GetAgentIDinFlock(flockID);
            for (int i = 0; i < agentIDs.Length; i++)
            {
                FlockMember member = FameManager.GetFlockMember(agentIDs[i]);
                member.FieldFlag = fieldFlag;
            }
        }
    }

    #endregion

    protected FormationControl ctrlScript;
    bool init = false;
    Vector3 currentPos;


    /// <summary>
    /// move the group to a delta position
    /// </summary>
    /// <param name="deltaPos">movement vector</param>
    void MoveToPosition(Vector3 deltaPos)
    {
        currentPos += currentPos + deltaPos;
        FAME.Singleton.TranslateFlockAbs(flockID, currentPos.x, currentPos.y, currentPos.z);
    }

    public void InitFlock(Vector3[] pointsV3, FlockType flockType, float meshHeight = 10)
    {
        this.meshHeight = meshHeight;
        if (flockID == -1)
        {
            this.flockType = flockType;
            FVec3[] points = FameUnityUtil.Vec3ToFVec3(pointsV3);
            switch (flockType)
            {
                case FlockType.Ground:
                    flockID = FAME.Singleton.CreateGroundFlock(points);
                    break;
                case FlockType.Flying:
                    flockID = FAME.Singleton.CreateFlyingFlock(points, meshHeight);
                    break;
            }
            FameManager.RegisterFlock(flockID, this);
        }
        else
        {
            Debug.LogError("Flock has been initialize:" + flockID);
        }
        init = true;
    }

    /// <summary>
    /// flag to indicate whether the flockGroup has been initialised
    /// </summary>
    /// <returns>true if flockGroup has been initialised, false otherwise</returns>
    public bool IsInit()
    {
        return init;
    }

    public void InitFlock(FVec3[] pointsFVec3, FlockType flockType)
    {
        if (flockID == -1)
        {
            this.flockType = flockType;
            Vector3 centroidV3 = FameUnityUtil.CalculateCentroid(pointsFVec3);
            gameObject.transform.position = centroidV3;
            switch (FlockType)
            {
                case FlockType.Ground:
                    flockID = FAME.Singleton.CreateGroundFlock(pointsFVec3);
                    break;
                case FlockType.Flying:
                    flockID = FAME.Singleton.CreateFlyingFlock(pointsFVec3, meshHeight);
                    break;
            }
            FameManager.RegisterFlock(flockID, this);
        }
        else
        {
            Debug.Log("Flock has been initialize:" + flockID);
        }
        init = true;

    }

    public void InitFlock()
    {
        if (flockID == -1)
        {
            FVec3[] points = FameUnityUtil.Vec3ToFVec3(shapeConstraint.ToArray());
            FVec3 centroid = FameUnityUtil.Vec3ToFVec3(gameObject.transform.position);
            switch (FlockType)
            {
                case FlockType.Ground:
                    flockID = FAME.Singleton.CreateGroundFlock(centroid, points);
                    break;
                case FlockType.Flying:
                    flockID = FAME.Singleton.CreateFlyingFlock(centroid, points, 10);
                    break;
            }
            FameManager.RegisterFlock(flockID, this);
            SetFormationCtrlColor(new Color(1, 1, 1), new Color(0.8f, 0.8f, 0.8f), null);
        }
        else
        {
            Debug.Log("Flock has been initialize:" + flockID);
        }
        init = true;
    }

    /// <summary>
    /// Delete the flock, this method simply destroy the gameobject. It will automatically unregister the flock in FAME and FameManager upon deletion
    /// </summary>
    public void DeleteFlock()
    {
        Destroy(gameObject);
    }

    protected virtual void Awake()
    {
        currentPos = gameObject.transform.position;
    }

	protected virtual void Start () {
        if (NumAgents != 0 && agentTemplate != null)
        {
            if (ShapeConstraint.Count >= 3)
            {
                if (!init)
                {
                    InitFlock();
                    //if (!agentCreated)
                    //{
                    //    PopulateFlock(numAgents, agentTemplate, typeof(FlockMember));
                    //}
                }
            }
            else
            {
                UnityEngine.Debug.LogError("shapeConstraint: Need to define the shape of the flock with 3 or more points");
            }
        }
    }

    /// <summary>
    /// Delete all the flockMembers belonging to the group
    /// </summary>
    public void DeleteFlockMembers()
    {
        int[] agents = FameManager.GetAgentIDinFlock(flockID);
        foreach (int agentID in agents)
        {
            FlockMember member = FameManager.GetFlockMember(agentID);
            DestroyImmediate(member.gameObject);
        }
        agentCreated = false;
    }

    /// <summary>
    /// populate the flock with a number of agents specified. Existing flockmembers will be deleted.
    /// </summary>
    /// <param name="numAgents">Number of agents to populate</param>
    /// <param name="prefab">Prefab models to use for the agents</param>
    /// <param name="flockMemberType">The flockMembers component or its subclass that is used for the agents</param>
    /// <returns></returns>
    public int[] PopulateFlock(int numAgents, GameObject prefab, Type flockMemberType)
    {
        DeleteFlockMembers();

        int[] agents = FAME.Singleton.Populate(flockID, numAgents);
        CreateAgentModels(agents, prefab, flockMemberType);
        AddSteeringBehavior(behaviorList);

        FameManager.SetAgentAttributeGroup(flockID, AgentAttribute.mRadius, AgentRadius);
        FameManager.SetAgentAttributeGroup(flockID, AgentAttribute.mMaxSpeed, AgentMaxSpeed);
        FameManager.SetAgentAttributeGroup(flockID, AgentAttribute.mMinSpeed, AgentMinSpeed);
        FameManager.SetAgentAttributeGroup(flockID, AgentAttribute.mMass, AgentMass);
        FameManager.SetAgentAttributeGroup(flockID, AgentAttribute.mTurnSpeed, 180f);
        FameManager.SetAgentAttributeGroup(flockID, AgentAttribute.mMaxForce, AgentMaxForce);

        for (int i = 0; i < agents.Length; i++)
        {
            FlockMember flockmember = FameManager.GetFlockMember(agents[i]);
            flockmember.AgentRadius = AgentRadius;
            flockmember.AgentMinSpeed = AgentMinSpeed;
            flockmember.AgentMaxSpeed = AgentMaxSpeed;
            flockmember.AgentMass = AgentMass;
            flockmember.AgentMaxForce = AgentMaxForce;
        }
        ApplyCollisionFlag();
        ApplyFieldFlag();

        agentCreated = true;
        return agents;
    }

    public GameObject agentTemplate;
    /// <summary>
    /// The prefab to use for the agents.
    /// </summary>
    public GameObject AgentTemplate
    {
        get { return agentTemplate; }
        set { agentTemplate = value; }
    }
	
	private void CreateAgentModels(int[] agentID, GameObject prefab, Type flockMemberType){
		for(int i = 0; i < agentID.Length; i++){
			FVec3 agentPos = FAME.Singleton.GetAgentPosition(agentID[i]);
			Vector3 posV3 = new Vector3(agentPos.x, agentPos.y, agentPos.z);
            GameObject agentGameobject = (GameObject)Instantiate(prefab, posV3, Quaternion.identity);
            FlockMember flockMember = agentGameobject.AddComponent(flockMemberType) as FlockMember;
			flockMember.Init(agentID[i], flockID, FlockType);
            agentGameobject.name = prefab.name + "_" + agentID;
            agentGameobject.transform.parent = this.gameObject.transform;
		}
	}

    /// <summary>
    /// Get the list of agents in the group
    /// </summary>
    /// <returns>an array of flockMember id</returns>
    public int[] GetAgentsInGroup()
    {
        if (init)
        {
            return FameManager.GetAgentIDinFlock(flockID);
        }
        else
        {
            return new int[0];
        }
    }

	/// <summary>
	/// Move the group by a movement vector
	/// </summary>
	/// <param name="dt">move vector</param>
	public void MoveGroup(Vector3 dt){
		FAME.Singleton.TranslateFlock(flockID, dt.x, dt.y, dt.z);
	}
	
    /// <summary>
    /// Move the group to the specified location in the world coordinate
    /// </summary>
    /// <param name="pos">position in the world coordinate</param>
	public void MoveGroupAbs(Vector3 pos){
		FAME.Singleton.TranslateFlockAbs(flockID, pos.x, pos.y, pos.z);		
	}
	
    /// <summary>
    /// Rotate the formation shape
    /// </summary>
    /// <param name="rad">angle to rotate</param>
	public void RotateFormation(float rad){
		FAME.Singleton.RotateFormation(flockID, rad);	
	}

    /// <summary>
    /// Scale the size of the formation shape
    /// </summary>
    /// <param name="scale">the scale value (Cannot be 0!)</param>
    public void ScaleFormation(float scale)
    {
        if (scale != 0)
        {
            FAME.Singleton.ScaleFormation(flockID, scale);
        }
        else
        {
            Debug.LogError("Input value cannot be zero");
        }
    }

    /// <summary>
    /// Apply steering behaviors to all the flockMember belonging to the group. Existing behaviour will be removed.
    /// </summary>
    /// <param name="behaviorList">array of behaviors to apply</param>
	public void AddSteeringBehavior(BehaviorParam[] behaviorList){
        this.behaviorList = behaviorList;
        FAME.Singleton.RemoveAllSteeringBehaviorGroup(flockID);
        for (int i = 0; i < behaviorList.Length; i++)
        {
            if (behaviorList[i].Active)
            {
                FAME.Singleton.AddSteeringBehaviorGroup(flockID,
                    System.Enum.GetName(typeof(SteeringBehaviors),
                    behaviorList[i].Behavior),
                    behaviorList[i].Radius,
                    behaviorList[i].Weight);
            }
        }
    }

    /// <summary>
    /// Set the altitude (Y-axis) of the formation shape. For flying type flock only. 
    /// </summary>
    /// <param name="altitude">y axis height</param>
    public void SetAltitude(float altitude)
    {
        FAME.Singleton.TranslateFlockYAbs(flockID, altitude);
    }
    /// <summary>
    /// Get the current altitude of the formaiton shape. For flying type flock only.
    /// </summary>
    /// <returns></returns>
    public float GetAltitude()
    {
        return FAME.Singleton.GetFlockYPos(flockID);
    }

    /// <summary>
    /// Change the parameters of existing steering behavior
    /// </summary>
    /// <param name="behavior">The steering behaviour to edit</param>
    /// <param name="radius">radius of effect</param>
    /// <param name="weight">weight of the behavior</param>
    public void EditSteeringBehavior(SteeringBehaviors behavior, float radius, float weight)
    {
        FAME.Singleton.EditSteeringBehaviourGroup(flockID, System.Enum.GetName(typeof(SteeringBehaviors), behavior), radius, weight);
    }

    /// <summary>
    /// Remove all steering behaviour from its flockMember
    /// </summary>
    public void RemoveAllSteeringBehavior()
    {
        FAME.Singleton.RemoveAllSteeringBehaviorGroup(flockID);
    }

    /// <summary>
    /// Remove a particular steering behaviour
    /// </summary>
    /// <param name="behavior">behavior to remove</param>
    public void RemoveSteeringBehavior(SteeringBehaviors behavior)
    {
        FAME.Singleton.RemoveSteeringBehaviorGroup(flockID, System.Enum.GetName(typeof(SteeringBehaviors), behavior));
    }

    /// <summary>
    /// Change the specified agent attribute for all its flockMember
    /// </summary>
    /// <param name="attribute">Attribute to change</param>
    /// <param name="value">new value to apply</param>
    private void SetAgentAttributeGroup(AgentAttribute attribute, float value)
    {
        FAME.Singleton.SetAgentAttributeGroup(this.flockID, System.Enum.GetName(typeof(AgentAttribute), attribute), value);
    }

    /// <summary>
    /// Perform path following
    /// </summary>
    /// <param name="pathID">the id of the path to follow</param>
    /// <param name="speed">speed to travel</param>
    public void PathFollow(int pathID, float speed)
    {
        FAME.Singleton.PerformPathFollowing(pathID, flockID, speed);
    }

    /// <summary>
    /// Morph the formation to the specified shape
    /// </summary>
    /// <param name="newFormationWorld">the array of points defining the new formation shape</param>
    public void Morph(Vector3[] newFormationWorld)
    {
        if (newFormationWorld.Length >= 3)
        {
            FAME.Singleton.Morph(flockID, FameUnityUtil.Vec3ToFVec3(newFormationWorld));
        }
        else
        {
            Debug.LogError("Number of ctrl points must be at least 3 or more");
        }
    }

    /// <summary>
    /// Spread out the flockMembers belonging to the group to fill up the shape uniformly
    /// </summary>
    public void SpreadOut()
    {
        FAME.Singleton.RelaxSamplePoints(flockID);
    }

    /// <summary>
    /// Set the look and color of the formation control
    /// </summary>
    /// <param name="ctrlPtColor">color of the control points</param>
    /// <param name="lineColor">color of the line renderer</param>
    /// <param name="lineMaterial">the material to use for the line renderer</param>
    /// <returns></returns>
    public FormationControl SetFormationCtrlColor(Color ctrlPtColor, Color lineColor, Material lineMaterial)
    {
        if (ctrlScript == null)
        {
            GameObject formationCtrlObj = new GameObject("Ctrl");
            formationCtrlObj.transform.parent = gameObject.transform;
            ctrlScript = formationCtrlObj.AddComponent<FormationControl>();
        }
        switch (FlockType)
        {
            case FlockType.Ground:
                ctrlScript.Init(this, ctrlPtColor, lineColor, lineMaterial);
                break;
            case FlockType.Flying:
                ctrlScript.Init(this, ctrlPtColor, lineColor, lineMaterial);
                break;
        }
        return ctrlScript;
    }

    /// <summary>
    /// set the layer for the formation ctrl
    /// </summary>
    /// <param name="ctrlPointLayer">layer number for the control points</param>
    /// <param name="lineRendererLayer">layer number for the line renderer</param>
    public void SetFormationControlLayer(int ctrlPointLayer, int lineRendererLayer)
    {
        ctrlScript.SetLayer(ctrlPointLayer, lineRendererLayer);
    }


    public string GetObjectName()
    {
        return "Flock";
    }

    public void ShowCtrlPoint(bool value)
    {
        if (ctrlScript != null)
        {
            this.ctrlScript.ShowCtrlPoint(value);
        }
    }

    protected virtual void OnDestroy()
    {
        if (init)
        {
            if (ctrlScript != null)
            {
                DestroyImmediate(ctrlScript.gameObject);
            }
            DeleteFlockMembers();
            FameManager.RemoveFlock(flockID);
        }
    }
}
