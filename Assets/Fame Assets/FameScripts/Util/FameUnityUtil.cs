using System;
using FameCore.Util;
using UnityEngine;
using System.Collections.Generic;

public static class FameUnityUtil
{
	public static FVec3[] Vec3ToFVec3(List<Vector3> vec3){
		FVec3[] result = new FVec3[vec3.Count];
		for(int i = 0; i < vec3.Count; i++){
		result[i] = new FVec3(vec3[i].x, vec3[i].y, vec3[i].z);	
		}
		return result;
	}

    public static FVec3[] Vec3ToFVec3(Vector3[] vec3)
    {
        FVec3[] result = new FVec3[vec3.Length];
        for (int i = 0; i < vec3.Length; i++)
        {
            result[i] = new FVec3(vec3[i].x, vec3[i].y, vec3[i].z);
        }
        return result;

    }

    public static FVec3 Vec3ToFVec3(Vector3 vec3)
    {
        return new FVec3(vec3.x, vec3.y, vec3.z);
    }

    public static Vector3 CalculateCentroid(FVec3[] arr)
    {
        Vector3 result = Vector3.zero;
        for (int i = 0; i < arr.Length; i++)
        {
            result += FVec3ToVec(arr[i]);
        }
        result /= arr.Length;
        return result;
    }

    public static Vector3 CalculateCentroid(Vector3[] arr)
    {
        Vector3 result = Vector3.zero;
        for (int i = 0; i < arr.Length; i++)
        {
            result += arr[i];
        }
        result /= arr.Length;
        return result;
    }

    public static Vector3[] FVec3ToVec(FVec3[] v)
    {
        Vector3[] result = new Vector3[v.Length];
        for (int i = 0; i < v.Length; i++)
        {
            result[i] = FVec3ToVec(v[i]);
        }
        return result;
    }

    public static Vector3 FVec3ToVec(FVec3 v)
    {
        return new Vector3(v.x, v.y, v.z);
    }

    public static Vector3 Rotate(Vector3 A, float radian)
    {
        double cosine = System.Math.Cos(radian);
        double sine = System.Math.Sin(radian);
        float x = (float)(A.x * cosine - A.z * sine);
        float y = A.y;
        float z = (float)(A.x * sine + A.z * cosine);
        return new Vector3(x, y, z);
    }

    public static void DebugPrint(FVec3[] arr)
    {
        string s = "";
        for (int i = 0; i < arr.Length; i++)
        {
            s += arr[i].ToString() + ",";
        }
        Debug.Log(s);
    }

    public static void DebugPrint2D(string msg, FVec3[] arr)
    {
        string s = msg + ":";
        for (int i = 0; i < arr.Length; i++)
        {
            s += String.Format("({0},{1})", arr[i].x, arr[i].z) + ",";
        }
        Debug.Log(s);
    }

    public static void DebugPrint(Vector3[] arr)
    {
        string s = "";
        for (int i = 0; i < arr.Length; i++)
        {
            s += arr[i].ToString() + ",";
        }
        Debug.Log(s);
    }
}

