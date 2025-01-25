// https://discussions.unity.com/t/custom-hid-layout-find-which-bits-represent-which-buttons/841149


using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.HID;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.XInput;

#if UNITY_EDITOR
[InitializeOnLoad] // Make sure static constructor is called during startup.
#endif
public class RegisterG920Layout
{
	static RegisterG920Layout()
	{
		Debug.Log("registering g920 layout");
		InputSystem.RegisterLayout<XInputController>(
			matches: new InputDeviceMatcher()
				.WithInterface("HID")
				.WithCapability("vendorId", 0x46D)
				.WithCapability("productId", 0xC262));
	}
}
