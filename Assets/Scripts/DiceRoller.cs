using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DiceRoller : MonoBehaviour {

	// Use this for initialization
	void Start () {
        DiceValues = new int[4];
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public int[] DiceValues;
    public int DiceSum;

    public Text DiceDisplay;

    public Sprite DiceImageOne;
    public Sprite DiceImageZero;

    public void NewTurn()
    {

    }

    public void RollDice()
    {
        // In The Royal Game of Ur, there are 4 dices (tetrahedons), which have half the faces with a value of 1 and the rest of 0

        DiceSum = 0;
        for (int i = 0; i < DiceValues.Length; i++)
        {
            DiceValues[i] = Random.Range(0, 2);
            DiceSum += DiceValues[i];

            if (DiceValues[i] == 0)
            {
                this.transform.GetChild(i).GetComponent<Image>().sprite = DiceImageZero;
            } else
            {
                this.transform.GetChild(i).GetComponent<Image>().sprite = DiceImageOne;
            }
        }

        DiceDisplay.text = DiceSum.ToString();
    }

}
