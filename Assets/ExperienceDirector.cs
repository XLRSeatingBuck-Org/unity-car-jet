using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// controls the main experience (moving beacons around, displaying text maybe, etc)
/// </summary>
public class ExperienceDirector : MonoBehaviour
{
    public GameObject fireBeacon, homeBeacon;
    public CanvasGroup loadingFader;

    private void Awake()
    {
        fireBeacon.SetActive(true);
        homeBeacon.SetActive(false);
        loadingFader.alpha = 1;
        Time.timeScale = 0;
    }

    private IEnumerator Start()
    {
        var osmLoader = FindAnyObjectByType<OsmLoader>();
        yield return new WaitUntil(() => osmLoader.Loaded);
        Debug.Log("loaded!");
        loadingFader.alpha = 0;
        Time.timeScale = 1;
    }

    [ContextMenu("test extinguish")]
    public void OnFireExtinguished()
    {
        fireBeacon.SetActive(false);
        homeBeacon.SetActive(true);
        // TODO: maybe put text or something
        // TODO: sound
    }
}
