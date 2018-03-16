using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRig : MonoBehaviour {

	StateManager stateManager;

	public float PivotAngle = 45f;
	float pivotVelocity;

	void Start () {
		stateManager = GameObject.FindObjectOfType<StateManager>();
	}

	void Update () {

		float angle = transform.rotation.eulerAngles.y;
		if (angle > 180) {
			angle -= 360f;
		}

		if (stateManager.CurrentPlayerId == 0) {
			angle = Mathf.SmoothDamp (
				angle,
				PivotAngle,
				ref pivotVelocity, 
				0.25f);
		} else {
			angle = Mathf.SmoothDamp (
				angle,
				-PivotAngle,
				ref pivotVelocity, 
				0.25f);
		}


		transform.rotation = Quaternion.Euler (new Vector3 (0, angle, 0));
	}
}
