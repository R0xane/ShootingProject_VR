using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using System.Collections.Generic;
using System.Collections; 

public class GunShoot : MonoBehaviour
{

    private ParticleSystem muzzleFlashParticles;
    private Light muzzleLight;
    

    private GameObject muzzleFlashInstance;

    public AudioSource gunAudio;
    

    public Transform firePoint; 

    void Start()
    {

        if (firePoint == null) firePoint = transform;

        LoadAddressableEffect();
    }

    private void LoadAddressableEffect()
    {
        string labelToLoad = "";

        #if UNITY_ANDROID
            labelToLoad = "Quest"; 
        #else
            labelToLoad = "PCVR"; 
        #endif

        Addressables.LoadAssetsAsync<GameObject>(labelToLoad, (obj) =>
        {
            if (obj.name.Contains("MuzzleFlash")) 
            {

                muzzleFlashInstance = Instantiate(obj, firePoint.position, firePoint.rotation, firePoint);
                
                muzzleFlashParticles = muzzleFlashInstance.GetComponent<ParticleSystem>();
                muzzleLight = muzzleFlashInstance.GetComponentInChildren<Light>();

                if (muzzleLight != null) muzzleLight.enabled = false;
                
                if (muzzleFlashParticles != null) muzzleFlashParticles.Stop();
            }
        }); 
      
    }

    public void Fire()
    {
        if(gunAudio != null) gunAudio.Play();

        if (muzzleFlashParticles != null)
        {
            muzzleFlashParticles.Play(); 

            if (muzzleLight != null)
            {
                muzzleLight.enabled = true;
                StartCoroutine(TurnOffLight()); 
            }
        }
        else
        {
            Debug.LogWarning("MuzzleFlash pas encore chargé ou introuvable !");
        }
    }

    IEnumerator TurnOffLight()
    {
        yield return new WaitForSeconds(0.1f); // Attend 0.1 seconde
        if (muzzleLight != null) muzzleLight.enabled = false;
    }
}