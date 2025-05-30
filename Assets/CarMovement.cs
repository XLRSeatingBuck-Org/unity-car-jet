using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// handles all input/movement/physics with the car
/// </summary>
public class CarMovement : MonoBehaviour
{
	public InputActionReference GasInput, BrakeInput, SteerInput, GearRInput;

	public Rigidbody Body;

	public WheelCollider LeftFront, RightFront, LeftBack, RightBack;

	public float GasTorque, BrakeTorque, SteerAngle;

	private void FixedUpdate()
	{
		// get input
		var gas = GasInput.action.ReadValue<float>();
		var brake = BrakeInput.action.ReadValue<float>();
		var steer = SteerInput.action.ReadValue<float>();
		var reverse = GearRInput.action.IsPressed();

		gas *= reverse ? -1 : 1;

		// apply forces
		LeftBack.motorTorque = gas * GasTorque;
		LeftBack.brakeTorque = brake * BrakeTorque;
		RightBack.motorTorque = gas * GasTorque;
		RightBack.brakeTorque = brake * BrakeTorque;
		LeftFront.brakeTorque = brake * BrakeTorque;
		RightFront.brakeTorque = brake * BrakeTorque;

		RightFront.steerAngle = steer * SteerAngle;
		LeftFront.steerAngle = steer * SteerAngle;
	}

	#if UNITY_EDITOR
	private void OnGUI()
	{
		var gas = GasInput.action.ReadValue<float>();
		var brake = BrakeInput.action.ReadValue<float>();
		var steer = SteerInput.action.ReadValue<float>();
		var reverse = GearRInput.action.IsPressed();
		GUILayout.Label($"gas = {gas}\nbrake = {brake}\nsteer = {steer}\nreverse = {reverse}");

		GUILayout.Label($"wheel rpm = \n{LeftFront.rpm}\t{RightFront.rpm}\n{LeftBack.rpm}\t{RightBack.rpm}");

		GUILayout.Label($"velocity = {Body.linearVelocity} / {transform.InverseTransformDirection(Body.linearVelocity)}\n" + 
		                $"angular velocity = {Body.angularVelocity * Mathf.Rad2Deg} / {transform.InverseTransformDirection(Body.angularVelocity) * Mathf.Rad2Deg}");
	}
	#endif
}
