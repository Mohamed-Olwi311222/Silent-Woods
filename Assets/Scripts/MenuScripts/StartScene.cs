using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartScene : MonoBehaviour
{
    [SerializeField] AudioClip[] audioClip;
    [SerializeField] Transform cameraTransform;
    private bool playedSound = false;
    void Update()
    {
        if (!playedSound)
        {
            StartCoroutine(PlayRandom());
        }
    }
    IEnumerator PlayRandom()
    {
        playedSound = true;
        AudioManager.instance.PlayRandomSoundFXClip(audioClip, cameraTransform, 0.5f, 0f, Sound.SoundType.Default, false);
        yield return new WaitForSeconds(12f);
        playedSound = false;
    }
    public void StartGame()
    {
        SceneManager.LoadScene("ControlsHelpScene");
    }
}
