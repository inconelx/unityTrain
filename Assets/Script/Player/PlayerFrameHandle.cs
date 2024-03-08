using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFrameHandle
{
    protected int stateMark = 0;
    protected PlayerFrameHandle stateNextState;
    protected float typeAheadTime = 0;
    protected PlayerInformation playerInformation;
    public virtual void FrameStart() { }
    public virtual void FrameAlways() { }
    public virtual void FrameEnd() { }
    public PlayerFrameHandle(PlayerInformation playerInformation)
    {
        this.playerInformation = playerInformation;
    }
    protected void UpdateTypeAheadState(float stateTypeAheadTime)
    {
        if (playerInformation.nextState != playerInformation.baseState)
        {
            stateNextState = playerInformation.nextState;
            playerInformation.nextState = playerInformation.baseState;
            typeAheadTime = stateTypeAheadTime;
        }
        if (typeAheadTime <= 0)
        {
            stateNextState = playerInformation.baseState;
        }
        typeAheadTime = Math.Max(-1, typeAheadTime - Time.fixedDeltaTime);
    }
}

