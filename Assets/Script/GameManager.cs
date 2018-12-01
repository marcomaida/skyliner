using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;
using UnityEngine.Advertisements;

public class GameManager : MonoBehaviour
{//Game manager manages score and gamestates

    public static GameManager active;
    public PlayerController player;
    public LinesManager lineManager;
    public SkylineManager skylineManager;

    //Camera and reset management
    public GameObject mainCamera;
    public float cameraSpeed = 5;
    public Transform cameraStartPostion;
    public Transform cameraRunningPostion;
    public Transform playerStartPosition;

    //UI management
    public GameObject runningUI;
    public GameObject gameOverUI;
	public Text scoreText;
    public Text bestScoreText;
    public Text gameOverScoreText;
    public Text gameHintText;
    public float hintDuration = 6;
    public Text CreatedByText;
    public Image obscurer;
    public Image logoImage;
    public float fadeSpeed = 30;
    public float splashScreenTime = 2;

    //Heart management
    public GameObject heart;//heart sprite
    Image heartImage;
    public float heartFullScale = 1.1f;//scale of the heart sprite when the heart is full
    public float heartBeatSpeed = 1;//speed of the heart returning to scale 1
    float currentHeartScale = 1;//current heart scale

    //Score management
	int bestScore;
    public int score;
    public float scoreSpeedTick = 1; //How many seconds pass before the score increases by 1
    float scoreSpeedCount;//counter to check if enough time has passed to increase score. Gets zeroed every time

    //Ads Management
    int playedGames;

    //Game state management
    public enum GameState {APPSTART,LOADING, STARTING, RUNNING, GAME_OVER }

    GameState state = GameState.APPSTART;
    public GameState State
    {
        get
        {
            return state;
        }
        set
        {//Switch GameMode
            if (state != value)
            {
                SwitchGameMode(value);
                state = value;
            }
        }
    }
    void SwitchGameMode(GameState stateToSwitchTo)
    {//Shows canvas, puts player in start pos, ...
        if (stateToSwitchTo == GameState.GAME_OVER)
        {
            playedGames++;
            bestScoreText.text = "Best: " + bestScore;

            if (bestScore < score)
			{//new best!!!
				bestScore = score;
                bestScoreText.text = "New best!";
            }

            SavePrefs();

            //UI stuff
            gameOverScoreText.text = "" + score;
            runningUI.SetActive(false);
            gameOverUI.SetActive(true);
            skylineManager.running = false;
            heart.SetActive(false);
            StopAllCoroutines();
            gameHintText.enabled = false;
        }
        else if (stateToSwitchTo == GameState.RUNNING)
        {
            ShowHint();
            runningUI.SetActive(true);
            gameOverUI.SetActive(false);
            skylineManager.running = true;
            heart.SetActive(true);
        }
        else if (stateToSwitchTo == GameState.STARTING)
        {
            runningUI.SetActive(false);
            gameOverUI.SetActive(false);
            heart.SetActive(false);
            obscurer.color = Color.black;
            obscurer.enabled = true;
            ResetGame();
        }
        else if (stateToSwitchTo == GameState.LOADING)
        {
            obscurer.enabled = true;
            runningUI.SetActive(false);
            gameOverUI.SetActive(false);
            heart.SetActive(false);
            ResetGame();
            StartCoroutine(SplashScreenRoutine());
        }

    }

    private void ResetGame()
    {//Resets camera and player position, score, blocks, etc
        mainCamera.transform.position = cameraStartPostion.position;
        player.transform.position = playerStartPosition.position;
        player.Restart();
        score = 0;
        scoreSpeedTick = 0;
        scoreText.text = " 0";
        skylineManager.ClearBlocks();
        skylineManager.running = false;
    }

    void Awake()
    {
        active = this;
		heartImage = heart.GetComponent<Image>();
        lineManager.OnLineChanged += OnLineChanged;
		LoadPrefs ();
    }
    void Start ()
    {
        State = GameState.LOADING;
        //StartCoroutine(SplashScreenRoutine());
	}

    void Update()
    {
        //Back button input 
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }

        if (obscurer.enabled && state != GameState.LOADING)
        {
            obscurer.color = Color.Lerp(obscurer.color, new Color(0, 0, 0, 0), fadeSpeed * Time.deltaTime);
            if (obscurer.color.a < 0.05)
            {
                obscurer.enabled = false;
            }
        }

    }
    void FixedUpdate()
    {
        UpdateCamera();
        if (state == GameState.RUNNING)
        {
            UpdateScore();
            UpdateHeartBeat();
        }
    }

    void UpdateCamera()
    {
        if(state == GameState.RUNNING)
            mainCamera.transform.position = Vector3.Lerp(mainCamera.transform.position, cameraRunningPostion.position, cameraSpeed * Time.deltaTime);
    }

    void UpdateScore()
    {
        scoreSpeedCount += Time.deltaTime;

        if (scoreSpeedCount > lineManager.CurrentLine.scoreSpeedTick)
        {
            scoreSpeedCount = 0;
			if (lineManager.CurrentLine.negativeScore)
			{
				score--;
				AudioManager.activeManager.PlayClipFromLibrary (1, transform.position, false);
				if (score < 0)
				{
					score = 0;
					player.Die ();
				}
			} 
			else
			{
				score++;
			}
            currentHeartScale = heartFullScale;
            scoreText.text = " "+score;//Update UI
        }
    }

    void UpdateHeartBeat()
    {
        if (currentHeartScale != 1)
        {
            currentHeartScale = Mathf.Lerp(currentHeartScale, 1, heartBeatSpeed * Time.deltaTime);
            heart.transform.localScale = new Vector3(currentHeartScale, currentHeartScale, currentHeartScale);

            if (currentHeartScale <= 1.05)
                currentHeartScale = 1;
        }
    }

    public void OnLineChanged()
    {
		heartImage.color = lineManager.CurrentLine.idleColor;
    }

    public void ReloadButtonPressed()
    {
        if (state != GameState.LOADING)
        {
            State = GameState.STARTING;
        }
    }
	public void SavePrefs()
	{//Saves player prefs
		PlayerPrefs.SetInt("BestScore",bestScore);
		PlayerPrefs.SetInt("PlayedGames",playedGames);
    }
	public void LoadPrefs()
	{//Loads Player prefs
        if (PlayerPrefs.HasKey("BestScore"))
        {
            bestScore = PlayerPrefs.GetInt("BestScore");
        }
        if (PlayerPrefs.HasKey("PlayedGames"))
        {
            playedGames = PlayerPrefs.GetInt("PlayedGames");
        }
    }
    public void ShowHint()
    {
        StartCoroutine(HintRoutine());
    }
    IEnumerator HintRoutine()
    {
        gameHintText.enabled = true;
        yield return new WaitForSeconds(hintDuration);
        gameHintText.enabled = false;
    }
    IEnumerator SplashScreenRoutine()
    {//I Enable everything before starting to cache elements in the canvas avoiding stuttering
        obscurer.enabled = true;
        obscurer.color = Color.black;
        CreatedByText.enabled = true;
        runningUI.SetActive(true);
        gameOverUI.SetActive(true);
        heart.SetActive(true);
        yield return new WaitForSeconds(splashScreenTime);
        runningUI.SetActive(false);
        gameOverUI.SetActive(false);
        heart.SetActive(false);
        CreatedByText.enabled = false;
        logoImage.enabled = false;
        State = GameState.STARTING;
        player.gameObject.SetActive(true);

    }
}
