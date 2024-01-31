using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;


//using UnityEngine.UI.ColorBlock;
//using UnityEngine.UI.ColorBlock;


public class QuizManager : MonoBehaviour
{
    public TMP_Text questionText;
    public TMP_Text questionHeader;
    public Button[] answerButtons;
    public Button nextQuestion;
    public Button checkQuestion;
    public TMP_Text scoreText;
    public TMP_Text endScoreText;
    public TMP_Text questionNumberNow;
    public TMP_Text endMessage;
    public GameObject endingPanel;


    public AudioSource yay;
    public AudioSource buh;
    private bool isCorrect;

    private string[] questions = { "Wer schrieb Gespenster?", "In welchem Land isst man Croissants?", "Wer malte das Bild der Schrei?", "Zu welchem Team gehört Stephen Curry?", "Wer schrieb die Moonlight Sonata?" };
    private string[] headers = { "Literatur", "Geografie", "Kunst", "Sport", "Musik" };
    private string[][] answers = {
        new string[] { "Henrik Ibsen", "Ich", "Du" },
        new string[] { "Hier", "Frankreich", "Wo anders" },
        new string[] { "Edvard Munich", "Du", "Ich" },
        new string[] { "Meinem", "Deinem", "Golden State Warriors" },
        new string[] { "Du", "Beethoven", "Ich" }        
    };

   public GameObject[] imageSprites;

    private List<int>[] correctAnswers = {
        new List<int> {0},
        new List<int> {1},
        new List<int> {0},
        new List<int> {2},
        new List<int> {1},
    };

    private int selectedAnswerIndex = -1;

    public TMP_Text timerText;
    public float totalTime = 10f;
    private float currentTime;
    private bool timerRunning;

    private List<int> questionOrder;


    private int currentQuestionIndex = 0;
    private int score = 0;


    public Color prettyGreen;
    public Color prettyRed;



    void Start()
    {
   
       questionOrder = new List<int>();

        for (int i = 0; i < questions.Length; i++)
        {
            questionOrder.Add(i);
        }

        currentTime = totalTime;
        endingPanel.SetActive(false);
        timerRunning = true;

        ShuffleQuestions();

        nextQuestion.onClick.AddListener(() => OnNextClick());
        checkQuestion.onClick.AddListener(() => OnCheckClick());

        ShowQuestion();
    }

    void Update()
    {
        if (timerRunning)
        {
            if (timerText.text != "00:00")
            {
                currentTime -= Time.deltaTime;
                UpdateTimerDisplay();
            }
            else
            {
               
                Debug.Log("Timer abgelaufen!");
                timerRunning = false;

                EndQuiz();
            }
        } 
    }


    void ShuffleQuestions()
    {
       
        int n = questionOrder.Count;
        while (n > 1)
        {
            n--;
            int k = Random.Range(0, n + 1);
            int value = questionOrder[k];
            questionOrder[k] = questionOrder[n];
            questionOrder[n] = value;
        }
    }

    void UpdateTimerDisplay()
    {
        
        float minutes = Mathf.Floor(currentTime / 60);
        float seconds = Mathf.RoundToInt(currentTime % 10);

        
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    void ShowQuestion()
    {

            questionNumberNow.text = "Frage " + (currentQuestionIndex + 1) + "/5";

            nextQuestion.interactable = false;

            currentTime = totalTime;

            int shuffledIndex = questionOrder[currentQuestionIndex];
            questionText.text = questions[shuffledIndex];
            questionHeader.text = headers[shuffledIndex];

            
            for (int i = 0; i < imageSprites.Length; i++)
            {
                imageSprites[i].SetActive(false);
                imageSprites[shuffledIndex].SetActive(true);
            }


            for (int i = 0; i < answerButtons.Length; i++)
            {
                answerButtons[i].GetComponent<Image>().color = Color.white;
                answerButtons[i].GetComponentInChildren<TMP_Text>().text = answers[shuffledIndex][i];
                int index = i;
                answerButtons[i].onClick.RemoveAllListeners();
                answerButtons[i].onClick.AddListener(() => OnAnswerClick(index));
            }
        
    }

    void OnAnswerClick(int answerIndex)
    {
        nextQuestion.interactable = true;
        selectedAnswerIndex = answerIndex;

            string selectedAnswer = answers[currentQuestionIndex][answerIndex];
            if (IsCorrectAnswer(answerIndex))
            {
                
                
                isCorrect = true;

            }
            else
            {
                isCorrect = false;
            }
    }

    void OnCheckClick()
    {
        timerRunning = false;

        for (int i = 0; i < answerButtons.Length; i++)
        {
            if (IsCorrectAnswer(i))
            {
                answerButtons[i].GetComponent<Image>().color = prettyGreen;
            }
            else
            {
                answerButtons[i].GetComponent<Image>().color = prettyRed;
            }
        }

        if (isCorrect)
        {
            yay.Play();

        }
        else
        {
            buh.Play();
        }
    }


    void OnNextClick()
    {
        if (isCorrect)
        {
            score++;
        }


        timerRunning = true;
        scoreText.text = "Score: " + score;

        nextQuestion.interactable = false;

        currentQuestionIndex++;
        

        if (currentQuestionIndex < questions.Length)
        {

            ShowQuestion();    
        }

        else
        {
            EndQuiz();
        }

        
    }

    bool IsCorrectAnswer(int answerIndex)
    {

        return correctAnswers[questionOrder[currentQuestionIndex]].Contains(answerIndex);
    }

    void EndQuiz()
    {

        endScoreText.text = "Final Score " + score + " / 5";

        scoreText.text = "Score: " + score;

        if (score < 1)
        {
            endMessage.text = "... schlechter gehts nicht...";
        }
        else if (score < 3)
        {
            endMessage.text = "Das war ziemich mies!";
        }
        else if(score < 5)
        {
            endMessage.text = "Nicht schlecht!";
        }
        else if(score == 5)
        {
            endMessage.text = "Du Profi!";
        }


        endingPanel.SetActive(true);

        for (int i = 0; i < answerButtons.Length; i++)
      {
            answerButtons[i].interactable = false;
      }
        nextQuestion.interactable = false;
        checkQuestion.interactable = false;

    }

}

