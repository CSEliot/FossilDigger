using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LoLSDK;

public class Master : MonoBehaviour {

    public QuizMan _QuizMan;

	// Use this for initialization
	void Start () {
        LOLSDK.Init("com.kiteliongames.fossildigger");
        CBUG.Do("PreQHandle");
        LOLSDK.Instance.QuestionsReceived += new QuestionListReceivedHandler(this.QuestionsReceived);
        CBUG.Do("PostQHandle");
        LOLSDK.Instance.SubmitProgress(0, 0, 8);
        //AudioManager.PlayM(0);
    }
	
	// Update is called once per frame
	void Update () {

        //Arbitrarily Catch Debug - Breakpoint!
        if (Input.GetKeyDown("k"))
            CBUG.Do("Debug!");

	}

    public void QuestionsReceived(MultipleChoiceQuestionList questionList)
    {
        Debug.Log("Questions Received!");
        _QuizMan.GetQuestions(questionList);
    }
}
