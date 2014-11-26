using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using FameCSharp;

public class TopCamCtrl : MonoBehaviour {
	public double height = 10.0;
	public double speed = 100.0;
	public int CameraType = 0;
    
//	private Vector3 mousePrevPos;
    private Vector3? mousePrevTerrainPoint;

	private Vector3 prevTarget = new Vector3();
	public Vector3 deltaMouseDistance = Vector3.zero;


    private static TopCamCtrl mSingleton;
    public static TopCamCtrl Singleton
    {
        get
        {
            return mSingleton;
        }
    }


    public Camera CreateSubCamera(){
        GameObject subCamera = new GameObject("Sub Camera");
        Camera subCam = subCamera.AddComponent<Camera>();
        subCam.cullingMask = 1 << 12 | 1<<10 | 1<<9;// selectionLayer, field, ctrlPoint;
        camera.cullingMask = ~subCam.cullingMask;
        
        subCamera.transform.parent = gameObject.transform;
        subCam.transform.localPosition = Vector3.zero;
        subCam.transform.localRotation = Quaternion.identity;
        subCam.rect = camera.rect;
        subCam.isOrthoGraphic = camera.isOrthoGraphic;
        subCam.orthographicSize = camera.orthographicSize;
        subCam.clearFlags = CameraClearFlags.Depth;
        SyncCamera(camera, subCam);
        return subCam;

    }

    private void SyncCamera(Camera main, Camera subCam)
    {
        subCam.rect = main.rect;
        subCam.isOrthoGraphic = main.isOrthoGraphic;
        subCam.orthographicSize = main.orthographicSize;
        subCam.nearClipPlane = main.nearClipPlane;
        subCam.farClipPlane = main.farClipPlane;
        subCam.depth = main.depth + 1;
    }

    void Awake()
    {
        if (mSingleton == null) mSingleton = this;
        else Debug.LogError("Cannot create instance of topCamCtrl more than once");
    }
	// Use this for initialization
    Camera subCamera;
	void Start () {
        subCamera = CreateSubCamera();
    }


    void OnGUI()
    {
        if (!MouseCursorInFrame())
        {
            mousePrevTerrainPoint = null;
            return;
        }
        Event e = Event.current;
        if (e.isMouse && e.button == 1)
        {
            switch (e.type)
            {
                case EventType.MouseUp:
                    mousePrevTerrainPoint = null;
                    break;
                case EventType.MouseDown:
                    {
                        Vector3 terrainPoint = Vector3.zero;
                        if (TerrainUtil.GetMouseTerrainPt(camera, out terrainPoint))
                        {
                            mousePrevTerrainPoint = terrainPoint;
                        }
                    }
                    break;
                case EventType.MouseDrag:
                    {
                        Vector3 terrainPoint = Vector3.zero;
                        if (TerrainUtil.GetMouseTerrainPt(camera, out terrainPoint))
                        {
                            if (mousePrevTerrainPoint != null)
                            {
                                if (mousePrevTerrainPoint.HasValue)
                                {
                                    Vector3 delta = mousePrevTerrainPoint.Value - terrainPoint;
                                    transform.Translate(new Vector3(delta.x, 0, delta.z), Space.World);
                                }
                            }
                            //mousePrevTerrainPoint = terrainPoint;
                        }

                        break;
                    }
            }
        }
    }


	// Update is called once per frame
	void Update () {
		//float v = 0;
		//float h = 0;
		//float deltaV = 0;
		//float deltaH = 0;
		float scrollWheel=0;
		if(MouseCursorInFrame()){
			scrollWheel = Input.GetAxis ("Mouse ScrollWheel");
			//do raycast
			Plane targetPlane = new Plane(Vector3.up, new Vector3(0,0,0));
			float hitdist = 0f;
			Ray ray = camera.ScreenPointToRay (Input.mousePosition);
			Vector3 targetPoint = new Vector3();
			if (targetPlane.Raycast(ray, out hitdist) == true) {
				// Get the point along the ray that hits the calculated distance.
				targetPoint = ray.GetPoint(hitdist);
				deltaMouseDistance = targetPoint - prevTarget;
				deltaMouseDistance.y = 0;
				//moveToTargetPos(targetPoint,true);			
			}

			//mouse scrollWheel, change camera viewing angle
			camera.orthographicSize -= scrollWheel*100;
			if(camera.orthographicSize > 1000){
				camera.orthographicSize = 1000;
			}
			if(camera.orthographicSize < 10){
				camera.orthographicSize = 10;
			}
			prevTarget = targetPoint;
		}
        SyncCamera(camera, subCamera);
	}
	
	public bool MouseCursorInFrame() {
		Vector3 cursorPos = Input.mousePosition;
		float xMinCoord = Screen.width * camera.rect.xMin;
		float yMinCoord = Screen.height * camera.rect.yMin;
		float xMaxCoord = Screen.width * camera.rect.xMax;
		float yMaxCoord = Screen.height * camera.rect.yMax;
		if(cursorPos.x > xMinCoord && cursorPos.x < xMaxCoord &&
		   cursorPos.y > yMinCoord && cursorPos.y < yMaxCoord)
			return true;
		else
			return false;
	}

    public void FullScreen()
    {
        camera.enabled = true;
        subCamera.enabled = true;
        camera.rect = new Rect(0, 0, 1, 1);
        subCamera.rect = new Rect(0, 0, 1, 1);
    }

    public void Split()
    {
        camera.enabled = true;
        subCamera.enabled = true;
        camera.rect = new Rect(0, 0, 0.5f, 1);
        subCamera.rect = new Rect(0, 0, 0.5f, 1);

    }
    public void Hide()
    {
        camera.enabled = false;
        subCamera.enabled = false;
    }
}
