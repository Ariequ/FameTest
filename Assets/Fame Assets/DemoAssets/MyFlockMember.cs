using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class MyFlockMember : FlockMember, SelectableUnit, MoveableUnit
{
    Color selectionColor = new Color(1,1,1,1);

    public Color SelectionColor
    {
        get { return selectionColor; }
        set
        {
            selectionColor = value;
            if (selectionTexture == null)
            {
                CreateSelectionTexture();
            }
            selectionTexture.renderer.material.color = value; 
        }
    }
    bool isSelected = false;
    public bool IsSelected
    {
        get
        {
            return isSelected;
        }
        set
        {
            isSelected = value;
            if (selectionTexture != null)
            {
                selectionTexture.renderer.enabled = value;
            }
        }
    }
    protected override void Start()
    {
        base.Start();
        CreateSelectionTexture();
    }
    protected void Update()
    {
        //Debug.DrawRay(Destination, Vector3.up * 5);

    }

    GameObject selectionTexture = null;
    private void CreateSelectionTexture()
    {
        selectionTexture = GameObject.CreatePrimitive(PrimitiveType.Plane);
        selectionTexture.transform.parent = gameObject.transform;
        selectionTexture.transform.localPosition = Vector3.zero;
        selectionTexture.renderer.material = AssetManager.Singleton.unitSelection;
        selectionTexture.renderer.enabled = isSelected;
        selectionTexture.layer = 12;
        selectionTexture.renderer.material.color = selectionColor;
    }

    public int ID
    {
        get { return AgentID; }
    }

    public void MoveTo(Vector3 point)
    {
        if (FlockID == -1)
        {
            Destination = point;
            //FameManager.MoveAgent(AgentID, point);
        }
    }
}
