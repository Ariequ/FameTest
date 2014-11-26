using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class FormationControl : MonoBehaviour
{
    CtrlPointInterface obj;
    GameObject[] sphereControlPoint = new GameObject[0];
    bool init = false;
    public void Init(CtrlPointInterface obj, Color ctrlPtColor, Color lineColor, Material lineMaterial)
    {
        init = true;
        this.obj = obj;
        this.ctrlPointColor = ctrlPtColor;
        CreateShapeLineRenderer(obj.GetCtrlPointWorld(), ctrlPtColor, lineColor, lineMaterial);
    }
    // Use this for initialization
    void Start()
    {
    }

    public void SetLayer(int ctrlPtLayer, int lineRendererLayer)
    {
        gameObject.layer = lineRendererLayer;
        foreach (GameObject obj in sphereControlPoint)
        {
            obj.layer = ctrlPtLayer;
        }
    }
    Color ctrlPointColor;
    public void SetColor(Color c, Color lineColor)
    {
        foreach (GameObject ctrlPtObj in sphereControlPoint)
        {
            ctrlPtObj.renderer.material.color = c;
        }
        ctrlPointColor = c;
        LineRenderer lineRenderer = gameObject.GetComponent<LineRenderer>();
        lineRenderer.material.color = lineColor;
    }
    float yOffset = 1;
    protected void CreateShapeLineRenderer(Vector3[] polyShape, Color ctrlPtColor, Color lineColor, Material lineMaterial)
    {
        for (int i = 0; i < polyShape.Length; i++)
        {
            polyShape[i].y = yOffset;
        }
        LineRenderer lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.material = lineMaterial;
        lineRenderer.SetVertexCount(polyShape.Length + 1);
        for (int i = 0; i < polyShape.Length + 1; i++)
        {
            lineRenderer.SetPosition(i, polyShape[i % polyShape.Length]);
        }
        lineRenderer.material.color = lineColor;
        UpdateControlPoint(polyShape, lineRenderer, ctrlPtColor);
    }

    public Vector3[] ControlPoints
    {
        get
        {
            return obj.GetCtrlPointWorld();
        }
    }
    private float ctrlPointSize = 3;
    private void UpdateControlPoint(Vector3[] ctrlPoint, LineRenderer lineRendererComp, Color ctrlPtColor)
    {
        for (int i = 0; i < ctrlPoint.Length; i++)
        {
            ctrlPoint[i].y += yOffset;
        }
        if (sphereControlPoint.Length != ctrlPoint.Length)
        {
            lineRendererComp.SetVertexCount(ctrlPoint.Length + 1);
            for (int i = 0; i < sphereControlPoint.Length; i++)
            {
                if (sphereControlPoint[i] != null)
                    Destroy(sphereControlPoint[i]); // destroy everything first
            }
            sphereControlPoint = new GameObject[ctrlPoint.Length];  // initialize the correct size
            for (int i = 0; i < sphereControlPoint.Length; i++)
            {
                GameObject ctrlPtObj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                ctrlPtObj.layer = 9;
                CtrlPtNode ctrlPtNode = ctrlPtObj.AddComponent<CtrlPtNode>();
                ctrlPtNode.Init(obj, i);
                ctrlPtObj.transform.parent = gameObject.transform;
                sphereControlPoint[i] = ctrlPtObj;
                ctrlPtObj.renderer.material.color = ctrlPointColor;
                ctrlPtObj.name = "CtrlPoint";
                ctrlPtNode.SetSize(ctrlPointSize);
                ctrlPtNode.SetColor(ctrlPointColor);
                
            }
        }
        
        for (int i = 0; i <= ctrlPoint.Length; i++)
        {
            Vector3 pt = ctrlPoint[i % ctrlPoint.Length];
            float y = 0;
            if (Terrain.activeTerrain != null)
            {
                y = Terrain.activeTerrain.SampleHeight(pt) + yOffset;
            }
            pt.y = y;
            lineRendererComp.SetPosition(i, pt);
            if (i < ctrlPoint.Length)
            {

                sphereControlPoint[i].transform.position = pt;
            }
        }
    }

    public void SetEnableRenderer(bool enable)
    {
        Renderer[] renderers = GetComponentsInChildren<Renderer>();
        for (int i = 0; i <= renderers.Length; i++)
        {
            renderers[i].enabled = enable;
        }
    }


    public void ShowCtrlPoint(bool value)
    {
        for (int i = 0; i < sphereControlPoint.Length; i++)
        {
            sphereControlPoint[i].renderer.enabled = value;
        }
    }


    void Update()
    {
        if (init)
        {
            Vector3[] ctrlPoint = ControlPoints;
            LineRenderer lineRendererComp = GetComponent<LineRenderer>();
            if (ctrlPoint.Length >= 3)
            {
                UpdateControlPoint(ctrlPoint, lineRendererComp, ctrlPointColor);
            }
        }
    }
}
