using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsCheck : MonoBehaviour
{
    //ÒÑ·ÏÆú
    private static float rayLength = 0.04f;

    private float dashCorrectMaxLength;
    private BoxCollider2D boxCollider;
    private Rigidbody2D rigidbody2d;
    private Vector2 boxSize;
    private PlayerInformation playerInformation;
    private void Start()
    {
        playerInformation = GetComponent<PlayerInformation>();
        boxCollider = GetComponent<BoxCollider2D>();
        rigidbody2d = GetComponent<Rigidbody2D>();
        boxSize = boxCollider.bounds.size;
        dashCorrectMaxLength = boxSize.y / 8;
        StartCoroutine(ConditionCheck());
    }
    private void WallCheck()
    {
        playerInformation.currentCondition = PlayerInformation.Condition.aloft;
        RaycastHit2D standCheck = Physics2D.BoxCast(boxCollider.bounds.center + new Vector3(0, -boxSize.y / 2, 0), new Vector2(boxSize.x, rayLength), 0, new Vector2(0, -1), 0, 1 << LayerMask.NameToLayer("Wall"));
        if(standCheck.collider != null && standCheck.normal == Vector2.up)
        {
            playerInformation.currentCondition = PlayerInformation.Condition.stand;
        }
        else if(playerInformation.climbGotten && playerInformation.climbAllowed && !playerInformation.isJumping)
        {
            playerInformation.canStayOnWall = false;
            RaycastHit2D climbCheckRightDown = Physics2D.BoxCast(boxCollider.bounds.center + new Vector3(boxSize.x / 2, -boxSize.y / 3, 0), new Vector2(rayLength, boxSize.y / 3), 0, new Vector2(1, 0), 0, 1 << LayerMask.NameToLayer("Wall"));
            if(climbCheckRightDown.collider != null && climbCheckRightDown.normal == Vector2.left)
            {
                playerInformation.currentCondition = PlayerInformation.Condition.climb;
                playerInformation.climbDirection = PlayerInformation.DirectionX.right;
                RaycastHit2D climbCheckRightUp = Physics2D.BoxCast(boxCollider.bounds.center + new Vector3(boxSize.x / 2, boxSize.y / 3, 0), new Vector2(rayLength, boxSize.y / 3), 0, new Vector2(1, 0), 0, 1 << LayerMask.NameToLayer("Wall"));
                if (climbCheckRightUp.collider != null && climbCheckRightUp.normal == Vector2.left)
                {
                    playerInformation.canStayOnWall = true;
                }
            }
            else
            {
                RaycastHit2D climbCheckLeftDown = Physics2D.BoxCast(boxCollider.bounds.center + new Vector3(-boxSize.x / 2, -boxSize.y / 3, 0), new Vector2(rayLength, boxSize.y / 3), 0, new Vector2(-1, 0), 0, 1 << LayerMask.NameToLayer("Wall"));
                if(climbCheckLeftDown.collider != null && climbCheckLeftDown.normal == Vector2.right)
                {
                    playerInformation.currentCondition = PlayerInformation.Condition.climb;
                    playerInformation.climbDirection = PlayerInformation.DirectionX.left;
                    RaycastHit2D climbCheckLeftUp = Physics2D.BoxCast(boxCollider.bounds.center + new Vector3(-boxSize.x / 2, boxSize.y / 3, 0), new Vector2(rayLength, boxSize.y / 3), 0, new Vector2(-1, 0), 0, 1 << LayerMask.NameToLayer("Wall"));
                    if (climbCheckLeftUp.collider != null && climbCheckLeftUp.normal == Vector2.right)
                    {
                        playerInformation.canStayOnWall = true;
                    }
                }
            }
        }

        if(!playerInformation.invincible && playerInformation.invincibleTime <= 0)
        {
            RaycastHit2D hitCheck = Physics2D.BoxCast(boxCollider.bounds.center, new Vector2(boxSize.x, boxSize.y), 0, new Vector2(0, -1), 0, 1 << LayerMask.NameToLayer("EnemyAttack"));
            if (hitCheck.collider != null)
            {
                Debug.Log("wasHit");
                //playerInformation.wasHit = true;
                playerInformation.healthPoint--;
                if (hitCheck.collider.bounds.center.x < boxCollider.bounds.center.x)
                {
                    playerInformation.hitDirection = PlayerInformation.DirectionX.right;
                }
                else
                {
                    playerInformation.hitDirection = PlayerInformation.DirectionX.left;
                }
            }
        }

        if(playerInformation.currentState == playerInformation.dashState)
        {
            int dir = (int)playerInformation.dashDirection;
            RaycastHit2D dashCorrectCheck = Physics2D.BoxCast(boxCollider.bounds.center + new Vector3(dir * boxSize.x / 2, dashCorrectMaxLength / 2, 0), new Vector2(rayLength, boxSize.y - dashCorrectMaxLength), 0, new Vector2(dir, 0), 0, 1 << LayerMask.NameToLayer("Wall"));
            if(dashCorrectCheck.collider == null)
            {
                RaycastHit2D dashCorrectLengthCheck = Physics2D.Raycast(boxCollider.bounds.center + new Vector3(dir * (boxSize.x / 2 + rayLength / 2), -boxSize.y / 2 + dashCorrectMaxLength, 0), new Vector2(0, -1), dashCorrectMaxLength, 1 << LayerMask.NameToLayer("Wall"));
                if (dashCorrectLengthCheck.collider != null)
                {
                    float dashCorrectLength = dashCorrectLengthCheck.point.y - boxCollider.bounds.center.y + boxSize.y / 2;
                    rigidbody2d.MovePosition(boxCollider.bounds.center + new Vector3(0, -boxSize.y / 2 + dashCorrectLength + rayLength / 2, 0));
                }
            }
        }
    }
    /*private void OnCollisionStay2D(Collision2D collision)
    {
        bool left_points_assigned = false;
        float left_up_points_y = 0;
        float left_down_points_y = 0;
        bool right_points_assigned = false;
        float right_up_points_y = 0;
        float right_down_points_y = 0;

        foreach (ContactPoint2D contact in collision.contacts)
        {
            Vector2 contactNormal = contact.normal;
            if (contactNormal.y > 0.96)
            {
                playerInformation.currentCondition = PlayerInformation.condition.stand;
                break;
            }
            if (playerInformation.climbGotten)
            {
                if (contactNormal == Vector2.right)
                {
                    if (left_points_assigned)
                    {
                        left_up_points_y = Math.Max(contact.point.y, left_up_points_y);
                        left_down_points_y = Math.Min(contact.point.y, left_down_points_y);
                    }
                    else
                    {
                        left_up_points_y = left_down_points_y = contact.point.y;
                        left_points_assigned = true;
                    }
                }
                else if (contactNormal == Vector2.left)
                {
                    if (right_points_assigned)
                    {
                        right_up_points_y = Math.Max(contact.point.y, right_up_points_y);
                        right_down_points_y = Math.Min(contact.point.y, right_down_points_y);
                    }
                    else
                    {
                        right_up_points_y = right_down_points_y = contact.point.y;
                        right_points_assigned = true;
                    }
                }
            }
        }
        if (left_points_assigned && (left_up_points_y - left_down_points_y >= boxSize.y * 0.5f))
        {
            playerInformation.climbDirection = PlayerInformation.directionX.left;
            if(playerInformation.currentCondition != PlayerInformation.condition.stand && !playerInformation.isJumping)
            {
                playerInformation.currentCondition = PlayerInformation.condition.climb;
            }
        }
        else if (right_points_assigned && (right_up_points_y - right_down_points_y >= boxSize.y * 0.5f))
        {
            playerInformation.climbDirection = PlayerInformation.directionX.right;
            if (playerInformation.currentCondition != PlayerInformation.condition.stand && !playerInformation.isJumping)
            {
                playerInformation.currentCondition = PlayerInformation.condition.climb;
            }
        }
    }*/
    private IEnumerator ConditionCheck()
    {
        while (true)
        {
            WallCheck();
            yield return new WaitForFixedUpdate();
        }
    }
}
