using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CurrentPlayerDisplay : MonoBehaviour {

	StateManager stateManager;
	Text displayText;

	void Start () {
		stateManager = GameObject.FindObjectOfType<StateManager>();
		displayText = GetComponent<Text> ();

	}
	
	// Update is called once per frame
	void Update () {
		if (stateManager.CurrentPlayerId == 0) {
			displayText.text = "Current player: " + stateManager.PlayerOneName + " (" + stateManager.PlayersScores[0] + ")";
		} else {
			displayText.text = "Current player: " + stateManager.PlayerTwoName + " (" + stateManager.PlayersScores[1] + ")";
		}
	}
}
