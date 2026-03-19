using UnityEngine;

//Attach to an enemy. The closer the enemy is to the player, the higher the Intensity — the music literally tracks danger.
public class IntensityByProximity : MonoBehaviour
{
    public AdaptiveMusicController musicController;
    public Transform player;
    public float dangerRadius = 15f;   // Max intensity within this distance
    public float safeRadius = 40f;   // Zero intensity beyond this distance

    void Update()
    {
        float dist = Vector3.Distance(transform.position, player.position);

        // Map distance to 0-1 intensity (closer = higher value)
        float intensity = 1f - Mathf.InverseLerp(dangerRadius, safeRadius, dist);

        musicController.SetIntensity(intensity);
    }
}