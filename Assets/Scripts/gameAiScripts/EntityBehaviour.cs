using UnityEngine;
using UnityEngine.AI;
using System.Threading.Tasks;
using System.Collections;

[RequireComponent(typeof(NavMeshAgent))]
public class EntityBehaviour : MonoBehaviour
{
    [SerializeField] public float deathRadius;
    NavMeshAgent entity;
    public Hearing hearing;
    public FieldOfView fieldOfView;
    private bool hasVision;
    private bool hasHearing;
    public bool playerNotFoundAtAwareState = false;
    public bool playerIsFoundAtAwareState = false;
    private float checkInterval = 1f / 33f; // 30 times per second
    private float duration = 3f; // default value

    void Start()
    {
        entity = GetComponent<NavMeshAgent>();
        hasHearing = TryGetComponent<Hearing>(out hearing);
        hasVision = TryGetComponent<FieldOfView>(out fieldOfView);
    }

    public async void StartFindingPlayerTask(float customDuration)
    {
            duration = customDuration;
            await FindingPlayer();
    }
    public async Task<bool> FindingPlayer() 
    {
        float elapsedTime = 0f;
        playerNotFoundAtAwareState = false;
        playerIsFoundAtAwareState = false;
        entity.SetDestination(fieldOfView.playerRef.transform.position);
        StartCoroutine(GoToLastPlayerKnownLocation());
        while (elapsedTime < duration)
        {
            if ((hasVision && fieldOfView.canSeePlayer) || (hasHearing && hearing.CanHear))
            {
                playerIsFoundAtAwareState = true;
                playerNotFoundAtAwareState = false;
                return playerIsFoundAtAwareState; // Player found
            }
            await Task.Delay(Mathf.RoundToInt(checkInterval * 1000)); // Convert seconds to milliseconds
            elapsedTime += checkInterval;
        }
        playerIsFoundAtAwareState = false;
        playerNotFoundAtAwareState = true;
        return playerNotFoundAtAwareState; // Player not found
    }
    IEnumerator GoToLastPlayerKnownLocation()
    {
        Debug.Log("CoroutineStart");
        while (!(!entity.pathPending && entity.remainingDistance < 0.5f))
        {
            yield return null;
        }
        entity.isStopped = true;
        Quaternion originalRotation = entity.transform.rotation;
        Quaternion lookRight = Quaternion.Euler(0, entity.transform.eulerAngles.y + 50, 0);
        Quaternion lookLeft = Quaternion.Euler(0, entity.transform.eulerAngles.y - 50, 0);

        float duration = 1.5f; // Time in seconds for each rotation
        float elapsedTime = 0f;

        // Rotate to the right
        while (elapsedTime < duration)
        {
            entity.transform.rotation = Quaternion.Slerp(originalRotation, lookRight, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        elapsedTime = 0f;

        // Rotate to the left
        while (elapsedTime < duration)
        {
            entity.transform.rotation = Quaternion.Slerp(lookRight, lookLeft, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }
}
