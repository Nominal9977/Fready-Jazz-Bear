using FMOD.Studio;
using UnityEngine;
using static ChannelNames;
using TMPro;
using Unity.VisualScripting.FullSerializer;

public class PlayerController : MonoBehaviour
{
    #region Variables
    // Conditions
    bool mIsGrounded = false;

    // Inputs
    float mHorInput = 0.0f;
    float mVerInput = 0.0f;
    bool mJumpPressed = false;
    bool mJumpDown = false;
    bool mSpecialPressed = false;

    // Settings
    [Header("Movement Settings")]
    public float mMoveSpeed = 2.0f;
    public float mMoveAcceleration = 30.0f;
    public float mMoveDecceleration = 90.0f;

    [Header("Rolling Settings")]
    public float mRollSpeed = 5.0f;
    public float mRollEndSpeed = 2.0f;
    public float mRollTime = 1.0f;
    public float mLastRollTime = 0f;

    [Header("Jump Settings")]
    public float mJumpForce = 4.0f;
    public float mLastJumpTime = 0f;
    public float mJumpCutDivisor = 3f;
    private float mJumpGraceTime = 0.1f;

    [Header("Tilt Settings")]
    public float mMaxTilt = 15.0f;
    public float mTiltSpeed = 10.0f;

    // Components
    public Rigidbody mRigidBody;
    public GameObject mCapsule;
    public StateMachine mStateMachine;

    // Debug Compontents
    [Header("Debug")]
    [SerializeField] private TMP_Text mStateText;
    [SerializeField] private TMP_Text mGroundedText;
    #endregion

    void Start()
    {
        FetchComponents();

        mStateMachine.StartStateWithAuto(this);

        SetupTransitions();
    }

    void Update()
    {
        // Get Inputs Each Frame
        FetchInputs();

        // Update State Machine
        mStateMachine.update();

        // Visualie Movement
        VisualizeMovement();
    }

    private void FixedUpdate()
    {
        CheckGrounded();
        mStateMachine.fixedUpdate();
    }

    #region Visualize Functions
    public void VisualizeMovement()
    {
        // Get Current Velocity
        Vector3 currentVelocity = mRigidBody.linearVelocity;
        Vector3 horizontalVelocity = new Vector3(currentVelocity.x, 0, currentVelocity.z);

        // Calculate tilt based on grounded state
        Quaternion targetTilt = Quaternion.identity;

        if (horizontalVelocity.magnitude > 0.1f)
        {
            // Get the movement direction (normalized)
            Vector3 moveDirection = horizontalVelocity.normalized;

            // Determine tilt direction based on grounded and vertical velocity
            float tiltMultiplier = -1;

            // Calculate tilt angle based on horizontal speed
            float speedRatio = Mathf.Clamp01(horizontalVelocity.magnitude / mMoveSpeed);
            float tiltAngle = mMaxTilt * speedRatio * tiltMultiplier;

            // Create tilt axis (perpendicular to movement direction, horizontal)
            Vector3 tiltAxis = Vector3.Cross(Vector3.up, moveDirection).normalized;

            // Apply tilt rotation
            targetTilt = Quaternion.AngleAxis(tiltAngle, tiltAxis);
        }

        // Smoothly interpolate to target tilt
        mCapsule.transform.localRotation = Quaternion.Slerp(
            mCapsule.transform.localRotation,
            targetTilt,
            Time.deltaTime * mTiltSpeed
        );
    }

    public void VisualizeRoll(float percent)
    {
        float capsuleScale = 1f;
        float capusleRollScale = 0.5f;
        float capsuleHeight = 0.0f;
        float capusleRollHeight = -0.5f;
        float currentScale = 0.0f;
        float currentHeight = 0.0f;

        if (percent < 0.25)
        {
            currentScale = Mathf.Lerp(capsuleScale, capusleRollScale, percent*4);
            currentHeight = Mathf.Lerp(capsuleHeight, capusleRollHeight, percent * 4);
        }
        else if (percent < 0.75)
        {
            currentScale = capusleRollScale;
            currentHeight = capusleRollHeight;
        } 
        else if (percent <= 1)
        {
            float newPercent = percent - 0.75f;
            currentScale = Mathf.Lerp(capusleRollScale, capsuleScale, newPercent * 4);
            currentHeight = Mathf.Lerp(capusleRollHeight, capsuleHeight, newPercent * 4);

        }

        // Set Scale
        mCapsule.transform.localScale = new Vector3(capsuleScale, currentScale, capsuleScale);

        // Set Height
        mCapsule.transform.localPosition = new Vector3(0, currentHeight, 0);
    }    
    #endregion
    public void MovePlayer(float effectiveness = 1.0f)
    {
        // Get Script Rigidbody
        Rigidbody rb = mRigidBody;

        // Get Wished Input (Input * Camera Direction)
        Vector3 wishedInput = GetWishedInput();

        // Get Current & Target Velocity
        Vector3 currentVelocity = rb.linearVelocity;
        Vector3 currentHorizontal = new Vector3(currentVelocity.x, 0, currentVelocity.z);
        Vector3 targetVelocity = wishedInput * mMoveSpeed;

        bool isReversing = Vector3.Dot(currentHorizontal.normalized, wishedInput.normalized) < -0.5f;


        // Apply Acceleration If Direction Input, Otherwise Deccelerate
        float rate = (wishedInput.magnitude > 0) ? mMoveAcceleration : mMoveDecceleration;

        // Get The New Horizontal Vecovity
        Vector3 newHorizontalVelocity = Vector3.MoveTowards(
            new Vector3(currentVelocity.x, 0, currentVelocity.z),
            targetVelocity,
            (rate * Time.fixedDeltaTime)// * effectiveness
        );

        // Set Horizontal Velocity
        rb.linearVelocity = new Vector3(newHorizontalVelocity.x, currentVelocity.y, newHorizontalVelocity.z);
    }
    void CheckGrounded()
    {
        // Raycast or SphereCast downward
        float rayLength = 1.1f;
        mIsGrounded = Physics.Raycast(transform.position, Vector3.down, rayLength);
        SetGroundedText($"Grounded: {mIsGrounded}");
    }

    #region Input Functions
    public Vector3 GetWishedInput()
    {
        // Get Camera Forward & Right
        Vector3 cameraForward = Camera.main.transform.forward;
        Vector3 cameraRight = Camera.main.transform.right;

        // Remove Y Component & Normalize
        cameraForward.y = 0;
        cameraRight.y = 0;
        cameraForward.Normalize();
        cameraRight.Normalize();

        // Normalize Input
        Vector3 inputDirection = new Vector3(mHorInput, 0, mVerInput);
        inputDirection.Normalize();


        return (cameraForward * inputDirection.z + cameraRight * inputDirection.x);
    }

    void FetchInputs()
    {
        // Get Movement Inputs
        mHorInput = Input.GetAxisRaw("Horizontal");
        mVerInput = Input.GetAxisRaw("Vertical");

        // Get Other Inputs
        mJumpPressed = Input.GetKeyDown(KeyCode.Space);
        mJumpDown = Input.GetKey(KeyCode.Space);
        mSpecialPressed = Input.GetKeyDown(KeyCode.L);
    }

    public bool IsJumpDown()
    {
        return mJumpDown; 
    }
    #endregion
    
    #region Setup Functions
    /*
        Currently using per state transitions, could be better to use
        global transitions and rely on updaing conditions, but will
        start with this for now and change later if needed
     */
    void SetupTransitions()
    {
        #region Idle Transitions

        // If Jump Pressed, Transition To Jump State
        mStateMachine[sIdle.mtype].addTrans(sJumping.mtype, () =>
        {
            return mJumpPressed;
        });

        // If Any Direction Input, Transition To Walk State
        mStateMachine[sIdle.mtype].addTrans(sWalking.mtype, () =>
        {
            if(mHorInput != 0.0f || mVerInput != 0.0f)
            {
                return true;
            }

            return false;
        });

        #endregion

        #region Walking Transitions 
        // If Jump Pressed, Transition To Jump State
        mStateMachine[sWalking.mtype].addTrans(sJumping.mtype, () =>
        {
            return mJumpPressed;
        });

        // If No Direction Input & No Velocity, Transition To Idle
        mStateMachine[sWalking.mtype].addTrans(sIdle.mtype, () =>
        {
            return (mHorInput == 0.0f && mVerInput == 0.0f) && 
            (mRigidBody.linearVelocity.magnitude < 0.05f);
        });

        // If Special Pressed, Transition To Rolling State
        mStateMachine[sWalking.mtype].addTrans(sRolling.mtype, () =>
        {
            return mSpecialPressed;
        });
        #endregion

        #region Jumping Transitions

        // If No Direction Input & Grounded, Transition to Idle State
        mStateMachine[sJumping.mtype].addTrans(sIdle.mtype, () =>
        {
            bool noJumpGrace = Time.time > mLastJumpTime + mJumpGraceTime;
            return (mHorInput == 0.0f && mVerInput == 0.0f) && mIsGrounded && noJumpGrace;
        });

        // If Grounded & Any Direction Input, Transition to Walking State
        mStateMachine[sJumping.mtype].addTrans(sWalking.mtype, () =>
        {
            bool noJumpGrace = Time.time > mLastJumpTime + mJumpGraceTime;
            return (mHorInput != 0.0f || mVerInput != 0.0f) && mIsGrounded && noJumpGrace;
        });

        // If Not Grounded & Special Input, Transition To Dash State
        mStateMachine[sJumping.mtype].addTrans(sDashing.mtype, () =>
        {
            return mSpecialPressed && !mIsGrounded;
        });

        #endregion

        #region Roll Transitions
        
        // If Roll Finsihed & Any Direction Input, Transition To Walk State
        mStateMachine[sRolling.mtype].addTrans(sWalking.mtype, () =>
        {
            bool rollFinished = Time.time > mLastRollTime + mRollTime;
            return (mHorInput != 0.0f || mVerInput != 0.0f) && rollFinished;
        });

        // If Roll Finsihed & No Direction Input, Transition To Idle State
        mStateMachine[sRolling.mtype].addTrans(sIdle.mtype, () =>
        {
            bool rollFinished = Time.time > mLastRollTime + mRollTime;
            return (mHorInput == 0.0f && mVerInput == 0.0f) && rollFinished;
        });
        #endregion
    }

    void FetchComponents()
    {
        // Get Player Rigidbody
        mRigidBody = GetComponent<Rigidbody>();

        // Find The State Machine In Scene If Not Set Already
        if (mStateMachine == null)
        {
            mStateMachine = GetComponent<StateMachine>();
        }
    }
    #endregion

    #region Debug Functions
    public void SetStateText(string text)
    {
        mStateText.text = text;
    }

    public void SetGroundedText(string text)
    {
        mGroundedText.text = text;
    }
    #endregion
}

#region States
public class sIdle : StateAuto<sIdle, PlayerController>
{
    public override bool mIsDefault => true;
    public override void enter()
    {
        mScript.SetStateText("State: Idle");
    }

    public override void update()
    {

    }

    public override void exit()
    {
    }
}

public class sWalking : StateAuto<sWalking, PlayerController>
{
    public override void enter()
    {
        mScript.SetStateText("State: Walking");
    }

    public override void update()
    {

    }

    public override void fixedUpdate()
    {
        mScript.MovePlayer();
    }

    public override void exit()
    {
    }
}

public class sRolling : StateAuto<sRolling, PlayerController>
{
    Vector3 rollDirection;
    public override void enter()
    {
        mScript.SetStateText("State: Rolling");

        // Get Wished Direction
        rollDirection = mScript.GetWishedInput();
        

        // Set Last Roll Time
        mScript.mLastRollTime = Time.time;
    }

    public override void update()
    {

    }

    public override void fixedUpdate()
    {
        float timeInRoll = Time.time - mScript.mLastRollTime;
        float percentOfRoll = timeInRoll / mScript.mRollTime;
        float currentSpeed = Mathf.Lerp(mScript.mRollSpeed, mScript.mRollEndSpeed, percentOfRoll);

        // Set Initial Velocity
        Rigidbody rb = mScript.mRigidBody;
        rb.linearVelocity = new Vector3(
            rollDirection.x * currentSpeed,
            rb.linearVelocity.y,
            rollDirection.z * currentSpeed
        );

        // Visualize Roll
        mScript.VisualizeRoll(percentOfRoll);
    }

    public override void exit()
    {
        mScript.VisualizeRoll(1f);
    }
}

public class sJumping : StateAuto<sJumping, PlayerController>
{
    bool jumpCut = false;
    public override void enter()
    {
        mScript.SetStateText("State: Jumping");

        // Set Initial Velocity
        Rigidbody rb = mScript.mRigidBody;
        rb.linearVelocity = new Vector3(
            rb.linearVelocity.x,
            mScript.mJumpForce,
            rb.linearVelocity.z
        );

        // Set Last Jump Time
        mScript.mLastJumpTime = Time.time;

        // Reset Jump Cut
        jumpCut = false;
    }

    public override void update()
    {
        
    }

    public override void fixedUpdate()
    {
        mScript.MovePlayer(0.5f);

        Rigidbody rb = mScript.mRigidBody;
        bool rising = rb.linearVelocity.y > 0;

        // Cut Jump Velocity If Jump Released
        if(!mScript.IsJumpDown() && !jumpCut && rising)
        {
            // Set Initial Velocity
            rb.linearVelocity = new Vector3(
                rb.linearVelocity.x,
                rb.linearVelocity.y/mScript.mJumpCutDivisor ,
                rb.linearVelocity.z
            );
            jumpCut = true;
        }    
    }

    public override void exit()
    {
    }
}

public class sDashing : StateAuto<sDashing, PlayerController>
{
    public override void enter()
    {
        mScript.SetStateText("State: Dashing");
    }

    public override void update()
    {

    }

    public override void exit()
    {
    }
}
#endregion



