using UnityEngine;

public class StartGame : MonoBehaviour
{
    public static bool playerEntered = false;
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerEntered = true;
        }
    }
    void OnTriggerExit(Collider other)
    {
        playerEntered = false;
    }
}
