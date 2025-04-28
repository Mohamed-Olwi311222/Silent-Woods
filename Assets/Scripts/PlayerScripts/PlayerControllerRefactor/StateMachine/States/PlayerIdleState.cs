using Unity.Cinemachine;
using UnityEngine;

public class PlayerIdleState : IPlayerState
{
    #region PlayerCameraWhileIdlingNeededProperties
    private CinemachineBasicMultiChannelPerlin noise;
    private readonly CinemachineCamera firstPersonCamera;
    private readonly PlayerController playerController; //for getting the moveinput
    #endregion
    public PlayerIdleState(CinemachineCamera firstPersonCamera, Transform playertransform)
    {
        noise = firstPersonCamera.GetComponent<CinemachineBasicMultiChannelPerlin>();
        this.firstPersonCamera = firstPersonCamera;
        playerController = playertransform.GetComponent<PlayerController>();
    }
    public void FrameUpdate()
    {
    }

    public void OnEnterState()
    {
        noise.AmplitudeGain = 0.7f;
        noise.FrequencyGain = 0.33f;
        playerController.moveInput.x = 0; 
        playerController.moveInput.y = 0;
    }

    public void OnExitState()
    {
    }

    public void PhysicsUpdate()
    {
    }
}
