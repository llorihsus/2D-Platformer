using UnityEngine;
using FMODUnity;

//For fire-and-forget sounds (footsteps, UI clicks, shots), use the static helper — no manual instance management needed.
public class WeaponAudio : MonoBehaviour
{
    [SerializeField] private EventReference shootSfxRef;
    [SerializeField] private EventReference reloadSfxRef;

    public void PlayShoot()
    {
        // Plays at the object's world position (3D spatialized automatically)
        RuntimeManager.PlayOneShot(shootSfxRef, transform.position);
    }

    public void PlayReload()
    {
        RuntimeManager.PlayOneShot(reloadSfxRef, transform.position);
    }
}