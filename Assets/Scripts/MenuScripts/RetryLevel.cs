using UnityEngine;
using UnityEngine.SceneManagement;
public class RetryLevel : MonoBehaviour
{
    public void RetryScene()
    {
        SceneManager.LoadScene("Level");
    }
}
