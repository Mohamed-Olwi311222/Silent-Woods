using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerController : MonoBehaviour
{

    [Header("Player")]
    private Rigidbody myRigidbody;
    private SphereCollider mySphereCollider;
    private CapsuleCollider myCapsuleCollider;
    [SerializeField] Transform sphere;
    private float myCapsuleColliderHeight;
    private float myCapsuleColliderCrouchHeight;
    private float myCapsuleColliderCrouchOffset = -0.47f;


    [Header("Movement")]
    Vector2 moveInput;
    Vector3 forceVector;
    float velocity;
    float walkSpeed = 2500;
    float sprintSpeed = 4000;
    float crouchSpeed = 1500;
    private bool isGrounded = true;
    private bool isSprinting = false;
    public bool isCrouching = false;
    private bool isMoving = false;
    private bool isIdle = true;
    private float dragFactor = 25f;
    private float customDrag;
    private bool sprintButtonIsPressed = false;
    public LayerMask ground;




    [Header("PlayerSound")]
    [SerializeField] AudioClip walkingFX;
    readonly float defaultMoveVolume = 0.8f;
    readonly float crouchMoveVolume = 0.5f;
    readonly float sprintMoveVolume = 1f;
    float currentMoveVolume;
    private float playerSoundRange;
    private float stepTimer = 0f;
    private float stepDelay = 0.5f; 
    enum SoundsRanges
    {
        Sprinting = 35,
        Crouching = 15,
        Walking = 25
    }



    [Header("Camera Movement")]
    //[SerializeField] C firstPersonCamera;
    [SerializeField] Transform cameraPosition;
    [SerializeField] float topClamp = 90.0f;
    [SerializeField] float bottomClamp = -90.0f;

    float sensitivity = 50f;
    private const float _threshold = 0.01f;
    float yMovement;
    float xMovement;
    Vector2 mouseInput;



    void Awake()
    {
        myRigidbody = GetComponent<Rigidbody>();
        mySphereCollider = sphere.GetComponent<SphereCollider>();
        myCapsuleCollider = GetComponent<CapsuleCollider>();
        myCapsuleColliderHeight = myCapsuleCollider.height;
        myCapsuleColliderCrouchHeight = myCapsuleColliderHeight / 2;
    }


    void FixedUpdate()
    {
        Move();
        HandleSoundFX();
    }

    private void HandleSoundFX()
    {
        if (isMoving)
        {
            currentMoveVolume = defaultMoveVolume;
            if (isSprinting)
            {
                playerSoundRange = (float)SoundsRanges.Sprinting;
                stepDelay = 0.3f;
                currentMoveVolume = sprintMoveVolume;
            }
            else if (isCrouching)
            {
                playerSoundRange = (float)SoundsRanges.Crouching;
                stepDelay = 0.7f;
                currentMoveVolume = crouchMoveVolume;
            }
            else
            {
                stepDelay = 0.5f;
                playerSoundRange = (float)SoundsRanges.Walking;
            }
            stepTimer += Time.fixedDeltaTime;
            if (stepTimer >= stepDelay)
            {
                AudioManager.instance.PlaySoundFXClip(walkingFX, transform, 1f * currentMoveVolume, playerSoundRange, Sound.SoundType.Dangerous, true);
                stepTimer = 0f;
            }
        }
    }


    public void Look(InputAction.CallbackContext context)
    {
        mouseInput = context.ReadValue<Vector2>().normalized;
        if (mouseInput.sqrMagnitude >= _threshold)
        {
            xMovement += -mouseInput.y * sensitivity * Time.deltaTime;
            yMovement += mouseInput.x * sensitivity * Time.deltaTime;
            xMovement = ClampAngle(xMovement, bottomClamp, topClamp);
            cameraPosition.localRotation = Quaternion.Euler(xMovement, yMovement, 0);
            transform.localRotation = Quaternion.Euler(0, yMovement, 0);
        }
    }

    private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
    {
        if (lfAngle < -360f) lfAngle += 360f;
        if (lfAngle > 360f) lfAngle -= 360f;
        return Mathf.Clamp(lfAngle, lfMin, lfMax);
    }


    public void OnMovement(InputAction.CallbackContext context)
    {
        isMoving = true;
        moveInput = context.ReadValue<Vector2>();
        if (context.canceled) { isMoving = false; }
    }

    private void Move()
    {
        if (isSprinting && isGrounded && !isCrouching) { velocity = sprintSpeed; }
        else if (isCrouching) { velocity = crouchSpeed; }
        else { velocity = walkSpeed; }
        customDrag = 1 - (dragFactor * Time.fixedDeltaTime);
        forceVector = new Vector3(moveInput.x * velocity * customDrag, 0, moveInput.y * velocity * customDrag);
        myRigidbody.AddRelativeForce(forceVector, ForceMode.Force);//moves the player relative to his local axes
    }
    public void Sprint(InputAction.CallbackContext context)
    {
        sprintButtonIsPressed = true;
        if (sprintButtonIsPressed) { isCrouching = false; }
        if (!isCrouching)
        {
            if (context.performed  && (moveInput.y > 0 || moveInput.x > 0) && isGrounded) { isSprinting = true; }
            else { isSprinting = false; }
            if (context.canceled)
            {
                sprintButtonIsPressed = false;
                isSprinting = false;
            }
        }
    }
    public void Crouch(InputAction.CallbackContext context)
    {
        if (context.performed && !isSprinting)
        {
            isCrouching = !isCrouching;
            if (isCrouching)
            {
                myCapsuleCollider.height = myCapsuleColliderCrouchHeight;
                myCapsuleCollider.center = new Vector3(0, myCapsuleColliderCrouchOffset, 0);
            }
            if (!isCrouching)
            {
                myCapsuleCollider.height = myCapsuleColliderHeight;
                myCapsuleCollider.center = new Vector3(0, 0, 0);
            }
        }
    }
}
