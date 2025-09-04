using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

/// <summary>
/// Refactored GameManager that uses event-driven architecture
/// Demonstrates SOLID principles:
/// - Single Responsibility: Only manages game state and flow
/// - Open/Closed: Extensible through event system
/// - Dependency Inversion: Depends on event abstractions, not concrete classes
/// </summary>
public class GameManager : MonoBehaviour
{
    [Header("Game Settings")]
    [SerializeField] private int winScore = 30;
    [SerializeField] private bool showDebugLogs = true;
    
    [Header("Game Flow Settings")]
    [SerializeField] private float gameOverDelay = 2f;
    [SerializeField] private float gameWinDelay = 2f;
    [SerializeField] private bool autoRestart = true;
    
    // Game state (private - accessed through events)
    private bool gameOver = false;
    private bool gameWon = false;
    private float gameTime = 0f;
    private int currentScore = 0;
    
    /// <summary>
    /// Current game state
    /// </summary>
    public enum GameState
    {
        Playing,
        Paused,
        GameOver,
        Won
    }
    
    /// <summary>
    /// Current game state (read-only)
    /// </summary>
    public GameState CurrentState { get; private set; } = GameState.Playing;
    
    /// <summary>
    /// Current game time (read-only)
    /// </summary>
    public float CurrentGameTime => gameTime;
    
    /// <summary>
    /// Whether the game is currently active
    /// </summary>
    public bool IsGameActive => CurrentState == GameState.Playing;
    
    void Start()
    {
        InitializeGameManager();
    }
    
    void Update()
    {
        UpdateGameState();
        HandleDebugInput();
    }
    
    void OnDestroy()
    {
        CleanupEventSubscriptions();
    }
    
    /// <summary>
    /// Initialize the game manager and subscribe to events
    /// </summary>
    private void InitializeGameManager()
    {
        // Subscribe to game events instead of direct component references
        SubscribeToEvents();
        
        // Initialize game state
        ResetGameState();
        
        // Trigger game started event
        GameEvents.TriggerGameStarted();
        
        if (showDebugLogs)
            Debug.Log("GameManager: Initialized with event-driven architecture");
    }
    
    /// <summary>
    /// Subscribe to relevant game events
    /// </summary>
    private void SubscribeToEvents()
    {
        // Player events
        GameEvents.OnPlayerDied += HandlePlayerDied;
        GameEvents.OnPlayerScoreChanged += HandleScoreChanged;
        
        // Game control events
        GameEvents.OnGameRestarted += HandleGameRestart;
        GameEvents.OnGamePaused += HandleGamePaused;
        
        if (showDebugLogs)
            Debug.Log("GameManager: Subscribed to game events");
    }
    
    /// <summary>
    /// Unsubscribe from events to prevent memory leaks
    /// </summary>
    private void CleanupEventSubscriptions()
    {
        GameEvents.OnPlayerDied -= HandlePlayerDied;
        GameEvents.OnPlayerScoreChanged -= HandleScoreChanged;
        GameEvents.OnGameRestarted -= HandleGameRestart;
        GameEvents.OnGamePaused -= HandleGamePaused;
        
        if (showDebugLogs)
            Debug.Log("GameManager: Cleaned up event subscriptions");
    }
    
    /// <summary>
    /// Update game state and timer
    /// </summary>
    private void UpdateGameState()
    {
        // Only update timer when game is active
        if (IsGameActive)
        {
            gameTime += Time.deltaTime;
            
            // Trigger timer update event for UI
            GameEvents.TriggerTimerUpdated(gameTime);
        }
    }
    
    /// <summary>
    /// Handle debug input (development only)
    /// </summary>
    private void HandleDebugInput()
    {
        // Note: Input handling is now separated from game logic
        // Real input should come through the input system and events
        
        #if UNITY_EDITOR || DEVELOPMENT_BUILD
        // Emergency restart for development
        if (Input.GetKeyDown(KeyCode.R) && Input.GetKey(KeyCode.LeftShift))
        {
            if (showDebugLogs)
                Debug.Log("GameManager: Emergency restart triggered");
            RestartGame();
        }
        #endif
    }
    
    /// <summary>
    /// Handle player death event
    /// </summary>
    private void HandlePlayerDied()
    {
        if (gameOver || gameWon) return; // Prevent multiple triggers
        
        SetGameState(GameState.GameOver);
        
        // Trigger game over event
        GameEvents.TriggerGameOver();
        
        if (showDebugLogs)
            Debug.Log("GameManager: Player died - triggering game over");
        
        // Schedule restart if auto-restart is enabled
        if (autoRestart)
        {
            Invoke(nameof(RestartGame), gameOverDelay);
        }
    }
    
    /// <summary>
    /// Handle score changed event and check win condition
    /// </summary>
    /// <param name="newScore">New score value</param>
    private void HandleScoreChanged(int newScore)
    {
        currentScore = newScore;
        
        // Check win condition
        if (newScore >= winScore && IsGameActive)
        {
            HandleGameWon();
        }
        
        if (showDebugLogs)
            Debug.Log($"GameManager: Score updated to {newScore}. Win threshold: {winScore}");
    }
    
    /// <summary>
    /// Handle game won condition
    /// </summary>
    private void HandleGameWon()
    {
        if (gameOver || gameWon) return; // Prevent multiple triggers
        
        SetGameState(GameState.Won);
        
        // Trigger game won event
        GameEvents.TriggerGameWon();
        
        if (showDebugLogs)
            Debug.Log($"GameManager: Game won with score {currentScore}!");
        
        // Schedule restart if auto-restart is enabled
        if (autoRestart)
        {
            Invoke(nameof(RestartGame), gameWinDelay);
        }
    }
    
    /// <summary>
    /// Handle game restart event
    /// </summary>
    private void HandleGameRestart()
    {
        if (showDebugLogs)
            Debug.Log("GameManager: Game restart requested");
        
        RestartGame();
    }
    
    /// <summary>
    /// Handle game paused event
    /// </summary>
    /// <param name="isPaused">Whether the game should be paused</param>
    private void HandleGamePaused(bool isPaused)
    {
        if (isPaused)
        {
            SetGameState(GameState.Paused);
            Time.timeScale = 0f;
        }
        else
        {
            SetGameState(GameState.Playing);
            Time.timeScale = 1f;
        }
        
        if (showDebugLogs)
            Debug.Log($"GameManager: Game {(isPaused ? "paused" : "resumed")}");
    }
    
    /// <summary>
    /// Set the current game state
    /// </summary>
    /// <param name="newState">New game state</param>
    private void SetGameState(GameState newState)
    {
        if (CurrentState == newState) return;
        
        CurrentState = newState;
        
        // Update internal flags for backward compatibility
        gameOver = (newState == GameState.GameOver);
        gameWon = (newState == GameState.Won);
        
        if (showDebugLogs)
            Debug.Log($"GameManager: State changed to {newState}");
    }
    
    /// <summary>
    /// Reset game state to initial values
    /// </summary>
    private void ResetGameState()
    {
        gameTime = 0f;
        currentScore = 0;
        SetGameState(GameState.Playing);
        Time.timeScale = 1f;
        
        if (showDebugLogs)
            Debug.Log("GameManager: Game state reset");
    }
    
    /// <summary>
    /// Restart the game (public method for external calls)
    /// </summary>
    public void RestartGame()
    {
        if (showDebugLogs)
            Debug.Log("GameManager: Restarting game...");
        
        // Cancel any pending invokes
        CancelInvoke();
        
        // Reset time scale
        Time.timeScale = 1f;
        
        // Trigger restart event before scene reload
        GameEvents.TriggerGameRestarted();
        
        // Reload the current scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    
    /// <summary>
    /// Manually trigger game over (for external systems)
    /// </summary>
    public void TriggerGameOver()
    {
        if (showDebugLogs)
            Debug.Log("GameManager: Game over triggered manually");
        
        HandlePlayerDied();
    }
    
    /// <summary>
    /// Manually trigger game won (for external systems)
    /// </summary>
    public void TriggerGameWon()
    {
        if (showDebugLogs)
            Debug.Log("GameManager: Game won triggered manually");
        
        HandleGameWon();
    }
    
    /// <summary>
    /// Legacy method for backwards compatibility with old Player.cs
    /// Triggers game over through the event system
    /// </summary>
    public void GameOver()
    {
        if (showDebugLogs)
            Debug.Log("GameManager: GameOver() called (legacy method)");
        
        TriggerGameOver();
    }
    
    /// <summary>
    /// Set the win score threshold
    /// </summary>
    /// <param name="score">New win score</param>
    public void SetWinScore(int score)
    {
        winScore = score;
        
        if (showDebugLogs)
            Debug.Log($"GameManager: Win score set to {score}");
    }
    
    /// <summary>
    /// Enable or disable auto-restart
    /// </summary>
    /// <param name="enable">Whether to enable auto-restart</param>
    public void SetAutoRestart(bool enable)
    {
        autoRestart = enable;
        
        if (showDebugLogs)
            Debug.Log($"GameManager: Auto-restart {(enable ? "enabled" : "disabled")}");
    }
    
    /// <summary>
    /// Get current win score threshold
    /// </summary>
    /// <returns>Win score threshold</returns>
    public int GetWinScore()
    {
        return winScore;
    }
    
    /// <summary>
    /// Get current score
    /// </summary>
    /// <returns>Current score</returns>
    public int GetCurrentScore()
    {
        return currentScore;
    }
    
    /// <summary>
    /// Check if game is over
    /// </summary>
    /// <returns>True if game is over</returns>
    public bool IsGameOver()
    {
        return CurrentState == GameState.GameOver;
    }
    
    /// <summary>
    /// Check if game is won
    /// </summary>
    /// <returns>True if game is won</returns>
    public bool IsGameWon()
    {
        return CurrentState == GameState.Won;
    }
}
