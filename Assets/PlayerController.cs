using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
	public float Speed;

	private Rigidbody Rigidbody;
	// Use this for initialization
	void Start ()
	{
		Rigidbody = GetComponent<Rigidbody>();
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		Vector3 movement = new Vector3(
			Input.GetAxis("Horizontal") * Speed * Time.deltaTime,
			0f,
			Input.GetAxis("Vertical") * Speed * Time.deltaTime
		);

		Rigidbody.MovePosition(Vector3.MoveTowards(
			transform.position,
			transform.position + movement,
			Speed
		));
	}
}
