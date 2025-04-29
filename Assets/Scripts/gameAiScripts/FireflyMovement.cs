using UnityEngine;
using UnityEngine.AI;

public class FireflyMovement : MonoBehaviour
{
    NavMeshAgent fireflyAgent;
    [SerializeField] private Transform[] fireFlyPoints;
    private int currentPointIdx = 0;
    private bool playerReachedMe = false;
    void Awake()
    {
        fireflyAgent = GetComponent<NavMeshAgent>();
        fireflyAgent.SetDestination(fireFlyPoints[currentPointIdx].position);
        fireflyAgent.isStopped = false;
        currentPointIdx++;
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && fireflyAgent.isStopped)
        {
            playerReachedMe = true;
        }
    }
    void Update()
    {
        Debug.Log(currentPointIdx);
        //check if the path has been computed and the remaining distance is small
        if (!fireflyAgent.pathPending && fireflyAgent.remainingDistance < 0.5f)
        {
            fireflyAgent.isStopped = true;
            if (playerReachedMe)
            {
                //go to the next patrol point
                GoToNextPoint();
                playerReachedMe = false;
                fireflyAgent.isStopped = false;
            }
        }
    }
    private void GoToNextPoint()
    {
        if (fireFlyPoints.Length == 0)
            return;
        fireflyAgent.SetDestination(fireFlyPoints[currentPointIdx].position);
        currentPointIdx = (currentPointIdx + 1) % fireFlyPoints.Length;
        return;
    }
}
