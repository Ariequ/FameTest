using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class CtrlPtNode : MonoBehaviour
{
    int index;
    CtrlPointInterface _interface;
    public void Init(CtrlPointInterface _interface, int index)
    {
        this.index = index;
        this._interface = _interface;
    }

    public CtrlPointInterface GetObject()
    {
        return _interface;
    }
    public int GetObjectID()
    {
        return _interface.ID;
    }

    public string GetObjectName()
    {
        return _interface.GetObjectName();
    }

    public void Move(Vector3 position)
    {
        _interface.SetCtrlPointPos(index, position);
    }

    public void SetSize(float size)
    {
        Vector3 ctrlPointV3 = new Vector3(size, size, size);
        transform.localScale = ctrlPointV3;

        SphereCollider collider = gameObject.collider as SphereCollider;
        collider.radius = 0.5f;
    }

    public void SetColor(Color c)
    {
        renderer.material.color = c;
    }
}