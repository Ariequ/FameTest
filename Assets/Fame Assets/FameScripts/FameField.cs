using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using FameCSharp;
using FameCore.Util;
using System;

public enum FameFieldAttribute
{
    radius,
    magnitude,
    forceDirection
}

public class FameField : MonoBehaviour {

    private bool init = false;
    public bool IsInit
    {
        get { return init; }
    }

	[SerializeField]
    private FameFieldType fieldType = FameFieldType.Uniform;
    public FameFieldType FieldType
    {
        get { return fieldType; }
        set
        {
            fieldType = value;
        }
    }
	
	[SerializeField]
    private FameCircularDirection circularFieldDirection = FameCircularDirection.Clockwise;
    public FameCircularDirection CircularFieldDirection
    {
        get { return circularFieldDirection; }
        set { 
            circularFieldDirection = value;
		}
    }
	
	[SerializeField]
	private float fieldWidthX = 30;
    public float FieldWidthX
    {
        get { return fieldWidthX; }
        set
        {
            fieldWidthX = value;
        }
    }
	
	[SerializeField]
    private float fieldWidthZ = 30;
    public float FieldWidthZ
    {
        get { return fieldWidthZ; }
        set
        {
            fieldWidthZ = value;
        }
    }
	
	[SerializeField]
    private float fieldRadius = 40;
    public float FieldRadius
    {
        get { return fieldRadius; }
        set
        {
            fieldRadius = value;
        }
    }
	
	[SerializeField]
    private float fieldAngleRad = 0;
    public float FieldAngleRad
    {
        get { return fieldAngleRad; }
        set
        {
            fieldAngleRad = value;
        }
    }
    const float ToDeg = 57.2957795f;
    const float ToRad = 0.0174532925f;
    public float FieldAngleDeg
    {
        get { return fieldAngleRad * ToDeg; }
        set
        {
            fieldAngleRad = value * ToRad;
        }
    }

    public Vector3 GetForceDirection()
    {
        float x = 1;
        float z = 0;
        double cosine = System.Math.Cos(fieldAngleRad);
        double sine = System.Math.Sin(fieldAngleRad);
        Vector3 result = new Vector3(0, 0, 0);
        result.x = (float)(x * cosine - z * sine);
        result.z = (float)(x * sine + z * cosine);
        return result;
    }

	[SerializeField]
    private float fieldMagnitude = 30;
    public float FieldMagnitude
    {
        get { return fieldMagnitude; }
        set
        {
            fieldMagnitude = value;
        }
    }

	[SerializeField]
    private int fieldID = 0;
    public int FieldID
    {
        get { return fieldID; }
        set { fieldID = value; }
    }
	
	// Use this for initialization
	protected virtual void Start () {
		if (!init)
        {
            InitField();
        }
        FameManager.RegisterField(fieldID, this);
	}

    public void InitField(Vector3 pos, float width, float height, float mag, float fieldAngleRad)
    {
        if (!init)
        {
            this.fieldWidthX = width;
            this.fieldWidthZ = height;
            this.fieldMagnitude = mag;
            this.fieldAngleRad = fieldAngleRad;
            this.fieldType = FameFieldType.Uniform;
            init = true;
            transform.position = pos;

            FVec2 angle = new FVec2(1, 0);
            angle.Rotate(fieldAngleRad);

            fieldID = FAME.Singleton.CreateUniformField(pos.x, pos.z, fieldWidthX, fieldWidthZ, fieldMagnitude, angle.x, angle.z);
            ApplyFieldFlag();

        }
        else
        {
            Debug.LogError("FameField has already been initialized. You are only allowed to initialize it once");
        }
    }
    public void InitField(Vector3 pos, float radius, float mag, FameCircularDirection direction)
    {
        if (!init)
        {
  
            this.fieldRadius = radius;
            this.fieldMagnitude = mag;
            this.circularFieldDirection = direction;
            this.fieldType = FameFieldType.Circular;
            transform.position = pos;
            fieldID = FAME.Singleton.CreateCircularField(pos.x, pos.z, fieldRadius, fieldMagnitude, direction);
            ApplyFieldFlag();
            init = true;
        }
        else
        {
            Debug.LogError("FameField has already been initialized. You are only allowed to initialize it once");
        }
    }
	private void InitField(){
        if (!init)
        {
            Vector3 pos = gameObject.transform.position;
            if (fieldType == FameFieldType.Circular)
            {
                fieldID = FAME.Singleton.CreateCircularField(pos.x, pos.z, fieldRadius, fieldMagnitude, circularFieldDirection);
            }
            else
            {
                FVec2 angle = new FVec2(1, 0);
                angle.Rotate(fieldAngleRad);
                fieldID = FAME.Singleton.CreateUniformField(pos.x, pos.z, fieldWidthX, fieldWidthZ, fieldMagnitude, angle.x, angle.z);
            }
            ApplyFieldFlag();
            init = true;
        }
        else
        {
            Debug.LogError("FameField has already been initialized. You are only allowed to initialize it once");
        }
	}

    /// <summary>
    /// Apply the Field Settings to FameField
    /// </summary>
    public void ApplyFameFieldSetting()
    {
        if (init)
        {
            switch (fieldType)
            {
                case FameFieldType.Circular:
                    FAME.Singleton.SetFieldParam(fieldID, circularFieldDirection, fieldRadius, fieldMagnitude);
                    break;
                case FameFieldType.Uniform:
                    Vector3 force = GetForceDirection();
                    FAME.Singleton.SetFieldParam(fieldID, fieldWidthX, fieldWidthZ, force.x, force.z, fieldMagnitude);
                    break;
            }
        }
    }


    [SerializeField]
    private byte fieldflag = 0;
    /// <summary>
    /// The Field flag for the vector field
    /// </summary>
    public byte FieldFlag
    {
        get
        {
            return fieldflag;
        }
        set
        {
            fieldflag = value;
        }
    }

    /// <summary>
    /// Turn on the flag for certain bits specified. (bitwise OR operator is performed)
    /// </summary>
    /// <param name="flag">bits to turn off</param>
    public void OnFieldFlag(byte flag)
    {
        this.fieldflag |= flag;
    }
    /// <summary>
    /// Turn off the flag for certain bits specified. (bitwise AND operator is performed)
    /// </summary>
    /// <param name="flag">bits to turn off</param>
    public void OffFieldFlag(byte flag)
    {
        this.fieldflag &= (byte)~flag;
    }
    /// <summary>
    /// Apply the field flag settings to FAME
    /// </summary>
    public void ApplyFieldFlag()
    {
        if (init)
        {
            FAME.Singleton.SetFieldFlag(fieldID, fieldflag);
        }
    }

    protected virtual void OnDestroy()
    {
        if (init)
        {
            FameManager.RemoveField(fieldID);
        }
    }
}
