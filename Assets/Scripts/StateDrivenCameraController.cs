using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public class StateDrivenCameraController : MonoBehaviour
{
    [SerializeField] List<CinemachineVirtualCamera> cinemachineVirtualCameras;
    [SerializeField] private Camera playerCamera;
    [SerializeField] private Transform player;
    [Header("Tunable parameters")]
    [SerializeField] private float offsetSpeed = 2f;
    [SerializeField] private Vector2 maxLookUpAhead = new Vector2(5f, 5f);
    [SerializeField] private bool applyLerpSmoothing = false;
    [SerializeField] private float lerpSpeed = 10f;


    private InputManager inputManager;

    private void Start()
    {
        inputManager = InputManager.GetInstance();
        foreach (CinemachineVirtualCamera cam in cinemachineVirtualCameras)
        {
            cam.GetComponent<CinemachineConfiner>().m_BoundingShape2D = GameManager.GetInstance().GetConfinerCollider();
        }
        if (GameManager.GetInstance().PlayerCount != 1)
        {
            playerCamera.GetComponent<AudioListener>().enabled = false;
        }
    }

    private void Update()
    {
        ApplyOffset();
    }

    private void ApplyOffset()
    {
        if(GameManager.GetInstance().GetGameState() == GameManager.GameState.GAMEOVER)
        {
            foreach (CinemachineVirtualCamera cam in cinemachineVirtualCameras)
            {
                CinemachineFramingTransposer transposer = cam.GetCinemachineComponent<CinemachineFramingTransposer>();
                transposer.m_TrackedObjectOffset = Vector3.Lerp(transposer.m_TrackedObjectOffset, Vector3.zero, lerpSpeed * Time.deltaTime);
            }
                return;
        }
       
        foreach (CinemachineVirtualCamera cam in cinemachineVirtualCameras)
        {
            Vector2 mouseDelta = inputManager.GetMouseDelta();
            CinemachineFramingTransposer transposer = cam.GetCinemachineComponent<CinemachineFramingTransposer>();
            Vector3 currentOffset = transposer.m_TrackedObjectOffset;
            Vector3 targetOffset = currentOffset + new Vector3(mouseDelta.x * offsetSpeed * Time.deltaTime, mouseDelta.y * offsetSpeed * Time.deltaTime, 0f);

            targetOffset = new Vector3(Mathf.Clamp(targetOffset.x, -maxLookUpAhead.x, maxLookUpAhead.x), 0f, 0f);
            if (applyLerpSmoothing)
            {
                transposer.m_TrackedObjectOffset = Vector3.Lerp(currentOffset, targetOffset, lerpSpeed * Time.deltaTime);
            }
            else
            {
                transposer.m_TrackedObjectOffset = targetOffset;
            }
            
        }
    }
}
