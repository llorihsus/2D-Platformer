using UnityEngine;
using FMODUnity;

//Global parameters affect all events at once — ideal for system-wide states like player health, underwater, or time of day.
public class GlobalAudioState : MonoBehaviour
{
    // Call when player health changes (0.0 = critical, 1.0 = full health)
    public void OnHealthChanged(float normalizedHealth)
    {
        RuntimeManager.StudioSystem.setParameterByName("PlayerHealth", normalizedHealth);
    }

    // Trigger the underwater audio filter snapshot
    public void EnterWater()
    {
        RuntimeManager.StudioSystem.setParameterByName("IsUnderwater", 1f);
    }

    public void ExitWater()
    {
        RuntimeManager.StudioSystem.setParameterByName("IsUnderwater", 0f);
    }
}