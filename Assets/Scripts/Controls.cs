using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controls : MonoBehaviour {

    public bool IsTouch = false; //Defaults is 'false' for editor.

    private Touch[] allInputs;
    private int totalInputs;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        readForInputs();
	}

    private void readForInputs()
    {
        if (IsTouch)
        {
            allInputs = Input.touches;
            totalInputs = Input.touchCount;

        }
        else
        {

        }
    }

}



/*
 * private void readScreenActivity()
    {
        wasRightActive = IsRightActive;
        wasLeftActive = IsLeftActive;
        isLeftActive = false;
        isRightActive = false;

        allInputs = Input.touches;
        totalInputs = Input.touchCount;

        Debug.Log("total inputs is: " + totalInputs);

        //Update left region and right region activity.
        // Each region has it's own unique location.
        // Each region has 2 variables: If Touch down and 
        for (inputNum = 0; inputNum < totalInputs; inputNum++)
        {
            tempTouch = allInputs[inputNum];

            if (tempTouch.position.x < rgnLength)
            {
                if (tempTouch.position.y < leftRgnHeight)
                {
                    LeftScnPos = tempTouch.position;
                    isLeftActive = true;
                }
            }
            else
            {
                if (tempTouch.position.y < rightRgnHeight)
                {
                    RightScnPos = tempTouch.position;
                    isRightActive = true;
                }
            }
        }
        //Set boolean triggers
        isLeftToggledOn = !wasLeftActive && isLeftActive ? true : false;
        isLeftToggledOff = wasLeftActive && !isLeftActive ? true : false;
        isRightToggledOn = !wasRightActive && isRightActive ? true : false;
        isRightToggledOff = wasRightActive && !isRightActive ? true : false;
    }

    private void assignScreenActivityPCTEST()
    {
        wasRightActive = IsRightActive;
        wasLeftActive = IsLeftActive;
        isLeftActive = false;
        isRightActive = false;
        
        if (Input.mousePosition.x < rgnLength)
        {
            if (Input.mousePosition.y < leftRgnHeight)
            {
                LeftScnPos = Input.mousePosition;
                isLeftActive = true;
            }
        }
        else
        {
            if (Input.mousePosition.y < rightRgnHeight)
            {
                RightScnPos = Input.mousePosition;
                isRightActive = true;
            }
        }

        //Set boolean triggers
        isLeftToggledOn = !wasLeftActive && isLeftActive ? true : false;
        isLeftToggledOff = wasLeftActive && !isLeftActive ? true : false;
        isRightToggledOn = !wasRightActive && isRightActive ? true : false;
        isRightToggledOff = wasRightActive && !isRightActive ? true : false;
    }
*/