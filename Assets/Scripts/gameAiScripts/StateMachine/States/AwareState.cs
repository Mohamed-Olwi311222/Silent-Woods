using UnityEngine;
using UnityEngine.AI;

public class AwareState : IState
{
    public NavMeshAgent entity;
    public Vector3 playerPosition;
    EntityBehaviour entityBehaviour;
    BacteriaBehaviour bacteria;
    float customDuration = 6.5f; //Affects entity rotation


    public AwareState(NavMeshAgent entity, Vector3 playerPosition, EntityBehaviour entityBehaviour, BacteriaBehaviour bacteria)
    {
        this.entity = entity;
        this.playerPosition = playerPosition;
        this.entityBehaviour = entityBehaviour;
        this.bacteria = bacteria;
    }
    public void FrameUpdate()
    {

    }

    public void OnEnterState()
    {
        entityBehaviour.StartFindingPlayerTask(customDuration);
        entity.isStopped = false;
        Debug.Log("Aware State Enter");
    }
    
    public void OnExitState()
    {
        Debug.Log("Aware State Exit");
    }

    public void PhysicsUpdate()
    {
    }
}
