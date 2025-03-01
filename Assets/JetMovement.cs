using Bhaptics.SDK2;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class JetMovement : MonoBehaviour
{
	public Rigidbody Body;

	public float ThrottleAmount, TurnAmount, LiftAmount;

	public InputActionReference Throttle, Pitch, Yaw, Roll;

	public AnimationCurve SpeedToMotorPowerCurve, ImpulseToMotorPowerCurve;
	public int ImpulseMotorDuration;

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

		var localForwardSpeed = Vector3.Project(Body.linearVelocity, transform.forward).magnitude;
		{
			var liftForce = transform.up * Mathf.Max(0, localForwardSpeed * LiftAmount);
			Body.AddForce(liftForce);
		}

		{
			var motorPower = Mathf.RoundToInt(SpeedToMotorPowerCurve.Evaluate(localForwardSpeed));
			// docs recommend 100 ms or more
			BhapticsLibrary.PlayMotors((int)PositionType.GloveL, Enumerable.Repeat(motorPower, 6).ToArray(), 100);
			BhapticsLibrary.PlayMotors((int)PositionType.GloveR, Enumerable.Repeat(motorPower, 6).ToArray(), 100);
			BhapticsLibrary.PlayMotors((int)PositionType.Vest, Enumerable.Repeat(motorPower, 32).ToArray(), 100);
		}
	}

	private void OnCollisionEnter(Collision other)
	{
		var motorPower = Mathf.RoundToInt(ImpulseToMotorPowerCurve.Evaluate(other.impulse.magnitude));
		Debug.Log($"impulse motor power = {motorPower}");
		BhapticsLibrary.PlayMotors((int)PositionType.GloveL, Enumerable.Repeat(motorPower, 6).ToArray(), ImpulseMotorDuration);
		BhapticsLibrary.PlayMotors((int)PositionType.GloveR, Enumerable.Repeat(motorPower, 6).ToArray(), ImpulseMotorDuration);
		BhapticsLibrary.PlayMotors((int)PositionType.Vest, Enumerable.Repeat(motorPower, 32).ToArray(), ImpulseMotorDuration);


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

		var motorPower = Mathf.RoundToInt(SpeedToMotorPowerCurve.Evaluate(localForwardSpeed));
		GUILayout.Label($"speed motor power = {motorPower}");
	}
}
