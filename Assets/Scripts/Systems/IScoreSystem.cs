using UnityEngine;

/// <summary>
/// Interface for score management system
/// Follows Single Responsibility Principle - only handles score-related functionality
/// </summary>
public interface IScoreSystem
{
    int CurrentScore { get; }
    
    void AddScore(int points);
    void SubtractScore(int points);
    void ResetScore();
    void SetScore(int score);
    
    event System.Action<int> OnScoreChanged;
}
