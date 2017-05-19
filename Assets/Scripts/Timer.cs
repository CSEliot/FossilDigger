using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using LoLSDK;

public class Timer : MonoBehaviour {

    public GameManager _GameManager;

    public Text AreYouSure;
    public Text AreYouSureBG;
    public InputField MinutesField;

    public GameObject[] DisableOnStart;
    public GameObject EnableOnNo;
    public GameObject DisableOnNo;

    private int minutes;
    private int secondsRemaining;

    public Text TimeText;
    public Text TimeTextBG;

    public bool timerStarted;

	// Use this for initialization
	void Start () {
        MinutesField.characterLimit = 2;
        minutes = 0;
        timerStarted = false;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        if (timerStarted)
        {
            secondsRemaining--;
            TimeText.text = "" + secondsRemaining;
            TimeTextBG.text = "" + secondsRemaining;
            if (secondsRemaining % 20 == 0)
                LOLSDK.Instance.SubmitProgress((int)_GameManager.Depth, secondsRemaining / (minutes * 60), 100);
            if (secondsRemaining == 0)
                _GameManager.GameOver();
        }
	}

    public void SetAreYouSure()
    {
        AreYouSure.text = MinutesField.text + " minutes. \nAre you sure?";
        AreYouSureBG.text = MinutesField.text + " minutes. \nAre you sure?";
        minutes = System.Convert.ToInt32(MinutesField.text);
    }

    public void Yes()
    {
        if (minutes == 0)
        {
            No();
            return;
        }

        for(int x = 0; x < DisableOnStart.Length; x++)
        {
            DisableOnStart[x].SetActive(false);
        }
        startTimer();
    }

    public void No ()
    {
        EnableOnNo.SetActive(true);
        DisableOnNo.SetActive(false);
    }

    private void startTimer()
    {
        timerStarted = true;
        secondsRemaining = minutes * 60;
    }
}
