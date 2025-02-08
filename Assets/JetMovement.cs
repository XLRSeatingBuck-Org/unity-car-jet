using UnityEngine;
using UnityEngine.InputSystem;

public class JetMovement : MonoBehaviour
{
	public Rigidbody Body;

	public float ThrottleAmount, TurnAmount, LiftAmount;

	public InputActionReference Throttle, Pitch, Yaw, Roll;

	private void Update()
	{
		{
			var throttle = Throttle.action.ReadValue<float>();
			var pitch = Pitch.action.ReadValue<float>();
			var yaw = Yaw.action.ReadValue<float>();
			var roll = Roll.action.ReadValue<float>();

			Body.AddForce(transform.forward * throttle * ThrottleAmount);
			Body.AddTorque(transform.right * pitch * TurnAmount);
			Body.AddTorque(transform.up * yaw * TurnAmount);
			Body.AddTorque(transform.forward * roll * TurnAmount);
		}

		{
			var forwardVelocity = Vector3.Project(Body.GetPointVelocity(transform.position), transform.forward);
			var liftForce = Vector3.Cross(forwardVelocity, transform.right) * LiftAmount;
			Body.AddForceAtPosition(liftForce, transform.position);
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

	private void OnGUI()
	{
		var throttle = Throttle.action.ReadValue<float>();
		var pitch = Pitch.action.ReadValue<float>();
		var yaw = Yaw.action.ReadValue<float>();
		var roll = Roll.action.ReadValue<float>();
		GUILayout.Label($"throttle = {throttle} \t pitch = {pitch} \t yaw = {yaw} \t roll = {roll}");

		var forwardVelocity = Vector3.Project(Body.GetPointVelocity(transform.position), transform.forward);
		var liftForce = Vector3.Cross(forwardVelocity, transform.right) * LiftAmount;
		GUILayout.Label($"forward = {forwardVelocity.magnitude} \t lift = {liftForce.magnitude}");
	}
}
