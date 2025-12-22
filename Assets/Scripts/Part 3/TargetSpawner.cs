using System.Collections;
using UnityEngine;

public class TargetSpawner : MonoBehaviour
{
    [SerializeField] private GamePlayManager gameplay;
    [SerializeField] private TargetPool targetPool;
    [SerializeField] private float spawnInterval = 1f;

    private void Awake()
    {
        if (gameplay == null) gameplay = FindFirstObjectByType<GamePlayManager>();
        if (targetPool == null) targetPool = FindFirstObjectByType<TargetPool>();
    }

    private void Start()
    {
        StartCoroutine(SpawnLoop());
    }

    private IEnumerator SpawnLoop()
    {
        while (targetPool != null && !targetPool.IsReady)
            yield return null;

        WaitForSeconds wait = new WaitForSeconds(spawnInterval);

        while (true)
        {
            if (gameplay != null && targetPool != null)
            {
                if (gameplay.currentTargets < gameplay.maxTargets)
                {
                    if (targetPool.TrySpawnTarget())
                    {
                        gameplay.RegisterTargetSpawned();
                    }
                }
            }

            yield return wait;
        }
    }
}
