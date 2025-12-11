using UnityEngine;
using UnityEngine.AddressableAssets; 
using UnityEngine.ResourceManagement.AsyncOperations; 

public class Explosiontarget : MonoBehaviour
{
    // On change en GameObject pour être plus générique (le prefab entier)
    private GameObject explosionEffectPrefab;

    public int positionIndex;

    public void Start()
    {
        LoadAddressableEffect();
    }

    private void LoadAddressableEffect()
    {
        string labelToLoad = "";

        // 1. On définit le label selon la plateforme
        #if UNITY_ANDROID
            labelToLoad = "Quest"; 
        #else
            labelToLoad = "PCVR"; 
        #endif

        // 2. On lance le chargement EN DEHORS des blocs #if/#else
        // Sinon, sur Android, cette partie n'existait pas !
        Addressables.LoadAssetsAsync<GameObject>(labelToLoad, (obj) =>
        {
            // Filtre : on ne garde que l'objet qui contient "Explosion" dans son nom
            if (obj.name.Contains("Explosion")) 
            {
                explosionEffectPrefab = obj;
            }
        });
    }   

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.name.Contains("Bullet"))
        {
            if (explosionEffectPrefab != null)
            {
                // Instantiation classique du prefab chargé en mémoire
                Instantiate(explosionEffectPrefab, transform.position, Quaternion.identity);
            }

            // Retour au pool
            TargetPool.Instance.ReturnTarget(this);
        }
    }
}