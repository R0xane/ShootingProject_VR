using UnityEngine;

public class DestroyingEffects : MonoBehaviour
{
    // Public variable to set the time in seconds before the object is destroyed.
    // Making it public allows you to adjust it in the Unity Inspector.
    public float lifetime = 5.0f; 

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // The static Destroy method is called to destroy an object.
        // The second parameter is the delay (in seconds) before the object is destroyed.
        Destroy(gameObject, lifetime);
    }

    // Update is called once per frame
    void Update()
    {
        // No code needed in Update for simple timed destruction.
    }
}