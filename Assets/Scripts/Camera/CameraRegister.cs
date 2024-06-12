using UnityEngine;
using Cinemachine;

public class CameraRegister : MonoBehaviour
{
    void OnEnable()
    {
        CameraManager.RegisterCamera(GetComponent<CinemachineVirtualCamera>());
    }
    void OnDisable()
    {
        CameraManager.UnregisterCamera(GetComponent<CinemachineVirtualCamera>());
    }
}
