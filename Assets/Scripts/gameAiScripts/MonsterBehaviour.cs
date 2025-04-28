using System;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(EntityBehaviour))]
[RequireComponent(typeof(FieldOfView))]
[RequireComponent(typeof(Hearing))]

public class MonsterBehaviour : MonoBehaviour
{
    EntityBehaviour entityBehaviour;
    FieldOfView fieldOfView;
    Hearing hearing;
    StateMachine stateMachine;
    NavMeshAgent monsterEntity;
    GameObject playerRef;
    [SerializeField] Transform[] patrolPoints;
    [SerializeField] bool triggerMeToStartPatrol = false;
    public bool playerWasFound = false;
    [SerializeField] Animator animator;
    private void AddTransition(IState from, IState to, Func<bool> condition) =>
                                                                stateMachine.AddTransition(from, to, condition);
    void Awake()
    {
        stateMachine = new StateMachine();
        monsterEntity = GetComponent<NavMeshAgent>();
        entityBehaviour = GetComponent<EntityBehaviour>();
        fieldOfView = GetComponent<FieldOfView>();
        hearing = GetComponent<Hearing>();
        animator = GetComponent<Animator>();
        playerRef = fieldOfView.playerRef;

        //states
        IdleState idleState = new IdleState(monsterEntity, animator);
        PatrolState patrolState = new PatrolState(monsterEntity, playerRef.transform.position, patrolPoints, animator);
        AggressiveState aggressiveState = new AggressiveState(monsterEntity, playerRef.transform, animator);
        AwareState awareState = new AwareState(monsterEntity, playerRef.transform.position, entityBehaviour, this, animator);
        PlayerFoundState playerFoundState = new PlayerFoundState(monsterEntity);

        //transitions
        AddTransition(idleState, patrolState, MonsterTriggered());
        AddTransition(patrolState, aggressiveState, CanSeeOrCanHear());
        AddTransition(aggressiveState, playerFoundState, PlayerIsCaught());
        AddTransition(aggressiveState, awareState, PlayerNotCaught());
        AddTransition(awareState, patrolState, PlayerIsLost());
        AddTransition(awareState, aggressiveState, PlayerIsFoundAgain());


        stateMachine.SetState(idleState);


    }

    Func<bool> MonsterTriggered()
    {
        return () => triggerMeToStartPatrol; //TODO When the player picks up a key or something
    }
    public Func<bool> CanSeeOrCanHear()
    {
        return () => hearing.CanHear || fieldOfView.canSeePlayer;
    }
    Func<bool> PlayerIsCaught()
    {
        return () =>
                Vector3.Distance(playerRef.transform.position, transform.position) < entityBehaviour.deathRadius;
    }
    Func<bool> PlayerNotCaught()
    {
        return PlayerEscaped;
    }
    private bool PlayerEscaped()
    {
        if (CanSeeOrCanHear()()) { return false; }
        else { return true; }
    }

    Func<bool> PlayerIsLost()
    {
        return () =>  entityBehaviour.playerNotFoundAtAwareState;
    }
    Func<bool> PlayerIsFoundAgain()
    {
        return () => entityBehaviour.playerIsFoundAtAwareState;
    }
    void Update()
    {
        stateMachine.FrameUpdate();
    }

}