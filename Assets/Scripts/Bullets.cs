using UnityEngine;

public class Bullets : MonoBehaviour
{
    public float lifetime = 5f;
    
    private PoolingManager bulletPool;

    public void Initialize(PoolingManager pool)
    {
        bulletPool = pool;
    }

    private void OnEnable()
    {
        Invoke(nameof(ReturnToPool), lifetime);
    }

    private void OnDisable()
    {
        CancelInvoke();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player")) return;
        ReturnToPool();
    }

    private void ReturnToPool()
    {
        if (bulletPool != null && gameObject.activeSelf)
        {
            bulletPool.ReturnBullet(gameObject);
        }
        else if (gameObject.activeSelf)
        {
            Destroy(gameObject); 
        }
    }
}