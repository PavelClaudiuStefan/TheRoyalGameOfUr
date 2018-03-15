using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CurrentPlayerDisplay : MonoBehaviour {

	StateManager stateManager;

	void Start () {
		stateManager = GameObject.FindObjectOfType<StateManager>();
	}
	
	// Update is called once per frame
	void Update () {
		if (stateManager.CurrentPlayerId == 0) {
			GetComponent<Text>().text = "Current player: One";
		} else {
			GetComponent<Text>().text = "Current player: Two";
		}
	}
}
