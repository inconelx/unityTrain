using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInputGet : MonoBehaviour
{
    private PlayerInformation playerInformation;
    private PlayerInputController inputControl;
    private void Awake()
    {
        inputControl = new PlayerInputController();
    }
    private void OnEnable()
    {
        inputControl.Enable();
    }
    private void OnDisable()
    {
        inputControl.Disable();
    }
    private void Start()
    {
        playerInformation = GetComponent<PlayerInformation>();
    }
    private void Update()
    {//inputDirectionX
        if (inputControl.MyPlayer.DirectionLeft.WasPressedThisFrame()) { playerInformation.inputDirectionX = PlayerInformation.DirectionX.left; }
        else if (inputControl.MyPlayer.DirectionRight.WasPressedThisFrame()) { playerInformation.inputDirectionX = PlayerInformation.DirectionX.right; }
        else if (inputControl.MyPlayer.DirectionLeft.WasReleasedThisFrame())
        {
            if (inputControl.MyPlayer.DirectionRight.IsPressed()) playerInformation.inputDirectionX = PlayerInformation.DirectionX.right;
            else playerInformation.inputDirectionX = PlayerInformation.DirectionX.middle;
        }
        else if (inputControl.MyPlayer.DirectionRight.WasReleasedThisFrame())
        {
            if (inputControl.MyPlayer.DirectionLeft.IsPressed()) playerInformation.inputDirectionX = PlayerInformation.DirectionX.left;
            else playerInformation.inputDirectionX = PlayerInformation.DirectionX.middle;
        }
        if (!inputControl.MyPlayer.DirectionLeft.IsPressed() && !inputControl.MyPlayer.DirectionRight.IsPressed()) playerInformation.inputDirectionX = PlayerInformation.DirectionX.middle;
        //inputDirectionY
        if (inputControl.MyPlayer.DirectionUp.WasPressedThisFrame()) { playerInformation.inputDirectionY = PlayerInformation.DirectionY.up; }
        else if (inputControl.MyPlayer.DirectionDown.WasPressedThisFrame()) { playerInformation.inputDirectionY = PlayerInformation.DirectionY.down; }
        else if (inputControl.MyPlayer.DirectionUp.WasReleasedThisFrame())
        {
            if (inputControl.MyPlayer.DirectionDown.IsPressed()) playerInformation.inputDirectionY = PlayerInformation.DirectionY.down;
            else playerInformation.inputDirectionY = PlayerInformation.DirectionY.middle;
        }
        else if (inputControl.MyPlayer.DirectionDown.WasReleasedThisFrame())
        {
            if (inputControl.MyPlayer.DirectionUp.IsPressed()) playerInformation.inputDirectionY = PlayerInformation.DirectionY.up;
            else playerInformation.inputDirectionY = PlayerInformation.DirectionY.middle;
        }
        if (!inputControl.MyPlayer.DirectionUp.IsPressed() && !inputControl.MyPlayer.DirectionDown.IsPressed()) playerInformation.inputDirectionY = PlayerInformation.DirectionY.middle;
        //others
        playerInformation.inputJump = inputControl.MyPlayer.Jump.IsPressed();
        playerInformation.inputDash = inputControl.MyPlayer.Dash.IsPressed();
        playerInformation.inputAttack = inputControl.MyPlayer.Attack.IsPressed();

        SetNextState();
    }
    private void SetNextState()
    {
        if (inputControl.MyPlayer.Attack.WasPressedThisFrame())
        {
            playerInformation.nextState = playerInformation.attackState;
            playerInformation.potentialNextState = playerInformation.baseState;
        }
        else if (inputControl.MyPlayer.Dash.WasPressedThisFrame() && playerInformation.dashGotten && playerInformation.dashCd <= 0.09f)
        {
            playerInformation.potentialNextState = playerInformation.dashState;
        }
        if (playerInformation.potentialNextState == playerInformation.dashState && playerInformation.dashCd <= 0)
        {
            if (playerInformation.dashChance)
            {
                playerInformation.nextState = playerInformation.dashState;
            }
            playerInformation.potentialNextState = playerInformation.baseState;
        }
    }
}
