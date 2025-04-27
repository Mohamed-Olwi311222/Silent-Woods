using System.Collections;
using UnityEngine;

public class Hearing : MonoBehaviour
{
    public bool CanHear {set; get;}
    private Sound sound;
    void Start()
    {
        sound = AudioManager.instance.sound;
        StartCoroutine(RespondToSound());
    }
     private IEnumerator RespondToSound()
     {
        WaitForSeconds wait = new WaitForSeconds(0.333f);

        while (true)
        {
            if (Vector3.Distance(sound.SoundPosition, transform.position) <= sound.Range)
            {
                if (sound.MySoundType == Sound.SoundType.Dangerous)
                {
                    CanHear = true;
                }
            }
            else
            {
                CanHear = false;
            }
            yield return wait;
        }
    }

}
