using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CurrentPlayerDisplay : MonoBehaviour {

	private StateManager stateManager;
	private Text displayText;
    private InputField inputField;
    public GameObject inGame;
    public GameObject startGame;
    private InputField.SubmitEvent se;

    void Start () {
		stateManager = GameObject.FindObjectOfType<StateManager>();
        inputField = GetComponent<InputField>();
        se = new InputField.SubmitEvent();
        se.AddListener(SetName);
        inputField.onEndEdit = se;

    }

    private void SetName(string arg0)
    {
        stateManager.PlayerOneName = arg0;
        startGame.SetActive(false);
        inGame.SetActive(true);
        stateManager.start = true;
        stateManager.averageScoreSet = true;
    }

}
