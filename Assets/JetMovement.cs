using UnityEngine;
using UnityEngine.InputSystem;

public class JetMovement : MonoBehaviour
{
	public Rigidbody Body;

	public float ThrottleAmount, LiftAmount;

	public InputActionReference Throttle, Stick;

	private void Update()
	{
		var throttle = Throttle.action.ReadValue<float>();
		var stick = Stick.action.ReadValue<Vector2>();
		Debug.Log($"throttle = {throttle}, stick = {stick}");

		{
			Body.AddForce(transform.forward * throttle * ThrottleAmount);
		}

		if (false)
		{
			var forwardVelocity = Vector3.Project(Body.GetPointVelocity(transform.position), transform.forward);
			var liftForce = Vector3.Cross(forwardVelocity, transform.right) * LiftAmount;
			Body.AddForceAtPosition(liftForce, transform.position);
			Debug.Log($"forward = {forwardVelocity.magnitude}\t|\tlift = {liftForce.magnitude}");
		}
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
