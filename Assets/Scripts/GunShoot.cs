using UnityEngine;

public class GunShoot : MonoBehaviour
{
    public ParticleSystem muzzleFlash;
    public Light muzzleLight;


    public void Fire()
    {
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