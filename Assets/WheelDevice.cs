// https://docs.unity3d.com/Packages/com.unity.inputsystem@1.12/manual/Layouts.html
// https://docs.unity3d.com/Packages/com.unity.inputsystem@1.12/manual/HID.html
// https://discussions.unity.com/t/custom-hid-layout-find-which-bits-represent-which-buttons/841149

using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.Utilities;

public struct WheelDeviceState : IInputStateTypeInfo
{
	public FourCC format => new("G920");

	[InputControl(name = "Steer", layout = "Axis", offset = 0)]
	public short Steer;

	[InputControl(name = "Gear 1", layout = "Button", offset = 0, bit = 2)]
	[InputControl(name = "Gear 2", layout = "Button", offset = 0, bit = 3)]
	[InputControl(name = "Gear 3", layout = "Button", offset = 0, bit = 4)]
	[InputControl(name = "Gear 4", layout = "Button", offset = 0, bit = 5)]
	[InputControl(name = "Gear 5", layout = "Button", offset = 0, bit = 6)]
	[InputControl(name = "Gear 6", layout = "Button", offset = 0, bit = 7)]
	public byte Gear;
}

[InputControlLayout(stateType = typeof(WheelDeviceState))]
public class WheelDevice : Gamepad
{
#if UNITY_EDITOR
	[InitializeOnLoadMethod]
#endif
	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
	public static void Register()
	{
		Debug.Log("registering wheel device");

		InputSystem.RegisterLayout<WheelDevice>(
			matches: new InputDeviceMatcher()
				.WithInterface("HID")
				.WithCapability("vendorId", 0x46D)
				.WithCapability("productId", 0xC262));
	}
}
