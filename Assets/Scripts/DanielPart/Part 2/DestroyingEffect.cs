using UnityEngine;
using System.Collections;

public class DestroyingEffects : MonoBehaviour
{

    public float lifetime = 5.0f;

    void OnEnable()
    {

        StartCoroutine(DisableAfterDelay());
    }

    IEnumerator DisableAfterDelay()
    {

        yield return new WaitForSeconds(lifetime);
        gameObject.SetActive(false);
    }
    

    void OnDisable()
    {
        StopAllCoroutines();
    }
}