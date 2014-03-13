using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ClipRangePresets : MonoBehaviour {
	
	public Audio_Utils utils;
	public AudioClip theClip;
	public string theClipName;
	public int smpSizePower;
	public List<WaveRange> ranges;
	private AudioSource audioSrc;

	//Initializes the Attached Clip to Play Within the Audio Src On This GameObject
	void Start () {
		Mathf.Clamp (smpSizePower, 6, 13);
		//Set Audio Source, Then Check If It Actually Exists
		audioSrc = audio;
		if (audioSrc != null) {

			//Sets the Clip to the Attached Clip
			if (theClip != null) {
				audioSrc.clip = theClip;

				//Find the Audio_Utils Component Attached To the GameObject
				utils = GetComponent<Audio_Utils> ();
				if (utils != null) {
					utils.setSampleRanges(ranges, smpSizePower);
				}
			}
			
			if (!audioSrc.isPlaying)
				audioSrc.Play();
		}
	}

}
