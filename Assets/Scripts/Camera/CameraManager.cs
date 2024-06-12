using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraManager : MonoBehaviour
{
    static List<CinemachineVirtualCamera> cameras = new List<CinemachineVirtualCamera>();
    public static CinemachineVirtualCamera ActiveCamera = null;
    public static bool IsCameraActive(CinemachineVirtualCamera camera)
    {
        return ActiveCamera == camera;
    }
    public static void SwitchCamera(CinemachineVirtualCamera camera)
    {
        camera.Priority = 10;
        ActiveCamera = camera;

        foreach (CinemachineVirtualCamera cam in cameras)
        {
            if (cam != camera)
            {
                cam.Priority = 0;
            }
        }
    }
    public static void RegisterCamera(CinemachineVirtualCamera camera)
    {
        cameras.Add(camera);
    }
    public static void UnregisterCamera(CinemachineVirtualCamera camera)
    {
        cameras.Remove(camera);
    }
}
