using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerMovement : MonoBehaviourPunCallbacks
{
    // Base code was taken from a previous personal project of mine

    public PlayerMaster pm;
    public GameManager gm;

    // PARAMETERS
    [Header("Movement Parameters")]
    public float walkSpeed = 5f;        // walking speed on ground
    public float jumpSpeed = 20f;       // jump speed
    public float m_AirSpeed = 5f;       // max air speed, separated from walk speed so i can change it to become more intuitive
    public float airAccel = 0.5f;       // air acceleration deceleration
    public float m_DashSpeed = 10f;     // initial speed after pressing dash button
    public float dashDecel = 0.5f;      // dash acceleration deceleration
    public int m_DashCount = 1;         // max dash count (if i want more than one)
    public bool isMine;

    // PRIVATE MOVEMENT VARIABLES
    private float c_AirSpeed;       // [FixedUpdate] current airspeed for control
    private float c_DashSpeed;      // [FixedUpdate] current dashspeed
    private float storedDashX;      // stored dash direction X
    private float storedDashY;      // stored dash direction Y
    private int dashCount = 1;      // actual dash count

    // GAMEPLAY STATE CHECKS
    private bool grounded;  // ground check
    private bool inDash;    // dash check
    private bool isMovementEnabled = true;  // bool used by throw to disable movement
    private bool isDashEnabled = true;      // enables/disables dash for throw animation
    private float faceDir; // stored facing direction for animation

    // EXTRA PRIVATE STATS
    public int score = 0;

    #region Update Loops
    private void Awake()
    {
        gm = FindObjectOfType<GameManager>();
    }

    // Movement Logic Loop
    private void Update()
    {
        if (isMine) // for photon view
        {
            PlayerMovementLogic();
        }
    }

    // Fixed Update Loop for consistency
    void FixedUpdate()
    {
        // Dash Deceleration
        if (inDash)
        {
            c_DashSpeed -= dashDecel;
            if (!grounded)
                pm.Physics.velocity = new Vector2(c_DashSpeed * storedDashX, c_DashSpeed * storedDashY);
            else
                pm.Physics.velocity = new Vector2(c_DashSpeed * storedDashX, 0);

            if (c_DashSpeed <= 0)
            {
                SetPlayerInDash(false);
            }
        }

        // gradually move towards intended direction
        c_AirSpeed = pm.Physics.velocity.x;
        if (-m_AirSpeed > c_AirSpeed)
            c_AirSpeed += airAccel;
        else if (m_AirSpeed < c_AirSpeed)
            c_AirSpeed -= airAccel;
    }
    #endregion

    #region Movement Actions
    // Keyboard and Condition Checks for Update()
    void PlayerMovementLogic()
    {
        // get input axis
        float x = Input.GetAxisRaw("Horizontal");
        float y = Input.GetAxisRaw("Vertical");
        objectFlip(x);

        // standard movement
        if (grounded)
        {
            dashCount = m_DashCount;
            if (Input.GetKeyDown(Constants.PLAYER_MOVEMENT_JUMP))
            {
                Jump();
            }
            else if (!inDash && isMovementEnabled)
            {
                DoGroundMovement(x);
            }
        }
        else if (isMovementEnabled)
        {
            DoAirMovement(x);
        }

        // dash is available regardless of player position
        if (Input.GetMouseButtonDown(Constants.PLAYER_MOVEMENT_DASH) && (x != 0 || y != 0) && inDash == false && isDashEnabled && dashCount > 0)
        {
            Dash(x, y);
        }

        //ANIMATION
        if (inDash)
        {
            changeStates(1);
        }
        else if (isMovementEnabled)
        {
            if (x == 0)
            {
                changeStates(4);
            }
            else
            {
                changeStates(0);
            }
        }
    }

    void Jump()
    {
        grounded = false;
        SetPlayerInDash(false);
        pm.Physics.velocity = new Vector2(pm.Physics.velocity.x, jumpSpeed);
    }

    void DoAirMovement(float x)
    {
        pm.Physics.velocity = new Vector2(c_AirSpeed + (airAccel * x), pm.Physics.velocity.y);
    }

    void DoGroundMovement(float x)
    {
        // GROUND MOVEMENT
        pm.Physics.velocity = new Vector2(walkSpeed * x, pm.Physics.velocity.y);
    }

    void Dash(float x, float y) 
    {
        SetPlayerInDash(true);
        //GameManager.instance.camera.Shake();
        dashCount--;
        c_DashSpeed = m_DashSpeed;
        storedDashX = x;
        storedDashY = y;
        pm.Physics.velocity = new Vector2(m_DashSpeed * storedDashX, m_DashSpeed * storedDashY);
    }
    #endregion

    #region Animation Methods
    public void changeStates(int state)
    {
        bool[] stateArray = new bool[4];
        for (int i = 0; i < 4; i++)
        {
            stateArray[i] = (state == i);
        }
        pm.Anim.SetBool("isRunning", stateArray[0]);
        pm.Anim.SetBool("isDashing", stateArray[1]);
        pm.Anim.SetBool("isFeinting", stateArray[2]);
        pm.Anim.SetBool("isThrowing", stateArray[3]);
    }

    // Object Animation Flipping
    void objectFlip(float x)
    {
        faceDir = x;
        if(faceDir != 0)
            transform.rotation = new Quaternion(0f, 90f - (90 * faceDir), 0f, 0f);
    }
    #endregion

    #region Setters and Getters
    public void SetPlayerInDash(bool dash)
    {
        inDash = dash;
    }

    public void SetIsDashEnabled(bool isEnabled)
    {
        isDashEnabled = isEnabled;
    }

    public void SetStandardPlayerMovementEnabled(bool isEnabled)
    {
        pm.Physics.velocity = new Vector2(0, pm.Physics.velocity.y);
        isMovementEnabled = isEnabled;
    }

    public void SetGrounded(bool isGrounded)
    {
        grounded = isGrounded;
    }

    public bool GetGrounded()
    {
        return grounded;
    }

    public bool GetInDash()
    {
        return inDash;
    }
    #endregion

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(isMine && collision.gameObject.tag == "Player" && inDash)
        {
            if (!collision.gameObject.GetComponent<PlayerMaster>().Anim.GetBool("isDashing"))
            {
                collision.gameObject.GetComponent<PlayerMaster>().RPC.Die();
                gm.AddScore(photonView.Owner.NickName, false);
            }
        }
    }
}
