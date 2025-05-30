using UnityEngine;
using System.Collections;

/// <summary>
/// handles collision between plane foam and fire, extinguishing as necessary
/// </summary>
public class GasCollision : MonoBehaviour
{
    void OnParticleCollision(GameObject other)
    {
        if (other.CompareTag("Fire"))
        {
            Debug.Log("Collision detected with fire!");

            // Scale the fire GameObject to (0, 0, 0)
            other.transform.localScale = Vector3.zero;

            // Loop through all children of Fire
            foreach (Transform child in other.transform)
            {
                // Skip scaling if the child is SmokeEffect
                if (child.name == "SmokeEffect")
                {
                    // Get the ParticleSystem and start adjusting emission
                    ParticleSystem smoke = child.GetComponent<ParticleSystem>();
                    if (smoke != null)
                    {
                        StartCoroutine(AdjustSmokeEmission(smoke));
                    }
                    continue;
                }

                // Scale all other children to (0, 0, 0)
                child.localScale = Vector3.zero;
            }
            
            // tell system we are extinguished
            other.GetComponent<FireExtinguishTracker>().Extinguished = true;
        }
    }

    // Coroutine to gradually reduce the emission rate
    private IEnumerator AdjustSmokeEmission(ParticleSystem smoke)
    {
        var emission = smoke.emission;
        float currentRate = 100f;
        emission.rateOverTime = currentRate;

        while (currentRate > 0)
        {
            yield return new WaitForSeconds(0.5f);
            currentRate -= 10f;
            emission.rateOverTime = Mathf.Max(0, currentRate);
        }
    }
}