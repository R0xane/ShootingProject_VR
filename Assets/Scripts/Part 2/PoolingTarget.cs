using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets; 
using UnityEngine.ResourceManagement.AsyncOperations; 

public class TargetPool : MonoBehaviour
{
    public static TargetPool Instance;

    private GameObject targetPrefab; 
    
    public int initialPoolSize = 6; 

    private Queue<Explosiontarget> pool = new Queue<Explosiontarget>();

    public bool IsReady { get; private set; }

    public GamePlayManager gameplay;

    private Vector3[] spawnPositions = new Vector3[]
    {
        new Vector3(6.455f, 1.14425f, -6.387f),
        new Vector3(5.818f, 1.14425f, -6.387f),
        new Vector3(7.1349f, 1.14425f, -6.387f),
        new Vector3(7.994f, 1.14425f, -6.387f),
        new Vector3(8.853f, 1.14425f, -6.387f),
        new Vector3(9.711f, 1.14425f, -6.387f)
    };

    private bool[] occupied;

    private void Awake()
    {
        Instance = this;
        occupied = new bool[spawnPositions.Length];
        gameplay = GameObject.Find("GameManager").GetComponent<GamePlayManager>();
    }

    private void Start()
    {
        LoadPlatformAddressable();
    }

    private void LoadPlatformAddressable()
    {
        string labelToLoad = "";

        
        #if UNITY_ANDROID
            labelToLoad = "Quest"; 
            Debug.Log("Compilation Android : Chargement Quest");
        #else
            labelToLoad = "PCVR"; 
            Debug.Log("Compilation PC/Editor : Chargement PCVR");
        #endif
        

        Addressables.LoadAssetsAsync<GameObject>(labelToLoad, (obj) =>
        {
            if (obj.name.Contains("Target")) 
            {
                targetPrefab = obj;
                Debug.Log($"Target trouvée : {obj.name}");
            }
        }).Completed += OnAssetsLoaded;
    }

    private void OnAssetsLoaded(AsyncOperationHandle<IList<GameObject>> handle)
    {
        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            if (targetPrefab != null)
            {
                InitPool();
            }
            else
            {
                Debug.LogError("Addressables chargés mais aucun objet 'Target' trouvé !");
            }
        }
        else
        {
            Debug.LogError("Erreur lors du chargement des Addressables.");
        }
    }

    private void InitPool()
    {
        for (int i = 0; i < initialPoolSize; i++)
            CreateNewTargetInPool();

        IsReady = true; 
    }

    private Explosiontarget CreateNewTargetInPool()
    {
        if (targetPrefab == null) return null;

        GameObject obj = Instantiate(targetPrefab);
        
        obj.SetActive(false);
        obj.transform.SetParent(transform); 

        Explosiontarget newTarget = obj.GetComponent<Explosiontarget>();

        if (newTarget == null)
        {
            Debug.LogError("Le prefab chargé n'a pas le script 'Explosiontarget' !");
            Destroy(obj); 
            return null;
        }

        pool.Enqueue(newTarget);
        return newTarget;
    }

    private Explosiontarget GetTarget()
    {
        if (pool.Count == 0) return CreateNewTargetInPool();
        return pool.Dequeue();
    }

    private void SpawnAtIndex(int index)
    {
        Explosiontarget t = GetTarget();
        if (t == null) return; 

        t.positionIndex = index;
        t.transform.position = spawnPositions[index];
        t.transform.rotation = Quaternion.identity;
        
        occupied[index] = true;
        t.gameObject.SetActive(true);
    }

    public void ReturnTarget(Explosiontarget target)
    {
        int index = target.positionIndex;
        occupied[index] = false;

        target.gameObject.SetActive(false);
        target.transform.SetParent(transform);
        pool.Enqueue(target);

        if (gameplay != null) gameplay.TargetDespawned();
    }


    public bool TrySpawnTarget()
    {
        int freeIndex = -1;
        for (int i = 0; i < occupied.Length; i++)
        {
            if (!occupied[i])
            {
                freeIndex = i;
                break;
            }
        }

        if (freeIndex == -1) return false; 

        SpawnAtIndex(freeIndex);
        return true;
    }

}