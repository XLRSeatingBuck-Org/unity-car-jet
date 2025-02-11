using UnityEngine;
using UnityEngine.InputSystem;

public class CarMovement : MonoBehaviour
{
	public InputActionReference GasInput, BrakeInput, SteerInput;

	public Rigidbody Body;

	public WheelCollider LeftFront, RightFront, LeftBack, RightBack;

	public float GasTorque, BrakeTorque, SteerAngle;

	private void FixedUpdate()
	{
		var gas = GasInput.action.ReadValue<float>();
		var brake = BrakeInput.action.ReadValue<float>();
		var steer = SteerInput.action.ReadValue<float>();

		LeftBack.motorTorque = gas * GasTorque;
		LeftBack.brakeTorque = brake * BrakeTorque;
		RightBack.motorTorque = gas * GasTorque;
		RightBack.brakeTorque = brake * BrakeTorque;
		LeftFront.brakeTorque = brake * BrakeTorque;
		RightFront.brakeTorque = brake * BrakeTorque;

		RightFront.steerAngle = steer * SteerAngle;
		LeftFront.steerAngle = steer * SteerAngle;
	}

	private void OnGUI()
	{
		var gas = GasInput.action.ReadValue<float>();
		var brake = BrakeInput.action.ReadValue<float>();
		var steer = SteerInput.action.ReadValue<float>();
		GUILayout.Label($"gas = {gas}\nbrake = {brake}\nsteer = {steer}");

		GUILayout.Label($"wheel rpm = \n{LeftFront.rpm}\t{RightFront.rpm}\n{LeftBack.rpm}\t{RightBack.rpm}");

		GUILayout.Label($"velocity = {Body.linearVelocity} / {transform.InverseTransformDirection(Body.linearVelocity)}\n" +
			$"angular velocity = {Body.angularVelocity} / {transform.InverseTransformDirection(Body.angularVelocity)}");
	}
}
