using UnityEngine;

public class FinishGame : MonoBehaviour
{
    [SerializeField] GameObject winPanel;
    [SerializeField] Rigidbody playerRigidBody;
    [SerializeField] GameObject entity;
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            winPanel.SetActive(true);
            playerRigidBody.constraints = RigidbodyConstraints.FreezeAll;
            entity.SetActive(false);
        }
    }
}
