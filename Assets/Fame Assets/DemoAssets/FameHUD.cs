using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using FameCSharp;

public enum TransformMode{
    Translate = 0,
    Rotate = 1,
    Scale = 2,
}

public enum WindowID
{
    setting = 0,
    flockProperty,
    fieldProperty,
    obstacleProperty,
    transformTool,
}
public class FameFieldParam
{
    FameCircularDirection fieldDirection;

    public FameCircularDirection FieldDirection
    {
        get { return fieldDirection; }
        set { fieldDirection = value; }
    }
    float radius;

    public float Radius
    {
        get { return radius; }
        set { radius = value; }
    }
    float magnitude;

    public float Magnitude
    {
        get { return magnitude; }
        set { magnitude = value; }
    }
    float width;

    public float Width
    {
        get { return width; }
        set { width = value; }
    }
    float height;

    public float Height
    {
        get { return height; }
        set { height = value; }
    }
    FameFieldType fieldType;

    public FameFieldType FieldType
    {
        get { return fieldType; }
        set { fieldType = value; }
    }

    float angleDeg;

    public float AngleDeg
    {
        get { return angleDeg; }
        set { angleDeg = value; }
    }

    byte flag = 0;
    public byte Flag
    {
        get { return flag; }
        set { flag = value; }

    }
    public void OnFlag(byte flag)
    {
        this.flag |= flag;
    }
    public void OffFlag(byte flag)
    {
        this.flag &= (byte)~flag;
    }

    public FameFieldParam(FameCircularDirection dir, float radius, float magnitude, float width, float height, FameFieldType fieldType, float angleDeg)
    {
        this.fieldDirection = dir;
        this.radius = radius;
        this.magnitude = magnitude;
        this.width = width;
        this.height = height;
        this.fieldType = fieldType;
        this.angleDeg = angleDeg;
    }
}

public class FameHUD : MonoBehaviour {
    public GameObject groudUnitPrefab;
    public GameObject airUnitPrefab;

    GUIContent[] groupSelectionContent = new GUIContent[] { 
        new GUIContent("Group"), 
        new GUIContent("Individual") };
    public static TransformMode transformMode = TransformMode.Translate;
    public GUIContent[] transformModeUI = new GUIContent[]{
        new GUIContent("T"),
        new GUIContent("R"),
        new GUIContent("S")
    };
    public GUIContent[] camCtrlUI = new GUIContent[]{
        new GUIContent("1", "Split View"),
        new GUIContent("2", "Top View"),
        new GUIContent("3", "Perspective View")
    };

    public GUIContent settingIcon;
    public GUIContent createFlockIcon;
    public GUIContent flyingFlockIcon = new GUIContent("Flying Flock", "Create flying flock");
    public GUIContent deleteFlockIcon = new GUIContent("Delete", "Delete Selected");
    public GUIContent populateIcon = new GUIContent("P", "Populate Agent");
    public GUIContent leaveGroupIcon = new GUIContent("Leave Group", "Leave Group");
    public GUIContent joinGroupIcon = new GUIContent("Join Group", "Join Group");
    public GUIContent formGroupIcon = new GUIContent("Form Group", "Form Group");
    public GUIContent spreadIcon = new GUIContent("Spread", "Form up uniformly within shape");
    public GUIContent morphIcon = new GUIContent("Morph", "Morph to new formation shape");
    public GUIContent pathFollowIcon = new GUIContent("Path Follow", "Perform path following");
    public GUIContent createPathIcon = new GUIContent("Draw Path", "Create a path");
    public GUIContent deletePathIcon = new GUIContent("Delete Path", "Delete a path");
    public GUIContent roundObstacleIcon = new GUIContent("Round Obstacle", "Place a round obstacle");
    public GUIContent polyObstacleIcon = new GUIContent("Poly Obstacle", "Draw a polygonal obstacle");
    public GUIContent stopObstacleIcon = new GUIContent("Stop Obstacles", "Stop a moving obstacle");
    public GUIContent moveObstacleIcon = new GUIContent("Move Obstacles", "Move an obstacle");

    public GUIContent deleteObstacleIcon = new GUIContent("Delete Obstacles", "Delete an obstacle");
    public GUIContent addVectorFieldIcon = new GUIContent("Add Field", "Add a vector field");
    public GUIContent deleteVectorFieldIcon = new GUIContent("Delete Field", "Delete a vector Field");
    public GUIContent flockPropertyIcon = new GUIContent("Flock Property", "Flock Property");
    public GUIContent obstaclePropertyIcon = new GUIContent("Obstacle Property", "Obstacle Property");
    public GUIContent fieldPropertyIcon = new GUIContent("Field Property", "Field Property");
    public Texture2D cam;
    // Use this for initialization
	void Start () {
	
	}

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.BackQuote))
        {
            showGUI = !showGUI;
        }
    }
    int numAgent = 40;
    string instruction;
    void SetInstruction(string instruction)
    {
        this.instruction = instruction;
    }
    void ClearInstruction()
    {
        instruction = "";
    }

    void DrawCamCtrlGUI()
    {
        GUILayout.BeginArea(new Rect(Screen.width - 140, 3, 140, 30));
        GUILayout.BeginHorizontal();
        if (GUILayout.Button(settingIcon,GUILayout.Width(30), GUILayout.Height(30)))
        {
            FameSettingProperty.Singleton.Show();
        }
        CamMode mode = (CamMode)GUILayout.SelectionGrid((int)CamHUD.CurrentCamMode, camCtrlUI, 3, GUILayout.Width(30 * 3 + 12), GUILayout.Height(30));
        if(mode != CamHUD.CurrentCamMode){
            CamHUD.SwitchCamMode(mode);
            CamHUD.CurrentCamMode = mode;
        }
        GUILayout.EndHorizontal();
        GUILayout.EndArea();
    }

    void DrawTransformTool(int winID)
    {
        GUILayout.BeginVertical();
        transformMode = (TransformMode)GUILayout.SelectionGrid((int)transformMode, transformModeUI, 1, GUILayout.Width(30), GUILayout.Height(100));
        GUILayout.EndVertical();
        GUI.DragWindow();
    }

    Rect transformToolArea = new Rect(20, 100, 30, 100);
    int iconSize = 40;
    bool showGUI = true;
    void OnGUI()
    {
        if (showGUI)
        {
            DrawCamCtrlGUI();
            transformToolArea = MyGUIUtility.KeepRectWithinScreen(GUILayout.Window((int)WindowID.transformTool, transformToolArea, DrawTransformTool, new GUIContent("")));

            float leftMargin = Mathf.Max(0, (Screen.width - 700) / 2);

            Rect area = new Rect(leftMargin + 10, Screen.height - 154, 250, 154);
            GUILayout.BeginArea(area);
            GUILayout.BeginHorizontal();
            GUILayout.Label("Selection Mode");

            UnitSelection.UnitSelectionMode = (SelectionMode)GUILayout.SelectionGrid((int)UnitSelection.UnitSelectionMode, groupSelectionContent, 2);
            GUILayout.EndHorizontal();
            GUILayout.EndArea();

            GUI.Label(new Rect(20, 0, Screen.width, 50), instruction);
            Rect flockCreationRect = new Rect(leftMargin + 0, Screen.height - 130, 280, 130);
            Rect pathRect = new Rect(leftMargin + 281, Screen.height - 130, 60, 130);
            Rect obstacleRect = new Rect(leftMargin + 342, Screen.height - 130, 150, 130);
            Rect vectorFieldRect = new Rect(leftMargin + 493, Screen.height - 130, 108, 130);
            Rect showCtrlRect = new Rect(leftMargin + 602, Screen.height - 24, 100, 24);
            GUI.Box(flockCreationRect, "Flock Creation Tools");
            GUI.Box(pathRect, "Path");
            GUI.Box(obstacleRect, "Obstacle");
            GUI.Box(vectorFieldRect, "Vector Field");
            GUILayout.BeginArea(GetInnerRect(flockCreationRect));
            GUILayout.BeginHorizontal();
            if (GUILayout.Button(createFlockIcon, GUILayout.Width(iconSize), GUILayout.Height(iconSize)))
            {
                SetInstruction("Draw the new flock formation while holding down the left mouse button.");
                MouseCtrl.Singleton.StartPlottingLine(CreateFlockCallback, false);
            }

            string text = GUILayout.TextField(numAgent.ToString(), GUILayout.Width(iconSize), GUILayout.Height(iconSize));
            int tempResult = 0;
            if (int.TryParse(text, out tempResult))
            {
                if (tempResult < 1) tempResult = 1;
                if (tempResult > 999) tempResult = 999;
                numAgent = tempResult;
            }

            if (UnitSelection.UnitSelectionMode == SelectionMode.individual || UnitSelection.SelectionCount() == 0) GUI.enabled = false;
            if (GUILayout.Button(populateIcon, GUILayout.Width(iconSize), GUILayout.Height(iconSize)))
            {
                PopulateAgent_Clicked();
            }
            GUI.enabled = true;

            if (UnitSelection.SelectionCount() == 0 || UnitSelection.UnitSelectionMode == SelectionMode.individual) GUI.enabled = false;
            if (GUILayout.Button(morphIcon, GUILayout.Width(iconSize), GUILayout.Height(iconSize)))
            {
                Morph_Clicked();
            }
            GUI.enabled = true;
            if (UnitSelection.SelectionCount() == 0 || UnitSelection.UnitSelectionMode == SelectionMode.individual) GUI.enabled = false;
            if (GUILayout.Button(pathFollowIcon, GUILayout.Width(iconSize), GUILayout.Height(iconSize)))
            {
                PathFollow_Clicked();
            }
            GUI.enabled = true;

            if (GUILayout.Button(flockPropertyIcon, GUILayout.Width(iconSize), GUILayout.Height(iconSize)))
            {
                SetInstruction("Click on the flock to open the flock property menu.");
                MouseCtrl.Singleton.GetPointClick(OpenFlockProperty);
            }
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            if (GUILayout.Button(flyingFlockIcon, GUILayout.Width(iconSize), GUILayout.Height(iconSize)))
            {
                SetInstruction("Draw the new flock formation while holding down the left mouse button.");
                MouseCtrl.Singleton.StartPlottingLine(CreateAirFlockCallback, false);
            }

            if (GUILayout.Button(deleteFlockIcon, GUILayout.Width(iconSize), GUILayout.Height(iconSize)))
            {
                DeleteFlock_Clicked();
            }
            if (UnitSelection.SelectionCount() == 0 || UnitSelection.UnitSelectionMode != SelectionMode.individual)
                GUI.enabled = false;
            if (GUILayout.Button(formGroupIcon, GUILayout.Width(iconSize), GUILayout.Height(iconSize)))
            {
                FormGroup_Clicked();
            }
            GUI.enabled = true;
            if (UnitSelection.SelectionCount() == 0 || UnitSelection.UnitSelectionMode != SelectionMode.individual)
                GUI.enabled = false;
            if (GUILayout.Button(leaveGroupIcon, GUILayout.Width(iconSize), GUILayout.Height(iconSize)))
            {
                LeaveGroup_Clicked();
            }
            GUI.enabled = true;

            if (UnitSelection.SelectionCount() == 0 || UnitSelection.UnitSelectionMode != SelectionMode.individual)
                GUI.enabled = false;
            if (GUILayout.Button(joinGroupIcon, GUILayout.Width(iconSize), GUILayout.Height(iconSize)))
            {
                JoinGroup_Clicked();
            }
            GUI.enabled = true;

            if (UnitSelection.SelectionCount() == 0 || UnitSelection.UnitSelectionMode == SelectionMode.individual) GUI.enabled = false;
            if (GUILayout.Button(spreadIcon, GUILayout.Width(iconSize), GUILayout.Height(iconSize)))
            {
                Spread_Clicked();
            }
            GUI.enabled = true;

            GUILayout.EndHorizontal();
            GUILayout.EndArea();
            GUILayout.BeginArea(GetInnerRect(pathRect));
            if (GUILayout.Button(createPathIcon, GUILayout.Width(iconSize), GUILayout.Height(iconSize)))
            {
                CreatePath_Clicked();
            }
            if (GUILayout.Button(deletePathIcon, GUILayout.Width(iconSize), GUILayout.Height(iconSize)))
            {
                DeletePath_Clicked();
            }
            GUILayout.EndArea();
            GUILayout.BeginArea(GetInnerRect(obstacleRect));
            GUILayout.BeginHorizontal();
            if (GUILayout.Button(roundObstacleIcon, GUILayout.Width(iconSize), GUILayout.Height(iconSize)))
            {
                SetInstruction("Left click anywhere on the map to place an obstacle. \nPress the '[' or ']' button to decrease or increase the size of the obstacle.");
                MouseCtrl.Singleton.GetPlaceObstaclePointClick(PlaceRoundObstacle);
            }
            if (GUILayout.Button(polyObstacleIcon, GUILayout.Width(iconSize), GUILayout.Height(iconSize)))
            {
                SetInstruction("Create a polygonal shape obstacle by left clicking on the map. Right Click to end.");
                MouseCtrl.Singleton.GetPointSeries(PlacePolyObstacle);
            }
            if (GUILayout.Button(obstaclePropertyIcon, GUILayout.Width(iconSize), GUILayout.Height(iconSize)))
            {
                SetInstruction("Click on the obstacle to bring up the obstacle property menu.");
                MouseCtrl.Singleton.GetPointClick(OpenObstacleProperty);
            }
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            if (GUILayout.Button(deleteObstacleIcon, GUILayout.Width(iconSize), GUILayout.Height(iconSize)))
            {
                SetInstruction("Click on the obstacles that you want to remove.");
                MouseCtrl.Singleton.GetPointClick(RemoveObstacle);
            }
            if (GUILayout.Button(moveObstacleIcon, GUILayout.Width(iconSize), GUILayout.Height(iconSize)))
            {
                SetInstruction("Click on the obstacle and hold your left mouse button to plot \nthe waypoints for the obstacles.");
                MouseCtrl.Singleton.GetPointClick(GetObstacleToMove);
            }
            if (GUILayout.Button(stopObstacleIcon, GUILayout.Width(iconSize), GUILayout.Height(iconSize)))
            {
                SetInstruction("Click on the obstacles to make it stop moving.");
                MouseCtrl.Singleton.GetPointClick(StopMovingObstacle);
            }
            GUILayout.EndHorizontal();
            GUILayout.EndArea();

            GUILayout.BeginArea(GetInnerRect(vectorFieldRect));
            GUILayout.BeginHorizontal();
            if (GUILayout.Button(addVectorFieldIcon, GUILayout.Width(iconSize), GUILayout.Height(iconSize)))
            {
                SetInstruction("Left click anywhere on the map to place an vector field.");
                MouseCtrl.Singleton.GetPlaceFieldPointClick(PlaceField);
            }

            if (GUILayout.Button(deleteVectorFieldIcon, GUILayout.Width(iconSize), GUILayout.Height(iconSize)))
            {
                SetInstruction("Click on the vector field that you want to remove.");
                MouseCtrl.Singleton.GetPointClick(RemoveField);
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            if (GUILayout.Button(fieldPropertyIcon, GUILayout.Width(iconSize), GUILayout.Height(iconSize)))
            {
                SetInstruction("Click on the obstacle to bring up the field property menu.");
                MouseCtrl.Singleton.GetPointClick(OpenFieldProperty);
            }
            GUILayout.EndHorizontal();

            GUILayout.EndArea();

            if (UnitSelection.UnitSelectionMode == SelectionMode.group)
            {
                if (UnitSelection.SelectionCount() == 1)
                {
                    FlockGroup group = UnitSelection.GetCurrentlySelectedUnit()[0] as FlockGroup;
                    float altitude = group.GetAltitude();
                    altitude = GUI.VerticalSlider(new Rect(leftMargin + 605, Screen.height - 124, 30, 100), altitude, 200, 0);
                    GUI.Label(new Rect(leftMargin + 627, Screen.height - 134 + (1 - altitude / 100) * 90 - 10, 50, 100), "Altitude:" + altitude.ToString("N1"));
                    group.SetAltitude(altitude);
                }
            }
            showCtrlPoint = GUI.Toggle(showCtrlRect, showCtrlPoint, new GUIContent("Show Ctrl Pt", "Show Control Point"));
            GUIStyle style = new GUIStyle();
            Vector2 tooltipSize = style.CalcSize(new GUIContent(GUI.tooltip));
            Rect tooltipRect = new Rect(Input.mousePosition.x + 5, Screen.height - Input.mousePosition.y - 20, tooltipSize.x, 30);
            GUI.Label(MyGUIUtility.KeepRectWithinScreen(tooltipRect), GUI.tooltip);

            //Draw perspective cam position;
            if (TopCamCtrl.Singleton.camera.enabled)
            {
                Vector3 pCamPos = PerspectiveCamCtrl.Singleton.gameObject.transform.position;
                Vector3 screenPos = TopCamCtrl.Singleton.camera.WorldToScreenPoint(pCamPos);
                screenPos.y = Screen.height - screenPos.y;
                Rect camBound = TopCamCtrl.Singleton.camera.rect;
                camBound.width *= Screen.width;
                camBound.height *= Screen.height;
                if (camBound.Contains(new Vector2(screenPos.x, screenPos.y)))
                {
                    Matrix4x4 matrixBackup = GUI.matrix;
                    GUIUtility.RotateAroundPivot(PerspectiveCamCtrl.Singleton.camYRot - 90, screenPos);
                    GUI.DrawTexture(new Rect(screenPos.x - 16, screenPos.y - 8, 32, 16), cam);
                    GUI.matrix = matrixBackup;

                }
            }
        }

    }
    public static bool _showCtrlPoint = true;
    public static bool showCtrlPoint
    {
        get
        {
            return _showCtrlPoint;
        }
        set
        {
            _showCtrlPoint = value;
            int[] groupList = FameManager.GetFlockIDs();
            for (int i = 0; i < groupList.Length; i++)
            {
                FlockGroup f = FameManager.GetFlockGroup(groupList[i]);
                f.ShowCtrlPoint(value);
            }
            int[] pathID = FameManager.GetPathIDs();
            for (int i = 0; i < pathID.Length; i++)
            {
                FamePath p = FameManager.GetPath(pathID[i]);
                if (p is MyFamePath)
                {
                    MyFamePath myFamePath = p as MyFamePath;
                    myFamePath.ShowCtrlPoint(value);
      
                }
            }
        }
    }

    int taskObstacleID;
    public void GetObstacleToMove(Vector3 point)
    {
        taskObstacleID = FameManager.PointInObstacle(point.x, point.z);
        if (taskObstacleID != -1)
        {
            SetInstruction("While holding your left mouse button, move your mouse cursor around to plot \nthe waypoints for the obstacles.");
            //FameObstacle obstacle = FameManager.GetObstacle(taskObstacleID);
            MouseCtrl.Singleton.StartPlottingLine(SetObstacleWaypoint, true, point);
        }
        else
        {
            ClearInstruction();
        }
    }
    public void StopMovingObstacle(Vector3 point)
    {
        int obstacleID = FameManager.PointInObstacle(point.x, point.z);
        if (obstacleID != -1)
        {
            FameObstacle obstacle = FameManager.GetObstacle(obstacleID);
            obstacle.ClearWaypoint();
        }
        ClearInstruction();
    }

    public void PlaceField(Vector3 point, FameFieldParam param)
    {
        switch (param.FieldType)
        {
            case FameFieldType.Circular:
                FameManager.CreateField(point, param.Radius, param.Magnitude, param.FieldDirection, typeof(MyFameField));
                break;
            case FameFieldType.Uniform:
                FameManager.CreateField(point, param.Width, param.Height, param.Magnitude, param.AngleDeg * 0.0174532925f, typeof(MyFameField));
                break;
        }
        ClearInstruction();
    }

    public void RemoveField(Vector3 point)
    {
        int fieldID = FameManager.PointInField(point.x, point.z);
        if (fieldID != -1)
        {
            FameField field = FameManager.GetField(fieldID);
            Destroy(field.gameObject);
        }
        ClearInstruction();
    }

    public void RemoveObstacle(Vector3 point)
    {
        int obstacleID = FameManager.PointInObstacle(point.x, point.z);
        if (obstacleID != -1)
        {
            FameObstacle obstacle = FameManager.GetObstacle(obstacleID);
            Destroy(obstacle.gameObject);
        }
        ClearInstruction();
    }

    public void SetObstacleWaypoint(Vector3[] waypoints)
    {
        if (waypoints.Length >= 2)
        {
            FameObstacle obstacle = FameManager.GetObstacle(taskObstacleID);
            obstacle.SetWaypoint(waypoints);
        }
        ClearInstruction();
    }

    public void OpenFlockProperty(Vector3 point)
    {
        int clickOnFlock = FameManager.PointInFlock(point.x, point.z);
        if (clickOnFlock != -1)
        {
            FlockProperty.Singleton.Show(clickOnFlock);
        }
        ClearInstruction();
    }
    public void OpenObstacleProperty(Vector3 point)
    {
        int clickOnObstacle = FameManager.PointInObstacle(point.x, point.z);
        if (clickOnObstacle != -1)
        {
            ObstacleProperty.Singleton.Show(clickOnObstacle);
        }
        ClearInstruction();
    }
    public void OpenFieldProperty(Vector3 point)
    {
        int clickOnField = FameManager.PointInField(point.x, point.z);
        if (clickOnField != -1)
        {
            FieldProperty.Singleton.Show(clickOnField);
        }
        ClearInstruction();
    }

    public void CreateFlockCallback(Vector3[] points)
    {
        if (points.Length >= 3)
        {
            FameManager.CreateFlock(points, FlockType.Ground, typeof(MyFlockGroup));
        }
        ClearInstruction();
        UnitSelection.UnitSelectionMode = SelectionMode.group;

    }

    public void CreateAirFlockCallback(Vector3[] points)
    {
        if (points.Length >= 3)
        {
            FameManager.CreateFlock(points, FlockType.Flying, typeof(MyFlockGroup), 5f);
        }
        ClearInstruction();
        UnitSelection.UnitSelectionMode = SelectionMode.group;

    }
    
    
    int morphFromID;

    public void PathFollowCallback(int pathID)
    {
        if (pathID != -1)
        {
            SelectableUnit[] selectedUnit = UnitSelection.GetCurrentlySelectedUnit();
            if (selectedUnit.Length == 1)
            {
                FlockGroup group = FameManager.GetFlockGroup(selectedUnit[0].ID);
                group.PathFollow(pathID, 10f);
            }
        }
        ClearInstruction();
    }
    public void DeletePathCallback(int pathID)
    {
        if (pathID != -1)
        {
            FamePath path = FameManager.GetPath(pathID);
            Destroy(path.gameObject);
        }
        ClearInstruction();
    }

    public void MorphTo(Vector3[] point)
    {
        if (point.Length >= 3)
        {
            FlockGroup group = FameManager.GetFlockGroup(morphFromID);
            group.Morph(point);
        }
        ClearInstruction();
    }

    public void PlacePolyObstacle(Vector3[] points)
    {
        if (points.Length >= 3)
        {
            FameManager.CreateObstacle(points, typeof(MyFameObstacle));
        }
        ClearInstruction();
    }


    public void PlaceRoundObstacle(Vector3 position, float radius)
    {
        FameManager.CreateObstacle(position, radius, typeof(MyFameObstacle));
        ClearInstruction();
    }

    public void CreatePathCallback(Vector3[] points)
    {
        ClearInstruction();
        FamePath famePath = FameManager.CreatePath(points, typeof(MyFamePath));
        GameObject pathObject = famePath.gameObject;
        LineRenderer renderer = pathObject.GetComponent<LineRenderer>();
        renderer.material = AssetManager.Singleton.pathMaterial;
        MyFamePath path = famePath as MyFamePath;
        path.SetCtrlPointLayer(9);
    }
    void CreatePath_Clicked()
    {
        SetInstruction("Draw the new flock formation while holding down the left mouse button.");
        MouseCtrl.Singleton.StartPlottingLine(CreatePathCallback, false);
    }
    void DeletePath_Clicked()
    {
        SetInstruction("Click on the path to delete.");
        MouseCtrl.Singleton.GetClickOnPath(DeletePathCallback);
    }
    void DeleteFlock_Clicked()
    {
        SelectableUnit[] unit = UnitSelection.GetCurrentlySelectedUnit();
        UnitSelection.DeselectUnit();
        for (int i = 0; i < unit.Length; i++)
        {
            if (unit[i] is FlockGroup)
            {
                FlockGroup flockGroup = unit[i] as FlockGroup;
                flockGroup.DeleteFlock();
            }
            else if (unit[i] is FlockMember)
            {
                FlockMember flockMember = unit[i] as FlockMember;
                Destroy(flockMember.gameObject);
            }
        }
    }
    void PopulateAgent_Clicked()
    {
        SelectableUnit[] unit = UnitSelection.GetCurrentlySelectedUnit();
        if (unit.Length == 1)
        {
            if (unit[0] is FlockGroup)
            {
                FlockGroup flockGroup = unit[0] as FlockGroup;
                int[] agents = null;
                switch (flockGroup.FlockType)
                {
                    case FlockType.Ground:
                        agents = flockGroup.PopulateFlock(numAgent, groudUnitPrefab, typeof(MyFlockMember));
                        flockGroup.AgentRadius = 4f;
                        break;
                    case FlockType.Flying:
                        agents = flockGroup.PopulateFlock(numAgent, airUnitPrefab, typeof(MyFlockMember));
                        flockGroup.AgentRadius = 4f;
                        flockGroup.AgentMinSpeed = 10f;
                        flockGroup.AgentMaxSpeed = 30f;
                        break;
                }
                if (flockGroup is MyFlockGroup)
                {
                    MyFlockGroup myflockGroup = (MyFlockGroup)flockGroup;
                    for (int i = 0; i < agents.Length; i++)
                    {
                        MyFlockMember member = (MyFlockMember)FameManager.GetFlockMember(agents[i]);
                        member.SelectionColor = myflockGroup.FlockColor;
                    }

                }
            }
        }
    }

    void FormGroup_Clicked()
    {
        SelectableUnit[] agent = UnitSelection.GetCurrentlySelectedUnit();
        List<int> gid = new List<int>();
        List<int> fid = new List<int>();
        for (int i = 0; i < agent.Length; i++)
        {
            if (agent[i] is FlockMember)
            {
                FlockMember flockGroup = agent[i] as FlockMember;
                if (flockGroup.FlockType == FlockType.Flying)
                {
                    fid.Add(agent[i].ID);
                }
                else
                {
                    gid.Add(agent[i].ID);
                }
            }
        }

        if (gid.Count != 0)
        {
            MyFlockGroup group = (MyFlockGroup)FameManager.CreateFlock(typeof(MyFlockGroup), FlockType.Ground, gid.ToArray());
            for (int i = 0; i < gid.Count; i++)
            {
                FlockMember member = FameManager.GetFlockMember(gid[i]);
                if (member is MyFlockMember)
                {
                    MyFlockMember myFlockMember = (MyFlockMember)member;
                    myFlockMember.SelectionColor = group.FlockColor;
                }
            }
        }
        if (fid.Count != 0)
        {
            MyFlockGroup group = (MyFlockGroup)FameManager.CreateFlock(typeof(MyFlockGroup), FlockType.Flying, fid.ToArray());
            for (int i = 0; i < fid.Count; i++)
            {
                FlockMember member = FameManager.GetFlockMember(fid[i]);
                if (member is MyFlockMember)
                {
                    MyFlockMember myFlockMember = (MyFlockMember)member;
                    myFlockMember.SelectionColor = group.FlockColor;
                }
            }
        }
        UnitSelection.UnitSelectionMode = SelectionMode.group;
    }

    int[] taskJoinGroupID;
    void JoinGroup_Clicked()
    {
        SelectableUnit[] agent = UnitSelection.GetCurrentlySelectedUnit();
        taskJoinGroupID = new int[agent.Length];
        for (int i = 0; i < agent.Length; i++)
        {
            taskJoinGroupID[i] = agent[i].ID;
        }
        SetInstruction("Click on the group that the agents should join.");
        MouseCtrl.Singleton.GetPointClick(JoinGroup_Callback);
    }

    void JoinGroup_Callback(Vector3 point)
    {
        int clickOnFlock = FameManager.PointInFlock(point.x, point.z);
        if (clickOnFlock != -1)
        {
            FlockMember[] arr = new FlockMember[taskJoinGroupID.Length];
            for(int i = 0; i < taskJoinGroupID.Length; i++)
            {
                int agentID = taskJoinGroupID[i];
                FlockMember member = FameManager.GetFlockMember(agentID);
                arr[i] = member;
            }
            FlockMember.JoinGroup(clickOnFlock, arr);                
        }
        ClearInstruction();
    }
    
    void LeaveGroup_Clicked()
    {
        SelectableUnit[] agent = UnitSelection.GetCurrentlySelectedUnit();
        int[] id = new int[agent.Length];
        for (int i = 0; i < agent.Length; i++)
        {
            id[i] = agent[i].ID;
        }
        for (int i = 0; i < id.Length; i++)
        {
            FlockMember member = FameManager.GetFlockMember(id[i]);
            member.LeaveGroup();
            if (member is MyFlockMember)
            {
                MyFlockMember myFlockMember = (MyFlockMember)member;
                myFlockMember.SelectionColor = new Color(0.9f, 0.9f, 0.9f, 0.9f);
            }
        }
    }

    void Morph_Clicked()
    {
        if (UnitSelection.SelectionCount() == 1)
        {
            morphFromID = UnitSelection.GetCurrentlySelectedUnit()[0].ID;
            SetInstruction("Draw the new shape to morph to");
            MouseCtrl.Singleton.StartPlottingLine(MorphTo, false);
        }
        else
        {
            UnityEngine.Debug.LogError("Something wrong");
        }
    }
    void PathFollow_Clicked()
    {
        SetInstruction("Click on the path to perform path-following.");
        MouseCtrl.Singleton.GetClickOnPath(PathFollowCallback);
    }

    void Spread_Clicked()
    {
        SelectableUnit[] unit = UnitSelection.GetCurrentlySelectedUnit();
        if (unit.Length == 1)
        {
            if (unit[0] is FlockGroup)
            {
                FlockGroup group = (FlockGroup)unit[0];
                group.SpreadOut();
            }
        }
    }
    static Rect GetInnerRect(Rect r)
    {
        Rect result = new Rect(r.xMin + 10, r.yMin + 23, r.width - 20, r.height - 30);
        return result;
    }
}
