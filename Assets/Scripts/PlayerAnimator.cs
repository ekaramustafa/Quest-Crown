using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{

    private const string IS_WALKING = "IsWalking";
    private const string IS_RUNNING = "IsRunning";


    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }
    
}
