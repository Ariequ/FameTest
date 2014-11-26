using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class MyFlockGroup : FlockGroup, SelectableUnit
{
    static Color GroundSelectedColor = new Color(0.3f, 1f, 0.3f, 1.0f);
    static Color GroundUnselectedColor = new Color(0.3f, 0.6f, 0.3f, 1.0f);
    static Color AirSelectedColor = new Color(0.5f, 0.5f, 1.0f, 1.0f);
    static Color AirUnselectedColor = new Color(0.3f, 0.3f, 0.9f, 1.0f);
    bool isSelected;
    public bool IsSelected
    {
        get
        {
            return isSelected;
        }
        set
        {
            isSelected = value;
            if (FlockType == FlockType.Ground)
            {
                ctrlScript.SetColor(value ? GroundSelectedColor : GroundUnselectedColor, flockColor);
            }
            else
            {
                ctrlScript.SetColor(value ? AirSelectedColor : AirUnselectedColor, flockColor);
            }
        }
    }

    Color flockColor;
    public Color FlockColor
    {
        get { return flockColor; }
        set { flockColor = value; }
    }

    protected override void Awake()
    {
        base.Awake();
        // Add your codes here
        // ...

        int randIndex = Random.Range(0, AssetManager.Singleton.colorScheme.Length);
        flockColor = AssetManager.Singleton.colorScheme[randIndex];
        flockColor.a = 0.8f;
    }
     
    protected override void Start()
    {
        base.Start();
        // Add your codes here
        // ...
        Color ctrlPtColor = FlockType == FlockType.Ground? GroundUnselectedColor: AirUnselectedColor;
        SetFormationCtrlColor(ctrlPtColor, flockColor, AssetManager.Singleton.pathMaterial);
        SetFormationControlLayer(9, 12);
    }

}
