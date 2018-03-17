using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StateManager : MonoBehaviour {

	public int CurrentPlayerId = 0;
	public string PlayerOneName;
	public string PlayerTwoName;
	public int[] PlayersScores;

	AIPlayer[] PlayerAIs;

	public int DiceSum;
	public bool IsDoneRolling = false;
	public bool IsDoneClicking = false;
	public int PlayingAnimations = 0;

	public GameObject NoLegalMovesMessage;
	public GameObject GameOverMessage;

	int numberOfGamePieces = 6;

	void Start () {
		PlayersScores = new int[2];
		PlayersScores [0] = 0;
		PlayersScores [1] = 0;

		PlayerAIs = new AIPlayer[2];
		PlayerAIs [0] = null;				// Human player
        //PlayerAIs [0] = new AIPlayer();
        PlayerAIs [1] = new AIPlayer();		// Ai
	}

	void Update () {
		// PlayerOne won
		if (PlayersScores[0] == numberOfGamePieces) {
			GameOverMessage.GetComponentInChildren<Text>().text = PlayerOneName + " won!!!";
			GameOverMessage.SetActive (true);
			return;
		}

		// PlayerTwo won
		if (PlayersScores[1] == numberOfGamePieces) {
			GameOverMessage.GetComponentInChildren<Text>().text = PlayerTwoName + " won!!!";
			GameOverMessage.SetActive (true);
			return;
		}

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
