using UnityEngine;

/// <summary>
/// Interface for health management system
/// Follows Single Responsibility Principle - only handles health-related functionality
/// </summary>
public interface IHealthSystem
{
    float CurrentHealth { get; }
    float MaxHealth { get; }
    bool IsAlive { get; }
    
    void TakeDamage(float amount);
    void Heal(float amount);
    void SetMaxHealth(float maxHealth);
    
    event System.Action<float> OnHealthChanged;
    event System.Action OnDeath;
}
