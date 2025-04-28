using UnityEngine;
using UnityEngine.AI;

public class AwareState : IState
{
    public NavMeshAgent entity;
    public Vector3 playerPosition;
    EntityBehaviour entityBehaviour;
    MonsterBehaviour monster;
    private Animator animator;
    float customDuration = 6.5f; //Affects entity rotation


    public AwareState(NavMeshAgent entity, Vector3 playerPosition, EntityBehaviour entityBehaviour, MonsterBehaviour monster, Animator animator)
    {
        this.entity = entity;
        this.playerPosition = playerPosition;
        this.entityBehaviour = entityBehaviour;
        this.monster = monster;
        this.animator = animator;
    }
    public void FrameUpdate()
    {

    }

    public void OnEnterState()
    {
        entityBehaviour.StartFindingPlayerTask(customDuration);
        entity.isStopped = false;
        Debug.Log("Aware State Enter");
        animator.SetBool("isRunning", false);
        animator.SetBool("isPatroling", false);
    }
    
    public void OnExitState()
    {
        Debug.Log("Aware State Exit");
    }

    public void PhysicsUpdate()
    {
    }
}
