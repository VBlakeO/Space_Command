using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using UnityEngine.Animations;

public class PlayerMovement : MonoBehaviour
{
    public static PlayerMovement m_Instance;

    public float DefaltFOV = 60f;

    #region Public variables
    [Header("===PlayerMovement===")]
    public bool cantMove = false;
    public bool cantLook = false;
    public bool cantJump = false;
    public float mouseSensitivity = 3f;
    public float CameraHeightRatio = 0.9f;

    [Header("===Crouching===")]
    public float PlayerHeightCrouching = 1.1f;
    public float PlayerHeightStanding = 2.0f;
    public float CrouchingSharpness = 10f;
    public bool IsCrouching;
    public UnityAction<bool> OnStanceChanged;

    [Header("===Speed===")]
    public float walkingSpeed = 10f;
    public float runningSpeed = 13f;
    public float speedReductionWhenJumping = 4f;
    public float speedReductionWhenCrouching = 6f;


    [Header("===Gravity===")]
    public float gravity = -28f;
    public float jumpForce = 10f;
    public float movementSmoothTime = 0.30f;

    [Header("===Components===")]
    public Camera cameraFps;
    public Transform head;
    public Transform middle = null;

    public float y_Recoil = 0.0f, x_Recoil = 0.0f;
    #endregion

    #region Private variables
    private bool isJumping;
    private bool groundCheck = false;

    public float headPitch = 0f;
    private float velocityY = 0f;
    public float currentSpeed = 0f;
    private float defaultWalkingSpeed = 0f;
    private float defaultRunningSpeed = 0f;
    private float m_TargetCharacterHeight = 0f;

    private Vector3 velocity = Vector3.zero;
    private Vector2 targetDir = Vector2.zero;
    private Vector2 currentDir = Vector2.zero;
    private Vector2 targetMouseDelta = Vector2.zero;
    private Vector2 currentDirVelocity = Vector2.zero;

    private bool crouchingTest;

    [HideInInspector]
    public CharacterController m_Controller;
    public Vector2 halfHeight = Vector2.zero;

    public Vector2 CurrentDir { get => currentDir; private set => currentDir = value; }
    public float DefaultWalkingSpeed { get => defaultWalkingSpeed;}
    public float DefaultRunningSpeed { get => defaultRunningSpeed;}
    #endregion

    private void Awake()
    {
        m_Instance = this;
        Initialize();
    }

    public void Initialize()
    {
        defaultWalkingSpeed = walkingSpeed;
        defaultRunningSpeed = runningSpeed;
        currentSpeed = walkingSpeed;

        m_Controller = GetComponent<CharacterController>();

        cameraFps.fieldOfView = DefaltFOV;

        SetCrouchingState(false, true);
        UpdateCharacterHeight(true);

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        if (Time.timeScale == 0)
            return;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (m_Controller.isGrounded && !cantJump)
                Jump();
        }
       
        if (!cantMove)
            SetCrouchingState(Input.GetKey(KeyCode.LeftControl), false);

        #region Camera
        if (y_Recoil <= 0)
            targetMouseDelta = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
        else
            targetMouseDelta = new Vector2(Input.GetAxis("Mouse X") + x_Recoil * 10, Input.GetAxis("Mouse Y") + y_Recoil * 10);
        #endregion

        UpdateCharacterHeight(false);

        targetDir = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        Movement();
    }

    private void FixedUpdate()
    {
        if (Time.timeScale == 0)
            return;

        Running();
    }

    private void LateUpdate()
    {
        if (Time.timeScale == 0)
            return;

        MouseLook();
    }

    void MouseLook()
    {
        if (cantLook)
            return;

        headPitch -= targetMouseDelta.y * mouseSensitivity;
        headPitch = Mathf.Clamp(headPitch, -85, 75);

        head.localEulerAngles = Vector3.right * headPitch;
        transform.Rotate((targetMouseDelta.x) * mouseSensitivity * Vector3.up);
    }


    void Movement()
    {
        if (cantMove)
            return;

        targetDir.Normalize();
        CurrentDir = Vector2.SmoothDamp(CurrentDir, targetDir, ref currentDirVelocity, movementSmoothTime);

        if (m_Controller.isGrounded && !isJumping)
            velocityY = 0;

        velocityY += gravity * Time.deltaTime;

        velocity = (transform.forward * CurrentDir.y + transform.right * CurrentDir.x) * currentSpeed + Vector3.up * velocityY;

        m_Controller.Move(velocity * Time.deltaTime);

        if ((Mathf.Abs(targetDir.x) > 0 || Mathf.Abs(targetDir.y) > 0) && OnSlope())
            m_Controller.Move(Vector3.down * m_Controller.height / 2 * 5 * Time.deltaTime);

       
        if (m_Controller.isGrounded)
        {
            if(!groundCheck)
            {
                SetSpeed();
                groundCheck = true;
            }
        }
        else
        {
            if (groundCheck)
            {
                groundCheck = false;
            }
        }

        if ((m_Controller.collisionFlags & CollisionFlags.Below) != 0)
        {

        }
    }

    private void Running()
    {
        if (Input.GetKey(KeyCode.LeftShift))
        {
            if (m_Controller.isGrounded)
            {
                if (IsCrouching)
                    currentSpeed = runningSpeed - speedReductionWhenCrouching * 2;
                else if (currentSpeed < runningSpeed)
                {
                    currentSpeed = runningSpeed;
                }
            }
            else
            {
                if (currentSpeed != runningSpeed - speedReductionWhenJumping)
                    currentSpeed = runningSpeed - speedReductionWhenJumping;
            }
        }
        else
        {
            if (m_Controller.isGrounded)
            {

                if (IsCrouching)
                    currentSpeed = walkingSpeed - speedReductionWhenCrouching;
                else if (currentSpeed > walkingSpeed)
                    currentSpeed = walkingSpeed;
            }
            else
            {
                if (currentSpeed != walkingSpeed - speedReductionWhenJumping)
                    currentSpeed = walkingSpeed - speedReductionWhenJumping;
            }
        }
    }

    public void SetSpeed()
    {
        walkingSpeed = DefaultWalkingSpeed;
        runningSpeed = DefaultRunningSpeed;
        
        currentSpeed = walkingSpeed;
    }
  
    public void PausePlayer(bool active)
    {
        cantLook = active;
        cantMove = active;
        CurrentDir = Vector2.zero;
        GetComponentInChildren<LookAtConstraint>().enabled = active;

        if (active)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
        else
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }

    }

    public void DontJump()
    {
        cantJump = true;
    }

    private bool OnSlope()
    {
        if (isJumping)
            return false;

        RaycastHit hit;

        if (Physics.Raycast(transform.position, Vector3.down, out hit, m_Controller.height / 2 * 2))
            if (hit.normal != Vector3.up)
                return true;

        return false;
    }
    
    private void Jump()
    {
        isJumping = true;
        velocityY = jumpForce;

        StartCoroutine(BackToGround());
    }

    void UpdateCharacterHeight(bool force)
    {
        // Update height instantly
        if (force)
        {
            m_Controller.height = m_TargetCharacterHeight;
            m_Controller.center = 0.5f * m_Controller.height * Vector3.up;
            head.transform.localPosition = CameraHeightRatio * m_TargetCharacterHeight * Vector3.up;
        }
        // Update smooth height
        else if (m_Controller.height != m_TargetCharacterHeight)
        {
            // resize the capsule and adjust camera position
            m_Controller.height = Mathf.Lerp(m_Controller.height, m_TargetCharacterHeight,
                CrouchingSharpness * Time.deltaTime);
            m_Controller.center = 0.5f * m_Controller.height * Vector3.up;
            head.transform.localPosition = Vector3.Lerp(head.transform.localPosition,
                CameraHeightRatio * m_TargetCharacterHeight * Vector3.up, CrouchingSharpness * Time.deltaTime);
        }
    }

    bool SetCrouchingState(bool crouched, bool ignoreObstructions)
    {
        // set appropriate heights
        if (crouched)
        {
            m_TargetCharacterHeight = PlayerHeightCrouching;
            middle.transform.localPosition = new Vector3(0, halfHeight.x, 0);
        }
        else
        {
            // Detect obstructions
            if (!ignoreObstructions)
            {
                Collider[] standingOverlaps = Physics.OverlapCapsule(
                    GetCapsuleBottomHemisphere(),
                    GetCapsuleTopHemisphere(PlayerHeightStanding),
                    m_Controller.radius,
                    -1,
                    QueryTriggerInteraction.Ignore);
                foreach (Collider c in standingOverlaps)
                {
                    if (c != m_Controller)
                    {
                        return false;
                    }
                }
            }

            m_TargetCharacterHeight = PlayerHeightStanding;
            middle.transform.localPosition = new Vector3(0, halfHeight.y, 0);
        }


        OnStanceChanged?.Invoke(crouched);

        IsCrouching = crouched;

        if (crouchingTest != crouched)
        {
            SetSpeed();
            crouchingTest = crouched;
        }

        return true;
    }

    Vector3 GetCapsuleBottomHemisphere()
    {
        return transform.position + (transform.up * m_Controller.radius);
    }

    Vector3 GetCapsuleTopHemisphere(float atHeight)
    {
        return transform.position + (transform.up * (atHeight - m_Controller.radius));
    }

    private IEnumerator BackToGround()
    {
        WaitForSeconds wfs = new(0.1f);

        yield return wfs;
        isJumping = false;
    }
}