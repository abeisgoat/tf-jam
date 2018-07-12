using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Boo.Lang;
using UnityEngine;
using UnityEngine.Assertions.Comparers;

public class BallController : MonoBehaviour
{
	public Vector3 Force;
	public float Distance;
	public static int SuccessCount = 0;
	public static int ShotCount = 1;

	public Material MaterialBallScored;
	private Vector3 Scaler = new Vector3(1000, 1000, 1000);

	private bool hasBeenScored = false;
	
	// Use this for initialization
	void Start ()
	{
		var scaledForce = Vector3.Scale(Scaler, Force);
		GetComponent<Rigidbody>().AddForce(scaledForce);
		StartCoroutine(DoDespawn(30));
		ShotCount++;
	}

	IEnumerator DoDespawn(float delay)
	{
		yield return new WaitForSeconds(delay);
		Destroy(gameObject);
	}
	
	private bool hasTriggeredTop = false;

	private void OnCollisionEnter(Collision other)
	{
		if (other.gameObject.name == "Court")
		{
			StartCoroutine(DoDespawn(2.5f));
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.name == "TriggerTop")
		{
			hasTriggeredTop = true;
		} else if (other.name == "TriggerBottom") {
			if (hasTriggeredTop  && !hasBeenScored)
			{
				GetComponent<Renderer>().material = MaterialBallScored;
				DoReport(String.Format("{0}, {1}, {2}", SuccessCount++, Distance, Force.y));
			}
			hasBeenScored = true;
		}
	}

	void DoReport(string info)
	{
		Debug.Log(info);
		File.AppendAllText("successful_shots.csv", info += "\n");
	}

	IEnumerator DoReport()
	{
		using (WWW www = new WWW("localhost:8080/success?distance=" + Distance + "&force=" + Force.y))
		{
			yield return www;
			Debug.Log("Sent!");
		}
	}
}
