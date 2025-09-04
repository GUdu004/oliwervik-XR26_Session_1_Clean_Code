using UnityEngine;
using TMPro;

/// <summary>
/// Single Responsibility: Manage score-related UI elements
/// Listens to GameEvents for score changes and updates the UI accordingly
/// Demonstrates Separation of Concerns - UI logic separate from game logic
/// Implements IScoreUI interface for consistency and testability
/// </summary>
public class ScoreUIController : MonoBehaviour, IScoreUI
{
    [Header("Score UI Elements")]
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private bool showDebugLogs = false;
    
    [Header("Score Display Configuration")]
    [SerializeField] private string scorePrefix = "Score: ";
    [SerializeField] private string scoreSuffix = "";
    [SerializeField] private bool animateScoreChanges = true;
    [SerializeField] private float animationDuration = 0.3f;
    
    /// <summary>
    /// Current score value displayed on the UI
    /// </summary>
    public int CurrentDisplayedScore { get; private set; }
    
    /// <summary>
    /// Target score for animation
    /// </summary>
    private int targetScore;
    
    /// <summary>
    /// Animation coroutine reference
    /// </summary>
    private Coroutine scoreAnimationCoroutine;
    
    void Start()
    {
        Initialize();
        
        if (showDebugLogs)
            Debug.Log("ScoreUIController: Initialized and subscribed to score events");
    }
    
    void OnDestroy()
    {
        UnsubscribeFromEvents();
        
        if (showDebugLogs)
            Debug.Log("ScoreUIController: Unsubscribed from score events");
    }
    
    /// <summary>
    /// Initialize the UI controller (implements IUIController)
    /// </summary>
    public void Initialize()
    {
        InitializeScoreUI();
        SubscribeToEvents();
    }
    
    /// <summary>
    /// Subscribe to relevant events (implements IUIController)
    /// </summary>
    public void SubscribeToEvents()
    {
        GameEvents.OnPlayerScoreChanged += UpdateScore;
        GameEvents.OnGameRestarted += ResetScore;
        
        // Optional: Listen to specific score events for visual feedback
        GameEvents.OnCollectiblePickup += HandleCollectiblePickup;
    }
    
    /// <summary>
    /// Unsubscribe from events to prevent memory leaks (implements IUIController)
    /// </summary>
    public void UnsubscribeFromEvents()
    {
        GameEvents.OnPlayerScoreChanged -= UpdateScore;
        GameEvents.OnGameRestarted -= ResetScore;
        GameEvents.OnCollectiblePickup -= HandleCollectiblePickup;
    }
    
    /// <summary>
    /// Show or hide the UI element (implements IUIController)
    /// </summary>
    /// <param name="visible">Whether the UI should be visible</param>
    public void SetVisible(bool visible)
    {
        SetScoreVisible(visible);
    }
    
    /// <summary>
    /// Initialize the score UI components
    /// </summary>
    private void InitializeScoreUI()
    {
        CurrentDisplayedScore = 0;
        targetScore = 0;
        UpdateScoreDisplay(0);
        
        if (scoreText == null)
        {
            Debug.LogWarning("ScoreUIController: Score text component not assigned!");
        }
    }
    
    /// <summary>
    /// Update the score display with new score value
    /// </summary>
    /// <param name="newScore">New score value</param>
    private void UpdateScore(int newScore)
    {
        if (scoreText == null) return;
        
        targetScore = newScore;
        
        if (animateScoreChanges && gameObject.activeInHierarchy)
        {
            // Stop any existing animation
            if (scoreAnimationCoroutine != null)
            {
                StopCoroutine(scoreAnimationCoroutine);
            }
            
            // Start new animation
            scoreAnimationCoroutine = StartCoroutine(AnimateScoreChange(CurrentDisplayedScore, targetScore));
        }
        else
        {
            // Update immediately
            CurrentDisplayedScore = newScore;
            UpdateScoreDisplay(newScore);
        }
        
        if (showDebugLogs)
            Debug.Log($"ScoreUIController: Score updated to {newScore}");
    }
    
    /// <summary>
    /// Reset the score display to zero
    /// </summary>
    private void ResetScore()
    {
        CurrentDisplayedScore = 0;
        targetScore = 0;
        
        // Stop any ongoing animation
        if (scoreAnimationCoroutine != null)
        {
            StopCoroutine(scoreAnimationCoroutine);
            scoreAnimationCoroutine = null;
        }
        
        UpdateScoreDisplay(0);
        
        if (showDebugLogs)
            Debug.Log("ScoreUIController: Score reset to 0");
    }
    
    /// <summary>
    /// Handle collectible pickup event for visual feedback
    /// </summary>
    /// <param name="points">Points gained from collectible</param>
    private void HandleCollectiblePickup(int points)
    {
        if (showDebugLogs)
            Debug.Log($"ScoreUIController: Collectible picked up for {points} points");
        
        // Could add pickup animation or effect here
        StartCoroutine(FlashScoreText(Color.yellow, 0.2f));
    }
    
    /// <summary>
    /// Update the actual score text display
    /// </summary>
    /// <param name="score">Score value to display</param>
    private void UpdateScoreDisplay(int score)
    {
        if (scoreText != null)
        {
            scoreText.text = $"{scorePrefix}{score}{scoreSuffix}";
        }
    }
    
    /// <summary>
    /// Animate score change from current to target value
    /// </summary>
    /// <param name="startScore">Starting score value</param>
    /// <param name="endScore">Target score value</param>
    private System.Collections.IEnumerator AnimateScoreChange(int startScore, int endScore)
    {
        float elapsedTime = 0f;
        
        while (elapsedTime < animationDuration)
        {
            elapsedTime += Time.deltaTime;
            float progress = elapsedTime / animationDuration;
            
            // Use smooth easing for animation
            progress = EaseOutQuart(progress);
            
            int animatedScore = Mathf.RoundToInt(Mathf.Lerp(startScore, endScore, progress));
            CurrentDisplayedScore = animatedScore;
            UpdateScoreDisplay(animatedScore);
            
            yield return null;
        }
        
        // Ensure final value is exact
        CurrentDisplayedScore = endScore;
        UpdateScoreDisplay(endScore);
        scoreAnimationCoroutine = null;
    }
    
    /// <summary>
    /// Easing function for smooth animation
    /// </summary>
    /// <param name="t">Time value (0-1)</param>
    /// <returns>Eased value</returns>
    private float EaseOutQuart(float t)
    {
        return 1f - Mathf.Pow(1f - t, 4f);
    }
    
    /// <summary>
    /// Flash the score text with a specific color for visual feedback
    /// </summary>
    /// <param name="flashColor">Color to flash</param>
    /// <param name="duration">Duration of the flash</param>
    private System.Collections.IEnumerator FlashScoreText(Color flashColor, float duration)
    {
        if (scoreText == null) yield break;
        
        Color originalColor = scoreText.color;
        
        // Flash to the specified color
        scoreText.color = flashColor;
        yield return new WaitForSeconds(duration);
        
        // Return to original color
        scoreText.color = originalColor;
    }
    
    /// <summary>
    /// Manually set score display (for testing or special cases)
    /// </summary>
    /// <param name="score">Score value to set</param>
    public void SetScoreDisplay(int score)
    {
        CurrentDisplayedScore = score;
        targetScore = score;
        
        // Stop any ongoing animation
        if (scoreAnimationCoroutine != null)
        {
            StopCoroutine(scoreAnimationCoroutine);
            scoreAnimationCoroutine = null;
        }
        
        UpdateScoreDisplay(score);
    }
    
    /// <summary>
    /// Set the score prefix text
    /// </summary>
    /// <param name="prefix">New prefix text</param>
    public void SetScorePrefix(string prefix)
    {
        scorePrefix = prefix;
        UpdateScoreDisplay(CurrentDisplayedScore);
    }
    
    /// <summary>
    /// Set the score suffix text
    /// </summary>
    /// <param name="suffix">New suffix text</param>
    public void SetScoreSuffix(string suffix)
    {
        scoreSuffix = suffix;
        UpdateScoreDisplay(CurrentDisplayedScore);
    }
    
    /// <summary>
    /// Enable or disable score animation
    /// </summary>
    /// <param name="animate">Whether to animate score changes</param>
    public void SetScoreAnimation(bool animate)
    {
        animateScoreChanges = animate;
    }
    
    /// <summary>
    /// Enable or disable the score text visibility
    /// </summary>
    /// <param name="visible">Whether the score text should be visible</param>
    public void SetScoreVisible(bool visible)
    {
        if (scoreText != null)
        {
            scoreText.gameObject.SetActive(visible);
        }
    }
}
