using System.Linq;
using UnityEngine;

/// <summary>
/// increase all fires in a wildfire
/// </summary>
public class IncreaseFire : MonoBehaviour
{
    public GameObject[] wildfires; // Assign these in the Inspector
    private float timer = 0f;
    public float scaleIncreaseAmount = 0.4f;
    public float scaleInterval = 3f;
    public float maxScale = 80f;

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= scaleInterval)
        {
            foreach (GameObject wildfire in wildfires)
            {
                // Scale wildfire only if not at max
                if (!ReachedMaxScale(wildfire.transform.localScale))
                {
                    wildfire.transform.localScale = IncreaseAndClamp(wildfire.transform.localScale);
                }
                else
                {
                    // if any fire gets too big, lose
                    ExperienceDirector.Instance.OnLose(ExperienceDirector.LoseType.Spread);
                    enabled = false;
                    return;
                }

                // Scale each child if not at max
                for (int i = 0; i < wildfire.transform.childCount; i++)
                {
                    Transform child = wildfire.transform.GetChild(i);
                    if (!ReachedMaxScale(child.localScale))
                    {
                        child.localScale = IncreaseAndClamp(child.localScale);
                    }
                }
            }

            timer = 0f; // Reset the timer
        }
    }

    // Increases the scale by the specified amount and clamps it at maxScale
    Vector3 IncreaseAndClamp(Vector3 currentScale)
    {
        float x = Mathf.Min(currentScale.x + scaleIncreaseAmount, maxScale);
        float y = Mathf.Min(currentScale.y + scaleIncreaseAmount, maxScale);
        float z = Mathf.Min(currentScale.z + scaleIncreaseAmount, maxScale);
        return new Vector3(x, y, z);
    }

    // Checks if all scale components have reached maxScale
    bool ReachedMaxScale(Vector3 scale)
    {
        return scale.x >= maxScale && scale.y >= maxScale && scale.z >= maxScale;
    }
}