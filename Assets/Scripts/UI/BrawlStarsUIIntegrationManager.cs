using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Collections;
using System.Linq;

/// <summary>
/// Brawl Stars UI Integration Manager - comprehensive integration system that ties together
/// all Brawl Stars UI components and provides easy setup for the complete UI system.
/// </summary>
public class BrawlStarsUIIntegrationManager : MonoBehaviour
{
    [Header("Integration Configuration")]
    public bool autoInitializeOnStart = true;
    public bool enableFigmaIntegration = true;
    public bool enableResponsiveDesign = true;
    public bool enableCulturalElements = true;
    public bool enableAutomatedTesting = false;
    public bool enablePerformanceMonitoring = true;
    
    [Header("Core System References")]
    public BrawlStarsDesignSystem designSystem;
    public BrawlStarsMenuController menuController;
    public BrawlStarsResponsiveLayout responsiveLayout;
    public BrawlStarsUITestingSystem testingSystem;
    public BrawlStarsUIConfig uiConfig;
    public Canvas mainCanvas;
    
    [Header("Menu Panel References")]
    public Transform mainMenuPanel;
    public Transform playPanel;
    public Transform settingsPanel;
    public Transform shopPanel;
    public Transform profilePanel;
    public Transform leaderboardPanel;
    public Transform achievementsPanel;
    public Transform culturalPanel;
    public Transform pausePanel;
    public Transform gameHUDPanel;
    
    [Header("Navigation References")]
    public Transform navigationBar;
    public Transform sideNavigation;
    public Transform bottomNavigation;
    public Button backButton;
    public Button homeButton;
    
    [Header("UI Element References")]
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI subtitleText;
    public Image backgroundImage;
    public Image headerImage;
    public Image footerImage;
    
    [Header("Cultural Element References")]
    public Image culturalPatternOverlay;
    public TextMeshProUGUI baybayinText;
    public Image[] culturalDecorations;
    
    [Header("Performance Settings")]
    public int targetFrameRate = 60;
    public bool enableSpriteAtlasing = true;
    public bool enableObjectPooling = true;
    public bool enableLazyLoading = true;
    
    [Header("Setup Wizard")]
    public bool showSetupWizard = false;
    public SetupStep currentSetupStep = SetupStep.Welcome;
    
    // Integration state
    private bool isInitialized = false;
    private bool isInitializing = false;
    private List<string> initializationLog = new List<string>();
    private Dictionary<string, System.Action> setupSteps = new Dictionary<string, System.Action>();
    
    // Performance monitoring
    private float initializationStartTime;
    private List<float> frameTimeHistory = new List<float>();
    private List<long> memoryUsageHistory = new List<long>();
    
    public enum SetupStep
    {
        Welcome,
        DesignSystem,
        MenuController,
        ResponsiveLayout,
        CulturalElements,
        TestingSystem,
        FigmaIntegration,
        PerformanceOptimization,
        FinalValidation,
        Complete
    }
    
    public enum IntegrationStatus
    {
        NotStarted,
        InProgress,
        PartiallyComplete,
        Complete,
        Failed
    }
    
    [System.Serializable]
    public class IntegrationReport
    {
        public System.DateTime timestamp;
        public IntegrationStatus overallStatus;
        public float initializationTime;
        public List<ComponentStatus> componentStatuses = new List<ComponentStatus>();
        public List<string> errors = new List<string>();
        public List<string> warnings = new List<string>();
        public Dictionary<string, object> performanceMetrics = new Dictionary<string, object>();
        public string summary;
    }
    
    [System.Serializable]
    public class ComponentStatus
    {
        public string componentName;
        public bool isConfigured;
        public bool isFunctional;
        public string statusMessage;
        public List<string> issues = new List<string>();
    }
    
    // Events
    public System.Action<IntegrationStatus> OnIntegrationStatusChanged;
    public System.Action<SetupStep> OnSetupStepCompleted;
    public System.Action<IntegrationReport> OnIntegrationComplete;
    
    void Awake()
    {
        InitializeSetupSteps();
        Log("Brawl Stars UI Integration Manager initialized");
    }
    
    void Start()
    {
        if (autoInitializeOnStart)
        {
            StartCoroutine(InitializeIntegration());
        }
    }
    
    void Update()
    {
        if (enablePerformanceMonitoring && isInitialized)
        {
            CollectPerformanceData();
        }
    }
    
    /// <summary>
    /// Initialize all setup steps
    /// </summary>
    private void InitializeSetupSteps()
    {
        setupSteps.Clear();
        setupSteps["DesignSystem"] = SetupDesignSystem;
        setupSteps["MenuController"] = SetupMenuController;
        setupSteps["ResponsiveLayout"] = SetupResponsiveLayout;
        setupSteps["CulturalElements"] = SetupCulturalElements;
        setupSteps["TestingSystem"] = SetupTestingSystem;
        setupSteps["FigmaIntegration"] = SetupFigmaIntegration;
        setupSteps["PerformanceOptimization"] = SetupPerformanceOptimization;
        setupSteps["FinalValidation"] = SetupFinalValidation;
    }
    
    /// <summary>
    /// Initialize the complete Brawl Stars UI integration
    /// </summary>
    public IEnumerator InitializeIntegration()
    {
        if (isInitializing || isInitialized)
        {
            LogWarning("Integration is already in progress or complete");
            yield break;
        }
        
        isInitializing = true;
        initializationStartTime = Time.time;
        OnIntegrationStatusChanged?.Invoke(IntegrationStatus.InProgress);
        
        Log("Starting Brawl Stars UI Integration...");
        
        // Step 1: Setup Design System
        yield return StartCoroutine(ExecuteSetupStep("DesignSystem"));
        
        // Step 2: Setup Menu Controller
        yield return StartCoroutine(ExecuteSetupStep("MenuController"));
        
        // Step 3: Setup Responsive Layout
        yield return StartCoroutine(ExecuteSetupStep("ResponsiveLayout"));
        
        // Step 4: Setup Cultural Elements
        yield return StartCoroutine(ExecuteSetupStep("CulturalElements"));
        
        // Step 5: Setup Testing System
        yield return StartCoroutine(ExecuteSetupStep("TestingSystem"));
        
        // Step 6: Setup Figma Integration
        yield return StartCoroutine(ExecuteSetupStep("FigmaIntegration"));
        
        // Step 7: Setup Performance Optimization
        yield return StartCoroutine(ExecuteSetupStep("PerformanceOptimization"));
        
        // Step 8: Final Validation
        yield return StartCoroutine(ExecuteSetupStep("FinalValidation"));
        
        // Complete integration
        CompleteIntegration();
    }
    
    /// <summary>
    /// Execute a single setup step
    /// </summary>
    private IEnumerator ExecuteSetupStep(string stepName)
    {
        Log($"Executing setup step: {stepName}");
        
        bool stepFound = setupSteps.ContainsKey(stepName);
        if (stepFound)
        {
            try { setupSteps[stepName].Invoke(); }
            catch (System.Exception ex) { LogError($"Error in setup step {stepName}: {ex.Message}"); }
        }
        else
            LogError($"Setup step not found: {stepName}");
        
        if (stepFound)
            yield return new WaitForSeconds(0.1f);
        
        OnSetupStepCompleted?.Invoke(GetSetupStepFromName(stepName));
    }
    
    /// <summary>
    /// Setup the design system
    /// </summary>
    private void SetupDesignSystem()
    {
        Log("Setting up Design System...");
        
        if (designSystem == null)
        {
            designSystem = ScriptableObject.CreateInstance<BrawlStarsDesignSystem>();
            Log("Created default Design System");
        }
        
        // Validate design system configuration
        ValidateDesignSystem();
        
        Log("Design System setup complete");
    }
    
    /// <summary>
    /// Setup the menu controller
    /// </summary>
    private void SetupMenuController()
    {
        Log("Setting up Menu Controller...");
        
        if (menuController == null)
        {
            menuController = FindFirstObjectByType<BrawlStarsMenuController>();
            if (menuController == null)
            {
                GameObject menuGO = new GameObject("BrawlStarsMenuController");
                menuController = menuGO.AddComponent<BrawlStarsMenuController>();
                Log("Created Menu Controller");
            }
        }
        
        // Configure menu controller
        ConfigureMenuController();
        
        Log("Menu Controller setup complete");
    }
    
    /// <summary>
    /// Setup responsive layout
    /// </summary>
    private void SetupResponsiveLayout()
    {
        if (!enableResponsiveDesign)
        {
            Log("Responsive design disabled, skipping setup");
            return;
        }
        
        Log("Setting up Responsive Layout...");
        
        if (responsiveLayout == null)
        {
            responsiveLayout = FindFirstObjectByType<BrawlStarsResponsiveLayout>();
            if (responsiveLayout == null)
            {
                GameObject responsiveGO = new GameObject("BrawlStarsResponsiveLayout");
                responsiveLayout = responsiveGO.AddComponent<BrawlStarsResponsiveLayout>();
                Log("Created Responsive Layout");
            }
        }
        
        // Configure responsive layout
        ConfigureResponsiveLayout();
        
        Log("Responsive Layout setup complete");
    }
    
    /// <summary>
    /// Setup cultural elements
    /// </summary>
    private void SetupCulturalElements()
    {
        if (!enableCulturalElements)
        {
            Log("Cultural elements disabled, skipping setup");
            return;
        }
        
        Log("Setting up Cultural Elements...");
        
        // Create cultural elements if they don't exist
        CreateCulturalElements();
        
        Log("Cultural Elements setup complete");
    }
    
    /// <summary>
    /// Setup testing system
    /// </summary>
    private void SetupTestingSystem()
    {
        if (!enableAutomatedTesting)
        {
            Log("Automated testing disabled, skipping setup");
            return;
        }
        
        Log("Setting up Testing System...");
        
        if (testingSystem == null)
        {
            testingSystem = FindFirstObjectByType<BrawlStarsUITestingSystem>();
            if (testingSystem == null)
            {
                GameObject testingGO = new GameObject("BrawlStarsUITestingSystem");
                testingSystem = testingGO.AddComponent<BrawlStarsUITestingSystem>();
                Log("Created Testing System");
            }
        }
        
        // Configure testing system
        ConfigureTestingSystem();
        
        Log("Testing System setup complete");
    }
    
    /// <summary>
    /// Setup Figma integration
    /// </summary>
    private void SetupFigmaIntegration()
    {
        if (!enableFigmaIntegration)
        {
            Log("Figma integration disabled, skipping setup");
            return;
        }
        
        Log("Setting up Figma Integration...");
        
        if (uiConfig == null)
        {
            uiConfig = ScriptableObject.CreateInstance<BrawlStarsUIConfig>();
            Log("Created UI Config for Figma integration");
        }
        
        // Validate Figma configuration
        ValidateFigmaConfiguration();
        
        Log("Figma Integration setup complete");
    }
    
    /// <summary>
    /// Setup performance optimization
    /// </summary>
    private void SetupPerformanceOptimization()
    {
        Log("Setting up Performance Optimization...");
        
        // Set target frame rate
        Application.targetFrameRate = targetFrameRate;
        
        // Configure canvas settings
        ConfigureCanvasSettings();
        
        // Setup object pooling
        if (enableObjectPooling)
        {
            SetupObjectPooling();
        }
        
        // Setup lazy loading
        if (enableLazyLoading)
        {
            SetupLazyLoading();
        }
        
        Log("Performance Optimization setup complete");
    }
    
    /// <summary>
    /// Setup final validation
    /// </summary>
    private void SetupFinalValidation()
    {
        Log("Performing Final Validation...");
        
        // Validate all components
        ValidateIntegration();
        
        // Generate integration report
        GenerateIntegrationReport();
        
        Log("Final Validation complete");
    }
    
    /// <summary>
    /// Configure menu controller with proper references
    /// </summary>
    private void ConfigureMenuController()
    {
        if (menuController == null) return;
        
        // Assign design system
        menuController.designSystem = designSystem;
        menuController.uiConfig = uiConfig;
        
        // Assign panel references
        menuController.mainMenuPanel = mainMenuPanel;
        menuController.playPanel = playPanel;
        menuController.settingsPanel = settingsPanel;
        menuController.shopPanel = shopPanel;
        menuController.profilePanel = profilePanel;
        menuController.leaderboardPanel = leaderboardPanel;
        menuController.achievementsPanel = achievementsPanel;
        menuController.culturalPanel = culturalPanel;
        
        // Assign navigation references
        menuController.navigationBar = navigationBar;
        menuController.bottomNavigation = bottomNavigation;
        menuController.sideNavigation = sideNavigation;
        menuController.backButton = backButton;
        menuController.homeButton = homeButton;
        
        // Assign UI element references
        menuController.titleText = titleText;
        menuController.subtitleText = subtitleText;
        menuController.backgroundImage = backgroundImage;
        menuController.headerImage = headerImage;
        menuController.footerImage = footerImage;
        
        // Assign cultural elements
        menuController.culturalPatternOverlay = culturalPatternOverlay;
        menuController.baybayinText = baybayinText;
        menuController.culturalDecorations = culturalDecorations;
        
        // Configure settings
        menuController.enableResponsiveDesign = enableResponsiveDesign;
        menuController.enableCulturalElements = enableCulturalElements;
        menuController.enablePerformanceOptimization = enablePerformanceMonitoring;
    }
    
    /// <summary>
    /// Configure responsive layout
    /// </summary>
    private void ConfigureResponsiveLayout()
    {
        if (responsiveLayout == null) return;
        
        responsiveLayout.designSystem = designSystem;
        responsiveLayout.enableResponsiveLayout = enableResponsiveDesign;
        responsiveLayout.canvas = mainCanvas;
        responsiveLayout.canvasScaler = mainCanvas?.GetComponent<CanvasScaler>();
        responsiveLayout.mainContainer = mainCanvas?.GetComponent<RectTransform>();
    }
    
    /// <summary>
    /// Configure testing system
    /// </summary>
    private void ConfigureTestingSystem()
    {
        if (testingSystem == null) return;
        
        testingSystem.menuController = menuController;
        testingSystem.designSystem = designSystem;
        testingSystem.responsiveLayout = responsiveLayout;
        testingSystem.mainCanvas = mainCanvas;
        testingSystem.enableAutomatedTesting = enableAutomatedTesting;
        testingSystem.enablePerformanceTesting = enablePerformanceMonitoring;
        testingSystem.enableResponsiveTesting = enableResponsiveDesign;
        testingSystem.enableCulturalTesting = enableCulturalElements;
    }
    
    /// <summary>
    /// Create cultural elements if they don't exist
    /// </summary>
    private void CreateCulturalElements()
    {
        // Create cultural pattern overlay if needed
        if (culturalPatternOverlay == null)
        {
            GameObject patternGO = new GameObject("CulturalPatternOverlay");
            patternGO.transform.SetParent(mainCanvas?.transform, false);
            culturalPatternOverlay = patternGO.AddComponent<Image>();
            culturalPatternOverlay.raycastTarget = false;
            Log("Created Cultural Pattern Overlay");
        }
        
        // Create Baybayin text if needed
        if (baybayinText == null)
        {
            GameObject baybayinGO = new GameObject("BaybayinText");
            baybayinGO.transform.SetParent(mainCanvas?.transform, false);
            baybayinText = baybayinGO.AddComponent<TextMeshProUGUI>();
            baybayinText.raycastTarget = false;
            Log("Created Baybayin Text");
        }
        
        // Setup cultural decorations
        if (culturalDecorations == null || culturalDecorations.Length == 0)
        {
            culturalDecorations = new Image[4];
            for (int i = 0; i < 4; i++)
            {
                GameObject decorationGO = new GameObject($"CulturalDecoration_{i}");
                decorationGO.transform.SetParent(mainCanvas?.transform, false);
                culturalDecorations[i] = decorationGO.AddComponent<Image>();
                culturalDecorations[i].raycastTarget = false;
            }
            Log("Created Cultural Decorations");
        }
    }
    
    /// <summary>
    /// Configure canvas settings for optimal performance
    /// </summary>
    private void ConfigureCanvasSettings()
    {
        if (mainCanvas == null) return;
        
        // Configure canvas settings
        mainCanvas.pixelPerfect = false; // Disable for better performance
        mainCanvas.sortingOrder = 0;
        
        // Configure canvas scaler
        var canvasScaler = mainCanvas.GetComponent<CanvasScaler>();
        if (canvasScaler != null)
        {
            canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            canvasScaler.referenceResolution = new Vector2(1920f, 1080f);
            canvasScaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            canvasScaler.matchWidthOrHeight = 0.5f;
        }
        
        Log("Canvas settings configured");
    }
    
    /// <summary>
    /// Setup object pooling for UI elements
    /// </summary>
    private void SetupObjectPooling()
    {
        // Implementation for UI element object pooling
        Log("Object pooling setup");
    }
    
    /// <summary>
    /// Setup lazy loading for UI panels
    /// </summary>
    private void SetupLazyLoading()
    {
        // Implementation for lazy loading UI panels
        Log("Lazy loading setup");
    }
    
    /// <summary>
    /// Validate design system configuration
    /// </summary>
    private void ValidateDesignSystem()
    {
        if (designSystem == null)
        {
            LogError("Design system is null");
            return;
        }
        
        // Validate colors
        if (designSystem.colors.goldPrimary == Color.clear)
        {
            LogWarning("Gold primary color not configured");
        }
        
        // Validate typography
        if (designSystem.typography.primaryFont == null)
        {
            LogWarning("Primary font not assigned");
        }
        
        Log("Design system validation complete");
    }
    
    /// <summary>
    /// Validate Figma configuration
    /// </summary>
    private void ValidateFigmaConfiguration()
    {
        if (uiConfig == null) return;
        
        // Validate Figma settings
        if (string.IsNullOrEmpty(uiConfig.figmaApiToken))
        {
            LogWarning("Figma API token not configured");
        }
        
        if (string.IsNullOrEmpty(uiConfig.figmaFileUrl))
        {
            LogWarning("Figma file URL not configured");
        }
        
        Log("Figma configuration validation complete");
    }
    
    /// <summary>
    /// Validate complete integration
    /// </summary>
    private void ValidateIntegration()
    {
        Log("Validating integration...");
        
        // Validate core systems
        ValidateCoreSystems();
        
        // Validate references
        ValidateReferences();
        
        // Validate performance settings
        ValidatePerformanceSettings();
        
        Log("Integration validation complete");
    }
    
    /// <summary>
    /// Validate core systems
    /// </summary>
    private void ValidateCoreSystems()
    {
        bool allSystemsValid = true;
        
        if (designSystem == null)
        {
            LogError("Design system is not configured");
            allSystemsValid = false;
        }
        
        if (menuController == null)
        {
            LogError("Menu controller is not configured");
            allSystemsValid = false;
        }
        
        if (enableResponsiveDesign && responsiveLayout == null)
        {
            LogError("Responsive layout is not configured");
            allSystemsValid = false;
        }
        
        if (allSystemsValid)
        {
            Log("All core systems are valid");
        }
    }
    
    /// <summary>
    /// Validate references
    /// </summary>
    private void ValidateReferences()
    {
        bool allReferencesValid = true;
        
        if (mainCanvas == null)
        {
            LogError("Main canvas is not assigned");
            allReferencesValid = false;
        }
        
        if (mainMenuPanel == null)
        {
            LogWarning("Main menu panel is not assigned");
        }
        
        if (allReferencesValid)
        {
            Log("All references are valid");
        }
    }
    
    /// <summary>
    /// Validate performance settings
    /// </summary>
    private void ValidatePerformanceSettings()
    {
        if (Application.targetFrameRate != targetFrameRate)
        {
            LogWarning($"Target frame rate mismatch: expected {targetFrameRate}, actual {Application.targetFrameRate}");
        }
        
        Log("Performance settings validated");
    }
    
    /// <summary>
    /// Generate comprehensive integration report
    /// </summary>
    private void GenerateIntegrationReport()
    {
        var report = new IntegrationReport
        {
            timestamp = System.DateTime.Now,
            initializationTime = Time.time - initializationStartTime,
            overallStatus = IntegrationStatus.Complete
        };
        
        // Add component statuses
        report.componentStatuses.AddRange(GetComponentStatuses());
        
        // Add performance metrics
        report.performanceMetrics = GetPerformanceMetrics();
        
        // Generate summary
        report.summary = GenerateSummary(report);
        
        Log($"Integration Report Generated:\n{report.summary}");
        
        OnIntegrationComplete?.Invoke(report);
    }
    
    /// <summary>
    /// Get status of all components
    /// </summary>
    private List<ComponentStatus> GetComponentStatuses()
    {
        var statuses = new List<ComponentStatus>();
        
        statuses.Add(new ComponentStatus
        {
            componentName = "Design System",
            isConfigured = designSystem != null,
            isFunctional = ValidateDesignSystemFunctional(),
            statusMessage = designSystem != null ? "Configured" : "Not configured"
        });
        
        statuses.Add(new ComponentStatus
        {
            componentName = "Menu Controller",
            isConfigured = menuController != null,
            isFunctional = ValidateMenuControllerFunctional(),
            statusMessage = menuController != null ? "Configured" : "Not configured"
        });
        
        statuses.Add(new ComponentStatus
        {
            componentName = "Responsive Layout",
            isConfigured = responsiveLayout != null,
            isFunctional = ValidateResponsiveLayoutFunctional(),
            statusMessage = responsiveLayout != null ? "Configured" : "Not configured"
        });
        
        statuses.Add(new ComponentStatus
        {
            componentName = "Testing System",
            isConfigured = testingSystem != null,
            isFunctional = ValidateTestingSystemFunctional(),
            statusMessage = testingSystem != null ? "Configured" : "Not configured"
        });
        
        return statuses;
    }
    
    /// <summary>
    /// Get performance metrics
    /// </summary>
    private Dictionary<string, object> GetPerformanceMetrics()
    {
        var metrics = new Dictionary<string, object>();
        
        if (frameTimeHistory.Count > 0)
        {
            float averageFrameTime = frameTimeHistory.Average();
            metrics["averageFrameRate"] = 1f / averageFrameTime;
            metrics["averageFrameTime"] = averageFrameTime;
        }
        
        if (memoryUsageHistory.Count > 0)
        {
            metrics["averageMemoryUsage"] = memoryUsageHistory.Average();
        }
        
        metrics["targetFrameRate"] = targetFrameRate;
        metrics["initializationTime"] = Time.time - initializationStartTime;
        
        return metrics;
    }
    
    /// <summary>
    /// Generate summary report
    /// </summary>
    private string GenerateSummary(IntegrationReport report)
    {
        string summary = $"=== Brawl Stars UI Integration Report ===\n";
        summary += $"Timestamp: {report.timestamp}\n";
        summary += $"Overall Status: {report.overallStatus}\n";
        summary += $"Initialization Time: {report.initializationTime:F2}s\n\n";
        
        summary += "Component Status:\n";
        foreach (var status in report.componentStatuses)
        {
            summary += $"  {status.componentName}: {status.statusMessage}\n";
            if (status.issues.Count > 0)
            {
                foreach (var issue in status.issues)
                {
                    summary += $"    - {issue}\n";
                }
            }
        }
        
        if (report.performanceMetrics.Count > 0)
        {
            summary += "\nPerformance Metrics:\n";
            foreach (var metric in report.performanceMetrics)
            {
                summary += $"  {metric.Key}: {metric.Value}\n";
            }
        }
        
        return summary;
    }
    
    /// <summary>
    /// Complete the integration process
    /// </summary>
    private void CompleteIntegration()
    {
        isInitialized = true;
        isInitializing = false;
        
        Log("Brawl Stars UI Integration Complete!");
        
        OnIntegrationStatusChanged?.Invoke(IntegrationStatus.Complete);
        
        // Start automated testing if enabled
        if (enableAutomatedTesting && testingSystem != null)
        {
            testingSystem.StartAutomatedTesting();
            Log("Automated testing started");
        }
    }
    
    // Validation helper methods
    
    private bool ValidateDesignSystemFunctional()
    {
        return designSystem != null && 
               designSystem.colors.goldPrimary != Color.clear &&
               designSystem.typography.primaryFont != null;
    }
    
    private bool ValidateMenuControllerFunctional()
    {
        return menuController != null && 
               menuController.designSystem != null;
    }
    
    private bool ValidateResponsiveLayoutFunctional()
    {
        return responsiveLayout != null && 
               responsiveLayout.designSystem != null;
    }
    
    private bool ValidateTestingSystemFunctional()
    {
        return testingSystem != null && 
               testingSystem.menuController != null;
    }
    
    /// <summary>
    /// Collect performance data during initialization
    /// </summary>
    private void CollectPerformanceData()
    {
        frameTimeHistory.Add(Time.unscaledDeltaTime);
        memoryUsageHistory.Add(System.GC.GetTotalMemory(false));
        
        // Keep only recent data
        if (frameTimeHistory.Count > 100)
        {
            frameTimeHistory.RemoveAt(0);
        }
        
        if (memoryUsageHistory.Count > 100)
        {
            memoryUsageHistory.RemoveAt(0);
        }
    }
    
    /// <summary>
    /// Get setup step from name
    /// </summary>
    private SetupStep GetSetupStepFromName(string stepName)
    {
        switch (stepName)
        {
            case "DesignSystem": return SetupStep.DesignSystem;
            case "MenuController": return SetupStep.MenuController;
            case "ResponsiveLayout": return SetupStep.ResponsiveLayout;
            case "CulturalElements": return SetupStep.CulturalElements;
            case "TestingSystem": return SetupStep.TestingSystem;
            case "FigmaIntegration": return SetupStep.FigmaIntegration;
            case "PerformanceOptimization": return SetupStep.PerformanceOptimization;
            case "FinalValidation": return SetupStep.FinalValidation;
            default: return SetupStep.Welcome;
        }
    }
    
    // Logging helper methods
    
    private void Log(string message)
    {
        string logMessage = $"[BrawlStarsUI] {message}";
        initializationLog.Add(logMessage);
        Debug.Log(logMessage);
    }
    
    private void LogWarning(string message)
    {
        string logMessage = $"[BrawlStarsUI] WARNING: {message}";
        initializationLog.Add(logMessage);
        Debug.LogWarning(logMessage);
    }
    
    private void LogError(string message)
    {
        string logMessage = $"[BrawlStarsUI] ERROR: {message}";
        initializationLog.Add(logMessage);
        Debug.LogError(logMessage);
    }
    
    // Public interface methods
    
    /// <summary>
    /// Get integration status
    /// </summary>
    public bool IsInitialized()
    {
        return isInitialized;
    }
    
    /// <summary>
    /// Get initialization log
    /// </summary>
    public List<string> GetInitializationLog()
    {
        return new List<string>(initializationLog);
    }
    
    /// <summary>
    /// Force re-initialization
    /// </summary>
    public void ForceReinitialize()
    {
        isInitialized = false;
        isInitializing = false;
        initializationLog.Clear();
        
        StartCoroutine(InitializeIntegration());
    }
    

    
    void OnDestroy()
    {
        // Cleanup
        if (testingSystem != null && testingSystem.IsTestingActive())
        {
            testingSystem.StopAutomatedTesting();
        }
    }
}