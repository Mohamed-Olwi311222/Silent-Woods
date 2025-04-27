using UnityEngine;
using UnityEngine.AI;

public class AggressiveState : IState
{
    public NavMeshAgent entity;
    public Transform playerTransform;

    public AggressiveState(NavMeshAgent entity, Transform playerTransform)
    {
        this.entity = entity;
        this.playerTransform = playerTransform;
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
        entity.speed = 12;
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
