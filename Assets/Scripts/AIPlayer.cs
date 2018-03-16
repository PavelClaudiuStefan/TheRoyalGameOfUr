using UnityEngine;
using System.Collections.Generic;

public class AIPlayer
{
	StateManager stateManager;


	public AIPlayer () {
		stateManager = GameObject.FindObjectOfType<StateManager> ();
	}


	virtual public void Play() {
		if (stateManager.IsDoneRolling == false) {
			DoRoll ();
		}

		if (stateManager.IsDoneClicking == false) {
			DoPieceMove ();
		}
	}


	virtual protected void DoRoll() {
		GameObject.FindObjectOfType<DiceRoller> ().RollDice ();
	}


	virtual protected void DoPieceMove() {
		// Pick a piece and move it

		PlayerPiece[] legalMoves = GetLegalMoves ();

		if (legalMoves == null || legalMoves.Length == 0) {
			// I should not get here
			return;
		}

		// BasicAI - picks random piece that can legally move
		PlayerPiece chosenPiece = legalMoves[Random.Range (0, legalMoves.Length)];
		chosenPiece.Move ();
	}

	// Returns list of pieces that can be moved
	protected PlayerPiece[] GetLegalMoves() {

		List<PlayerPiece> legalPieces = new List<PlayerPiece> ();

		// A zero is rolled -> No legal moves
		if (stateManager.DiceSum == 0) {
			return legalPieces.ToArray ();
		}

		PlayerPiece[] pieces = GameObject.FindObjectsOfType<PlayerPiece> ();
		foreach (PlayerPiece piece in pieces) {
			if (piece.PlayerId == stateManager.CurrentPlayerId) {
				if (piece.CanLegallyMove(stateManager.DiceSum)) {
					legalPieces.Add (piece);
				}
			}
		}
		return legalPieces.ToArray ();
	}
}

