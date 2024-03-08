using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitFrame : PlayerFrameHandle
{
    private static float hitTypeAheadTime = 0.15f;
    private static float hitDuration = 0.49f;
    private static float hitSpeedX = 4;
    private static float hitSpeedY = 4;
    public HitFrame(PlayerInformation playerInformation) : base(playerInformation) { }
    public override void FrameStart()
    {
        playerInformation.nextState = playerInformation.baseState;
        playerInformation.potentialNextState = playerInformation.baseState;
        playerInformation.climbAllowed = false;
        playerInformation.walkAllowed = false;
        playerInformation.jumpAllowed = false;
        playerInformation.turnRoundAllowed = false;
        playerInformation.gravityAllowed = true;
        playerInformation.timeMark = hitDuration;
        stateMark = 0;
        typeAheadTime = 0;
        playerInformation.velocity.y = hitSpeedY;
        playerInformation.velocity.x = hitSpeedX * (int)playerInformation.hitDirection;
        playerInformation.invincibleTime = PlayerInformation.hitInvincibleTime;
    }
    public override void FrameAlways()
    {
        UpdateTypeAheadState(hitTypeAheadTime);

        if (playerInformation.timeMark <= 0)
        {
            playerInformation.currentState = stateNextState;
            FrameEnd();
            playerInformation.currentState.FrameStart();
        }
    }
}


