using System.Collections.Generic;
using System.Linq;
using Bhaptics.SDK2;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// controls plane movement and physics.
/// 
/// based on:
/// - plane tutorial https://github.com/vazgriz/FlightSim/blob/part-1/Assets/Scripts/Plane.cs
/// - plane prefab https://github.com/vazgriz/FlightSim/blob/part-1/Assets/Prefabs/F15.prefab
/// - Payton's implementation https://github.com/XLRSeatingBuck-Org/unreal_jet/blob/main/Source/UnrealSim/Jet.cpp
/// </summary>
public class JetMovement : MonoBehaviour
{
	public InputActionReference ThrottleInput, PitchInput, YawInput, RollInput;
	public InputActionReference FlapsInput;

    public AnimationCurve SpeedToMotorPowerCurve;

    /// <summary>
    /// apply a force up at this position, scaled by this scale
    /// </summary>
    public Transform DownwardTorqueForce;
    
    [Space]
    

    [SerializeField]
    float maxThrust;
    [SerializeField]
    float throttleSpeed;
    [SerializeField]
    float gLimit;
    [SerializeField]
    float gLimitPitch;

    [Header("Lift")]
    [SerializeField]
    float liftPower;
    [SerializeField]
    AnimationCurve liftAOACurve;
    [SerializeField]
    float inducedDrag;
    [SerializeField]
    AnimationCurve inducedDragCurve;
    [SerializeField]
    float rudderPower;
    [SerializeField]
    AnimationCurve rudderAOACurve;
    [SerializeField]
    AnimationCurve rudderInducedDragCurve;
    [SerializeField]
    float flapsLiftPower;
    [SerializeField]
    float flapsAOABias;
    [SerializeField]
    float flapsDrag;
    [SerializeField]
    float flapsRetractSpeed;

    [Header("Steering")]
    [SerializeField]
    Vector3 turnSpeed;
    [SerializeField]
    Vector3 turnAcceleration;
    [SerializeField]
    AnimationCurve steeringCurve;

    [Header("Drag")]
    [SerializeField]
    AnimationCurve dragForward;
    [SerializeField]
    AnimationCurve dragBack;
    [SerializeField]
    AnimationCurve dragLeft;
    [SerializeField]
    AnimationCurve dragRight;
    [SerializeField]
    AnimationCurve dragTop;
    [SerializeField]
    AnimationCurve dragBottom;
    [SerializeField]
    Vector3 angularDrag;
    [SerializeField]
    float airbrakeDrag;

    float throttleInput;
    Vector3 controlInput;

    Vector3 lastVelocity;

    public Rigidbody Rigidbody { get; private set; }
    public float Throttle { get; private set; }
    public Vector3 EffectiveInput { get; private set; }
    public Vector3 Velocity { get; private set; }
    public Vector3 LocalVelocity { get; private set; }
    public Vector3 LocalGForce { get; private set; }
    public Vector3 LocalAngularVelocity { get; private set; }
    public float AngleOfAttack { get; private set; }
    public float AngleOfAttackYaw { get; private set; }
    public bool AirbrakeDeployed { get; private set; }

    [SerializeField]
    bool flapsDeployed;
    public bool FlapsDeployed {
        get {
            return flapsDeployed;
        }
        private set {
            flapsDeployed = value;
        }
    }

    void Start() {
        Rigidbody = GetComponent<Rigidbody>();
    }

    /* handle input */
    public void SetThrottleInput(float input) {
        throttleInput = input;
    }

    public void SetControlInput(Vector3 input) {
        controlInput = input;
    }

    void UpdateThrottle(float dt) {
        Throttle = throttleInput;

        AirbrakeDeployed = Throttle == 0;
    }

    void UpdateFlaps() {
        if (LocalVelocity.z > flapsRetractSpeed) {
            FlapsDeployed = false;
        } else if (LocalVelocity.z < flapsRetractSpeed) {
            FlapsDeployed = FlapsInput.action.IsPressed();
        }

    }
    
    
    /* calculations */
    void CalculateAngleOfAttack() {
        if (LocalVelocity.sqrMagnitude < 0.1f) {
            AngleOfAttack = 0;
            AngleOfAttackYaw = 0;
            return;
        }

        AngleOfAttack = Mathf.Atan2(-LocalVelocity.y, LocalVelocity.z);
        AngleOfAttackYaw = Mathf.Atan2(LocalVelocity.x, LocalVelocity.z);
    }

    void CalculateGForce(float dt) {
        var invRotation = Quaternion.Inverse(Rigidbody.rotation);
        var acceleration = (Velocity - lastVelocity) / dt;
        LocalGForce = invRotation * acceleration;
        lastVelocity = Velocity;
    }

    void CalculateState(float dt) {
        var invRotation = Quaternion.Inverse(Rigidbody.rotation);
        Velocity = Rigidbody.linearVelocity;
        LocalVelocity = invRotation * Velocity;  //transform world velocity into local space
        LocalAngularVelocity = invRotation * Rigidbody.angularVelocity;  //transform into local space

        CalculateAngleOfAttack();
    }

    /* update state and forces */
    void UpdateThrust() {
        Rigidbody.AddRelativeForce(Throttle * maxThrust * Vector3.forward);
    }

    void UpdateDrag() {
        var lv = LocalVelocity;
        var lv2 = lv.sqrMagnitude;  //velocity squared

        float airbrakeDrag = AirbrakeDeployed ? this.airbrakeDrag : 0;
        float flapsDrag = FlapsDeployed ? this.flapsDrag : 0;

        //calculate coefficient of drag depending on direction on velocity
        var coefficient = Utilities.Scale6(
            lv.normalized,
            dragRight.Evaluate(Mathf.Abs(lv.x)), dragLeft.Evaluate(Mathf.Abs(lv.x)),
            dragTop.Evaluate(Mathf.Abs(lv.y)), dragBottom.Evaluate(Mathf.Abs(lv.y)),
            dragForward.Evaluate(Mathf.Abs(lv.z)) + airbrakeDrag + flapsDrag,   //include extra drag for forward coefficient
            dragBack.Evaluate(Mathf.Abs(lv.z))
        );

        var drag = coefficient.magnitude * lv2 * -lv.normalized;    //drag is opposite direction of velocity

        Rigidbody.AddRelativeForce(drag);
        
        // make the plane tilt downward when falling
        Rigidbody.AddForceAtPosition(Vector3.up * (Mathf.Sqrt(Mathf.Abs(-Rigidbody.linearVelocity.y)) * DownwardTorqueForce.localScale.x), DownwardTorqueForce.position);
    }

    Vector3 CalculateLift(float angleOfAttack, Vector3 rightAxis, float liftPower, AnimationCurve aoaCurve, AnimationCurve inducedDragCurve) {
        var liftVelocity = Vector3.ProjectOnPlane(LocalVelocity, rightAxis);    //project velocity onto YZ plane
        var v2 = liftVelocity.sqrMagnitude;                                     //square of velocity

        //lift = velocity^2 * coefficient * liftPower
        //coefficient varies with AOA
        var liftCoefficient = aoaCurve.Evaluate(angleOfAttack * Mathf.Rad2Deg);
        var liftForce = v2 * liftCoefficient * liftPower;

        //lift is perpendicular to velocity
        var liftDirection = Vector3.Cross(liftVelocity.normalized, rightAxis);
        var lift = liftDirection * liftForce;

        //induced drag varies with square of lift coefficient
        var dragForce = liftCoefficient * liftCoefficient;
        var dragDirection = -liftVelocity.normalized;
        var inducedDrag = dragDirection * v2 * dragForce * this.inducedDrag * inducedDragCurve.Evaluate(Mathf.Max(0, LocalVelocity.z));

        return lift + inducedDrag;
    }

    void UpdateLift() {
        if (LocalVelocity.sqrMagnitude < 1f) return;

        float flapsLiftPower = FlapsDeployed ? this.flapsLiftPower : 0;
        float flapsAOABias = FlapsDeployed ? this.flapsAOABias : 0;

        // do lift from all flaps
        var liftForce = CalculateLift(
            AngleOfAttack + (flapsAOABias * Mathf.Deg2Rad), Vector3.right,
            liftPower + flapsLiftPower,
            liftAOACurve,
            inducedDragCurve
        );

        var yawForce = CalculateLift(AngleOfAttackYaw, Vector3.up, rudderPower, rudderAOACurve, rudderInducedDragCurve);

        Rigidbody.AddRelativeForce(liftForce);
        Rigidbody.AddRelativeForce(yawForce);
    }

    void UpdateAngularDrag() {
        var av = LocalAngularVelocity;
        var drag = av.sqrMagnitude * -av.normalized;    //squared, opposite direction of angular velocity
        Rigidbody.AddRelativeTorque(Vector3.Scale(drag, angularDrag), ForceMode.Acceleration);  //ignore rigidbody mass
    }

    Vector3 CalculateGForce(Vector3 angularVelocity, Vector3 velocity) {
        //estiamte G Force from angular velocity and velocity
        //Velocity = AngularVelocity * Radius
        //G = Velocity^2 / R
        //G = (Velocity * AngularVelocity * Radius) / Radius
        //G = Velocity * AngularVelocity
        //G = V cross A
        return Vector3.Cross(angularVelocity, velocity);
    }

    Vector3 CalculateGForceLimit(Vector3 input) {
        return Utilities.Scale6(input,
            gLimit, gLimitPitch,    //pitch down, pitch up
            gLimit, gLimit,         //yaw
            gLimit, gLimit          //roll
        ) * 9.81f;
    }

    float CalculateGLimiter(Vector3 controlInput, Vector3 maxAngularVelocity) {
        if (controlInput.magnitude < 0.01f) {
            return 1;
        }

        //if the player gives input with magnitude less than 1, scale up their input so that magnitude == 1
        var maxInput = controlInput.normalized;

        var limit = CalculateGForceLimit(maxInput);
        var maxGForce = CalculateGForce(Vector3.Scale(maxInput, maxAngularVelocity), LocalVelocity);

        if (maxGForce.magnitude > limit.magnitude) {
            //example:
            //maxGForce = 16G, limit = 8G
            //so this is 8 / 16 or 0.5
            return limit.magnitude / maxGForce.magnitude;
        }

        return 1;
    }

    float CalculateSteering(float dt, float angularVelocity, float targetVelocity, float acceleration) {
        var error = targetVelocity - angularVelocity;
        var accel = acceleration * dt;
        return Mathf.Clamp(error, -accel, accel);
    }

    void UpdateSteering(float dt) {
        var speed = Mathf.Max(0, LocalVelocity.z);
        var steeringPower = steeringCurve.Evaluate(speed);

        var gForceScaling = CalculateGLimiter(controlInput, turnSpeed * Mathf.Deg2Rad * steeringPower);

        // get steering from input
        var targetAV = Vector3.Scale(controlInput, turnSpeed * steeringPower * gForceScaling);
        var av = LocalAngularVelocity * Mathf.Rad2Deg;

        var correction = new Vector3(
            CalculateSteering(dt, av.x, targetAV.x, turnAcceleration.x * steeringPower),
            CalculateSteering(dt, av.y, targetAV.y, turnAcceleration.y * steeringPower),
            CalculateSteering(dt, av.z, targetAV.z, turnAcceleration.z * steeringPower)
        );

        Rigidbody.AddRelativeTorque(correction * Mathf.Deg2Rad, ForceMode.VelocityChange);    //ignore rigidbody mass

        var correctionInput = new Vector3(
            Mathf.Clamp((targetAV.x - av.x) / turnAcceleration.x, -1, 1),
            Mathf.Clamp((targetAV.y - av.y) / turnAcceleration.y, -1, 1),
            Mathf.Clamp((targetAV.z - av.z) / turnAcceleration.z, -1, 1)
        );

        var effectiveInput = (correctionInput + controlInput) * gForceScaling;

        EffectiveInput = new Vector3(
            Mathf.Clamp(effectiveInput.x, -1, 1),
            Mathf.Clamp(effectiveInput.y, -1, 1),
            Mathf.Clamp(effectiveInput.z, -1, 1)
        );
    }

    void FixedUpdate() {
        float dt = Time.fixedDeltaTime;

        //calculate at start, to capture any changes that happened externally
        CalculateState(dt);
        CalculateGForce(dt);
        UpdateFlaps();

        //handle user input
        {
            var throttle = ThrottleInput.action.ReadValue<float>();
            var pitch = PitchInput.action.ReadValue<float>();
            var yaw = YawInput.action.ReadValue<float>();
            var roll = RollInput.action.ReadValue<float>();
            
            SetThrottleInput(throttle);
            SetControlInput(new Vector3(pitch, yaw, roll));
        }
        UpdateThrottle(dt);

        //apply updates
        UpdateThrust();
        UpdateLift();

        UpdateSteering(dt);

        UpdateDrag();
        UpdateAngularDrag();

        //calculate again, so that other systems can read this plane's state
        CalculateState(dt);
        
        
        
        // update haptics
        {
		    var localForwardSpeed = Vector3.Project(Rigidbody.linearVelocity, transform.forward).magnitude;
            
            var motorPower = Mathf.RoundToInt(SpeedToMotorPowerCurve.Evaluate(localForwardSpeed));
            // docs recommend 100 ms or more
            BhapticsLibrary.PlayMotors((int)PositionType.GloveL, Enumerable.Repeat(motorPower, 6).ToArray(), 100);
            BhapticsLibrary.PlayMotors((int)PositionType.GloveR, Enumerable.Repeat(motorPower, 6).ToArray(), 100);
            BhapticsLibrary.PlayMotors((int)PositionType.Vest, Enumerable.Repeat(motorPower, 32).ToArray(), 100);
        }
    }

    
    
    

	#if UNITY_EDITOR
	private void OnGUI()
	{
		var throttle = ThrottleInput.action.ReadValue<float>();
		var pitch = PitchInput.action.ReadValue<float>();
		var yaw = YawInput.action.ReadValue<float>();
		var roll = RollInput.action.ReadValue<float>();
		GUILayout.Label($"throttle = {throttle}\npitch = {pitch}\nyaw = {yaw}\nroll = {roll}");

		var localForwardSpeed = Vector3.Project(Rigidbody.linearVelocity, transform.forward).magnitude;
		/*
		var liftForce = transform.up * Mathf.Max(0, localForwardSpeed * LiftAmount);
		GUILayout.Label($"forward = {localForwardSpeed}\nlift = {liftForce.magnitude}");
		*/

		GUILayout.Label($"velocity = {Rigidbody.linearVelocity} / {transform.InverseTransformDirection(Rigidbody.linearVelocity)}\n" + 
                        $"angular velocity = {Rigidbody.angularVelocity * Mathf.Rad2Deg} / {transform.InverseTransformDirection(Rigidbody.angularVelocity) * Mathf.Rad2Deg}");

		var motorPower = Mathf.RoundToInt(SpeedToMotorPowerCurve.Evaluate(localForwardSpeed));
		GUILayout.Label($"speed motor power = {motorPower}");

        GUILayout.Label(
            $"Throttle = {Throttle}\n" +
            $"EffectiveInput = {EffectiveInput}\n" +
            $"Velocity = {Velocity}\n" +
            $"LocalVelocity = {LocalVelocity}\n" +
            $"LocalGForce = {LocalGForce}\n" +
            $"LocalAngularVelocity = {LocalAngularVelocity * Mathf.Rad2Deg}\n" +
            $"AngleOfAttack = {AngleOfAttack * Mathf.Rad2Deg}\n" +
            $"AngleOfAttackYaw = {AngleOfAttackYaw * Mathf.Rad2Deg}\n" +
            $"AirbrakeDeployed = {AirbrakeDeployed}\n" +
            $"FlapsDeployed = {FlapsDeployed}\n"
        );
    }
	#endif
}


public static class Utilities {
    public static float MoveTo(float value, float target, float speed, float deltaTime, float min = 0, float max = 1) {
        float diff = target - value;
        float delta = Mathf.Clamp(diff, -speed * deltaTime, speed * deltaTime);
        return Mathf.Clamp(value + delta, min, max);
    }

    //similar to Vector3.Scale, but has separate factor negative values on each axis
    public static Vector3 Scale6(
        Vector3 value,
        float posX, float negX,
        float posY, float negY,
        float posZ, float negZ
    ) {
        Vector3 result = value;

        if (result.x > 0) {
            result.x *= posX;
        } else if (result.x < 0) {
            result.x *= negX;
        }

        if (result.y > 0) {
            result.y *= posY;
        } else if (result.y < 0) {
            result.y *= negY;
        }

        if (result.z > 0) {
            result.z *= posZ;
        } else if (result.z < 0) {
            result.z *= negZ;
        }

        return result;
    }
}