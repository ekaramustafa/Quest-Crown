using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey.Utils;
using UnityEngine.Diagnostics;
using Unity.VisualScripting;

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


    private List<CinemachineFramingTransposer> framingTransposers;


    private void Awake()
    {
        framingTransposers = new List<CinemachineFramingTransposer>();
    }

    private void Start()
    {
        inputManager = InputManager.GetInstance();
        
        foreach (CinemachineVirtualCamera cam in cinemachineVirtualCameras)
        {
            cam.GetComponent<CinemachineConfiner>().m_BoundingShape2D = GameManager.GetInstance().GetConfinerCollider();
        }

        foreach (CinemachineVirtualCamera cam in cinemachineVirtualCameras)
        {
            CinemachineFramingTransposer transposer = cam.GetCinemachineComponent<CinemachineFramingTransposer>();
            framingTransposers.Add(transposer);
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
            foreach (CinemachineFramingTransposer transposer in framingTransposers)
            {
                transposer.m_TrackedObjectOffset = Vector3.Lerp(transposer.m_TrackedObjectOffset, Vector3.zero, lerpSpeed * Time.deltaTime);
            }
                return;
        }
       
        foreach (CinemachineFramingTransposer transposer in framingTransposers)
        {
            Vector2 mouseDelta = inputManager.GetMouseDelta();
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

            //To split the sprite when the player is in idle and look opposite directions
            if (!player.GetComponent<PlayerController>().IsWalking())
            {

                if (transposer.m_TrackedObjectOffset.x < -1f)
                {
                    player.transform.localScale = new Vector2(-1f, 1f);
                }
                else if (transposer.m_TrackedObjectOffset.x > 1f)
                {
                    player.transform.localScale = new Vector2(1f, 1f);
                }
            }

        }
    }
}
