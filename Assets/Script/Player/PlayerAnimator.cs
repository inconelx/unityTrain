using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    private static float dashStandUpTime = 0.09f;

    private Animator animator;
    private PlayerInformation playerInformation;
    private PlayerFrameHandle previousState;
    private PlayerInformation.Condition previousCondition;
    private bool previousIsJumping;
    void Start()
    {
        playerInformation = GetComponent<PlayerInformation>();
        animator = GetComponent<Animator>();
        previousState = playerInformation.baseState;
        animator.SetInteger("PlayerState", 0);
        previousIsJumping = false;
        previousCondition = PlayerInformation.Condition.aloft;
    }
    private void Update()
    {
        animator.ResetTrigger("StateChange");
        animator.ResetTrigger("BaseChange");
        animator.ResetTrigger("DashTryEnd");
        bool stateChange = false;
        if (previousState != playerInformation.currentState)
        {
            stateChange = true;
            if (playerInformation.currentState == playerInformation.baseState)
            {
                animator.SetInteger("PlayerState", 0);
            }
            else if(playerInformation.currentState == playerInformation.attackState)
            {
                animator.SetInteger("PlayerState", 1);
            }
            else if (playerInformation.currentState == playerInformation.dashState)
            {
                animator.SetInteger("PlayerState", 2);
            }
            else if (playerInformation.currentState == playerInformation.hitState)
            {
                animator.SetInteger("PlayerState", 3);
            }
            else if (playerInformation.currentState == playerInformation.deathState)
            {
                animator.SetInteger("PlayerState", 4);
            }
            else
            {
                animator.SetInteger("PlayerState", 0);
            }
            animator.SetTrigger("StateChange");
        }

        if(playerInformation.currentState == playerInformation.baseState)
        {
            if(playerInformation.currentCondition == PlayerInformation.Condition.aloft)
            {
                animator.SetFloat("BaseState", playerInformation.isJumping ? 2 : 3);
            }
            else if(playerInformation.currentCondition == PlayerInformation.Condition.climb)
            {
                animator.SetFloat("BaseState", 4);
            }
            else
            {
                if (playerInformation.inputDirectionX != PlayerInformation.DirectionX.middle)
                {
                    animator.SetFloat("BaseState", 1);
                }
                else
                {
                    animator.SetFloat("BaseState", 0);
                }
            }

            bool theTrigger = false;
            if (stateChange || previousCondition != playerInformation.currentCondition)
            {
                if (playerInformation.currentCondition == PlayerInformation.Condition.climb)
                {
                    animator.SetInteger("BaseAction", 3);
                    theTrigger = true;
                }
                if (playerInformation.currentCondition == PlayerInformation.Condition.stand && previousCondition == PlayerInformation.Condition.aloft)
                {
                    animator.SetInteger("BaseAction", 2);
                    theTrigger = true;
                }
                if (playerInformation.currentCondition == PlayerInformation.Condition.aloft && !playerInformation.isJumping)
                {
                    animator.SetInteger("BaseAction", 1);
                    theTrigger = true;
                }
            }

            if (stateChange || previousIsJumping != playerInformation.isJumping)
            {
                if (playerInformation.isJumping)
                {
                    animator.SetInteger("BaseAction", 0);
                    theTrigger = true;
                }
                else
                {
                    if (playerInformation.currentCondition == PlayerInformation.Condition.aloft)
                    {
                        animator.SetInteger("BaseAction", 1);
                        theTrigger = true;
                    }
                }
            }
            
            if (theTrigger)
            {
                animator.SetTrigger("BaseChange");
            }
        }
        else if (playerInformation.currentState == playerInformation.attackState)
        {
            animator.SetInteger("AttackState", playerInformation.attackCondition);
        }
        else if (playerInformation.currentState == playerInformation.dashState)
        {
            if(playerInformation.timeMark <= dashStandUpTime)
            {
                animator.SetTrigger("DashTryEnd");
            }
        }
        else if (playerInformation.currentState == playerInformation.hitState)
        {

        }
        else if (playerInformation.currentState == playerInformation.deathState)
        {
            
        }
        previousState = playerInformation.currentState;
        previousCondition = playerInformation.currentCondition;
        previousIsJumping = playerInformation.isJumping;
    }
}
