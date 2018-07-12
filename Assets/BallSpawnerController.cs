using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;
using TensorFlow;

[System.Serializable]
class Prediction
{
	public float result;
}

public class BallSpawnerController : MonoBehaviour
{
	public Transform TransformGoal;
	public Transform TransformAim;
	public GameObject PrefabBall;

	[Range(0, 10)]
	public float maxVariance;
	
	private float test = 1f;

	private TFGraph graph;
	private TFSession session;
	
	void Start ()
	{	
		File.WriteAllText("successful_shots.csv", "");
		TextAsset graphModel = Resources.Load ("frozen.pb") as TextAsset;
		graph = new TFGraph ();
		graph.Import (graphModel.bytes);
		
		session = new TFSession (graph);
		
		StartCoroutine(DoShoot());
	}

	private void Update()
	{
		TransformAim.LookAt(TransformGoal);
	}

	IEnumerator DoShoot()
	{
		while (true)
		{
//			yield return new WaitUntil(() => !Input.GetButton("Jump"));
//			yield return new WaitUntil(() => Input.GetButton("Jump"));

			var gv2 = new Vector2(
				TransformGoal.position.x,
				TransformGoal.position.z);

			var tv2 = new Vector2(
				transform.position.x, transform.position.z);

			var dir = (gv2 - tv2).normalized;
			var dist = (gv2 - tv2).magnitude;
			var arch = 0.5f;

			var closeness = Math.Min(10f, dist) / 10f;
			
			float force = GetForceFromTensorFlow(dist);

			var ball = Instantiate(PrefabBall, transform.position, Quaternion.identity);
			var bc = ball.GetComponent<BallController>();
			bc.Force = new Vector3(
				dir.x * arch * closeness,
				force,//* (1f / closeness)
				dir.y * arch * closeness
			);
			bc.Distance = dist;
			
			yield return new WaitForSeconds(0.02f);
			MoveToRandomDistance();
		}
	}

	float GetForceFromTensorFlow(float distance)
	{
		var runner = session.GetRunner ();

		runner.AddInput (
			graph["shots_input"][0], 
			new float[1,1]{{distance}}
		);
		runner.Fetch (graph ["shots/BiasAdd"] [0]);
		float[,] recurrent_tensor = runner.Run () [0].GetValue () as float[,];
		var force = recurrent_tensor[0, 0] / 10;
		Debug.Log(String.Format("{0}, {1}", distance, force));
		return force;
	}

	float GetForceRandomly(float distance)
	{
		return Random.Range(0f, 1f);
	}
	
	float GetForceFromMagicFormula(float distance)
	{
		var variance = Random.Range(1f, maxVariance);
		return (0.125f) + (0.0317f * distance * variance);
	}

	void MoveToRandomDistance()
	{
		var newPosition = new Vector3(TransformGoal.position.x + Random.Range(2.5f, 23f), transform.parent.position.y, TransformGoal.position.z);
		transform.parent.position = newPosition;
	}
}
