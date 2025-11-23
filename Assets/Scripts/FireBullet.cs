using UnityEngine;

public class FireBullet : MonoBehaviour
{
    public GameObject bulletPrefab;
    public Transform firePoint;
        public float bulletSpeed = 40f;

    public void Fire()
    {
        GameObject spawnedBullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);

        Rigidbody rb = spawnedBullet.GetComponent<Rigidbody>();

        if (rb != null)
        {
            rb.AddForce(firePoint.forward * bulletSpeed, ForceMode.Impulse);
        }

    }
}