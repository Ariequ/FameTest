using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class FallingObstacle : MonoBehaviour
{
    void Awake()
    {

    }

    const float yInit = 10;
    float y = yInit;
    float fallTime = 1.0f;
    float currentT = 0;
    float yDest = 0;

    public void SetYDest(float value)
    {
        yDest = value;
    }
    void Start()
    {
        SetPos();
    }

    void SetPos()
    {
        Vector3 pos = gameObject.transform.localPosition;
        pos.y = y;
        gameObject.transform.localPosition = pos;
    }

    void Update()
    {
        if (currentT < fallTime)
        {
            currentT += Time.deltaTime;
            float t = currentT / fallTime;
            if (t > 1) t = 1;
            float halfPi = Mathf.PI / 2;
            y = Mathf.Sin(t * halfPi + halfPi) * yInit + yDest;
            SetPos();
        }
    }
}

