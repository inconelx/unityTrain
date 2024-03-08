using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackFrame : PlayerFrameHandle
{
    private static float attackTypeAheadTime = 0.23f;
    private static float attackDuration = 0.41f;
    private static float attackDeterminationStart = 0.21f;
    private static float attackDeterminationEnd = 0.11f;
    private static float attackCanClimb = 0.09f;
    private static float attackCanInterrupt = 0.09f;
    public AttackFrame(PlayerInformation playerInformation) : base(playerInformation) { }
    public override void FrameStart()
    {
        playerInformation.nextState = playerInformation.baseState;
        playerInformation.potentialNextState = playerInformation.baseState;
        playerInformation.climbAllowed = (int)playerInformation.faceDirection == -(int)playerInformation.climbCheckResult;
        playerInformation.walkAllowed = true;
        playerInformation.jumpAllowed = true;
        playerInformation.turnRoundAllowed = false;
        playerInformation.gravityAllowed = true;
        playerInformation.timeMark = attackDuration;
        typeAheadTime = 0;
        stateMark = 0;
        playerInformation.attackCondition = 0;
    }
    public override void FrameAlways()
    {
        UpdateTypeAheadState(attackTypeAheadTime);

        if(playerInformation.timeMark < attackCanClimb)
        {
            playerInformation.climbAllowed = true;
        }
        else
        {
            playerInformation.climbAllowed = (int)playerInformation.faceDirection == -(int)playerInformation.climbCheckResult;
        }

        if (stateNextState == playerInformation.dashState && playerInformation.timeMark < attackCanInterrupt)
        {
            playerInformation.currentState = stateNextState;
            FrameEnd();
            playerInformation.stateUpdateMark = true;
        }
        else
        {
            if (playerInformation.inputAttack && playerInformation.timeMark < attackCanInterrupt)
            {
                stateNextState = playerInformation.attackState;
                typeAheadTime = attackTypeAheadTime / 3;
            }
            if (playerInformation.timeMark < attackDeterminationStart && playerInformation.timeMark > attackDeterminationEnd)
            {
                if (playerInformation.attackCondition == 0)
                {
                    playerInformation.attackDeterminationCollider0.enabled = true;
                }
                else if (playerInformation.attackCondition == 1)
                {
                    playerInformation.attackDeterminationCollider1.enabled = true;
                }
            }
            else
            {
                playerInformation.attackDeterminationCollider0.enabled = false;
                playerInformation.attackDeterminationCollider1.enabled = false;
            }
            if (playerInformation.timeMark < 0)
            {
                if (stateNextState == playerInformation.attackState)
                {
                    playerInformation.timeMark = attackDuration;
                    playerInformation.attackCondition = 1 - playerInformation.attackCondition;
                }
                else
                {
                    playerInformation.currentState = stateNextState;
                    FrameEnd();
                    playerInformation.stateUpdateMark = true;
                }
            }
        }
    }
    public override void FrameEnd()
    {
        playerInformation.attackDeterminationCollider0.enabled = false;
        playerInformation.attackDeterminationCollider1.enabled = false;
    }
}

