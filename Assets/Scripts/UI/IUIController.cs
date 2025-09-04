using UnityEngine;

/// <summary>
/// Base interface for all UI controllers
/// Ensures consistency and provides common functionality
/// Follows Interface Segregation Principle
/// </summary>
public interface IUIController
{
    /// <summary>
    /// Initialize the UI controller
    /// </summary>
    void Initialize();
    
    /// <summary>
    /// Subscribe to relevant events
    /// </summary>
    void SubscribeToEvents();
    
    /// <summary>
    /// Unsubscribe from events to prevent memory leaks
    /// </summary>
    void UnsubscribeFromEvents();
    
    /// <summary>
    /// Show or hide the UI element
    /// </summary>
    /// <param name="visible">Whether the UI should be visible</param>
    void SetVisible(bool visible);
}

/// <summary>
/// Interface for health-related UI functionality
/// Follows Interface Segregation Principle - only health-specific methods
/// </summary>
public interface IHealthUI : IUIController
{
    /// <summary>
    /// Current health value displayed
    /// </summary>
    float CurrentDisplayedHealth { get; }
    
    /// <summary>
    /// Maximum health value
    /// </summary>
    float MaxHealth { get; }
    
    /// <summary>
    /// Set the maximum health value
    /// </summary>
    /// <param name="maxHealth">Maximum health value</param>
    void SetMaxHealth(float maxHealth);
    
    /// <summary>
    /// Set the current health display
    /// </summary>
    /// <param name="health">Health value to display</param>
    void SetHealthDisplay(float health);
    
    /// <summary>
    /// Get the current health percentage (0-1)
    /// </summary>
    /// <returns>Health percentage</returns>
    float GetHealthPercentage();
}

/// <summary>
/// Interface for score-related UI functionality
/// Follows Interface Segregation Principle - only score-specific methods
/// </summary>
public interface IScoreUI : IUIController
{
    /// <summary>
    /// Current score value displayed
    /// </summary>
    int CurrentDisplayedScore { get; }
    
    /// <summary>
    /// Set the score display
    /// </summary>
    /// <param name="score">Score value to display</param>
    void SetScoreDisplay(int score);
    
    /// <summary>
    /// Set the score prefix text
    /// </summary>
    /// <param name="prefix">Prefix text</param>
    void SetScorePrefix(string prefix);
    
    /// <summary>
    /// Set the score suffix text
    /// </summary>
    /// <param name="suffix">Suffix text</param>
    void SetScoreSuffix(string suffix);
    
    /// <summary>
    /// Enable or disable score animation
    /// </summary>
    /// <param name="animate">Whether to animate score changes</param>
    void SetScoreAnimation(bool animate);
}

/// <summary>
/// Interface for game status UI functionality
/// Follows Interface Segregation Principle - only game status methods
/// </summary>
public interface IGameStatusUI : IUIController
{
    /// <summary>
    /// Current game time in seconds
    /// </summary>
    float GameTime { get; }
    
    /// <summary>
    /// Whether the timer is currently running
    /// </summary>
    bool TimerRunning { get; }
    
    /// <summary>
    /// Start the game timer
    /// </summary>
    void StartTimer();
    
    /// <summary>
    /// Stop the game timer
    /// </summary>
    void StopTimer();
    
    /// <summary>
    /// Reset the game timer
    /// </summary>
    void ResetTimer();
    
    /// <summary>
    /// Set custom status text
    /// </summary>
    /// <param name="message">Status message</param>
    void SetCustomStatusText(string message);
    
    /// <summary>
    /// Get formatted time string
    /// </summary>
    /// <returns>Formatted time</returns>
    string GetFormattedTime();
    
    /// <summary>
    /// Show or hide the timer
    /// </summary>
    /// <param name="show">Whether to show timer</param>
    void SetTimerVisible(bool show);
}
