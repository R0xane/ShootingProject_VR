using UnityEngine;

public class FireBullet : MonoBehaviour
{
    public PoolingManager bulletPool; 
    
    public Transform firePoint;
    public float bulletSpeed = 2f;

    public void Fire()
    {
        GameObject bullet = bulletPool.GetBullet();

        bullet.transform.position = firePoint.position;
        bullet.transform.rotation = firePoint.rotation;
        bullet.transform.localScale = new Vector3(0.05f, 0.05f, 0.05f); 

        bullet.SetActive(true);

        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.AddForce(firePoint.forward * bulletSpeed, ForceMode.Impulse);
        }
    }
}