using UnityEngine;

public class BringDownFire : MonoBehaviour
{
    [Header("Assign the Water GameObject here")]
    public GameObject waterObject; // Drag your Water GameObject here

    private GameObject embers;
    private GameObject fire;
    private GameObject smokeEffect;

    private Vector3 initialScale;
    private Vector3 embersInitialScale;
    private Vector3 fireInitialScale;
    private Vector3 smokeEffectInitialScale;

    private float wildfireScaleRate = 0.1f;
    private float otherScaleRate = 0.1f;
    private float growthTimer = 0f;
    private float growthInterval = 6f;
    private float growthAmount = 0.1f;

    private float timer = 0f;
    private int waterContactCount = 0;

    private const float maxScale = 2f;

    void Start()
    {
        embers = transform.Find("Embers")?.gameObject;
        fire = transform.Find("Fire")?.gameObject;
        smokeEffect = transform.Find("SmokeEffect")?.gameObject;

        if (embers == null || fire == null || smokeEffect == null)
        {
            Debug.LogError("Missing child objects (Embers, Fire, SmokeEffect)");
            return;
        }

        initialScale = transform.localScale;
        embersInitialScale = embers.transform.localScale;
        fireInitialScale = fire.transform.localScale;
        smokeEffectInitialScale = smokeEffect.transform.localScale;
    }

    void Update()
    {
        // Reset contact count if water is disabled
        if (waterObject != null && !waterObject.activeInHierarchy)
        {
            waterContactCount = 0;
        }

        if (IsInWater())
        {
            timer += Time.deltaTime;

            // Clamp t values so we eventually reach zero
            float wildfireT = Mathf.Clamp01(timer * wildfireScaleRate);
            float otherT = Mathf.Clamp01(timer * otherScaleRate);

            transform.localScale = Vector3.Lerp(initialScale, Vector3.zero, wildfireT);
            embers.transform.localScale = Vector3.Lerp(embersInitialScale, Vector3.zero, otherT);
            fire.transform.localScale = Vector3.Lerp(fireInitialScale, Vector3.zero, otherT);
            smokeEffect.transform.localScale = Vector3.Lerp(smokeEffectInitialScale, Vector3.zero, otherT);

            // Fully extinguish fire when it's small enough
            if (fire.transform.localScale.x <= 0.53f)
            {
                GetComponent<FireExtinguishTracker>().Extinguished = true;
                gameObject.SetActive(false);
            }
        }
        else
        {
            // Grow fire if not in water, but stop at maxScale
            growthTimer += Time.deltaTime;

            if (growthTimer >= growthInterval)
            {
                if (transform.localScale.x < maxScale)
                {
                    Vector3 growth = new Vector3(growthAmount, growthAmount, growthAmount);

                    transform.localScale += growth;
                    embers.transform.localScale += growth;
                    fire.transform.localScale += growth;
                    smokeEffect.transform.localScale += growth;

                    // Clamp to max scale
                    transform.localScale = Vector3.Min(transform.localScale, Vector3.one * maxScale);
                    embers.transform.localScale = Vector3.Min(embers.transform.localScale, Vector3.one * maxScale);
                    fire.transform.localScale = Vector3.Min(fire.transform.localScale, Vector3.one * maxScale);
                    smokeEffect.transform.localScale = Vector3.Min(smokeEffect.transform.localScale, Vector3.one * maxScale);

                    // Save new scales for future shrink lerp
                    initialScale = transform.localScale;
                    embersInitialScale = embers.transform.localScale;
                    fireInitialScale = fire.transform.localScale;
                    smokeEffectInitialScale = smokeEffect.transform.localScale;
                }
                
                // lose if we've spread too much
                if (transform.localScale == Vector3.one * maxScale)
                {
                    ExperienceDirector.Instance.OnLose(ExperienceDirector.LoseType.Spread);
                }

                growthTimer = 0f;
            }

            timer = 0f; // Reset shrinking timer
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Water"))
        {
            waterContactCount++;

            // Save scale at time of contact
            initialScale = transform.localScale;
            embersInitialScale = embers.transform.localScale;
            fireInitialScale = fire.transform.localScale;
            smokeEffectInitialScale = smokeEffect.transform.localScale;

            timer = 0f;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Water"))
        {
            waterContactCount = Mathf.Max(0, waterContactCount - 1);
        }
    }

    private bool IsInWater()
    {
        return waterContactCount > 0;
    }

    public void ResetFire()
    {
        gameObject.SetActive(true);

        transform.localScale = initialScale;
        embers.transform.localScale = embersInitialScale;
        fire.transform.localScale = fireInitialScale;
        smokeEffect.transform.localScale = smokeEffectInitialScale;

        timer = 0f;
        growthTimer = 0f;
        waterContactCount = 0;

        GetComponent<FireExtinguishTracker>().Extinguished = false;
    }
}
