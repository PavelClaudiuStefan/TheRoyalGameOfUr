using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateManager : MonoBehaviour {

	public int CurrentPlayerId = 0;
	public string PlayerOneName;
	public string PlayerTwoName;

	AIPlayer[] PlayerAIs;

	public int DiceSum;
	public bool IsDoneRolling = false;
	public bool IsDoneClicking = false;
	public int PlayingAnimations = 0;

	public GameObject NoLegalMovesMessage;

	void Start () {
		PlayerAIs = new AIPlayer[2];
		PlayerAIs [0] = null;		// Human player
		PlayerAIs [1] = new AIPlayer();		// Ai
	}

	void Update () {
		if (IsDoneRolling && IsDoneClicking && PlayingAnimations == 0) {
			NewTurn();
		}

		if (PlayerAIs[CurrentPlayerId] != null) {
			PlayerAIs [CurrentPlayerId].Play ();
		}
	}

	public void NewTurn() {
		IsDoneRolling = false;
		IsDoneClicking = false;

		CurrentPlayerId = (CurrentPlayerId + 1) % 2;
	}

	public void RollAgain() {
		IsDoneRolling = false;
		IsDoneClicking = false;
	}

	public void CheckLegalMoves() {
		// A zero is rolled -> No legal moves
		if (DiceSum == 0) {
			StartCoroutine (NoLegalMove());
			return;
		}

		PlayerPiece[] pieces = GameObject.FindObjectsOfType<PlayerPiece> ();
		bool hasLegalMove = false;
		foreach (PlayerPiece piece in pieces) {
			if (piece.PlayerId == CurrentPlayerId) {
				if (piece.CanLegallyMove (DiceSum)) {
					// TODO - Show stones that can be moved
					hasLegalMove = true;
				}
			}
		}

		if (hasLegalMove == false) {
			StartCoroutine (NoLegalMove());
			return;
		}

	}

	IEnumerator NoLegalMove() {
		NoLegalMovesMessage.SetActive (true);
		yield return new WaitForSeconds (1f);
		NoLegalMovesMessage.SetActive (false);

		NewTurn();
	}
}
