using UnityEngine;

/// <summary>
/// Example integration showing how Phase 1 and Phase 2 components work together
/// Demonstrates SOLID principles in action with decoupled, event-driven architecture
/// </summary>
public class IntegratedPlayerController : MonoBehaviour
{
    [Header("Dependencies (Injected via GetComponent)")]
    private IHealthSystem healthSystem;
    private IScoreSystem scoreSystem;
    private IMovementController movementController;
    private IInputHandler inputHandler;
    
    [Header("Configuration")]
    [SerializeField] private bool enableMovement = true;
    [SerializeField] private bool enableInput = true;
    
    void Start()
    {
        InitializeComponents();
        SetupEventConnections();
        InitializeCursor();
    }
    
    void Update()
    {
        if (!enableInput) return;
        
        HandlePlayerInput();
    }
    
    void FixedUpdate()
    {
        if (!enableMovement) return;
        
        HandleMovement();
    }
    
    /// <summary>
    /// Initialize all component dependencies
    /// Demonstrates Dependency Inversion Principle - depending on abstractions
    /// </summary>
    private void InitializeComponents()
    {
        // Get components via interfaces (not concrete classes)
        healthSystem = GetComponent<IHealthSystem>();
        scoreSystem = GetComponent<IScoreSystem>();
        movementController = GetComponent<IMovementController>();
        inputHandler = GetComponent<IInputHandler>();
        
        // Verify all required components exist
        ValidateComponents();
        
        Debug.Log("IntegratedPlayerController: All components initialized successfully");
    }
    
    /// <summary>
    /// Setup event connections between local and global systems
    /// Demonstrates Observer Pattern and Event-Driven Architecture
    /// </summary>
    private void SetupEventConnections()
    {
        // Connect local component events to global game events
        if (healthSystem != null)
        {
            healthSystem.OnHealthChanged += GameEvents.TriggerPlayerHealthChanged;
            healthSystem.OnDeath += GameEvents.TriggerPlayerDied;
        }
        
        if (scoreSystem != null)
        {
            scoreSystem.OnScoreChanged += GameEvents.TriggerPlayerScoreChanged;
        }
        
        if (movementController != null)
        {
            movementController.OnJumped += GameEvents.TriggerPlayerJumped;
            movementController.OnLanded += GameEvents.TriggerPlayerLanded;
        }
        
        // Connect input events to actions
        if (inputHandler != null)
        {
            inputHandler.OnJumpPressed += HandleJumpInput;
            inputHandler.OnRestartPressed += HandleRestartInput;
            inputHandler.OnPausePressed += HandlePauseInput;
        }
        
        Debug.Log("IntegratedPlayerController: Event connections established");
    }
    
    /// <summary>
    /// Handle player input through the input handler
    /// Demonstrates Single Responsibility - input handling separated from game logic
    /// </summary>
    private void HandlePlayerInput()
    {
        if (inputHandler == null) return;
        
        // Rotation input
        float mouseX = inputHandler.MouseX;
        if (Mathf.Abs(mouseX) > 0.01f)
        {
            movementController?.Rotate(mouseX);
        }
        
        // Note: Jump input is handled through events (HandleJumpInput)
        // This demonstrates event-driven input handling vs polling
    }
    
    /// <summary>
    /// Handle movement through the movement controller
    /// Demonstrates Interface Segregation - movement logic is separate
    /// </summary>
    private void HandleMovement()
    {
        if (inputHandler == null || movementController == null) return;
        
        Vector2 movementInput = inputHandler.MovementInput;
        if (movementInput.magnitude > 0.1f)
        {
            movementController.Move(movementInput);
        }
    }
    
    /// <summary>
    /// Initialize cursor settings
    /// </summary>
    private void InitializeCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    
    // ========================================
    // INPUT EVENT HANDLERS
    // ========================================
    
    private void HandleJumpInput()
    {
        movementController?.Jump();
        // Note: Jump events are automatically triggered by the movement controller
    }
    
    private void HandleRestartInput()
    {
        Debug.Log("Restart requested by player input");
        GameEvents.TriggerGameRestarted();
    }
    
    private void HandlePauseInput()
    {
        Debug.Log("Pause requested by player input");
        GameEvents.TriggerGamePaused(true);
    }
    
    // ========================================
    // UTILITY METHODS
    // ========================================
    
    /// <summary>
    /// Validate that all required components are present
    /// </summary>
    private void ValidateComponents()
    {
        if (healthSystem == null)
            Debug.LogError("IntegratedPlayerController: IHealthSystem component missing!");
        
        if (scoreSystem == null)
            Debug.LogError("IntegratedPlayerController: IScoreSystem component missing!");
        
        if (movementController == null)
            Debug.LogError("IntegratedPlayerController: IMovementController component missing!");
        
        if (inputHandler == null)
            Debug.LogError("IntegratedPlayerController: IInputHandler component missing!");
    }
    
    /// <summary>
    /// Enable or disable player movement
    /// </summary>
    /// <param name="enabled">Whether movement should be enabled</param>
    public void SetMovementEnabled(bool enabled)
    {
        enableMovement = enabled;
        movementController?.SetMovementEnabled(enabled);
        Debug.Log($"Player movement {(enabled ? "enabled" : "disabled")}");
    }
    
    /// <summary>
    /// Enable or disable player input
    /// </summary>
    /// <param name="enabled">Whether input should be enabled</param>
    public void SetInputEnabled(bool enabled)
    {
        enableInput = enabled;
        if (inputHandler != null)
            inputHandler.InputEnabled = enabled;
        Debug.Log($"Player input {(enabled ? "enabled" : "disabled")}");
    }
    
    /// <summary>
    /// Clean up event subscriptions when component is destroyed
    /// </summary>
    void OnDestroy()
    {
        // Disconnect local component events from global events
        if (healthSystem != null)
        {
            healthSystem.OnHealthChanged -= GameEvents.TriggerPlayerHealthChanged;
            healthSystem.OnDeath -= GameEvents.TriggerPlayerDied;
        }
        
        if (scoreSystem != null)
        {
            scoreSystem.OnScoreChanged -= GameEvents.TriggerPlayerScoreChanged;
        }
        
        if (movementController != null)
        {
            movementController.OnJumped -= GameEvents.TriggerPlayerJumped;
            movementController.OnLanded -= GameEvents.TriggerPlayerLanded;
        }
        
        if (inputHandler != null)
        {
            inputHandler.OnJumpPressed -= HandleJumpInput;
            inputHandler.OnRestartPressed -= HandleRestartInput;
            inputHandler.OnPausePressed -= HandlePauseInput;
        }
        
        Debug.Log("IntegratedPlayerController: Event cleanup completed");
    }
}
