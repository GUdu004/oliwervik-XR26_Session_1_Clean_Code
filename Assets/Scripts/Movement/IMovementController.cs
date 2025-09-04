using UnityEngine;

/// <summary>
/// Interface for movement control system
/// Follows Single Responsibility Principle - only handles movement-related functionality
/// </summary>
public interface IMovementController
{
    bool IsGrounded { get; }
    bool IsJumping { get; }
    
    void Move(Vector2 input);
    void Rotate(float mouseX);
    void Jump();
    void SetMovementEnabled(bool enabled);
    
    event System.Action OnJumped;
    event System.Action OnLanded;
}
