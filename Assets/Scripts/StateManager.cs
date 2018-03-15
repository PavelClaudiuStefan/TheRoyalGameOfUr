using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateManager : MonoBehaviour {

	public int CurrentPlayerId = 0;

	public int DiceSum;
	public bool isDoneRolling = false;
	public bool isDoneClicking = false;
	public bool isDoneAnimating = false;

	void Start () {
		
	}

	public void NewTurn() {
		isDoneRolling = false;
		isDoneClicking = false;
		isDoneAnimating = false;

		CurrentPlayerId = (CurrentPlayerId + 1) % 2;
	}

	void Update () {
		if (isDoneRolling && isDoneClicking && isDoneAnimating) {
			NewTurn();
		}
	}
}
