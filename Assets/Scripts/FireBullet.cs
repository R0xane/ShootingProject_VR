using UnityEngine;

public class FireBullet : MonoBehaviour
{
    public Transform firePoint;
    public float bulletSpeed = 40f;

public void Fire()
{
    GameObject bullet = PoolingManager.Instance.GetBullet();

    // 1. Reset Physics (while still inactive)
    Rigidbody rb = bullet.GetComponent<Rigidbody>();
    if (rb != null)
    {
        // Note: Use 'velocity' for older Unity, 'linearVelocity' for Unity 6+
        rb.velocity = Vector3.zero; 
        rb.angularVelocity = Vector3.zero;
    }

    // 2. Position the bullet (while still inactive)
    bullet.transform.position = firePoint.position;
    bullet.transform.rotation = firePoint.rotation;

    // 3. ACTIVATE THE BULLET NOW
    bullet.SetActive(true); // <--- MOVE IT HERE

    // 4. Add Force
    if (rb != null)
    {
        rb.AddForce(firePoint.forward * bulletSpeed, ForceMode.Impulse);
    }
}
}
    