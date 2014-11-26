using UnityEngine;
using System.Collections;
using FameCSharp;

public enum UDirection
{
    up,
    down,
    left,
    right,
    clockwise,
    anticlockwise,
}


public static class FameSetting
{
    //Polygon Design
    public static bool Polygon3D = false;
    public static string agentCount = "40";
    public static string inputResolution = "3";
    public static string iteration = "3";
    public static string scaleStep = "0.05";
    public static string minSplitLength = "30";
    public static float altitude = 50;

    public static Rect cameraBound = new Rect(0, 0, 2000, 2000);


    public static FlockType selectedFlockType = FlockType.Ground;

}