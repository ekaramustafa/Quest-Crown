using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{

    /// <summary>
    /// Animation parameters 
    /// </summary>
    private const string IS_WALKING = "IsWalking";
    private const string IS_CLIMBING = "IsClimbing";
    private const string DYING = "Dying";
    private const string SHOOTING_ATTEMPT = "ShootingAttempt";
    private const string SHOOTING_WAITING = "ShootingWaiting";
    private const string SHOOTING_RELEASING = "ShootingReleasing";


    [SerializeField] PlayerController player;

    private Animator animator;
    private InputManager inputManager;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        
    }
    private void Start()
    {
        inputManager = InputManager.GetInstance();
        inputManager.OnShootPerformed += OnShootPerformed;
        inputManager.OnShootCanceled += OnShootCanceled;
        
    }

    // Update is called once per frame
    void Update()
    {
        animator.SetBool(IS_WALKING, player.IsWalking());
        animator.SetBool(IS_CLIMBING, player.IsClimbing());
    }
    
    private void OnShootPerformed(object sender, EventArgs e)
    {
        animator.SetBool(SHOOTING_ATTEMPT, true);
    }

    //animation event reference
    private void AtAttemptShootingStarted()
    {
        
    }

    //animation event reference
    private void AtAttemptShootingFinished()
    {
        animator.SetBool(SHOOTING_WAITING, true);
    }


    //animation event reference
    private void AtReleasingShootingMoment()
    {
        player.ShootArrow();
    }
    private void OnShootCanceled(object sender, EventArgs e)
    {
        animator.SetBool(SHOOTING_RELEASING, true);
        animator.SetBool(SHOOTING_ATTEMPT, false);
        animator.SetBool(SHOOTING_WAITING, false);
    }

    //animation event reference
    private void AtReleasingShootingFinished()
    {
        animator.SetBool(SHOOTING_RELEASING, false);
        animator.SetBool(SHOOTING_ATTEMPT, false);
        animator.SetBool(SHOOTING_WAITING, false);
        player.SetCanMove(true);
    }


    
}
