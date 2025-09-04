using UnityEngine;

/// <summary>
/// Health system component that implements IHealthSystem interface
/// Single Responsibility: Manages only health-related state and logic
/// </summary>
public class HealthComponent : MonoBehaviour, IHealthSystem
{
    [Header("Health Settings")]
    [SerializeField] private float maxHealth = 30f;
    
    private float currentHealth;
    
    // Properties
    public float CurrentHealth => currentHealth;
    public float MaxHealth => maxHealth;
    public bool IsAlive => currentHealth > 0;
    
    // Events
    public event System.Action<float> OnHealthChanged;
    public event System.Action OnDeath;
    
    void Start()
    {
        InitializeHealth();
    }
    
    /// <summary>
    /// Initialize health to maximum value
    /// </summary>
    private void InitializeHealth()
    {
        currentHealth = maxHealth;
        OnHealthChanged?.Invoke(currentHealth);
        GameEvents.TriggerPlayerHealthChanged(currentHealth);
    }
    
    /// <summary>
    /// Apply damage to the health system
    /// </summary>
    /// <param name="amount">Amount of damage to apply</param>
    public void TakeDamage(float amount)
    {
        if (amount <= 0) return;
        
        currentHealth = Mathf.Max(0, currentHealth - amount);
        OnHealthChanged?.Invoke(currentHealth);
        
        // Trigger global game event
        GameEvents.TriggerPlayerHealthChanged(currentHealth);
        GameEvents.TriggerDamageTaken(amount, "Unknown");
        
        Debug.Log($"Health reduced by {amount}. Current health: {currentHealth}");
        
        if (currentHealth <= 0)
        {
            OnDeath?.Invoke();
            GameEvents.TriggerPlayerDied();
            Debug.Log("Health system triggered death event");
        }
    }
    
    /// <summary>
    /// Restore health up to maximum value
    /// </summary>
    /// <param name="amount">Amount of health to restore</param>
    public void Heal(float amount)
    {
        if (amount <= 0) return;
        
        float previousHealth = currentHealth;
        currentHealth = Mathf.Min(maxHealth, currentHealth + amount);
        
        if (currentHealth != previousHealth)
        {
            OnHealthChanged?.Invoke(currentHealth);
            GameEvents.TriggerPlayerHealthChanged(currentHealth);
            Debug.Log($"Health restored by {amount}. Current health: {currentHealth}");
        }
    }
    
    /// <summary>
    /// Set the maximum health value and adjust current health if necessary
    /// </summary>
    /// <param name="newMaxHealth">New maximum health value</param>
    public void SetMaxHealth(float newMaxHealth)
    {
        if (newMaxHealth <= 0)
        {
            Debug.LogWarning("Max health cannot be zero or negative");
            return;
        }
        
        maxHealth = newMaxHealth;
        
        // Adjust current health if it exceeds new maximum
        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
            OnHealthChanged?.Invoke(currentHealth);
        }
    }
    
    /// <summary>
    /// Reset health to maximum (useful for respawning)
    /// </summary>
    public void ResetHealth()
    {
        InitializeHealth();
    }
}
