using UnityEngine;

public class Gunshot1 : MonoBehaviour
{
    public ParticleSystem muzzleFlash;
    public Light muzzleLight;
    public AudioSource gunAudio;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void Fire()
    {
        gunAudio.Play();
        if (muzzleFlash != null){
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
