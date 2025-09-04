using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Single Responsibility: Manage health-related UI elements
/// Listens to GameEvents for health changes and updates the UI accordingly
/// Demonstrates Separation of Concerns - UI logic separate from game logic
/// Implements IHealthUI interface for consistency and testability
/// </summary>
public class HealthUIController : MonoBehaviour, IHealthUI
{
    [Header("Health UI Elements")]
    [SerializeField] private Slider healthBar;
    [SerializeField] private bool showDebugLogs = false;
    
    [Header("Health Bar Configuration")]
    [SerializeField] private float maxHealthValue = 30f;
    [SerializeField] private bool initializeOnStart = true;
    
    /// <summary>
    /// Current health value displayed on the UI
    /// </summary>
    public float CurrentDisplayedHealth { get; private set; }
    
    /// <summary>
    /// Maximum health value for the health bar
    /// </summary>
    public float MaxHealth { get; private set; }
    
    void Start()
    {
        Initialize();
        
        if (showDebugLogs)
            Debug.Log("HealthUIController: Initialized and subscribed to health events");
    }
    
    void OnDestroy()
    {
        UnsubscribeFromEvents();
        
        if (showDebugLogs)
            Debug.Log("HealthUIController: Unsubscribed from health events");
    }
    
    /// <summary>
    /// Initialize the UI controller (implements IUIController)
    /// </summary>
    public void Initialize()
    {
        InitializeHealthUI();
        SubscribeToEvents();
    }
    
    /// <summary>
    /// Subscribe to relevant events (implements IUIController)
    /// </summary>
    public void SubscribeToEvents()
    {
        GameEvents.OnPlayerHealthChanged += UpdateHealthBar;
        GameEvents.OnPlayerDied += HandlePlayerDeath;
        
        // Optional: Listen to health system events for more specific updates
        GameEvents.OnPlayerHealed += HandlePlayerHealed;
        GameEvents.OnPlayerDamaged += HandlePlayerDamaged;
    }
    
    /// <summary>
    /// Unsubscribe from events to prevent memory leaks (implements IUIController)
    /// </summary>
    public void UnsubscribeFromEvents()
    {
        GameEvents.OnPlayerHealthChanged -= UpdateHealthBar;
        GameEvents.OnPlayerDied -= HandlePlayerDeath;
        GameEvents.OnPlayerHealed -= HandlePlayerHealed;
        GameEvents.OnPlayerDamaged -= HandlePlayerDamaged;
    }
    
    /// <summary>
    /// Show or hide the UI element (implements IUIController)
    /// </summary>
    /// <param name="visible">Whether the UI should be visible</param>
    public void SetVisible(bool visible)
    {
        SetHealthBarVisible(visible);
    }
    
    /// <summary>
    /// Initialize the health UI components
    /// </summary>
    private void InitializeHealthUI()
    {
        if (healthBar != null && initializeOnStart)
        {
            SetMaxHealth(maxHealthValue);
            UpdateHealthBar(maxHealthValue); // Start with full health
        }
        else if (healthBar == null)
        {
            Debug.LogWarning("HealthUIController: Health bar slider not assigned!");
        }
    }
    
    /// <summary>
    /// Update the health bar display
    /// </summary>
    /// <param name="currentHealth">Current health value</param>
    private void UpdateHealthBar(float currentHealth)
    {
        if (healthBar == null) return;
        
        CurrentDisplayedHealth = currentHealth;
        healthBar.value = currentHealth;
        
        // Update health bar color based on health percentage
        UpdateHealthBarColor(currentHealth / MaxHealth);
        
        if (showDebugLogs)
            Debug.Log($"HealthUIController: Health updated to {currentHealth}/{MaxHealth}");
    }
    
    /// <summary>
    /// Set the maximum health value for the health bar
    /// </summary>
    /// <param name="maxHealth">Maximum health value</param>
    public void SetMaxHealth(float maxHealth)
    {
        if (healthBar == null) return;
        
        MaxHealth = maxHealth;
        healthBar.maxValue = maxHealth;
        
        if (showDebugLogs)
            Debug.Log($"HealthUIController: Max health set to {maxHealth}");
    }
    
    /// <summary>
    /// Handle player death event
    /// </summary>
    private void HandlePlayerDeath()
    {
        if (healthBar != null)
        {
            healthBar.value = 0f;
            CurrentDisplayedHealth = 0f;
        }
        
        if (showDebugLogs)
            Debug.Log("HealthUIController: Player death - health bar set to 0");
    }
    
    /// <summary>
    /// Handle player healed event (optional visual feedback)
    /// </summary>
    /// <param name="healAmount">Amount healed</param>
    private void HandlePlayerHealed(float healAmount)
    {
        if (showDebugLogs)
            Debug.Log($"HealthUIController: Player healed by {healAmount}");
        
        // Could add healing animation or effect here
        StartCoroutine(FlashHealthBar(Color.green, 0.2f));
    }
    
    /// <summary>
    /// Handle player damaged event (optional visual feedback)
    /// </summary>
    /// <param name="damageAmount">Amount of damage taken</param>
    private void HandlePlayerDamaged(float damageAmount)
    {
        if (showDebugLogs)
            Debug.Log($"HealthUIController: Player damaged by {damageAmount}");
        
        // Could add damage animation or effect here
        StartCoroutine(FlashHealthBar(Color.red, 0.2f));
    }
    
    /// <summary>
    /// Update health bar color based on health percentage
    /// </summary>
    /// <param name="healthPercentage">Health as percentage (0-1)</param>
    private void UpdateHealthBarColor(float healthPercentage)
    {
        if (healthBar?.fillRect?.GetComponent<Image>() == null) return;
        
        Image fillImage = healthBar.fillRect.GetComponent<Image>();
        
        // Color transition: Green -> Yellow -> Red
        if (healthPercentage > 0.6f)
        {
            fillImage.color = Color.green;
        }
        else if (healthPercentage > 0.3f)
        {
            fillImage.color = Color.yellow;
        }
        else
        {
            fillImage.color = Color.red;
        }
    }
    
    /// <summary>
    /// Flash the health bar with a specific color for visual feedback
    /// </summary>
    /// <param name="flashColor">Color to flash</param>
    /// <param name="duration">Duration of the flash</param>
    private System.Collections.IEnumerator FlashHealthBar(Color flashColor, float duration)
    {
        if (healthBar?.fillRect?.GetComponent<Image>() == null) yield break;
        
        Image fillImage = healthBar.fillRect.GetComponent<Image>();
        Color originalColor = fillImage.color;
        
        // Flash to the specified color
        fillImage.color = flashColor;
        yield return new WaitForSeconds(duration);
        
        // Return to original color
        fillImage.color = originalColor;
    }
    
    /// <summary>
    /// Manually set health bar value (for testing or special cases)
    /// </summary>
    /// <param name="health">Health value to set</param>
    public void SetHealthDisplay(float health)
    {
        UpdateHealthBar(health);
    }
    
    /// <summary>
    /// Get the current health percentage (0-1)
    /// </summary>
    /// <returns>Health percentage</returns>
    public float GetHealthPercentage()
    {
        return MaxHealth > 0 ? CurrentDisplayedHealth / MaxHealth : 0f;
    }
    
    /// <summary>
    /// Enable or disable the health bar visibility
    /// </summary>
    /// <param name="visible">Whether the health bar should be visible</param>
    public void SetHealthBarVisible(bool visible)
    {
        if (healthBar != null)
        {
            healthBar.gameObject.SetActive(visible);
        }
    }
}
