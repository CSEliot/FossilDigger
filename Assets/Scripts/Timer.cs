using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using LoLSDK;

public class Timer : MonoBehaviour {

    public GameManager _GameManager;

    public Text AreYouSureText;
    public Text AreYouSureTextBG;

    public Text HowManyMinutesText;
    public Text HowManyMinutesTextBG;

    public GameObject[] DisableOnStart;
    public GameObject EnableOnNo;
    public GameObject DisableOnNo;

    private int totalMinutes;
    private int secondsRemaining;

    public Text TimeText;
    public Text TimeTextBG;

    public bool timerStarted;

    private float progressThreshold;

    private bool paused;

    public GameObject PauseOverlay;

	// Use this for initialization
	void Start () {
        totalMinutes = 1;
        timerStarted = false;
        progressThreshold = 0.10f;
        LOLSDK.Instance.GameStateChanged += new GameStateChangedHandler(PauseIf);
    }
	
	// Update is called once per frame
	void FixedUpdate () {
        if (timerStarted && !paused)
        {
            secondsRemaining--;
            TimeText.text = "" + secondsRemaining;
            TimeTextBG.text = "" + secondsRemaining;
            float tempCurrentProgress = (((float)(totalMinutes * 60f) - secondsRemaining)) / ((float)totalMinutes * 60f);
            if (tempCurrentProgress >= progressThreshold)
            {
                progressThreshold += 0.10f;
                CBUG.Do("ProgresS: " + tempCurrentProgress);
                LOLSDK.Instance.SubmitProgress((int)_GameManager.DeepestDepth, (int)(progressThreshold * 10) - 1, 10);
            }
            if (secondsRemaining == 0)
            {
                LOLSDK.Instance.SubmitProgress((int)_GameManager.DeepestDepth, 10, 10);
                _GameManager.GameOver();
                timerStarted = false;
                TimeText.text = "Fossil Explorer";
                TimeTextBG.text = "Fossil Explorer";
            }
        }
	}

    public void SetAreYouSure()
    {
        AreYouSureText.text = totalMinutes + " minutes. \nAre you sure?";
        AreYouSureTextBG.text = totalMinutes + " minutes. \nAre you sure?";
    }

    public void IncreaseMinutes()
    {
        totalMinutes++;
        HowManyMinutesText.text = "How many minutes?\n" + totalMinutes;
        HowManyMinutesTextBG.text = "How many minutes?\n" + totalMinutes;
    }

    public void DecreaseMinutes()
    {
        if(totalMinutes != 1)
            totalMinutes--;
        HowManyMinutesText.text = "How many minutes?\n" + totalMinutes;
        HowManyMinutesTextBG.text = "How many minutes?\n" + totalMinutes;
    }

    public void Yes()
    {
        if (totalMinutes == 0)
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
        secondsRemaining = totalMinutes * 60;
    }

    public void PauseIf(GameState gameState)
    {
        switch (gameState)
        {
            case GameState.Paused:
                {
                    paused = true;
                    PauseOverlay.SetActive(true);
                }
                break;

            case GameState.Resumed:
                {
                    paused = false;
                    PauseOverlay.SetActive(false);
                }
                break;
        }
    }
}
