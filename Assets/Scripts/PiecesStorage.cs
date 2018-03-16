using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PiecesStorage : MonoBehaviour {

	public GameObject PiecePrefab;
	public GameTile StartingTile;

	void Start () {

		// Initialize one piece for each PiecePosition
		for (int i = 0; i < this.transform.childCount; i++) {
			GameObject piece = Instantiate (PiecePrefab);
			piece.GetComponent<PlayerPiece>().StartingTile = this.StartingTile;
			piece.GetComponent<PlayerPiece> ().PiecesStorage = this;
			AddPieceToStorage (piece, this.transform.GetChild (i));
		}

	}

	void Update () {
		
	}

	public void AddPieceToStorage(GameObject piece, Transform piecePosition=null) {

		if (piecePosition == null) {
			//Find empty PiecePosition
			for (int i = 0; i < this.transform.childCount; i++) {
				Transform pos = this.transform.GetChild (i);
				if (pos.childCount == 0) {
					piecePosition = pos;
					break;
				}
			}

			if (piecePosition == null) {
				Debug.LogError ("No empty places to store piece");
			}
		}

		// Parent the piece to the PiecePosition
		piece.transform.SetParent (piecePosition);

		// Reset piece's local position to 0 0 0
		piece.transform.localPosition = Vector3.zero;
	}
}
