using UnityEngine;

public class FireBullet2 : MonoBehaviour
{
    public Transform firePoint;

    public float bulletSpeed = 10f; // J'ai augmenté la vitesse pour un meilleur effet de tir

    public GameObject bulletPrefab;

    public void Fire2()
    {
        GameObject newBullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        
        Rigidbody rb = newBullet.GetComponent<Rigidbody>();

        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero; 
            rb.angularVelocity = Vector3.zero;

            rb.AddForce(firePoint.forward * bulletSpeed, ForceMode.Impulse);
        }

    }
}