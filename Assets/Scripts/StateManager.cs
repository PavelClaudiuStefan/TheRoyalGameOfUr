using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;
using UnityEngine.UI;

public class StateManager : MonoBehaviour {

	public int CurrentPlayerId = 0;
	public string PlayerOneName;
	public string PlayerTwoName;
	public int[] PlayersScores;
    public Text playerNameAndScore;
    public Text averageScore;
    public String aScore;
    public bool start = false;
    public bool averageScoreSet = false;

	AIPlayer[] PlayerAIs;

	public int DiceSum;
	public bool IsDoneRolling = false;
	public bool IsDoneClicking = false;
	public int PlayingAnimations = 0;
    public Client client;

	private float noLegalMovesDuration = 0.1f;  // How long the "No legal moves" message is displayed

	public GameObject NoLegalMovesMessage;
	public GameObject GameOverMessage;

	int numberOfGamePieces = 6;

	void Start () {
		PlayersScores = new int[2];
		PlayersScores [0] = 0;
		PlayersScores [1] = 0;

		PlayerAIs = new AIPlayer[2];
		PlayerAIs [0] = null;				// Human player
        //PlayerAIs [0] = new AIPlayerV1();
        PlayerAIs [1] = new AIPlayerV1();       // Ai

        startClient();
	}

	void Update () {
        // PlayerOne won
        if (PlayersScores[0] == numberOfGamePieces && start) {
			GameOverMessage.GetComponentInChildren<Text>().text = PlayerOneName + " won!!!";
			GameOverMessage.SetActive (true);
            if (client.getConnection())
            {
                client.send(PlayerOneName,(numberOfGamePieces-PlayersScores[1]));
            }
            start = false;
			return;
		}

		// PlayerTwo won
		if (PlayersScores[1] == numberOfGamePieces && start ) {
			GameOverMessage.GetComponentInChildren<Text>().text = PlayerTwoName + " won!!!";
			GameOverMessage.SetActive (true);
            if (client.getConnection())
            {
                client.send(PlayerOneName, (numberOfGamePieces - PlayersScores[1]));
            }
            start = false;
            return;
		}

		if (IsDoneRolling && IsDoneClicking && PlayingAnimations == 0 && start) {
			NewTurn();
		}

		if (PlayerAIs[CurrentPlayerId] != null && start) {
            PlayerAIs[CurrentPlayerId].Play();
        }

        if (start)
        {
            SetScoreAndName();
        }

        if (averageScoreSet==true)
        {
            if (client.getConnection())
            {
                client.send(PlayerOneName, -1);
                Debug.Log("aici1");
                aScore = client.read();
                Debug.Log("aici2");
            }
            averageScoreSet = false;
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
		yield return new WaitForSeconds (noLegalMovesDuration);
		NoLegalMovesMessage.SetActive (false);

		NewTurn();
	}

    private void startClient()
    {
        try
        {
            client = new Client();
            client.Start();
            client.connect();
        }
        catch (SocketException)
        {
            Debug.Log("Failed to connect to server");
        }
    }

    public void SetScoreAndName()
    {
        if (CurrentPlayerId == 0)
        {
            averageScore.gameObject.SetActive(true);
            playerNameAndScore.text = "Current player: " + PlayerOneName + " (" + PlayersScores[0] + ")";
            averageScore.text = "Average score:     "+aScore;
        }
        else if (CurrentPlayerId == 1)
        {
            playerNameAndScore.text = "Current player: " + PlayerTwoName + " (" + PlayersScores[1] + ")";
            averageScore.gameObject.SetActive(false);
        }
    }
}
