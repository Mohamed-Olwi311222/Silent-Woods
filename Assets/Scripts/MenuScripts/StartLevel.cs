using UnityEngine;
using UnityEngine.SceneManagement;

public class StartLevel : MonoBehaviour
{
    public void StartLevel0()
    {
        SceneManager.LoadScene("Level");
    }
}
