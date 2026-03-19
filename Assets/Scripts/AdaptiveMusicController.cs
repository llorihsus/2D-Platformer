using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public class AdaptiveMusicController : MonoBehaviour
{
    [SerializeField]
    private EventReference musicEventRef;   // Assign in Inspector

    private EventInstance musicInstance;

    void Start()
    {
        // Create and start the FMOD event instance
        musicInstance = RuntimeManager.CreateInstance(musicEventRef);
        musicInstance.start();
    }

    // Call this from any gameplay script to change the music intensity
    public void SetIntensity(float value)
    {
        value = Mathf.Clamp01(value);   // Keep within 0.0 - 1.0 range
        musicInstance.setParameterByName("Intensity", value);
    }

    void OnDestroy()
    {
        // Always stop and release instances to avoid memory leaks
        musicInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        musicInstance.release();
    }
}