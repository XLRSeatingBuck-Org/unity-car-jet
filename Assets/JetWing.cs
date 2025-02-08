using UnityEngine;

/// <summary>
/// provides lift
/// </summary>
public class JetWing : MonoBehaviour
{
	public Rigidbody Body;

	public float LiftAmount;

	private void Update()
	{
		var forwardVelocity = Vector3.Project(Body.GetPointVelocity(transform.position), transform.forward);
		var liftForce = Vector3.Cross(forwardVelocity, transform.right) * LiftAmount;
		Body.AddForceAtPosition(liftForce, transform.position);
		Debug.Log($"forward = {forwardVelocity.magnitude}\t|\tlift = {liftForce.magnitude}");
	}

	private void OnDrawGizmos()
	{
		var forwardVelocity = Vector3.Project(Body.GetPointVelocity(transform.position), transform.forward);
		var liftForce = Vector3.Cross(forwardVelocity, transform.right) * LiftAmount;
		Gizmos.color = Color.red;
		Gizmos.DrawLine(transform.position, transform.position + forwardVelocity);
		Gizmos.color = Color.green;
		Gizmos.DrawLine(transform.position, transform.position + liftForce);
	}
}
