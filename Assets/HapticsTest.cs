using Bhaptics.SDK2;
using UnityEngine;

public class HapticsTest : MonoBehaviour
{
	private void Update()
	{
		var x = (int)(Mathf.Sin(Time.time) * 50 + 50);
		Debug.Log(x);
		BhapticsLibrary.PlayMotors((int)PositionType.GloveR, new[] { x, x, x, x, x, x }, 100);
	}
}
