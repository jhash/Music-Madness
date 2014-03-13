using UnityEngine;
using System.Collections;

public class CameraFollow : MonoBehaviour {

	public Transform playerTransform;

	// Use this for initialization
	void Start () {
		playerTransform = GameObject.FindGameObjectWithTag ("Player").transform;
	}
	
	// Update is called once per frame
	void Update () {
		transform.position = new Vector3 (playerTransform.position.x, playerTransform.position.y+0.75f, playerTransform.position.z-3f);
	}
}
