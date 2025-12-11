using UnityEngine;
using UnityEngine.AddressableAssets; 
using UnityEngine.ResourceManagement.AsyncOperations; 

public class Explosiontarget : MonoBehaviour
{
    private GameObject explosionEffectPrefab;

    public int positionIndex;

    public void Start()
    {
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
                Instantiate(explosionEffectPrefab, transform.position, Quaternion.identity);
            }

            TargetPool.Instance.ReturnTarget(this);
        }
    }
}