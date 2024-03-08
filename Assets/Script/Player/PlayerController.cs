using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private static readonly float correctLengthScale = 1.56f;//1.5<x<2
    private static readonly float upCorrectScale = 0.125f;
    private static readonly float downCorrectScale = 0.125f;
    private static readonly float climbCheckScaleMin = 0.125f;
    private static readonly float climbCheckScaleMax = 0.375f;
    private static readonly float stayWallCheckScaleMin = 0.75f;
    private static readonly float stayWallCheckScaleMax = 1f;

    private float correctLength;
    private float upCorrectMaxLength;
    private float downCorrectMaxLength;
    private float climbCheckMinLength;
    private float climbCheckMaxLength;
    private float stayWallCheckMinLength;
    private float stayWallCheckMaxLength;

    private BoxCollider2D boxCollider;
    private Vector2 boxSize;
    private Rigidbody2D rigidbody2d;
    private PlayerInformation playerInformation;
    private PlayerInformation.Condition defaultCondition = PlayerInformation.Condition.aloft;
    private PlayerInformation.Condition previousCondition = PlayerInformation.Condition.aloft;

    private bool upCorrectCheckLeftResult;
    private bool upCorrectCheckRightResult;
    private float upCorrectLeftLength;
    private float upCorrectRightLength;

    private int damageMoment;

    private void Start()
    {
        correctLength = Physics2D.defaultContactOffset * correctLengthScale;
        rigidbody2d = GetComponent<Rigidbody2D>();
        playerInformation = GetComponent<PlayerInformation>();
        playerInformation.currentState = playerInformation.baseState;
        playerInformation.faceDirection = PlayerInformation.DirectionX.right;
        boxCollider = GetComponent<BoxCollider2D>();
        boxSize = boxCollider.bounds.size;

        upCorrectMaxLength = boxSize.y * upCorrectScale;
        downCorrectMaxLength = boxSize.y * downCorrectScale;
        climbCheckMinLength = boxSize.y * climbCheckScaleMin;
        climbCheckMaxLength = boxSize.y * climbCheckScaleMax;
        stayWallCheckMinLength = boxSize.y * stayWallCheckScaleMin;
        stayWallCheckMaxLength = boxSize.y * stayWallCheckScaleMax;

        playerInformation.baseState.FrameStart();
    }
    private void FixedUpdate()
    {
        PlayerPhysicsLogicUpdate();
    }
    private void SetScale()
    {
        if (playerInformation.currentCondition == PlayerInformation.Condition.climb && playerInformation.currentState == playerInformation.baseState)
        {
            transform.localScale = new Vector3(Math.Abs(transform.localScale.x) * (float)playerInformation.climbDirection, transform.localScale.y, transform.localScale.z);
        }
        else
        {
            transform.localScale = new Vector3(Math.Abs(transform.localScale.x) * (float)playerInformation.faceDirection, transform.localScale.y, transform.localScale.z);
        }
    }
    private void PlayerPhysicsLogicUpdate()
    {
        playerInformation.velocity = rigidbody2d.velocity;
        playerInformation.position = rigidbody2d.position;

        playerInformation.stateUpdateMark = false;

        PhyscisCheck();
        playerInformation.hitCheckResult = HitCheck();

        ConditionAndInputsUpdate();

        if (playerInformation.hitCheckResult)
        {
            playerInformation.currentState.FrameEnd();
            playerInformation.healthPoint -= damageMoment;
            if (playerInformation.healthPoint > 0)
            {
                playerInformation.currentState = playerInformation.hitState;
            }
            else
            {
                playerInformation.currentState = playerInformation.deathState;
            }
            playerInformation.stateUpdateMark = true;
        }
        else
        {
            playerInformation.currentState.FrameAlways();
        }

        ConditionAndInputsUpdate();
        defaultCondition = PlayerInformation.Condition.aloft;

        if (playerInformation.stateUpdateMark)
        {
            playerInformation.currentState.FrameStart();
            ConditionAndInputsUpdate();
        }

        if (playerInformation.currentCondition != PlayerInformation.Condition.climb || (int)playerInformation.climbDirection != -(int)playerInformation.inputDirectionX)
        {
            playerInformation.climbRetainTimeMark = PlayerInformation.climbRetainTime;
        }
        if (playerInformation.gravityAllowed && playerInformation.currentCondition != PlayerInformation.Condition.stand)
        {
            if (playerInformation.currentCondition == PlayerInformation.Condition.climb)
            {
                if (playerInformation.canStayOnWall && playerInformation.climbDirection == playerInformation.inputDirectionX && !playerInformation.isJumping)
                {
                    playerInformation.velocity.y = 0;
                }
                else
                {
                    playerInformation.velocity.y = -Math.Min(PlayerInformation.maxClimbDropSpeed,
                        -playerInformation.velocity.y + PlayerInformation.gravitationalAcceleration * Time.fixedDeltaTime);
                }
            }
            else
            {
                playerInformation.velocity.y = -Math.Min(PlayerInformation.maxDropSpeed,
                    -playerInformation.velocity.y + PlayerInformation.gravitationalAcceleration * Time.fixedDeltaTime);
            }
        }

        if (playerInformation.jumpAllowed)
        {
            if (playerInformation.inputJump)
            {
                if (playerInformation.canJump)
                {
                    playerInformation.canJump = false;
                    bool jumpTriggered = false;
                    if (playerInformation.jumpAble && (playerInformation.currentCondition == PlayerInformation.Condition.stand || playerInformation.currentCondition == PlayerInformation.Condition.climb))
                    {
                        playerInformation.jumpAble = false;
                        jumpTriggered = true;
                    }
                    else if (playerInformation.doubleJumpAble && playerInformation.currentCondition == PlayerInformation.Condition.aloft)
                    {
                        playerInformation.doubleJumpAble = false;
                        jumpTriggered = true;
                    }
                    if (jumpTriggered)
                    {
                        playerInformation.isJumping = true;
                        playerInformation.gravityAllowed = true;
                        playerInformation.velocity.y = PlayerInformation.jumpSpeed;
                        if (playerInformation.currentCondition == PlayerInformation.Condition.climb)
                        {
                            playerInformation.climbJumpForceMoveTime = PlayerInformation.climbjumpTime;
                            playerInformation.climbJumpForceMoveDirection = (PlayerInformation.DirectionX)(-(int)(playerInformation.climbDirection));
                        }
                    }
                }
            }
        }

        if (playerInformation.walkAllowed)
        {
            playerInformation.velocity.x = (float)(playerInformation.inputDirectionX) * PlayerInformation.runSpeed;
            if (playerInformation.climbJumpForceMoveTime > 0)
            {
                playerInformation.velocity.x = (float)(playerInformation.inputDirectionX) * PlayerInformation.runSpeed;
                float forecFactor = 1 - (float)(playerInformation.climbJumpForceMoveDirection) * (float)(playerInformation.inputDirectionX);
                playerInformation.velocity.x += (forecFactor * PlayerInformation.runSpeed + PlayerInformation.climbjumpExtraSpeed)
                    * (float)(playerInformation.climbJumpForceMoveDirection)
                    * playerInformation.climbJumpForceMoveTime / PlayerInformation.climbjumpTime;
            }
            else if(playerInformation.climbRetainTimeMark > 0 && playerInformation.currentCondition == PlayerInformation.Condition.climb)
            {
                playerInformation.velocity.x = 0;
                playerInformation.climbRetainTimeMark = Math.Max(-1, playerInformation.climbRetainTimeMark - Time.fixedDeltaTime);
            }
        }
        else
        {
            playerInformation.climbJumpForceMoveTime = -1;
        }

        if (playerInformation.correctAlowed)
        {
            UpCorrect();
            DownCorrect();
        }
        previousCondition = playerInformation.currentCondition;

        rigidbody2d.velocity = playerInformation.velocity;
        SetScale();

        playerInformation.nextState = playerInformation.baseState;
        playerInformation.dashCd = Math.Max(-1, playerInformation.dashCd - Time.fixedDeltaTime);
        playerInformation.invincibleTime = Math.Max(-1, playerInformation.invincibleTime - Time.fixedDeltaTime);
        playerInformation.timeMark = Math.Max(-1, playerInformation.timeMark - Time.fixedDeltaTime);
        playerInformation.climbJumpForceMoveTime = Math.Max(-1, playerInformation.climbJumpForceMoveTime - Time.fixedDeltaTime);

        Debug.Log(playerInformation.currentCondition.ToString());
    }
    private void ConditionAndInputsUpdate()
    {
        playerInformation.currentCondition = defaultCondition;
        if (playerInformation.standCheckResult)
        {
            playerInformation.currentCondition = PlayerInformation.Condition.stand;
        }
        else if (playerInformation.climbCheckResult != PlayerInformation.DirectionX.middle && playerInformation.climbAllowed && !playerInformation.isJumping)
        {
            playerInformation.currentCondition = PlayerInformation.Condition.climb;
            playerInformation.climbDirection = playerInformation.climbCheckResult;
        }
        if (playerInformation.currentCondition == PlayerInformation.Condition.stand || playerInformation.currentCondition == PlayerInformation.Condition.climb)
        {
            playerInformation.jumpAble = true;
            playerInformation.doubleJumpAble = playerInformation.doubleJumpGotten;
            playerInformation.dashChance = true;
        }

        if (playerInformation.turnRoundAllowed && playerInformation.inputDirectionX != PlayerInformation.DirectionX.middle)
        {
            playerInformation.faceDirection = playerInformation.inputDirectionX;
        }
        if (playerInformation.currentCondition == PlayerInformation.Condition.climb)
        {
            playerInformation.faceDirection = (PlayerInformation.DirectionX)(-(int)(playerInformation.climbDirection));
        }
        if (!playerInformation.inputJump) { playerInformation.canJump = true; }
        if (playerInformation.velocity.y <= 0 || !playerInformation.jumpAllowed)
        {
            playerInformation.isJumping = false;
        }
        if (playerInformation.isJumping && !playerInformation.inputJump)
        {
            playerInformation.isJumping = false;
            playerInformation.velocity.y = 0;
        }
    }
    private int PhyscisCheck()
    {
        playerInformation.standCheckResult = false;
        playerInformation.climbCheckResult = PlayerInformation.DirectionX.middle;
        upCorrectCheckLeftResult = false;
        upCorrectCheckRightResult = false;
        float climbCheckLeftMin = float.MaxValue;
        float climbCheckLeftMax = float.MinValue;
        bool climbCheckLeft = false;
        float climbCheckRightMin = float.MaxValue;
        float climbCheckRightMax = float.MinValue;
        bool climbCheckRight = false;
        playerInformation.canStayOnWall = false;
        Vector2 boxPosition = boxCollider.bounds.center;
        if (boxCollider.IsTouchingLayers(1 << LayerMask.NameToLayer("Wall")))
        {
            List<ContactPoint2D> contactPoint2Ds = new();
            ContactFilter2D contactFilter2D = new();
            contactFilter2D.SetLayerMask(1 << LayerMask.NameToLayer("Wall"));
            boxCollider.GetContacts(contactFilter2D, contactPoint2Ds);
            foreach (ContactPoint2D contact in contactPoint2Ds)
            {
                if (boxCollider.Distance(contact.collider).distance < 0)
                {
                    float contactPointY = contact.point.y - boxPosition.y + boxSize.y / 2;
                    if (contact.normal == Vector2.up)
                    {
                        playerInformation.standCheckResult = true;
                    }
                    else if (contact.normal == Vector2.right)
                    {
                        climbCheckLeftMin = Math.Min(climbCheckLeftMin, contactPointY);
                        climbCheckLeftMax = Math.Max(climbCheckLeftMax, contactPointY);
                    }
                    else if (contact.normal == Vector2.left)
                    {
                        climbCheckRightMin = Math.Min(climbCheckRightMin, contactPointY);
                        climbCheckRightMax = Math.Max(climbCheckRightMax, contactPointY);
                    }
                }
            }
            if (climbCheckLeftMin <= climbCheckLeftMax)
            {
                if(climbCheckLeftMax < upCorrectMaxLength)
                {
                    upCorrectCheckLeftResult = true;
                    upCorrectLeftLength = climbCheckLeftMax;
                }
                else
                {
                    climbCheckLeft = climbCheckLeftMax > climbCheckMinLength && climbCheckLeftMin < climbCheckMaxLength;
                }
            }
            if (climbCheckRightMin <= climbCheckRightMax)
            {
                if (climbCheckRightMax < upCorrectMaxLength)
                {
                    upCorrectCheckRightResult = true;
                    upCorrectRightLength = climbCheckRightMax;
                }
                else
                {
                    climbCheckRight = climbCheckRightMax > climbCheckMinLength && climbCheckRightMin < climbCheckMaxLength;
                }
            }
            if (climbCheckLeft)
            {
                playerInformation.climbCheckResult = PlayerInformation.DirectionX.left;
                playerInformation.canStayOnWall = climbCheckLeftMax > stayWallCheckMinLength && climbCheckLeftMin < stayWallCheckMaxLength;
            }
            else if (climbCheckRight)
            {
                playerInformation.climbCheckResult = PlayerInformation.DirectionX.right;
                playerInformation.canStayOnWall = climbCheckRightMax > stayWallCheckMinLength && climbCheckRightMin < stayWallCheckMaxLength;
            }
            return contactPoint2Ds.Count;
        }
        return 0;
    }
    private bool HitCheck()
    {
        if (!playerInformation.invincible && playerInformation.invincibleTime <= 0)
        {
            RaycastHit2D hitCheck = Physics2D.BoxCast(boxCollider.bounds.center, new Vector2(boxSize.x, boxSize.y), 0, new Vector2(0, -1), 0, 1 << LayerMask.NameToLayer("EnemyAttack"));
            if (hitCheck.collider != null)
            {
                Debug.Log("wasHit");
                playerInformation.hitCheckResult = true;
                damageMoment = 1;
                if (hitCheck.collider.bounds.center.x < boxCollider.bounds.center.x)
                {
                    playerInformation.hitDirection = PlayerInformation.DirectionX.right;
                }
                else
                {
                    playerInformation.hitDirection = PlayerInformation.DirectionX.left;
                }
                return true;
            }
        }
        return false;
    }
    private bool UpCorrect()
    {
        int upCorrectDirection = playerInformation.velocity.x == 0 ? 0 : (playerInformation.velocity.x > 0 ? 1 : -1);
        float upCorrectLength = 0;
        bool canUpCorrect = false;
        if (upCorrectDirection == -1 && upCorrectCheckLeftResult)
        {
            upCorrectLength = upCorrectLeftLength;
            canUpCorrect = true;
        }
        else if (upCorrectDirection == 1 && upCorrectCheckRightResult)
        {
            upCorrectLength = upCorrectRightLength;
            canUpCorrect = true;
        }
        if (canUpCorrect)
        {
            RaycastHit2D upCorrectRaycast = Physics2D.BoxCast(boxCollider.bounds.center + new Vector3(0, upCorrectMaxLength / 2 + correctLength, 0), new Vector2(boxSize.x + correctLength * 2, boxSize.y - upCorrectMaxLength), 0, new Vector2(0, 1), upCorrectMaxLength, 1 << LayerMask.NameToLayer("Wall"));
            if(upCorrectRaycast.collider == null)
            {
                int residualFrames = (int)(playerInformation.velocity.y / PlayerInformation.gravitationalAcceleration / Time.fixedDeltaTime);
                if (residualFrames * Time.fixedDeltaTime * (2 * playerInformation.velocity.y - residualFrames * PlayerInformation.gravitationalAcceleration * Time.fixedDeltaTime) / 2 <= Math.Max(0, upCorrectLength))
                {
                    Debug.Log("up");
                    defaultCondition = PlayerInformation.Condition.stand;
                    rigidbody2d.MovePosition(rigidbody2d.position + new Vector2(0, upCorrectLength + correctLength));
                    playerInformation.velocity.y = 0;
                    return true;
                }
            }
        }
        return false;
    }
    private bool DownCorrect()
    {
        if (playerInformation.gravityAllowed && playerInformation.walkAllowed && !playerInformation.isJumping && playerInformation.currentCondition == PlayerInformation.Condition.aloft && previousCondition == PlayerInformation.Condition.stand)
        {
            if(playerInformation.velocity.y <= 0)
            {
                float downVelocity = Math.Min(-2 * downCorrectMaxLength / Time.fixedDeltaTime, playerInformation.velocity.y);
                Vector2 raycastVector = new Vector2(playerInformation.velocity.x, downVelocity).normalized;
                RaycastHit2D downCorrectCheck = Physics2D.BoxCast(boxCollider.bounds.center + new Vector3(0, 0, 0), new Vector2(boxSize.x, boxSize.y), 0, raycastVector, downCorrectMaxLength + correctLength, 1 << LayerMask.NameToLayer("Wall"));
                if (downCorrectCheck.collider != null && downCorrectCheck.normal == Vector2.up)
                {
                    Debug.Log("down");
                    playerInformation.currentCondition = PlayerInformation.Condition.stand;
                    playerInformation.velocity.y = downVelocity;
                    return true;
                }
            }
        }
        return false;
    }
}
