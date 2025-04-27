using UnityEngine;
using UnityEngine.AI;

public class PlayerFoundState : IState
{
    public NavMeshAgent entity;
    public PlayerFoundState(NavMeshAgent entity)
    {
        this.entity = entity;
    }
    public void FrameUpdate()
    {

    }

    public void OnEnterState()
    {
        Debug.Log("PlayerFound State Enter");
        entity.isStopped = true;
        //TODO Freeze Player
        //TODO Play Cutscene
        //TODO Restart Level
    }

    public void OnExitState()
    {
        Debug.Log("PlayerFound State Exit");
    }

    public void PhysicsUpdate()
    {
    }
}
