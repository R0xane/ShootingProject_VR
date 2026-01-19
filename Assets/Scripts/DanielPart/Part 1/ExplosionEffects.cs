using UnityEngine;

public class Exp : MonoBehaviour
{
    public GameObject explosionEffect;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Bullet")
        {
            // Instancier l'effet d'explosion à la position de l'objet actuel
            Instantiate(explosionEffect, transform.position, transform.rotation);

            // Détruire l'objet actuel
            Destroy(gameObject);
        }
    }

}
