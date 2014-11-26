using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public enum ObstacleType{
	Round,
	Polygon
}

public class FameObstacle : MonoBehaviour {

    private bool init = false;
    public bool IsInit { get { return init; } }
    
	[SerializeField]
    private int obstacleID = -1;
    /// <summary>
    /// the unique id to identify the obstacle
    /// </summary>
    public int ObstacleID{
        get { return obstacleID; }
        set { obstacleID = value; }
    }
	
	[SerializeField]
    private ObstacleType obstacleType = ObstacleType.Round;
    /// <summary>
    /// the type of obstacle (round or polygonal)
    /// </summary>
    public ObstacleType ObstacleType{
        get { return obstacleType; }
        set
        {
            if (init)
            {
                Debug.LogWarning("Not allowed to change obstacle type after initialization.");
            }
            else
            {
                obstacleType = value; 
            }
        }
    }
	
	[SerializeField]
    private float obstacleRadius = 40;
    /// <summary>
    /// radius of the obstacle (round type)
    /// </summary>
    public float ObstacleRadius{
        get { return obstacleRadius; }
        set
        {
            obstacleRadius = value;
            if (init)
            {

            }
        }
    }
	
	[SerializeField]
    private List<Vector3> obstaclePoints = new List<Vector3>(){
       new Vector3(0,0,0),
       new Vector3(100,0,0),
       new Vector3(0,0,100),
    };

    [SerializeField]
    private byte collisionFlag = 0;
    /// <summary>
    /// The collision flag for the obstacle
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
    /// <summary>
    /// Turn on the flag for certain bits specified. (bitwise OR operator is performed)
    /// </summary>
    /// <param name="flag">bits to turn off</param>
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
    /// Apply the collision flag settings to FAME
    /// </summary>
    public void ApplyCollisionFlag()
    {
        if (init) FameManager.SetObstacleCollisionFlag(obstacleID, collisionFlag);
    }

    public Vector3 Position
    {
        get
        {
            return gameObject.transform.position;
        }
    }

    /// <summary>
    /// Get the centroid of the obstacle
    /// </summary>
    /// <returns>position of the centroid</returns>
    public Vector3 GetObstacleCentroid()
    {
        return FameManager.GetObstacleCentroid(obstacleID);      
    }

    /// <summary>
    /// The list of points defining the shape of the obstacle (polygonal obstacle)
    /// </summary>
    public List<Vector3> ObstaclePoints{
        get { return obstaclePoints; }
        set { obstaclePoints = value; }
    }

    Vector3[] waypoints;
    int wp_movementDirection = 1;
    int wp_currentIndex = 0;
    float wp_speed = 40;

    /// <summary>
    /// speed at which the moving obstacle should be travelling
    /// </summary>
    public float Speed
    {
        get
        {
            return wp_speed;
        }
        set
        {
            wp_speed = value;
        }
    }

    /// <summary>
    /// set the waypoints for the obstacle
    /// </summary>
    /// <param name="waypoints"></param>
    public void SetWaypoint(Vector3[] waypoints)
    {
        if (waypoints.Length >= 2)
        {
            this.waypoints = waypoints;
            wp_currentIndex = 0;
            wp_movementDirection = 1;
        }
        else
        {
            Debug.LogWarning("Number of waypoints should be 2 or more");
        }
    }
    //remove all the waypoints
    public void ClearWaypoint()
    {
        waypoints = null;
    }

	private List<Vector3> GetWorldCoordinate(List<Vector3> vec3list){
		for (int i = 0; i < vec3list.Count; i++){
			vec3list[i] += gameObject.transform.position;
		}
		return vec3list;
	}

	
	// Use this for initialization
	protected virtual void Start () {
		if (!init)
        {
            switch (obstacleType)
            {
                case ObstacleType.Round:
                    Vector3 pos = gameObject.transform.position;
                    InitObstacle(pos, obstacleRadius);
                    break;
                case ObstacleType.Polygon:
                    obstaclePoints = GetWorldCoordinate(obstaclePoints);
                    InitObstacle(obstaclePoints.ToArray());
                    break;
            } 
        }
        FameManager.RegisterObstacle(obstacleID, this);
	}

    public virtual void InitObstacle(Vector3 position, float radius)
    {
        if (!init)
        {
            init = true;
            gameObject.transform.position = position;
            obstacleType = ObstacleType.Round;
            obstacleRadius = radius;
            obstacleID = FameManager.AddObstacles(position.x, position.z, radius);
            ApplyCollisionFlag();
        }
        else
        {
            Debug.LogError("Obstacles has been initialized");
        }
    }

    public virtual void InitObstacle(Vector3[] worldPoints) {
        if (worldPoints.Length > 2)
        {
            if (!init)
            {
                init = true;
                Vector3 centroid = FameUnityUtil.CalculateCentroid(worldPoints);
                gameObject.transform.position = centroid;
                obstacleType = ObstacleType.Polygon;
                obstacleID = FameManager.AddObstacles(worldPoints);
                obstaclePoints.Clear();
                for (int i = 0; i < worldPoints.Length; i++)
                {
                    obstaclePoints.Add(worldPoints[i] - centroid);
                }
                ApplyCollisionFlag();
            }
            else
            {
                Debug.LogError("Obstacles had been initialized, you are not allowed to initialize this class more than once");
            }
        }
        else
        {
            Debug.LogError("Number of ctrl points must be at least 3");
        }
    }

    // Update is called once per frame
    protected void Update()
    {
        if (waypoints != null)
        {
            Vector3 nextPoint = waypoints[wp_currentIndex + wp_movementDirection];
            Vector3 movementVec = nextPoint - Position;
            //movementVec.y = 0;
            float moveDist = wp_speed * Time.deltaTime;

            float distToDest = movementVec.magnitude;
    
            movementVec.Normalize();
            Vector3 deltaMovement = Vector3.zero;
            if (moveDist > distToDest)
            {
                deltaMovement = nextPoint - Position;
                wp_currentIndex += wp_movementDirection;
                if (wp_currentIndex == 0)
                {
                    wp_movementDirection = 1;
                }
                if (wp_currentIndex == waypoints.Length - 1)
                {
                    wp_movementDirection = -1;
                }
            }
            else
            {
                deltaMovement = movementVec * moveDist;
            }
            //deltaMovement.y = 0;
            FameManager.MoveObstacle(obstacleID, deltaMovement.x, deltaMovement.y, deltaMovement.z);
            gameObject.transform.position = gameObject.transform.position + deltaMovement;

        }
	}

    protected virtual void OnDestroy()
    {
        if (init)
        {
            FameManager.RemoveObstacle(obstacleID);
        }
    }
}
