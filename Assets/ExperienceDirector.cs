using System;
using UnityEngine;

/// <summary>
/// controls the main experience (moving beacons around, displaying text maybe, etc)
/// </summary>
public class ExperienceDirector : MonoBehaviour
{
    public GameObject fireBeacon, homeBeacon;

    private void Awake()
    {
        fireBeacon.SetActive(true);
        homeBeacon.SetActive(false);
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
