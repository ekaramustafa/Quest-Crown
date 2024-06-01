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



    private Animator animator;
    private InputManager inputManager;
    private PlayerController player;


    private bool isBowTensionReachedMax;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        isBowTensionReachedMax = false;


    }
    private void Start()
    {
        inputManager = InputManager.GetInstance();
        inputManager.OnShootPerformed += OnShootPerformed;
        inputManager.OnShootCanceled += OnShootCanceled;
        player = transform.parent.GetComponent<PlayerController>();
        player.OnDied += PlayerOnDied;
        player.OnMaximumBowTensionReached += OnMaximumBowTensionReached;
        
        
    }

    private void OnMaximumBowTensionReached(object sender, EventArgs e)
    {
        isBowTensionReachedMax = true;
        player.ShootArrow();
        animator.SetBool(SHOOTING_RELEASING, true);
        animator.SetBool(SHOOTING_ATTEMPT, false);
        animator.SetBool(SHOOTING_WAITING, false);
    }

    private void PlayerOnDied(object sender, EventArgs e)
    {
        animator.SetTrigger(DYING);
    }

    // Update is called once per frame
    void Update()
    {
        animator.SetBool(IS_WALKING, player.IsWalking());
        animator.SetBool(IS_CLIMBING, player.IsClimbing());
    }
    
    private void OnShootPerformed(object sender, EventArgs e)
    {
        animator.SetBool(SHOOTING_RELEASING, false);
        animator.SetBool(SHOOTING_WAITING, false);
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
        if (isBowTensionReachedMax)
        {
            isBowTensionReachedMax = false;
            return;
        }
        player.ShootArrow();
    }

    private void OnShootCanceled(object sender, EventArgs e)
    {
        animator.SetBool(SHOOTING_ATTEMPT,false);
        animator.SetBool(SHOOTING_WAITING, false);
        animator.SetBool(SHOOTING_RELEASING, true);

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
