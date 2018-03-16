using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateManager : MonoBehaviour {

	public int CurrentPlayerId = 0;
	public string PlayerOneName;
	public string PlayerTwoName;

	public int DiceSum;
	public bool isDoneRolling = false;
	public bool isDoneClicking = false;
	public bool isDoneAnimating = false;

	public GameObject NoLegalMovesMessage;

	void Start () {
		
	}

	void Update () {
		if (isDoneRolling && isDoneClicking && isDoneAnimating) {
			NewTurn();
		}
	}

	public void NewTurn() {

		isDoneRolling = false;
		isDoneClicking = false;
		isDoneAnimating = false;

		CurrentPlayerId = (CurrentPlayerId + 1) % 2;
	}

	public void RollAgain() {
		isDoneRolling = false;
		isDoneClicking = false;
		isDoneAnimating = false;
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
