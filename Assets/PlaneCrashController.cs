using System.Linq;
using Bhaptics.SDK2;
using UnityEngine;

public class PlaneCrashController : MonoBehaviour
{
    public AnimationCurve ImpulseToMotorPowerCurve;
    public int ImpulseMotorDuration;

    private void OnCollisionEnter(Collision other)
    {
        var motorPower = Mathf.RoundToInt(ImpulseToMotorPowerCurve.Evaluate(other.impulse.magnitude));
        Debug.Log($"impulse motor power = {motorPower}");
        BhapticsLibrary.PlayMotors((int)PositionType.GloveL, Enumerable.Repeat(motorPower, 6).ToArray(), ImpulseMotorDuration);
        BhapticsLibrary.PlayMotors((int)PositionType.GloveR, Enumerable.Repeat(motorPower, 6).ToArray(), ImpulseMotorDuration);
        BhapticsLibrary.PlayMotors((int)PositionType.Vest, Enumerable.Repeat(motorPower, 32).ToArray(), ImpulseMotorDuration);
    }
}