using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPiece : MonoBehaviour {

	public GameTile StartingTile;
	GameTile currentTile;

	bool isPoint = false;

	DiceRoller diceRoller;

	GameTile[] moveQueue;
	int moveQueueIndex;

	Vector3 targetPosition;
	Vector3 velocity = Vector3.zero;
	float smoothTimeHorizontal = 0.25f;
	float smoothTimeVertical = 0.1f;
	float smoothDistance = 0.01f;
	float smoothHeight = 0.5f;

	// Use this for initialization
	void Start () {
        diceRoller = GameObject.FindObjectOfType<DiceRoller>();
		targetPosition = this.transform.position;
	}
	
	// Update is called once per frame
	void Update () {
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
				SetTargetPosition(this.transform.position + Vector3.right*50);
			} else {
				SetTargetPosition (nextTile.transform.position);
				moveQueueIndex++;
			}
		}
	}

	void SetTargetPosition(Vector3 pos) {
		targetPosition = pos;
		velocity = Vector3.zero;
	}

    void OnMouseUp()
    {
        //TODO - Get out if click is on UI

        int spacesToMove = diceRoller.DiceSum;

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
            } else
            {
                if (finalTile.NextTiles == null || finalTile.NextTiles.Length == 0)
                {
                    // TODO - Implement scoring
					finalTile = null;
					isPoint = true;
                }
                else if (finalTile.NextTiles.Length > 1)
                {
                    // TODO - Implement nextTile choosing based on player
                    finalTile = finalTile.NextTiles[0];
                }
                else
                {
                    finalTile = finalTile.NextTiles[0];
                }
            }
			moveQueue[i] = finalTile;
        }

		moveQueueIndex = 0;
        currentTile = finalTile;
    }
}
