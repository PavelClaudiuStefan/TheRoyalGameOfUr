using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPiece : MonoBehaviour {

	public GameTile StartingTile;
	GameTile currentTile;

	public int PlayerId;
	public PiecesStorage PiecesStorage;

	bool isPoint = false;

	StateManager stateManager;

	GameTile[] moveQueue;
	int moveQueueIndex;

	bool isAnimating = false;

	PlayerPiece pieceToTakeout;

	Vector3 targetPosition;
	Vector3 velocity = Vector3.zero;
	float smoothTimeHorizontal = 0.2f;
	float smoothTimeVertical = 0.1f;
	float smoothDistance = 0.01f;
	float smoothHeight = 0.5f;

	// Use this for initialization
	void Start () {
		stateManager = GameObject.FindObjectOfType<StateManager>();
		targetPosition = this.transform.position;
	}
	
	// Update is called once per frame
	void Update () {
		if (isAnimating == false) {
			return;
		}

		if (Vector3.Distance(new Vector3 (this.transform.position.x, targetPosition.y, this.transform.position.z), targetPosition) < smoothDistance) {

			//
			if ((moveQueue == null || moveQueueIndex == (moveQueue.Length)) && (this.transform.position.y-smoothDistance) > targetPosition.y) {
				// We are above target
				// No more moves -> lower piece
				this.transform.position = Vector3.SmoothDamp (
					this.transform.position, 
					new Vector3 (this.transform.position.x, targetPosition.y, this.transform.position.z), 
					ref velocity, 
					smoothTimeVertical);

				// Check for takeouts
				if (pieceToTakeout != null) {
					pieceToTakeout.ReturnToStorage ();
					pieceToTakeout = null;
				}

			} else {
				// Height is raised -> advance piece
				AdvanceMoveQueue();
			}
		} else if (this.transform.position.y < smoothHeight - smoothDistance) {
			// Raise piece
			this.transform.position = Vector3.SmoothDamp (
				this.transform.position, 
				new Vector3(this.transform.position.x, smoothHeight, this.transform.position.z), 
				ref velocity, 
				smoothTimeVertical);
		} else {
			this.transform.position = Vector3.SmoothDamp (
				this.transform.position, 
				new Vector3(targetPosition.x, smoothHeight, targetPosition.z), 
				ref velocity, 
				smoothTimeHorizontal);
		}
	}

	void AdvanceMoveQueue() {
		if (moveQueue != null && moveQueueIndex < moveQueue.Length) {
			GameTile nextTile = moveQueue [moveQueueIndex];
			if (nextTile == null) {
				// TODO - Send piece to scoring stack
				Debug.Log ("Scored");
				SetTargetPosition (this.transform.position + Vector3.right * 10);
			} else {
				SetTargetPosition (nextTile.transform.position);
				moveQueueIndex++;
			}
		} else {
			isAnimating = false;
			stateManager.isDoneAnimating = true;

			if (currentTile != null && currentTile.IsRollAgain) {
				stateManager.RollAgain();
			}
		}
	}

	void SetTargetPosition(Vector3 position) {
		targetPosition = position;
		velocity = Vector3.zero;
		isAnimating = true;
	}

    void OnMouseUp()
    {
        //TODO - Get out if click is on UI

		// Check if piece belongs to right player
		if (stateManager.CurrentPlayerId != PlayerId) {
			return;
		}

		if (!stateManager.isDoneRolling) {
			return;
		}

		if (stateManager.isDoneClicking) {
			return;
		}

        int spacesToMove = stateManager.DiceSum;

		moveQueue = GetTilesAhead (spacesToMove);
		GameTile finalTile = moveQueue [moveQueue.Length - 1];

		if (finalTile == null) {
			// TODO - score me
			isPoint = true;
		} else  {
			if (CanLegallyMoveTo (finalTile) == false) {
				Debug.Log ("Not allowed");
				finalTile = currentTile;
				moveQueue = null;
				return;
			}

			if (finalTile.PlayerPiece != null) {
				pieceToTakeout = finalTile.PlayerPiece;
				pieceToTakeout.currentTile.PlayerPiece = null;
				pieceToTakeout.currentTile = null;
			}
		}

		this.transform.SetParent (null);

		if (currentTile != null) {
			currentTile.PlayerPiece = null;
		}

		finalTile.PlayerPiece = this;

		moveQueueIndex = 0;
        currentTile = finalTile;
		stateManager.isDoneClicking = true;
		isAnimating = true;
    }

	GameTile[] GetTilesAhead (int spacesToMove) {
		if (spacesToMove == 0) {
			return null;
		}

		GameTile[] listOfTiles = new GameTile[spacesToMove];
		GameTile finalTile = currentTile;

		for (int i = 0; i < spacesToMove; i++)
		{
			if (finalTile == null)
			{
				finalTile = StartingTile;
			} else {
				if (finalTile.NextTiles == null || finalTile.NextTiles.Length == 0)
				{
					// Overshooting last tile (scoring tile)
					break;
				}
				else if (finalTile.NextTiles.Length > 1)
				{
					finalTile = finalTile.NextTiles[PlayerId];
				}
				else
				{
					finalTile = finalTile.NextTiles[0];
				}
			}
			listOfTiles[i] = finalTile;
		}
		return listOfTiles;
	}

	GameTile GetTileAhead(int numberOfTiles) {
		GameTile[] tiles = GetTilesAhead (numberOfTiles);

		if (tiles == null) {
			return currentTile;
		}

		return tiles [tiles.Length-1];
	}

	public void ReturnToStorage() {
		Vector3 savedPosition = this.transform.position;

		PiecesStorage.AddPieceToStorage (this.gameObject);

		SetTargetPosition (this.transform.position);

		this.transform.position = savedPosition;
	}

	bool CanLegallyMoveTo(GameTile destinationTile) {

		if (destinationTile == null) {
			// Overshooting last tile (scoring tile)
			return false;
		}

		// Tile is empty
		if (destinationTile.PlayerPiece == null) {
			return true;
		}

		// Tile already has one piece that belongs to CurrentPlayerId
		if (destinationTile.PlayerPiece.PlayerId == this.PlayerId) {
			return false;
		}

		// TODO - Safe tiles

		if (destinationTile.IsRollAgain == true) {
			return false;
		}

		return true;
	}

	public bool CanLegallyMove(int numberOfTiles) {
		GameTile tile = GetTileAhead (numberOfTiles);
		return CanLegallyMoveTo (tile);
	}
}
