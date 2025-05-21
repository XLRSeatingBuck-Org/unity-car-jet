using UnityEngine;

public class ParticleTest : MonoBehaviour
{
    // void Update()
    // {
        // transform.position += Vector3.left * Time.deltaTime;
    // }
    
    void FixedUpdate()
    {
        GetComponent<Rigidbody>().AddForce(Vector3.left);
    }
}
