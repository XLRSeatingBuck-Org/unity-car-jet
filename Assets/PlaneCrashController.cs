using System.Linq;
using Bhaptics.SDK2;
using UnityEngine;

public class PlaneCrashController : MonoBehaviour
{
    public float crashImpulseThreshold; 
    [Space]
    public AnimationCurve ImpulseToMotorPowerCurve;
    public int ImpulseMotorDuration;

    private void OnCollisionEnter(Collision collision)
    {
        var impulse = collision.impulse.magnitude;

        {
            var motorPower = Mathf.RoundToInt(ImpulseToMotorPowerCurve.Evaluate(impulse));
            Debug.Log($"impulse = {impulse}, vel = {collision.relativeVelocity.magnitude}, impulse motor power = {motorPower}");
            BhapticsLibrary.PlayMotors((int)PositionType.GloveL, Enumerable.Repeat(motorPower, 6).ToArray(), ImpulseMotorDuration);
            BhapticsLibrary.PlayMotors((int)PositionType.GloveR, Enumerable.Repeat(motorPower, 6).ToArray(), ImpulseMotorDuration);
            BhapticsLibrary.PlayMotors((int)PositionType.Vest, Enumerable.Repeat(motorPower, 32).ToArray(), ImpulseMotorDuration);
        }
        
        if (collision.collider.CompareTag("Runway"))
        {
            impulse /= 10;
        }

        if (impulse > crashImpulseThreshold)
        {
            ExperienceDirector.Instance.OnCrash();
        }
    }
}