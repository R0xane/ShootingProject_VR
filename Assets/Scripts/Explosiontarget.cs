using UnityEngine;

public class Explosiontarget : MonoBehaviour
{
    public ParticleSystem explosionEffect;

    // Identifiant de la position assignée
    public int positionIndex;

    public void ResetState()
    {
        // reset si nécessaire
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.name.Contains("Bullet"))
        {
            if (explosionEffect != null)
                Instantiate(explosionEffect, transform.position, Quaternion.identity);

            // Retour au pool + libération de la position
            TargetPool.Instance.ReturnTarget(this);
        }
    }
}
