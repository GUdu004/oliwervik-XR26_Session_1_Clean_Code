using UnityEngine;

/// <summary>
/// Modern PlayerController that uses composition instead of inheritance
/// Demonstrates SOLID principles in action:
/// - Single Responsibility: Only coordinates between components
/// - Open/Closed: Extensible through component addition
/// - Liskov Substitution: Works with any component implementing the interfaces
/// - Interface Segregation: Uses focused, specific interfaces
/// - Dependency Inversion: Depends on abstractions, not concrete classes
/// </summary>
public class PlayerController : MonoBehaviour
{
    [Header("Component Dependencies (Auto-resolved)")]
    private IHealthSystem healthSystem;
    private IScoreSystem scoreSystem;
    private IMovementController movementController;
    private IInputHandler inputHandler;
    
    [Header("Configuration")]
    [SerializeField] private bool enableInput = true;
    [SerializeField] private bool enableMovement = true;
    [SerializeField] private bool showDebugLogs = false;
    
    [Header("Cursor Settings")]
    [SerializeField] private bool lockCursor = true;
    [SerializeField] private bool hideCursor = true;
    
    /// <summary>
    /// Whether the player controller is currently active
    /// </summary>
    public bool IsActive { get; private set; } = true;
    
    /// <summary>
    /// Whether all required components are present
    /// </summary>
    public bool IsInitialized { get; private set; } = false;
    
    void Start()
    {
        InitializePlayerController();
        
        if (showDebugLogs)
            Debug.Log("PlayerController: Initialization complete");
    }
    
    void Update()
    {
        if (!IsActive || !IsInitialized || !enableInput) return;
        
        HandlePlayerInput();
    }
    
    void FixedUpdate()
    {
        if (!IsActive || !IsInitialized || !enableMovement) return;
        
        HandlePlayerMovement();
    }
    
    void OnDestroy()
    {
        CleanupEventListeners();
        
        if (showDebugLogs)
            Debug.Log("PlayerController: Cleanup complete");
    }
    
    /// <summary>
    /// Initialize the player controller and all its components
    /// </summary>
    private void InitializePlayerController()
    {
        // Resolve component dependencies
        ResolveDependencies();
        
        // Setup event connections
        SetupEventConnections();
        
        // Initialize cursor settings
        InitializeCursorSettings();
        
        // Mark as initialized
        IsInitialized = true;
        
        // Trigger game started event
        GameEvents.TriggerGameStarted();
        
        if (showDebugLogs)
            Debug.Log("PlayerController: All systems initialized successfully");
    }
    
    /// <summary>
    /// Resolve component dependencies using dependency injection pattern
    /// </summary>
    private void ResolveDependencies()
    {
        // Get required components from this GameObject
        healthSystem = GetComponent<IHealthSystem>();
        scoreSystem = GetComponent<IScoreSystem>();
        movementController = GetComponent<IMovementController>();
        inputHandler = GetComponent<IInputHandler>();
        
        // Validate that all required components are present
        ValidateDependencies();
    }
    
    /// <summary>
    /// Validate that all required dependencies are resolved
    /// </summary>
    private void ValidateDependencies()
    {
        bool allComponentsFound = true;
        
        if (healthSystem == null)
        {
            Debug.LogError("PlayerController: IHealthSystem component not found! Add HealthComponent to this GameObject.");
            allComponentsFound = false;
        }
        
        if (scoreSystem == null)
        {
            Debug.LogError("PlayerController: IScoreSystem component not found! Add ScoreComponent to this GameObject.");
            allComponentsFound = false;
        }
        
        if (movementController == null)
        {
            Debug.LogError("PlayerController: IMovementController component not found! Add PlayerMovement to this GameObject.");
            allComponentsFound = false;
        }
        
        if (inputHandler == null)
        {
            Debug.LogError("PlayerController: IInputHandler component not found! Add PlayerInputHandler to this GameObject.");
            allComponentsFound = false;
        }
        
        if (!allComponentsFound)
        {
            Debug.LogError("PlayerController: Missing required components! Player will not function correctly.");
            IsActive = false;
        }
    }
    
    /// <summary>
    /// Setup event connections between local components and global events
    /// </summary>
    private void SetupEventConnections()
    {
        if (healthSystem != null)
        {
            // Connect health system events to global events
            healthSystem.OnHealthChanged += GameEvents.TriggerPlayerHealthChanged;
            healthSystem.OnDeath += HandlePlayerDeath;
        }
        
        if (scoreSystem != null)
        {
            // Connect score system events to global events
            scoreSystem.OnScoreChanged += GameEvents.TriggerPlayerScoreChanged;
        }
        
        if (movementController != null && movementController is PlayerMovement playerMovement)
        {
            // Connect movement events if the movement controller supports them
            playerMovement.OnJumped += GameEvents.TriggerPlayerJumped;
            playerMovement.OnLanded += GameEvents.TriggerPlayerLanded;
        }
        
        if (inputHandler != null && inputHandler is PlayerInputHandler playerInput)
        {
            // Connect input events for additional functionality
            playerInput.OnJumpPressed += HandleJumpInput;
            playerInput.OnRestartPressed += HandleRestartInput;
            playerInput.OnPausePressed += HandlePauseInput;
        }
        
        if (showDebugLogs)
            Debug.Log("PlayerController: Event connections established");
    }
    
    /// <summary>
    /// Initialize cursor settings
    /// </summary>
    private void InitializeCursorSettings()
    {
        if (lockCursor)
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
        
        if (hideCursor)
        {
            Cursor.visible = false;
        }
        
        if (showDebugLogs)
            Debug.Log($"PlayerController: Cursor settings - Locked: {lockCursor}, Hidden: {hideCursor}");
    }
    
    /// <summary>
    /// Handle player input processing
    /// </summary>
    private void HandlePlayerInput()
    {
        if (inputHandler == null) return;
        
        // Handle rotation input
        float mouseX = inputHandler.MouseX;
        if (Mathf.Abs(mouseX) > 0.01f)
        {
            movementController?.Rotate(mouseX);
        }
        
        // Note: Jump input is handled through events (HandleJumpInput)
        // This demonstrates both direct input handling and event-driven input
    }
    
    /// <summary>
    /// Handle player movement processing
    /// </summary>
    private void HandlePlayerMovement()
    {
        if (inputHandler == null || movementController == null) return;
        
        // Get movement input and apply it
        Vector2 movementInput = inputHandler.MovementInput;
        if (movementInput.magnitude > 0.1f)
        {
            movementController.Move(movementInput);
        }
    }
    
    /// <summary>
    /// Handle jump input event
    /// </summary>
    private void HandleJumpInput()
    {
        if (movementController != null && IsActive)
        {
            movementController.Jump();
        }
    }
    
    /// <summary>
    /// Handle restart input event
    /// </summary>
    private void HandleRestartInput()
    {
        if (showDebugLogs)
            Debug.Log("PlayerController: Restart requested by player");
        
        GameEvents.TriggerGameRestarted();
    }
    
    /// <summary>
    /// Handle pause input event
    /// </summary>
    private void HandlePauseInput()
    {
        if (showDebugLogs)
            Debug.Log("PlayerController: Pause requested by player");
        
        // Toggle pause state (this could be more sophisticated)
        bool isPaused = Time.timeScale == 0f;
        GameEvents.TriggerGamePaused(!isPaused);
    }
    
    /// <summary>
    /// Handle player death event
    /// </summary>
    private void HandlePlayerDeath()
    {
        IsActive = false;
        
        // Trigger global death event
        GameEvents.TriggerPlayerDied();
        
        if (showDebugLogs)
            Debug.Log("PlayerController: Player died - controller deactivated");
    }
    
    /// <summary>
    /// Cleanup event listeners to prevent memory leaks
    /// </summary>
    private void CleanupEventListeners()
    {
        if (healthSystem != null)
        {
            healthSystem.OnHealthChanged -= GameEvents.TriggerPlayerHealthChanged;
            healthSystem.OnDeath -= HandlePlayerDeath;
        }
        
        if (scoreSystem != null)
        {
            scoreSystem.OnScoreChanged -= GameEvents.TriggerPlayerScoreChanged;
        }
        
        if (movementController != null && movementController is PlayerMovement playerMovement)
        {
            playerMovement.OnJumped -= GameEvents.TriggerPlayerJumped;
            playerMovement.OnLanded -= GameEvents.TriggerPlayerLanded;
        }
        
        if (inputHandler != null && inputHandler is PlayerInputHandler playerInput)
        {
            playerInput.OnJumpPressed -= HandleJumpInput;
            playerInput.OnRestartPressed -= HandleRestartInput;
            playerInput.OnPausePressed -= HandlePauseInput;
        }
    }
    
    /// <summary>
    /// Enable or disable the player controller
    /// </summary>
    /// <param name="active">Whether the controller should be active</param>
    public void SetActive(bool active)
    {
        IsActive = active;
        
        if (showDebugLogs)
            Debug.Log($"PlayerController: Set active to {active}");
    }
    
    /// <summary>
    /// Enable or disable player input
    /// </summary>
    /// <param name="enabled">Whether input should be enabled</param>
    public void SetInputEnabled(bool enabled)
    {
        enableInput = enabled;
        
        if (inputHandler != null && inputHandler is PlayerInputHandler playerInput)
        {
            playerInput.InputEnabled = enabled;
        }
        
        if (showDebugLogs)
            Debug.Log($"PlayerController: Input enabled: {enabled}");
    }
    
    /// <summary>
    /// Enable or disable player movement
    /// </summary>
    /// <param name="enabled">Whether movement should be enabled</param>
    public void SetMovementEnabled(bool enabled)
    {
        enableMovement = enabled;
        
        if (movementController != null && movementController is PlayerMovement playerMovement)
        {
            playerMovement.SetMovementEnabled(enabled);
        }
        
        if (showDebugLogs)
            Debug.Log($"PlayerController: Movement enabled: {enabled}");
    }
    
    /// <summary>
    /// Get current health percentage
    /// </summary>
    /// <returns>Health percentage (0-1)</returns>
    public float GetHealthPercentage()
    {
        if (healthSystem == null) return 0f;
        return healthSystem.CurrentHealth / healthSystem.MaxHealth;
    }
    
    /// <summary>
    /// Get current score
    /// </summary>
    /// <returns>Current score</returns>
    public int GetCurrentScore()
    {
        if (scoreSystem == null) return 0;
        return scoreSystem.CurrentScore;
    }
    
    /// <summary>
    /// Check if player is grounded
    /// </summary>
    /// <returns>True if grounded</returns>
    public bool IsGrounded()
    {
        if (movementController == null) return false;
        return movementController.IsGrounded;
    }
    
    /// <summary>
    /// Check if player is alive
    /// </summary>
    /// <returns>True if alive</returns>
    public bool IsAlive()
    {
        if (healthSystem == null) return false;
        return healthSystem.IsAlive;
    }
}
