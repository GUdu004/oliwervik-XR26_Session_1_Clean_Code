using UnityEngine;

/// <summary>
/// Score system component that implements IScoreSystem interface
/// Single Responsibility: Manages only score-related state and logic
/// </summary>
public class ScoreComponent : MonoBehaviour, IScoreSystem
{
    [Header("Score Settings")]
    [SerializeField] private int startingScore = 0;
    
    private int currentScore;
    
    // Properties
    public int CurrentScore => currentScore;
    
    // Events
    public event System.Action<int> OnScoreChanged;
    
    void Start()
    {
        InitializeScore();
    }
    
    /// <summary>
    /// Initialize score to starting value
    /// </summary>
    private void InitializeScore()
    {
        currentScore = startingScore;
        OnScoreChanged?.Invoke(currentScore);
        GameEvents.TriggerPlayerScoreChanged(currentScore);
    }
    
    /// <summary>
    /// Add points to the current score
    /// </summary>
    /// <param name="points">Points to add (must be positive)</param>
    public void AddScore(int points)
    {
        if (points <= 0)
        {
            Debug.LogWarning($"Cannot add negative or zero points: {points}");
            return;
        }
        
        currentScore += points;
        OnScoreChanged?.Invoke(currentScore);
        GameEvents.TriggerPlayerScoreChanged(currentScore);
        
        Debug.Log($"Score increased by {points}. Current score: {currentScore}");
    }
    
    /// <summary>
    /// Subtract points from the current score
    /// </summary>
    /// <param name="points">Points to subtract (must be positive)</param>
    public void SubtractScore(int points)
    {
        if (points <= 0)
        {
            Debug.LogWarning($"Cannot subtract negative or zero points: {points}");
            return;
        }
        
        currentScore = Mathf.Max(0, currentScore - points);
        OnScoreChanged?.Invoke(currentScore);
        
        Debug.Log($"Score decreased by {points}. Current score: {currentScore}");
    }
    
    /// <summary>
    /// Reset score to starting value
    /// </summary>
    public void ResetScore()
    {
        currentScore = startingScore;
        OnScoreChanged?.Invoke(currentScore);
        
        Debug.Log($"Score reset to {startingScore}");
    }
    
    /// <summary>
    /// Set score to a specific value
    /// </summary>
    /// <param name="score">New score value</param>
    public void SetScore(int score)
    {
        if (score < 0)
        {
            Debug.LogWarning("Score cannot be negative");
            score = 0;
        }
        
        currentScore = score;
        OnScoreChanged?.Invoke(currentScore);
        
        Debug.Log($"Score set to {currentScore}");
    }
}
