using UnityEngine;
using UnityEngine.AI;
public class IdleState : IState
{
    public NavMeshAgent entity;
    private Animator animator;
    public IdleState(NavMeshAgent entity, Animator animator)
    {
        this.entity = entity;
        this.animator = animator;
    }
    
    public void OnEnterState()
    {
        //Debug.Log("Idle State Enter");
        entity.isStopped = true;
        animator.SetBool("isRunning", false);
        animator.SetBool("isPatroling", false);
    }
    public void FrameUpdate()
    {
    }


    public void OnExitState()
    {
        Debug.Log("Idle State Exit");
    }

    public void PhysicsUpdate()
    {
    }
}
