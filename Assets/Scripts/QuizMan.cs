﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using LoLSDK;

public class QuizMan : MonoBehaviour {

    public BoardMan _BoardMan;

    public GameObject[] QButtons;
    public GameObject QPanel;

    public GameObject CorrectPanel;
    public GameObject IncorrectPanel;
    public float ConfirmationPanelHangTime;

    [System.Serializable]
    public struct Quiz
    {
        public string Question;
        public string Q_ID;
        public string[] Answers_ID;
        public string[] Answers;
        public string CorrectAnswer_ID;
        public int TotalAnswerChoices;
        public string SpriteURL;
    }

    private Quiz[] quizzes;

    public Text QuestionBox;
    public Text[] AnswerBoxes;

    private int currentQuestion;
    public Animator QuizAnim;

    public Image QuizImg;
    public GameObject QuizImgObject;
    public GameObject QuizImgObjectBackground;
    public Sprite LoadingImg;

    private string[] alternativeIDs;

    public int DailyQuestionsMax = 3;
    private int currentDailyQuestions;

    private int tilesToTeleport;

	// Use this for initialization
	void Start () {

        if (Application.isEditor || !Application.absoluteURL.Contains("harness"))
        {
            testQuizFunctionality();
        }

        currentQuestion = -1;
        tilesToTeleport = 0;
        LOLSDK.Instance.QuestionsReceived += new QuestionListReceivedHandler(GetQuestions);
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void GetQuestions(MultipleChoiceQuestionList questionList) {
        MultipleChoiceQuestionList qTemp = questionList;
        if (qTemp == null || qTemp.questions == null)
        {
            if (!Application.isEditor)
                CBUG.Do("No questions found.");
            return;
        }
        CBUG.Do("Questions found! Total Questions: " + qTemp.questions.Length);
        quizzes = new Quiz[qTemp.questions.Length];
        for (int x = 0; x < qTemp.questions.Length; x++)
        {
            quizzes[x] = new Quiz();
            quizzes[x].TotalAnswerChoices = qTemp.questions[x].alternatives.Length;
            quizzes[x].Answers = new string[quizzes[x].TotalAnswerChoices];
            quizzes[x].Answers_ID = new string[quizzes[x].TotalAnswerChoices];
            quizzes[x].Question = checkForImage(qTemp.questions[x].stem);
            quizzes[x].Q_ID = qTemp.questions[x].questionId;
            quizzes[x].SpriteURL = qTemp.questions[x].imageURL;
            if (string.IsNullOrEmpty(quizzes[x].SpriteURL))
            {
                CBUG.Do("Question: " + quizzes[x].Answers_ID + " has no image.");
            }
            CBUG.Do("Question imageURL: " + quizzes[x].SpriteURL);

            for (int y = 0; y < quizzes[x].TotalAnswerChoices; y++)
            {
                quizzes[x].Answers[y] = qTemp.questions[x].alternatives[y].text;
                quizzes[x].Answers_ID[y] = qTemp.questions[x].alternatives[y].alternativeId;
                quizzes[x].CorrectAnswer_ID = qTemp.questions[x].correctAlternativeId;
            }
        }
    }

    public void skipQuestion()
    { 
        MultipleChoiceAnswer answer = new MultipleChoiceAnswer();
        answer.alternativeId = "";
        answer.questionId = "";
        LOLSDK.Instance.SubmitAnswer(answer);
    }

    /// <summary>
    /// Called by UI Button.
    /// </summary>
    /// <param name="answerNum">Answer number as 0 - 3.</param>
    public void CheckAnswer(int answerNum)
    {

        MultipleChoiceAnswer answer = new MultipleChoiceAnswer();
        answer.alternativeId = quizzes[currentQuestion].Answers_ID[answerNum];
        answer.questionId = quizzes[currentQuestion].Q_ID;
        LOLSDK.Instance.SubmitAnswer(answer);
        if (quizzes[currentQuestion].Answers_ID[answerNum] == quizzes[currentQuestion].CorrectAnswer_ID) {
            //gCont.CurrentScore++;
        }

        for (int x = 0; x < AnswerBoxes.Length; x++)
        {
            QButtons[x].SetActive(false);
        }

        if (currentDailyQuestions >= DailyQuestionsMax)
        {
            if (quizzes[currentQuestion].Answers_ID[answerNum] == quizzes[currentQuestion].CorrectAnswer_ID)
            {
                //gCont.SpeechBubble.GetComponentInChildren<Text>().text = gCont.Days[gCont.CurrentDay][gCont.CurrentNPC].Thanks;
                showConfirmationPanel(true);
                StartCoroutine(hideConfirmationPanel(true));
                tilesToTeleport += 10;
            }
            else
            {
                //gCont.SpeechBubble.GetComponentInChildren<Text>().text = gCont.Days[gCont.CurrentDay][gCont.CurrentNPC].Anger;
                showConfirmationPanel(false);
                StartCoroutine(hideConfirmationPanel(false));
            }
            StartCoroutine(delayNextQuestion(true));
        }
        else
        {
            if (quizzes[currentQuestion].Answers_ID[answerNum] == quizzes[currentQuestion].CorrectAnswer_ID)
            {
                //gCont.SpeechBubble.GetComponentInChildren<Text>().text = gCont.Days[gCont.CurrentDay][gCont.CurrentNPC].Thanks;
                showConfirmationPanel(true);
                StartCoroutine(hideConfirmationPanel(true));
                tilesToTeleport += 10;
            }
            else
            {
                //gCont.SpeechBubble.GetComponentInChildren<Text>().text = gCont.Days[gCont.CurrentDay][gCont.CurrentNPC].Anger;
                showConfirmationPanel(false);
                StartCoroutine(hideConfirmationPanel(false));
            }
            StartCoroutine(delayNextQuestion(false));
        }
    }

    public void Init(bool firstLoad)
    {
        if (currentQuestion >= quizzes.Length - 1)
            currentQuestion = -1;
        currentQuestion++;
        currentDailyQuestions++;

        //If no image exists, hide the graphic
        QuizImg.color = new Color(1f, 1f, 1f, 0f);
        QuizImgObject.SetActive(false);
        QuizImgObjectBackground.SetActive(false);

        string spriteURL = null;
        spriteURL = quizzes[currentQuestion].SpriteURL;
        if (!string.IsNullOrEmpty(spriteURL))
        {
            //If it DOES Exist, first set graphic to "loading"
            QuizImg.sprite = LoadingImg;
            StartCoroutine(setQuestionImage(quizzes[currentQuestion].SpriteURL));
        }

        for (int x = 0; x < quizzes[currentQuestion].TotalAnswerChoices; x++)
        { 
            AnswerBoxes[x].text = "";
        }

        for (int x = 0; x < AnswerBoxes.Length; x++)
        {
                QButtons[x].SetActive(false);
        }

        FinishQInit();
        if (firstLoad)
            QuizAnim.SetTrigger("Quiz");
    }

    /// <summary>
    /// Called by quiz anim.
    /// </summary>
    public void FinishQInit()
    {
        QuestionBox.text = quizzes[currentQuestion].Question;
        string spriteURL = null;
        spriteURL = quizzes[currentQuestion].SpriteURL;
        if (!string.IsNullOrEmpty(spriteURL))
        {
            QuizImgObject.SetActive(true);
            QuizImgObjectBackground.SetActive(true);
            QuizImg.color = new Color(1f, 1f, 1f, 1f);
        }

        for (int x = 0; x < quizzes[currentQuestion].TotalAnswerChoices; x++)
        {
            AnswerBoxes[x].text = quizzes[currentQuestion].Answers[x];
        }
        for (int x = 0; x < AnswerBoxes.Length; x++)
        {
            if (x >= quizzes[currentQuestion].TotalAnswerChoices)
            {
                QButtons[x].SetActive(false);
            }
            else
            {
                QButtons[x].SetActive(true);
            }
        }
    }

    /// <summary>
    /// When an image exists, sets the <see cref="QuizImg"/> sprite texture via given <paramref name="url"/>.
    /// </summary>
    /// <param name="url">The web address of the image to load.</param>
    /// <returns></returns>
    private IEnumerator setQuestionImage(string url)
    {
        CBUG.Do("URL TO TEST IS: " + url);
        // Start a download of the given URL
        var www = new WWW(url);
        // wait until the download is done
        yield return www;
        // Create a texture in DXT1 format
        Texture2D texture = new Texture2D(www.texture.width, www.texture.height, TextureFormat.DXT1, false);

        if (www == null || texture == null || string.IsNullOrEmpty(url))
        {
            if (www == null)
                CBUG.Do("BAD URL!! + www null");
            else if (texture == null)
                CBUG.Do("BAD URL!! + texture null");
            else
                CBUG.Do("BAD URL!! + " + string.IsNullOrEmpty(url));
        }
        else
        {
            // assign the downloaded image to sprite
            www.LoadImageIntoTexture(texture);
            Rect rec = new Rect(0, 0, texture.width, texture.height);
            Sprite spriteToUse = Sprite.Create(texture, rec, new Vector2(0.5f, 0.5f), 100);
            QuizImg.sprite = spriteToUse;

            LayoutElement l_element = QuizImg.GetComponent<LayoutElement>();
            if (texture.width > texture.height)
            {
                l_element.preferredWidth = texture.width;
                l_element.minHeight = texture.height;
            }
            else
            {
                l_element.minWidth = texture.width;
                l_element.preferredHeight = texture.height;
            }


            www.Dispose();
            www = null;
        }
        //QuizImgObject.SetActive(false);
        //QuizImg.color = new Color(1f, 1f, 1f, 1f);
    }

    /// <summary>
    /// For local use only. In-editor functionality testing.
    /// </summary>
    private void testQuizFunctionality()
    {
        int quizTotal = 10;
        quizzes = new Quiz[quizTotal];
        for (int x = 0; x < quizTotal; x++)
        {
            CBUG.Do("Question added: " + x);
            quizzes[x] = new Quiz();
            quizzes[x].Question = "Q" + x;
            quizzes[x].TotalAnswerChoices = Random.Range(1, 4);
            quizzes[x].Answers = new string[quizzes[x].TotalAnswerChoices];
            quizzes[x].Answers_ID = new string[quizzes[x].TotalAnswerChoices];
            // 50/50 chance spriteURL is either null or an image.
            quizzes[x].SpriteURL =  Random.Range(0, 2) == 0 ? null : "http://i.imgur.com/jQLvoPK.png";

            for (int y = 0; y < quizzes[x].TotalAnswerChoices; y++)
            {
                quizzes[x].Answers[y] = "A" + y;
                quizzes[x].Answers_ID[y] = "" + y;
                quizzes[x].Q_ID = "" + x;
                quizzes[x].CorrectAnswer_ID = "" + Mathf.Clamp(x, 1, 4);
                CBUG.Do("Answer added: " + y);
            }
        }
    }

    private string checkForImage(string text)
    {
        if (text.Contains("[IMAGE]"))
        {
            return text.Replace("[IMAGE]", "\n(See Image)\n");
        }
        return text;
    }

    private void showConfirmationPanel(bool isAnswerCorrect)
    {
        if (isAnswerCorrect)
        {
            CorrectPanel.SetActive(true);
            CorrectPanel.GetComponent<Animator>().SetTrigger("Quiz");
        }
        else
        {
            IncorrectPanel.SetActive(true);
            IncorrectPanel.GetComponent<Animator>().SetTrigger("Quiz");
        }
    }

    private IEnumerator hideConfirmationPanel(bool isAnswerCorrect)
    {
        yield return new WaitForSeconds(ConfirmationPanelHangTime);
        if(isAnswerCorrect)
            CorrectPanel.GetComponent<Animator>().SetTrigger("Exit");
        else
            IncorrectPanel.GetComponent<Animator>().SetTrigger("Exit");
        CorrectPanel.SetActive(false);
        IncorrectPanel.SetActive(false);
        //Yes i'm aware it's redundant to call an animation then immediately disable it.
    }

    private IEnumerator delayNextQuestion(bool isLastQuestion)
    {
        yield return new WaitForSeconds(ConfirmationPanelHangTime);
        if (isLastQuestion)
        {
            QuizAnim.SetTrigger("Exit");
            _BoardMan.GamePaused = false;
            currentDailyQuestions = 0;
            _BoardMan.TeleportDistance(tilesToTeleport);
            tilesToTeleport = 0;
        }
        else
        {
            Init(false);
        }

    }
}

