using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPiece : MonoBehaviour {

    DiceRoller diceRoller;
	GameTile currentTile;

	public GameTile StartingTile;

	// Use this for initialization
	void Start () {
        diceRoller = GameObject.FindObjectOfType<DiceRoller>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnMouseUp()
    {
        //TODO - Get out if click is on UI

        int spacesToMove = diceRoller.DiceSum;

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
                    Debug.Log("Point!");
                    Destroy(gameObject);
                    return;
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
        }

        if(finalTile == null)
        {
            return;
        }

        // Send piece to the final tile

        this.transform.position = finalTile.transform.position;
        currentTile = finalTile;
    }
}
