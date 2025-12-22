using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class PoolingManager : MonoBehaviour
{

    [Header ("Bullet Pooling Settings")]
    public GameObject bulletPrefab;
    public int initialPoolSize = 20;

    [Header ("Cleanup Settings")]
    public Transform referencePoint;
    public float cleanupDistance = 50f;
    public float cleanupInterval = 0.1f;

    private Queue<GameObject> pool = new Queue<GameObject>();

    private List<GameObject> activeBullets = new List<GameObject>();

    private void Awake()
    {
        for (int i = 0; i < initialPoolSize; i++)
        {
            CreateNewBullet();
        }
    }

    public void Start(){
        if (referencePoint == null)
        {
            referencePoint = transform;
        }
        StartCoroutine(CleanupRoutine());
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
        activeBullets.Add(bullet);
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
        activeBullets.Remove(bullet);
        pool.Enqueue(bullet);
    }

    private IEnumerator CleanupRoutine()
    {
        WaitForSeconds wait = new WaitForSeconds(cleanupInterval);

        while (true)
        {
            for (int i = activeBullets.Count - 1; i >= 0; i--)
            {
                GameObject ab = activeBullets[i];

                if (ab == null)
                {
                    activeBullets.RemoveAt(i);
                    continue;
                }

                float dist = Vector3.Distance(referencePoint.position, ab.transform.position);
                if (dist > cleanupDistance)
                {
                    ReturnBullet(ab);
                    Debug.Log($"Clean: {ab.name} id={ab.GetInstanceID()} dist={dist}");

                }
            }

            yield return wait;
        }
    }
}