using System.Collections.Generic;
using UnityEngine;

public class PoolingManager : MonoBehaviour
{


    public GameObject bulletPrefab;
    public int initialPoolSize = 20;

    private Queue<GameObject> pool = new Queue<GameObject>();

    private void Awake()
    {
        for (int i = 0; i < initialPoolSize; i++)
        {
            CreateNewBullet();
        }
    }

    private GameObject CreateNewBullet()
    {
        GameObject obj = Instantiate(bulletPrefab);
        
        Bullets bulletScript = obj.GetComponent<Bullets>();
        if (bulletScript != null)
        {
            bulletScript.Initialize(this);
        }

        obj.SetActive(false);
        obj.transform.SetParent(transform);
        pool.Enqueue(obj);
        return obj;
    }

    public GameObject GetBullet()
    {
        GameObject bullet;

        if (pool.Count > 0)
        {
            bullet = pool.Dequeue();
        }
        else
        {
            bullet = CreateNewBullet();
        }

        bullet.transform.SetParent(null);
        bullet.SetActive(false); 
        return bullet;
    }

    public void ReturnBullet(GameObject bullet)
    {
        bullet.SetActive(false);
        
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