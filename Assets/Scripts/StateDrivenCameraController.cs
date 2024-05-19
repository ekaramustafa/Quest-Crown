using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public class StateDrivenCameraController : MonoBehaviour
{
    [SerializeField] List<CinemachineVirtualCamera> cinemachineVirtualCameras;
    [SerializeField] private Camera playerCamera;
    [SerializeField] private Transform player;

    private void Start()
    {
        foreach (CinemachineVirtualCamera cam in cinemachineVirtualCameras)
        {
            cam.GetComponent<CinemachineConfiner>().m_BoundingShape2D = GameManager.GetInstance().GetConfinerCollider();
        }
        if(GameManager.GetInstance().PlayerCount != 1)
        {
            playerCamera.GetComponent<AudioListener>().enabled = false;
        }
    }

}
