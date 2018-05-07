using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPiece : MonoBehaviour {

	public GameTile StartingTile;
	public GameTile CurrentTile { get; protected set; }

	public int PlayerId;
	public PiecesStorage PiecesStorage;

	StateManager stateManager;

	GameTile[] moveQueue;
	int moveQueueIndex;

	bool isAnimating = false;

	PlayerPiece pieceToTakeout;

	Vector3 targetPosition;
	Vector3 velocity = Vector3.zero;
	//float smoothTimeHorizontal = 0.2f;
	//float smoothTimeVertical = 0.1f;
	float smoothDistance = 0.01f;
	float smoothHeight = 0.5f;

	// Testing - faster animations
	float smoothTimeHorizontal = 0.1f;
	float smoothTimeVertical = 0.1f;

	void Start () {
		stateManager = GameObject.FindObjectOfType<StateManager>();
		targetPosition = this.transform.position;
	}

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
			stateManager.PlayingAnimations--;

			if (CurrentTile != null && CurrentTile.IsRollAgain) {
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

		this.Move ();
    }

	public void Move() {
		// Is this the correct player?
		if (stateManager.CurrentPlayerId != PlayerId) {
			return;
		}

		// Have we rolled the dice?
		if (stateManager.IsDoneRolling == false) {
			// We can't move yet.
			return;
		}
		if (stateManager.IsDoneClicking == true) {
			// We've already done a move!
			return;
		}

		int spacesToMove = stateManager.DiceSum;
		if (spacesToMove == 0) {
			return;
		}

		// Where should we end up?
		moveQueue = GetTilesAhead(spacesToMove);
		GameTile finalTile = moveQueue[ moveQueue.Length-1 ];

		if (finalTile == null) {
			return;
		} else {
			if(CanLegallyMoveTo(finalTile) == false) {
				// Not allowed!
				finalTile = CurrentTile;
				moveQueue = null;
				return;
			}

			// If there is an enemy tile in our legal space, the we kick it out.
			if(finalTile.PlayerPiece != null) {
				//finalTile.PlayerStone.ReturnToStorage();
				pieceToTakeout = finalTile.PlayerPiece;
				pieceToTakeout.CurrentTile.PlayerPiece = null;
				pieceToTakeout.CurrentTile = null;
			}
		}

		this.transform.SetParent(null); // Become Batman

		// Remove ourselves from our old tile
		if(CurrentTile != null)
		{
			CurrentTile.PlayerPiece = null;
		}

		// Even before the animation is done, set our current tile to the new tile
		CurrentTile = finalTile;
		if( finalTile.IsScoringTile == false )   // "Scoring" tiles are always "empty"
		{
			finalTile.PlayerPiece = this;
		}

		moveQueueIndex = 0;

		stateManager.IsDoneClicking = true;
		this.isAnimating = true;
		stateManager.PlayingAnimations++;

		if (finalTile.IsScoringTile) {
			stateManager.PlayersScores[PlayerId]++;
		}
	}

	GameTile[] GetTilesAhead (int spacesToMove) {
		if (spacesToMove == 0) {
			return null;
		}

		GameTile[] listOfTiles = new GameTile[spacesToMove];
		GameTile finalTile = CurrentTile;

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

	public GameTile GetTileAhead() {
		return GetTileAhead (stateManager.DiceSum);
	}

	public GameTile GetTileAhead(int numberOfTiles) {
		GameTile[] tiles = GetTilesAhead (numberOfTiles);

		if (tiles == null) {
			return CurrentTile;
		}

		return tiles [tiles.Length-1];
	}

	public void ReturnToStorage() {
		this.isAnimating = true;
		stateManager.PlayingAnimations++;

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
		if (CurrentTile != null && CurrentTile.IsScoringTile) {
			// Piece is on a scoring tile so it can't be moved
			return false;
		}

		GameTile tile = GetTileAhead (numberOfTiles);
		return CanLegallyMoveTo (tile);
	}
}
