// https://docs.unity3d.com/Packages/com.unity.inputsystem@1.12/manual/Layouts.html
// https://docs.unity3d.com/Packages/com.unity.inputsystem@1.12/manual/HID.html
// https://discussions.unity.com/t/custom-hid-layout-find-which-bits-represent-which-buttons/841149

using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.Utilities;

[InputControlLayout(stateType = typeof(WheelDeviceState))]
public class WheelDevice : InputDevice
{
#if UNITY_EDITOR
	[InitializeOnLoadMethod]
#endif
	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
	public static void Register()
	{
		Debug.Log("registering wheel device");

		InputSystem.RegisterProcessor<RemapProcessor>();

		InputSystem.RegisterLayout<WheelDevice>(
			matches: new InputDeviceMatcher()
				.WithInterface("HID")
				.WithCapability("vendorId", 0x46D)
				.WithCapability("productId", 0xC262));
	}
}

public struct WheelDeviceState : IInputStateTypeInfo
{
	public FourCC format => new('H', 'I', 'D');

	[InputControl(name = "Steer", layout = "Axis", offset = 4)]
	public ushort Steer;

	[InputControl(name = "Clutch", layout = "Axis", offset = 8)]
	public byte Clutch;

	[InputControl(name = "Brake", layout = "Axis", offset = 6)]
	[InputControl(name = "Gas", layout = "Axis", offset = 6)]
	public byte BrakeGas;

	[InputControl(name = "Gear 1", layout = "Button", offset = 3, bit = 7)]
	[InputControl(name = "Gear 2", layout = "Button", offset = 3, bit = 6)]
	[InputControl(name = "Gear 3", layout = "Button", offset = 3, bit = 5)]
	[InputControl(name = "Gear 4", layout = "Button", offset = 3, bit = 4)]
	[InputControl(name = "Gear 5", layout = "Button", offset = 3, bit = 3)]
	[InputControl(name = "Gear 6", layout = "Button", offset = 3, bit = 2)]
	public byte Gear;
}

public class RemapProcessor : InputProcessor<float>
{
	public float FromA, FromB, ToA, ToB;

	public override float Process(float value, InputControl control) => Mathf.Lerp(ToA, ToB, Mathf.InverseLerp(FromA, FromB, value));
}
