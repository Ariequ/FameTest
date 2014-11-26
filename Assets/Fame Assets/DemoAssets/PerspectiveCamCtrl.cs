using UnityEngine;
using System.Collections;

public class PerspectiveCamCtrl : MonoBehaviour {
	public double height = 10.0;
	public double speed = 100.0;
    float yRotSpeed = 30f;
    public float mouseSensivity = 120;

	private Vector3 mousePrevPos;

    //FreeCam variables
    public float camMoveSpeed = 700f;
    public float camXRot = 19;
    public float camYRot = 90;
    public float camZRot = 0;
    public Vector3 camFreeModePos;

    public void SetCamPosition(Vector3 position)
    {
        camFreeModePos = position;
    }
    private static PerspectiveCamCtrl mSingleton;
    public static PerspectiveCamCtrl Singleton
    {
        get
        {
            return mSingleton;
        }
    }
    void Awake()
    {
        if (mSingleton == null) mSingleton = this;
        else Debug.LogError("Cannot create instance of topCamCtrl more than once");
    }



	// Use this for initialization
	void Start () {
        camera.cullingMask = ~(1 << 9);
	}
	
	// Update is called once per frame
	void Update () {
        FreeCam();
	}

    public void FreeCam()
    {
        Vector3 camPos = camera.transform.position;
        Vector3 camMoveDir = new Vector3();
        Vector3 camDesiredRot = Vector3.zero;

        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
        {
            camMoveDir = camera.transform.forward;
        }
        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
        {
            camMoveDir = -camera.transform.forward;
        }
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            camMoveDir = Vector3.Cross(camera.transform.forward, camera.transform.up);
        }
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            camMoveDir = -Vector3.Cross(camera.transform.forward, camera.transform.up);
        }
        if (Input.GetKey(KeyCode.Q))
        {
            camYRot -= Time.deltaTime * yRotSpeed;
        }
        if (Input.GetKey(KeyCode.E))
        {
            camYRot += Time.deltaTime * yRotSpeed;
        }
        if (mouseCursorInFrame())
        {
            if (Input.GetMouseButton(1))
            {
                camYRot -= Input.GetAxis("Mouse X") * (Time.deltaTime * mouseSensivity);
                camYRot = ClampAngle(camYRot, -360, 360);

                camXRot += Input.GetAxis("Mouse Y") * (Time.deltaTime * mouseSensivity);
                camXRot = ClampAngle(camXRot, -85, 85);
            }

            if (Input.GetAxis("Mouse ScrollWheel") > 0)
            {
                camMoveDir = camera.transform.forward;
            }
            if (Input.GetAxis("Mouse ScrollWheel") < 0)
            {
                camMoveDir = -camera.transform.forward;
            }
        }
        

        camDesiredRot = new Vector3(camXRot, camYRot, camZRot);
        camMoveDir.Normalize();
        camMoveDir *= (camMoveSpeed * Time.deltaTime);
        camFreeModePos += camMoveDir;

        if (Terrain.activeTerrain != null)
        {
            float desiredTerrainFreeModeY = Terrain.activeTerrain.SampleHeight(camFreeModePos);
            float camFreeModePosY = Mathf.Clamp(camFreeModePos.y, desiredTerrainFreeModeY + 1f, desiredTerrainFreeModeY + 100f);
            camFreeModePos = new Vector3(camFreeModePos.x, camFreeModePosY, camFreeModePos.z);
            //Limit the elevation
        }

        Vector3 finalPosition = Vector3.Lerp(camPos, camFreeModePos, Time.deltaTime + 0.1f);
        if (Terrain.activeTerrain != null)
        {
            float finalDesiredY = Terrain.activeTerrain.SampleHeight(finalPosition);
            if (finalPosition.y < finalDesiredY)
            {
                finalPosition.y = finalDesiredY;
            }
        }
        camera.transform.position = finalPosition;
        //Vector3 currentCamera = camera.transform.rotation.eulerAngles;
        camera.transform.rotation = Quaternion.Slerp(camera.transform.rotation, Quaternion.Euler(camDesiredRot), Time.deltaTime + 0.05f);
    }


	private bool mouseCursorInFrame() {
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

    private static float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360)
            angle += 360;
        if (angle > 360)
            angle -= 360;
        return Mathf.Clamp(angle, min, max);
    }

    public void FullScreen()
    {
        camera.enabled = true;
        camera.rect = new Rect(0, 0, 1, 1);
    }

    public void Split()
    {
        camera.enabled = true;
        camera.rect = new Rect(0.5f, 0, 0.5f, 1);
    }
    public void Hide()
    {
        camera.enabled = false;
    }
}
