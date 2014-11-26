using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using System.Collections;

public interface IActiveUIRegion
{
    Rect[] ActiveUIRect
    {
        get;
    }
}

public class MouseCtrl : MonoBehaviour
{
    public FameFieldParam taskFieldParam = new FameFieldParam(FameCircularDirection.AntiClockwise, 10, 2, 10, 10, FameFieldType.Circular, 0);

    MouseActionTask currentTask = MouseActionTask.None;
    CtrlPtNode task_ctrlPt;
    int task_flockID = -1;
    float obstacleRadius = 20f;
    enum MouseActionTask
    {
        None,
        Selection,
        CtrlPoint,
        Plotting,
        MovingFlock,
        PlaceObstacle,
        PlaceField,
        GetPointSeries,
        ListenPointClick,
        GetPath,
    }

    void SetMouseAction(MouseActionTask at)
    {
        currentTask = at;
    }

    void ClearMouseAction()
    {
        currentTask = MouseActionTask.None;
        task_flockID = -1;
    }

    List<IActiveUIRegion> activeUIRects = new List<IActiveUIRegion>();
    public void RegisterActiveUIRegion(IActiveUIRegion newRect)
    {
        activeUIRects.Add(newRect);
    }

    bool ClickedOnActiveUI(Vector3 point)
    {
        return !activeUIRects.Any(ui => MyGUIUtility.RectContainsMousePoint(ui.ActiveUIRect));
    }

    private bool mouseLeftDrag = false;
    private Vector2 mouseButton1DownPoint;

    private Vector2 mouseButton1UpPoint;
    private Vector3? mouseButton1DownTerrainHitPoint;

    private float raycastLength = 1000.0f;

    public Texture selectionTexture;

    public int mouseButtonReleaseBlurRange = 20;

    static MouseCtrl singleton = null;
    public static MouseCtrl Singleton
    {
        get { return singleton; }
        set { singleton = value; }
    }

    void Awake()
    {
        if (singleton == null)
        {
            singleton = this;
        }
        else
        {
            Destroy(this);
        }
    }

    float mouseDelta_x = 0;
    float mouseDelta_y = 0;


    public void OnGUI()
    {
        if (currentTask == MouseActionTask.PlaceField)
        {
            FieldProperty.DrawPlaceFieldHUD(taskFieldParam);
        }
        mouseDelta_x = Input.GetAxis("Mouse X");
        mouseDelta_y = Input.GetAxis("Mouse Y");
        Event e = Event.current;
        if (e.isMouse)
        {
            switch (e.button)
            {
                case 0:
                    if (e.type == EventType.MouseDown)
                    {
                        if (e.clickCount == 1)
                        {
                            Mouse1Down(Input.mousePosition);
                        }
                        else { Mouse1DownDouble(Input.mousePosition); }
                    }
                    if (e.type == EventType.MouseDrag) { Mouse1DownDrag(Input.mousePosition); }
                    if (e.type == EventType.MouseUp) { Mouse1Up(Input.mousePosition); }
                    break;
                case 1:
                    if (e.type == EventType.MouseDown)
                    {
                        if (e.clickCount == 1)
                        {
                            Mouse2Down();
                        }
                    }
                    break;
            }
        }

        if (mouseLeftDrag)
        {
            if (currentTask == MouseActionTask.Selection)
            {
                int width = (int)(mouseButton1UpPoint.x - mouseButton1DownPoint.x);
                int height = (int)((Screen.height - mouseButton1UpPoint.y) - (Screen.height - mouseButton1DownPoint.y));
                Rect rect = new Rect(mouseButton1DownPoint.x, Screen.height - mouseButton1DownPoint.y, width, height);
                GUI.DrawTexture(rect, selectionTexture, ScaleMode.StretchToFill, true);
            }
        }
       
        //GUI.Label(new Rect(Input.mousePosition.x, Screen.height - Input.mousePosition.y, 200, 30), currentTask.ToString());
    }

    LineRenderer lineRenderer;
    public Material lineRendererMat;
    void Start()
    {
        lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.enabled = false;
        lineRenderer.material = lineRendererMat;
        gameObject.layer = 12;
    }

    public void GetPointSeries(PointSeriesDelegateCallback callback)
    {
        SetMouseAction(MouseActionTask.GetPointSeries);
        lineRenderer.enabled = true;
        lineRenderer.SetVertexCount(0);
        pointPlot.Clear();
        pointSeriesDelegateCallback = callback;
    }
    public void GetPointSeries(PointSeriesDelegateCallback callback, Vector3 initialPoint)
    {
        SetMouseAction(MouseActionTask.GetPointSeries);
        lineRenderer.enabled = true;
        lineRenderer.SetVertexCount(0);
        pointPlot.Clear();
        Plot(initialPoint, 999);
        pointSeriesDelegateCallback = callback;
    }

    private void GetPolyPointsDone(Vector3[] point)
    {
        ClearMouseAction();
        lineRenderer.enabled = false;
        pointSeriesDelegateCallback(point);
    }
    public void GetPlaceFieldPointClick(PlaceVectorFieldCallbackFunc callback)
    {
        SetMouseAction(MouseActionTask.PlaceField);
        lineRenderer.enabled = true;
        lineRenderer.SetVertexCount(0);
        placeVectorCallbackFunc = callback;
    }

    public void GetPlaceFieldPointClickDone(Vector3 point, FameFieldParam param)
    {
        ClearMouseAction();
        lineRenderer.enabled = false;
        placeVectorCallbackFunc(point, param);
    }

    public void GetPlaceObstaclePointClick(PlaceObstacleCallbackFunc callback)
    {
        SetMouseAction(MouseActionTask.PlaceObstacle);
        lineRenderer.enabled = true;
        lineRenderer.SetVertexCount(0);
        placeObstacleCallbackFunc = callback;
    }

    public void GetPointClick(PointClickCallbackFunc callback)
    {
        SetMouseAction(MouseActionTask.ListenPointClick);
        lineRenderer.enabled = false;
        lineRenderer.SetVertexCount(0);
        pointClickCallbackFunc = callback;
    }

    private void GetPlaceObstaclePointClickDone(Vector3 point)
    {
        ClearMouseAction();
        lineRenderer.enabled = false;
        placeObstacleCallbackFunc(point, obstacleRadius);
    }

    private void GetPointClickDone(Vector3 point)
    {
        ClearMouseAction();
        lineRenderer.enabled = false;
        pointClickCallbackFunc(point);
    }

    public void StartPlottingLine(PlotDelegateCallback callback, bool startPlotImmediately, Vector3? initPoint = null)
    {
        SetMouseAction(MouseActionTask.Plotting);
        pointPlot.Clear();
        lineRenderer.enabled = true;
        if (initPoint.HasValue)
        {
            lineRenderer.SetVertexCount(1);
            lineRenderer.SetPosition(0, initPoint.Value);
            pointPlot.Add(initPoint.Value);
            Debug.Log(initPoint);
        }
        else
        {
            lineRenderer.SetVertexCount(0);
        }
        startedPlotting = startPlotImmediately;
        plotPointCallbackFunc = callback;
    }

    bool startedPlotting = false;
    private void Plot(Vector3 point, float minDistSq)
    {
        point.y += 2; // add some offset so that the line will be visible
        if (pointPlot.Count > 0)
        {
            Vector3 lastPt = pointPlot[pointPlot.Count - 1];
            Vector3 diff = lastPt - point;
            if (diff.sqrMagnitude > minDistSq)
            {
                
                pointPlot.Add(point);
            }
        }
        else
        {
            pointPlot.Add(point);
        }

        //update line renderer
        lineRenderer.SetVertexCount(pointPlot.Count);
        lineRenderer.SetPosition(pointPlot.Count - 1, point);
    }

    private void PlotDone()
    {
        startedPlotting = false;
        lineRenderer.enabled = false;
        plotPointCallbackFunc(pointPlot.ToArray());
    }

    public void GetClickOnPath(ClickOnPathCallback callback)
    {
        SetMouseAction(MouseActionTask.GetPath);
        clickOnPathCallbackFunc = callback;
    }
    public void GetClickOnPathDone(int pathID)
    {
        ClearMouseAction();
        clickOnPathCallbackFunc(pathID);

    }

    public delegate void ClickOnPathCallback(int pathID);
    ClickOnPathCallback clickOnPathCallbackFunc;

    public delegate void PlotDelegateCallback(Vector3[] points);
    PlotDelegateCallback plotPointCallbackFunc;

    public delegate void PlaceObstacleCallbackFunc(Vector3 points, float radius);
    PlaceObstacleCallbackFunc placeObstacleCallbackFunc;

    public delegate void PlaceVectorFieldCallbackFunc(Vector3 points, FameFieldParam param);
    PlaceVectorFieldCallbackFunc placeVectorCallbackFunc;

    public delegate void PointClickCallbackFunc(Vector3 points);
    PointClickCallbackFunc pointClickCallbackFunc;

    public delegate void PointSeriesDelegateCallback(Vector3[] points);
    PointSeriesDelegateCallback pointSeriesDelegateCallback;

    List<Vector3> pointPlot = new List<Vector3>();
    const float minDistSq = 25;

    void Update()
    {
        if (currentTask == MouseActionTask.PlaceObstacle)
        {
            if (Input.GetKey(KeyCode.LeftBracket))
            {
                obstacleRadius -= 10 * Time.deltaTime;
            }
            if (Input.GetKey(KeyCode.RightBracket))
            {
                obstacleRadius += 10 * Time.deltaTime;
            }
            if (obstacleRadius < 5) obstacleRadius = 5;
            if (obstacleRadius > 100) obstacleRadius = 100;

            Vector3[] circle = PlotCircle(obstacleRadius, 1);
            lineRenderer.SetVertexCount(circle.Length);
            Vector3 point = Vector3.zero;
            if (TerrainUtil.GetMouseTerrainPt(theCameraToUse, out point))
            {
                for(int i = 0; i < circle.Length; i++){
                    lineRenderer.SetPosition(i, circle[i] + point);
                }
            }

        }
        else if (currentTask == MouseActionTask.PlaceField)
        {
            if (taskFieldParam.FieldType == FameFieldType.Circular)
            {
                if (Input.GetKey(KeyCode.LeftBracket))
                {
                    taskFieldParam.Radius -= 10 * Time.deltaTime;
                }
                if (Input.GetKey(KeyCode.RightBracket))
                {
                    taskFieldParam.Radius += 10 * Time.deltaTime;
                }
                if (taskFieldParam.Radius < 5) taskFieldParam.Radius = 5;
                if (taskFieldParam.Radius > 100) taskFieldParam.Radius = 100;

                Vector3[] circle = PlotCircle(taskFieldParam.Radius, 1);
                lineRenderer.SetVertexCount(circle.Length);
                Vector3 point = Vector3.zero;
                if (TerrainUtil.GetMouseTerrainPt(GetCamera(), out point))
                {
                    for (int i = 0; i < circle.Length; i++)
                    {
                        lineRenderer.SetPosition(i, circle[i] + point);
                    }
                }
            }
            else if (taskFieldParam.FieldType == FameFieldType.Uniform)
            {
                Vector3[] rect = PlotRect(taskFieldParam.Width, taskFieldParam.Height, 1);
                lineRenderer.SetVertexCount(rect.Length);
                Vector3 point = Vector3.zero;
                if (TerrainUtil.GetMouseTerrainPt(GetCamera(), out point))
                {
                    for (int i = 0; i < rect.Length; i++)
                    {
                        lineRenderer.SetPosition(i, rect[i] + point);
                    }
                }
            }
        }
        if (mouseLeftDrag)
        {
            if (currentTask == MouseActionTask.CtrlPoint)
            {
                Vector3 point = Vector3.zero;
                if (TerrainUtil.GetMouseTerrainPt(theCameraToUse, out point))
                {
                    task_ctrlPt.Move(point);
                }
            }
            if (currentTask == MouseActionTask.Plotting)
            {
                Vector3 point = Vector3.zero;
                if (TerrainUtil.GetMouseTerrainPt(theCameraToUse, out point))
                {
                    Plot(point, minDistSq);
                }

            }
        }
    }

    void Mouse2Down()
    {
        Vector3 point = Vector3.zero;
        if (TerrainUtil.GetMouseTerrainPt(GetCamera(), out point))
        {
            if (currentTask == MouseActionTask.GetPointSeries)
            {
                Vector3[] pointSeriesResult = pointPlot.ToArray();
                GetPolyPointsDone(pointSeriesResult);
            }
            else
            {
                if (WorldStateManager.CanMoveUnit)
                {
                    SelectableUnit[] units = UnitSelection.GetCurrentlySelectedUnit();
                    foreach (SelectableUnit unit in units)
                    {
                        if (unit is MoveableUnit)
                        {
                            MoveableUnit moveableUnit = unit as MoveableUnit;
                            moveableUnit.MoveTo(point);
                        }
                    }
                }
            }
        }
    }

    void Mouse1DownDrag(Vector2 screenPosition)
    {
        // Only show the drag selection texture if the mouse has been moved and not if the user made only a single left mouse click
        if (screenPosition != mouseButton1DownPoint)
        {
            mouseButton1DownPoint = MyGUIUtility.BoundedMousePosition(mouseButton1DownPoint);

            mouseLeftDrag = true;
            // while dragging, update the current mouse pos for the selection rectangle.
            mouseButton1UpPoint = screenPosition;
            if (currentTask == MouseActionTask.MovingFlock)
            {
                Vector3 mouseTerrainHitPoint = Vector3.zero;
                if (TerrainUtil.GetMouseTerrainPt(theCameraToUse, out mouseTerrainHitPoint))
                {
                    if (mouseButton1DownTerrainHitPoint.HasValue)
                    {
                        FlockGroup group = FameManager.GetFlockGroup(task_flockID);
                        switch (FameHUD.transformMode)
                        {
                            case TransformMode.Translate:
                                Vector3 delta = mouseTerrainHitPoint - mouseButton1DownTerrainHitPoint.Value;
                                group.MoveGroup(delta);
                                break;
                            case TransformMode.Rotate:
                                group.RotateFormation(mouseDelta_x);
                                break;
                            case TransformMode.Scale:
                                group.ScaleFormation(1+ mouseDelta_y * 0.2f);
                                break;
                        }
                    }
                    mouseButton1DownTerrainHitPoint = mouseTerrainHitPoint;
           
                }
                else
                {
                    mouseButton1DownTerrainHitPoint = null;
                }
            }

        }
    }

    void Mouse1DownDouble(Vector2 screenPosition)
    {
        if (ClickedOnActiveUI(screenPosition))
        {
            RaycastHit hit = new RaycastHit();
            var ray = GetCamera().ScreenPointToRay(mouseButton1DownPoint);
            if (Physics.Raycast(ray, out hit, raycastLength)) // terrainLayerMask
            {
                if (hit.collider.name == "Terrain")
                {
                    mouseButton1DownTerrainHitPoint = hit.point;
                    int clickedOnFlock = FameManager.PointInFlock(hit.point.x, hit.point.z);
                    if (clickedOnFlock != -1)
                    {
                        FlockProperty.Singleton.Show(clickedOnFlock);

                    }
                    else
                    {
                        //poly obstacle do not have a collider
                        int clickOnObstacle = FameManager.PointInObstacle(hit.point.x, hit.point.z);
                        if (clickOnObstacle != -1)
                        {
                            ObstacleProperty.Singleton.Show(clickOnObstacle);
                        }
                        else
                        {
                            int clickOnField = FameManager.PointInField(hit.point.x, hit.point.z);
                            if (clickOnField != -1)
                            {
                                FieldProperty.Singleton.Show(clickOnField);
                            }
                            else if (GetCamera() == PerspectiveCamCtrl.Singleton.camera)
                            {
                                PerspectiveCamCtrl.Singleton.SetCamPosition(hit.point);
                            }
                        }


                    }
                }
                else if (hit.collider.gameObject.layer == 11) // obstacle layer
                {
                    int clickOnObstacle = FameManager.PointInObstacle(hit.point.x, hit.point.z);
                    if (clickOnObstacle != -1)
                    {
                        ObstacleProperty.Singleton.Show(clickOnObstacle);
                    }
                }
                
            }
        }
    }
    Camera theCameraToUse;
    private Camera GetCamera()
    {
        switch (CamHUD.CurrentCamMode)
        {
            case CamMode.perspective:
                return PerspectiveCamCtrl.Singleton.gameObject.camera;
            case CamMode.top:
                return TopCamCtrl.Singleton.gameObject.camera;
            case CamMode.split:
                if (TopCamCtrl.Singleton.MouseCursorInFrame())
                {
                    return TopCamCtrl.Singleton.gameObject.camera;
                }
                else
                {
                    return PerspectiveCamCtrl.Singleton.gameObject.camera;
                }
        }
        return null;//will never get here
    }

    void Mouse1Down(Vector2 screenPosition)
    {
        mouseButton1DownPoint = screenPosition;
        if (ClickedOnActiveUI(mouseButton1DownPoint))
        {
            //Debug.Log(mouseButton1DownPoint);
            RaycastHit hit = new RaycastHit();
            var ray = GetCamera().ScreenPointToRay(mouseButton1DownPoint);
            theCameraToUse = GetCamera();
            if (Physics.Raycast(ray, out hit, raycastLength, 1<<8 | 1<<9)) // terrainLayerMask
            {
                if (hit.collider.name == "CtrlPoint")
                {
                    if (currentTask == MouseActionTask.None)
                    {
                        SetMouseAction(MouseActionTask.CtrlPoint);
                        task_ctrlPt = hit.collider.gameObject.GetComponent<CtrlPtNode>();
                        if (task_ctrlPt.GetObjectName().Equals("Flock"))
                        {
                            FlockGroup group = FameManager.GetFlockGroup(task_ctrlPt.GetObjectID());
                            if (group != null)
                            {
                                if (group is SelectableUnit && UnitSelection.UnitSelectionMode == SelectionMode.group)
                                {
                                    UnitSelection.DeselectUnit();
                                    UnitSelection.SetSelected(group as SelectableUnit);
                                }
                            }
                        }
                        else if (task_ctrlPt.GetObjectName().Equals("Path"))
                        {
                            FamePath path = task_ctrlPt.GetObject() as FamePath;
                            if (path is SelectableUnit)
                            {
                                SelectableUnit selectablePath = path as SelectableUnit;
                                selectablePath.IsSelected = true;
                            }
                        }
                    }
                    else if (currentTask == MouseActionTask.GetPath)
                    {
                        task_ctrlPt = hit.collider.gameObject.GetComponent<CtrlPtNode>();
                        if (task_ctrlPt.GetObjectName().Equals("Path"))
                        {
                            FamePath path = task_ctrlPt.GetObject() as FamePath;
                            GetClickOnPathDone(path.PathID);
                        }
                        else
                        {
                            GetClickOnPathDone(-1);
                        }
                    }
                }
                else if (hit.collider.name == "Terrain")
                {
                    mouseButton1DownTerrainHitPoint = hit.point;
                    if (currentTask == MouseActionTask.Plotting)
                    {
                        startedPlotting = true;
                        Plot(hit.point, minDistSq);
                    }
                    else if (currentTask == MouseActionTask.PlaceObstacle)
                    {
                        GetPlaceObstaclePointClickDone(hit.point);
                    }
                    else if (currentTask == MouseActionTask.PlaceField)
                    {
                        GetPlaceFieldPointClickDone(hit.point, taskFieldParam);
                    }
                    else if (currentTask == MouseActionTask.ListenPointClick)
                    {
                        GetPointClickDone(hit.point);
                    }
                    else if (currentTask == MouseActionTask.GetPointSeries)
                    {
                        startedPlotting = true;
                        Plot(hit.point, 0);
                    }
                    else
                    {
                        if (UnitSelection.UnitSelectionMode == SelectionMode.group)
                        {
                            int clickedOnFlock = FameManager.PointInFlock(hit.point.x, hit.point.z);
                            if (clickedOnFlock != -1)
                            {
                                SetMouseAction(MouseActionTask.MovingFlock);
                                task_flockID = clickedOnFlock;
                                FlockGroup group = FameManager.GetFlockGroup(clickedOnFlock);
                                if (group is SelectableUnit)
                                {
                                    UnitSelection.DeselectUnit();
                                    UnitSelection.SetSelected(group as SelectableUnit);
                                    
                                }
                            }
                            else
                            {
                                UnitSelection.DeselectUnit();
                            }
                        }
                        else
                        {
                            UnitSelection.DeselectUnit();
                            SetMouseAction(MouseActionTask.Selection);
                        }
                    }
                }
            }
        }
    }

    void Mouse1Up(Vector2 screenPosition)
    {
        mouseButton1DownTerrainHitPoint = null;
        mouseButton1UpPoint = screenPosition;
        mouseLeftDrag = false;
        if (currentTask == MouseActionTask.Selection)
        {
            if (WorldStateManager.UnitSelect && !IsInRange(mouseButton1DownPoint, mouseButton1UpPoint))
            {
                Vector3 pt1 = mouseButton1DownPoint;
                Vector3 pt2 = Input.mousePosition;

                Vector3 pt00 = new Vector3(Mathf.Min(pt1.x, pt2.x), Mathf.Min(pt1.y, pt2.y));
                Vector3 pt01 = new Vector3(Mathf.Max(pt1.x, pt2.x), Mathf.Min(pt1.y, pt2.y));
                Vector3 pt10 = new Vector3(Mathf.Max(pt1.x, pt2.x), Mathf.Max(pt1.y, pt2.y));
                Vector3 pt11 = new Vector3(Mathf.Min(pt1.x, pt2.x), Mathf.Max(pt1.y, pt2.y));
                Vector3 point1 = Vector3.zero;
                Vector3 point2 = Vector3.zero;
                Vector3 point3 = Vector3.zero;
                Vector3 point4 = Vector3.zero;
                bool validPoint = true;
                if (!TerrainUtil.GetMouse2TerrainPt(GetCamera(), out point1, pt00)) { validPoint = false; }
                if (!TerrainUtil.GetMouse2TerrainPt(GetCamera(), out point2, pt01)) { validPoint = false; }
                if (!TerrainUtil.GetMouse2TerrainPt(GetCamera(), out point3, pt10)) { validPoint = false; }
                if (!TerrainUtil.GetMouse2TerrainPt(GetCamera(), out point4, pt11)) { validPoint = false; }
                if (validPoint)
                {
                    Vector3[] ptList = new Vector3[4];
                    ptList[0] = point1;
                    ptList[1] = point2;
                    ptList[2] = point3;
                    ptList[3] = point4;
                    UnitSelection.SetSelectedUnits(ptList);
                }
            }
            ClearMouseAction();
        }
        if (currentTask == MouseActionTask.Plotting && startedPlotting)
        {
            PlotDone();
            ClearMouseAction();
        }
        else if(currentTask == MouseActionTask.MovingFlock)
        {
            ClearMouseAction();
        }
        else if (currentTask == MouseActionTask.CtrlPoint)
        {
            ClearMouseAction();
        }
        
    }

    bool IsInRange(Vector2 v1, Vector2 v2)
    {
        float dist = Vector2.Distance(v1, v2);
        if (dist < mouseButtonReleaseBlurRange)
        {
            return true;
        }
        return false;
    }

    Vector3[] PlotRect(float width, float height, float yOffset)
    {
        float halfWidth = width / 2;
        float halfHeight= height / 2;
        Vector3[] result = new Vector3[]{
            new Vector3(-halfWidth, yOffset, -halfHeight),
            new Vector3(halfWidth, yOffset, -halfHeight),
            new Vector3(halfWidth, yOffset, halfHeight),
            new Vector3(-halfWidth, yOffset, halfHeight),
            new Vector3(-halfWidth, yOffset, -halfHeight)
        };
        return result;
    }
    Vector3[] PlotCircle(float radius, float yOffset)
    {
        int numPoints = 20;
        Vector3[] result = new Vector3[numPoints + 1];
        double teeta = 2 * Math.PI / numPoints;
        for (int i = 0; i < numPoints; i++)
        {
            float x = (float)(radius * Math.Cos(teeta * i));
            float z = (float)(radius * Math.Sin(teeta * i));
            result[i] = new Vector3(x, yOffset, z);
        }
        result[numPoints] = result[0];
        return result;
    }
}