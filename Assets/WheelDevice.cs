// https://docs.unity3d.com/Packages/com.unity.inputsystem@1.12/manual/HID.html
// https://discussions.unity.com/t/custom-hid-layout-find-which-bits-represent-which-buttons/841149
// https://docs.unity3d.com/Packages/com.unity.inputsystem@1.12/manual/Layouts.html

/*
 * layout
 * byte 0: report id
 * byte 1: abxy, dpad
 * byte 2: other buttons
 * byte 3: gear shift (gear 1-6 = bit 7-2)
 * byte 4-5: wheel
 * byte 6: gas (1 is up)
 * byte 7: brake
 * byte 8: clutch
 * byte 9: ?
 */

using System.Runtime.InteropServices;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.Utilities;


// We receive data as raw HID input reports. This struct
// describes the raw binary format of such a report.
[StructLayout(LayoutKind.Explicit, Size = 10)]
public struct LogitechG920InputReport : IInputStateTypeInfo
{
    // Because all HID input reports are tagged with the 'HID ' FourCC,
    // this is the format we need to use for this state struct.
    public FourCC format => new FourCC('H', 'I', 'D');

    // HID input reports can start with an 8-bit report ID. It depends on the device
    // whether this is present or not. On the PS4 DualShock controller, it is
    // present. We don't really need to add the field, but let's do so for the sake of
    // completeness. This can also help with debugging.
    [FieldOffset(0)] public byte reportId;

    [InputControl(layout = "Button", bit = 0)]
    [InputControl(layout = "Button", bit = 1)]
    [InputControl(layout = "Button", bit = 2)]
    [InputControl(layout = "Button", bit = 3)]
    [InputControl(layout = "Dpad", bit = 4, sizeInBits = 4, defaultState = 0b1000)]
    [InputControl(layout = "Button", bit = 8)]
    [InputControl(layout = "Button", bit = 9)]
    [InputControl(layout = "Button", bit = 10)]
    [InputControl(layout = "Button", bit = 11)]
    [InputControl(layout = "Button", bit = 12)]
    [InputControl(layout = "Button", bit = 13)]
    [InputControl(layout = "Button", bit = 14)]
    [InputControl(layout = "Button", bit = 15)]
    [FieldOffset(1)] public ushort buttons;

    [FieldOffset(3)] public byte gear;

    [FieldOffset(4)] public ushort wheel;

    [FieldOffset(6)] public byte gas;
    [FieldOffset(7)] public byte brake;
    [FieldOffset(8)] public byte clutch;

    // makes it show up in input debugger
    [InputControl(layout = "Button")]
    [FieldOffset(9)] public byte padding;
}








// Using InputControlLayoutAttribute, we tell the system about the state
// struct we created, which includes where to find all the InputControl
// attributes that we placed on there. This is how the Input System knows
// what controls to create and how to configure them.
[InputControlLayout(stateType = typeof(LogitechG920InputReport))]
#if UNITY_EDITOR
[InitializeOnLoad] // Make sure static constructor is called during startup.
#endif
public class LogitechG920 : InputDevice
{
    static LogitechG920()
    {
        // Alternatively, you can also match by PID and VID, which is generally
        // more reliable for HIDs.
        InputSystem.RegisterLayout<LogitechG920>(
            matches: new InputDeviceMatcher()
                .WithInterface("HID")
                .WithCapability("vendorId", 0x46D)
                .WithCapability("productId", 0xC262));
    }

    // In the Player, to trigger the calling of the static constructor,
    // create an empty method annotated with RuntimeInitializeOnLoadMethod.
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void Init() {}
}
