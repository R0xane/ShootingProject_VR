using UnityEngine;

public class Explosiontarget : MonoBehaviour
{
    public ParticleSystem explosionEffect;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.name.Contains("Bullet"))
        {
        if (explosionEffect != null)
        {
            Instantiate(explosionEffect, transform.position, Quaternion.identity);
        }
        Destroy(gameObject);
        }

    }
}
