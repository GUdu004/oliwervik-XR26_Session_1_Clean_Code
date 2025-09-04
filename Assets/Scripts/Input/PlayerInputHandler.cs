using UnityEngine;

/// <summary>
/// Player input handler that implements IInputHandler interface
/// Single Responsibility: Manages only input detection and processing
/// Decouples input detection from input handling/response
/// </summary>
public class PlayerInputHandler : MonoBehaviour, IInputHandler
{
    [Header("Input Settings")]
    [SerializeField] private bool enableInput = true;
    [SerializeField] private float mouseSensitivity = 1f;
    
    [Header("Input Keys")]
    [SerializeField] private KeyCode jumpKey = KeyCode.Space;
    [SerializeField] private KeyCode restartKey = KeyCode.R;
    [SerializeField] private KeyCode pauseKey = KeyCode.Escape;
    [SerializeField] private KeyCode interactKey = KeyCode.E;
    
    // Input state
    private bool inputEnabled = true;
    
    // Properties
    public Vector2 MovementInput => GetMovementInput();
    public float MouseX => GetMouseX();
    public float MouseY => GetMouseY();
    
    public bool JumpPressed => inputEnabled && Input.GetKeyDown(jumpKey);
    public bool JumpHeld => inputEnabled && Input.GetKey(jumpKey);
    public bool JumpReleased => inputEnabled && Input.GetKeyUp(jumpKey);
    
    public bool RestartPressed => inputEnabled && Input.GetKeyDown(restartKey);
    public bool PausePressed => inputEnabled && Input.GetKeyDown(pauseKey);
    public bool InteractPressed => inputEnabled && Input.GetKeyDown(interactKey);
    
    public bool InputEnabled 
    { 
        get => inputEnabled; 
        set => inputEnabled = value; 
    }
    
    // Events
    public event System.Action OnJumpPressed;
    public event System.Action OnRestartPressed;
    public event System.Action OnPausePressed;
    public event System.Action OnInteractPressed;
    
    void Start()
    {
        InitializeInput();
    }
    
    void Update()
    {
        if (!enableInput) return;
        
        ProcessInputEvents();
    }
    
    /// <summary>
    /// Initialize input system
    /// </summary>
    private void InitializeInput()
    {
        inputEnabled = enableInput;
        Debug.Log("PlayerInputHandler initialized");
    }
    
    /// <summary>
    /// Process input events and trigger corresponding events
    /// </summary>
    private void ProcessInputEvents()
    {
        if (!inputEnabled) return;
        
        // Check for action inputs and fire events
        if (Input.GetKeyDown(jumpKey))
        {
            OnJumpPressed?.Invoke();
        }
        
        if (Input.GetKeyDown(restartKey))
        {
            OnRestartPressed?.Invoke();
        }
        
        if (Input.GetKeyDown(pauseKey))
        {
            OnPausePressed?.Invoke();
        }
        
        if (Input.GetKeyDown(interactKey))
        {
            OnInteractPressed?.Invoke();
        }
    }
    
    /// <summary>
    /// Get normalized movement input
    /// </summary>
    /// <returns>Movement vector (x = horizontal, y = vertical)</returns>
    private Vector2 GetMovementInput()
    {
        if (!inputEnabled) return Vector2.zero;
        
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        
        return new Vector2(horizontal, vertical);
    }
    
    /// <summary>
    /// Get mouse X input with sensitivity applied
    /// </summary>
    /// <returns>Mouse X movement</returns>
    private float GetMouseX()
    {
        if (!inputEnabled) return 0f;
        
        return Input.GetAxis("Mouse X") * mouseSensitivity;
    }
    
    /// <summary>
    /// Get mouse Y input with sensitivity applied
    /// </summary>
    /// <returns>Mouse Y movement</returns>
    private float GetMouseY()
    {
        if (!inputEnabled) return 0f;
        
        return Input.GetAxis("Mouse Y") * mouseSensitivity;
    }
    
    /// <summary>
    /// Enable or disable input processing
    /// </summary>
    /// <param name="enabled">Whether input should be enabled</param>
    public void SetInputEnabled(bool enabled)
    {
        inputEnabled = enabled;
        Debug.Log($"Input {(enabled ? "enabled" : "disabled")}");
    }
    
    /// <summary>
    /// Set mouse sensitivity
    /// </summary>
    /// <param name="sensitivity">New mouse sensitivity value</param>
    public void SetMouseSensitivity(float sensitivity)
    {
        mouseSensitivity = Mathf.Max(0.1f, sensitivity);
        Debug.Log($"Mouse sensitivity set to {mouseSensitivity}");
    }
    
    /// <summary>
    /// Check if any movement input is active
    /// </summary>
    /// <returns>True if any movement input is pressed</returns>
    public bool HasMovementInput()
    {
        return MovementInput.magnitude > 0.1f;
    }
    
    /// <summary>
    /// Check if any mouse input is active
    /// </summary>
    /// <returns>True if any mouse movement is detected</returns>
    public bool HasMouseInput()
    {
        return Mathf.Abs(MouseX) > 0.01f || Mathf.Abs(MouseY) > 0.01f;
    }
    
    /// <summary>
    /// Get debug information about current input state
    /// </summary>
    /// <returns>Debug string with input information</returns>
    public string GetInputDebugInfo()
    {
        return $"Movement: {MovementInput}, Mouse: ({MouseX:F2}, {MouseY:F2}), " +
               $"Jump: {JumpHeld}, Input Enabled: {inputEnabled}";
    }
    
    void OnDisable()
    {
        // Clear events when component is disabled
        OnJumpPressed = null;
        OnRestartPressed = null;
        OnPausePressed = null;
        OnInteractPressed = null;
    }
}
