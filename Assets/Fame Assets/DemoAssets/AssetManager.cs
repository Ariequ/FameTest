using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
public class AssetManager : MonoBehaviour
{
    public Material arrow;
    public Material border;
    public Material anticlockwise;
    public Material clockwise;
    public Material attract;
    public Material repel;
    public Material rock;
    public Material unitSelection;
    public Material pathMaterial;
    public Color[] colorScheme;

    private static AssetManager mSingleton;
    public static AssetManager Singleton
    {
        get
        {
            return mSingleton;
        }
    }

    void Awake()
    {
        mSingleton = this;
    }
    void Start()
    {
    }
}
