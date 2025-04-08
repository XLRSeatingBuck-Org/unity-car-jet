using UnityEngine;

public class HoseMovement : MonoBehaviour
{
    // Rotation speed (how much it rotates per second)
    public float rotationSpeed = 50f;

    // Update is called once per frame
    void Update()
    {
        // Get the current rotation of the gameObject (only the Z-axis will be affected)
        Vector3 currentRotation = transform.rotation.eulerAngles;

        // Check if the K key is being held down to rotate left (decrease Z-axis rotation)
        if (Input.GetKey(KeyCode.K))
        {
            // Rotate left around the Z-axis
            transform.rotation = Quaternion.Euler(currentRotation.x, currentRotation.y, currentRotation.z - rotationSpeed * Time.deltaTime);
        }
        // Check if the L key is being held down to rotate right (increase Z-axis rotation)
        else if (Input.GetKey(KeyCode.L))
        {
            // Rotate right around the Z-axis
            transform.rotation = Quaternion.Euler(currentRotation.x, currentRotation.y, currentRotation.z + rotationSpeed * Time.deltaTime);
        }
    }
}
