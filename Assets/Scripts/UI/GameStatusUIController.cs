using UnityEngine;
using TMPro;

/// <summary>
/// Single Responsibility: Manage game status and timer UI elements
/// Listens to GameEvents for game state changes and updates the UI accordingly
/// Demonstrates Separation of Concerns - UI logic separate from game logic
/// Implements IGameStatusUI interface for consistency and testability
/// </summary>
public class GameStatusUIController : MonoBehaviour, IGameStatusUI
{
    [Header("Game Status UI Elements")]
    [SerializeField] private TextMeshProUGUI gameStatusText;
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private bool showDebugLogs = false;
    
    [Header("Timer Configuration")]
    [SerializeField] private bool showTimer = true;
    [SerializeField] private string timerPrefix = "Time: ";
    [SerializeField] private string timerSuffix = "s";
    [SerializeField] private bool pauseTimerOnGameOver = true;
    
    [Header("Status Messages")]
    [SerializeField] private string gameStartedMessage = "Game Started!";
    [SerializeField] private string gameOverMessage = "GAME OVER!";
    [SerializeField] private string gameWonMessage = "YOU WIN!";
    [SerializeField] private string gamePausedMessage = "PAUSED";
    
    /// <summary>
    /// Current game time in seconds
    /// </summary>
    public float GameTime { get; private set; }
    
    /// <summary>
    /// Whether the timer is currently running
    /// </summary>
    public bool TimerRunning { get; private set; }
    
    /// <summary>
    /// Current game status
    /// </summary>
    public GameStatus CurrentStatus { get; private set; } = GameStatus.Playing;
    
    /// <summary>
    /// Game status enumeration
    /// </summary>
    public enum GameStatus
    {
        Playing,
        Paused,
        GameOver,
        Won
    }
    
    void Start()
    {
        Initialize();
        
        if (showDebugLogs)
            Debug.Log("GameStatusUIController: Initialized and subscribed to game events");
    }
    
    void Update()
    {
        // Timer updates now come from GameManager via events
        // No need to update timer locally anymore
    }
    
    void OnDestroy()
    {
        UnsubscribeFromEvents();
        
        if (showDebugLogs)
            Debug.Log("GameStatusUIController: Unsubscribed from game events");
    }
    
    /// <summary>
    /// Initialize the UI controller (implements IUIController)
    /// </summary>
    public void Initialize()
    {
        InitializeGameStatusUI();
        SubscribeToEvents();
        StartTimer();
    }
    
    /// <summary>
    /// Subscribe to relevant events (implements IUIController)
    /// </summary>
    public void SubscribeToEvents()
    {
        GameEvents.OnGameStarted += HandleGameStarted;
        GameEvents.OnGameOver += HandleGameOver;
        GameEvents.OnGameWon += HandleGameWon;
        GameEvents.OnGamePaused += HandleGamePaused;
        GameEvents.OnGameRestarted += HandleGameRestarted;
        GameEvents.OnPlayerDied += HandlePlayerDied;
        GameEvents.OnTimerUpdated += HandleTimerUpdated;
    }
    
    /// <summary>
    /// Unsubscribe from events to prevent memory leaks (implements IUIController)
    /// </summary>
    public void UnsubscribeFromEvents()
    {
        GameEvents.OnGameStarted -= HandleGameStarted;
        GameEvents.OnGameOver -= HandleGameOver;
        GameEvents.OnGameWon -= HandleGameWon;
        GameEvents.OnGamePaused -= HandleGamePaused;
        GameEvents.OnGameRestarted -= HandleGameRestarted;
        GameEvents.OnPlayerDied -= HandlePlayerDied;
        GameEvents.OnTimerUpdated -= HandleTimerUpdated;
    }
    
    /// <summary>
    /// Show or hide the UI element (implements IUIController)
    /// </summary>
    /// <param name="visible">Whether the UI should be visible</param>
    public void SetVisible(bool visible)
    {
        SetStatusTextVisible(visible);
        SetTimerVisible(visible && showTimer);
    }
    
    /// <summary>
    /// Initialize the game status UI components
    /// </summary>
    private void InitializeGameStatusUI()
    {
        GameTime = 0f;
        TimerRunning = false;
        CurrentStatus = GameStatus.Playing;
        
        // Initialize status text
        if (gameStatusText != null)
        {
            gameStatusText.text = gameStartedMessage;
        }
        else
        {
            Debug.LogWarning("GameStatusUIController: Game status text component not assigned!");
        }
        
        // Initialize timer text
        if (timerText != null)
        {
            UpdateTimerDisplay();
        }
        else if (showTimer)
        {
            Debug.LogWarning("GameStatusUIController: Timer text component not assigned!");
        }
        
        // Initialize game over panel
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }
    }
    
    /// <summary>
    /// Handle game started event
    /// </summary>
    private void HandleGameStarted()
    {
        CurrentStatus = GameStatus.Playing;
        SetStatusText(gameStartedMessage);
        StartTimer();
        
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);
        
        if (showDebugLogs)
            Debug.Log("GameStatusUIController: Game started");
    }
    
    /// <summary>
    /// Handle game over event
    /// </summary>
    private void HandleGameOver()
    {
        CurrentStatus = GameStatus.GameOver;
        SetStatusText(gameOverMessage);
        
        if (pauseTimerOnGameOver)
            StopTimer();
        
        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);
        
        if (showDebugLogs)
            Debug.Log("GameStatusUIController: Game over");
    }
    
    /// <summary>
    /// Handle game won event
    /// </summary>
    private void HandleGameWon()
    {
        CurrentStatus = GameStatus.Won;
        SetStatusText(gameWonMessage);
        
        if (pauseTimerOnGameOver)
            StopTimer();
        
        if (showDebugLogs)
            Debug.Log("GameStatusUIController: Game won");
    }
    
    /// <summary>
    /// Handle game paused event
    /// </summary>
    /// <param name="isPaused">Whether the game is paused</param>
    private void HandleGamePaused(bool isPaused)
    {
        if (isPaused)
        {
            CurrentStatus = GameStatus.Paused;
            SetStatusText(gamePausedMessage);
            StopTimer();
        }
        else
        {
            CurrentStatus = GameStatus.Playing;
            SetStatusText(gameStartedMessage);
            StartTimer();
        }
        
        if (showDebugLogs)
            Debug.Log($"GameStatusUIController: Game {(isPaused ? "paused" : "resumed")}");
    }
    
    /// <summary>
    /// Handle game restarted event
    /// </summary>
    private void HandleGameRestarted()
    {
        CurrentStatus = GameStatus.Playing;
        ResetTimer();
        SetStatusText(gameStartedMessage);
        
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);
        
        if (showDebugLogs)
            Debug.Log("GameStatusUIController: Game restarted");
    }
    
    /// <summary>
    /// Handle player died event
    /// </summary>
    private void HandlePlayerDied()
    {
        // Player death will typically trigger game over, but we can add specific logic here
        if (showDebugLogs)
            Debug.Log("GameStatusUIController: Player died");
    }
    
    /// <summary>
    /// Handle timer updated event from GameManager
    /// </summary>
    /// <param name="gameTime">Current game time from GameManager</param>
    private void HandleTimerUpdated(float gameTime)
    {
        // Sync our timer with the GameManager's timer
        GameTime = gameTime;
        UpdateTimerDisplay();
    }
    
    /// <summary>
    /// Set the status text display
    /// </summary>
    /// <param name="statusMessage">Status message to display</param>
    private void SetStatusText(string statusMessage)
    {
        if (gameStatusText != null)
        {
            gameStatusText.text = statusMessage;
        }
    }
    
    /// <summary>
    /// Start the game timer
    /// </summary>
    public void StartTimer()
    {
        TimerRunning = true;
        
        if (showDebugLogs)
            Debug.Log("GameStatusUIController: Timer started");
    }
    
    /// <summary>
    /// Stop the game timer
    /// </summary>
    public void StopTimer()
    {
        TimerRunning = false;
        
        if (showDebugLogs)
            Debug.Log("GameStatusUIController: Timer stopped");
    }
    
    /// <summary>
    /// Reset the game timer
    /// </summary>
    public void ResetTimer()
    {
        GameTime = 0f;
        TimerRunning = true;
        UpdateTimerDisplay();
        
        if (showDebugLogs)
            Debug.Log("GameStatusUIController: Timer reset");
    }
    
    
    /// <summary>
    /// Update the timer display
    /// </summary>
    private void UpdateTimerDisplay()
    {
        if (timerText != null && showTimer)
        {
            int displayTime = Mathf.FloorToInt(GameTime);
            timerText.text = $"{timerPrefix}{displayTime}{timerSuffix}";
        }
    }
    
    /// <summary>
    /// Manually set the status text (for testing or special cases)
    /// </summary>
    /// <param name="message">Status message to set</param>
    public void SetCustomStatusText(string message)
    {
        SetStatusText(message);
    }
    
    /// <summary>
    /// Get the current game time as a formatted string
    /// </summary>
    /// <returns>Formatted time string</returns>
    public string GetFormattedTime()
    {
        int minutes = Mathf.FloorToInt(GameTime / 60f);
        int seconds = Mathf.FloorToInt(GameTime % 60f);
        return $"{minutes:00}:{seconds:00}";
    }
    
    /// <summary>
    /// Enable or disable the timer display
    /// </summary>
    /// <param name="show">Whether to show the timer</param>
    public void SetTimerVisible(bool show)
    {
        showTimer = show;
        
        if (timerText != null)
        {
            timerText.gameObject.SetActive(show);
        }
    }
    
    /// <summary>
    /// Enable or disable the status text display
    /// </summary>
    /// <param name="visible">Whether the status text should be visible</param>
    public void SetStatusTextVisible(bool visible)
    {
        if (gameStatusText != null)
        {
            gameStatusText.gameObject.SetActive(visible);
        }
    }
    
    /// <summary>
    /// Show or hide the game over panel manually
    /// </summary>
    /// <param name="show">Whether to show the game over panel</param>
    public void SetGameOverPanelVisible(bool show)
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(show);
        }
    }
}
