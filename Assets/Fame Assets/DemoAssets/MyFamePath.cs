using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
public class MyFamePath : FamePath, SelectableUnit, CtrlPointInterface
{
    private bool selected = false;
    private float ctrlPointSize = 3;
    public GameObject[] sphereControlPoint = new GameObject[0];
    public List<Vector3> ctrlPoint = new List<Vector3>();
    static Color UnselectedColor = new Color(0.3f, 0.6f, 0.3f, 1.0f);

    public int ID
    {
        get
        {
            return PathID;
        }
    }

    public string GetObjectName()
    {
        return "Path";
    }

    public bool IsSelected
    {
        get
        {
            return selected;
        }
        set
        {
            if (value != selected)
            {
                selected = value;
            }
        }

    }

    public void SetCtrlPointLayer(int layerNum)
    {
        for (int i = 0; i < sphereControlPoint.Length; i++)
        {
            sphereControlPoint[i].gameObject.layer = layerNum;
        }
    }

    private void ChangeNodeColour(Color c)
    {
        foreach (GameObject sphere in sphereControlPoint)
        {
            sphere.renderer.material.color = c;

        }
    }
    protected override void Start() {
        gameObject.layer = 12;
    }

    public override void InitialisePath(int pathID, Vector3[] controlPoints, float[][] splinePoints)
    {
        base.InitialisePath(pathID, controlPoints, splinePoints);
        //Draw ControlPoints
        ctrlPoint = new List<Vector3>(controlPoints);
        if (sphereControlPoint.Length != ctrlPoint.Count)
        {
            for (int i = 0; i < sphereControlPoint.Length; i++)
            {
                if (sphereControlPoint[i] != null)
                    Destroy(sphereControlPoint[i]); // destroy everything first
            }
            sphereControlPoint = new GameObject[ctrlPoint.Count];  // initialise the correct size
            for (int i = 0; i < sphereControlPoint.Length; i++)
            {
                GameObject ctrlPtObj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                CtrlPtNode ctrlPtNode = ctrlPtObj.AddComponent<CtrlPtNode>();
                ctrlPtNode.Init(this, i);
                ctrlPtNode.transform.position = ctrlPoint[i];
                ctrlPtObj.transform.parent = gameObject.transform;
                sphereControlPoint[i] = ctrlPtObj;
                ctrlPtObj.renderer.material.color = UnselectedColor;
                ctrlPtObj.name = "CtrlPoint";
                ctrlPtNode.SetSize(ctrlPointSize);
                ctrlPtNode.SetColor(UnselectedColor);
            }
        }
    }


    public void ShowCtrlPoint(bool value)
    {
        foreach (GameObject ctrlPt in sphereControlPoint)
        {
            ctrlPt.renderer.enabled = value;
        }
    }

    public override void SetCtrlPointPos(int index, Vector3 worldPos)
    {
        base.SetCtrlPointPos(index, worldPos);

        GameObject sphereObject = sphereControlPoint[index];
        Vector3 spherePosition = sphereObject.transform.position;
        spherePosition.x = worldPos.x;
        spherePosition.z = worldPos.z;
        ctrlPoint[index] = spherePosition;
        sphereObject.transform.position = spherePosition;
    }

}
