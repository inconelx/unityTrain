using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashFrame : PlayerFrameHandle
{
    private static float dashTypeAheadTime = 0.19f;
    private static float dashDuration = 0.29f;
    private static float dashSpeed = 18;
    public DashFrame(PlayerInformation playerInformation) : base(playerInformation) { }
    public override void FrameStart()
    {
        playerInformation.nextState = playerInformation.baseState;
        playerInformation.potentialNextState = playerInformation.baseState;
        playerInformation.climbAllowed = false;
        playerInformation.walkAllowed = false;
        playerInformation.jumpAllowed = false;
        playerInformation.turnRoundAllowed = false;
        playerInformation.gravityAllowed = false;
        playerInformation.timeMark = dashDuration;
        stateMark = 0;
        typeAheadTime = 0;
        playerInformation.velocity.y = 0;
        if(playerInformation.inputDirectionX == PlayerInformation.DirectionX.middle || playerInformation.currentCondition == PlayerInformation.Condition.climb)
        {
            playerInformation.dashDirection = playerInformation.faceDirection;
        }
        else
        {
            playerInformation.dashDirection = playerInformation.inputDirectionX;
        }
        playerInformation.velocity.x = dashSpeed * (int)playerInformation.dashDirection;
        playerInformation.dashCd = dashDuration + PlayerInformation.dashCdTime;
        if(playerInformation.currentCondition == PlayerInformation.Condition.aloft)
        {
            playerInformation.dashChance = false;
        }
    }
    public override void FrameAlways()
    {
        UpdateTypeAheadState(dashTypeAheadTime);

        playerInformation.velocity.x = dashSpeed * (int)playerInformation.dashDirection;
        playerInformation.velocity.y = 0;
        if (playerInformation.timeMark <= 0)
        {
            playerInformation.currentState = stateNextState;
            FrameEnd();
            playerInformation.stateUpdateMark = true;
        }
    }
}

