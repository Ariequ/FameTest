using UnityEngine;
using System.Collections;

public enum CamMode
{
    split,
    top,
    perspective,
}
public static class CamHUD {


    public static CamMode CurrentCamMode = CamMode.split;
    public static void SwitchCamMode(CamMode mode)
    {
        CurrentCamMode = mode;
        switch (mode)
        {
            case CamMode.perspective:
                TopCamCtrl.Singleton.Hide();
                PerspectiveCamCtrl.Singleton.FullScreen();
                break;
            case CamMode.split:
                TopCamCtrl.Singleton.Split();
                PerspectiveCamCtrl.Singleton.Split();
                break;
            case CamMode.top:
                TopCamCtrl.Singleton.FullScreen();
                PerspectiveCamCtrl.Singleton.Hide();
                break;
        }
    }
}
