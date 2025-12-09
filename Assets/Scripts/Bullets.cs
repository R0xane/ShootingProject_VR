using UnityEngine;

public class Bullets : MonoBehaviour
{
    public float lifetime = 3f; 

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
        ReturnToPool();
    }

    private void ReturnToPool()
    {
        if (gameObject.activeSelf)  
        {
            PoolingManager.Instance.ReturnBullet(gameObject);
        }
    }
}
