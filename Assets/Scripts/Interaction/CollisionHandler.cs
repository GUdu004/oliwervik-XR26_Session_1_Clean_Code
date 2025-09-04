using UnityEngine;

/// <summary>
/// Single Responsibility: Handle collision interactions between player and game objects
/// Separates collision logic from player movement and game logic
/// Demonstrates Open/Closed Principle - easy to extend with new collision types
/// </summary>
public class CollisionHandler : MonoBehaviour
{
    [Header("Component Dependencies")]
    private IHealthSystem healthSystem;
    private IScoreSystem scoreSystem;
    private IMovementController movementController;
    
    [Header("Collision Configuration")]
    [SerializeField] private bool showDebugLogs = true;
    [SerializeField] private LayerMask interactableLayers = -1;
    
    [Header("Default Values (TODO: Move to ScriptableObjects)")]
    [SerializeField] private int defaultCollectiblePoints = 10;
    [SerializeField] private float defaultEnemyDamage = 10f;
    
    /// <summary>
    /// Event triggered when player collects an item
    /// </summary>
    public event System.Action<GameObject, int> OnItemCollected;
    
    /// <summary>
    /// Event triggered when player takes damage from collision
    /// </summary>
    public event System.Action<GameObject, float> OnDamageTaken;
    
    /// <summary>
    /// Event triggered when player lands on ground
    /// </summary>
    public event System.Action OnPlayerLanded;
    
    void Start()
    {
        InitializeComponents();
        
        if (showDebugLogs)
            Debug.Log("CollisionHandler: Initialized and ready for interactions");
    }
    
    /// <summary>
    /// Initialize component dependencies
    /// </summary>
    private void InitializeComponents()
    {
        // Get required components from the same GameObject
        healthSystem = GetComponent<IHealthSystem>();
        scoreSystem = GetComponent<IScoreSystem>();
        movementController = GetComponent<IMovementController>();
        
        // Validate dependencies
        ValidateComponents();
    }
    
    /// <summary>
    /// Validate that all required components are present
    /// </summary>
    private void ValidateComponents()
    {
        if (healthSystem == null)
            Debug.LogWarning("CollisionHandler: IHealthSystem component not found! Health-related collisions will be ignored.");
        
        if (scoreSystem == null)
            Debug.LogWarning("CollisionHandler: IScoreSystem component not found! Score-related collisions will be ignored.");
        
        if (movementController == null)
            Debug.LogWarning("CollisionHandler: IMovementController component not found! Movement-related collisions will be ignored.");
    }
    
    /// <summary>
    /// Handle collision enter events
    /// </summary>
    /// <param name="collision">Collision information</param>
    void OnCollisionEnter(Collision collision)
    {
        GameObject collidedObject = collision.gameObject;
        
        // Check if object is on interactable layers
        if (!IsInteractable(collidedObject))
            return;
        
        // Handle different collision types
        switch (collidedObject.tag)
        {
            case "Ground":
                HandleGroundCollision(collidedObject);
                break;
                
            case "Collectible":
                HandleCollectibleCollision(collidedObject);
                break;
                
            case "Enemy":
                HandleEnemyCollision(collidedObject);
                break;
                
            case "PowerUp":
                HandlePowerUpCollision(collidedObject);
                break;
                
            case "Hazard":
                HandleHazardCollision(collidedObject);
                break;
                
            default:
                HandleUnknownCollision(collidedObject);
                break;
        }
    }
    
    /// <summary>
    /// Check if object is on interactable layers
    /// </summary>
    /// <param name="obj">GameObject to check</param>
    /// <returns>True if interactable</returns>
    private bool IsInteractable(GameObject obj)
    {
        return ((1 << obj.layer) & interactableLayers) != 0;
    }
    
    /// <summary>
    /// Handle ground collision (landing)
    /// </summary>
    /// <param name="ground">Ground object</param>
    private void HandleGroundCollision(GameObject ground)
    {
        if (movementController != null)
        {
            // Update movement controller ground state if it has the capability
            // Note: This depends on the movement controller implementation
            if (showDebugLogs)
                Debug.Log($"CollisionHandler: Player landed on {ground.name}");
            
            // Trigger events
            OnPlayerLanded?.Invoke();
            GameEvents.TriggerPlayerLanded();
        }
    }
    
    /// <summary>
    /// Handle collectible collision
    /// </summary>
    /// <param name="collectible">Collectible object</param>
    private void HandleCollectibleCollision(GameObject collectible)
    {
        if (scoreSystem == null) return;
        
        // Get points value from collectible (using interface if available)
        int points = GetCollectibleValue(collectible);
        
        // Add points to score
        scoreSystem.AddScore(points);
        
        // Trigger events
        OnItemCollected?.Invoke(collectible, points);
        GameEvents.TriggerCollectiblePickup(points);
        
        // Destroy collectible
        DestroyCollectedItem(collectible);
        
        if (showDebugLogs)
            Debug.Log($"CollisionHandler: Collected {collectible.name} for {points} points");
    }
    
    /// <summary>
    /// Handle enemy collision
    /// </summary>
    /// <param name="enemy">Enemy object</param>
    private void HandleEnemyCollision(GameObject enemy)
    {
        if (healthSystem == null) return;
        
        // Get damage value from enemy (using interface if available)
        float damage = GetEnemyDamage(enemy);
        
        // Apply damage
        healthSystem.TakeDamage(damage);
        
        // Trigger events
        OnDamageTaken?.Invoke(enemy, damage);
        
        // Destroy enemy (simple implementation)
        DestroyEnemy(enemy);
        
        if (showDebugLogs)
            Debug.Log($"CollisionHandler: Hit by {enemy.name} for {damage} damage");
    }
    
    /// <summary>
    /// Handle power-up collision
    /// </summary>
    /// <param name="powerUp">Power-up object</param>
    private void HandlePowerUpCollision(GameObject powerUp)
    {
        if (healthSystem == null) return;
        
        // Get heal amount from power-up
        float healAmount = GetPowerUpValue(powerUp);
        
        // Apply healing
        healthSystem.Heal(healAmount);
        
        // Trigger events
        GameEvents.TriggerPlayerHealed(healAmount);
        
        // Destroy power-up
        DestroyCollectedItem(powerUp);
        
        if (showDebugLogs)
            Debug.Log($"CollisionHandler: Used power-up {powerUp.name} for {healAmount} health");
    }
    
    /// <summary>
    /// Handle hazard collision
    /// </summary>
    /// <param name="hazard">Hazard object</param>
    private void HandleHazardCollision(GameObject hazard)
    {
        if (healthSystem == null) return;
        
        // Get damage from hazard
        float damage = GetHazardDamage(hazard);
        
        // Apply damage
        healthSystem.TakeDamage(damage);
        
        // Trigger events
        OnDamageTaken?.Invoke(hazard, damage);
        
        if (showDebugLogs)
            Debug.Log($"CollisionHandler: Hit hazard {hazard.name} for {damage} damage");
    }
    
    /// <summary>
    /// Handle unknown collision types
    /// </summary>
    /// <param name="unknown">Unknown object</param>
    private void HandleUnknownCollision(GameObject unknown)
    {
        if (showDebugLogs)
            Debug.Log($"CollisionHandler: Unknown collision with {unknown.name} (Tag: {unknown.tag})");
    }
    
    /// <summary>
    /// Get collectible point value
    /// </summary>
    /// <param name="collectible">Collectible object</param>
    /// <returns>Point value</returns>
    private int GetCollectibleValue(GameObject collectible)
    {
        // Try to get value from component first
        var collectibleComponent = collectible.GetComponent<ICollectible>();
        if (collectibleComponent != null)
        {
            return collectibleComponent.PointValue;
        }
        
        // Fall back to default value
        return defaultCollectiblePoints;
    }
    
    /// <summary>
    /// Get enemy damage value
    /// </summary>
    /// <param name="enemy">Enemy object</param>
    /// <returns>Damage value</returns>
    private float GetEnemyDamage(GameObject enemy)
    {
        // Try to get damage from component first
        var enemyComponent = enemy.GetComponent<IEnemy>();
        if (enemyComponent != null)
        {
            return enemyComponent.Damage;
        }
        
        // Fall back to default value
        return defaultEnemyDamage;
    }
    
    /// <summary>
    /// Get power-up heal value
    /// </summary>
    /// <param name="powerUp">Power-up object</param>
    /// <returns>Heal amount</returns>
    private float GetPowerUpValue(GameObject powerUp)
    {
        // Try to get value from component first
        var powerUpComponent = powerUp.GetComponent<IPowerUp>();
        if (powerUpComponent != null)
        {
            return powerUpComponent.HealAmount;
        }
        
        // Fall back to default value
        return 10f; // Default heal amount
    }
    
    /// <summary>
    /// Get hazard damage value
    /// </summary>
    /// <param name="hazard">Hazard object</param>
    /// <returns>Damage value</returns>
    private float GetHazardDamage(GameObject hazard)
    {
        // Try to get damage from component first
        var hazardComponent = hazard.GetComponent<IHazard>();
        if (hazardComponent != null)
        {
            return hazardComponent.Damage;
        }
        
        // Fall back to default value
        return 5f; // Default hazard damage
    }
    
    /// <summary>
    /// Destroy collected item with optional effects
    /// </summary>
    /// <param name="item">Item to destroy</param>
    private void DestroyCollectedItem(GameObject item)
    {
        // Could add particle effects, sound, etc. here
        Destroy(item);
    }
    
    /// <summary>
    /// Destroy enemy with optional effects
    /// </summary>
    /// <param name="enemy">Enemy to destroy</param>
    private void DestroyEnemy(GameObject enemy)
    {
        // Could add death effects, sound, scoring, etc. here
        Destroy(enemy);
    }
    
    /// <summary>
    /// Set default collectible points value
    /// </summary>
    /// <param name="points">Points value</param>
    public void SetDefaultCollectiblePoints(int points)
    {
        defaultCollectiblePoints = points;
    }
    
    /// <summary>
    /// Set default enemy damage value
    /// </summary>
    /// <param name="damage">Damage value</param>
    public void SetDefaultEnemyDamage(float damage)
    {
        defaultEnemyDamage = damage;
    }
    
    /// <summary>
    /// Enable or disable debug logging
    /// </summary>
    /// <param name="enable">Whether to enable debug logs</param>
    public void SetDebugLogging(bool enable)
    {
        showDebugLogs = enable;
    }
}

// ========================================
// OPTIONAL INTERFACES FOR EXTENSIBILITY
// ========================================

/// <summary>
/// Interface for collectible objects
/// </summary>
public interface ICollectible
{
    int PointValue { get; }
    void OnCollected();
}

/// <summary>
/// Interface for enemy objects
/// </summary>
public interface IEnemy
{
    float Damage { get; }
    void OnDefeated();
}

/// <summary>
/// Interface for power-up objects
/// </summary>
public interface IPowerUp
{
    float HealAmount { get; }
    void OnUsed();
}

/// <summary>
/// Interface for hazard objects
/// </summary>
public interface IHazard
{
    float Damage { get; }
    bool IsActive { get; }
}
