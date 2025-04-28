using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;
    public Sound sound;
    [SerializeField] private AudioSource soundFXPrefab; // Use a prefab for new AudioSources

    void Awake()
    {
        if (instance == null)
        { 
            sound = new Sound();
            instance = this;
        }
        else { Destroy(gameObject); }
    }

    public void PlaySoundFXClip(AudioClip audioClip, Transform objectTransform, float volume, float soundRange, Sound.SoundType soundType, bool randomPitchEnable)
    {
        if (audioClip == null) return;

        // Create a new AudioSource instance for each sound
        AudioSource newAudioSource = Instantiate(soundFXPrefab, objectTransform.position, Quaternion.identity);
        sound.SoundPosition = newAudioSource.transform.position;
        sound.Range = soundRange;
        sound.MySoundType = soundType;
        if (randomPitchEnable)
        {
            newAudioSource.pitch = Random.Range(0.7f, 1f); // Added slight variation
        }
        newAudioSource.clip = audioClip; 
        newAudioSource.volume = volume;
        newAudioSource.Play();

        // Destroy the AudioSource after the clip finishes
        Destroy(newAudioSource.gameObject, audioClip.length);
    }
    public void PlayRandomSoundFXClip(AudioClip[] audioClip, Transform objectTransform, float volume, float soundRange, Sound.SoundType soundType, bool randomPitchEnable)
    {
        if (audioClip.Length == 0) return;

        // Create a new AudioSource instance for each sound
        AudioSource newAudioSource = Instantiate(soundFXPrefab, objectTransform.position, Quaternion.identity);
        sound.SoundPosition = newAudioSource.transform.position;
        sound.Range = soundRange;
        sound.MySoundType = soundType;
        if (randomPitchEnable)
        {
            newAudioSource.pitch = Random.Range(0.7f, 1f); // Added slight variation
        }
        int randomIdx = Random.Range(0, audioClip.Length - 1);
        newAudioSource.clip = audioClip[randomIdx]; 
        newAudioSource.volume = volume;
        newAudioSource.Play();

        // Destroy the AudioSource after the clip finishes
        Destroy(newAudioSource.gameObject, audioClip[randomIdx].length);
    }
}
