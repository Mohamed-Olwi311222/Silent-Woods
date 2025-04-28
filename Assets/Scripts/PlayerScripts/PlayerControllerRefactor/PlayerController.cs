using System;
using System.Collections;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    PlayerStateMachine playerStateMachine;
    [SerializeField] CinemachineCamera firstPersonCamera;
    private Rigidbody myRigidbody;
    public Vector2 moveInput;
    [SerializeField] LayerMask ground;
    [SerializeField] private float groundcheckRaycastLength = 1.15f;
#region PlayerStaminaNeededProperties
    public readonly float maximumStamina = 450;
    public float currentStamina;
    public readonly float staminaRegen = 5f;
    public readonly float staminaDrain = 50f;
    public bool sprintLocked = false;
    private bool staminaCorutineStarted = false;
#endregion

#region PlayerCrouchingNeededProperties
    private CapsuleCollider myCapsuleCollider;
    private float myCapsuleColliderHeight;
    private float myCapsuleColliderCrouchHeight;
    private readonly float myCapsuleColliderCrouchOffset = -0.47f;
#endregion

#region PlayerLookNeededProperties
    [SerializeField] public Transform cameraPosition;
    [SerializeField] float topClamp = 90.0f;
    [SerializeField] float bottomClamp = -90.0f;
    float sensitivity = 50f;
    private const float _threshold = 0.01f;
    float yMovement;
    float xMovement;
    Vector2 mouseInput;
#endregion

#region PlayerBooleanNeededProperties
    public bool isMoving;
    public bool isSprinting;
    public bool isCrouching;
    public bool isGrounded;
#endregion

#region PlayerFXNeededProperties
    [Header("PlayerSound")]
    [SerializeField] AudioClip walkingFX;
    public enum SoundsRanges
    {
        Walking = 25,
        Crouching = 15,
        Sprinting = 35
    }
#endregion
    private void Awake()
    {
        playerStateMachine = new();
        myRigidbody = GetComponent<Rigidbody>();
        currentStamina = maximumStamina;
        //states
        PlayerIdleState playerIdleState = new(firstPersonCamera, transform);
        PlayerWalkState playerWalkState = new(firstPersonCamera, myRigidbody, transform, walkingFX);
        PlayerSprintState playerSprintState = new(firstPersonCamera, myRigidbody, transform, walkingFX);

#region Transitions
        //from idle to X
        AddTransition(playerIdleState, playerWalkState, PlayerIsWalking());
        AddTransition(playerIdleState, playerSprintState, PlayerIsSprinting());

        //from walk to X
        AddTransition(playerWalkState, playerIdleState, PlayerIsIdling());
        AddTransition(playerWalkState, playerSprintState, PlayerIsSprinting());

        //from sprint to X
        AddTransition(playerSprintState, playerIdleState, PlayerIsIdling());
        AddTransition(playerSprintState, playerWalkState, PlayerIsWalking());


#endregion /*  Transitions */

        myCapsuleCollider = transform.GetComponent<CapsuleCollider>();
        myCapsuleColliderHeight = myCapsuleCollider.height;
        myCapsuleColliderCrouchHeight = myCapsuleColliderHeight / 2;

        playerStateMachine.SetState(playerIdleState);

    }
#region StateConditions
    private void AddTransition(IPlayerState from, IPlayerState to, Func<bool> condition) =>
                                                                    playerStateMachine.AddTransition(from, to, condition);
    Func<bool> PlayerIsIdling()
    {
        return () => isGrounded && !isMoving;
    }
    Func<bool> PlayerIsWalking()
    {
        return () => isGrounded && isMoving && (!isSprinting | sprintLocked);
    }
    Func<bool> PlayerIsSprinting()
    {
        return () =>    isGrounded &&
                        isSprinting &&
                        moveInput.y > 0 &&
                        !sprintLocked;
    }
#endregion /* StateConditions */

    void Update()
    {
        Checkgrounded();
        playerStateMachine.FrameUpdate();
    }

    void FixedUpdate()
    {
        RegenStamina();
        playerStateMachine.FixedFrameUpdate();
    }

    #region MoveFunctions
    private void Checkgrounded()
    {
        isGrounded = Physics.Raycast(myCapsuleCollider.transform.position, Vector3.down, groundcheckRaycastLength, ground);
        Debug.DrawRay(myCapsuleCollider.transform.position, Vector3.down * groundcheckRaycastLength, Color.red);
    }
    public void OnMovement(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            moveInput = context.ReadValue<Vector2>();
            isMoving = true;
        }
        else
        {
            isMoving = false;
        }
    }
#endregion /* MoveFunctions */

#region SprintFunctions
    private void RegenStamina()
    {
        if (currentStamina <= maximumStamina - 0.01 && !staminaCorutineStarted && !isSprinting)
        {
            StartCoroutine(HandleStaminaRegin());
        }
    }

    private IEnumerator HandleStaminaRegin()
    {
        staminaCorutineStarted = true;
        LockSprinting(); //lock sprinting if staming < 40%
        yield return new WaitForSeconds(3f);
        while (!isSprinting && currentStamina < maximumStamina)
        {
            currentStamina += staminaRegen * Time.deltaTime;
            yield return null;
            UnlockSprinting(); //unlock sprinting if staming >= 40%
        }
        staminaCorutineStarted = false;
    }
 
    private void UnlockSprinting()
    {
        if (currentStamina >= maximumStamina * 0.4)
        {
            sprintLocked = false;
        }
    }
 
    private void LockSprinting()
    {
        if (currentStamina < maximumStamina * 0.4)
        {
            sprintLocked = true;
        }
    }
 
    public void Sprint(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Stand();
            isSprinting = true;
        }
        else
        {
            isSprinting = false;
        }
    }
#endregion /* SprintFunctions */

#region CrouchFunctions
    public void Crouch(InputAction.CallbackContext context)
    {
        if (context.performed && !isSprinting && !isCrouching)
        {
            isCrouching = true;
            if (!isMoving)
            {
                Crouch();
            }
        }
        else if (context.performed && isCrouching)
        {
            Stand();
        }
    }
    public void Crouch()
    {
        myCapsuleCollider.height = myCapsuleColliderCrouchHeight;
        myCapsuleCollider.center = new Vector3(0, myCapsuleColliderCrouchOffset, 0);
    }
    public void Stand()
    {
        isCrouching = false;
        myCapsuleCollider.height = myCapsuleColliderHeight;
        myCapsuleCollider.center = new Vector3(0, 0, 0);
    }
#endregion /* CrouchFunctions */

#region UseItemFunctions
    public void UseItem(InputAction.CallbackContext context)//TODO: implement
    {

    }
#endregion /* UseItemFunctions */

#region MouseLookFunctions
    private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
    {
        if (lfAngle < -360f) lfAngle += 360f;
        if (lfAngle > 360f) lfAngle -= 360f;
        return Mathf.Clamp(lfAngle, lfMin, lfMax);
    }
    public void Look(InputAction.CallbackContext context)
    {
        mouseInput = context.ReadValue<Vector2>().normalized;
        if (mouseInput.sqrMagnitude >= _threshold)
        {
            xMovement += -mouseInput.y * sensitivity * Time.deltaTime;//the new input system proccesses continuous input in the update loop
            yMovement += mouseInput.x * sensitivity * Time.deltaTime;
            xMovement = ClampAngle(xMovement, bottomClamp, topClamp);
            cameraPosition.localRotation = Quaternion.Euler(xMovement, yMovement, 0f);
            transform.localRotation = Quaternion.Euler(0, yMovement, 0);//responsible for moving relative to local axes
        }
    }
#endregion /* MouseLookFunctions */
}
