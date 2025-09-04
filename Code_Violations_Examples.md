# Code Violations - Detailed Examples
## XR26 Session 1 - Clean Code Exercise

---

## 🔍 **Specific Code Examples of SOLID Violations**

### **1. Single Responsibility Principle (SRP) Violations**

#### **Player.cs - Line-by-Line Analysis:**

```csharp
// VIOLATION: Player class handles TOO MANY responsibilities
public class Player : MonoBehaviour
{
    // RESPONSIBILITY 1: Movement Configuration
    private float moveSpeed = 5f;
    private float jumpForce = 10f;
    private float rotationSpeed = 0.5f;
    
    // RESPONSIBILITY 2: Health System
    private float health = 30f;
    
    // RESPONSIBILITY 3: Score System  
    private int score = 0;
    
    // RESPONSIBILITY 4: UI Management (WRONG!)
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private Slider healthBar;
    
    // RESPONSIBILITY 5: Game State Management (WRONG!)
    [SerializeField] private GameManager gameManager;
}
```

#### **OnCollisionEnter - Multiple Behaviors:**
```csharp
void OnCollisionEnter(Collision collision)
{
    // BEHAVIOR 1: Ground Detection
    if (collision.gameObject.CompareTag("Ground")) { /* ... */ }
    
    // BEHAVIOR 2: Collectible System
    if (collision.gameObject.CompareTag("Collectible"))
    {
        score += 10;           // Score logic
        UpdateScoreUI();       // UI logic  
        Destroy(collision.gameObject); // Object management
    }
    
    // BEHAVIOR 3: Combat System
    if (collision.gameObject.CompareTag("Enemy"))
    {
        TakeDamage(10);        // Health logic
        Destroy(collision.gameObject); // Object management
    }
}
```

---

### **2. Open/Closed Principle (OCP) Violations**

#### **Hard-coded Values Everywhere:**
```csharp
// VIOLATION: Cannot extend without modification
score += 10;                    // Fixed score value
TakeDamage(10);                // Fixed damage value
if (player.GetScore() >= 30)   // Fixed win condition
health = 30f;                  // Fixed max health
healthBar.maxValue = 30f;      // UI tightly coupled to game logic
```

#### **Modification Required for Extension:**
```csharp
// TO ADD: Different collectible types, you must MODIFY this method
void OnCollisionEnter(Collision collision)
{
    if (collision.gameObject.CompareTag("Collectible"))
    {
        score += 10; // What if we want different point values?
    }
    // What if we want power-ups, different enemy types, etc.?
}
```

---

### **3. Liskov Substitution Principle (LSP) Violations**

#### **No Proper Abstractions:**
```csharp
// VIOLATION: Cannot substitute with different implementations
[SerializeField] private GameManager gameManager; // Concrete dependency

// PROBLEM: Cannot substitute Player with:
// - AIPlayer
// - NetworkPlayer  
// - ReplayPlayer
// Because everything is tightly coupled to specific implementations
```

---

### **4. Interface Segregation Principle (ISP) Violations**

#### **Monolithic Interfaces:**
```csharp
// CURRENT: Player "interface" forces everything together
public class Player : MonoBehaviour // Monolithic!
{
    // Health methods
    public void TakeDamage(float amount) { /* */ }
    
    // Score methods  
    public int GetScore() { /* */ }
    
    // Movement is implicit
    // UI updates are forced
    // Input handling is forced
}

// PROBLEM: Cannot implement just health without movement, 
// or just movement without UI, etc.
```

---

### **5. Dependency Inversion Principle (DIP) Violations**

#### **High-Level Depends on Low-Level:**
```csharp
public class Player : MonoBehaviour // HIGH-LEVEL
{
    // VIOLATION: Depends on LOW-LEVEL UI components
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private Slider healthBar;
    
    // VIOLATION: Depends on LOW-LEVEL concrete GameManager
    [SerializeField] private GameManager gameManager;
    
    private void UpdateScoreUI()
    {
        // HIGH-LEVEL game logic depends on LOW-LEVEL UI implementation
        scoreText.text = "Score: " + score;
    }
}
```

```csharp
public class GameManager : MonoBehaviour // HIGH-LEVEL
{
    // VIOLATION: Depends on LOW-LEVEL Player implementation
    [SerializeField] private Player player;
    
    void Update()
    {
        // HIGH-LEVEL game state depends on LOW-LEVEL player details
        if (player.GetScore() >= 30)
        {
            WinGame();
        }
    }
}
```

---

## 🚫 **Anti-Patterns Identified**

### **1. God Object**
```csharp
// Player class does EVERYTHING:
// - Movement ✓
// - Health ✓  
// - Score ✓
// - UI Updates ✓
// - Input Handling ✓
// - Collision Detection ✓
// - Game State Checking ✓
```

### **2. Feature Envy**
```csharp
// Player is envious of UI responsibilities:
private void UpdateScoreUI() { /* Player shouldn't know about UI! */ }
private void UpdateHealthUI() { /* Player shouldn't know about UI! */ }
```

### **3. Magic Numbers**
```csharp
score += 10;                    // Why 10?
TakeDamage(10);                // Why 10?
if (player.GetScore() >= 30)   // Why 30?
health = 30f;                  // Why 30?
```

### **4. Shotgun Surgery**
```csharp
// To change score system, you must modify:
// 1. Player.cs (score variable)
// 2. Player.cs (UpdateScoreUI method)  
// 3. Player.cs (OnCollisionEnter)
// 4. GameManager.cs (win condition)
// 5. UI components in scene
```

---

## 🔧 **Before vs After Examples**

### **Current (Violated) Code:**
```csharp
public class Player : MonoBehaviour
{
    private int score = 0;
    [SerializeField] private TextMeshProUGUI scoreText;
    
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Collectible"))
        {
            score += 10;
            UpdateScoreUI();
            Destroy(collision.gameObject);
        }
    }
    
    private void UpdateScoreUI()
    {
        scoreText.text = "Score: " + score;
    }
}
```

### **Refactored (SOLID) Code:**
```csharp
// SOLID: Single Responsibility
public class CollectibleHandler : MonoBehaviour
{
    [SerializeField] private ScoreConfig config;
    private IScoreSystem scoreSystem;
    private IUIEventSystem uiEvents;
    
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Collectible"))
        {
            var collectible = collision.gameObject.GetComponent<ICollectible>();
            scoreSystem.AddScore(collectible.GetValue());
            collectible.Collect();
        }
    }
}

// SOLID: Interface Segregation  
public interface IScoreSystem
{
    void AddScore(int points);
    int GetCurrentScore();
    event Action<int> OnScoreChanged;
}

// SOLID: Dependency Inversion
public class ScoreUIController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI scoreText;
    
    void Start()
    {
        var scoreSystem = ServiceLocator.Get<IScoreSystem>();
        scoreSystem.OnScoreChanged += UpdateDisplay;
    }
    
    private void UpdateDisplay(int newScore)
    {
        scoreText.text = $"Score: {newScore}";
    }
}
```

---

## 📊 **Metrics of Violations**

### **Coupling Metrics:**
- **Player.cs**: Coupled to 4+ different systems
- **GameManager.cs**: Coupled to 3+ different systems
- **Circular Dependencies**: Player ↔ GameManager

### **Cohesion Metrics:**
- **Player.cs**: Low cohesion (7+ responsibilities)
- **GameManager.cs**: Low cohesion (5+ responsibilities)

### **Complexity Metrics:**
- **OnCollisionEnter**: Cyclomatic complexity = 4
- **Update methods**: Mixed concerns = High complexity

---

**Next Step:** Use this analysis to guide your refactoring priorities!
