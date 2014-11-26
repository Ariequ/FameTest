using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class FlockProperty : MonoBehaviour
{
    private static FlockProperty mSingleton = null;
    public static FlockProperty Singleton
    {
        get
        {
            if (mSingleton == null)
            {
                GameObject go = new GameObject("FlockProperty");
                go.AddComponent<FlockProperty>();
            }
            return mSingleton;
        }
    }

    void Awake()
    {
        mSingleton = this;
    }

    Rect windowRect = new Rect(Screen.width * 0.15f, Screen.height * 0.05f, Screen.width * 0.7f, 400);
    void OnGUI()
    {
        if (!show) return;
        windowRect = new Rect(Screen.width * 0.15f, Screen.height * 0.05f, Screen.width * 0.7f, 400);
        windowRect = GUI.Window((int)WindowID.flockProperty, windowRect, DrawWindow, "Flock Property");
    }

    bool show = false;
    // Make the contents of the window
    public void DrawWindow(int windowID)
    {
        GUILayout.BeginArea(new Rect(20, 30, windowRect.width - 40, windowRect.height  - 60));
        GUILayout.BeginHorizontal();
        CreateLeftPanel();
        GUILayout.Space(windowRect.width * 0.05f);
        CreateRightPanel();
        GUILayout.EndHorizontal();
        //GUI.DragWindow(new Rect(0, 0, 10000, 10000));
        GUILayout.EndArea();

        GUILayout.BeginArea(new Rect(windowRect.width / 4, windowRect.height - 40, windowRect.width / 2, 50));
        GUILayout.BeginHorizontal();

        if (GUILayout.Button("Ok"))
        {
            OKButton();
            Hide();
        }
        if (GUILayout.Button("Cancel"))
        {
            Hide();
        }
        GUILayout.EndHorizontal();
        GUILayout.EndArea();
        GUI.DragWindow();
    }

    int flockIDToShow = -1;
    float mMaxSpeed;
    float mMinSpeed;
    float mRadius;
    float mMaxForce;
    float mMass;
    BehaviorParam[] behaviorList;
    byte fieldFlag;
    byte collisionFlag;
    public void Show(int flockID)
    {
        flockIDToShow = flockID;
        this.show = true;
        FlockGroup f = FameManager.GetFlockGroup(flockID);
        mMaxForce = f.AgentMaxForce;
        mMinSpeed = f.AgentMinSpeed;
        mMaxSpeed = f.AgentMaxSpeed;
        mRadius = f.AgentRadius;
        mMass = f.AgentMass;
        behaviorList = f.behaviorList;
        this.fieldFlag = f.FieldFlag;
        this.collisionFlag = f.CollisionFlag;
        //morphToList = new DropDownListItem("Morph to");
        //int[] flockIDs = FameManager.GetFlockIDs();
        //foreach (int fID in flockIDs)
        //{
        //    FlockGroup group = FameManager.GetFlock(fID);
        //    if (group.FlockType == f.FlockType && f.FlockID != fID)
        //    {
        //        morphToList.Add(new GUIListItem(fID.ToString()));
        //    }
        //}
    }
    //DropDownListItem morphToList;
    
    public void Hide()
    {
        this.show = false;
    }

    void OKButton(){
        int flockID = flockIDToShow;
        FlockGroup flock = FameManager.GetFlockGroup(flockID);
        flock.AgentMaxSpeed = mMaxSpeed;
        flock.AgentMinSpeed = mMinSpeed;
        flock.AgentMass = mMass;
        flock.AgentMaxForce = mMaxForce;
        flock.AgentRadius = mRadius;
        flock.AddSteeringBehavior(behaviorList);
        flock.CollisionFlag = collisionFlag;
        flock.FieldFlag = fieldFlag;
        flock.ApplyCollisionFlag();
        flock.ApplyFieldFlag();

    }

    private void CreateRightPanel()
    {
        GUILayout.BeginVertical(GUILayout.MaxWidth(windowRect.width / 2 - 40));
        GUILayout.Label("Steering Behaviours");
        //Steering Behavior
        for (int i = 0; i < behaviorList.Length; i++)
        {
            behaviorList[i] = BehaviorFieldEx(behaviorList[i]);

        }
        GUILayout.EndVertical();
    }

    public static BehaviorParam BehaviorFieldEx(BehaviorParam param)
    {
        GUILayout.BeginHorizontal();
        param.Active = GUILayout.Toggle(param.Active, "", GUILayout.Width(18));
        GUILayout.Label(System.Enum.GetName(typeof(SteeringBehaviors), param.Behavior));
        GUILayout.EndHorizontal();
        if (!param.Active)
            GUI.enabled = false;
        GUILayout.BeginHorizontal();
        GUILayout.Space(10);
        GUILayout.Label("Weight", GUILayout.Width(60));
        param.Weight = Mathf.Abs(NumericTextfield(param.Weight, GUILayout.Width(55)));
        GUILayout.Space(5);
        if (param.HasRadius())
        {
            GUILayout.Label("Radius", GUILayout.Width(60));
            param.Radius = Mathf.Abs(NumericTextfield(param.Radius, GUILayout.Width(55)));
        }
        GUILayout.EndHorizontal();

        GUI.enabled = true;
        return param;
    }

    private void CreateLeftPanel()
    {
        // All rectangles are now adjusted to the group. (0,0) is the topleft corner of the group.
        // We'll make a box so you can see where the group is on-screen.
        GUILayout.BeginVertical(GUILayout.MaxWidth(windowRect.width / 2 - 40));

        GUILayout.BeginHorizontal();
        GUILayout.Label("FlockID:", GUILayout.Width(70));
        GUILayout.Label(flockIDToShow.ToString());
        GUILayout.EndHorizontal();
        GUILayout.Label("Agent Attributes");
        GUILayout.BeginHorizontal();
        GUILayout.Space(10);
        GUILayout.Label("MaxSpeed:", GUILayout.Width(70));
        mMaxSpeed = NumericTextfield(mMaxSpeed);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Space(10);
        GUILayout.Label("MinSpeed:", GUILayout.Width(70));
        mMinSpeed = NumericTextfield(mMinSpeed);
        GUILayout.EndHorizontal();
        
        GUILayout.BeginHorizontal();
        GUILayout.Space(10);
        GUILayout.Label("Radius:", GUILayout.Width(70));
        mRadius = NumericTextfield(mRadius);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Space(10);
        GUILayout.Label("MaxForce:", GUILayout.Width(70));
        mMaxForce = NumericTextfield(mMaxForce);
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        GUILayout.Label("Collision Flag");
        if (GUILayout.Toggle((collisionFlag & (byte)FameFlag.Bit7) != 0, "", GUILayout.Width(12)))
            OnCollisionFlag(FameFlag.Bit7);
        else
            OffCollisionFlag(FameFlag.Bit7);
        if (GUILayout.Toggle((collisionFlag & (byte)FameFlag.Bit6) != 0, "", GUILayout.Width(12)))
            OnCollisionFlag(FameFlag.Bit6);
        else
            OffCollisionFlag(FameFlag.Bit6);
        if (GUILayout.Toggle((collisionFlag & (byte)FameFlag.Bit5) != 0, "", GUILayout.Width(12)))
            OnCollisionFlag(FameFlag.Bit5);
        else
            OffCollisionFlag(FameFlag.Bit5);
        if (GUILayout.Toggle((collisionFlag & (byte)FameFlag.Bit4) != 0, "", GUILayout.Width(12)))
            OnCollisionFlag(FameFlag.Bit4);
        else
            OffCollisionFlag(FameFlag.Bit4);
        if (GUILayout.Toggle((collisionFlag & (byte)FameFlag.Bit3) != 0, "", GUILayout.Width(12)))
            OnCollisionFlag(FameFlag.Bit3);
        else
            OffCollisionFlag(FameFlag.Bit3);
        if (GUILayout.Toggle((collisionFlag & (byte)FameFlag.Bit2) != 0, "", GUILayout.Width(12)))
            OnCollisionFlag(FameFlag.Bit2);
        else
            OffCollisionFlag(FameFlag.Bit2);
        if (GUILayout.Toggle((collisionFlag & (byte)FameFlag.Bit1) != 0, "", GUILayout.Width(12)))
            OnCollisionFlag(FameFlag.Bit1);
        else
            OffCollisionFlag(FameFlag.Bit1);
        if (GUILayout.Toggle((collisionFlag & (byte)FameFlag.Bit0) != 0, "", GUILayout.Width(12)))
            OnCollisionFlag(FameFlag.Bit0);
        else
            OffCollisionFlag(FameFlag.Bit0);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("Field Flag");
        if (GUILayout.Toggle((fieldFlag & (byte)FameFlag.Bit7) != 0, "", GUILayout.Width(12)))
            OnFieldFlag(FameFlag.Bit7);
        else
            OffFieldFlag(FameFlag.Bit7);
        if (GUILayout.Toggle((fieldFlag & (byte)FameFlag.Bit6) != 0, "", GUILayout.Width(12)))
            OnFieldFlag(FameFlag.Bit6);
        else
            OffFieldFlag(FameFlag.Bit6);
        if (GUILayout.Toggle((fieldFlag & (byte)FameFlag.Bit5) != 0, "", GUILayout.Width(12)))
            OnFieldFlag(FameFlag.Bit5);
        else
            OffFieldFlag(FameFlag.Bit5);
        if (GUILayout.Toggle((fieldFlag & (byte)FameFlag.Bit4) != 0, "", GUILayout.Width(12)))
            OnFieldFlag(FameFlag.Bit4);
        else
            OffFieldFlag(FameFlag.Bit4);
        if (GUILayout.Toggle((fieldFlag & (byte)FameFlag.Bit3) != 0, "", GUILayout.Width(12)))
            OnFieldFlag(FameFlag.Bit3);
        else
            OffFieldFlag(FameFlag.Bit3);
        if (GUILayout.Toggle((fieldFlag & (byte)FameFlag.Bit2) != 0, "", GUILayout.Width(12)))
            OnFieldFlag(FameFlag.Bit2);
        else
            OffFieldFlag(FameFlag.Bit2);
        if (GUILayout.Toggle((fieldFlag & (byte)FameFlag.Bit1) != 0, "", GUILayout.Width(12)))
            OnFieldFlag(FameFlag.Bit1);
        else
            OffFieldFlag(FameFlag.Bit1);
        if (GUILayout.Toggle((fieldFlag & (byte)FameFlag.Bit0) != 0, "", GUILayout.Width(12)))
            OnFieldFlag(FameFlag.Bit0);
        else
            OffFieldFlag(FameFlag.Bit0);
        GUILayout.EndHorizontal();

        GUILayout.EndVertical();
    }

    public void OnFieldFlag(byte flag)
    {
        fieldFlag |= flag;
    }
    public void OffFieldFlag(byte flag)
    {
        fieldFlag &= (byte)~flag;

    }
    public void OnCollisionFlag(byte flag)
    {
        collisionFlag |= flag;
    }
    public void OffCollisionFlag(byte flag)
    {
        collisionFlag &= (byte)~flag;
    }

    static float NumericTextfield(float value, params GUILayoutOption[] options)
    {
        String tempValue = GUILayout.TextField(value.ToString(), options);
        float tempResult;
        if (float.TryParse(tempValue, out tempResult))
        {
            return tempResult;
        }
        else
        {
            return value;
        }
    }

    void InsertDropDown(DropDownListItem dropDown)
    {
        GUILayout.BeginHorizontal();
        GUILayout.Label(dropDown.displayText + ":", GUILayout.Width(70));
        GUILayout.BeginHorizontal();
        string SelectedItemCaption = (dropDown.SelectedListItem == -1) ? "Select an item..." : dropDown.list[dropDown.SelectedListItem].Name();
        string ButtonText = (dropDown.DropdownVisible) ? "<<" : ">>";
        GUILayout.TextField(SelectedItemCaption);
        dropDown.DropdownVisible = GUILayout.Toggle(dropDown.DropdownVisible, ButtonText, "button", GUILayout.Width(32), GUILayout.Height(20));
        GUILayout.EndHorizontal();
        GUILayout.EndHorizontal();
        //Show the dropdown list if required (make sure any controls that should appear behind the list are before this block) 
        GUILayout.BeginHorizontal();
        GUILayout.Space(70);
        if (dropDown.DropdownVisible)
        {
            dropDown.ScrollPos = GUILayout.BeginScrollView(dropDown.ScrollPos, false, true);
            GUILayout.BeginVertical();
            for (int i = 0; i < dropDown.list.Count; i++)
            {
                if (!dropDown.list[i].Selected && GUILayout.Button(dropDown.list[i].Name()))
                {
                    if (dropDown.SelectedListItem != -1) dropDown.list[dropDown.SelectedListItem].Disable();//Turn off the previously selected item 
                    dropDown.SelectedListItem = i;//Set the index for our currrently selected item 
                    dropDown.list[dropDown.SelectedListItem].Enable();//Turn on the item we clicked 
                    dropDown.DropdownVisible = false; //Hide the list 
                }
            }
            GUILayout.EndVertical();
            GUILayout.EndScrollView();
        }
        GUILayout.EndHorizontal();
    }
}
