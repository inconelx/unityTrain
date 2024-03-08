using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class PlayerInformation : MonoBehaviour
{
    public enum DirectionX {left = -1, middle, right }
    public enum DirectionY { down = -1, middle, up }
    public enum Condition { aloft, stand, climb }

    public static float jumpSpeed = 16;
    public static float runSpeed = 6;
    public static float climbjumpTime = 0.2f;
    public static float climbjumpExtraSpeed = 3;
    public static float gravitationalAcceleration = 32;
    public static float maxClimbDropSpeed = 6;
    public static float maxDropSpeed = 16;
    public static float hitInvincibleTime = 1;
    public static float dashCdTime = 0.29f;
    public static float climbRetainTime = 0.07f;

    public GameObject attackDetermination0;
    public GameObject attackDetermination1;

    public Collider2D attackDeterminationCollider0;
    public Collider2D attackDeterminationCollider1;

    public BaseFrame baseState;
    public DashFrame dashState;
    public AttackFrame attackState;
    public HitFrame hitState;
    public DeathFrame deathState;

    public int healthPoint = 5;

    public PlayerFrameHandle currentState;
    public PlayerFrameHandle nextState;
    public PlayerFrameHandle potentialNextState;

    public DirectionX inputDirectionX = DirectionX.middle;
    public DirectionY inputDirectionY = DirectionY.middle;
    public bool inputAttack;
    public bool inputJump;
    public bool inputDash;

    public bool dashGotten = true;
    public bool climbGotten = true;
    public bool doubleJumpGotten = true;

    public DirectionX faceDirection = DirectionX.right;
    public DirectionX dashDirection;
    public DirectionX climbDirection;
    public DirectionX hitDirection;

    public float climbJumpForceMoveTime;
    public DirectionX climbJumpForceMoveDirection;

    public Condition currentCondition = Condition.aloft;
    public bool canStayOnWall = false;

    public float climbRetainTimeMark = -1;

    public float timeMark = -1;

    public bool climbAllowed;
    public bool walkAllowed;
    public bool jumpAllowed;
    public bool turnRoundAllowed;
    //public bool turnRoundOnWallAllowed;//可能有用
    public bool gravityAllowed;

    public bool stateUpdateMark = false;

    public bool correctAlowed = true;

    public bool canJump = true;
    public bool isJumping = false;

    public float dashCd = -1;
    public bool dashChance = true;

    public bool jumpAble = true;
    public bool doubleJumpAble = false;

    public Vector2 velocity = Vector2.zero;
    public Vector2 position;

    public float invincibleTime = -1;
    public bool invincible = false;

    public DirectionX climbCheckResult;
    public bool standCheckResult;
    public bool hitCheckResult;

    public int attackCondition;

    public PlayerInformation()
    {
        baseState = new BaseFrame(this);
        dashState = new DashFrame(this);
        attackState = new AttackFrame(this);
        hitState = new HitFrame(this);
        deathState = new DeathFrame(this);
        currentState = baseState;
        nextState = baseState;
        potentialNextState = baseState;
        doubleJumpAble = doubleJumpGotten;
    }
    private void Start()
    {
        GameObject a0 = Instantiate(attackDetermination0);
        GameObject a1 = Instantiate(attackDetermination1);
        if (a0 != null)
        {
            a0.transform.SetParent(gameObject.transform, false);
            attackDeterminationCollider0 = a0.GetComponent<Collider2D>();
            attackDeterminationCollider0.enabled = false;
        }
        if (a1 != null)
        {
            a1.transform.SetParent(gameObject.transform, false);
            attackDeterminationCollider1 = a1.GetComponent<Collider2D>();
            attackDeterminationCollider1.enabled = false;
        }
    }
}
