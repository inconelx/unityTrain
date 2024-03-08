using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathFrame : PlayerFrameHandle
{
    private static float deathSpeedX = 4;
    private static float deathSpeedY = 4;
    public DeathFrame(PlayerInformation playerInformation) : base(playerInformation) { }
    public override void FrameStart()
    {
        playerInformation.nextState = playerInformation.baseState;
        playerInformation.potentialNextState = playerInformation.baseState;
        playerInformation.climbAllowed = false;
        playerInformation.walkAllowed = false;
        playerInformation.jumpAllowed = false;
        playerInformation.turnRoundAllowed = false;
        playerInformation.gravityAllowed = true;
        playerInformation.timeMark = 0;
        stateMark = 0;
        typeAheadTime = 0;
        playerInformation.velocity.y = deathSpeedY;
        playerInformation.velocity.x = deathSpeedX * (int)playerInformation.hitDirection;
        playerInformation.invincible = true;
    }
    public override void FrameAlways()
    {
        UpdateTypeAheadState(-1);
        playerInformation.velocity.x *= 0.9375f;
    }
}


