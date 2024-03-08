using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoarControl : EnemyControl
{
    private static readonly float rayLength = 0.05f;
    private static readonly float idelTime = 1.0f;
    private static readonly float walkTime = 3.0f;
    private static readonly float hitTime = 0.15f;
    private static readonly float deathTime = 0.45f;
    private static readonly float walkSpeed = 2;
    private static readonly float runSpeed = 8;
    private static readonly int maxHealth = 10;
    private static readonly float hitSpeedX = 4;
    private static readonly float hitSpeedY = 1.5f;
    private enum State { idel, walk, run, hit, death}

    public GameObject damageCheck;

    private BoxCollider2D boxCollider;
    private Collider2D damageCheckCollider;
    private Vector2 boxSize;
    private Rigidbody2D rigidbody2d;
    private Animator animator;
    private float timeMark;
    private State previousState;
    private State stateMark;
    private Vector2 velocity;

    protected override void DoWhileStart()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        boxSize = boxCollider.bounds.size;
        rigidbody2d = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        faceDirection = DirectionX.right;
        healthPoint = maxHealth;
        stateMark = State.idel;
        previousState = State.idel;
        animator.SetFloat("BoarState", 0);
        GameObject boarDamageCheck = Instantiate(damageCheck);
        if (boarDamageCheck != null)
        {
            boarDamageCheck.transform.SetParent(gameObject.transform, false);
            damageCheckCollider = boarDamageCheck.GetComponent<Collider2D>();
        }
    }
    protected override void EnemyPhysicsControl()
    {
        velocity = rigidbody2d.velocity;

        if (wasHit)
        {
            velocity.y = hitSpeedY;
            velocity.x = hitSpeedX * (int)hitDirection;
            faceDirection = (DirectionX)(-(int)hitDirection);
            
            if (healthPoint > 0)
            {
                stateMark = State.hit;
                timeMark = hitTime;
                invincible = true;
            }
            else
            {
                stateMark = State.death;
                timeMark = deathTime;
                damageCheckCollider.enabled = false;
            }
        }
        wasHit = false;

        if (stateMark == State.walk)
        {
            bool isTrans = false;
            if (blockDirection != DirectionX.middle)
            {
                faceDirection = blockDirection;
                isTrans = true;
            }
            else if(missDirection != DirectionX.middle)
            {
                faceDirection = missDirection;
                isTrans = true;
            }
            velocity.x = (int)faceDirection * walkSpeed;

            if(timeMark < 0)
            {
                if (!isTrans)
                {
                    faceDirection = (DirectionX)(-(int)faceDirection);
                }
                stateMark = State.idel;
                timeMark = idelTime;
                velocity.x = 0;
            }
        }
        else if (stateMark == State.idel)
        {
            if (timeMark < 0)
            {
                stateMark = State.walk;
                velocity.x = (int)faceDirection * walkSpeed;
                timeMark = walkTime;
            }
        }
        else if (stateMark == State.hit)
        {
            if (timeMark < 0)
            {
                invincible = false;
                stateMark = State.walk;
                velocity.x = (int)faceDirection * walkSpeed;
                timeMark = walkTime;
            }
        }
        else if (stateMark == State.death)
        {
            velocity.x *= 0.9375f;
            if (timeMark < 0)
            {
                Destroy(gameObject);
            }
        }
        rigidbody2d.velocity = velocity;
        timeMark = Math.Max(-1, timeMark - Time.fixedDeltaTime);
    }
    protected override void EnemyPhysicsCheck()
    {
        if(stateMark != State.death && stateMark != State.hit)
        {
            enemyCondition = Condition.aloft;
            RaycastHit2D standCheck = Physics2D.BoxCast(boxCollider.bounds.center + new Vector3(0, -boxSize.y / 2, 0), new Vector2(boxSize.x, rayLength), 0, new Vector2(0, -1), 0, 1 << LayerMask.NameToLayer("Wall"));
            if (standCheck.collider != null && standCheck.normal == Vector2.up)
            {
                enemyCondition = Condition.stand;
            }

            RaycastHit2D blockCheckRight = Physics2D.BoxCast(boxCollider.bounds.center + new Vector3(boxSize.x / 2, 0, 0), new Vector2(rayLength, boxSize.y), 0, new Vector2(1, 0), 0, 1 << LayerMask.NameToLayer("Wall"));
            if (blockCheckRight.collider != null && blockCheckRight.normal == Vector2.left)
            {
                blockDirection = DirectionX.left;
            }
            else
            {
                RaycastHit2D blockCheckLeft = Physics2D.BoxCast(boxCollider.bounds.center + new Vector3(-boxSize.x / 2, 0, 0), new Vector2(rayLength, boxSize.y), 0, new Vector2(-1, 0), 0, 1 << LayerMask.NameToLayer("Wall"));
                if (blockCheckLeft.collider != null && blockCheckLeft.normal == Vector2.right)
                {
                    blockDirection = DirectionX.right;
                }
                else
                {
                    blockDirection = DirectionX.middle;
                }
            }

            missDirection = DirectionX.middle;
            if (enemyCondition == Condition.stand)
            {
                RaycastHit2D missRightCheck = Physics2D.Raycast(boxCollider.bounds.center + new Vector3(boxSize.x / 2, -boxSize.y / 2, 0), new Vector2(0, -1), rayLength, 1 << LayerMask.NameToLayer("Wall"));
                if (missRightCheck.collider == null)
                {
                    missDirection = DirectionX.left;
                }
                else
                {
                    RaycastHit2D missLeftCheck = Physics2D.Raycast(boxCollider.bounds.center + new Vector3(-boxSize.x / 2, -boxSize.y / 2, 0), new Vector2(0, -1), rayLength, 1 << LayerMask.NameToLayer("Wall"));
                    if (missLeftCheck.collider == null)
                    {
                        missDirection = DirectionX.right;
                    }
                    else
                    {
                        missDirection = DirectionX.middle;
                    }
                }
            }

            if (!invincible && invincibleTime <= 0)
            {
                RaycastHit2D hitCheck = Physics2D.BoxCast(boxCollider.bounds.center, new Vector2(boxSize.x, boxSize.y), 0, new Vector2(0, -1), 0, 1 << LayerMask.NameToLayer("CharacterAttack"));
                if (hitCheck.collider != null)
                {
                    AttackCheck attackCheck = hitCheck.collider.gameObject.GetComponent<AttackCheck>();
                    if (attackCheck != null)
                    {
                        wasHit = true;
                        healthPoint -= attackCheck.damage;
                        Debug.Log(healthPoint.ToString() + " " + attackCheck.damage.ToString());
                        if (hitCheck.collider.bounds.center.x < boxCollider.bounds.center.x)
                        {
                            hitDirection = DirectionX.right;
                        }
                        else
                        {
                            hitDirection = DirectionX.left;
                        }
                    }
                }
            }
        }
    }
    protected override void EnemyAnimationControl()
    {
        animator.SetFloat("BoarState", (int)stateMark);
    }
}
