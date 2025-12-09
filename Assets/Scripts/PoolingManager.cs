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
            // si le pool est vide, on en crée une nouvelle
            bullet = Instantiate(bulletPrefab);
        }

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
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        bullet.transform.SetParent(transform);
        pool.Enqueue(bullet);
    }
}
