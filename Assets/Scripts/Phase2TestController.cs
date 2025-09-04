using UnityEngine;

/// <summary>
/// Phase 2 test controller to verify event system and input handler
/// Demonstrates decoupled communication through events
/// </summary>
public class Phase2TestController : MonoBehaviour
{
    [Header("Testing Controls")]
    [SerializeField] private bool enableTesting = true;
    [SerializeField] private bool logEventDetails = true;
    
    private IHealthSystem healthSystem;
    private IScoreSystem scoreSystem;
    private IMovementController movementController;
    private IInputHandler inputHandler;
    
    // Event subscription tracking
    private int healthEventCount = 0;
    private int scoreEventCount = 0;
    private int jumpEventCount = 0;
    
    void Start()
    {
        InitializeComponents();
        SubscribeToAllEvents();
        
        Debug.Log("=== Phase 2 Test Controller Initialized ===");
        Debug.Log("Event System and Input Handler Ready for Testing");
        Debug.Log("Use WASD, Mouse, Space, R, Esc to test input system");
    }
    
    void Update()
    {
        if (!enableTesting) return;
        
        ProcessInputThroughHandler();
        
        // Debug key to show event statistics
        if (Input.GetKeyDown(KeyCode.F1))
        {
            ShowEventStatistics();
        }
        
        // Debug key to test all systems at once
        if (Input.GetKeyDown(KeyCode.F2))
        {
            TestAllSystemsSequentially();
        }
    }
    
    /// <summary>
    /// Initialize all components and verify they exist
    /// </summary>
    private void InitializeComponents()
    {
        healthSystem = GetComponent<IHealthSystem>();
        scoreSystem = GetComponent<IScoreSystem>();
        movementController = GetComponent<IMovementController>();
        inputHandler = GetComponent<IInputHandler>();
        
        // Verify components
        if (healthSystem == null) Debug.LogWarning("Health System not found!");
        if (scoreSystem == null) Debug.LogWarning("Score System not found!");
        if (movementController == null) Debug.LogWarning("Movement Controller not found!");
        if (inputHandler == null) Debug.LogWarning("Input Handler not found!");
        
        Debug.Log("Components initialized successfully");
    }
    
    /// <summary>
    /// Subscribe to all game events to test the event system
    /// </summary>
    private void SubscribeToAllEvents()
    {
        // Player Events
        GameEvents.OnPlayerHealthChanged += OnPlayerHealthChanged;
        GameEvents.OnPlayerDied += OnPlayerDied;
        GameEvents.OnPlayerScoreChanged += OnPlayerScoreChanged;
        GameEvents.OnPlayerJumped += OnPlayerJumped;
        GameEvents.OnPlayerLanded += OnPlayerLanded;
        
        // Game State Events
        GameEvents.OnGameWon += OnGameWon;
        GameEvents.OnGameOver += OnGameOver;
        GameEvents.OnGameRestarted += OnGameRestarted;
        GameEvents.OnGamePaused += OnGamePaused;
        
        // Interaction Events
        GameEvents.OnCollectiblePickup += OnCollectiblePickup;
        GameEvents.OnEnemyDefeated += OnEnemyDefeated;
        GameEvents.OnDamageTaken += OnDamageTaken;
        
        // Input Events
        if (inputHandler != null)
        {
            inputHandler.OnJumpPressed += OnInputJumpPressed;
            inputHandler.OnRestartPressed += OnInputRestartPressed;
            inputHandler.OnPausePressed += OnInputPausePressed;
            inputHandler.OnInteractPressed += OnInputInteractPressed;
        }
        
        Debug.Log("Subscribed to all game events");
    }
    
    /// <summary>
    /// Process input through the input handler and trigger movement
    /// </summary>
    private void ProcessInputThroughHandler()
    {
        if (inputHandler == null || movementController == null) return;
        
        // Movement input
        Vector2 movement = inputHandler.MovementInput;
        if (movement.magnitude > 0.1f)
        {
            movementController.Move(movement);
        }
        
        // Rotation input
        float mouseX = inputHandler.MouseX;
        if (Mathf.Abs(mouseX) > 0.01f)
        {
            movementController.Rotate(mouseX);
        }
        
        // Jump input
        if (inputHandler.JumpPressed)
        {
            movementController.Jump();
            GameEvents.TriggerPlayerJumped(); // Trigger through event system
        }
    }
    
    /// <summary>
    /// Test all systems in sequence to verify functionality
    /// </summary>
    private void TestAllSystemsSequentially()
    {
        Debug.Log("=== Testing All Systems ===");
        
        // Test Health System
        if (healthSystem != null)
        {
            Debug.Log("Testing Health System...");
            healthSystem.TakeDamage(5f);
            healthSystem.Heal(2f);
        }
        
        // Test Score System
        if (scoreSystem != null)
        {
            Debug.Log("Testing Score System...");
            scoreSystem.AddScore(25);
        }
        
        // Test Event System
        Debug.Log("Testing Event System...");
        GameEvents.TriggerShowUIMessage("All systems test completed!");
        GameEvents.TriggerCollectiblePickup(15);
        
        // Test Input System
        if (inputHandler != null)
        {
            // Cast to concrete type to access debug method (temporary workaround)
            var concreteHandler = inputHandler as PlayerInputHandler;
            if (concreteHandler != null)
            {
                Debug.Log($"Input System Status: {concreteHandler.GetInputDebugInfo()}");
            }
            else
            {
                Debug.Log("Input System Status: Handler found but debug info not available");
            }
        }
    }
    
    /// <summary>
    /// Display statistics about event firing
    /// </summary>
    private void ShowEventStatistics()
    {
        Debug.Log("=== Event Statistics ===");
        Debug.Log($"Health Events Fired: {healthEventCount}");
        Debug.Log($"Score Events Fired: {scoreEventCount}");
        Debug.Log($"Jump Events Fired: {jumpEventCount}");
        
        // Show GameEvents debug info
        GameEvents.LogEventSubscribers();
    }
    
    // ========================================
    // EVENT HANDLERS
    // ========================================
    
    #region Game Event Handlers
    
    private void OnPlayerHealthChanged(float health)
    {
        healthEventCount++;
        if (logEventDetails)
            Debug.Log($"[Phase2Test] Health Event #{healthEventCount}: Health = {health}");
    }
    
    private void OnPlayerDied()
    {
        if (logEventDetails)
            Debug.Log("[Phase2Test] Player Death Event Received!");
    }
    
    private void OnPlayerScoreChanged(int score)
    {
        scoreEventCount++;
        if (logEventDetails)
            Debug.Log($"[Phase2Test] Score Event #{scoreEventCount}: Score = {score}");
    }
    
    private void OnPlayerJumped()
    {
        jumpEventCount++;
        if (logEventDetails)
            Debug.Log($"[Phase2Test] Jump Event #{jumpEventCount}: Player Jumped!");
    }
    
    private void OnPlayerLanded()
    {
        if (logEventDetails)
            Debug.Log("[Phase2Test] Player Landed Event Received!");
    }
    
    private void OnGameWon()
    {
        if (logEventDetails)
            Debug.Log("[Phase2Test] Game Won Event Received!");
    }
    
    private void OnGameOver()
    {
        if (logEventDetails)
            Debug.Log("[Phase2Test] Game Over Event Received!");
    }
    
    private void OnGameRestarted()
    {
        if (logEventDetails)
            Debug.Log("[Phase2Test] Game Restarted Event Received!");
    }
    
    private void OnGamePaused(bool isPaused)
    {
        if (logEventDetails)
            Debug.Log($"[Phase2Test] Game Pause Event: {(isPaused ? "Paused" : "Unpaused")}");
    }
    
    private void OnCollectiblePickup(int points)
    {
        if (logEventDetails)
            Debug.Log($"[Phase2Test] Collectible Pickup Event: +{points} points");
    }
    
    private void OnEnemyDefeated(float reward)
    {
        if (logEventDetails)
            Debug.Log($"[Phase2Test] Enemy Defeated Event: +{reward} reward");
    }
    
    private void OnDamageTaken(float damage, string source)
    {
        if (logEventDetails)
            Debug.Log($"[Phase2Test] Damage Taken Event: {damage} from {source}");
    }
    
    #endregion
    
    #region Input Event Handlers
    
    private void OnInputJumpPressed()
    {
        if (logEventDetails)
            Debug.Log("[Phase2Test] Input Jump Event Received!");
    }
    
    private void OnInputRestartPressed()
    {
        if (logEventDetails)
        {
            Debug.Log("[Phase2Test] Input Restart Event Received!");
            GameEvents.TriggerGameRestarted();
        }
    }
    
    private void OnInputPausePressed()
    {
        if (logEventDetails)
        {
            Debug.Log("[Phase2Test] Input Pause Event Received!");
            GameEvents.TriggerGamePaused(true);
        }
    }
    
    private void OnInputInteractPressed()
    {
        if (logEventDetails)
            Debug.Log("[Phase2Test] Input Interact Event Received!");
    }
    
    #endregion
    
    /// <summary>
    /// Clean up event subscriptions
    /// </summary>
    void OnDestroy()
    {
        // Unsubscribe from game events
        GameEvents.OnPlayerHealthChanged -= OnPlayerHealthChanged;
        GameEvents.OnPlayerDied -= OnPlayerDied;
        GameEvents.OnPlayerScoreChanged -= OnPlayerScoreChanged;
        GameEvents.OnPlayerJumped -= OnPlayerJumped;
        GameEvents.OnPlayerLanded -= OnPlayerLanded;
        GameEvents.OnGameWon -= OnGameWon;
        GameEvents.OnGameOver -= OnGameOver;
        GameEvents.OnGameRestarted -= OnGameRestarted;
        GameEvents.OnGamePaused -= OnGamePaused;
        GameEvents.OnCollectiblePickup -= OnCollectiblePickup;
        GameEvents.OnEnemyDefeated -= OnEnemyDefeated;
        GameEvents.OnDamageTaken -= OnDamageTaken;
        
        // Unsubscribe from input events
        if (inputHandler != null)
        {
            inputHandler.OnJumpPressed -= OnInputJumpPressed;
            inputHandler.OnRestartPressed -= OnInputRestartPressed;
            inputHandler.OnPausePressed -= OnInputPausePressed;
            inputHandler.OnInteractPressed -= OnInputInteractPressed;
        }
        
        Debug.Log("Phase2TestController cleanup completed");
    }
}
