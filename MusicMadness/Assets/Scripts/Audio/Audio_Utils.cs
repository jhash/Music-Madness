using UnityEngine;
using System.Collections;
using System.Collections.Generic;


[System.Serializable]
public class WaveRange {

	//Object to Attach - Use The EffectScript to Catch 
	public GameObject effectThis;
	private bool objectWasSet = false;

	public string rangeID;
	public int sampleMin = 0;
	public int sampleMax = 5;
	public float threshold = 0f;
	public float hitRate = 0.1f;
	public bool automaticThreshold = true;
	public bool resetThreshold = true;
	public float resetTimerLength = 5f;
	public Color thresholdColor = Color.blue;
	private float resetTimer = 0f;
	private float hitTimer = 0f;
	private int sampleSize;
	private int numHits = 0;
	private float avg = 0f;
	private float maxAvg = 0f;
	private bool didThreshChange = true;

	//Public Access Methods
	public bool isHitting () {
		return hitTimer > 0f;
	}

	//Functions Utilized by Audio_Utils

	//Updates the Timers Associated with this WaveRange
	public void UpdateTimer() {

		//Decrement the Hit Timer!
		if (hitTimer > 0f)
			hitTimer -= Time.deltaTime;

		//If the Reset Timer Runs Out the Threshold Timer is Reset to 0
		// - This Happens When a Range's Threshold Hasn't Been Exceeded For a While
		// - Can Be Disabled Via the Reset Threshold Boolean
		if (resetTimer > 0f)
			resetTimer -= Time.deltaTime;
		else if (resetThreshold) 
			resetThresh();

		//If the User Changes the Automatic Threshold Boolean in the Inspector
		// - The Threshold is Reset and the Number of Hits is Set to 0
		if (didThreshChange != automaticThreshold) {
			didThreshChange = automaticThreshold;
			numHits = 0;
			resetThresh();
		}

		if (!objectWasSet && effectThis != null) {
			effectThis.GetComponent<EffectScript>().initWithRange(this);
			objectWasSet = true;
		}
	}

	//Updates the Threshold Line and ReDraws it On the Screen
	public bool UpdateRange(int i, float[] spect) {	

		if (i == sampleMax) {

			//Take the Average of the Range and Set the Maximum
			// - Also Detects If the Threshold Was Exceeded and Calls Hit()
			avg = 0f;
			int j = 0;
			while (j < sampleSize) {
				avg += spect[sampleMin+j];
				if (spect[sampleMin+j] > threshold) {
					Hit();
				}
				j++;
			}
			avg = avg / (float) (sampleSize);
			
			if (avg > maxAvg) {
				maxAvg = avg;
			}

			//If Automatic Threshold is Enabled the Threshold Moves to Highest Average Found So Far
			if (maxAvg > threshold && automaticThreshold)
				threshold = (maxAvg);
			
		}

		//If i is Within the Range Draw the Threshold Lines
		if (i >= sampleMin && i < sampleMax) {

			Debug.DrawLine(new Vector3(i - 1, threshold + 10, 0), new Vector3(i, threshold + 10, 0), thresholdColor);
			Debug.DrawLine(new Vector3(Mathf.Log(i - 1), threshold - 10, 1), new Vector3(Mathf.Log(i), threshold - 10, 1), thresholdColor); 
			Debug.DrawLine(new Vector3(i - 1, Mathf.Log(threshold) + 10, 2), new Vector3(i, Mathf.Log(threshold) + 10, 2), thresholdColor);
			Debug.DrawLine(new Vector3(Mathf.Log(i - 1), Mathf.Log(threshold), 3), new Vector3(Mathf.Log(i), Mathf.Log(threshold), 3), thresholdColor);

			return true;
			
		}
		
		return false;
	}

	//Called When The Range is Exceeded By One of the Samples In the Range
	void Hit() {
		if (hitTimer > 0f)
			return;
		
		numHits++;
		hitTimer = hitRate;

		//If Connected To An Object, This Will Call The Objects OnHit Function
		if (effectThis != null) {
			//Outputs the Attached GameObject's Name
			//Uncomment to Test Connection
//			Debug.Log (effectThis.gameObject.name);

			effectThis.GetComponent<EffectScript>().OnHit();
		}
		else {
			Debug.Log ("Range " + rangeID + " from  " + sampleMin + " to " + sampleMax + " hit " + numHits + " times");
		}
		
		if (resetThreshold)
			resetTimer = resetTimerLength;
	}

	//Resets the Threshold and Max Average to 0 and Resets the Timer
	void resetThresh() {
		threshold = 0f;
		maxAvg = 0f;
		if (resetThreshold)
			resetTimer = resetTimerLength;
	}
	
	//Constructors
	public WaveRange() {
		sampleSize = sampleMax - sampleMin - 1;
		didThreshChange = automaticThreshold;
		rangeID = "default";
	}
	
	public WaveRange(int min, int max, string ID) {
		sampleMin = min;
		sampleMax = max;
		sampleSize = sampleMax - sampleMin - 1;
		didThreshChange = automaticThreshold;
		rangeID = ID;
	}	
	
	public WaveRange(int min, int max, string ID, float hRate) {
		sampleMin = min;
		sampleMax = max;
		sampleSize = sampleMax - sampleMin - 1;
		didThreshChange = automaticThreshold;
		rangeID = ID;
		hitRate = hRate;
	}		
	
	public WaveRange(int min, int max, string ID, float hRate, float rstTime) {
		sampleMin = min;
		sampleMax = max;
		sampleSize = sampleMax - sampleMin - 1;
		didThreshChange = automaticThreshold;
		rangeID = ID;
		hitRate = hRate;
		resetTimerLength = rstTime;
	}	
	
	public WaveRange(int min, int max, string ID, float hRate, float rstTime, GameObject effectObject) {
		sampleMin = min;
		sampleMax = max;
		sampleSize = sampleMax - sampleMin - 1;
		didThreshChange = automaticThreshold;
		rangeID = ID;
		hitRate = hRate;
		resetTimerLength = rstTime;
		effectThis = effectObject;
		effectObject.GetComponent<EffectScript>().initWithRange(this);
		objectWasSet = true;
	}	
}

public class Audio_Utils : MonoBehaviour {

	public List<WaveRange> ranges;
	private int sampleSizePower = 10;
	private int sampleSize = 1024;
	
	void Start () {

		//Make Sure Sample Size Will Be Between 64 and 6192
		if (sampleSizePower < 6 || sampleSizePower > 13)
			sampleSizePower = 10;

		sampleSize = (int)Mathf.Pow (2f, sampleSizePower);
	}
	
	void Update() {

		//Updates Each WaveRange's Timer Because They Do Not Inherit MonoBehaviour
		foreach (WaveRange wv in ranges) {
			wv.UpdateTimer();
		}

		//Get the Spectrum Data for the Audio Clip Currently Playing
		// - To Increase Performance (But Decrease Accuracy) Change BlackmanHarris to Other FFTWindow Type (eg. Rectangular)
		float[] spectrum = audio.GetSpectrumData(sampleSize, 0, FFTWindow.BlackmanHarris);

		//Goes Through Every Sample in the Spectrum and Draws the Necessary Lines
		int i = 1;
		while ( i < sampleSize-1 ) {
			//Draw Regular Colors
			Debug.DrawLine(new Vector3(Mathf.Log(i - 1), spectrum[i - 1] - 10, 1), new Vector3(Mathf.Log(i), spectrum[i] - 10, 1), Color.green);
			Debug.DrawLine(new Vector3(i - 1, spectrum[i] + 10, 0), new Vector3(i, spectrum[i + 1] + 10, 0), Color.red);
			Debug.DrawLine(new Vector3(i - 1, Mathf.Log(spectrum[i - 1]) + 10, 2), new Vector3(i, Mathf.Log(spectrum[i]) + 10, 2), Color.cyan);
			Debug.DrawLine(new Vector3(Mathf.Log(i - 1), Mathf.Log(spectrum[i - 1]), 3), new Vector3(Mathf.Log(i), Mathf.Log(spectrum[i]), 3), Color.yellow);

			//Draw the Waves
			foreach (WaveRange wv in ranges) {
				//If i is Within the Wave Range, Draw the Line Above in White Over Top
				if (wv.UpdateRange(i, spectrum)) {
					Debug.DrawLine(new Vector3(i - 1, spectrum[i] + 10, 0), new Vector3(i, spectrum[i + 1] + 10, 0), Color.white);
					Debug.DrawLine(new Vector3(Mathf.Log(i - 1), spectrum[i - 1] - 10, 1), new Vector3(Mathf.Log(i), spectrum[i] - 10, 1), Color.white);
					Debug.DrawLine(new Vector3(i - 1, Mathf.Log(spectrum[i - 1]) + 10, 2), new Vector3(i, Mathf.Log(spectrum[i]) + 10, 2), Color.white);
					Debug.DrawLine(new Vector3(Mathf.Log(i - 1), Mathf.Log(spectrum[i - 1]), 3), new Vector3(Mathf.Log(i), Mathf.Log(spectrum[i]), 3), Color.white);
				} 
			}


			i++;
		}

	}

	//Returns Whether a Specific Wave Range Has Been Hit Recently or Not
	public bool isHitting(string ID) {
		WaveRange tmp = findRangeWithID (ID);
		if (tmp != null) {
			return tmp.isHitting();
		}

		return false;

	}

	//Adds a New Sample Range Within the List, Two Different Options for Input 
	public void addSampleRange(int min, int max, string ID) {
		ranges.Add(new WaveRange(min, max, ID));
	}

	public void addSampleRange(WaveRange newRange) {
		ranges.Add(newRange);
	}

	public void setSampleRanges(List<WaveRange> waveRangeList, int smpSizePower) {
		ranges.Clear ();
		ranges.AddRange (waveRangeList);

		sampleSizePower = smpSizePower;
		sampleSize = (int)Mathf.Pow (2f, sampleSizePower);
	}

	//Returns the WaveRange With the Given ID if it Exists
	public WaveRange findRangeWithID (string ID) {
		foreach (WaveRange wv in ranges) {
			if (wv.rangeID == ID)
				return wv;
		}

		return null;
	}

}
