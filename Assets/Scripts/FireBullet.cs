using UnityEngine;

public class FireBullet : MonoBehaviour
{
    // Tu dois glisser ton objet "PoolingManager" ici dans l'Inspector d'Unity
    public PoolingManager bulletPool; 
    
    public Transform firePoint;
    public float bulletSpeed = 2f;

    public void Fire()
    {
        // On demande la balle au pool qu'on a référencé
        GameObject bullet = bulletPool.GetBullet();

        bullet.transform.position = firePoint.position;
        bullet.transform.rotation = firePoint.rotation;
        bullet.transform.localScale = new Vector3(0.05f, 0.05f, 0.05f); // Remet à l'échelle normale au cas où

        bullet.SetActive(true);

        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.AddForce(firePoint.forward * bulletSpeed, ForceMode.Impulse);
        }
    }
}