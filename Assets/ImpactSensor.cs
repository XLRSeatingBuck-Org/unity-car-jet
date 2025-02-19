using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider))]
public class ImpactSensor : MonoBehaviour
{
	public UnityEvent<Vector3> OnImpact;

	private void OnCollisionEnter(Collision other)
	{
		Debug.Log($"impact {other.impulse}");
		OnImpact.Invoke(other.impulse);
	}
}
