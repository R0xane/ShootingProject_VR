using UnityEngine;

public class FireBullet : MonoBehaviour
{
    public Transform firePoint;
    public float bulletSpeed = 40f;

    public void Fire()
    {
        GameObject bullet = PoolingManager.Instance.GetBullet();

        // Réinitialiser la position et rotation
        bullet.transform.position = firePoint.position;
        bullet.transform.rotation = firePoint.rotation;

        Rigidbody rb = bullet.GetComponent<Rigidbody>();

        if (rb != null)
        {
            rb.velocity = Vector3.zero; 
            rb.AddForce(firePoint.forward * bulletSpeed, ForceMode.Impulse);
        }
    }
}
