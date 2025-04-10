using UnityEngine;

public class BringDownFire : MonoBehaviour
{
    // References to the child game objects
    private GameObject embers;
    private GameObject fire;
    private GameObject smokeEffect;

    // Initial scale values
    private Vector3 initialScale;
    private Vector3 embersInitialScale;
    private Vector3 fireInitialScale;
    private Vector3 smokeEffectInitialScale;

    // Scale reduction rates
    private float wildfireScaleRate = 0.2f;
    private float otherScaleRate = 0.1f;

    // Flag to check if collision with Water has happened
    private bool isInWater = false;

    // Timer to track how much time has passed
    private float timer = 0f;

    void Start()
    {
        // Find the child objects (assuming they are named exactly as "Embers", "Fire", "SmokeEffect")
        embers = transform.Find("Embers")?.gameObject;
        fire = transform.Find("Fire")?.gameObject;
        smokeEffect = transform.Find("SmokeEffect")?.gameObject;

        // Check if any of the child objects are not found and log a message
        if (embers == null || fire == null || smokeEffect == null)
        {
            Debug.LogError("One or more child objects (Embers, Fire, SmokeEffect) are missing or misnamed. Please check the Unity hierarchy.");
            return;
        }

        // Store the initial scale of each game object
        initialScale = transform.localScale;
        embersInitialScale = embers.transform.localScale;
        fireInitialScale = fire.transform.localScale;
        smokeEffectInitialScale = smokeEffect.transform.localScale;
    }

    void Update()
    {
        if (isInWater)
        {
            // Increment timer
            timer += Time.deltaTime;

            // Reduce scale of WildFire at a rate of 0.2 per second
            if (transform.localScale.x > 0 && transform.localScale.y > 0 && transform.localScale.z > 0)
            {
                transform.localScale = Vector3.Lerp(initialScale, Vector3.zero, timer * wildfireScaleRate);
            }

            // Reduce scale of Embers, Fire, and SmokeEffect at a rate of 0.1 per second
            if (embers.transform.localScale.x > 0 && embers.transform.localScale.y > 0 && embers.transform.localScale.z > 0)
            {
                embers.transform.localScale = Vector3.Lerp(embersInitialScale, Vector3.zero, timer * otherScaleRate);
            }

            if (fire.transform.localScale.x > 0 && fire.transform.localScale.y > 0 && fire.transform.localScale.z > 0)
            {
                fire.transform.localScale = Vector3.Lerp(fireInitialScale, Vector3.zero, timer * otherScaleRate);
            }

            if (smokeEffect.transform.localScale.x > 0 && smokeEffect.transform.localScale.y > 0 && smokeEffect.transform.localScale.z > 0)
            {
                smokeEffect.transform.localScale = Vector3.Lerp(smokeEffectInitialScale, Vector3.zero, timer * otherScaleRate);
            }
        }
    }

    // This method is called when the WildFire object enters a trigger collider
    private void OnTriggerEnter(Collider other)
    {
        // Check if the trigger object is named "Water"
        if (other.CompareTag("Water"))
        {
            isInWater = true;
        }
    }

    // Optionally, we can reset the scale if needed (for example, when the fire is "extinguished")
    public void ResetFire()
    {
        transform.localScale = initialScale;
        embers.transform.localScale = embersInitialScale;
        fire.transform.localScale = fireInitialScale;
        smokeEffect.transform.localScale = smokeEffectInitialScale;
        timer = 0f;
        isInWater = false;
    }
}
