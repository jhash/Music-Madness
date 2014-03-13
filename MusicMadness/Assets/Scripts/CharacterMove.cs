using UnityEngine;
using System.Collections;

public class CharacterMove : MonoBehaviour {

	public float moveSpeed = 5f;
	public float jumpForce = 400f;

	public bool grounded = false;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		HorizontalMove ();
		VerticalMove ();
		Jump ();
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

		if (Input.GetButtonDown("Jump") && grounded) {
			rigidbody.AddForce (new Vector3 (0f, jumpForce, 0f));
		} 	
	}

	void OnCollisionEnter (Collision col) {
		if (col.gameObject.tag == "Ground" || col.gameObject.tag == "Musical") grounded = true;
	}
	
	void OnCollisionExit (Collision col) {
		if (col.gameObject.tag == "Ground" || col.gameObject.tag == "Musical") grounded = false;
	}
}
