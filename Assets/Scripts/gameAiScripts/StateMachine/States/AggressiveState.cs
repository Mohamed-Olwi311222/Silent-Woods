using UnityEngine;
using UnityEngine.AI;

public class AggressiveState : IState
{
    public NavMeshAgent entity;
    public Transform playerTransform;
    private Animator animator;

    public AggressiveState(NavMeshAgent entity, Transform playerTransform, Animator animator)
    {
        this.entity = entity;
        this.playerTransform = playerTransform;
        this.animator = animator;
    }
    public void FrameUpdate()
    {
        entity.SetDestination(playerTransform.position);
        //TODO Add sound of the scream
    }

    public void OnEnterState()
    {
        Debug.Log("Aggressive State Enter");
        entity.isStopped = false;
        entity.speed = 7;
        animator.SetBool("isRunning", true);
        animator.SetBool("isPatroling", false);
    }

    public void OnExitState()
    {
        Debug.Log("Aggressive State Exit");
    }

    public void PhysicsUpdate()
    {
        throw new System.NotImplementedException();
    }
}
