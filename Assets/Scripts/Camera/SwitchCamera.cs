using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class SwitchCamera : MonoBehaviour
{
    [SerializeField] CinemachineVirtualCamera cameraToSwitchTo;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            CameraManager.SwitchCamera(cameraToSwitchTo);
        }
    }
}
