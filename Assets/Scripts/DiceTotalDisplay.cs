using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DiceTotalDisplay : MonoBehaviour {

	StateManager stateManager;

	void Start () {
		stateManager = GameObject.FindObjectOfType<StateManager> ();
	}

	void Update () {
		if (stateManager.isDoneRolling == false) {
			GetComponent<Text> ().text = "?";
		} else {
			GetComponent<Text> ().text = stateManager.DiceSum.ToString ();
		}
	}
}
