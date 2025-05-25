using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// stores if a fire is extinguished
/// </summary>
public class FireExtinguishTracker : MonoBehaviour
{
    private static List<FireExtinguishTracker> _instances = new();
    
    private bool _extinguished;
    public bool Extinguished
    {
        set
        {
            _extinguished = value;
            Debug.Log($"{_instances.Count(x => x._extinguished)} out of {_instances.Count} extinguished");
            if (_instances.All(x => x._extinguished))
            {
                ExperienceDirector.Instance.OnFireExtinguished();
            }
        }
    }

    private void Awake()
    {
        _instances.Add(this);
    }

    private void OnDestroy()
    {
        _instances.Remove(this);
    }
}