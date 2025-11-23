using UnityEngine;

public class GunShoot : MonoBehaviour
{
    public ParticleSystem muzzleFlash;
    public Light muzzleLight;
    public AudioSource gunAudio;


    public void Fire()
    {
        gunAudio.Play();
        if (muzzleFlash != null)
        {
            muzzleFlash.Play(); 
            if (muzzleLight != null)
            {
                muzzleLight.enabled = true; 
            }
        }
        else {
            muzzleLight.enabled = false; 
        }
    }
}