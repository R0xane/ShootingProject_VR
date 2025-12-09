using System.Collections.Generic;
using UnityEngine;

public class TargetPool : MonoBehaviour
{
    public static TargetPool Instance;

    public Explosiontarget targetPrefab;
    public int initialPoolSize = 6;

    private Queue<Explosiontarget> pool = new Queue<Explosiontarget>();

    // Positions fixes
    private Vector3[] spawnPositions = new Vector3[]
    {
        new Vector3(6.455f, 1.14425f, -6.387f),
        new Vector3(5.818f, 1.14425f, -6.387f),
        new Vector3(7.1349f, 1.14425f, -6.387f),
        new Vector3(7.994f, 1.14425f, -6.387f),
        new Vector3(8.853f, 1.14425f, -6.387f),
        new Vector3(9.711f, 1.14425f, -6.387f)
    };

    // Quel point est occupé ?
    private bool[] occupied;

    private void Awake()
    {
        Instance = this;
        occupied = new bool[spawnPositions.Length];
    }

    private void Start()
    {
        // On place une target sur chaque position au début
        for (int i = 0; i < spawnPositions.Length; i++)
        {
            SpawnAtIndex(i);
        }
    }

    private Explosiontarget CreateNewTarget()
    {
        Explosiontarget newTarget = Instantiate(targetPrefab);
        newTarget.gameObject.SetActive(false);
        pool.Enqueue(newTarget);
        return newTarget;
    }

    private Explosiontarget GetTarget()
    {
        if (pool.Count == 0)
            CreateNewTarget();

        Explosiontarget t = pool.Dequeue();
        t.ResetState();
        return t;
    }

    private void SpawnAtIndex(int index)
    {
        Explosiontarget t = GetTarget();

        t.positionIndex = index;                 // assignation de l'index
        t.transform.position = spawnPositions[index];
        t.transform.rotation = Quaternion.identity;

        occupied[index] = true;

        t.gameObject.SetActive(true);
    }

    // Quand la cible est détruite
    public void ReturnTarget(Explosiontarget target)
    {
        int index = target.positionIndex;

        // On marque cet emplacement comme libre
        occupied[index] = false;

        target.gameObject.SetActive(false);
        pool.Enqueue(target);

        // Respawn après un délai → AU MÊME ENDROIT
        StartCoroutine(RespawnAfterDelay(index));
    }

    private System.Collections.IEnumerator RespawnAfterDelay(int index)
    {
        yield return new WaitForSeconds(1.5f);

        // Respawn seulement si la position est toujours vide
        if (!occupied[index])
        {
            SpawnAtIndex(index);
        }
    }
}
