using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyControl : MonoBehaviour
{
    public enum DirectionX { left = -1, middle, right }
    public enum Condition { aloft, stand }

    public int healthPoint;
    public bool standLeft;
    public bool standRight;
    public DirectionX faceDirection = DirectionX.left;
    public DirectionX blockDirection = DirectionX.middle;
    public DirectionX missDirection = DirectionX.middle;
    public DirectionX hitDirection = DirectionX.middle;
    public Condition enemyCondition;
    public float invincibleTime = -1;
    public bool invincible = false;
    public bool wasHit = false;

    private void Start()
    {
        DoWhileStart();
    }
    private void FixedUpdate()
    {
        EnemyPhysicsControl();
        EnemyPhysicsCheck();
    }
    private void LateUpdate()
    {
        transform.localScale = new Vector3(Math.Abs(transform.localScale.x) * (float)faceDirection, transform.localScale.y, transform.localScale.z);
    }
    private void Update()
    {
        EnemyAnimationControl();
    }
    protected virtual void EnemyPhysicsControl() { }
    protected virtual void EnemyPhysicsCheck(){ }
    protected virtual void EnemyAnimationControl() { }
    protected virtual void DoWhileStart() { }

}
