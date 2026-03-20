using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class LevelTimer : MonoBehaviour
{
    [Header("Timer Settings")]
    public float timeLimit = 60f;

    [Header("UI")]
    public TextMeshProUGUI timerText;
    public GameObject endPanel;
    public TextMeshProUGUI endMessageText;
    public TextMeshProUGUI scoreText;

    [Header("Sounds")]
    public AudioSource winSound;
    public AudioSource loseSound;

    private float timeRemaining;
    private bool gameEnded = false;

    private int totalCoins;
    private int coinsCollected;

    [Header("Ticking Sound")]
    public AudioSource tickSound;
    public float normalTickInterval = 1f;
    public float fastTickInterval = 0.25f;

    public Button nextLevelButton;


    private float tickTimer;

    // Blinking settings
    private float blinkSpeed = 6f;

    private bool timerRunning = true;

    void Start()
    {
        timeRemaining = timeLimit;

        GameObject[] coins = GameObject.FindGameObjectsWithTag("Coin");
        totalCoins = coins.Length;
        coinsCollected = 0;

        if (endPanel != null) endPanel.SetActive(false);
        UpdateTimerUI();
    }

    void Update()
    {
        if (gameEnded) return;

        if (!timerRunning || gameEnded) return;

        // WIN check
        if (coinsCollected >= totalCoins && totalCoins > 0)
        {
            Win();
            return;
        }

        // Countdown
        timeRemaining -= Time.deltaTime;
        HandleTicking();

        if (timeRemaining <= 0f)
        {
            timeRemaining = 0f;
            UpdateTimerUI();
            Lose();
            return;
        }

        UpdateTimerUI();
    }

    void UpdateTimerUI()
    {
        if (timerText == null) return;

        int seconds = Mathf.CeilToInt(timeRemaining);
        timerText.text = "Time: " + seconds;

        // BLINK RED WHEN 10 SECONDS LEFT
        if (timeRemaining <= 10f)
        {
            float t = Mathf.PingPong(Time.time * blinkSpeed, 1);
            timerText.color = Color.Lerp(Color.white, Color.red, t);
        }
        else
        {
            timerText.color = Color.white;
        }
    }

    void HandleTicking()
    {
        if (tickSound == null) return;

        float interval = (timeRemaining <= 10f) ? fastTickInterval : normalTickInterval;

        tickTimer += Time.deltaTime;

        if (tickTimer >= interval)
        {
            tickSound.Play();
            tickTimer = 0f;
        }
    }

    public void AddCoin()
    {
        if (gameEnded) return;

        coinsCollected++;

        if (coinsCollected >= totalCoins && totalCoins > 0)
            Win();
    }

    void Win()
    {
        gameEnded = true;

        StopTimer();

        endPanel.SetActive(true);


        endMessageText.text = "LEVEL COMPLETE!";
        endMessageText.color = Color.yellow;

        scoreText.text = coinsCollected.ToString();

        nextLevelButton.gameObject.SetActive(true); // show next level button

        if (winSound != null)
            winSound.Play();

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        Time.timeScale = 0f;
    }

    public void ReplayLevel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void NextLevel()
    {
        Time.timeScale = 1f;

        int nextScene = SceneManager.GetActiveScene().buildIndex + 1;

        if (nextScene < SceneManager.sceneCountInBuildSettings)
            SceneManager.LoadScene(nextScene);
    }

    public void MainMenu()
    {
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        SceneManager.LoadScene("MainMenu");
    }

    void Lose()
    {
        gameEnded = true;

        StopTimer();

        endPanel.SetActive(true);

        endMessageText.text = "TIME'S UP! TRY AGAIN";
        endMessageText.color = Color.red;

        scoreText.text = coinsCollected.ToString();

        nextLevelButton.gameObject.SetActive(false); // hide next level button

        if (loseSound != null)
            loseSound.Play();

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        Time.timeScale = 0f;
    }

    public void TryAgain()
    {
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void StartTimer()
    {
        timerRunning = true;
    }

    void StopTimer()
    {
        gameEnded = true;

        if (tickSound != null)
            tickSound.Stop();
    }
}