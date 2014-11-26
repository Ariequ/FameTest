using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using FameCSharp;
using FameCore.Util;


public class FameManager : MonoBehaviour
{
    #region Internal_DoNotUse
    private static Dictionary<int, FlockMember> agentDictionary = new Dictionary<int, FlockMember>();
    private static Dictionary<int, FlockGroup> flockDictionary = new Dictionary<int, FlockGroup>();
    private static Dictionary<int, FameObstacle> obstacleDictionary = new Dictionary<int, FameObstacle>();
    private static Dictionary<int, FamePath> pathDictionary = new Dictionary<int, FamePath>();
    private static Dictionary<int, FameField> fieldDictionary = new Dictionary<int, FameField>();
    //Add
    /// <summary>
    /// Internal Function 
    /// </summary>
    /// <param name="x"></param>
    /// <param name="z"></param>
    /// <param name="obstacleRadius"></param>
    /// <returns></returns>
    public static int AddObstacles(float x, float z, float obstacleRadius)
    {
        return FAME.Singleton.AddObstacles(x, z, obstacleRadius);
    }
    /// <summary>
    /// Internal Function
    /// </summary>
    /// <param name="obstaclePoints"></param>
    /// <returns></returns>
    public static int AddObstacles(Vector3[] obstaclePoints)
    {
        return FAME.Singleton.AddObstacles(FameUnityUtil.Vec3ToFVec3(obstaclePoints));
    }

    //Register
    /// <summary>
    /// Internal Function
    /// </summary>
    /// <param name="fieldID"></param>
    /// <param name="field"></param>
    public static void RegisterField(int fieldID, FameField field)
    {
        fieldDictionary.Add(fieldID, field);
    }
    /// <summary>
    /// Internal Function
    /// </summary>
    /// <param name="agentID"></param>
    /// <param name="member"></param>
    public static void RegisterAgent(int agentID, FlockMember member)
    {
        agentDictionary.Add(agentID, member);
    }
    /// <summary>
    /// Internal Function
    /// </summary>
    /// <param name="obstacleID"></param>
    /// <param name="obstacle"></param>
    public static void RegisterObstacle(int obstacleID, FameObstacle obstacle)
    {
        obstacleDictionary.Add(obstacleID, obstacle);
    }
    /// <summary>
    /// Internal Function
    /// </summary>
    /// <param name="flockID"></param>
    /// <param name="group"></param>
    public static void RegisterFlock(int flockID, FlockGroup group)
    {
        flockDictionary.Add(flockID, group);
    }
    /// <summary>
    /// Internal Function
    /// </summary>
    /// <param name="pathID"></param>
    /// <param name="path"></param>
    public static void RegisterPath(int pathID, FamePath path)
    {
        pathDictionary.Add(pathID, path);
    }


    //Remove
    /// <summary>
    /// Internal Function
    /// </summary>
    /// <param name="obstacleID"></param>
    public static void RemoveObstacle(int obstacleID)
    {
        bool hasItem = obstacleDictionary.Remove(obstacleID);
        if (hasItem) FAME.Singleton.RemoveObstacle(obstacleID);
    }
    /// <summary>
    /// Internal Function
    /// </summary>
    /// <param name="fieldID"></param>
    public static void RemoveField(int fieldID)
    {
        bool hasItem = fieldDictionary.Remove(fieldID);
        if (hasItem) FAME.Singleton.RemoveField(fieldID);
    }
    /// <summary>
    /// Internal Function
    /// </summary>
    /// <param name="agentID"></param>
    public static void RemoveAgent(int agentID)
    {
        bool hasItem = agentDictionary.Remove(agentID);
        if (hasItem) FAME.Singleton.RemoveAgent(agentID);
    }
    /// <summary>
    /// Internal Function
    /// </summary>
    /// <param name="flockID"></param>
    public static void RemoveFlock(int flockID)
    {
        bool hasItem = flockDictionary.Remove(flockID);
        if (hasItem) FAME.Singleton.RemoveFlock(flockID);
    }

    /// <summary>
    /// Internal Function
    /// </summary>
    /// <param name="pathID"></param>
    public static void RemovePath(int pathID)
    {
        bool hasItem = pathDictionary.Remove(pathID);
        if (hasItem) FAME.Singleton.DeletePath(pathID);
    }

    /// <summary>
    /// Internal Function
    /// </summary>
    /// <param name="flockID"></param>
    /// <param name="pos"></param>
    /// <param name="relaxFormation"></param>
    /// <returns></returns>
    public static int CreateAgent(int flockID, Vector3 pos, bool relaxFormation)
    {
        return FAME.Singleton.CreateAgent(flockID, pos.x, pos.y, pos.z, relaxFormation);
    }

    /// <summary>
    /// Internal Function
    /// </summary>
    /// <param name="flockType"></param>
    /// <param name="pos"></param>
    /// <returns></returns>
    public static int CreateAgent(FlockType flockType, Vector3 pos)
    {
        return FAME.Singleton.CreateAgent(flockType, pos.x, pos.y, pos.z);
    }

    //Set Attribute
    /// <summary>
    /// Internal Function. Call it in FlockMember instead
    /// </summary>
    /// <param name="agentID"></param>
    /// <param name="attribute"></param>
    /// <param name="value"></param>
    public static void SetAgentAttribute(int agentID, AgentAttribute attribute, float value)
    {
        FAME.Singleton.SetAgentAttributeAgent(agentID, System.Enum.GetName(typeof(AgentAttribute), attribute), value);
    }

    /// <summary>
    /// Internal Function. Call it in FlockGroup instead
    /// </summary>
    /// <param name="flockID"></param>
    /// <param name="attribute"></param>
    /// <param name="value"></param>
    public static void SetAgentAttributeGroup(int flockID, AgentAttribute attribute, float value)
    {
        FAME.Singleton.SetAgentAttributeGroup(flockID, System.Enum.GetName(typeof(AgentAttribute), attribute), value);
    }

    /// <summary>
    /// Internal Function. Call this within FlockMember instead.
    /// </summary>
    /// <param name="agentID"></param>
    /// <param name="fieldFlag"></param>
    public static void SetAgentFieldFlag(int agentID, byte fieldFlag)
    {
        FAME.Singleton.SetAgentFieldFlag(agentID, fieldFlag);
    }

    /// <summary>
    /// Internal Function. Call this within FlockMember instead.
    /// </summary>
    /// <param name="agentID"></param>
    /// <param name="collisionFlag"></param>
    public static void SetAgentCollisionFlag(int agentID, byte collisionFlag)
    {
        FAME.Singleton.SetAgentCollisionFlag(agentID, collisionFlag);
    }
    #endregion




    //Exists
    /// <summary>
    /// Check whether a flockID exist
    /// </summary>
    /// <param name="flockID">flockID</param>
    /// <returns>whether the flockID exist</returns>
    public static bool FlockExist(int flockID)
    {
        return flockDictionary.ContainsKey(flockID);
    }
    /// <summary>
    /// Check whether a agentID exist
    /// </summary>
    /// <param name="agentID">agentID</param>
    /// <returns>whether the agentID exist</returns>
    public static bool AgentExist(int agentID)
    {
        return agentDictionary.ContainsKey(agentID);
    }
    /// <summary>
    /// Check whether a FameField exist
    /// </summary>
    /// <param name="fieldID">fame field id</param>
    /// <returns>whether the famefield exist</returns>
    public static bool FieldExist(int fieldID)
    {
        return fieldDictionary.ContainsKey(fieldID);
    }
    /// <summary>
    /// Check whether a FamePath exist
    /// </summary>
    /// <param name="pathID">path id</param>
    /// <returns>whether the FamePath exist</returns>
    public static bool PathExist(int pathID)
    {
        return pathDictionary.ContainsKey(pathID);
    }
    /// <summary>
    /// Check whether a FameObstacle exist
    /// </summary>
    /// <param name="obstacleID">obstacle ID</param>
    /// <returns>whether the FameObstacle exist</returns>
    public static bool ObstacleExist(int obstacleID)
    {
        return obstacleDictionary.ContainsKey(obstacleID);
    }

    //Gets
    /// <summary>
    /// Get the FlockGroup object
    /// </summary>
    /// <param name="flockID">flockID</param>
    /// <returns>the flockGroup, null if flockGroup does not exist</returns>
    public static FlockGroup GetFlockGroup(int flockID)
    {
        if (flockDictionary.ContainsKey(flockID))
        {
            return flockDictionary[flockID];
        }
        else
        {
            Debug.LogError(String.Format("Flock with ID {0} does not exist.", flockID));
            return null;
        }
    }
    /// <summary>
    /// Get the FlockMember object
    /// </summary>
    /// <param name="agentID">flock member id</param>
    /// <returns>the FlockMember, null if flockMember does not exist</returns>
    public static FlockMember GetFlockMember(int agentID)
    {
        if (agentDictionary.ContainsKey(agentID))
        {
            return agentDictionary[agentID];
        }
        else
        {
            Debug.LogError(String.Format("Agent with ID {0} does not exist.", agentID));
            return null;
        }
    }
    /// <summary>
    /// Get the FamePath object
    /// </summary>
    /// <param name="pathID">the path ID</param>
    /// <returns>FamePath object, null is famePath does not exist</returns>
    public static FamePath GetPath(int pathID)
    {
        if (pathDictionary.ContainsKey(pathID))
        {
            return pathDictionary[pathID];
        }
        else
        {
            Debug.LogError(String.Format("Path with ID {0} does not exist.", pathID));
            return null;
        }
    }
    /// <summary>
    /// Get the FameObstacle object
    /// </summary>
    /// <param name="obstacleID">obstacleID</param>
    /// <returns>FameObject, null if FameObstacle does no exist</returns>
    public static FameObstacle GetObstacle(int obstacleID)
    {
        if (obstacleDictionary.ContainsKey(obstacleID))
        {
            return obstacleDictionary[obstacleID];
        }
        else
        {
            Debug.LogError(String.Format("Obstacle with ID {0} does not exist.", obstacleID));
            return null;
        }
    }
    /// <summary>
    /// Get the FameField object
    /// </summary>
    /// <param name="fieldID">Famefield ID</param>
    /// <returns>FameField, null if FameField does not exist</returns>
    public static FameField GetField(int fieldID)
    {
        if (fieldDictionary.ContainsKey(fieldID))
        {
            return fieldDictionary[fieldID];
        }
        else
        {
            Debug.LogError(String.Format("Field with ID {0} does not exist.", fieldID));
            return null;
        }
    }

    /// <summary>
    /// Get the list of Agent IDs belonging to the flock
    /// </summary>
    /// <param name="flockID">flockID to query</param>
    /// <returns>array of flockMember IDs (agent IDs) belonging to the flockGroup</returns>
    public static int[] GetAgentIDinFlock(int flockID)
    {
        return FAME.Singleton.GetAgentIDinFlock(flockID);
    }

    //Point in Query
    /// <summary>
    /// Check whether the input points is within any FlockGroup's formation shape.
    /// Useful or handling inputs from the game (example mouse click or touch)
    /// </summary>
    /// <param name="x">x coordinate in the world coordinate system</param>
    /// <param name="z">z coordinate in the world coordinate system</param>
    /// <returns>flock ID if any, -1 if no flockGroup at the specified point</returns>
    public static int PointInFlock(float x, float z)
    {
        return FAME.Singleton.PointInFlock(x, z);
    }
    /// <summary>
    /// Check whether the input points is within any Obstacle shape.
    /// Useful or handling inputs from the game (example mouse click or touch)
    /// </summary>
    /// <param name="x">x coordinate in the world coordinate system</param>
    /// <param name="z">z coordinate in the world coordinate system</param>
    /// <returns>obstacle id if any, -1 if no fameObstacle at the specified point</returns>
    public static int PointInObstacle(float x, float z)
    {
        return FAME.Singleton.PointInObstacle(x, z);
    }
    /// <summary>
    /// Check whether the input points is within any FameField.
    /// Useful or handling inputs from the game (example mouse click or touch)
    /// </summary>
    /// <param name="x">x coordinate in the world coordinate system</param>
    /// <param name="z">z coordinate in the world coordinate system</param>
    /// <returns>FameField id if any, -1 if no FameField at the specified point</returns>
    public static int PointInField(float x, float z)
    {
        return FAME.Singleton.PointInField(x, z);
    }
    /// <summary>
    /// Get the list of agents within the shape defined. 
    /// Useful for handling inputs from the game, i.e. selecting units
    /// </summary>
    /// <param name="polygonShape">Shape of the region to query. Must have 3 or more points</param>
    /// <returns>an array of agentID within the region specified</returns>
    public static int[] QueryAgents(Vector3[] polygonShape)
    {
        if (polygonShape.Length >= 3)
        {
            FVec3[] fVec3 = FameUnityUtil.Vec3ToFVec3(polygonShape);
            return FAME.Singleton.QueryAgents(fVec3);
        }
        else
        {
            Debug.LogError("The specified polygon shape to query must contain 3 or more points");
            return new int[0];
        }
    }

    //GetIDs
    /// <summary>
    /// Get all the flockIDs
    /// </summary>
    /// <returns>array of all the flockIDs</returns>
    public static int[] GetFlockIDs()
    {
        return flockDictionary.Keys.ToArray();
    }
    /// <summary>
    /// Get all the flockMember ids
    /// </summary>
    /// <returns>array of all the flockMember ids</returns>
    public static int[] GetFlockMemberIDs()
    {
        return agentDictionary.Keys.ToArray();
    }
    /// <summary>
    /// Get all the path ids
    /// </summary>
    /// <returns>array of all the path ids</returns>
    public static int[] GetPathIDs()
    {
        return pathDictionary.Keys.ToArray();
    }
    /// <summary>
    /// Get all the obstacle ids
    /// </summary>
    /// <returns>array of all the obstacle ids</returns>
    public static int[] GetObstacleIDs()
    {
        return obstacleDictionary.Keys.ToArray();
    }
    /// <summary>
    /// Get all the FameField IDs
    /// </summary>
    /// <returns>array of all the FameField ids</returns>
    public static int[] GetFieldIDs()
    {
        return fieldDictionary.Keys.ToArray();
    }


    //Nums
    /// <summary>
    /// Number of FamePaths in the scene
    /// </summary>
    /// <returns>Number of FamePaths in the scene</returns>
    public static int NumPaths()
    {
        return pathDictionary.Count;
    }
    /// <summary>
    /// Number of FlockGroup in the scene
    /// </summary>
    /// <returns>Number of FlockGroup in the scene</returns>
    public static int NumFlockGroup()
    {
        return flockDictionary.Count;
    }
    /// <summary>
    /// Number of FlockMember in the scene
    /// </summary>
    /// <returns>Number of FlockMember in the scene</returns>
    public static int NumAgents()
    {
        return agentDictionary.Count;
    }
    /// <summary>
    /// Number of FameObstacles in the scene
    /// </summary>
    /// <returns>Number of FameObstacles in the scene</returns>
    public static int NumObstacles()
    {
        return obstacleDictionary.Count;
    }
    /// <summary>
    /// Number of FameFields in the scene
    /// </summary>
    /// <returns>Number of FameFields in the scene</returns>
    public static int NumFields()
    {
        return fieldDictionary.Count;
    }


    //GetSet ControlPoints
    /// <summary>
    /// Get the array of points for the path spline
    /// </summary>
    /// <param name="pathID">famePath id</param>
    /// <returns>an array of points for the path spline</returns>
    public static Vector3[] GetPathSpline(int pathID)
    {
        float[][] path = FAME.Singleton.GetPathSpline(pathID);
        Vector3[] result = new Vector3[path.Length];
        for (int i = 0; i < path.Length; i++)
        {
            result[i] = new Vector3(path[i][0], 0, path[i][1]);
        }
        return result;
    }

    /// <summary>
    /// Set the position of the control point for the famepath
    /// </summary>
    /// <param name="pathID">FamePath id</param>
    /// <param name="index">index of the control point to move</param>
    /// <param name="worldPos">postion of the control point</param>
    public static void SetPathCtrlPoint(int pathID, int index, Vector3 worldPos)
    {
        FAME.Singleton.SetPathCtrlPoint(pathID, index, worldPos.x, worldPos.z);
    }

    /// <summary>
    /// Get the array of points for the path control point
    /// </summary>
    /// <param name="pathID">pathID</param>
    /// <returns>array of positions for the control points</returns>
    public static Vector3[] GetPathCtrlPoint(int pathID)
    {
        float[][] path = FAME.Singleton.GetPathControlPoint(pathID);
        Vector3[] result = new Vector3[path.Length];
        for (int i = 0; i < path.Length; i++)
        {
            result[i] = new Vector3(path[i][0], 0, path[i][1]);
        }
        return result;
    }

    /// <summary>
    /// Get the control points for the flockGroup that defines the formation shape
    /// </summary>
    /// <param name="flockID">flockID</param>
    /// <returns>array of control points</returns>
    public static Vector3[] GetFlockWorldCtrlPoint(int flockID)
    {
        FVec3[] arr = FAME.Singleton.GetFlockWorldCtrlPoint(flockID);
        return FameUnityUtil.FVec3ToVec(arr);
    }

    //Create
    /// <summary>
    /// Creates a gameobject with FameObstacle (or a subclass of FameObstacle) attached. Polygonal Type Obstacle
    /// </summary>
    /// <param name="points">points defining the shape of the polygonal obstacle</param>
    /// <param name="fameObstacleType">FameObstacle type or its subclass</param>
    /// <returns>FameObstacle component created</returns>
    public static FameObstacle CreateObstacle(Vector3[] points, Type fameObstacleType)
    {
        if (typeof(FameObstacle).IsAssignableFrom(fameObstacleType))
        {
            GameObject obstacleObject = new GameObject("Fame_Obstacle");
            FameObstacle fameObstacle = obstacleObject.AddComponent(fameObstacleType) as FameObstacle;
            fameObstacle.InitObstacle(points);
            return fameObstacle;
        }
        else
        {
            throw new Exception("Input type of be FameObstacle or a subclass of FameObstacle");
        }
    }

    /// <summary>
    /// Creates a gameobject with FameObstacle (or a subclass of FameObstacle) attached. Round Type Obstacle
    /// </summary>
    /// <param name="position">centroid of the round obstacle</param>
    /// <param name="radius">radius of the round obstacle</param>
    /// <param name="fameObstacleType">FameObstacle type or its subclass</param>
    /// <returns>FameObstacle component created</returns>
    public static FameObstacle CreateObstacle(Vector3 position, float radius, Type fameObstacleType)
    {
        if (typeof(FameObstacle).IsAssignableFrom(fameObstacleType))
        {
            GameObject obstacleObject = new GameObject("Fame_Obstacle");
            FameObstacle fameObstacle = obstacleObject.AddComponent(fameObstacleType) as FameObstacle;
            fameObstacle.InitObstacle(position, radius);
            return fameObstacle;
        }
        else
        {
            Debug.LogError("Input type of be FameObstacle or a subclass of FameObstacle");
            return null;
        }
    }
    /// <summary>
    /// Creates a gameobject with FameField (or a subclass of FameField) attached. Circular type
    /// </summary>
    /// <param name="position">centroid of the FameField</param>
    /// <param name="radius">radius of the fameField</param>
    /// <param name="magnitude">strength of the fameField</param>
    /// <param name="dir">direction of the force</param>
    /// <param name="fameFieldType">FameField type or its subclass</param>
    /// <returns>FameField component created</returns>
    public static FameField CreateField(Vector3 position, float radius, float magnitude, FameCircularDirection dir, Type fameFieldType)
    {
        if (typeof(FameField).IsAssignableFrom(fameFieldType))
        {
            GameObject fieldObject = new GameObject("Fame_Field");
            FameField famefield = fieldObject.AddComponent(fameFieldType) as FameField;
            famefield.InitField(position, radius, magnitude, dir);
            return famefield;
        }
        else
        {
            Debug.LogError("Input type of be FameField or a subclass of FameField");
            return null;
        }
    }

    /// <summary>
    /// Creates a gameobject with FameField (or a subclass of FameField) attached. Square type with directional force
    /// </summary>
    /// <param name="position">center position of the field</param>
    /// <param name="width">width of the field (x-axis length)</param>
    /// <param name="height">height of the field (z-axis length)</param>
    /// <param name="magnitude">strength of the field</param>
    /// <param name="forceDirectionAngleRad">direction in radian (0 -> force pointing to the right </param>
    /// <param name="fameFieldType">FameField type or its subclass</param>
    /// <returns>FameField component created</returns>
    public static FameField CreateField(Vector3 position, float width, float height, float magnitude, float forceDirectionAngleRad, Type fameFieldType)
    {
        if (typeof(FameField).IsAssignableFrom(fameFieldType))
        {
            GameObject fieldObject = new GameObject("Fame_Field");
            FameField famefield = fieldObject.AddComponent(fameFieldType) as FameField;
            famefield.InitField(position, width, height, magnitude, forceDirectionAngleRad);
            return famefield;
        }
        else
        {
            Debug.LogError("Input type of be FameField or a subclass of FameField");
            return null;
        }
    }

    /// <summary>
    /// Creates a gameobject with FamePath (or a subclass of FamePath) attached.
    /// </summary>
    /// <param name="points">list of points defining the path</param>
    /// <param name="famePathType">the type of FamePath component or its subclass </param>
    /// <returns>FamePath that is attached to the gameobject</returns>
    public static FamePath CreatePath(Vector3[] points, Type famePathType)
    {
        if (typeof(FamePath).IsAssignableFrom(famePathType))
        {
            FVec3[] pathPoints = FameUnityUtil.Vec3ToFVec3(points);
            float[] weights = new float[points.Length];
            for (int i = 0; i < points.Length; i++)
            {
                weights[i] = 200f;
            }
            int pathID = FAME.Singleton.CreatePath(pathPoints, weights);
            GameObject pathGameObject = new GameObject("Path_" + pathID.ToString());
            FamePath famePath = pathGameObject.AddComponent(famePathType) as FamePath;
            float[][] pathSpline = FAME.Singleton.GetPathSpline(pathID);

            famePath.InitialisePath(pathID, points, pathSpline);
            pathDictionary.Add(pathID, famePath);
            return famePath;
        }
        else
        {
            Debug.LogError("Input type of be FamePath or a subclass of FamePath");
            return null;
        }
    }

    /// <summary>
    /// Create a gameobject with FlockGroup (or a subclass of FlockGroup) attached. The FlockGroup is returned
    /// </summary>
    /// <param name="shape">the shape of the formation</param>
    /// <param name="flockType">the type of flock, ground or flying</param>
    /// <param name="flockComponentType">The flockgroup type or it's subclass</param>
    /// <param name="agents">list of agents to be transfered to this group</param>
    /// <returns></returns>
    public static FlockGroup CreateFlock(Vector3[] shape, FlockType flockType, Type flockComponentType, float meshHeight = 10, int[] agents = null)
    {
        if (typeof(FlockGroup).IsAssignableFrom(flockComponentType))
        {
            GameObject newFlockObj = new GameObject("Flock");
            FlockGroup group = newFlockObj.AddComponent(flockComponentType) as FlockGroup;
            group.InitFlock(shape, flockType, meshHeight);

            if (agents != null)
            {
                for (int i = 0; i < agents.Length; i++)
                {
                    FlockMember member = GetFlockMember(agents[i]);
                    if (member.FlockType == flockType)
                    {
                        member.ChangeFlockID(group);
                    }
                }
                FAME.Singleton.TransferAgent(agents, group.FlockID);
            }
            return group;
        }
        else
        {
            throw new Exception("flockComponentType must be FlockGroup a type");
        }
    }


    /// <summary>
    /// Reset the scene. 
    /// Destroy all FAME related objects 
    /// (FlockGroup, FlockMember, FameObstacle, FameField, FamePath)
    /// Clear everything in FAME library. 
    /// </summary>
    public static void ResetScene()
    {
        FameObstacle[] obstacleList = obstacleDictionary.Values.ToArray();
        foreach (FameObstacle obs in obstacleList)
        {
            DestroyImmediate(obs.gameObject);
        }
        obstacleDictionary.Clear();

        FamePath[] pathList = pathDictionary.Values.ToArray();
        foreach (FamePath path in pathList)
        {
            DestroyImmediate(path.gameObject);
        }
        pathDictionary.Clear();

        FlockMember[] memberList =  agentDictionary.Values.ToArray();
        foreach (FlockMember member in memberList)
        {
            DestroyImmediate(member.gameObject);
        }
        agentDictionary.Clear();

        FlockGroup[] groupList = flockDictionary.Values.ToArray();
        foreach (FlockGroup group in groupList)
        {
            DestroyImmediate(group.gameObject);
        }
        flockDictionary.Clear();
        FAME.Singleton.Reinit();
    }

    /// <summary>
    /// Create a flock of agent from existing flock members. The shape created will be a convex hull containing all the agent's current position. It will only transfer the agent if the flockType is the same as specified.
    /// </summary>
    /// <param name="flockComponentType">a FlockGroup type or its subclass</param>
    /// <param name="flockType">the type of flock to be created, ground or flying</param>
    /// <param name="agents">the list of agents that will belong to the new group. Note! Do not mix flying and ground units</param>
    /// <returns>newly created FlockGroup object, the list of agents object will be attached to this new gameobject</returns>
    public static FlockGroup CreateFlock(Type flockComponentType, FlockType flockType, int[] agents)
    {
        if (agents.Length == 0)
        {
            UnityEngine.Debug.LogError("number of agents cannot be 0");
            return null;
        }

        if (typeof(FlockGroup).IsAssignableFrom(flockComponentType))
        {
            GameObject newFlockObj = new GameObject("Flock");
            FlockGroup group = newFlockObj.AddComponent(flockComponentType) as FlockGroup;
            Vector3[] agentPosition = new Vector3[agents.Length];
            for (int i = 0; i < agents.Length; i++)
            {
                FlockMember member = GetFlockMember(agents[i]);
                agentPosition[i] = member.CurrentPosition;
            }
            if (agents.Length == 1)
            {
                FVec3[] hull = new FVec3[4];
                float r = FAME.Singleton.GetAgentAttribute(agents[0], "mRadius");
                if(r < 1f){ r = 1f; }
                hull[0] = new FVec3(agentPosition[0].x - r, agentPosition[0].y, agentPosition[0].z - r);
                hull[1] = new FVec3(agentPosition[0].x + r, agentPosition[0].y, agentPosition[0].z - r);
                hull[2] = new FVec3(agentPosition[0].x + r, agentPosition[0].y, agentPosition[0].z + r);
                hull[3] = new FVec3(agentPosition[0].x - r, agentPosition[0].y, agentPosition[0].z + r);
                group.InitFlock(hull, flockType);
            }
            else if (agents.Length == 2)
            {
                FVec3[] hull = new FVec3[4];
                Vector3 p1 = new Vector3(Mathf.Min(agentPosition[0].x,agentPosition[1].x), (agentPosition[0].y + agentPosition[1].y) / 2, Mathf.Min(agentPosition[0].z,agentPosition[1].z));
                Vector3 p2 = new Vector3(Mathf.Max(agentPosition[0].x,agentPosition[1].x), (agentPosition[0].y + agentPosition[1].y) / 2, Mathf.Max(agentPosition[0].z,agentPosition[1].z));
                float r = FAME.Singleton.GetAgentAttribute(agents[0], "mRadius");
                if ((p2.x - p1.x) < 2 * r)
                {
                    p1.x -= r;
                    p2.x += r;
                }
                if ((p2.z - p1.z) < 2 * r)
                {
                    p1.z -= r;
                    p2.z += r;
                }
                hull[0] = new FVec3(p1.x, p1.y, p1.z);
                hull[1] = new FVec3(p2.x, p1.y, p1.z);
                hull[2] = new FVec3(p2.x, p1.y, p2.z);
                hull[3] = new FVec3(p1.x, p1.y, p2.z);
                group.InitFlock(hull, flockType);
            }
            else
            {
                FVec3[] hull = FAME.Singleton.CalculateConvexHull(FameUnityUtil.Vec3ToFVec3(agentPosition));
                group.InitFlock(hull, flockType);
            }


            if (agents != null)
            {
                List<int> agentToTransfer = new List<int>();
                for (int i = 0; i < agents.Length; i++)
                {
                    FlockMember member = GetFlockMember(agents[i]);
                    if (member.FlockType == flockType) {
                        member.ChangeFlockID(group);
                        agentToTransfer.Add(agents[i]);
                    }
                }
                FAME.Singleton.TransferAgent(agentToTransfer.ToArray(), group.FlockID);
            }
            return group;
        }
        else
        {
            throw new Exception("flockComponentType must be FlockGroup a type");
        }
    }



    //Others
    /// <summary>
    /// Get the destination of the specified agent
    /// </summary>
    /// <param name="agentID">agent id</param>
    /// <returns>the destination position</returns>
    public static Vector3 GetAgentDestination(int agentID)
    {
        FVec3 result = FAME.Singleton.GetAgentDestination(agentID);
        return new Vector3(result.x, result.y, result.z);
    }
    /// <summary>
    /// Set the destination for a agent. Note that the agent cannot belong to any group.
    /// </summary>
    /// <param name="agentID">agent id</param>
    /// <param name="position">destination position</param>
    public static void MoveAgent(int agentID, Vector3 position)
    {
        // do not allow movement of agents that belongs to a group
        FlockMember member = GetFlockMember(agentID);
        if (member.FlockID == -1)
        {
            FAME.Singleton.MoveAgent(agentID, position.x, position.y, position.z);
        }
        else
        {
            Debug.LogError("A flock member belonging to a group cannot leave its formation. Try leave group first before calling this function");
        }
    }

    /// <summary>
    /// Instantaneously move the agent to its destination.
    /// </summary>
    /// <param name="agentID">agent id to move</param>
    public static void TeleportAgentToSamplePoint(int agentID)
    {
        FAME.Singleton.TeleportAgentToSamplePoint(agentID);
    }

    /// <summary>
    /// Instantaneously move the agent to the specified position.
    /// </summary>
    /// <param name="agentID">agent ID</param>
    /// <param name="x">x coordinate</param>
    /// <param name="z">z coordinate</param>
    public static void TeleportAgent(int agentID, float x, float z)
    {
        FAME.Singleton.TeleportAgent(agentID, x, z);
    }
    /// <summary>
    /// Instantaneously move the agent to the specified position.
    /// </summary>
    /// <param name="agentID">agent ID</param>
    /// <param name="position">position</param>
    public static void TeleportAgent(int agentID, Vector3 position)
    {
        FAME.Singleton.TeleportAgent(agentID, position.x, position.y, position.z);
    }

    /// <summary>
    /// Get the centroid of the specified obstacle
    /// </summary>
    /// <param name="obstacleID">obstacle ID</param>
    /// <returns>the centroid of the obstacle</returns>
    public static Vector3 GetObstacleCentroid(int obstacleID)
    {
        FVec3 obstacleCenter = FAME.Singleton.GetObstacleCentroid(obstacleID);
        return new Vector3(obstacleCenter.x, obstacleCenter.y, obstacleCenter.z);
    }

    /// <summary>
    /// Set the collision flag for the obstacle
    /// </summary>
    /// <param name="obstacleID">obstacleID</param>
    /// <param name="collisionFlag">collision flag to apply</param>
    public static void SetObstacleCollisionFlag(int obstacleID, byte collisionFlag){
        FAME.Singleton.SetObstacleCollisionFlag(obstacleID, collisionFlag);
    }

    /// <summary>
    /// Translate the obstacle by a vector
    /// </summary>
    /// <param name="obstacleID">obstacle ID</param>
    /// <param name="dx">move direction in x-axis</param>
    /// <param name="dy">move direction in y-axis</param>
    /// <param name="dz">move direction in z-axis</param>
    public static void MoveObstacle(int obstacleID, float dx, float dy, float dz)
    {
        FAME.Singleton.MoveObstacle(obstacleID, dx, dy, dz);
    }

    /// <summary>
    /// Set the orientation of the flockMember
    /// </summary>
    /// <param name="agentID">flockMember id</param>
    /// <param name="x">normalised x direction</param>
    /// <param name="y">normalised y direction</param>
    /// <param name="z">normalised z direction</param>
    public static void SetAgentOrientation(int agentID, float x, float y, float z)
    {
        FAME.Singleton.SetAgentOrientation(agentID, x, y, z);
    }
    /// <summary>
    /// enable or disable the agent. if disabled, the agent will not move and other agents will not see this agent as well 
    /// </summary>
    /// <param name="agentID">agent id</param>
    /// <param name="enable">enable or disable</param>
    public static void SetEnableAgent(int agentID, bool enable)
    {
        FAME.Singleton.SetEnableAgent(agentID, enable);
    }

    /// <summary>
    /// enable or disable agent movement. if disabled, the agent will not move and but the other agents will be able to see this agent and avoid them 
    /// </summary>
    /// <param name="agentID">agent id</param>
    /// <param name="enable">enable or disable</param>
    public static void SetEnableAgentMovement(int agentID, bool enable)
    {
        FAME.Singleton.SetAgentMovement(agentID, enable);
    }

    /// <summary>
    /// Internal Function, call this in flockGroup or flockMember instead
    /// </summary>
    /// <param name="agentID"></param>
    /// <param name="behavior"></param>
    /// <param name="radius"></param>
    /// <param name="weight"></param>
    public static void AddSteeringBehaviorAgent(int agentID, SteeringBehaviors behavior, float radius, float weight)
    {
        FAME.Singleton.AddSteeringBehaviorAgent(agentID, System.Enum.GetName(typeof(SteeringBehaviors),
                    behavior), radius, weight);
    }
    /// <summary>
    /// Internal function. Call this in flockGroup or flockMember instead
    /// </summary>
    /// <param name="agentID"></param>
    public static void RemoveAllSteeringBehaviorAgent(int agentID)
    {
        FAME.Singleton.RemoveAllSteeringBehaviorAgent(agentID);
    }
    /// <summary>
    /// Internal function. Call this in flockGroup or flockMember instead
    /// </summary>
    /// <param name="agentID"></param>
    public static void RemoveSteeringBehaviorAgent(int agentID, SteeringBehaviors behavior)
    {
        FAME.Singleton.RemoveSteeringBehaviorAgent(agentID, System.Enum.GetName(typeof(SteeringBehaviors), behavior));
    }
    /// <summary>
    /// Internal function. Call this in flockGroup or flockMember instead
    /// </summary>
    /// <param name="agentID"></param>
    public static void EditSteeringBehaviorAgent(int agentID, SteeringBehaviors behavior, float radius, float weight)
    {
        FAME.Singleton.EditSteeringBehaviourAgent(agentID, System.Enum.GetName(typeof(SteeringBehaviors), behavior), radius, weight);
    }
    /// <summary>
    /// Internal Function. Call this within the FlockMember instead!
    /// </summary>
    /// <param name="agentID"></param>
    public static void LeaveGroup(int agentID)
    {
        FAME.Singleton.LeaveGroup(agentID);
    }
    /// <summary>
    /// Internal Function. Call this in FlockMember instead
    /// </summary>
    /// <param name="agentIDs"></param>
    /// <param name="flockID"></param>
    /// <param name="retainBehavior"></param>
    public static void TransferAgent(int[] agentIDs, int flockID, bool retainBehavior)
    {
        FAME.Singleton.TransferAgent(agentIDs, flockID, retainBehavior);
    }

    /// <summary>
    /// Initialise the area of interest in FAME. This is extremely import!
    /// Any FAME Object outside the area may have undesired effect.
    /// Internally, FAME will do spatial partitioning to speed things up. 
    /// </summary>
    /// <param name="area">A rect defining the boundary of the crowd</param>
    public static void InitFAMEArea(Rect area)
    {
        FAME.Singleton.InitFameArea(area.xMin, area.yMin, area.width, area.height);
    }

    private static FameManager mSingleton;
    public static FameManager Singleton{
        get
        {
            return mSingleton;
        }
    }
    void Awake()
    {
        
        if (mSingleton == null)
        {
            mSingleton = this;
        }
        else
        {
            Debug.LogError("You are not allowed to create the FlockController instance twice.");
        }
    }

    #region FAMESettings

    [SerializeField]
    bool applyTerrainEffect = FameCSharp.FameSettings.ApplyTerrainEffect;
    public bool EnableTerrainEffect
    {
        get
        {
            return applyTerrainEffect;
        }
        set
        {
            applyTerrainEffect = value;
        }
    }
    [SerializeField]
    bool switchAgentPosition = FameCSharp.FameSettings.SwitchAgentPosition;
    public bool SwitchAgentPosition
    {
        get { return switchAgentPosition; }
        set
        {
            switchAgentPosition = value;
        }
    }
    
    [SerializeField]
    private float ignoreMinSpeed = FameCSharp.FameSettings.ignoreMinSpeed;
    public float IgnoreMinSpeed
    {
        get { return ignoreMinSpeed; }
        set { ignoreMinSpeed = value; }
    }

    [SerializeField]
    private float accelerationThreshold = FameCSharp.FameSettings.accelerationThreshold;
    public float AccelerationThreshold
    {
        get { return accelerationThreshold; }
        set { accelerationThreshold = value; }
    }

    [SerializeField]
    private float slowingDistance = FameCSharp.FameSettings.slowingDistance;
    public float SlowingDistance
    {
        get { return slowingDistance; }
        set { slowingDistance = value; }
    }

    #endregion

    #region FAMETerrainSettings

    [SerializeField]
    private float terrainWidth = 2000;
    public float TerrainWidth
    {
        get { return terrainWidth; }
        set { terrainWidth = value; }
    }

    [SerializeField]
    private float terrainLength = 2000;

    public float TerrainLength
    {
        get { return terrainLength; }
        set { terrainLength = value; }
    }


    [SerializeField]
    private float terrainHeight = 200;
    public float TerrainHeight
    {
        get { return terrainHeight; }
        set { terrainHeight = value; }
    }

    [SerializeField]
    private bool getInfoFromUnityTerrain = true;
    public bool GetInfoFromUnityTerrain
    {
        get { return getInfoFromUnityTerrain; }
        set { getInfoFromUnityTerrain = value; }
    }

    [SerializeField]
    private Texture2D terrainHeightMap;
    public Texture2D TerrainHeightMap
    {
        get { return terrainHeightMap; }
        set
        {
            if (value != null)
            {
                if (value.height == value.width)
                {
                    terrainHeightMap = value;
                }
                else
                {
                    Debug.LogError("Required heightmap must have equal length and width");
                }
            }
        }
    }

    [SerializeField]
    private int resolution = 513;
    public int Resolution
    {
        get { return resolution; }

        set { resolution = value; }
    }

    [SerializeField]
    bool warpTerrain = false;
    public bool WarpTerrain
    {
        get { return warpTerrain; }
        set { warpTerrain = value; }
    }

    [SerializeField]
    Rect warpTerrainSize = new Rect(FameCSharp.FameSettings.WrapTerrain_MinX, 
        FameCSharp.FameSettings.WrapTerrain_MinY, 
        FameCSharp.FameSettings.WrapTerrain_MaxX - FameCSharp.FameSettings.WrapTerrain_MinX,
        FameCSharp.FameSettings.WrapTerrain_MaxY - FameCSharp.FameSettings.WrapTerrain_MinY);

    public Rect WarpTerrainSize
    {
        get { return warpTerrainSize; }
        set { warpTerrainSize = value; }
    }
    #endregion


    void Start()
    {
        ApplyFameSettings();
        InitFameTerrain();
        isInit = true;
    }
    bool isInit = false;
    public bool IsInit()
    {
        return isInit;
    }

    void Update()
    {
        //This will update all the flock in every time step
        FAME.Singleton.Update(Time.deltaTime);
        foreach(KeyValuePair<int, FlockMember> kvp in agentDictionary){
            FVec3 agentPosition = FAME.Singleton.GetAgentPosition(kvp.Key);
            Vector3 pos = FameUnityUtil.FVec3ToVec(agentPosition);
            kvp.Value.SetPosition(pos);
        }
    }

    public void ApplyFameSettings()
    {
        FameCSharp.FameSettings.accelerationThreshold = accelerationThreshold;
        FameCSharp.FameSettings.ApplyTerrainEffect = applyTerrainEffect;
        FameCSharp.FameSettings.slowingDistance = slowingDistance;
        FameCSharp.FameSettings.ignoreMinSpeed = ignoreMinSpeed;
        FameCSharp.FameSettings.SwitchAgentPosition = switchAgentPosition;
        FameCSharp.FameSettings.WrapTerrain = warpTerrain;
        FameCSharp.FameSettings.WrapTerrain_MaxX = WarpTerrainSize.xMax;
        FameCSharp.FameSettings.WrapTerrain_MinX = WarpTerrainSize.xMin;
        FameCSharp.FameSettings.WrapTerrain_MaxY = WarpTerrainSize.yMax;
        FameCSharp.FameSettings.WrapTerrain_MinY = WarpTerrainSize.yMin;
    }

    void InitFameTerrain()
    {
        if (!getInfoFromUnityTerrain)
        {
            byte[,] terrainHeightMapByte = new byte[resolution, resolution];
            if (terrainHeightMap != null)
            {
                Color[] colorInfo = terrainHeightMap.GetPixels();
                for (int i = 0; i < colorInfo.Length; i++)
                {
                    int u = i / resolution;
                    int v = i % resolution;
                    terrainHeightMapByte[u, v] = (byte)(colorInfo[i].grayscale * 255);
                }
            }
            Vector3 pos = gameObject.transform.position;
            FAME.Singleton.InitTerrain(terrainHeightMapByte, terrainWidth, terrainHeight, terrainLength, pos.x, pos.y, pos.z);
            InitFAMEArea(new Rect(pos.x, pos.z, terrainWidth, terrainLength));
        }
        else
        {
            if (Terrain.activeTerrain != null)
            {
                Bounds terrainBound = Terrain.activeTerrain.collider.bounds;
                float[,] heightmap = Terrain.activeTerrain.terrainData.GetHeights(0, 0, Terrain.activeTerrain.terrainData.heightmapResolution, Terrain.activeTerrain.terrainData.heightmapResolution);
                FAME.Singleton.InitTerrain(heightmap, terrainBound.size.x, terrainBound.size.y, terrainBound.size.z, terrainBound.min.x, terrainBound.min.y, terrainBound.min.z);
                InitFAMEArea(new Rect(terrainBound.min.x, terrainBound.min.z, terrainBound.size.x, terrainBound.size.z));
            }
            else
            {
                Debug.LogError("Terrain is not found!");
            }
        }
    }
}
