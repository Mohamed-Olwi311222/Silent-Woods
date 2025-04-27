using UnityEngine;
using UnityEngine.AI;
public class IdleState : IState
{
    public NavMeshAgent entity;
    public IdleState(NavMeshAgent entity)
    {
        this.entity = entity;
    }
    
    public void OnEnterState()
    {
        //Debug.Log("Idle State Enter");
        entity.isStopped = true;
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
