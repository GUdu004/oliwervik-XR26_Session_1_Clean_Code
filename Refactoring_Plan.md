# Refactoring Plan - Step by Step
## XR26 Session 1 - Clean Code Exercise

---

## 🎯 **Refactoring Strategy Overview**

### **Approach:** Incremental refactoring with minimal breaking changes
### **Goal:** Transform monolithic code into SOLID-compliant architecture
### **Timeline:** 4 phases, each builds on the previous

---

## 📋 **Phase 1: Extract Single Responsibilities (SRP)**
*Estimated Time: 2-3 hours*

### **Step 1.1: Create Health System**
```csharp
// NEW FILE: Assets/Scripts/Systems/IHealthSystem.cs
public interface IHealthSystem
{
    float CurrentHealth { get; }
    float MaxHealth { get; }
    void TakeDamage(float amount);
    void Heal(float amount);
    bool IsAlive { get; }
    event System.Action<float> OnHealthChanged;
    event System.Action OnDeath;
}

// NEW FILE: Assets/Scripts/Systems/HealthComponent.cs
public class HealthComponent : MonoBehaviour, IHealthSystem
{
    [SerializeField] private float maxHealth = 30f;
    private float currentHealth;
    
    public float CurrentHealth => currentHealth;
    public float MaxHealth => maxHealth;
    public bool IsAlive => currentHealth > 0;
    
    public event System.Action<float> OnHealthChanged;
    public event System.Action OnDeath;
    
    void Start()
    {
        currentHealth = maxHealth;
    }
    
    public void TakeDamage(float amount)
    {
        currentHealth = Mathf.Max(0, currentHealth - amount);
        OnHealthChanged?.Invoke(currentHealth);
        
        if (currentHealth <= 0)
            OnDeath?.Invoke();
    }
    
    public void Heal(float amount)
    {
        currentHealth = Mathf.Min(maxHealth, currentHealth + amount);
        OnHealthChanged?.Invoke(currentHealth);
    }
}
```

### **Step 1.2: Create Score System**
```csharp
// NEW FILE: Assets/Scripts/Systems/IScoreSystem.cs
public interface IScoreSystem
{
    int CurrentScore { get; }
    void AddScore(int points);
    void ResetScore();
    event System.Action<int> OnScoreChanged;
}

// NEW FILE: Assets/Scripts/Systems/ScoreComponent.cs
public class ScoreComponent : MonoBehaviour, IScoreSystem
{
    private int currentScore = 0;
    
    public int CurrentScore => currentScore;
    public event System.Action<int> OnScoreChanged;
    
    public void AddScore(int points)
    {
        currentScore += points;
        OnScoreChanged?.Invoke(currentScore);
    }
    
    public void ResetScore()
    {
        currentScore = 0;
        OnScoreChanged?.Invoke(currentScore);
    }
}
```

### **Step 1.3: Create Movement System**
```csharp
// NEW FILE: Assets/Scripts/Movement/IMovementController.cs
public interface IMovementController
{
    void Move(Vector2 input);
    void Rotate(float mouseX);
    void Jump();
    bool IsGrounded { get; }
}

// NEW FILE: Assets/Scripts/Movement/PlayerMovement.cs
public class PlayerMovement : MonoBehaviour, IMovementController
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 10f;
    [SerializeField] private float rotationSpeed = 0.5f;
    
    private Rigidbody rb;
    private bool isGrounded;
    private float yaw;
    
    public bool IsGrounded => isGrounded;
    
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
    }
    
    public void Move(Vector2 input)
    {
        Vector3 direction = transform.forward * input.y + transform.right * input.x;
        Vector3 velocity = direction.normalized * moveSpeed;
        Vector3 newPos = rb.position + velocity * Time.fixedDeltaTime;
        rb.MovePosition(newPos);
    }
    
    public void Rotate(float mouseX)
    {
        yaw += mouseX * rotationSpeed;
        transform.rotation = Quaternion.Euler(0f, yaw, 0f);
    }
    
    public void Jump()
    {
        if (isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isGrounded = false;
        }
    }
    
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }
}
```

---

## 📋 **Phase 2: Create Event System (Decouple Dependencies)**
*Estimated Time: 1-2 hours*

### **Step 2.1: Create Game Events**
```csharp
// NEW FILE: Assets/Scripts/Events/GameEvents.cs
public static class GameEvents
{
    // Player Events
    public static event System.Action<float> OnPlayerHealthChanged;
    public static event System.Action OnPlayerDied;
    public static event System.Action<int> OnPlayerScoreChanged;
    
    // Game Events
    public static event System.Action OnGameWon;
    public static event System.Action OnGameOver;
    public static event System.Action OnGameRestarted;
    
    // Collectible Events
    public static event System.Action<int> OnCollectiblePickup;
    
    // Methods to trigger events
    public static void TriggerPlayerHealthChanged(float health) => OnPlayerHealthChanged?.Invoke(health);
    public static void TriggerPlayerDied() => OnPlayerDied?.Invoke();
    public static void TriggerPlayerScoreChanged(int score) => OnPlayerScoreChanged?.Invoke(score);
    public static void TriggerGameWon() => OnGameWon?.Invoke();
    public static void TriggerGameOver() => OnGameOver?.Invoke();
    public static void TriggerGameRestarted() => OnGameRestarted?.Invoke();
    public static void TriggerCollectiblePickup(int points) => OnCollectiblePickup?.Invoke(points);
}
```

### **Step 2.2: Create Input Handler**
```csharp
// NEW FILE: Assets/Scripts/Input/IInputHandler.cs
public interface IInputHandler
{
    Vector2 MovementInput { get; }
    float MouseX { get; }
    bool JumpPressed { get; }
    bool RestartPressed { get; }
}

// NEW FILE: Assets/Scripts/Input/PlayerInputHandler.cs
public class PlayerInputHandler : MonoBehaviour, IInputHandler
{
    public Vector2 MovementInput => new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
    public float MouseX => Input.GetAxis("Mouse X");
    public bool JumpPressed => Input.GetButtonDown("Jump");
    public bool RestartPressed => Input.GetKeyDown(KeyCode.R);
}
```

---

## 📋 **Phase 3: Extract UI Management**
*Estimated Time: 1-2 hours*

### **Step 3.1: Create UI Controllers**
```csharp
// NEW FILE: Assets/Scripts/UI/HealthUIController.cs
public class HealthUIController : MonoBehaviour
{
    [SerializeField] private Slider healthBar;
    
    void Start()
    {
        GameEvents.OnPlayerHealthChanged += UpdateHealthBar;
    }
    
    void OnDestroy()
    {
        GameEvents.OnPlayerHealthChanged -= UpdateHealthBar;
    }
    
    private void UpdateHealthBar(float health)
    {
        if (healthBar != null)
        {
            healthBar.value = health;
        }
    }
    
    public void SetMaxHealth(float maxHealth)
    {
        if (healthBar != null)
        {
            healthBar.maxValue = maxHealth;
        }
    }
}

// NEW FILE: Assets/Scripts/UI/ScoreUIController.cs
public class ScoreUIController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI scoreText;
    
    void Start()
    {
        GameEvents.OnPlayerScoreChanged += UpdateScoreDisplay;
    }
    
    void OnDestroy()
    {
        GameEvents.OnPlayerScoreChanged -= UpdateScoreDisplay;
    }
    
    private void UpdateScoreDisplay(int score)
    {
        if (scoreText != null)
        {
            scoreText.text = $"Score: {score}";
        }
    }
}
```

---

## 📋 **Phase 4: Refactor Main Classes**
*Estimated Time: 2-3 hours*

### **Step 4.1: Create New Player Controller**
```csharp
// NEW FILE: Assets/Scripts/Player/PlayerController.cs (Replace existing)
public class PlayerController : MonoBehaviour
{
    private IHealthSystem healthSystem;
    private IScoreSystem scoreSystem;
    private IMovementController movement;
    private IInputHandler input;
    
    void Start()
    {
        // Get components
        healthSystem = GetComponent<IHealthSystem>();
        scoreSystem = GetComponent<IScoreSystem>();
        movement = GetComponent<IMovementController>();
        input = GetComponent<IInputHandler>();
        
        // Setup event listeners
        healthSystem.OnHealthChanged += GameEvents.TriggerPlayerHealthChanged;
        healthSystem.OnDeath += GameEvents.TriggerPlayerDied;
        scoreSystem.OnScoreChanged += GameEvents.TriggerPlayerScoreChanged;
        
        // Initialize cursor
        Cursor.lockState = CursorLockMode.Locked;
    }
    
    void Update()
    {
        // Handle input
        movement.Rotate(input.MouseX);
        
        if (input.JumpPressed)
        {
            movement.Jump();
        }
    }
    
    void FixedUpdate()
    {
        movement.Move(input.MovementInput);
    }
    
    void OnDestroy()
    {
        // Cleanup event listeners
        if (healthSystem != null)
        {
            healthSystem.OnHealthChanged -= GameEvents.TriggerPlayerHealthChanged;
            healthSystem.OnDeath -= GameEvents.TriggerPlayerDied;
        }
        if (scoreSystem != null)
        {
            scoreSystem.OnScoreChanged -= GameEvents.TriggerPlayerScoreChanged;
        }
    }
}
```

### **Step 4.2: Create Collision Handlers**
```csharp
// NEW FILE: Assets/Scripts/Interaction/CollisionHandler.cs
public class CollisionHandler : MonoBehaviour
{
    private IHealthSystem healthSystem;
    private IScoreSystem scoreSystem;
    
    void Start()
    {
        healthSystem = GetComponent<IHealthSystem>();
        scoreSystem = GetComponent<IScoreSystem>();
    }
    
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Collectible"))
        {
            HandleCollectible(collision.gameObject);
        }
        else if (collision.gameObject.CompareTag("Enemy"))
        {
            HandleEnemy(collision.gameObject);
        }
    }
    
    private void HandleCollectible(GameObject collectible)
    {
        // Get collectible value (make this configurable later)
        int points = 10; // TODO: Get from ScriptableObject
        
        scoreSystem.AddScore(points);
        GameEvents.TriggerCollectiblePickup(points);
        Destroy(collectible);
    }
    
    private void HandleEnemy(GameObject enemy)
    {
        // Get enemy damage (make this configurable later)
        float damage = 10f; // TODO: Get from enemy component
        
        healthSystem.TakeDamage(damage);
        Destroy(enemy);
    }
}
```

### **Step 4.3: Refactor GameManager**
```csharp
// MODIFY: Assets/Scripts/Managers/GameManager.cs
public class GameManager : MonoBehaviour
{
    [Header("Game Settings")]
    [SerializeField] private int winScore = 30;
    
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI gameStatusText;
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private GameObject gameOverPanel;
    
    // Game state
    public bool gameOver = false;
    public float gameTime = 0f;
    
    void Start()
    {
        // Subscribe to events instead of direct references
        GameEvents.OnPlayerDied += HandleGameOver;
        GameEvents.OnPlayerScoreChanged += CheckWinCondition;
        
        // Initialize UI
        InitializeUI();
    }
    
    void Update()
    {
        if (!gameOver)
        {
            gameTime += Time.deltaTime;
            UpdateTimerUI();
        }
    }
    
    private void CheckWinCondition(int score)
    {
        if (score >= winScore && !gameOver)
        {
            HandleGameWon();
        }
    }
    
    private void HandleGameOver()
    {
        gameOver = true;
        GameEvents.TriggerGameOver();
        
        if (gameStatusText != null)
            gameStatusText.text = "GAME OVER!";
        
        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);
        
        Invoke(nameof(RestartGame), 2f);
    }
    
    private void HandleGameWon()
    {
        gameOver = true;
        GameEvents.TriggerGameWon();
        
        if (gameStatusText != null)
            gameStatusText.text = "YOU WIN!";
        
        Invoke(nameof(RestartGame), 2f);
    }
    
    public void RestartGame()
    {
        GameEvents.TriggerGameRestarted();
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    
    private void InitializeUI()
    {
        if (gameStatusText != null)
            gameStatusText.text = "Game Started!";
        
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);
    }
    
    private void UpdateTimerUI()
    {
        if (timerText != null)
        {
            timerText.text = $"Time: {Mathf.FloorToInt(gameTime)}s";
        }
    }
    
    void OnDestroy()
    {
        // Cleanup event subscriptions
        GameEvents.OnPlayerDied -= HandleGameOver;
        GameEvents.OnPlayerScoreChanged -= CheckWinCondition;
    }
}
```

---

## 📋 **Implementation Order**

### **Day 1: Foundation**
1. ✅ Create interfaces (IHealthSystem, IScoreSystem, IMovementController)
2. ✅ Create basic implementations (HealthComponent, ScoreComponent, PlayerMovement)
3. ✅ Create GameEvents static class

### **Day 2: UI Separation**
1. ✅ Create UI controllers (HealthUIController, ScoreUIController)
2. ✅ Create input handler (PlayerInputHandler)
3. ✅ Test event system works

### **Day 3: Integration**
1. ✅ Refactor PlayerController to use new components
2. ✅ Create CollisionHandler
3. ✅ Update GameManager to use events

### **Day 4: Polish & Configuration**
1. ✅ Create ScriptableObjects for configuration
2. ✅ Remove old Player.cs file
3. ✅ Test everything works together

---

## 🧪 **Testing Strategy**

### **Phase 1 Testing:**
- ✅ Health system: Damage, healing, death events
- ✅ Score system: Adding points, score events
- ✅ Movement: All directions, jumping, rotation

### **Phase 2 Testing:**
- ✅ Event system: All events fire correctly
- ✅ Input: All inputs registered properly
- ✅ Decoupling: Remove components, ensure no null references

### **Phase 3 Testing:**
- ✅ UI updates: Health bar, score text update correctly
- ✅ UI isolation: UI can work without game logic components

### **Phase 4 Testing:**
- ✅ Full integration: All systems work together
- ✅ Game flow: Start → Play → Win/Lose → Restart
- ✅ Performance: No memory leaks from events

---

## 🎯 **Success Criteria**

### **After Refactoring:**
- ✅ **Single Responsibility**: Each class has one job
- ✅ **Open/Closed**: Can add features without modifying existing code
- ✅ **Liskov Substitution**: Can swap implementations via interfaces
- ✅ **Interface Segregation**: Small, focused interfaces
- ✅ **Dependency Inversion**: High-level depends on abstractions

### **Measurable Improvements:**
- ✅ **Testability**: Can unit test individual components
- ✅ **Maintainability**: Changes require editing fewer files
- ✅ **Extensibility**: Can add new features easily
- ✅ **Readability**: Code intent is clear and focused

---

**Start with Phase 1 and work incrementally!** 🚀
