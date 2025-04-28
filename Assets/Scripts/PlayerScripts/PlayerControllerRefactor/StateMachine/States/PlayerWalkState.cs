using Unity.Cinemachine;
using UnityEngine;

public class PlayerWalkState : IPlayerState
{
    #region PlayerCameraWhileMovingNeededProperties
    private CinemachineBasicMultiChannelPerlin noise;
    private readonly CinemachineCamera firstPersonCamera;
    #endregion
    #region PlayerMovingNeededProperties
    private readonly Transform playerTransform; //for getting the moveinput
    private readonly PlayerController playerController; //for getting the moveinput
    private readonly Rigidbody myRigidBody;
    private readonly float dragFactor = 25f;
    private readonly float walkingVelocity = 200;
    private float customDrag;
    private Vector3 forceVector;
    #endregion


    #region PlayerMovingFX
    private readonly AudioClip walkingFX;
    private readonly float walkingFXVolume = 0.8f;
    private float playerSoundRange;
    private float stepTimer = 0f;
    private float stepDelay;
    private readonly float walkingStepDelay = 0.5f;
    private readonly float crouchingStepDelay = 0.7f;
    #endregion

    public PlayerWalkState(CinemachineCamera firstPersonCamera, Rigidbody myRigidBody
                                    , Transform playerTransform, AudioClip walkingFX)
    {
        this.playerTransform = playerTransform;
        this.myRigidBody = myRigidBody;
        this.firstPersonCamera = firstPersonCamera;
        this.walkingFX = walkingFX;
        playerController = playerTransform.GetComponent<PlayerController>();
        noise = firstPersonCamera.GetComponent<CinemachineBasicMultiChannelPerlin>();
        noise = firstPersonCamera.GetComponent<CinemachineBasicMultiChannelPerlin>();

        stepDelay = walkingStepDelay;
        playerSoundRange = (float) PlayerController.SoundsRanges.Walking;
    }
     public void OnEnterState()
    {
        Debug.Log("walk");
        noise.AmplitudeGain = 1f;
        noise.FrequencyGain = 0.6f;
    }
    private void HandleWalkingFX()
    {
        stepTimer += Time.fixedDeltaTime;
        if (playerController.isCrouching)
        {
            playerSoundRange = (float) PlayerController.SoundsRanges.Crouching;
            stepDelay = crouchingStepDelay;
        }
        else
        {
            playerSoundRange = (float) PlayerController.SoundsRanges.Walking;
            stepDelay = walkingStepDelay;
        }
        if (stepTimer >= stepDelay)
        {
            AudioManager.instance.PlaySoundFXClip(walkingFX, playerTransform, 1f * walkingFXVolume, playerSoundRange, Sound.SoundType.Dangerous, true);
            stepTimer = 0f;
        }
    }

    public void PhysicsUpdate()
    {
        HandleCrouch();
        customDrag = 1 - (dragFactor * Time.fixedDeltaTime);
        forceVector = new Vector3(playerController.moveInput.x * walkingVelocity * customDrag, 0, playerController.moveInput.y * walkingVelocity * customDrag);
        myRigidBody.AddRelativeForce(forceVector, ForceMode.Force);//moves the player relative to his local axes
        if (playerController.isGrounded) //only play the fx if the player is grounded
        {
            HandleWalkingFX();
        }
    }
    private void HandleCrouch()
    {
        if (playerController.isCrouching)
        {
            playerController.Crouch();
        }
        else
        {
            playerController.Stand();
        }
    }
   
    public void FrameUpdate()
    {

    }
    public void OnExitState()
    {
        if (!playerController.isCrouching)
        {
            playerController.Stand();
        }
    }

}
