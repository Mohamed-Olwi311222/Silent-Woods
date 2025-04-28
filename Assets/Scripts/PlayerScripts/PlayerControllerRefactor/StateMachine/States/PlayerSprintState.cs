using Unity.Cinemachine;
using UnityEngine;

public class PlayerSprintState : IPlayerState
{
    #region PlayerCameraWhileSprintingNeededProperties
    private CinemachineBasicMultiChannelPerlin noise;
    private readonly CinemachineCamera firstPersonCamera;
    #endregion
    #region PlayerSprintingNeededProperties
    private readonly Transform playerTransform; //for getting the moveinput
    private readonly PlayerController playerController; //for getting the moveinput

    private readonly Rigidbody myRigidBody;
    private readonly float dragFactor = 25f;
    private readonly float sprintVelocity = 300;
    private float customDrag;
    private Vector3 forceVector;

    #endregion
    #region PlayerMovingFX
    private readonly AudioClip sprintingFX;
    private readonly float sprintingFXVolume = 1f;
    private readonly float playerSoundRange = 35f;
    private float stepTimer = 0f;
    private readonly float stepDelay = 0.3f; // Delay between footstep sounds
    #endregion
     public PlayerSprintState(CinemachineCamera firstPersonCamera, Rigidbody myRigidBody
                                    , Transform playerTransform, AudioClip sprintingFX)
    {
        this.playerTransform = playerTransform;
        this.myRigidBody = myRigidBody;
        this.firstPersonCamera = firstPersonCamera;
        this.sprintingFX = sprintingFX;
        playerController = playerTransform.GetComponent<PlayerController>();
        noise = firstPersonCamera.GetComponent<CinemachineBasicMultiChannelPerlin>();
    }
    public void FrameUpdate()
    {
        HandleStaminaDrain();
    }
    private void HandleStaminaDrain()
    {
        if (playerController.currentStamina > 0)
        {
            playerController.currentStamina -= playerController.staminaDrain * Time.deltaTime;
        }
        else 
        {
            playerController.isSprinting = false;
        }
    }
    public void OnEnterState()
    {
        Debug.Log("sprint");
        noise.AmplitudeGain = 1.3f;
        noise.FrequencyGain = 0.7f;
    }

    public void OnExitState()
    {
    }

    public void PhysicsUpdate()
    {
        customDrag = 1 - (dragFactor * Time.fixedDeltaTime);
        forceVector = new Vector3(playerController.moveInput.x * sprintVelocity * customDrag, 0, playerController.moveInput.y * sprintVelocity * customDrag);
        myRigidBody.AddRelativeForce(forceVector, ForceMode.Force);//moves the player relative to his local axes
        if (playerController.isGrounded) //only play the fx if the player is grounded
        {
            HandleSprintingFX();
        }
    }

   
    private void HandleSprintingFX()
    {
        stepTimer += Time.fixedDeltaTime;
        if (stepTimer >= stepDelay)
        {
            AudioManager.instance.PlaySoundFXClip(sprintingFX, playerTransform, 1f * sprintingFXVolume, playerSoundRange, Sound.SoundType.Dangerous, true);
            stepTimer = 0f;
        }
    }

}
