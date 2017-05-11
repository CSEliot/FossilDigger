using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LoLSDK;

public class GameManager : MonoBehaviour {

    public QuizMan _QuizMan;
    public CharCon Player;

    public int StartingEnergy;
    public int StartingHealth;
    public int StartingDepth;
    public int StartingAbsoluteAge;

    private float depth;
    private float absoluteAge;

    private int energy;
    private int health;
    private int maxEnergy;
    private int maxHealth;


    // Use this for initialization
    void Start () {
        LOLSDK.Init("com.kiteliongames.fossildigger");
        CBUG.Do("PreQHandle");
        LOLSDK.Instance.QuestionsReceived += new QuestionListReceivedHandler(this.QuestionsReceived);
        CBUG.Do("PostQHandle");
        LOLSDK.Instance.SubmitProgress(0, 0, 8);
        HUDMan.SetAbsoluteAge(StartingAbsoluteAge);
        HUDMan.SetDepth(StartingDepth);
        HUDMan.SetEnergy(StartingEnergy, StartingEnergy);
        HUDMan.SetHealth(StartingHealth, StartingHealth);
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

    public void Update(BoardMan.Direction Direction, Item.Type TargetItem)
    {
        //where should item handle code go? Does GameManager actually do the item action?
        if(Direction != BoardMan.Direction.None)
            CharCon.Move(Direction);

        switch (TargetItem)
        {
            case global::Item.Type.
            default:
                CBUG.Error("Bad Item type given! " + TargetItem.ToString());
                break;
        }
    }

    public void Update(Item.Type Item)
    {
        //where should item handle code go? Does GameManager actually do the item action?
        Update(BoardMan.Direction.None, Item);
    }

    private void assignStartingValues()
    {

    }
}
