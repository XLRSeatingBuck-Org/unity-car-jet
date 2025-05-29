using System;
using System.Linq;
using Bhaptics.SDK2;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class CrashController : MonoBehaviour
{
    public static CrashController Instance;
    
    public float crashImpulseThreshold; 
    [Space]
    public AnimationCurve ImpulseToMotorPowerCurve;
    public int ImpulseMotorDuration;

    private Rigidbody _rigidbody;
    private bool _atHome;

    private void Awake()
    {
        Instance = this;
        _rigidbody = GetComponent<Rigidbody>();
    }

    private void OnDestroy()
    {
        Instance = null;
    }

    private void OnCollisionEnter(Collision collision)
    {
        var impulse = collision.impulse.magnitude;

        var motorPower = Mathf.RoundToInt(ImpulseToMotorPowerCurve.Evaluate(impulse));
        BhapticsLibrary.PlayMotors((int)PositionType.GloveL, Enumerable.Repeat(motorPower, 6).ToArray(), ImpulseMotorDuration);
        BhapticsLibrary.PlayMotors((int)PositionType.GloveR, Enumerable.Repeat(motorPower, 6).ToArray(), ImpulseMotorDuration);
        BhapticsLibrary.PlayMotors((int)PositionType.Vest, Enumerable.Repeat(motorPower, 32).ToArray(), ImpulseMotorDuration);
        
        Debug.Log($"impulse = {impulse}, vel = {collision.relativeVelocity.magnitude}\n" +
                  $"impulse motor power = {ImpulseMotorDuration}, at home = {_atHome}");
        
        if (_atHome)
        {
            impulse /= 10;
        }

        if (impulse > crashImpulseThreshold)
        {
            ExperienceDirector.Instance.OnLose();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Home")) _atHome = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Home")) _atHome = false;
    }

    public bool StoppedAtHome => _atHome && _rigidbody.linearVelocity.magnitude < .1f;
}