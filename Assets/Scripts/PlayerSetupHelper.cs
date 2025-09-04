using UnityEngine;

/// <summary>
/// Helper script to automatically set up all required components for the PlayerController
/// Use this to quickly configure a GameObject with all necessary components for the new modular player system
/// </summary>
public class PlayerSetupHelper : MonoBehaviour
{
    [Header("Auto-Setup Options")]
    [SerializeField] private bool setupOnStart = false;
    [SerializeField] private bool showDetailedLogs = true;
    
    [Header("Component Configuration")]
    [SerializeField] private float defaultHealth = 30f;
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 10f;
    [SerializeField] private float rotationSpeed = 0.5f;
    
    void Start()
    {
        if (setupOnStart)
        {
            SetupPlayerComponents();
        }
    }
    
    /// <summary>
    /// Set up all required components for the PlayerController system
    /// Call this method to automatically add and configure all necessary components
    /// </summary>
    [ContextMenu("Setup Player Components")]
    public void SetupPlayerComponents()
    {
        LogInfo("=== Starting Player Component Setup ===");
        
        bool anyComponentAdded = false;
        
        // Add PlayerController if missing
        if (GetComponent<PlayerController>() == null)
        {
            gameObject.AddComponent<PlayerController>();
            LogInfo("✅ Added PlayerController");
            anyComponentAdded = true;
        }
        else
        {
            LogInfo("✓ PlayerController already exists");
        }
        
        // Add HealthComponent if missing
        if (GetComponent<HealthComponent>() == null)
        {
            HealthComponent health = gameObject.AddComponent<HealthComponent>();
            // Configure health component via reflection or public method if available
            LogInfo("✅ Added HealthComponent");
            anyComponentAdded = true;
        }
        else
        {
            LogInfo("✓ HealthComponent already exists");
        }
        
        // Add ScoreComponent if missing
        if (GetComponent<ScoreComponent>() == null)
        {
            gameObject.AddComponent<ScoreComponent>();
            LogInfo("✅ Added ScoreComponent");
            anyComponentAdded = true;
        }
        else
        {
            LogInfo("✓ ScoreComponent already exists");
        }
        
        // Add PlayerMovement if missing
        if (GetComponent<PlayerMovement>() == null)
        {
            PlayerMovement movement = gameObject.AddComponent<PlayerMovement>();
            LogInfo("✅ Added PlayerMovement");
            anyComponentAdded = true;
        }
        else
        {
            LogInfo("✓ PlayerMovement already exists");
        }
        
        // Add PlayerInputHandler if missing
        if (GetComponent<PlayerInputHandler>() == null)
        {
            gameObject.AddComponent<PlayerInputHandler>();
            LogInfo("✅ Added PlayerInputHandler");
            anyComponentAdded = true;
        }
        else
        {
            LogInfo("✓ PlayerInputHandler already exists");
        }
        
        // Add CollisionHandler if missing
        if (GetComponent<CollisionHandler>() == null)
        {
            gameObject.AddComponent<CollisionHandler>();
            LogInfo("✅ Added CollisionHandler");
            anyComponentAdded = true;
        }
        else
        {
            LogInfo("✓ CollisionHandler already exists");
        }
        
        // Add Unity components if missing
        SetupUnityComponents();
        
        // Validate the setup
        ValidateSetup();
        
        if (anyComponentAdded)
        {
            LogInfo("🎉 Player component setup completed! New components added.");
            LogInfo("💡 You may want to configure component settings in the Inspector.");
        }
        else
        {
            LogInfo("✅ All components already present. Setup validation completed.");
        }
        
        LogInfo("=== Player Component Setup Finished ===");
    }
    
    /// <summary>
    /// Set up required Unity components (Rigidbody, Collider)
    /// </summary>
    private void SetupUnityComponents()
    {
        // Add Rigidbody if missing
        if (GetComponent<Rigidbody>() == null)
        {
            Rigidbody rb = gameObject.AddComponent<Rigidbody>();
            rb.freezeRotation = true; // Prevent physics from rotating the player
            rb.mass = 1f;
            rb.linearDamping = 5f; // Add some drag for better control
            LogInfo("✅ Added Rigidbody with recommended settings");
        }
        else
        {
            LogInfo("✓ Rigidbody already exists");
        }
        
        // Add Collider if missing
        if (GetComponent<Collider>() == null)
        {
            CapsuleCollider collider = gameObject.AddComponent<CapsuleCollider>();
            collider.height = 2f;
            collider.radius = 0.5f;
            collider.center = new Vector3(0, 1f, 0); // Offset for standing character
            LogInfo("✅ Added CapsuleCollider with recommended settings");
        }
        else
        {
            LogInfo("✓ Collider already exists");
        }
    }
    
    /// <summary>
    /// Validate that the setup is correct and all dependencies are satisfied
    /// </summary>
    private void ValidateSetup()
    {
        LogInfo("--- Validating Component Setup ---");
        
        // Check for required interfaces
        bool valid = true;
        
        if (GetComponent<IHealthSystem>() == null)
        {
            Debug.LogError("❌ IHealthSystem implementation not found!");
            valid = false;
        }
        
        if (GetComponent<IScoreSystem>() == null)
        {
            Debug.LogError("❌ IScoreSystem implementation not found!");
            valid = false;
        }
        
        if (GetComponent<IMovementController>() == null)
        {
            Debug.LogError("❌ IMovementController implementation not found!");
            valid = false;
        }
        
        if (GetComponent<IInputHandler>() == null)
        {
            Debug.LogError("❌ IInputHandler implementation not found!");
            valid = false;
        }
        
        if (GetComponent<Rigidbody>() == null)
        {
            Debug.LogError("❌ Rigidbody component required for movement!");
            valid = false;
        }
        
        if (GetComponent<Collider>() == null)
        {
            Debug.LogError("❌ Collider component required for collision detection!");
            valid = false;
        }
        
        if (valid)
        {
            LogInfo("✅ All required components and interfaces are present!");
            LogInfo("🎮 PlayerController should now work without errors.");
        }
        else
        {
            Debug.LogError("❌ Setup validation failed. Some required components are missing.");
        }
    }
    
    /// <summary>
    /// Remove this helper component after setup (optional)
    /// </summary>
    [ContextMenu("Remove Setup Helper")]
    public void RemoveSetupHelper()
    {
        LogInfo("Removing PlayerSetupHelper component...");
        
        if (Application.isPlaying)
        {
            Destroy(this);
        }
        else
        {
            DestroyImmediate(this);
        }
    }
    
    /// <summary>
    /// Show current component status
    /// </summary>
    [ContextMenu("Show Component Status")]
    public void ShowComponentStatus()
    {
        LogInfo("=== Current Component Status ===");
        
        LogInfo($"PlayerController: {(GetComponent<PlayerController>() != null ? "✅" : "❌")}");
        LogInfo($"HealthComponent: {(GetComponent<HealthComponent>() != null ? "✅" : "❌")}");
        LogInfo($"ScoreComponent: {(GetComponent<ScoreComponent>() != null ? "✅" : "❌")}");
        LogInfo($"PlayerMovement: {(GetComponent<PlayerMovement>() != null ? "✅" : "❌")}");
        LogInfo($"PlayerInputHandler: {(GetComponent<PlayerInputHandler>() != null ? "✅" : "❌")}");
        LogInfo($"CollisionHandler: {(GetComponent<CollisionHandler>() != null ? "✅" : "❌")}");
        LogInfo($"Rigidbody: {(GetComponent<Rigidbody>() != null ? "✅" : "❌")}");
        LogInfo($"Collider: {(GetComponent<Collider>() != null ? "✅" : "❌")}");
        
        LogInfo("=== Interface Implementation Status ===");
        LogInfo($"IHealthSystem: {(GetComponent<IHealthSystem>() != null ? "✅" : "❌")}");
        LogInfo($"IScoreSystem: {(GetComponent<IScoreSystem>() != null ? "✅" : "❌")}");
        LogInfo($"IMovementController: {(GetComponent<IMovementController>() != null ? "✅" : "❌")}");
        LogInfo($"IInputHandler: {(GetComponent<IInputHandler>() != null ? "✅" : "❌")}");
    }
    
    /// <summary>
    /// Log information message if detailed logging is enabled
    /// </summary>
    /// <param name="message">Message to log</param>
    private void LogInfo(string message)
    {
        if (showDetailedLogs)
        {
            Debug.Log($"[PlayerSetup] {message}");
        }
    }
    
    /// <summary>
    /// Display setup instructions in the inspector
    /// </summary>
    void OnValidate()
    {
        // This runs in editor when values change
        if (Application.isEditor && !Application.isPlaying)
        {
            // Could add editor-specific validation here
        }
    }
}
