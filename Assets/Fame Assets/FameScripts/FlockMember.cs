using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public enum AgentAttribute
{
    mMaxSpeed,
    mMinSpeed,
    mRadius,
    mMass,
    mMaxForce,
    mTurnSpeed
}

public class FlockMember : MonoBehaviour
{
    #region defaultAgentParam
    public static float Default_mRadius = 1;
    public static float Default_mMinSpeed = 0;
    public static float Default_mMaxSpeed = 40;
    public static float Default_mMass = 1;
    public static float Default_mMaxForce = 20;
    public static float Default_mTurnSpeed = 180;

    public static float Default_CohRad = 10;
    public static float Default_CohW = 1;

    public static float Default_AliRad = 5;
    public static float Default_AliW = 1;

    public static float Default_SepRad = 3;
    public static float Default_SepW = 1;

    public static float Default_Sep2Rad = 5;
    public static float Default_Sep2W = 1;

    public static float Default_GuiW = 1;
    public static float Default_PFW = 1;
    public static float Default_OAW = 1;

    #endregion

    #region agentParam

    [SerializeField]
    private BehaviorParam[] behaviorList = {
        new BehaviorParam(SteeringBehaviors.Cohesion,true, Default_CohRad, Default_CohW),
        new BehaviorParam(SteeringBehaviors.Alignment,true, Default_AliRad, Default_AliW),
        new BehaviorParam(SteeringBehaviors.Separation,true, Default_SepRad, Default_SepW),
        new BehaviorParam(SteeringBehaviors.SeparationFromOtherGroup,true, Default_Sep2Rad, Default_Sep2W),
        new BehaviorParam(SteeringBehaviors.Guide,true, Default_GuiW),
        new BehaviorParam(SteeringBehaviors.VectorField, true, Default_PFW)
    };


    public BehaviorParam[] BehaviorList
    {
        get { return behaviorList; }
        set { behaviorList = value; }
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
                UnityEngine.Debug.LogError("Cannot change flockType after it has been initialize");
            }
        }
    }

    [SerializeField]
    private float agentRadius = Default_mRadius;
    /// <summary>
    /// the radius of each agent.
    /// </summary>
    public float AgentRadius
    {
        get { return agentRadius; }
        set { agentRadius = value;
        if (init) SetAgentAttribute(AgentAttribute.mRadius, value);
        }
    }

    [SerializeField]
    private float agentMinSpeed = Default_mMinSpeed;
    /// <summary>
    /// the minimum speed that the agent should travel.
    /// </summary>
    public float AgentMinSpeed
    {
        get { return agentMinSpeed; }
        set
        {
            agentMinSpeed = value;
            if (init) SetAgentAttribute(AgentAttribute.mMinSpeed, value);
        }
    }

    [SerializeField]
    private float agentMaxSpeed = Default_mMaxSpeed;
    /// <summary>
    /// the maximum speed that the agent should travel.
    /// </summary>
    public float AgentMaxSpeed
    {
        get { return agentMaxSpeed; }
        set
        {
            agentMaxSpeed = value;
            if (init) SetAgentAttribute(AgentAttribute.mMaxSpeed, value);
        }
    }

    [SerializeField]
    private float agentMass = Default_mMass;
    /// <summary>
    /// the mass of the agent.
    /// </summary>
    public float AgentMass
    {
        get { return agentMass; }
        set
        {
            agentMass = value;
            if (init) SetAgentAttribute(AgentAttribute.mMass, value);
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
    private float mMaxForce = Default_mMaxForce;
    /// <summary>
    /// the maximum force that is applied to the agents during each update.
    /// </summary>
    public float AgentMaxForce
    {
        get { return mMaxForce; }
        set
        {
            mMaxForce = value;
            if (init) SetAgentAttribute(AgentAttribute.mMaxForce, value);
        }

    }
    [SerializeField]
    private float mTurnSpeed = Default_mTurnSpeed;
    public float TurnSpeed
    {
        get { return mTurnSpeed; }
        set
        {
            mTurnSpeed = value;
            if(init) SetAgentAttribute(AgentAttribute.mTurnSpeed, value);
        }
    }

    [SerializeField]
    private byte collisionFlag = 0;
    /// <summary>
    /// The collision flag of the agent. Note that you have to call ApplyCollisionFlag after setting the value.
    /// The collision flag is checked against the obstacleFlags in each obstacles to determine if the agents can
    /// go through the obstacles.
    /// The agent can pass this obstacle only when the “and (&)” operator of the collision flag and obstacle flag is a non-zero value.
    /// Example:
    /// Obstacle Flag -  10100001
    /// Collision Flag - 01011110 = 00000000 (cannot pass)
    /// Collision Flag - 10000000 = 10000000 (can pass)
    /// </summary>
    public byte CollisionFlag
    {
        get
        {
            return collisionFlag;
        }
        set
        {
            collisionFlag = value;
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
    /// Turn off the flag for certain bits specified. (bitwise AND operator is performed)
    /// </summary>
    /// <param name="flag">bits to turn off</param>
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
    /// Apply the field flag settings to FAME and all the flockMembers belonging to the group
    /// </summary>
    public void ApplyFieldFlag()
    {
        FameManager.SetAgentFieldFlag(agentID, fieldFlag);
    }
    /// <summary>
    /// Apply the collision flag settings to FAME and all the flockMembers belonging to the group
    /// </summary>
    public void ApplyCollisionFlag()
    {
        FameManager.SetAgentCollisionFlag(agentID, collisionFlag);
    }

    #endregion


    int agentID = -1;
    int flockID = -1;
	float movementSpeed;
    /// <summary>
    /// Current speed of the agent
    /// </summary>
    /// <returns></returns>
    public float MovementSpeed
    {
        get
        {
            return movementSpeed;
        }
    }

    private bool init = false;
    /// <summary>
    /// flag to indicate whether the flockMember has been initialised
    /// </summary>
    /// <returns>true if flockMember has been initialised, false otherwise</returns>
    public bool IsInit { get { return init; } }

    bool updateOrientation = true;
    bool isEnabled; // enable movement?

    /// <summary>
    /// Enable or disable the movement of the agent
    /// </summary>
    public bool IsEnabled
    {
        get
        {
            return isEnabled;
        }
        set
        {
            if (agentID != -1) {
                isEnabled = value;
                FameManager.SetEnableAgent(agentID, value);
            }
        }
    
    }

    float yRotOffset = 0;

    /// <summary>
    /// offset to use for the perfab. In events when the agent model is facing the wrong direction
    /// </summary>
    public float YRotOffset
    {
        get { return yRotOffset; }
        set { yRotOffset = value; }
    }

    protected virtual void Start()
    {
        if (!init)
        {
            FlockGroup flockGroup = null;
            if (transform.parent != null) { 
                flockGroup = transform.parent.GetComponent<FlockGroup>();
            }
            Vector3 pos = transform.position;
            if (flockGroup != null)
            {
                if (flockGroup.FlockID == -1)
                {
                    flockGroup.InitFlock();
                }
                flockID = flockGroup.FlockID;
                behaviorList = flockGroup.behaviorList;
                agentID = FameManager.CreateAgent(flockID, pos, false);
            }
            else
            {
                flockID = -1;
                agentID = FameManager.CreateAgent(flockType, pos);
            }
            
            AddSteeringBehaviour(behaviorList);
            InitAgentAttributes();
            SetOrientation();
            FameManager.RegisterAgent(agentID, this);
            ApplyCollisionFlag();
            ApplyFieldFlag();
            init = true;
        }
	}

    /// <summary>
    /// Set the orientation of the agent. 0 is facing right (0,0,1)
    /// </summary>
    private void SetOrientation()
    {
        Vector3 orientation = new Vector3(0, 0, 1);
        Vector3 direction = FameUnityUtil.Rotate(orientation, -Mathf.Deg2Rad * transform.localRotation.eulerAngles.y);
        FameManager.SetAgentOrientation(agentID, direction.x, 0, direction.z);
    }


    /// <summary>
    /// the flockID that this flockMember belongs to. -1 means it does not belong to any flock
    /// </summary>
    public int FlockID
    {
        get
        {
            return flockID;
        }
    }

    /// <summary>
    /// the unique id of the flockMember
    /// </summary>
	public int AgentID{
		get { return agentID; }
	}
	
	
	public void Init(int agentID, int flockID, FlockType type){
		if(!init){
		    //can only init once!
            this.flockID = flockID;
			this.agentID = agentID;
            this.flockType = type;
            FameManager.RegisterAgent(agentID, this);
            ApplyCollisionFlag();
            ApplyFieldFlag();
			init = true;
		}
	}
	

	public Vector3 CurrentPosition{
		get{
			return gameObject.transform.position;
		}
	}
	
    /// <summary>
    /// Do not use this function. (Called by FameManager on every update to update its position)
    /// </summary>
    /// <param name="position">Position of the agent</param>
	public void SetPosition(Vector3 position){
        
        Vector3 pos = position;
        if (flockType == FlockType.Ground)
        {
            if (Terrain.activeTerrain != null)
            {
                pos.y = Terrain.activeTerrain.SampleHeight(pos);
            }
        }

        Vector3 destPos = pos;
        //		destPos.y = Terrain.activeTerrain.SampleHeight(pos);
        Vector3 movement;
        movement = destPos - transform.position;
        movementSpeed = (movement.magnitude / Time.deltaTime);
        if (updateOrientation)
        {
            if (flockType == FlockType.Ground)
            {
                movement.y = 0;
            } 
			if(movement.normalized.magnitude != 0) {
                movement = movement.normalized;
                Quaternion rot = Quaternion.LookRotation(movement);
                Vector3 rotOffsetVec = new Vector3(0,yRotOffset,0);
                rot *= Quaternion.Euler(rotOffsetVec);
                transform.rotation = rot;
			}			
		}
        gameObject.transform.position = pos;
	}
	
    /// <summary>
    /// Teleport the agent to a position instantly
    /// </summary>
    /// <param name="position">position</param>
	public void TeleportToPosition(Vector3 position){
		switch (flockType){
			case FlockType.Ground:
                FameManager.TeleportAgent(agentID, position.x, position.z);
				break;
			case FlockType.Flying:
                FameManager.TeleportAgent(agentID, position);		
				break;
		}
	}
	
    /// <summary>
    /// Get or set the destination of the agent
    /// </summary>
	public Vector3 Destination{
		get{
            return FameManager.GetAgentDestination(agentID);
		}
		set{
            if (flockID == -1)
            {
                Vector3 posValue = value;
                FameManager.MoveAgent(agentID, posValue);
            }
            else
            {
                Debug.Log("Cannot move flockMembers while belonging to a flockGroup. Try LeaveGroup() before calling this method.");
            }
		}
	}


    private void InitAgentAttributes()
    {
        FameManager.SetAgentAttribute(agentID, AgentAttribute.mRadius, AgentRadius);
        FameManager.SetAgentAttribute(agentID, AgentAttribute.mMinSpeed, AgentMinSpeed);
        FameManager.SetAgentAttribute(agentID, AgentAttribute.mMaxSpeed, AgentMaxSpeed);
        FameManager.SetAgentAttribute(agentID, AgentAttribute.mTurnSpeed, TurnSpeed);
        FameManager.SetAgentAttribute(agentID, AgentAttribute.mMass, AgentMass);
        FameManager.SetAgentAttribute(agentID, AgentAttribute.mMaxForce, AgentMaxForce);
    }

    /// <summary>
    /// Apply steering behaviors to the flockMember. Existing behaviour will be removed.
    /// </summary>
    /// <param name="behaviorList">array of behaviors to apply</param>
    public void AddSteeringBehaviour(BehaviorParam[] behaviorList)
    {
        FameManager.RemoveAllSteeringBehaviorAgent(agentID);
        for (int i = 0; i < behaviorList.Length; i++)
        {
            if (behaviorList[i].Active)
            {
                FameManager.AddSteeringBehaviorAgent(agentID,
                    behaviorList[i].Behavior,
                    behaviorList[i].Radius,
                    behaviorList[i].Weight);
            }
        }
    }
    /// <summary>
    /// Change the parameters of existing steering behavior
    /// </summary>
    /// <param name="behavior">The steering behaviour to edit</param>
    /// <param name="radius">radius of effect</param>
    /// <param name="weight">weight of the behavior</param>
    public void EditSteeringBehavior(SteeringBehaviors behavior, float radius, float weight)
    {
        FameManager.EditSteeringBehaviorAgent(agentID, behavior, radius, weight);
    }

    /// <summary>
    /// Remove all steering behaviour from flockMember
    /// </summary>
    public void RemoveAllSteeringBehavior()
    {
        FameManager.RemoveAllSteeringBehaviorAgent(agentID);
    }
    /// <summary>
    /// Remove a particular steering behaviour
    /// </summary>
    /// <param name="behavior">behavior to remove</param>
    public void RemoveSteeringBehavior(SteeringBehaviors behavior)
    {
        FameManager.RemoveSteeringBehaviorAgent(agentID, behavior);
    }

    /// <summary>
    /// Change the specified agent attribute
    /// </summary>
    /// <param name="attribute">Attribute to change</param>
    /// <param name="value">new value to apply</param>
    protected void SetAgentAttribute(AgentAttribute attribute, float value)
    {
        FameManager.SetAgentAttribute(agentID, attribute, value);
    }


    /// <summary>
    /// Leave the flockGroup and roam around freely.
    /// </summary>
    public void LeaveGroup()
    {
        flockID = -1;
        gameObject.transform.parent = null;
        FameManager.LeaveGroup(agentID);
    }

    /// <summary>
    /// Do not use this function. Internal use only
    /// </summary>
    /// <param name="flockGroup"></param>
    public void ChangeFlockID(FlockGroup flockGroup)
    {
        this.flockID = flockGroup.FlockID;
        gameObject.transform.parent = flockGroup.gameObject.transform;
    }

    /// <summary>
    /// Specify a list of flockMembers to join the specified flockGroup
    /// </summary>
    /// <param name="flockID">ID of the flockGroup to join</param>
    /// <param name="flockMembers">the list of flockMembers involved</param>
    public static void JoinGroup(int flockID,FlockMember[] flockMembers)
    {
        if (FameManager.FlockExist(flockID))
        {
            FlockGroup flockGroup = FameManager.GetFlockGroup(flockID);
            int[] agentID = new int[flockMembers.Length];
            for (int i = 0; i < flockMembers.Length; i++)
            {
                flockMembers[i].ChangeFlockID(flockGroup);
                agentID[i] = flockMembers[i].agentID;
            }

            FameManager.TransferAgent(agentID, flockID, true);

        }
        else
        {
            UnityEngine.Debug.LogError(String.Format("FlockGroup with ID:{0} does not exist.", flockID));
        }
    }

    protected virtual void OnDestroy()
    {
        if (init)
        {
            FameManager.RemoveAgent(agentID);
        }
    }
}
