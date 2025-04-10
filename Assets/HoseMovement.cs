using UnityEngine;
using UnityEngine.InputSystem;

public class HoseMovement : MonoBehaviour
{
    // Rotation speed (how much it rotates per second)
    public float rotationSpeed = 50f;

    // Reference to the water GameObject
    public GameObject waterObject;
    
    public InputActionReference HoseAimInput, HoseFireInput;

    // Update is called once per frame
    void Update()
    {
        var hoseAim = HoseAimInput.action.ReadValue<Vector2>();
        var hoseFire = HoseFireInput.action.IsPressed();
        
        // Get the current rotation of the gameObject (only the Z-axis will be affected)
        Vector3 currentRotation = transform.rotation.eulerAngles;

        // Check if the K key is being held down to rotate left (decrease Z-axis rotation)
        if (hoseAim.x < 0)
        {
            // Rotate left around the Z-axis
            transform.rotation = Quaternion.Euler(currentRotation.x, currentRotation.y, currentRotation.z - rotationSpeed * Time.deltaTime);
        }
        // Check if the L key is being held down to rotate right (increase Z-axis rotation)
        else if (hoseAim.x > 0)
        {
            // Rotate right around the Z-axis
            transform.rotation = Quaternion.Euler(currentRotation.x, currentRotation.y, currentRotation.z + rotationSpeed * Time.deltaTime);
        }

        // When the F key is pressed, activate the water GameObject
        if (hoseFire)
        {
            waterObject.SetActive(true);
        }

        // When the F key is released, deactivate the water GameObject
        if (!hoseFire)
        {
            waterObject.SetActive(false);
        }
    }
}
