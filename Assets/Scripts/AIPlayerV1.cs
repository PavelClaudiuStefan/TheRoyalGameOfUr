using UnityEngine;
using System.Collections.Generic;

public class AIPlayerV1 : AIPlayer
{
	Dictionary<GameTile, float> tileDanger;

	override protected PlayerPiece PickStoneToMove( PlayerPiece[] legalPieces )
	{

		if(legalPieces == null || legalPieces.Length == 0)
		{
			Debug.LogError("I can't pick from an empty list");
			return null;
		}

		CalcTileDanger( legalPieces[0].PlayerId );

		// For each stone, we rank how good it would be to pick it, where 1 is super-awesome and -1 is horrible.
		PlayerPiece bestStone = null;
		float goodness = -Mathf.Infinity;

		foreach(PlayerPiece ps in legalPieces)
		{
			float g = GetStoneGoodness(ps, ps.CurrentTile, ps.GetTileAhead() );
			if(bestStone == null || g > goodness)
			{
				bestStone = ps;
				goodness = g;
			}
		}

		return bestStone;
	}

	virtual protected void CalcTileDanger( int myPlayerId )
	{
		tileDanger = new Dictionary<GameTile, float>();

		GameTile[] tiles = GameObject.FindObjectsOfType<GameTile>();

		foreach(GameTile t in tiles)
		{
			tileDanger[t] = 0;
		}


		PlayerPiece[] allPieces = GameObject.FindObjectsOfType<PlayerPiece>();

		foreach(PlayerPiece piece in allPieces)
		{
			if(piece.PlayerId == myPlayerId)
				continue;

			// This is an enemy stone, add a "danger" value to tiles in front of it (unless safe)

			for (int i = 1; i <= 4; i++)
			{
				GameTile t = piece.GetTileAhead(i);

				if( t == null )
				{
					// This tile (and subsequent tiles) are invalid, so we can just bail
					break;
				}

				if( t.IsScoringTile || t.IsSideline || t.IsRollAgain )
				{
					// This tile is not a danger zone, so we can ignore it.
					continue;
				}

				// Okay, this tile is within bopping range of an enemy, so it's dangerous.
				if(i == 2)
				{
					// 2 tiles is most likely, so most dangerous!
					tileDanger[t] += 0.3f;
				}
				else
				{
					tileDanger[t] += 0.2f;
				}
			}
		}
	}

	virtual protected float GetStoneGoodness( PlayerPiece stone, GameTile currentTile, GameTile futureTile )
	{
		float goodness = 0;//Random.Range(-0.1f, 0.1f);

		if( currentTile == null )
		{
			// We aren't on the board yet, and it's always nice to add more to the board to open up more options.
			goodness += 0.20f;
		}

		if( currentTile != null && (currentTile.IsRollAgain == true && currentTile.IsSideline == false) )
		{
			// We are sitting on a roll-again space in the middle.  Let's resist moving just because
			// it blocks the space from our opponent
			goodness -= 0.10f;
		}

		if( futureTile.IsRollAgain == true )
		{
			goodness += 0.50f;
		}

		if( futureTile.PlayerPiece != null && futureTile.PlayerPiece.PlayerId != stone.PlayerId )
		{
			// There's an enemy stone to bop!
			goodness += 0.50f;
		}

		if( futureTile.IsScoringTile == true )
		{
			goodness += 0.50f;
		}

		float currentDanger = 0;
		if(currentTile != null)
		{
			currentDanger = tileDanger[currentTile];
		}

		goodness += currentDanger - tileDanger[futureTile];

		// TODO:  Add goodness for tiles that are behind enemies, and therefore likely to contribute to future boppage
		// TODO:  Add goodness for moving a stone forward when we might be blocking friendlies

		return goodness;
	}


}