using UnityEngine;

/// <summary>
/// Player movement component that implements IMovementController interface
/// Single Responsibility: Manages only movement-related physics and state
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour, IMovementController
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 10f;
    [SerializeField] private float rotationSpeed = 0.5f;
    
    [Header("Ground Detection")]
    [SerializeField] private LayerMask groundLayerMask = -1;
    [SerializeField] private float groundCheckDistance = 0.1f;
    
    private Rigidbody rb;
    private bool isGrounded;
    private bool isJumping;
    private bool movementEnabled = true;
    private float yaw;
    
    // Properties
    public bool IsGrounded => isGrounded;
    public bool IsJumping => isJumping;
    
    // Events
    public event System.Action OnJumped;
    public event System.Action OnLanded;
    
    void Start()
    {
        InitializeMovement();
    }
    
    void FixedUpdate()
    {
        CheckGroundState();
    }
    
    /// <summary>
    /// Initialize movement system
    /// </summary>
    private void InitializeMovement()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogError("PlayerMovement requires a Rigidbody component!");
            return;
        }
        
        // Freeze rotation to prevent physics flipping the player
        rb.freezeRotation = true;
        
        Debug.Log("PlayerMovement system initialized");
    }
    
    /// <summary>
    /// Move the player based on input
    /// </summary>
    /// <param name="input">Movement input (x = horizontal, y = vertical)</param>
    public void Move(Vector2 input)
    {
        if (!movementEnabled || rb == null) return;
        
        Vector3 direction = transform.forward * input.y + transform.right * input.x;
        Vector3 velocity = direction.normalized * moveSpeed;
        Vector3 newPos = rb.position + velocity * Time.fixedDeltaTime;
        
        rb.MovePosition(newPos);
    }
    
    /// <summary>
    /// Rotate the player based on mouse input
    /// </summary>
    /// <param name="mouseX">Mouse X-axis input</param>
    public void Rotate(float mouseX)
    {
        if (!movementEnabled) return;
        
        yaw += mouseX * rotationSpeed;
        transform.rotation = Quaternion.Euler(0f, yaw, 0f);
    }
    
    /// <summary>
    /// Make the player jump if grounded
    /// </summary>
    public void Jump()
    {
        if (!movementEnabled || !isGrounded || isJumping || rb == null) return;
        
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        isGrounded = false;
        isJumping = true;
        
        OnJumped?.Invoke();
        GameEvents.TriggerPlayerJumped();
        Debug.Log("Player jumped!");
    }
    
    /// <summary>
    /// Enable or disable movement
    /// </summary>
    /// <param name="enabled">Whether movement should be enabled</param>
    public void SetMovementEnabled(bool enabled)
    {
        movementEnabled = enabled;
        Debug.Log($"Movement {(enabled ? "enabled" : "disabled")}");
    }
    
    /// <summary>
    /// Check if player is grounded using collision detection
    /// </summary>
    private void CheckGroundState()
    {
        if (rb == null) return;
        
        // Check if player is touching ground using a short raycast
        bool wasGrounded = isGrounded;
        isGrounded = Physics.Raycast(transform.position, Vector3.down, 
                                   transform.localScale.y * 0.5f + groundCheckDistance, 
                                   groundLayerMask);
        
        // Handle landing
        if (isGrounded && !wasGrounded && isJumping)
        {
            isJumping = false;
            OnLanded?.Invoke();
            GameEvents.TriggerPlayerLanded();
            Debug.Log("Player landed");
        }
    }
    
    /// <summary>
    /// Handle collision events for ground detection
    /// </summary>
    /// <param name="collision">Collision data</param>
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            if (!isGrounded && isJumping)
            {
                isGrounded = true;
                isJumping = false;
                OnLanded?.Invoke();
                Debug.Log("Player landed on ground");
            }
            else if (!isGrounded)
            {
                isGrounded = true;
            }
        }
    }
    
    /// <summary>
    /// Handle collision exit events
    /// </summary>
    /// <param name="collision">Collision data</param>
    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            // Small delay to prevent false positives when moving over edges
            Invoke(nameof(DelayedGroundCheck), 0.1f);
        }
    }
    
    /// <summary>
    /// Delayed ground check to handle edge cases
    /// </summary>
    private void DelayedGroundCheck()
    {
        CheckGroundState();
    }
    
    /// <summary>
    /// Draw debug information in scene view
    /// </summary>
    void OnDrawGizmosSelected()
    {
        // Draw ground check ray
        Gizmos.color = isGrounded ? Color.green : Color.red;
        Vector3 rayStart = transform.position;
        Vector3 rayEnd = rayStart + Vector3.down * (transform.localScale.y * 0.5f + groundCheckDistance);
        Gizmos.DrawLine(rayStart, rayEnd);
    }
}
