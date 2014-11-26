using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class MyFameField : FameField
{
    GameObject arrowObject;
    GameObject myfieldObject;
    protected override void Start()
    {
        base.Start();
        myfieldObject = GameObject.CreatePrimitive(PrimitiveType.Plane);
        myfieldObject.name = "MyField";
        myfieldObject.layer = 10;
        myfieldObject.transform.parent = gameObject.transform;
        myfieldObject.transform.localPosition = new Vector3(0, 0.2f, 0);
        myfieldObject.collider.enabled = false;
        arrowObject = GameObject.CreatePrimitive(PrimitiveType.Plane);
        arrowObject.layer = 10;
        arrowObject.transform.parent = gameObject.transform;
        arrowObject.transform.localPosition = new Vector3(0, 0.3f,0);
        arrowObject.SetActive(false);
        arrowObject.renderer.material = AssetManager.Singleton.arrow;
        arrowObject.collider.enabled = false;

        gameObject.layer = 10;
    }
    //bool init = false;
    //FameField field;
    //public void Init(FameField field)
    //{
    //    init = true;
    //    this.field = field;
    //}

    void Update()
    {
        if (IsInit)
        {
            switch (FieldType)
            {
                case FameFieldType.Uniform:
                    myfieldObject.renderer.material = AssetManager.Singleton.border;
                    arrowObject.SetActive(true);
                    float minXZ = Mathf.Min(FieldWidthX, FieldWidthZ);
                    myfieldObject.transform.localScale = new Vector3(FieldWidthX / 10, 1, FieldWidthZ / 10);
                    arrowObject.transform.localScale = new Vector3(minXZ / 10, 1, minXZ / 10);
                    Quaternion angleRotation = Quaternion.AngleAxis(-(FieldAngleDeg + 90), Vector3.up);
                    if (arrowObject.transform.rotation != angleRotation)
                        arrowObject.transform.rotation = angleRotation;
                    break;
                case FameFieldType.Circular:
                    {
                        arrowObject.SetActive(false);
                        myfieldObject.transform.localScale = new Vector3(FieldRadius / 5, 1, FieldRadius / 5);
                        switch (CircularFieldDirection)
                        {
                            case FameCircularDirection.AntiClockwise:
                                myfieldObject.renderer.material = AssetManager.Singleton.anticlockwise;
                                break;
                            case FameCircularDirection.Clockwise:
                                myfieldObject.renderer.material = AssetManager.Singleton.clockwise;
                                break;
                            case FameCircularDirection.Attraction:
                                myfieldObject.renderer.material = AssetManager.Singleton.attract;
                                break;
                            case FameCircularDirection.Repulsion:
                                myfieldObject.renderer.material = AssetManager.Singleton.repel;
                                break;
                        }
                    }
                    break;
            }

        }
    }
}

