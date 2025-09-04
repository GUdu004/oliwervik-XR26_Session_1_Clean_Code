using UnityEngine;

/// <summary>
/// Test script to verify Phase 1 implementation
/// This script demonstrates how the new SOLID-compliant components work together
/// </summary>
public class Phase1TestController : MonoBehaviour
{
    [Header("Testing Controls")]
    [SerializeField] private bool enableTesting = true;
    [SerializeField] private KeyCode testDamageKey = KeyCode.H;
    [SerializeField] private KeyCode testHealKey = KeyCode.J;
    [SerializeField] private KeyCode testScoreKey = KeyCode.K;
    [SerializeField] private KeyCode testMovementToggle = KeyCode.M;
    
    private IHealthSystem healthSystem;
    private IScoreSystem scoreSystem;
    private IMovementController movementController;
    
    void Start()
    {
        // Get the new components
        healthSystem = GetComponent<IHealthSystem>();
        scoreSystem = GetComponent<IScoreSystem>();
        movementController = GetComponent<IMovementController>();
        
        // Subscribe to events to test the event system
        if (healthSystem != null)
        {
            healthSystem.OnHealthChanged += OnHealthChanged;
            healthSystem.OnDeath += OnPlayerDeath;
        }
        
        if (scoreSystem != null)
        {
            scoreSystem.OnScoreChanged += OnScoreChanged;
        }
        
        if (movementController != null)
        {
            movementController.OnJumped += OnPlayerJumped;
            movementController.OnLanded += OnPlayerLanded;
        }
        
        Debug.Log("Phase 1 Test Controller initialized - Use H/J/K/M keys to test systems");
    }
    
    void Update()
    {
        if (!enableTesting) return;
        
        // Test health system
        if (Input.GetKeyDown(testDamageKey))
        {
            healthSystem?.TakeDamage(5f);
            Debug.Log("Applied 5 damage for testing");
        }
        
        if (Input.GetKeyDown(testHealKey))
        {
            healthSystem?.Heal(5f);
            Debug.Log("Applied 5 healing for testing");
        }
        
        // Test score system
        if (Input.GetKeyDown(testScoreKey))
        {
            scoreSystem?.AddScore(10);
            Debug.Log("Added 10 score for testing");
        }
        
        // Test movement toggle
        if (Input.GetKeyDown(testMovementToggle))
        {
            if (movementController != null)
            {
                // This is just for testing - in real implementation this would be handled differently
                bool currentState = movementController.IsGrounded; // Using this as a proxy for testing
                Debug.Log("Movement toggle test - check console for movement state");
            }
        }
        
        // Basic movement and rotation (this will be replaced by proper input handler in Phase 2)
        if (movementController != null)
        {
            Vector2 moveInput = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
            float mouseX = Input.GetAxis("Mouse X");
            
            movementController.Move(moveInput);
            movementController.Rotate(mouseX);
            
            if (Input.GetButtonDown("Jump"))
            {
                movementController.Jump();
            }
        }
    }
    
    // Event handlers to test the event system
    private void OnHealthChanged(float newHealth)
    {
        Debug.Log($"Health changed event received: {newHealth}");
    }
    
    private void OnPlayerDeath()
    {
        Debug.Log("Player death event received!");
    }
    
    private void OnScoreChanged(int newScore)
    {
        Debug.Log($"Score changed event received: {newScore}");
    }
    
    private void OnPlayerJumped()
    {
        Debug.Log("Player jumped event received!");
    }
    
    private void OnPlayerLanded()
    {
        Debug.Log("Player landed event received!");
    }
    
    void OnDestroy()
    {
        // Clean up event subscriptions
        if (healthSystem != null)
        {
            healthSystem.OnHealthChanged -= OnHealthChanged;
            healthSystem.OnDeath -= OnPlayerDeath;
        }
        
        if (scoreSystem != null)
        {
            scoreSystem.OnScoreChanged -= OnScoreChanged;
        }
        
        if (movementController != null)
        {
            movementController.OnJumped -= OnPlayerJumped;
            movementController.OnLanded -= OnPlayerLanded;
        }
    }
}
