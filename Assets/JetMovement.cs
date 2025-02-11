using UnityEngine;
using UnityEngine.InputSystem;

public class JetMovement : MonoBehaviour
{
	public Rigidbody Body;

	public float ThrottleAmount, TurnAmount, LiftAmount;

	public InputActionReference Throttle, Pitch, Yaw, Roll;

	private void FixedUpdate()
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
			var localForwardSpeed = Vector3.Project(Body.linearVelocity, transform.forward).magnitude;
			var liftForce = transform.up * Mathf.Max(0, localForwardSpeed * LiftAmount);
			Body.AddForce(liftForce);
		}
	}

	private void OnGUI()
	{
		var throttle = Throttle.action.ReadValue<float>();
		var pitch = Pitch.action.ReadValue<float>();
		var yaw = Yaw.action.ReadValue<float>();
		var roll = Roll.action.ReadValue<float>();
		GUILayout.Label($"throttle = {throttle}\npitch = {pitch}\nyaw = {yaw}\nroll = {roll}");

		var localForwardSpeed = Vector3.Project(Body.linearVelocity, transform.forward).magnitude;
		var liftForce = transform.up * Mathf.Max(0, localForwardSpeed * LiftAmount);
		GUILayout.Label($"forward = {localForwardSpeed}\nlift = {liftForce.magnitude}");

		GUILayout.Label($"velocity = {Body.linearVelocity} / {transform.InverseTransformDirection(Body.linearVelocity)}\n" +
			$"angular velocity = {Body.angularVelocity} / {transform.InverseTransformDirection(Body.angularVelocity)}");
	}
}
