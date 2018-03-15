using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPiece : MonoBehaviour {

	public GameTile StartingTile;
	GameTile currentTile;

	bool isPoint = false;

	StateManager stateManager;

	GameTile[] moveQueue;
	int moveQueueIndex;

	bool isAnimating = false;

	Vector3 targetPosition;
	Vector3 velocity = Vector3.zero;
	float smoothTimeHorizontal = 0.25f;
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
			// If on finalTile -> lower piece
			if ((moveQueue == null || moveQueueIndex == moveQueue.Length) && this.transform.position.y > smoothDistance) {
				this.transform.position = Vector3.SmoothDamp (
					this.transform.position, 
					new Vector3 (this.transform.position.x, 0, this.transform.position.z), 
					ref velocity, 
					smoothTimeVertical);
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
				SetTargetPosition (this.transform.position + Vector3.right * 50);
			} else {
				SetTargetPosition (nextTile.transform.position);
				moveQueueIndex++;
			}
		} else {
			isAnimating = false;
			stateManager.isDoneAnimating = true;
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

		if (!stateManager.isDoneRolling) {
			return;
		}

		if (stateManager.isDoneClicking) {
			return;
		}

        int spacesToMove = stateManager.DiceSum;
		if (spacesToMove == 0) {
			return;
		}

		moveQueue = new GameTile[spacesToMove];
		GameTile finalTile = currentTile;

        for (int i = 0; i < spacesToMove; i++)
        {
            if (finalTile == null)
            {
                finalTile = StartingTile;
            } else {
                if (finalTile.NextTiles == null || finalTile.NextTiles.Length == 0)
                {
                    // TODO - Implement scoring
					finalTile = null;
					isPoint = true;
                }
                else if (finalTile.NextTiles.Length > 1)
                {
					finalTile = finalTile.NextTiles[stateManager.CurrentPlayerId];
                }
                else
                {
                    finalTile = finalTile.NextTiles[0];
                }
            }
			moveQueue[i] = finalTile;
        }

		// TODO - Check to see if the destination is legal

		moveQueueIndex = 0;
        currentTile = finalTile;
		stateManager.isDoneClicking = true;
		isAnimating = true;
    }
}
