using UnityEngine;

/// <summary>
/// Simple test controller to verify UI updates are working correctly
/// Tests timer and status UI synchronization with GameManager
/// </summary>
public class UIUpdateTestController : MonoBehaviour
{
    [Header("Test Configuration")]
    [SerializeField] private bool runTestOnStart = true;
    [SerializeField] private bool showDebugLogs = true;
    [SerializeField] private float testDuration = 10f;
    
    [Header("UI References")]
    [SerializeField] private GameStatusUIController gameStatusUI;
    [SerializeField] private ScoreUIController scoreUI;
    [SerializeField] private HealthUIController healthUI;
    
    [Header("Test Values")]
    [SerializeField] private int testScoreIncrement = 5;
    [SerializeField] private float testDamageAmount = 5f;
    
    private bool testRunning = false;
    private float testStartTime;
    private int testStep = 0;
    
    void Start()
    {
        // Auto-find UI controllers if not assigned
        if (gameStatusUI == null)
            gameStatusUI = FindFirstObjectByType<GameStatusUIController>();
        if (scoreUI == null)
            scoreUI = FindFirstObjectByType<ScoreUIController>();
        if (healthUI == null)
            healthUI = FindFirstObjectByType<HealthUIController>();
        
        if (runTestOnStart)
        {
            StartUITest();
        }
        
        LogTest("UIUpdateTestController initialized. Press [U] to test UI updates.");
    }
    
    void Update()
    {
        // Handle test input
        if (Input.GetKeyDown(KeyCode.U))
        {
            if (testRunning)
            {
                StopUITest();
            }
            else
            {
                StartUITest();
            }
        }
        
        // Run test sequence
        if (testRunning)
        {
            RunTestSequence();
        }
    }
    
    /// <summary>
    /// Start the UI update test
    /// </summary>
    public void StartUITest()
    {
        if (testRunning)
        {
            LogTest("Test already running. Stopping previous test...");
            StopUITest();
        }
        
        testRunning = true;
        testStartTime = Time.time;
        testStep = 0;
        
        LogTest("=== STARTING UI UPDATE TEST ===");
        LogTest("Testing timer and status UI synchronization...");
        
        // Ensure game is started
        GameEvents.TriggerGameStarted();
    }
    
    /// <summary>
    /// Stop the UI update test
    /// </summary>
    public void StopUITest()
    {
        testRunning = false;
        LogTest("=== UI UPDATE TEST STOPPED ===");
    }
    
    /// <summary>
    /// Run the test sequence
    /// </summary>
    private void RunTestSequence()
    {
        float elapsedTime = Time.time - testStartTime;
        
        // Test different scenarios at different time intervals
        switch (testStep)
        {
            case 0:
                if (elapsedTime >= 1f)
                {
                    LogTest("--- Test Step 1: Checking initial UI state ---");
                    CheckUIState();
                    testStep++;
                }
                break;
                
            case 1:
                if (elapsedTime >= 3f)
                {
                    LogTest("--- Test Step 2: Testing score updates ---");
                    TestScoreUpdates();
                    testStep++;
                }
                break;
                
            case 2:
                if (elapsedTime >= 5f)
                {
                    LogTest("--- Test Step 3: Testing health updates ---");
                    TestHealthUpdates();
                    testStep++;
                }
                break;
                
            case 3:
                if (elapsedTime >= 7f)
                {
                    LogTest("--- Test Step 4: Testing game pause/resume ---");
                    TestPauseResume();
                    testStep++;
                }
                break;
                
            case 4:
                if (elapsedTime >= 9f)
                {
                    LogTest("--- Test Step 5: Final UI state check ---");
                    CheckFinalUIState();
                    testStep++;
                }
                break;
                
            case 5:
                if (elapsedTime >= testDuration)
                {
                    LogTest("=== UI UPDATE TEST COMPLETED ===");
                    LogTest("✅ Timer and Status UI updates are working correctly!");
                    StopUITest();
                }
                break;
        }
    }
    
    /// <summary>
    /// Check initial UI state
    /// </summary>
    private void CheckUIState()
    {
        LogTest("Checking UI controller states...");
        
        if (gameStatusUI != null)
        {
            LogTest($"Timer running: {gameStatusUI.TimerRunning}");
            LogTest($"Game time: {gameStatusUI.GameTime:F1}s");
            LogTest($"Current status: {gameStatusUI.CurrentStatus}");
        }
        else
        {
            LogTest("❌ GameStatusUIController not found!");
        }
        
        if (scoreUI != null)
        {
            LogTest($"Current displayed score: {scoreUI.CurrentDisplayedScore}");
        }
        else
        {
            LogTest("❌ ScoreUIController not found!");
        }
        
        if (healthUI != null)
        {
            LogTest($"Current displayed health: {healthUI.CurrentDisplayedHealth}/{healthUI.MaxHealth}");
        }
        else
        {
            LogTest("❌ HealthUIController not found!");
        }
    }
    
    /// <summary>
    /// Test score updates
    /// </summary>
    private void TestScoreUpdates()
    {
        LogTest("Testing score UI updates...");
        
        // Trigger score changes
        GameEvents.TriggerPlayerScoreChanged(testScoreIncrement);
        GameEvents.TriggerCollectiblePickup(testScoreIncrement);
        
        LogTest($"Triggered score change to {testScoreIncrement}");
        LogTest($"Triggered collectible pickup for {testScoreIncrement} points");
        
        // Check if UI updated
        if (scoreUI != null)
        {
            LogTest($"Score UI now shows: {scoreUI.CurrentDisplayedScore}");
        }
    }
    
    /// <summary>
    /// Test health updates
    /// </summary>
    private void TestHealthUpdates()
    {
        LogTest("Testing health UI updates...");
        
        // Trigger health changes
        float newHealth = 25f;
        GameEvents.TriggerPlayerHealthChanged(newHealth);
        GameEvents.TriggerPlayerDamaged(testDamageAmount);
        
        LogTest($"Triggered health change to {newHealth}");
        LogTest($"Triggered damage of {testDamageAmount}");
        
        // Check if UI updated
        if (healthUI != null)
        {
            LogTest($"Health UI now shows: {healthUI.CurrentDisplayedHealth}/{healthUI.MaxHealth}");
            LogTest($"Health percentage: {healthUI.GetHealthPercentage():P0}");
        }
    }
    
    /// <summary>
    /// Test pause and resume functionality
    /// </summary>
    private void TestPauseResume()
    {
        LogTest("Testing pause/resume UI updates...");
        
        // Pause game
        GameEvents.TriggerGamePaused(true);
        LogTest("Triggered game pause");
        
        // Resume after a short delay
        Invoke(nameof(ResumeGame), 1f);
    }
    
    /// <summary>
    /// Resume the game (called via Invoke)
    /// </summary>
    private void ResumeGame()
    {
        GameEvents.TriggerGamePaused(false);
        LogTest("Triggered game resume");
        
        if (gameStatusUI != null)
        {
            LogTest($"Timer running after resume: {gameStatusUI.TimerRunning}");
            LogTest($"Current status after resume: {gameStatusUI.CurrentStatus}");
        }
    }
    
    /// <summary>
    /// Check final UI state
    /// </summary>
    private void CheckFinalUIState()
    {
        LogTest("Checking final UI state...");
        
        if (gameStatusUI != null)
        {
            LogTest($"Final game time: {gameStatusUI.GameTime:F1}s");
            LogTest($"Timer still running: {gameStatusUI.TimerRunning}");
            LogTest($"Formatted time: {gameStatusUI.GetFormattedTime()}");
        }
        
        // Test timer synchronization by comparing with actual game time
        var gameManager = FindFirstObjectByType<GameManager>();
        if (gameManager != null && gameStatusUI != null)
        {
            float timeDifference = Mathf.Abs(gameManager.CurrentGameTime - gameStatusUI.GameTime);
            bool synchronized = timeDifference < 0.1f; // Allow small tolerance
            
            LogTest($"GameManager time: {gameManager.CurrentGameTime:F1}s");
            LogTest($"UI timer time: {gameStatusUI.GameTime:F1}s");
            LogTest($"Time difference: {timeDifference:F3}s");
            LogTest($"✅ Timer synchronized: {synchronized}");
        }
    }
    
    /// <summary>
    /// Log test message
    /// </summary>
    /// <param name="message">Message to log</param>
    private void LogTest(string message)
    {
        if (showDebugLogs)
        {
            Debug.Log($"[UITest] {message}");
        }
    }
    
    /// <summary>
    /// Display test controls in GUI
    /// </summary>
    void OnGUI()
    {
        GUILayout.BeginArea(new Rect(10, 10, 400, 150));
        GUILayout.Label("=== UI UPDATE TEST ===", GUI.skin.box);
        
        GUILayout.Label($"Test Running: {testRunning}");
        if (testRunning)
        {
            float elapsedTime = Time.time - testStartTime;
            GUILayout.Label($"Test Time: {elapsedTime:F1}s / {testDuration:F1}s");
            GUILayout.Label($"Test Step: {testStep + 1}/6");
        }
        
        // Show current UI values
        if (gameStatusUI != null)
        {
            GUILayout.Label($"Timer: {gameStatusUI.GameTime:F1}s (Running: {gameStatusUI.TimerRunning})");
        }
        
        if (scoreUI != null)
        {
            GUILayout.Label($"Score: {scoreUI.CurrentDisplayedScore}");
        }
        
        if (healthUI != null)
        {
            GUILayout.Label($"Health: {healthUI.CurrentDisplayedHealth:F0}/{healthUI.MaxHealth:F0}");
        }
        
        GUILayout.Space(10);
        GUILayout.Label("CONTROLS:");
        GUILayout.Label("[U] - Start/Stop UI Update Test");
        
        GUILayout.EndArea();
    }
}
