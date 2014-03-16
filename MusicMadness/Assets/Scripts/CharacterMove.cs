using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

[System.Serializable]
public class MusicCollection : IComparer<MusicCollection> {
	private AudioClip clipOfAudio = null;
	public List<float> startTimes;
	public List<float> endTimes;

	public bool recording = false;

	public int curClip = 0;
	
	public MusicCollection (AudioClip insClip, float time) {
		startTimes = new List<float> ();
		endTimes = new List<float> ();
		clipAction (insClip, time);
	}

	public void clipAction (AudioClip insClip, float time) {
		if (clipOfAudio == null)
			clipOfAudio = insClip;
			
		if (recording) {
			recording = false;
			if (time < endTimes[endTimes.Count-1]) {
				endTimes[endTimes.Count-1] = time;
			}
		} else {
			recording = true;
			startTimes.Add(time);
			endTimes.Add(time + insClip.length);
		}
	}
		
	public int Compare(MusicCollection x, MusicCollection y) {

		if (x.clipOfAudio == y.clipOfAudio) {
			return 0;
		} else {
			return 1;
		}
		
	}

}

public class CharacterMove : MonoBehaviour {
	
	public List<MusicCollection> recordedClips;

	public float moveSpeed = 5f;
	public float jumpForce = 400f;
	public Texture recordingIMG;
	
	private bool grounded = false;
	private bool recording = false;

	private float startRec = 0f;

	private List<MusicBox> touchingMusic;


	// Use this for initialization
	void Start () {
		touchingMusic = new List<MusicBox> ();
		recordedClips = new List<MusicCollection> ();
	}
	
	// Update is called once per frame
	void Update () {
		HorizontalMove ();
		VerticalMove ();
		Jump ();
		Record ();
	}

	void VerticalMove () {
		if (Input.GetAxis("Vertical") != 0f) {
			rigidbody.velocity = new Vector3 (rigidbody.velocity.x, rigidbody.velocity.y, moveSpeed * Input.GetAxis("Vertical"));
		} else {
			rigidbody.velocity = new Vector3 (rigidbody.velocity.x, rigidbody.velocity.y, 0f);
		}
	}

	void HorizontalMove () {
		if (Input.GetAxis("Horizontal") != 0f) {
			rigidbody.velocity = new Vector3 (moveSpeed * Input.GetAxis("Horizontal"), rigidbody.velocity.y, rigidbody.velocity.z);
		} else {
			rigidbody.velocity = new Vector3 (0f, rigidbody.velocity.y, rigidbody.velocity.z);
		}
	}

	void Jump () {

		if (Input.GetButtonDown("Jump") && (grounded || touchingMusic.Count > 0)) {
			rigidbody.AddForce (new Vector3 (0f, jumpForce, 0f));
		} 	

		if (Input.GetButtonUp("Jump") && rigidbody.velocity.y > 0f) {
			rigidbody.velocity = new Vector3 (rigidbody.velocity.x, 0f, rigidbody.velocity.x);
		} 	
	}

	void Record () {
		if (Input.GetKeyDown(KeyCode.R)) {
			if (recording) {
				startRec = 0f;

				finalizeRecording ();
			}
			recording = !recording;
		}
	}

	private void finalizeRecording () {

		foreach (MusicCollection col in recordedClips) {
			foreach (float time in col.startTimes) {

			}
		}
	}

	public void OnGUI() {
		// Process keyboard events as needed.
		int num;
		if (Event.current.type == EventType.KeyDown) {
			// Convert to numeric value for convenience :)
			num = Event.current.keyCode - KeyCode.Alpha1;

			if (num >= -1 && num < 9) {
				if (num == -1) num = 9;

				foreach (MusicBox muse in touchingMusic) {

					AudioClip insClip = muse.toggleMusic(num);
					if (recording && insClip != null) {
						if (startRec == 0f) {
							startRec = Time.time;
						}

						MusicCollection tmp = new MusicCollection (insClip, Time.time - startRec);
						bool found = false;
						foreach (MusicCollection col in recordedClips) {
							if (col.Compare(col, tmp) == 0) {
								col.clipAction (insClip, Time.time - startRec);
								found = true;
								break;
							} 
						}

						if (!found) {
							recordedClips.Add(tmp);
						}
					}

					if (Input.GetKey (KeyCode.L)) {
						muse.toggleLoop(num);
					}

				}
			}
		}

		if (recording) {
			GUI.DrawTexture (new Rect (Screen.width * 0.88f , Screen.height / 40f, Screen.width / 10f, Screen.height / 20f), recordingIMG);
		}
	}

	void OnCollisionEnter (Collision col) {
		if (col.gameObject.tag == "Ground" || col.gameObject.tag == "Musical") grounded = true;
	}
	
	void OnCollisionExit (Collision col) {
		if (col.gameObject.tag == "Ground" || col.gameObject.tag == "Musical") grounded = false;
	}

	public void hitMusicBox (MusicBox muse) {
		touchingMusic.Add (muse);
	}

	public void leftMusicBox (MusicBox muse) {
		touchingMusic.Remove (muse);
	}
}