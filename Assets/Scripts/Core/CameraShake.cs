// Add to a CameraShake.cs script on the Virtual Camera
// Usage: Call CameraShake.Instance.Shake(1.5f, 0.3f); from any script when damage occurs.

using Unity.Cinemachine;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public static CameraShake Instance { get; private set; }

    CinemachineCamera vcam;
    CinemachineBasicMultiChannelPerlin noise;
    float shakeTimer;

    void Awake()
    {
        Instance = this;
        vcam = GetComponent<CinemachineCamera>();
        noise = vcam.GetCinemachineComponent(CinemachineCore.Stage.Noise) as CinemachineBasicMultiChannelPerlin;
    }

    void Update()
    {
        if (shakeTimer > 0)
        {
            shakeTimer -= Time.deltaTime;
            if (shakeTimer <= 0)
            {
                noise.AmplitudeGain = 0;
                noise.FrequencyGain = 0;
            }
        }
    }

    public void Shake(float intensity = 1f, float duration = 0.2f)
    {
        noise.AmplitudeGain = intensity;
        noise.FrequencyGain = 2f;
        shakeTimer = duration;
    }
}