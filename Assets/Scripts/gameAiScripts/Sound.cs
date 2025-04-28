using UnityEngine;
public class Sound
{
    public enum SoundType {Default = -1, Interesting, Dangerous}; 
    public SoundType MySoundType {set; get;}
    public Vector3 SoundPosition {set; get;}
    public float Range {set; get;}
    /// <summary>
    /// This the intensity of the sound.
    /// </summary>
}
