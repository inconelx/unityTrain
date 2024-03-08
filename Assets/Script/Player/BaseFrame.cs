using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseFrame : PlayerFrameHandle
{
    public BaseFrame(PlayerInformation playerInformation) : base(playerInformation) { }
    public override void FrameStart()
    {
        playerInformation.nextState = playerInformation.baseState;
        playerInformation.potentialNextState = playerInformation.baseState;
        playerInformation.climbAllowed = true;
        playerInformation.walkAllowed = true;
        playerInformation.jumpAllowed = true;
        playerInformation.turnRoundAllowed = true;
        playerInformation.gravityAllowed = true;
        playerInformation.timeMark = -1;
        stateMark = 0;
        typeAheadTime = 0;
    }
    public override void FrameAlways()
    {
        UpdateTypeAheadState(Time.fixedDeltaTime / 2);
        if (stateNextState != playerInformation.baseState)
        {
            playerInformation.currentState = stateNextState;
            FrameEnd();
            playerInformation.stateUpdateMark = true;
        }
    }
}

