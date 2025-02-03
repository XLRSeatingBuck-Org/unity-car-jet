using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;

public class CarMovement : MonoBehaviour
{
	public InputActionReference GasInput, BrakeInput, SteerInput;

	public WheelCollider LeftFront, RightFront, LeftBack, RightBack;

	public float GasTorque, BrakeTorque, SteerAngle;

	private void Awake()
	{
		// pair the custom device with the default user to so that inputs are registered
		// theres probably a better way of doing this :P
		InputUser.PerformPairingWithDevice(InputSystem.GetDevice("LogitechG920"), InputUser.all[0]);
	}

	private void FixedUpdate()
	{
		var gas = GasInput.action.ReadValue<float>();
		var brake = BrakeInput.action.ReadValue<float>();
		var steer = SteerInput.action.ReadValue<float>();

		Debug.Log($"gas = {gas}, brake = {brake}, steer = {steer}");

		LeftBack.motorTorque = gas * GasTorque;
		LeftBack.brakeTorque = brake * BrakeTorque;
		RightBack.motorTorque = gas * GasTorque;
		RightBack.brakeTorque = brake * BrakeTorque;
		LeftFront.brakeTorque = brake * BrakeTorque;
		RightFront.brakeTorque = brake * BrakeTorque;

		RightFront.steerAngle = steer * SteerAngle;
		LeftFront.steerAngle = steer * SteerAngle;
	}
}
