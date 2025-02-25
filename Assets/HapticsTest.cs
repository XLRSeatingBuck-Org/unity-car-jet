using Bhaptics.SDK2;
using System.Linq;
using UnityEngine;

public class HapticsTest : MonoBehaviour
{
	private void Update()
	{
		var motorPower = (int)(Mathf.Sin(Time.time) * 50 + 50);
		motorPower /= 5;
		Debug.Log(motorPower);
		BhapticsLibrary.PlayMotors((int)PositionType.GloveL, Enumerable.Repeat(motorPower, 6).ToArray(), 100);
		BhapticsLibrary.PlayMotors((int)PositionType.GloveR, Enumerable.Repeat(motorPower, 6).ToArray(), 100);
		BhapticsLibrary.PlayMotors((int)PositionType.Vest, Enumerable.Repeat(motorPower, 32).ToArray(), 100);
	}

	public void OnImpact(Vector3 impulse)
	{
		var motorPower = 100;
		Debug.Log(motorPower);
		BhapticsLibrary.PlayMotors((int)PositionType.GloveL, Enumerable.Repeat(motorPower, 6).ToArray(), 1000);
		BhapticsLibrary.PlayMotors((int)PositionType.GloveR, Enumerable.Repeat(motorPower, 6).ToArray(), 1000);
		BhapticsLibrary.PlayMotors((int)PositionType.Vest, Enumerable.Repeat(motorPower, 32).ToArray(), 1000);
	}
}
