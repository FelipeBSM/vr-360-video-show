using UnityEngine;

public class SetPath : MonoBehaviour
{
    [Tooltip("Video Name inside Downloads folder in Quest")]
    public string toVideoName;

    [Tooltip("Clip relative to the audio")]
    public AudioClip videoAudioClip;

    public Perspective perspective;

    public Vector3 audioOrientation;
    public string GetVideoPath()
    {
        return this.toVideoName;
    }

    public AudioClip GetVideoAudioClip()
    {
        return this.videoAudioClip;
    }

    public Perspective GetPerspective()
    {
        return this.perspective;
    }

    public Vector3 GetAudioRot()
    {
        return this.audioOrientation;
    }
}
