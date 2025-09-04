using UnityEngine;

/// <summary>
/// Centralized event system for game-wide communication
/// Implements Observer pattern and decouples dependencies between systems
/// Follows Open/Closed Principle - can extend events without modifying existing code
/// </summary>
public static class GameEvents
{
    // ========================================
    // PLAYER EVENTS
    // ========================================
    
    /// <summary>Triggered when player's health changes</summary>
    public static event System.Action<float> OnPlayerHealthChanged;
    
    /// <summary>Triggered when player dies</summary>
    public static event System.Action OnPlayerDied;
    
    /// <summary>Triggered when player's score changes</summary>
    public static event System.Action<int> OnPlayerScoreChanged;
    
    /// <summary>Triggered when player jumps</summary>
    public static event System.Action OnPlayerJumped;
    
    /// <summary>Triggered when player lands</summary>
    public static event System.Action OnPlayerLanded;
    
    /// <summary>Triggered when player is healed</summary>
    public static event System.Action<float> OnPlayerHealed;
    
    /// <summary>Triggered when player takes damage</summary>
    public static event System.Action<float> OnPlayerDamaged;
    
    // ========================================
    // GAME STATE EVENTS
    // ========================================
    
    /// <summary>Triggered when game starts</summary>
    public static event System.Action OnGameStarted;
    
    /// <summary>Triggered when game is won</summary>
    public static event System.Action OnGameWon;
    
    /// <summary>Triggered when game is over</summary>
    public static event System.Action OnGameOver;
    
    /// <summary>Triggered when game is restarted</summary>
    public static event System.Action OnGameRestarted;
    
    /// <summary>Triggered when game is paused</summary>
    public static event System.Action<bool> OnGamePaused;
    
    // ========================================
    // INTERACTION EVENTS
    // ========================================
    
    /// <summary>Triggered when a collectible is picked up</summary>
    public static event System.Action<int> OnCollectiblePickup;
    
    /// <summary>Triggered when an enemy is defeated</summary>
    public static event System.Action<float> OnEnemyDefeated;
    
    /// <summary>Triggered when player takes damage from specific source</summary>
    public static event System.Action<float, string> OnDamageTaken;
    
    // ========================================
    // UI EVENTS
    // ========================================
    
    /// <summary>Triggered when UI needs to show a message</summary>
    public static event System.Action<string> OnShowUIMessage;
    
    /// <summary>Triggered when UI needs to update timer</summary>
    public static event System.Action<float> OnTimerUpdated;
    
    // ========================================
    // TRIGGER METHODS (Player Events)
    // ========================================
    
    /// <summary>Trigger player health changed event</summary>
    /// <param name="health">New health value</param>
    public static void TriggerPlayerHealthChanged(float health)
    {
        OnPlayerHealthChanged?.Invoke(health);
        Debug.Log($"[GameEvents] Player health changed: {health}");
    }
    
    /// <summary>Trigger player died event</summary>
    public static void TriggerPlayerDied()
    {
        OnPlayerDied?.Invoke();
        Debug.Log("[GameEvents] Player died");
    }
    
    /// <summary>Trigger player score changed event</summary>
    /// <param name="score">New score value</param>
    public static void TriggerPlayerScoreChanged(int score)
    {
        OnPlayerScoreChanged?.Invoke(score);
        Debug.Log($"[GameEvents] Player score changed: {score}");
    }
    
    /// <summary>Trigger player jumped event</summary>
    public static void TriggerPlayerJumped()
    {
        OnPlayerJumped?.Invoke();
        Debug.Log("[GameEvents] Player jumped");
    }
    
    /// <summary>Trigger player landed event</summary>
    public static void TriggerPlayerLanded()
    {
        OnPlayerLanded?.Invoke();
        Debug.Log("[GameEvents] Player landed");
    }
    
    /// <summary>Trigger player healed event</summary>
    /// <param name="healAmount">Amount healed</param>
    public static void TriggerPlayerHealed(float healAmount)
    {
        OnPlayerHealed?.Invoke(healAmount);
        Debug.Log($"[GameEvents] Player healed: {healAmount}");
    }
    
    /// <summary>Trigger player damaged event</summary>
    /// <param name="damageAmount">Amount of damage taken</param>
    public static void TriggerPlayerDamaged(float damageAmount)
    {
        OnPlayerDamaged?.Invoke(damageAmount);
        Debug.Log($"[GameEvents] Player damaged: {damageAmount}");
    }
    
    // ========================================
    // TRIGGER METHODS (Game State Events)
    // ========================================
    
    /// <summary>Trigger game started event</summary>
    public static void TriggerGameStarted()
    {
        OnGameStarted?.Invoke();
        Debug.Log("[GameEvents] Game started");
    }
    
    /// <summary>Trigger game won event</summary>
    public static void TriggerGameWon()
    {
        OnGameWon?.Invoke();
        Debug.Log("[GameEvents] Game won!");
    }
    
    /// <summary>Trigger game over event</summary>
    public static void TriggerGameOver()
    {
        OnGameOver?.Invoke();
        Debug.Log("[GameEvents] Game over!");
    }
    
    /// <summary>Trigger game restarted event</summary>
    public static void TriggerGameRestarted()
    {
        OnGameRestarted?.Invoke();
        Debug.Log("[GameEvents] Game restarted");
    }
    
    /// <summary>Trigger game paused event</summary>
    /// <param name="isPaused">Whether game is paused or unpaused</param>
    public static void TriggerGamePaused(bool isPaused)
    {
        OnGamePaused?.Invoke(isPaused);
        Debug.Log($"[GameEvents] Game {(isPaused ? "paused" : "unpaused")}");
    }
    
    // ========================================
    // TRIGGER METHODS (Interaction Events)
    // ========================================
    
    /// <summary>Trigger collectible pickup event</summary>
    /// <param name="points">Points gained from collectible</param>
    public static void TriggerCollectiblePickup(int points)
    {
        OnCollectiblePickup?.Invoke(points);
        Debug.Log($"[GameEvents] Collectible picked up: +{points} points");
    }
    
    /// <summary>Trigger enemy defeated event</summary>
    /// <param name="scoreReward">Score reward for defeating enemy</param>
    public static void TriggerEnemyDefeated(float scoreReward)
    {
        OnEnemyDefeated?.Invoke(scoreReward);
        Debug.Log($"[GameEvents] Enemy defeated: +{scoreReward} score");
    }
    
    /// <summary>Trigger damage taken event</summary>
    /// <param name="damage">Amount of damage taken</param>
    /// <param name="source">Source of the damage</param>
    public static void TriggerDamageTaken(float damage, string source = "Unknown")
    {
        OnDamageTaken?.Invoke(damage, source);
        Debug.Log($"[GameEvents] Damage taken: {damage} from {source}");
    }
    
    // ========================================
    // TRIGGER METHODS (UI Events)
    // ========================================
    
    /// <summary>Trigger show UI message event</summary>
    /// <param name="message">Message to display</param>
    public static void TriggerShowUIMessage(string message)
    {
        OnShowUIMessage?.Invoke(message);
        Debug.Log($"[GameEvents] UI Message: {message}");
    }
    
    /// <summary>Trigger timer updated event</summary>
    /// <param name="time">Current game time</param>
    public static void TriggerTimerUpdated(float time)
    {
        OnTimerUpdated?.Invoke(time);
        // Don't log timer updates as they happen every frame
    }
    
    // ========================================
    // UTILITY METHODS
    // ========================================
    
    /// <summary>
    /// Clear all event subscriptions (useful for cleanup or testing)
    /// WARNING: Use with caution in production code
    /// </summary>
    public static void ClearAllEvents()
    {
        OnPlayerHealthChanged = null;
        OnPlayerDied = null;
        OnPlayerScoreChanged = null;
        OnPlayerJumped = null;
        OnPlayerLanded = null;
        OnGameWon = null;
        OnGameOver = null;
        OnGameRestarted = null;
        OnGamePaused = null;
        OnCollectiblePickup = null;
        OnEnemyDefeated = null;
        OnDamageTaken = null;
        OnShowUIMessage = null;
        OnTimerUpdated = null;
        
        Debug.Log("[GameEvents] All events cleared");
    }
    
    /// <summary>
    /// Get debug information about current event subscribers
    /// </summary>
    public static void LogEventSubscribers()
    {
        Debug.Log("=== GameEvents Subscribers ===");
        Debug.Log($"OnPlayerHealthChanged: {OnPlayerHealthChanged?.GetInvocationList().Length ?? 0} subscribers");
        Debug.Log($"OnPlayerDied: {OnPlayerDied?.GetInvocationList().Length ?? 0} subscribers");
        Debug.Log($"OnPlayerScoreChanged: {OnPlayerScoreChanged?.GetInvocationList().Length ?? 0} subscribers");
        Debug.Log($"OnGameWon: {OnGameWon?.GetInvocationList().Length ?? 0} subscribers");
        Debug.Log($"OnGameOver: {OnGameOver?.GetInvocationList().Length ?? 0} subscribers");
        // Add more as needed for debugging
    }
}
