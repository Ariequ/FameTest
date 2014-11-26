using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public interface CtrlPointInterface
{
    Vector3[] GetCtrlPointWorld();
    void SetCtrlPointPos(int index, Vector3 worldPos);
    string GetObjectName();
    int ID
    {
        get;
    }

}
