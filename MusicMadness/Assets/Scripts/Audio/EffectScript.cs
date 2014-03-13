using UnityEngine;
using System.Collections;

public class EffectScript : MonoBehaviour {
	private WaveRange connectedRange;
	private Color originalColor;
	private bool initialized = false;
	private bool hitting = false;

	void Update () {
		//If the Object is Connected to a WaveRange and Has Been Initialized
		// - This Will Call ResetOffBeat When the WaveRange's hitTimer Runs Out
		if (initialized && hitting) {
			if (!connectedRange.isHitting()) {
				ResetOffBeat();
				hitting = false;
			}
		}
	}

	//Called On A Hit From the Connected WaveRange
	public void OnHit () {
		//Signifies That the WaveRange's hitTimer Has Been Started
		hitting = true;

		//Add Whatever You Would Like Here Using the Public Data From the WaveRange
		
		//Example OnHit Code - This Example Sets the Color to be White
		renderer.material.color = Color.white;
	}

	//Called By the WaveRange If It Has A Connected EffectThisScript
	public void initWithRange (WaveRange cnctRng) {
		connectedRange = cnctRng;
		initialized = true;

		//Add Whatever You Would Like Here Using the Public Data From the WaveRange

		//Example Initialize Code - This Example Sets the Color to the WaveRange's Color and Stores It
		renderer.material.color = originalColor = connectedRange.thresholdColor;
	}

	//Called When the WaveRange Is No Longer Hitting
	void ResetOffBeat () {
		//Add Whatever You Would Like Here Using the Public Data From the WaveRange

		//Example Reset Code - This Example Resets the Color to the WaveRange's Color
		renderer.material.color = originalColor;
	}
}
