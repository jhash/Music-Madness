using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MusicBox : MonoBehaviour {

	public bool playerInBox = false;

	public List<AudioClip> clipsOfAudio;
	public List<AudioSource> sourcesOfAudio;

	// Use this for initialization
	void Start () {
		int i = 0;
		foreach (AudioClip aud in clipsOfAudio) {
			AudioSource newAudio = (AudioSource) gameObject.AddComponent("AudioSource");
			sourcesOfAudio.Add(newAudio);
			newAudio.playOnAwake = false;
			newAudio.dopplerLevel = 0f;
			newAudio.loop = false;
			sourcesOfAudio[i].clip = clipsOfAudio[i];
			i++;
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnTriggerExit (Collider col) {
		if (col.gameObject.tag == "Player") {
			playerInBox = false;

			col.gameObject.GetComponent<CharacterMove> ().leftMusicBox(this);
		}	
	}

	void OnTriggerEnter (Collider col) {
		if (col.gameObject.tag == "Player") {
			//Play on Camera
//				AudioSource camAudio = Camera.main.audio;
//				camAudio.clip = this.audio.clip;
//				camAudio.Play ();
			col.gameObject.GetComponent<CharacterMove> ().hitMusicBox(this);

			playerInBox = true;
		}	
	}

	public AudioClip toggleMusic (int clipToToggle) {
		if (clipToToggle < sourcesOfAudio.Count) {
			if (sourcesOfAudio[clipToToggle].isPlaying) {
				sourcesOfAudio[clipToToggle].Stop ();
			} else {
				sourcesOfAudio[clipToToggle].Play ();
			}
			return clipsOfAudio[clipToToggle];
		}

		return null;
	}
	
	public void toggleLoop (int clipToToggle) {
		sourcesOfAudio[clipToToggle].loop = !sourcesOfAudio[clipToToggle].loop;
	}
	
}
