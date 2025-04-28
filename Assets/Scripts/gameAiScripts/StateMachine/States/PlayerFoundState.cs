using UnityEngine;
using UnityEngine.AI;

public class PlayerFoundState : IState
{
    public NavMeshAgent entity;
    private Transform playerCameraTransform;
    private Rigidbody playerRigidBody;
    private Animator animator;
    private Camera mainCamera;
    private AudioClip deathSound;
    private GameObject deathPanel;
    public PlayerFoundState(NavMeshAgent entity, 
                            Transform playerCameraTransform, 
                            Transform playerTransform, 
                            Animator animator, 
                            Camera mainCamera,
                            GameObject deathPanel,
                            AudioClip deathSound)
    {
        this.entity = entity;
        this.playerCameraTransform = playerCameraTransform;
        playerRigidBody = playerTransform.GetComponent<Rigidbody>();
        this.animator = animator;
        this.mainCamera = mainCamera;
        this.deathPanel = deathPanel;
        this.deathSound = deathSound;
    }
    public void FrameUpdate()
    {
        playerCameraTransform.LookAt(entity.transform.position + (Vector3.up * 2f));
    }

    public void OnEnterState()
    {
        Debug.Log("PlayerFound State Enter");
        AudioManager.instance.PlaySoundFXClip(deathSound, playerCameraTransform, 1f, 0f, Sound.SoundType.Default, false);
        entity.isStopped = true;
        deathPanel.SetActive(true);
        // Freeze only the position, NOT the rotation
        playerRigidBody.constraints = RigidbodyConstraints.FreezePosition;
    }

    public void OnExitState()
    {
        Debug.Log("PlayerFound State Exit");
    }

    public void PhysicsUpdate()
    {
    }
}
