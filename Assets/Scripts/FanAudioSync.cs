using UnityEngine;

public class FanAudioSync : MonoBehaviour
{
    [Header("Rotation")]
    [SerializeField] private Transform blades;
    [SerializeField] private Vector3 spinAxis = Vector3.forward;
    [SerializeField] private float currentSpeed = 0f;
    [SerializeField] private float targetSpeed = 720f;
    [SerializeField] private float acceleration = 400f;

    [Header("Audio")]
    [SerializeField] private AudioSource fanAudio;
    [SerializeField] private float minPitch = 0.8f;
    [SerializeField] private float maxPitch = 1.3f;
    [SerializeField] private float minVolume = 0.15f;
    [SerializeField] private float maxVolume = 0.6f;
    [SerializeField] private float maxSpeedForAudio = 1200f;

    void Update()
    {
        // Smooth speed change
        currentSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed, acceleration * Time.deltaTime);

        // Rotate blades
        if (blades != null)
        {
            blades.Rotate(spinAxis * currentSpeed * Time.deltaTime, Space.Self);
        }

        // Sync audio
        if (fanAudio != null)
        {
            float t = Mathf.InverseLerp(0f, maxSpeedForAudio, currentSpeed);

            fanAudio.pitch = Mathf.Lerp(minPitch, maxPitch, t);
            fanAudio.volume = Mathf.Lerp(minVolume, maxVolume, t);

            if (!fanAudio.isPlaying && currentSpeed > 0.01f)
                fanAudio.Play();

            if (fanAudio.isPlaying && currentSpeed <= 0.01f)
                fanAudio.Stop();
        }
    }

    public void SetTargetSpeed(float newSpeed)
    {
        targetSpeed = Mathf.Max(0f, newSpeed);
    }

    public void TurnOff()
    {
        targetSpeed = 0f;
    }

    public void SetLow()
    {
        targetSpeed = 400f;
    }

    public void SetMedium()
    {
        targetSpeed = 750f;
    }

    public void SetHigh()
    {
        targetSpeed = 1100f;
    }
}
