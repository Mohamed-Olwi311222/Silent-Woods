using System.Collections;
using Unity.Cinemachine;
using Unity.Mathematics;
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
    private readonly int maxPlayerStamina = 300;
    private float currentPlayerStamina;





    [Header("Movement")]
    Vector2 moveInput;
    Vector3 forceVector;
    float velocity;
    float walkSpeed = 200;
    float sprintSpeed = 300;
    float crouchSpeed = 150;
    private bool isSprinting = false;
    public bool isCrouching = false;
    private bool isMoving = false;
    private bool isIdle = true;
    private bool sprintButtonIsPressed = false;
    private float dragFactor = 25f;
    private float customDrag;
    private bool enableSprinting = true;
    private bool disableSprintingCoroutineIsRunning = false;




    [Header("PlayerSound")]
    [SerializeField] AudioClip walkingFX;
    [SerializeField] AudioClip heavyBreathing;
    [SerializeField] AudioClip droppingFX;
    [SerializeField] AudioClip flashlightOff;
    [SerializeField] AudioClip flashlightOn;
    bool handleDropSoundFXCoroutineIsRunning = false;
    readonly float defaultMoveVolume = 0.8f;
    readonly float crouchMoveVolume = 0.5f;
    readonly float sprintMoveVolume = 1f;
    float currentMoveVolume;
    private float playerSoundRange;
    private float stepTimer = 0f;
    private float stepDelay = 0.5f; // Delay between footstep sounds
    enum SoundsRanges
    {
        Sprinting = 35,
        Crouching = 15,
        Walking = 25
    }



    [Header("Jumping")]
    [SerializeField] LayerMask ground;
    bool isGrounded;
    Vector3 minJumpForce = Vector3.up * 24;
    Vector3 maxjumpForce = Vector3.up * 60;
    private readonly float coyoteTime = 0.3f;
    private float coyoteTimeCounter;
    private readonly float jumpBufferTime = 0.3f;
    private float jumpBufferTimeCounter;
    Vector3 minSnapToGround = Vector3.down * 0;
    Vector3 maxSnapToGround = Vector3.down * 65;
    readonly float freeFallTime = 100;
    readonly float jumpTime = 80;
    Vector3 jumpAcceleration;
    bool jumpButtonWasPressed = false;
    bool canPerformBufferedJump = false;
    bool handleJumpBufferCoroutineIsRunning = false;








    [Header("Game Objects")]
    [SerializeField] Light flashLight;
    private bool flashLightState = false;
    private float flashLightIntensity;








    [Header("Camera Movement")]
    [SerializeField] CinemachineCamera firstPersonCamera;
    [SerializeField] Transform cameraPosition;
    [SerializeField] float topClamp = 90.0f;
    [SerializeField] float bottomClamp = -90.0f;
    private CinemachineBasicMultiChannelPerlin noise;
    float sensitivity = 50f;//TODO:bind the sensitivity in the binding menu UI
    private const float _threshold = 0.01f;
    float yMovement;
    float xMovement;
    Vector2 mouseInput;
    [SerializeField] InventoryManager inventoryManager;
    bool isLookingBack = false;
    Quaternion currentRotation;
    Quaternion lookingBackAngle;



    void Awake()
    {
        noise = firstPersonCamera.GetComponent<CinemachineBasicMultiChannelPerlin>();
        myRigidbody = GetComponent<Rigidbody>();
        mySphereCollider = sphere.GetComponent<SphereCollider>();
        myCapsuleCollider = GetComponent<CapsuleCollider>();
        myCapsuleColliderHeight = myCapsuleCollider.height;
        myCapsuleColliderCrouchHeight = myCapsuleColliderHeight / 2;
        flashLightIntensity = flashLight.intensity;
        flashLight.intensity = 0;
        currentPlayerStamina = maxPlayerStamina;
        jumpAcceleration = Vector3.Lerp(minJumpForce, maxjumpForce, Time.deltaTime * jumpTime);
    }


    void Update()
    {
        HandleStamina();
        Checkgrounded();
        ReleaseSprintButton();
        DecrementJumpBufferCounter();
        isIdle = !isMoving;
    }

    void FixedUpdate()
    {
        Move();
        FallDown();
        ShakeCamera();
        HandleSoundFX();
    }

    private void HandleSoundFX()
    {
        if (isMoving && isGrounded)
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
        if (!isGrounded && !handleDropSoundFXCoroutineIsRunning) { StartCoroutine(HandleDropSoundFX()); }
    }

    IEnumerator HandleDropSoundFX()
    {
        handleDropSoundFXCoroutineIsRunning = true;
        yield return new WaitUntil(() => isGrounded);
        playerSoundRange = 30f;
        AudioManager.instance.PlaySoundFXClip(droppingFX, transform, 1f, playerSoundRange, Sound.SoundType.Dangerous, true);
        handleDropSoundFXCoroutineIsRunning = false;
    }

    private void Checkgrounded()
    {
        isGrounded = Physics.CheckSphere(sphere.transform.position, mySphereCollider.radius, ground, QueryTriggerInteraction.Ignore);
        if (isGrounded) { coyoteTimeCounter = coyoteTime; }
        else { coyoteTimeCounter -= Time.deltaTime; }
    }

    private void FallDown()
    {
        if (!isGrounded)
        {
            Vector3 terminalVelocity = Vector3.Lerp(minSnapToGround, maxSnapToGround, Time.fixedDeltaTime * freeFallTime);
            myRigidbody.AddRelativeForce(terminalVelocity);
            myCapsuleCollider.height = myCapsuleColliderHeight;
            myCapsuleCollider.center = new Vector3(0, 0, 0);
            isCrouching = false;
        }
    }
    private void ReleaseSprintButton()
    {/*checks if the sprint button is being held and whether or not the player is actually sprinting*/
        if (!isMoving)
        {
            isSprinting = false;
        }
        else
        {
            if (currentPlayerStamina <= 0)
            {
                isSprinting = false;
                if (!disableSprintingCoroutineIsRunning) { StartCoroutine(nameof(DisableSprinting)); }
            }
            if (sprintButtonIsPressed && (moveInput.y > 0 || moveInput.x > 0) && !isCrouching)
            {
                if (!disableSprintingCoroutineIsRunning) { isSprinting = true; }
            }
        }
    }

    public void Look(InputAction.CallbackContext context)
    {
        if (true == inventoryManager.inventoryState  || true == isLookingBack)
        {
            return;
        }
        mouseInput = context.ReadValue<Vector2>().normalized;
        if (mouseInput.sqrMagnitude >= _threshold)
        {
            xMovement += -mouseInput.y * sensitivity * Time.deltaTime;//the new input system proccesses continuous input in the update loop
            yMovement += mouseInput.x * sensitivity * Time.deltaTime;
            xMovement = ClampAngle(xMovement, bottomClamp, topClamp);
            if (!isLookingBack)
            {
                cameraPosition.localRotation = Quaternion.Euler(xMovement, yMovement, 0);
            }
            transform.localRotation = Quaternion.Euler(0, yMovement, 0);//responsible for moving relative to local axes
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
        if (isSprinting && isGrounded && currentPlayerStamina > 0 && !isCrouching) { velocity = sprintSpeed; }
        else if (isCrouching) { velocity = crouchSpeed; }
        else { velocity = walkSpeed; }
        customDrag = 1 - (dragFactor * Time.fixedDeltaTime);
        forceVector = new Vector3(moveInput.x * velocity * customDrag, 0, moveInput.y * velocity * customDrag);
        myRigidbody.AddRelativeForce(forceVector, ForceMode.Force);//moves the player relative to his local axes
    }
    public void Sprint(InputAction.CallbackContext context)
    {
        sprintButtonIsPressed = true;
        if(sprintButtonIsPressed){isCrouching=false;}
        if (enableSprinting && !isCrouching)
        {
            if (context.performed && currentPlayerStamina > 0 && (moveInput.y > 0 || moveInput.x > 0) && isGrounded) { isSprinting = true; }
            else { isSprinting = false; }
            if (context.canceled)
            {
                sprintButtonIsPressed = false;
                isSprinting = false;
            }
        }
    }

    public void Jump(InputAction.CallbackContext context)
    {
        if (context.performed && !isCrouching)
        {
            jumpButtonWasPressed = true;
            if (coyoteTimeCounter > 0 && !canPerformBufferedJump)
            {
                myRigidbody.AddRelativeForce(jumpAcceleration, ForceMode.Impulse);
            }
            else if (!isGrounded && jumpButtonWasPressed && !canPerformBufferedJump)
            {
                canPerformBufferedJump = true;
                jumpBufferTimeCounter = jumpBufferTime;
                if (!handleJumpBufferCoroutineIsRunning) { StartCoroutine(nameof(HandleJumpBuffer)); }
            }
        }
        if (context.canceled)
        {
            coyoteTimeCounter = 0;
            jumpButtonWasPressed = false;
        }
    }

    void DecrementJumpBufferCounter()
    {
        if (!isGrounded) { jumpBufferTimeCounter -= Time.deltaTime; }
    }
    private IEnumerator HandleJumpBuffer()
    {
        handleJumpBufferCoroutineIsRunning = true;
        yield return new WaitUntil(() => isGrounded);
        if (canPerformBufferedJump && jumpBufferTimeCounter > 0)
        {
            Vector3 negativeForce = new Vector3(myRigidbody.linearVelocity.x, 3.5f * Mathf.Abs(myRigidbody.linearVelocity.y), myRigidbody.linearVelocity.z);
            myRigidbody.AddRelativeForce(jumpAcceleration + negativeForce, ForceMode.Impulse);
            coyoteTimeCounter = 0;
        }
        Invoke(nameof(ToggleCanPerformBufferedJump), 0.3f);
        handleJumpBufferCoroutineIsRunning = false;
    }

    private void ToggleCanPerformBufferedJump()
    {
        canPerformBufferedJump = false;
    }

    public void Crouch(InputAction.CallbackContext context)
    {
        if (context.performed && !isSprinting && isGrounded)
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

    private void HandleStamina()
    {
        if (isSprinting && currentPlayerStamina > 0) { DeductStamina(); }
        else { Invoke(nameof(ReplinishStamina), 3f); }//TODO: stamina stops replinishing mid execution
    }

    void DeductStamina()
    {
        if ((moveInput.x > 0f || moveInput.y > 0f) && isSprinting && !isCrouching) { currentPlayerStamina -= 30f * Time.deltaTime; }
    }
    void ReplinishStamina()
    {
        if (currentPlayerStamina < maxPlayerStamina && !isSprinting) { currentPlayerStamina += 30f * Time.deltaTime; }
    }
    private IEnumerator DisableSprinting()
    {
        disableSprintingCoroutineIsRunning = true;
        AudioManager.instance.PlaySoundFXClip(heavyBreathing, transform, 1f, 0f, Sound.SoundType.Default, false);
        while (currentPlayerStamina < maxPlayerStamina * 0.4) { yield return enableSprinting = false; }
        enableSprinting = true;
        disableSprintingCoroutineIsRunning = false;
    }

    private void ShakeCamera()
    {
        if (isIdle || isCrouching)
        {
            noise.AmplitudeGain = 0.7f;
            noise.FrequencyGain = 0.33f;
        }
        else
        {
            if (isSprinting)
            {
                noise.AmplitudeGain = 1.3f;
                noise.FrequencyGain = 0.7f;
            }
            else if (isMoving)
            {
                noise.AmplitudeGain = 1f;
                noise.FrequencyGain = 0.6f;
            }
        }
    }

    public void ToggleFlash(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            flashLightState = !flashLightState;
            if (flashLightState)
            {
                AudioManager.instance.PlaySoundFXClip(flashlightOn, transform, 1f, 0f, Sound.SoundType.Default, false);
                flashLight.intensity = flashLightIntensity;
            }
            else
            {
                AudioManager.instance.PlaySoundFXClip(flashlightOff, transform, 1f, 0f, Sound.SoundType.Default, false);
                flashLight.intensity = 0;
            }
        }
    }

    public void UseItem(InputAction.CallbackContext context)//TODO: implement
    {

    }


    public void LookBackLeft(InputAction.CallbackContext context)
    {
        if (context.duration < 0.6f) { return; }
        if (context.performed && isSprinting && !isLookingBack)
        {
            isLookingBack = true;
            currentRotation = Quaternion.Euler(cameraPosition.transform.eulerAngles.x, cameraPosition.transform.eulerAngles.y, cameraPosition.transform.eulerAngles.z);
            lookingBackAngle = Quaternion.Euler(cameraPosition.transform.eulerAngles.x, cameraPosition.transform.eulerAngles.y - 135, cameraPosition.transform.eulerAngles.z);
            StartCoroutine(PerformLookBack(currentRotation, lookingBackAngle));
        }
        if (context.canceled || isLookingBack && !isSprinting)
        {
            lookingBackAngle = Quaternion.Euler(cameraPosition.transform.eulerAngles.x, cameraPosition.transform.eulerAngles.y, cameraPosition.transform.eulerAngles.z);
            currentRotation = Quaternion.Euler(xMovement, yMovement, 0);
            StartCoroutine(PerformLookBack(lookingBackAngle, currentRotation));
            Invoke("SetIsLookingBackToFalse", 0.1f);//fixes the visual artifacts
            //time must be slightly more than the duration in the perform coroutine
        }
    }

    public void LookBackRight(InputAction.CallbackContext context)
    {
        if (context.duration < 0.6f) { return; }
        if (context.performed && isSprinting && !isLookingBack)
        {
            isLookingBack = true;
            currentRotation = Quaternion.Euler(cameraPosition.transform.eulerAngles.x, cameraPosition.transform.eulerAngles.y, cameraPosition.transform.eulerAngles.z);
            lookingBackAngle = Quaternion.Euler(cameraPosition.transform.eulerAngles.x, cameraPosition.transform.eulerAngles.y + 135, cameraPosition.transform.eulerAngles.z);
            StartCoroutine(PerformLookBack(currentRotation, lookingBackAngle));
        }
        if (context.canceled || isLookingBack && !isSprinting)
        {
            lookingBackAngle = Quaternion.Euler(cameraPosition.transform.eulerAngles.x, cameraPosition.transform.eulerAngles.y, cameraPosition.transform.eulerAngles.z);
            currentRotation = Quaternion.Euler(xMovement, yMovement, 0);
            StartCoroutine(PerformLookBack(lookingBackAngle, currentRotation));
            Invoke("SetIsLookingBackToFalse", 0.1f);//fixes the visual artifacts
            //time must be slightly more than the duration in the perform coroutine
        }
    }

    IEnumerator PerformLookBack(quaternion fromRotation, quaternion toRotation)
    {

        float duration = 0.1f; // Time for the rotation
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            cameraPosition.transform.rotation = Quaternion.Slerp(fromRotation, toRotation, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }

    private void SetIsLookingBackToFalse()
    {
        isLookingBack = false;
    }
}
