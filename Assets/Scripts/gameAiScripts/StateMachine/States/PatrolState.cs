using UnityEngine;
using UnityEngine.AI;

public class PatrolState : IState
{
    public NavMeshAgent entity;
    public Vector3 playerPosition;
    public Transform[] patrolPoints;
    private int patrolPointsIdx;

    public PatrolState(NavMeshAgent entity, Vector3 playerPosition, Transform[] patrolPoints)
    {
        this.entity = entity;
        this.playerPosition = playerPosition;
        this.patrolPoints = patrolPoints;
    }
    public void FrameUpdate()
    {
        //check if the path has been computed and the remaining distance is small
        if (!entity.pathPending && entity.remainingDistance < 0.5f)
        {
            //go to the next patrol point
            GoToNextPoint();
        }
    }

    private void GoToNextPoint()
    {
        if (patrolPoints.Length == 0)
            return;
        entity.speed = 10;
        entity.SetDestination(patrolPoints[patrolPointsIdx].position);
        patrolPointsIdx = (patrolPointsIdx + 1) % patrolPoints.Length;
        return;
    }

    public void OnEnterState()
    {
        Debug.Log("Patrol State Enter");
        entity.isStopped = false;
    }

    public void OnExitState()
    {
        Debug.Log("Patrol State Exit");
    }

    public void PhysicsUpdate()
    {
    }
}
