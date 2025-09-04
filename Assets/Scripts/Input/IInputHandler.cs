using UnityEngine;

/// <summary>
/// Interface for input handling system
/// Follows Single Responsibility Principle - only handles input detection
/// Follows Interface Segregation Principle - small, focused interface
/// </summary>
public interface IInputHandler
{
    // Movement Input
    Vector2 MovementInput { get; }
    float MouseX { get; }
    float MouseY { get; }
    
    // Action Input
    bool JumpPressed { get; }
    bool JumpHeld { get; }
    bool JumpReleased { get; }
    
    // Game Control Input
    bool RestartPressed { get; }
    bool PausePressed { get; }
    bool InteractPressed { get; }
    
    // Input State
    bool InputEnabled { get; set; }
    
    // Events for input actions
    event System.Action OnJumpPressed;
    event System.Action OnRestartPressed;
    event System.Action OnPausePressed;
    event System.Action OnInteractPressed;
    
    // Debug and utility methods
    string GetInputDebugInfo();
}
