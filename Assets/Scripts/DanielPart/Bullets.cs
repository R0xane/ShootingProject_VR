using UnityEngine;

public class Bullets : MonoBehaviour
{
    public float lifetime = 5f;
    
    // La balle garde en mémoire qui est son "patron"
    private PoolingManager bulletPool;

    // Cette fonction sera appelée par le Manager pour se présenter
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
        // On vérifie qu'on a bien un pool assigné
        if (bulletPool != null && gameObject.activeSelf)
        {
            bulletPool.ReturnBullet(gameObject);
        }
        else if (gameObject.activeSelf)
        {
            // Sécurité si jamais la balle n'a pas de pool
            Destroy(gameObject); 
        }
    }
}