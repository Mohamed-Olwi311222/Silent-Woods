using System;
using System.Collections;
using UnityEngine;

public class FieldOfView : MonoBehaviour
{
    public EntityBehaviour entityBehaviour;
    public float aggresiveRadius;
    [Range(0, 360)]
    public float angle;
    [SerializeField] public GameObject playerRef;
    public LayerMask targetMask;
    public LayerMask obstructionMask;
    public bool canSeePlayer;


    private void Awake()
    {
        entityBehaviour = GetComponent<EntityBehaviour>();
    }
    private void Start()
    {
        StartCoroutine(FOVRoutine());
    }

    private IEnumerator FOVRoutine()
    {
        WaitForSeconds wait = new WaitForSeconds(0.333f);

        while (true)
        {
            yield return wait;
            FieldOfViewCheck();
        }
    }

    private void FieldOfViewCheck()
    {
        Collider[] rangeChecks = Physics.OverlapSphere(transform.position, aggresiveRadius, targetMask);

        CheckForCollision(rangeChecks);
    }

    public void CheckForCollision(Collider[] rangeChecks)
    {
        if (rangeChecks.Length != 0)
        {
            Transform target = rangeChecks[0].transform;
            Vector3 directionToTarget = (target.position - transform.position).normalized;

            if (Vector3.Angle(transform.forward, directionToTarget) < angle / 2)
            {
                float distanceToTarget = Vector3.Distance(transform.position, target.position);

                if (!Physics.Raycast(transform.position, directionToTarget, distanceToTarget, obstructionMask))
                    canSeePlayer = true;
                else
                    canSeePlayer = false;
            }
            else
                canSeePlayer = false;
        }
        else if (canSeePlayer)
            canSeePlayer = false;
    }
}
