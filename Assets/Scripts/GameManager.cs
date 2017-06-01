using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using LoLSDK;

public class GameManager : MonoBehaviour {

    public QuizMan _QuizMan;
    public CharCon Player;
    public BoardMan _BoardMan;
    public SpawnMan _SpawnMan;
    public GameObject ZeroHealthAnimObj;
    public GameObject ZeroEnergyAnimObj;

    public int StartingEnergy;
    public int StartingHealth;
    public int StartingDepth;
    public int StartingAbsoluteAge;
    public int DepthPerRow;

    private float depth;
    private float deepestDepth;
    private float absoluteAge;

    private int energy;
    private int health;
    private int maxEnergy;
    private int maxHealth;

    public int ZeroHealthPunishment;
    public int ZeroEnergyPunishment;

    public GameObject GameOverScreen;
    public Text CongratsText;
    public Text CongratsTextBG;

    /// <summary>
    /// 0 - 17 in difficulty.
    /// </summary>
    private int maxDifficulty;
    public int DepthPerDifficulty;
    public int EnergySpawnLossPerDifficulty;
    private float origEnergySpawnRate;

    public int CorrectAnswerReward;

    // Use this for initialization
    void Start () {
        LOLSDK.Init("com.kiteliongames.fossilexplorer");
        CBUG.Do("PreQHandle");
        LOLSDK.Instance.QuestionsReceived += new QuestionListReceivedHandler(this.QuestionsReceived);
        CBUG.Do("PostQHandle");
        LOLSDK.Instance.SubmitProgress(0, 0, 10);
        HUDMan.SetAbsoluteAge(StartingAbsoluteAge);
        HUDMan.SetDepth(StartingDepth);
        HUDMan.SetEnergy(StartingEnergy, StartingEnergy);
        HUDMan.SetHealth(StartingHealth, StartingHealth);
        energy = StartingEnergy;
        health = StartingHealth;
        maxEnergy = StartingEnergy;
        maxHealth = StartingHealth;
        depth = StartingDepth;
        maxDifficulty = 17;
        origEnergySpawnRate = _SpawnMan.Energy;
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

    /// <summary>
    /// Called every time the player moves.
    /// 
    /// Alternative programmatic designs include:
    /// Using a Dictionary of Item.Type and Functions as a lookup table.
    /// Some OO method of handing over an Item class
    /// Or the C# way of using Delegate functions
    /// 
    /// Not using any of these because a Switch in this case won't be too large
    /// and is the easiest to maintain when we want to iterate quickly.
    /// </summary>
    /// <param name="Direction"></param>
    /// <param name="TargetItem">Item the player landed on.</param>
    public void Update(BoardMan.Direction Direction, Item.Type TargetItem, bool onEdge, bool teleporting)
    {
        //Teleporting doesn't consume energy.
        if (!teleporting)
        {
            if(!CBUG.DEBUG_ON || !Application.isEditor)
                energy--;   
        }
        else
        {
            TargetItem = Item.Type.None;
        }
        //where should item handle code go? Does GameManager actually do the item action?
        if(Direction != BoardMan.Direction.None)
        {
            if(Direction == BoardMan.Direction.North)
            {
                depth -= DepthPerRow;
                absoluteAge -= 3;
            }
            else if(Direction == BoardMan.Direction.South)
            {
                depth += DepthPerRow;
                absoluteAge += 3;
                if (depth > deepestDepth)
                    deepestDepth = depth;
            }
        }

        if(!onEdge)
            CharCon.Move(Direction);
        else
            CharCon.Rotate(Direction);

        
        switch (TargetItem)
        {
            case Item.Type.None:
                //Player Data Calls
                //UI Calls
                break;
            case Item.Type.Fossil:
                //Player Data Calls
                //UI Calls
                //QNA Popup
                energy++;
                _BoardMan.GamePaused = true;
                _QuizMan.Init(true);
                break;
            case Item.Type.Energy:
                //give energy
                if(energy < maxEnergy)
                     energy++;
                //blah
                break;
            case Item.Type.Damage:
                //Player Data Calls
                if (!CBUG.DEBUG_ON || !Application.isEditor)
                    health--;
                //UI Calls
                break;
            case Item.Type.EBoost:
                maxEnergy++;
                energy = maxEnergy;
                break;
            case Item.Type.HBoost:
                maxHealth++;
                health = maxHealth;
                break;
            default:
                CBUG.Error("Bad Item type given! " + TargetItem.ToString());
                break;
        }
        if (energy == 0 || health == 0)
        {
            if (health == 0)
            {
                ZeroHealthAnimObj.GetComponent<Animator>().SetTrigger("Quiz");
                //coroutine
                Tools.DelayAnim(ZeroHealthAnimObj.GetComponent<Animator>(), 1.5f, trigger: "Exit");
            }
            else if (energy == 0)
            {
                ZeroEnergyAnimObj.GetComponent<Animator>().SetTrigger("Quiz");
                //coroutine
                Tools.DelayAnim(ZeroEnergyAnimObj.GetComponent<Animator>(), 1.5f, trigger: "Exit");
            }
            StartCoroutine(teleportDuringAnim());
            return;
        }
        HUDMan.SetEnergy(energy, maxEnergy);
        HUDMan.SetHealth(health, maxHealth);
        HUDMan.SetDepth(depth);
        HUDMan.SetAbsoluteAge(absoluteAge + (3) * 10f); //arbitrary age calculation
    }

    public void Update(BoardMan.Direction Direction, Item.Type Item)
    {
        //where should item handle code go? Does GameManager actually do the item action?
        Update(Direction, Item, false, false);
    }

    public void GameOver()
    {
        CongratsText.text = "Game is over!\nCongrats! The furthest you dug was: " + deepestDepth;
        CongratsTextBG.text = "Game is over!\nCongrats! The furthest you dug was: " + deepestDepth;
        GameOverScreen.SetActive(true);
        StartCoroutine(endGame());
    }

    public int GetDifficultyOfTile(int rowNum)
    {
        int difficultyOfTile = (rowNum * DepthPerRow) / DepthPerDifficulty;
        _SpawnMan.Energy = origEnergySpawnRate - (EnergySpawnLossPerDifficulty * difficultyOfTile);
        return difficultyOfTile > maxDifficulty ? maxDifficulty: difficultyOfTile;
    }

    public float Depth {
        get {
            return depth;
        }

        //set {
        //    depth = value;
        //}
    }

    public float DeepestDepth {
        get {
            return deepestDepth;
        }
    }

    private IEnumerator teleportDuringAnim()
    {
        yield return new WaitForSeconds(1f);
        // For if both energy and health == 0, energy anim comes after.
        if (energy == 0 && health == 0)
        {
            ZeroEnergyAnimObj.GetComponent<Animator>().SetTrigger("Quiz");
            Tools.DelayAnim(ZeroEnergyAnimObj.GetComponent<Animator>(), 1.5f, trigger: "Exit");
        }

        int totalPunishment =
            (energy == 0 ? ZeroEnergyPunishment : 0) +
            (health == 0 ? ZeroHealthPunishment : 0);
        energy = maxEnergy;
        health = maxHealth;


        _BoardMan.TeleportDistance(totalPunishment);
        HUDMan.SetEnergy(energy, maxEnergy);
        HUDMan.SetHealth(health, maxHealth);
        HUDMan.SetDepth(depth);
        HUDMan.SetAbsoluteAge(absoluteAge + (3) * 10f); //arbitrary age calculat
    }

    private IEnumerator endGame()
    {
        yield return new WaitForSeconds(5f);
        LOLSDK.Instance.CompleteGame();
    }

}
