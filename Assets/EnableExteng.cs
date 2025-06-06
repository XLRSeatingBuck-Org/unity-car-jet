using System;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// turn on/off fire foam with button press
/// </summary>
public class EnableExteng : MonoBehaviour
{
    // Array to hold references to 8 GameObjects
    public GameObject[] particleObjects = new GameObject[8];

    public InputActionReference UseInput;

    private void Awake()
    {
        foreach (var particleObject in particleObjects)
        {
            particleObject.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Enable all objects when 'P' is pressed down
        if (UseInput.action.WasPressedThisFrame())
        {
            foreach (GameObject obj in particleObjects)
            {
                if (obj != null)
                {
                    obj.SetActive(true);
                }
            }
        }

        // Disable all objects when 'P' is released
        if (UseInput.action.WasReleasedThisFrame())
        {
            foreach (GameObject obj in particleObjects)
            {
                if (obj != null)
                {
                    obj.SetActive(false);
                }
            }
        }
    }
}