using System.Collections.Generic;
using UnityEngine;

public class PoolingManager : MonoBehaviour
{
    public static PoolingManager Instance;

    public GameObject bulletPrefab;
    public int initialPoolSize = 20;

    private Queue<GameObject> pool = new Queue<GameObject>();

    private void Awake()
    {
        Instance = this;

        // Création initiale du pool
        for (int i = 0; i < initialPoolSize; i++)
        {
            GameObject obj = Instantiate(bulletPrefab);
            obj.SetActive(false);
            obj.transform.SetParent(transform);
            pool.Enqueue(obj);
        }
    }

    // on récupère la balle dans le pool
public GameObject GetBullet()
{
    GameObject bullet;

    if (pool.Count > 0)
    {
        bullet = pool.Dequeue();
    }
    else
    {
        bullet = Instantiate(bulletPrefab);
    }

    // IMPORTANT : détacher d'abord, PUIS activer
    bullet.transform.SetParent(null);
    bullet.SetActive(true);

    return bullet;
}


    // remet la balle dans le pool
    public void ReturnBullet(GameObject bullet)
    {
        bullet.SetActive(false);

        // Réinitialisation physique
        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        bullet.transform.SetParent(transform);
        pool.Enqueue(bullet);
    }
}
