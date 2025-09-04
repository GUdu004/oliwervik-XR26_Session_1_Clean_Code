using UnityEngine;

/// <summary>
/// Input debugging and troubleshooting helper
/// Use this script to diagnose input issues and verify input system functionality
/// </summary>
public class InputDebugger : MonoBehaviour
{
    [Header("Debug Settings")]
    [SerializeField] private bool enableInputDebugging = true;
    [SerializeField] private bool showInputInConsole = true;
    [SerializeField] private bool showInputOnScreen = true;
    [SerializeField] private float debugUpdateInterval = 0.1f;
    
    [Header("Component References")]
    [SerializeField] private PlayerController playerController;
    [SerializeField] private PlayerInputHandler inputHandler;
    
    // Debug state
    private float lastDebugTime;
    private bool hasPlayerController;
    private bool hasInputHandler;
    
    // Input state tracking
    private Vector2 lastMovementInput;
    private float lastMouseX;
    private bool lastJumpPressed;
    private bool lastRestartPressed;
    private bool lastPausePressed;
    private bool lastInteractPressed;
    
    void Start()
    {
        InitializeDebugger();
    }
    
    void Update()
    {
        if (!enableInputDebugging) return;
        
        DebugInputState();
        
        // Periodic detailed debug output
        if (Time.time - lastDebugTime > debugUpdateInterval)
        {
            if (showInputInConsole)
                LogDetailedInputState();
            lastDebugTime = Time.time;
        }
    }
    
    /// <summary>
    /// Initialize the input debugger
    /// </summary>
    private void InitializeDebugger()
    {
        // Find components if not assigned
        if (playerController == null)
            playerController = FindFirstObjectByType<PlayerController>();
            
        if (inputHandler == null)
            inputHandler = FindFirstObjectByType<PlayerInputHandler>();
        
        hasPlayerController = playerController != null;
        hasInputHandler = inputHandler != null;
        
        Debug.Log("=== INPUT DEBUGGER INITIALIZED ===");
        Debug.Log($"PlayerController found: {hasPlayerController}");
        Debug.Log($"InputHandler found: {hasInputHandler}");
        
        if (!hasPlayerController)
            Debug.LogError("InputDebugger: No PlayerController found in scene!");
        if (!hasInputHandler)
            Debug.LogError("InputDebugger: No PlayerInputHandler found in scene!");
            
        // Test basic Unity input
        TestUnityInputSystem();
    }
    
    /// <summary>
    /// Test Unity's basic input system
    /// </summary>
    private void TestUnityInputSystem()
    {
        Debug.Log("=== TESTING UNITY INPUT SYSTEM ===");
        
        // Test if Unity can detect basic inputs
        bool canDetectKeys = Input.inputString != null;
        bool canDetectMouse = Input.mousePosition != Vector3.zero;
        
        Debug.Log($"Unity Input System Active: {canDetectKeys}");
        Debug.Log($"Mouse Position Available: {canDetectMouse}");
        Debug.Log($"Current Mouse Position: {Input.mousePosition}");
        
        // Test specific input axes
        bool hasHorizontalAxis = true;
        bool hasVerticalAxis = true;
        bool hasMouseXAxis = true;
        
        try
        {
            float h = Input.GetAxis("Horizontal");
            float v = Input.GetAxis("Vertical");
            float mx = Input.GetAxis("Mouse X");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Input axis error: {e.Message}");
        }
        
        Debug.Log($"Input Axes Available - H: {hasHorizontalAxis}, V: {hasVerticalAxis}, MouseX: {hasMouseXAxis}");
    }
    
    /// <summary>
    /// Debug current input state
    /// </summary>
    private void DebugInputState()
    {
        // Test raw Unity input
        TestRawInput();
        
        // Test through InputHandler if available
        if (hasInputHandler)
            TestInputHandler();
        
        // Test PlayerController state if available
        if (hasPlayerController)
            TestPlayerController();
    }
    
    /// <summary>
    /// Test raw Unity input
    /// </summary>
    private void TestRawInput()
    {
        // Movement keys
        bool wPressed = Input.GetKey(KeyCode.W);
        bool aPressed = Input.GetKey(KeyCode.A);
        bool sPressed = Input.GetKey(KeyCode.S);
        bool dPressed = Input.GetKey(KeyCode.D);
        
        // Action keys
        bool spacePressed = Input.GetKeyDown(KeyCode.Space);
        bool rPressed = Input.GetKeyDown(KeyCode.R);
        bool escapePressed = Input.GetKeyDown(KeyCode.Escape);
        bool ePressed = Input.GetKeyDown(KeyCode.E);
        
        // Mouse
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");
        
        // Log if any input detected
        if (wPressed || aPressed || sPressed || dPressed || spacePressed || rPressed || escapePressed || ePressed || Mathf.Abs(mouseX) > 0.01f)
        {
            Debug.Log($"[RAW INPUT] Movement: W={wPressed} A={aPressed} S={sPressed} D={dPressed}, " +
                     $"Actions: Space={spacePressed} R={rPressed} Esc={escapePressed} E={ePressed}, " +
                     $"Mouse: X={mouseX:F3} Y={mouseY:F3}");
        }
    }
    
    /// <summary>
    /// Test input through the InputHandler
    /// </summary>
    private void TestInputHandler()
    {
        Vector2 movement = inputHandler.MovementInput;
        float mouseX = inputHandler.MouseX;
        bool jumpPressed = inputHandler.JumpPressed;
        bool restartPressed = inputHandler.RestartPressed;
        bool pausePressed = inputHandler.PausePressed;
        bool interactPressed = inputHandler.InteractPressed;
        bool inputEnabled = inputHandler.InputEnabled;
        
        // Check if input state changed
        bool inputChanged = movement != lastMovementInput || 
                           Mathf.Abs(mouseX - lastMouseX) > 0.01f ||
                           jumpPressed != lastJumpPressed ||
                           restartPressed != lastRestartPressed ||
                           pausePressed != lastPausePressed ||
                           interactPressed != lastInteractPressed;
        
        if (inputChanged)
        {
            Debug.Log($"[INPUT HANDLER] Enabled: {inputEnabled}, " +
                     $"Movement: {movement}, MouseX: {mouseX:F3}, " +
                     $"Jump: {jumpPressed}, Restart: {restartPressed}, Pause: {pausePressed}, Interact: {interactPressed}");
        }
        
        // Store current state
        lastMovementInput = movement;
        lastMouseX = mouseX;
        lastJumpPressed = jumpPressed;
        lastRestartPressed = restartPressed;
        lastPausePressed = pausePressed;
        lastInteractPressed = interactPressed;
    }
    
    /// <summary>
    /// Test PlayerController state
    /// </summary>
    private void TestPlayerController()
    {
        bool isActive = playerController.IsActive;
        bool isInitialized = playerController.IsInitialized;
        bool isAlive = playerController.IsAlive();
        
        Debug.Log($"[PLAYER CONTROLLER] Active: {isActive}, Initialized: {isInitialized}, Alive: {isAlive}");
    }
    
    /// <summary>
    /// Log detailed input state information
    /// </summary>
    private void LogDetailedInputState()
    {
        if (!hasInputHandler) return;
        
        string debugInfo = inputHandler.GetInputDebugInfo();
        Debug.Log($"[INPUT DEBUG] {debugInfo}");
    }
    
    /// <summary>
    /// Manual input test methods
    /// </summary>
    [ContextMenu("Test Space Key")]
    public void TestSpaceKey()
    {
        bool spacePressed = Input.GetKey(KeyCode.Space);
        Debug.Log($"Space Key Test: {spacePressed}");
    }
    
    [ContextMenu("Test Movement Keys")]
    public void TestMovementKeys()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        Debug.Log($"Movement Test - Horizontal: {horizontal}, Vertical: {vertical}");
    }
    
    [ContextMenu("Test Mouse Input")]
    public void TestMouseInput()
    {
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");
        Vector3 mousePos = Input.mousePosition;
        Debug.Log($"Mouse Test - X: {mouseX}, Y: {mouseY}, Position: {mousePos}");
    }
    
    [ContextMenu("Test Input Handler State")]
    public void TestInputHandlerState()
    {
        if (hasInputHandler)
        {
            Debug.Log($"InputHandler State: {inputHandler.GetInputDebugInfo()}");
        }
        else
        {
            Debug.LogError("No InputHandler available for testing!");
        }
    }
    
    [ContextMenu("Force Enable All Input")]
    public void ForceEnableAllInput()
    {
        if (hasInputHandler)
        {
            inputHandler.InputEnabled = true;
            Debug.Log("Forced InputHandler to enabled state");
        }
        
        if (hasPlayerController)
        {
            playerController.SetInputEnabled(true);
            Debug.Log("Forced PlayerController input to enabled state");
        }
    }
    
    /// <summary>
    /// Display input state on screen
    /// </summary>
    void OnGUI()
    {
        if (!showInputOnScreen || !enableInputDebugging) return;
        
        GUILayout.BeginArea(new Rect(10, 10, 400, 300));
        GUILayout.Label("=== INPUT DEBUGGER ===", GUI.skin.box);
        
        // Component status
        GUILayout.Label($"PlayerController: {(hasPlayerController ? "✅" : "❌")}");
        GUILayout.Label($"InputHandler: {(hasInputHandler ? "✅" : "❌")}");
        
        if (hasInputHandler)
        {
            GUILayout.Label($"Input Enabled: {inputHandler.InputEnabled}");
            GUILayout.Label($"Movement: {inputHandler.MovementInput}");
            GUILayout.Label($"Mouse X: {inputHandler.MouseX:F3}");
            GUILayout.Label($"Jump: {inputHandler.JumpPressed}");
            GUILayout.Label($"Restart: {inputHandler.RestartPressed}");
            GUILayout.Label($"Pause: {inputHandler.PausePressed}");
            GUILayout.Label($"Interact: {inputHandler.InteractPressed}");
        }
        
        // Raw input state
        GUILayout.Space(10);
        GUILayout.Label("RAW INPUT:");
        GUILayout.Label($"WASD: {Input.GetKey(KeyCode.W)}{Input.GetKey(KeyCode.A)}{Input.GetKey(KeyCode.S)}{Input.GetKey(KeyCode.D)}");
        GUILayout.Label($"Space: {Input.GetKey(KeyCode.Space)}");
        GUILayout.Label($"Mouse X: {Input.GetAxis("Mouse X"):F3}");
        
        // Quick test buttons
        GUILayout.Space(10);
        if (GUILayout.Button("Test Space Key"))
            TestSpaceKey();
        if (GUILayout.Button("Force Enable Input"))
            ForceEnableAllInput();
        
        GUILayout.EndArea();
    }
}
