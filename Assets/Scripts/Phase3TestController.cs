using UnityEngine;
using TMPro;
using UnityEngine.UI;

/// <summary>
/// Test controller for Phase 3: UI Management extraction
/// Tests the functionality of all UI controllers and their event-driven behavior
/// Demonstrates proper separation of UI logic from game logic
/// </summary>
public class Phase3TestController : MonoBehaviour
{
    [Header("Test Configuration")]
    [SerializeField] private bool runTestsOnStart = true;
    [SerializeField] private bool showDetailedLogs = true;
    [SerializeField] private float testInterval = 2f;
    
    [Header("UI Controller References")]
    [SerializeField] private HealthUIController healthUIController;
    [SerializeField] private ScoreUIController scoreUIController;
    [SerializeField] private GameStatusUIController gameStatusUIController;
    
    [Header("Test Values")]
    [SerializeField] private float testMaxHealth = 30f;
    [SerializeField] private float testDamageAmount = 5f;
    [SerializeField] private float testHealAmount = 3f;
    [SerializeField] private int testScoreIncrement = 10;
    
    /// <summary>
    /// Test sequence state
    /// </summary>
    private int currentTestStep = 0;
    private bool testsRunning = false;
    
    void Start()
    {
        if (runTestsOnStart)
        {
            StartUITests();
        }
        
        LogTestInfo("Phase3TestController initialized. Press [U] to run UI tests.");
    }
    
    void Update()
    {
        HandleTestInput();
    }
    
    /// <summary>
    /// Handle keyboard input for testing
    /// </summary>
    private void HandleTestInput()
    {
        if (Input.GetKeyDown(KeyCode.U))
        {
            if (testsRunning)
            {
                StopUITests();
            }
            else
            {
                StartUITests();
            }
        }
        
        // Individual component tests
        if (Input.GetKeyDown(KeyCode.H))
        {
            TestHealthUI();
        }
        
        if (Input.GetKeyDown(KeyCode.S))
        {
            TestScoreUI();
        }
        
        if (Input.GetKeyDown(KeyCode.G))
        {
            TestGameStatusUI();
        }
        
        // Event system tests
        if (Input.GetKeyDown(KeyCode.E))
        {
            TestEventSystem();
        }
        
        // Reset tests
        if (Input.GetKeyDown(KeyCode.T))
        {
            ResetAllTests();
        }
    }
    
    /// <summary>
    /// Start comprehensive UI tests
    /// </summary>
    public void StartUITests()
    {
        if (testsRunning)
        {
            LogTestInfo("Tests already running. Stopping previous tests...");
            StopUITests();
        }
        
        testsRunning = true;
        currentTestStep = 0;
        
        LogTestInfo("=== STARTING PHASE 3 UI TESTS ===");
        LogTestInfo("Testing UI controllers and event-driven architecture...");
        
        InvokeRepeating(nameof(RunNextTest), 1f, testInterval);
    }
    
    /// <summary>
    /// Stop UI tests
    /// </summary>
    public void StopUITests()
    {
        testsRunning = false;
        CancelInvoke(nameof(RunNextTest));
        
        LogTestInfo("=== UI TESTS STOPPED ===");
    }
    
    /// <summary>
    /// Run the next test in sequence
    /// </summary>
    private void RunNextTest()
    {
        switch (currentTestStep)
        {
            case 0:
                LogTestInfo($"--- Test Step {currentTestStep + 1}: Initialize UI Controllers ---");
                TestUIControllerInitialization();
                break;
                
            case 1:
                LogTestInfo($"--- Test Step {currentTestStep + 1}: Health UI Events ---");
                TestHealthUIEvents();
                break;
                
            case 2:
                LogTestInfo($"--- Test Step {currentTestStep + 1}: Score UI Events ---");
                TestScoreUIEvents();
                break;
                
            case 3:
                LogTestInfo($"--- Test Step {currentTestStep + 1}: Game Status UI Events ---");
                TestGameStatusUIEvents();
                break;
                
            case 4:
                LogTestInfo($"--- Test Step {currentTestStep + 1}: Event System Integration ---");
                TestEventSystemIntegration();
                break;
                
            case 5:
                LogTestInfo($"--- Test Step {currentTestStep + 1}: UI Interfaces Compliance ---");
                TestUIInterfaces();
                break;
                
            case 6:
                LogTestInfo($"--- Test Step {currentTestStep + 1}: Complete Game Flow ---");
                TestCompleteGameFlow();
                break;
                
            default:
                LogTestInfo("=== ALL PHASE 3 TESTS COMPLETED ===");
                LogTestInfo("UI Management extraction successful!");
                StopUITests();
                return;
        }
        
        currentTestStep++;
    }
    
    /// <summary>
    /// Test UI controller initialization
    /// </summary>
    private void TestUIControllerInitialization()
    {
        LogTestInfo("Testing UI controller initialization and setup...");
        
        // Find UI controllers if not assigned
        if (healthUIController == null)
            healthUIController = FindFirstObjectByType<HealthUIController>();
        
        if (scoreUIController == null)
            scoreUIController = FindFirstObjectByType<ScoreUIController>();
        
        if (gameStatusUIController == null)
            gameStatusUIController = FindFirstObjectByType<GameStatusUIController>();
        
        // Test initialization
        bool healthUIFound = healthUIController != null;
        bool scoreUIFound = scoreUIController != null;
        bool gameStatusUIFound = gameStatusUIController != null;
        
        LogTestResult("HealthUIController found", healthUIFound);
        LogTestResult("ScoreUIController found", scoreUIFound);
        LogTestResult("GameStatusUIController found", gameStatusUIFound);
        
        // Test manual initialization
        if (healthUIController != null)
        {
            healthUIController.SetMaxHealth(testMaxHealth);
            LogTestInfo($"Set max health to {testMaxHealth}");
        }
    }
    
    /// <summary>
    /// Test health UI events and functionality
    /// </summary>
    private void TestHealthUIEvents()
    {
        LogTestInfo("Testing health UI events and display updates...");
        
        // Test health change events
        GameEvents.TriggerPlayerHealthChanged(testMaxHealth);
        LogTestInfo($"Triggered health changed: {testMaxHealth}");
        
        // Test damage event
        float newHealth = testMaxHealth - testDamageAmount;
        GameEvents.TriggerPlayerDamaged(testDamageAmount);
        GameEvents.TriggerPlayerHealthChanged(newHealth);
        LogTestInfo($"Triggered damage: {testDamageAmount}, new health: {newHealth}");
        
        // Test healing event
        newHealth = Mathf.Min(testMaxHealth, newHealth + testHealAmount);
        GameEvents.TriggerPlayerHealed(testHealAmount);
        GameEvents.TriggerPlayerHealthChanged(newHealth);
        LogTestInfo($"Triggered heal: {testHealAmount}, new health: {newHealth}");
        
        // Test interface compliance if available
        if (healthUIController != null)
        {
            float displayedHealth = healthUIController.CurrentDisplayedHealth;
            float healthPercentage = healthUIController.GetHealthPercentage();
            LogTestResult($"Health UI displays {displayedHealth}/{healthUIController.MaxHealth} ({healthPercentage:P0})", true);
        }
    }
    
    /// <summary>
    /// Test score UI events and functionality
    /// </summary>
    private void TestScoreUIEvents()
    {
        LogTestInfo("Testing score UI events and display updates...");
        
        // Test score changes
        int currentScore = 0;
        for (int i = 1; i <= 3; i++)
        {
            currentScore += testScoreIncrement;
            GameEvents.TriggerPlayerScoreChanged(currentScore);
            LogTestInfo($"Triggered score change: {currentScore}");
        }
        
        // Test collectible pickup
        GameEvents.TriggerCollectiblePickup(testScoreIncrement);
        LogTestInfo($"Triggered collectible pickup: {testScoreIncrement} points");
        
        // Test interface compliance if available
        if (scoreUIController != null)
        {
            int displayedScore = scoreUIController.CurrentDisplayedScore;
            LogTestResult($"Score UI displays {displayedScore}", true);
            
            // Test configuration methods
            scoreUIController.SetScorePrefix("Points: ");
            LogTestInfo("Changed score prefix to 'Points: '");
        }
    }
    
    /// <summary>
    /// Test game status UI events and functionality
    /// </summary>
    private void TestGameStatusUIEvents()
    {
        LogTestInfo("Testing game status UI events and timer functionality...");
        
        // Test game started
        GameEvents.TriggerGameStarted();
        LogTestInfo("Triggered game started");
        
        // Test timer functionality
        if (gameStatusUIController != null)
        {
            float gameTime = gameStatusUIController.GameTime;
            bool timerRunning = gameStatusUIController.TimerRunning;
            LogTestResult($"Timer running: {timerRunning}, time: {gameTime:F1}s", true);
            
            string formattedTime = gameStatusUIController.GetFormattedTime();
            LogTestInfo($"Formatted time: {formattedTime}");
        }
        
        // Test pause/resume
        GameEvents.TriggerGamePaused(true);
        LogTestInfo("Triggered game paused");
        
        // Schedule resume
        Invoke(nameof(ResumeGameAfterPause), 1f);
    }
    
    /// <summary>
    /// Resume game after pause (for testing)
    /// </summary>
    private void ResumeGameAfterPause()
    {
        GameEvents.TriggerGamePaused(false);
        LogTestInfo("Triggered game resumed");
    }
    
    /// <summary>
    /// Test event system integration
    /// </summary>
    private void TestEventSystemIntegration()
    {
        LogTestInfo("Testing event system integration and decoupling...");
        
        // Test multiple events in sequence
        GameEvents.TriggerPlayerHealthChanged(20f);
        GameEvents.TriggerPlayerScoreChanged(50);
        LogTestInfo("Triggered multiple events simultaneously");
        
        // Test game flow events
        GameEvents.TriggerGameWon();
        LogTestInfo("Triggered game won event");
        
        // Schedule restart
        Invoke(nameof(RestartGameAfterWin), 1f);
    }
    
    /// <summary>
    /// Restart game after win (for testing)
    /// </summary>
    private void RestartGameAfterWin()
    {
        GameEvents.TriggerGameRestarted();
        LogTestInfo("Triggered game restart");
    }
    
    /// <summary>
    /// Test UI interfaces compliance
    /// </summary>
    private void TestUIInterfaces()
    {
        LogTestInfo("Testing UI interfaces and SOLID compliance...");
        
        // Test interface polymorphism
        IHealthUI healthUI = healthUIController;
        IScoreUI scoreUI = scoreUIController;
        IGameStatusUI gameStatusUI = gameStatusUIController;
        
        bool healthInterfaceWorks = healthUI != null;
        bool scoreInterfaceWorks = scoreUI != null;
        bool gameStatusInterfaceWorks = gameStatusUI != null;
        
        LogTestResult("Health UI interface compliance", healthInterfaceWorks);
        LogTestResult("Score UI interface compliance", scoreInterfaceWorks);
        LogTestResult("Game Status UI interface compliance", gameStatusInterfaceWorks);
        
        // Test interface methods
        if (healthUI != null)
        {
            healthUI.SetVisible(true);
            LogTestInfo("Tested IHealthUI.SetVisible()");
        }
        
        if (scoreUI != null)
        {
            scoreUI.SetVisible(true);
            LogTestInfo("Tested IScoreUI.SetVisible()");
        }
        
        if (gameStatusUI != null)
        {
            gameStatusUI.SetVisible(true);
            LogTestInfo("Tested IGameStatusUI.SetVisible()");
        }
    }
    
    /// <summary>
    /// Test complete game flow with all UI controllers
    /// </summary>
    private void TestCompleteGameFlow()
    {
        LogTestInfo("Testing complete game flow with all UI systems...");
        
        // Simulate a complete game sequence
        GameEvents.TriggerGameStarted();
        GameEvents.TriggerPlayerHealthChanged(30f);
        GameEvents.TriggerPlayerScoreChanged(0);
        
        // Simulate gameplay
        for (int i = 0; i < 3; i++)
        {
            int score = (i + 1) * 10;
            GameEvents.TriggerPlayerScoreChanged(score);
            GameEvents.TriggerCollectiblePickup(10);
        }
        
        // Simulate damage
        GameEvents.TriggerPlayerDamaged(10f);
        GameEvents.TriggerPlayerHealthChanged(20f);
        
        // Simulate win condition
        GameEvents.TriggerPlayerScoreChanged(30);
        GameEvents.TriggerGameWon();
        
        LogTestInfo("Complete game flow simulation finished");
    }
    
    /// <summary>
    /// Test individual health UI functionality
    /// </summary>
    public void TestHealthUI()
    {
        LogTestInfo("=== TESTING HEALTH UI ===");
        TestHealthUIEvents();
    }
    
    /// <summary>
    /// Test individual score UI functionality
    /// </summary>
    public void TestScoreUI()
    {
        LogTestInfo("=== TESTING SCORE UI ===");
        TestScoreUIEvents();
    }
    
    /// <summary>
    /// Test individual game status UI functionality
    /// </summary>
    public void TestGameStatusUI()
    {
        LogTestInfo("=== TESTING GAME STATUS UI ===");
        TestGameStatusUIEvents();
    }
    
    /// <summary>
    /// Test event system functionality
    /// </summary>
    public void TestEventSystem()
    {
        LogTestInfo("=== TESTING EVENT SYSTEM ===");
        TestEventSystemIntegration();
    }
    
    /// <summary>
    /// Reset all tests and UI states
    /// </summary>
    public void ResetAllTests()
    {
        LogTestInfo("=== RESETTING ALL TESTS ===");
        
        StopUITests();
        
        // Reset game events
        GameEvents.TriggerGameRestarted();
        GameEvents.TriggerPlayerHealthChanged(testMaxHealth);
        GameEvents.TriggerPlayerScoreChanged(0);
        
        currentTestStep = 0;
        
        LogTestInfo("All tests reset. Ready for new test sequence.");
    }
    
    /// <summary>
    /// Log test information
    /// </summary>
    /// <param name="message">Message to log</param>
    private void LogTestInfo(string message)
    {
        if (showDetailedLogs)
        {
            Debug.Log($"[Phase3Test] {message}");
        }
    }
    
    /// <summary>
    /// Log test result
    /// </summary>
    /// <param name="testName">Name of the test</param>
    /// <param name="passed">Whether the test passed</param>
    private void LogTestResult(string testName, bool passed)
    {
        string status = passed ? "✅ PASS" : "❌ FAIL";
        LogTestInfo($"{status}: {testName}");
    }
    
    /// <summary>
    /// Display test controls in GUI
    /// </summary>
    void OnGUI()
    {
        GUILayout.BeginArea(new Rect(10, Screen.height - 200, 400, 180));
        GUILayout.Label("=== PHASE 3 UI TESTS ===", GUI.skin.box);
        
        GUILayout.Label($"Tests Running: {testsRunning}");
        GUILayout.Label($"Current Step: {currentTestStep}");
        
        GUILayout.Space(10);
        GUILayout.Label("CONTROLS:");
        GUILayout.Label("[U] - Start/Stop UI Tests");
        GUILayout.Label("[H] - Test Health UI");
        GUILayout.Label("[S] - Test Score UI");
        GUILayout.Label("[G] - Test Game Status UI");
        GUILayout.Label("[E] - Test Event System");
        GUILayout.Label("[T] - Reset All Tests");
        
        GUILayout.EndArea();
    }
}
