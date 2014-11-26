using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using FameCSharp;

public class FamePath : MonoBehaviour
{
    int pathID = -1;
    bool init = false;
    public bool IsInit { get { return init; } }

    public int PathID
    {
        get { return pathID; }
    }

    // Use this for initialization
    protected virtual void Start()
    {
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        UpdateLineRenderer();
    }


    /// <summary>
    /// Initialise the path & draw control points
    /// </summary>
    /// <param name="pathID"></param>
    /// <param name="controlPoints"></param>
    /// <param name="splinePoints"></param>
    public virtual void InitialisePath(int pathID, Vector3[] controlPoints, float[][] splinePoints)
    {
        this.pathID = pathID;
        // Draw lineRenderer
        float[][] splinepoints = FAME.Singleton.GetPathSpline(pathID);
        LineRenderer lineRendererComp = (LineRenderer)gameObject.AddComponent("LineRenderer");
        lineRendererComp.SetVertexCount(splinepoints.Length);
        lineRendererComp.SetWidth(1, 1);
        for (int i = 0; i < splinepoints.Length; i++)
        {
            float heightY = (float)Terrain.activeTerrain.SampleHeight(new Vector3(splinepoints[i][0], 20f, splinepoints[i][1]));
            Vector3 pt = new Vector3(splinepoints[i][0], heightY + 0.5f, splinepoints[i][1]);
            lineRendererComp.SetPosition(i, pt);
        }
        init = true;
    }


    /// <summary>
    /// update visually the line renderer
    /// </summary>
    public void UpdateLineRenderer()
    {
        float[][] splinepoints = FAME.Singleton.GetPathSpline(pathID);
        LineRenderer lineRendererComp = gameObject.GetComponent<LineRenderer>();
        for (int i = 0; i < splinepoints.Length; i++)
        {
            float heightY = (float)Terrain.activeTerrain.SampleHeight(new Vector3(splinepoints[i][0], 20f, splinepoints[i][1]));
            Vector3 pt = new Vector3(splinepoints[i][0], heightY + 0.5f, splinepoints[i][1]);
            lineRendererComp.SetPosition(i, pt);
        }
    }

    /// <summary>
    /// Get the control points definining the path
    /// </summary>
    /// <returns>positions of the control points</returns>
    public Vector3[] GetCtrlPointWorld()
    {
        return FameManager.GetPathCtrlPoint(pathID);
    }

    /// <summary>
    /// Set the position of the specified control point index
    /// </summary>
    /// <param name="index">index of the control point</param>
    /// <param name="worldPos">new position of the control point in the world coordinate</param>
    public virtual void SetCtrlPointPos(int index, Vector3 worldPos)
    {
        FameManager.SetPathCtrlPoint(pathID, index, worldPos);
    }



    protected virtual void OnDestroy()
    {
        if (init)
        {
            FameManager.RemovePath(pathID);
        }
    }
}
