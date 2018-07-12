using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blocker : MonoBehaviour
{
	public Transform Target;
	private Vector3 Initial;
	private float Speed = 0.1f;

	private Rigidbody Rigidbody;

	public bool canJump = false;
	public bool canBlock = false;
	private bool canMove = true;

	public GameObject GameobjectArms;
	
	// Use this for initialization
	void Start ()
	{
		Rigidbody = GetComponent<Rigidbody>();
		StartCoroutine(DoBlock());
	}
	
	// Update is called once per frame
	private void OnCollisionEnter(Collision other)
	{
		if (other.gameObject.name == "Court")
		{
			canJump = true;
		}
	}
	
	private void OnCollisionExit(Collision other)
	{
		if (other.gameObject.name == "Court")
		{
			canJump = false;
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.name == "Positioned")
		{
			canBlock = true;
		}
	}
	
	private void OnTriggerExit(Collider other)
	{
		if (other.gameObject.name == "Positioned")
		{
			canBlock = false;
		}
	}

	IEnumerator DoBlock() {
		while (true)
		{
			yield return new WaitUntil(() => { return canBlock && canJump; });
			GameobjectArms.SetActive(true);
			canMove = false;
			
			yield return new WaitUntil(() => { return !(canBlock && canJump); });
			GameobjectArms.SetActive(false);
			canMove = true;
		}
	}

	void FixedUpdate ()
	{
//		if (!canMove) return;
		
		Rigidbody.MovePosition(Vector3.MoveTowards(
			transform.position,
			new Vector3(
				Target.position.x,
				Initial.y,
				Target.position.z),
			Speed
		));
	}
}
