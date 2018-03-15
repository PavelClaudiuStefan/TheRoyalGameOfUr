using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DiceRoller : MonoBehaviour {

	StateManager stateManager;

	public int[] DiceValues;

	public Sprite DiceImageOne;
	public Sprite DiceImageZero;

	void Start () {
        DiceValues = new int[4];
		stateManager = GameObject.FindObjectOfType<StateManager>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	/// Roll 4 dices (tetrahedons with half the vertices with a value of 1 and the rest of 0
	/// The returned value is the number of vertices with the value of 1 pointing up
    public void RollDice()
    {
		if (stateManager.isDoneRolling) {
			return;
		}

		stateManager.DiceSum = 0;
        for (int i = 0; i < DiceValues.Length; i++)
        {
            DiceValues[i] = Random.Range(0, 2);
			stateManager.DiceSum += DiceValues[i];

            if (DiceValues[i] == 0)
            {
                this.transform.GetChild(i).GetComponent<Image>().sprite = DiceImageZero;
            } else
            {
                this.transform.GetChild(i).GetComponent<Image>().sprite = DiceImageOne;
            }
        }

		stateManager.isDoneRolling = true;
    }

}
