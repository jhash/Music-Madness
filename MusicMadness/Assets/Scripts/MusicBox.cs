using UnityEngine;
using System.Collections;

public class MusicBox : MonoBehaviour {

	public bool playerInBox = false;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnTriggerExit (Collider col) {
		if (col.gameObject.tag == "Player") {
			playerInBox = false;
		}	
	}

	void OnTriggerEnter (Collider col) {
		if (col.gameObject.tag == "Player") {
			AudioSource camAudio = Camera.main.audio;
			camAudio.clip = this.audio.clip;
			camAudio.Play ();
			playerInBox = true;
		}	
	}

}
