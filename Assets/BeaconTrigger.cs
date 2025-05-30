using System;
using UnityEngine;

/// <summary>
/// signal when the player enters or exits the range of this beacon
/// </summary>
public class BeaconTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponentInParent<Rigidbody>()?.CompareTag("Player") ?? false)
            ExperienceDirector.Instance.OnEnterBeacon(gameObject.transform.parent.gameObject);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponentInParent<Rigidbody>()?.CompareTag("Player") ?? false)
            ExperienceDirector.Instance.OnExitBeacon(gameObject.transform.parent.gameObject);
    }
}