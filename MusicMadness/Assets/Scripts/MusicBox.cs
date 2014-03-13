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

	public void toggleMusic () {
		if (audio.isPlaying) {
			audio.Stop ();
		} else {
			audio.Play ();
		}
	}
	
}
