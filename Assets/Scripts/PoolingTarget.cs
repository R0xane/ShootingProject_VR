using System.Collections.Generic;
using UnityEngine;

public class TargetPool : MonoBehaviour
{
    public static TargetPool Instance;

    public Explosiontarget targetPrefab;
    public int initialPoolSize = 6; // Assure-toi que ce nombre est au moins égal au nombre de positions (6)

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

    private bool[] occupied;

    private void Awake()
    {
        Instance = this;
        occupied = new bool[spawnPositions.Length];

        // --- ICI : ON PRÉ-REMPLIT LE POOL ---
        // On crée les objets avant même que le jeu commence vraiment
        for (int i = 0; i < initialPoolSize; i++)
        {
            CreateNewTargetInPool();
        }
    }

    private void Start()
    {
        // À ce moment là, le pool est déjà plein, donc GetTarget() va juste piocher dedans
        for (int i = 0; i < spawnPositions.Length; i++)
        {
            SpawnAtIndex(i);
        }
    }

    // Méthode utilitaire pour créer et ranger dans le pool
    private Explosiontarget CreateNewTargetInPool()
    {
        Explosiontarget newTarget = Instantiate(targetPrefab);
        newTarget.gameObject.SetActive(false);
        newTarget.transform.SetParent(transform); // On range l'objet sous le Pool pour garder la scène propre
        pool.Enqueue(newTarget);
        return newTarget;
    }

    private Explosiontarget GetTarget()
    {
        // Sécurité : Si le pool est vide (ex: on a besoin de 7 cibles mais poolSize est à 6), on en crée une nouvelle
        if (pool.Count == 0)
        {
            return CreateNewTargetInPool();
        }

        Explosiontarget t = pool.Dequeue();
        
        // IMPORTANT : Si tu as une méthode Reset() dans Explosiontarget, appelle-la ici
        // t.ResetState(); 
        
        return t;
    }

    private void SpawnAtIndex(int index)
    {
        Explosiontarget t = GetTarget();

        t.positionIndex = index;
        t.transform.position = spawnPositions[index];
        t.transform.rotation = Quaternion.identity;
        
        // Comme on a parenté au pool, c'est mieux de remettre null si la cible doit bouger librement, 
        // sinon tu peux laisser transform.SetParent(transform) si elles sont statiques.
        // t.transform.SetParent(null); 

        occupied[index] = true;
        t.gameObject.SetActive(true);
    }

    public void ReturnTarget(Explosiontarget target)
    {
        int index = target.positionIndex;
        occupied[index] = false;

        target.gameObject.SetActive(false);
        target.transform.SetParent(transform); // On la range à nouveau
        pool.Enqueue(target);

        StartCoroutine(RespawnAfterDelay(index));
    }

    private System.Collections.IEnumerator RespawnAfterDelay(int index)
    {
        yield return new WaitForSeconds(1.5f);

        if (!occupied[index])
        {
            SpawnAtIndex(index);
        }
    }
}