using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/// <summary>
/// Brawl Stars UI System Validator - comprehensive validation and testing
/// for the complete Figma-to-Unity integration pipeline.
/// </summary>
public class BrawlStarsUIValidator : MonoBehaviour
{
    [Header("Validation Configuration")]
    [Tooltip("Enable automatic validation on start")]
    public bool autoValidateOnStart = true;
    
    [Tooltip("Enable detailed logging")]
    public bool enableDetailedLogging = true;
    
    [Tooltip("Stop on first critical error")]
    public bool stopOnCriticalError = true;
    
    [Tooltip("Test Figma connection")]
    public bool testFigmaConnection = true;
    
    [Tooltip("Test UI components")]
    public bool testUIComponents = true;
    
    [Tooltip("Test multi-device support")]
    public bool testMultiDeviceSupport = true;
    
    [Tooltip("Test performance targets")]
    public bool testPerformanceTargets = true;
    
    [Header("Validation Results")]
    public ValidationResults results;
    
    [System.Serializable]
    public class ValidationResults
    {
        public bool overallSuccess;
        public List<ValidationTest> tests = new List<ValidationTest>();
        public Dictionary<string, bool> componentStatus = new Dictionary<string, bool>();
        public string summaryReport;
        public System.DateTime validationDate;
        
        public ValidationResults()
        {
            overallSuccess = false;
            tests = new List<ValidationTest>();
            componentStatus = new Dictionary<string, bool>();
            validationDate = System.DateTime.Now;
        }
    }
    
    [System.Serializable]
    public class ValidationTest
    {
        public string testName;
        public bool passed;
        public string status;
        public string errorMessage;
        public string details;
        public float executionTime;
        public TestSeverity severity;
        
        public enum TestSeverity
        {
            Critical,
            Warning,
            Info
        }
        
        public ValidationTest(string name)
        {
            testName = name;
            passed = false;
            status = "Not Run";
            severity = TestSeverity.Info;
        }
    }
    
    void Start()
    {
        if (autoValidateOnStart)
        {
            StartCoroutine(RunCompleteValidation());
        }
    }
    
    /// <summary>
    /// Run complete validation of the Brawl Stars UI system
    /// </summary>
    public IEnumerator RunCompleteValidation()
    {
        LogHeader("Brawl Stars UI System Validation Starting...");
        
        results = new ValidationResults();
        float startTime = Time.time;
        
        // Test 1: Configuration Validation
        yield return StartCoroutine(ValidateConfiguration());
        
        // Test 2: Figma Integration
        if (testFigmaConnection)
        {
            yield return StartCoroutine(ValidateFigmaIntegration());
        }
        
        // Test 3: UI Components
        if (testUIComponents)
        {
            yield return StartCoroutine(ValidateUIComponents());
        }
        
        // Test 4: Multi-Device Support
        if (testMultiDeviceSupport)
        {
            yield return StartCoroutine(ValidateMultiDeviceSupport());
        }
        
        // Test 5: Performance Targets
        if (testPerformanceTargets)
        {
            yield return StartCoroutine(ValidatePerformanceTargets());
        }
        
        // Generate final report
        GenerateFinalReport();
        
        float totalTime = Time.time - startTime;
        LogHeader($"Validation Complete! Total Time: {totalTime:F2}s");
        
        yield return results;
    }
    
    /// <summary>
    /// Validate system configuration
    /// </summary>
    private IEnumerator ValidateConfiguration()
    {
        var test = new ValidationTest("Configuration Validation");
        float startTime = Time.time;
        
        LogTestStart(test.testName);
        
        try
        {
            // Check UI Config
            var uiConfig = FindFirstObjectByType<BrawlStarsUIConfig>();
            if (uiConfig == null)
            {
                test.errorMessage = "BrawlStarsUIConfig not found";
                test.severity = ValidationTest.TestSeverity.Critical;
                test.passed = false;
            }
            else
            {
                bool configValid = uiConfig.ValidateConfiguration();
                if (configValid)
                {
                    test.passed = true;
                    test.status = "Configuration Valid";
                }
                else
                {
                    test.errorMessage = "Configuration validation failed";
                    test.severity = ValidationTest.TestSeverity.Critical;
                    test.passed = false;
                }
            }
            
            // Check Community Config
            var communityConfig = FindFirstObjectByType<BrawlStarsCommunityUIConfig>();
            if (communityConfig != null)
            {
                bool communityValid = communityConfig.ValidateConfiguration();
                if (communityValid)
                {
                    test.details += "Community UI Config: Valid\n";
                }
                else
                {
                    test.details += "Community UI Config: Invalid\n";
                }
            }
        }
        catch (System.Exception e)
        {
            test.errorMessage = $"Configuration validation error: {e.Message}";
            test.severity = ValidationTest.TestSeverity.Critical;
            test.passed = false;
        }
        
        test.executionTime = Time.time - startTime;
        results.tests.Add(test);
        
        LogTestResult(test);
        
        if (!test.passed && stopOnCriticalError && test.severity == ValidationTest.TestSeverity.Critical)
        {
            LogError("Critical configuration error - stopping validation");
            yield break;
        }
        
        yield return null;
    }
    
    /// <summary>
    /// Validate Figma integration
    /// </summary>
    private IEnumerator ValidateFigmaIntegration()
    {
        var test = new ValidationTest("Figma Integration");
        float startTime = Time.time;
        bool connectionReady = false;
        BrawlStarsFigmaAPI figmaAPI = null;
        
        LogTestStart(test.testName);
        
        try
        {
            figmaAPI = FindFirstObjectByType<BrawlStarsFigmaAPI>();
            if (figmaAPI == null)
            {
                test.errorMessage = "BrawlStarsFigmaAPI not found";
                test.severity = ValidationTest.TestSeverity.Warning;
                test.passed = false;
                test.details = "Figma integration will use simulated data";
            }
            else
            {
                figmaAPI.OnConnectionTestComplete += (success, message) => {
                    connectionReady = true;
                    if (success) { test.passed = true; test.status = "Figma API Connected"; test.details = message; }
                    else { test.passed = false; test.status = "Figma API Connection Failed"; test.errorMessage = message; test.severity = ValidationTest.TestSeverity.Warning; }
                };
                figmaAPI.TestFigmaConnection();
            }
            
            var figmaImporter = FindFirstObjectByType<BrawlStarsFigmaImporter>();
            if (figmaImporter == null) test.details += "\nFigma Importer: Not Found";
            else { test.details += "\nFigma Importer: Found"; test.details += figmaImporter.uiConfig != null ? "\nUI Config: Assigned" : "\nUI Config: Not Assigned"; }
        }
        catch (System.Exception e)
        {
            test.errorMessage = $"Figma integration validation error: {e.Message}";
            test.severity = ValidationTest.TestSeverity.Warning;
            test.passed = false;
        }
        
        if (figmaAPI != null)
        {
            float timeout = 10f;
            float waitStart = Time.time;
            while (!connectionReady && Time.time - waitStart < timeout)
                yield return null;
            if (!connectionReady) { test.errorMessage = "Figma connection test timeout"; test.severity = ValidationTest.TestSeverity.Warning; test.passed = false; }
        }
        
        test.executionTime = Time.time - startTime;
        results.tests.Add(test);
        LogTestResult(test);
        yield return null;
    }
    
    /// <summary>
    /// Validate UI components
    /// </summary>
    private IEnumerator ValidateUIComponents()
    {
        var test = new ValidationTest("UI Components");
        float startTime = Time.time;
        
        LogTestStart(test.testName);
        
        try
        {
            // Check UI Controller
            var uiController = FindFirstObjectByType<BrawlStarsUIController>();
            if (uiController == null)
            {
                test.errorMessage = "BrawlStarsUIController not found";
                test.severity = ValidationTest.TestSeverity.Critical;
                test.passed = false;
            }
            else
            {
                bool uiValid = uiController.ValidateUI();
                if (uiValid)
                {
                    test.passed = true;
                    test.status = "UI Controller Valid";
                }
                else
                {
                    test.errorMessage = "UI Controller validation failed";
                    test.severity = ValidationTest.TestSeverity.Critical;
                    test.passed = false;
                }
            }
            
            // Check Interactive UI Manager
            var interactiveManager = FindFirstObjectByType<BrawlStarsInteractiveUIManager>();
            if (interactiveManager == null)
            {
                test.details += "\nInteractive UI Manager: Not Found";
            }
            else
            {
                test.details += "\nInteractive UI Manager: Found";
                var metrics = interactiveManager.GetPerformanceMetrics();
                test.details += $"\nTotal Elements: {metrics["Total_Elements"]}";
                test.details += $"\nPooled Objects: {metrics["Pooled_Objects"]}";
            }
            
            // Check Canvas Manager
            var canvasManager = FindFirstObjectByType<BrawlStarsCanvasManager>();
            if (canvasManager == null)
            {
                test.details += "\nCanvas Manager: Not Found";
            }
            else
            {
                test.details += "\nCanvas Manager: Found";
                bool canvasValid = canvasManager.ValidateConfiguration();
                test.details += $"\nCanvas Config: {(canvasValid ? "Valid" : "Invalid")}";
                
                var deviceMetrics = canvasManager.GetPerformanceMetrics();
                test.details += $"\nCurrent Device: {canvasManager.GetCurrentDeviceType()}";
                test.details += $"\nScreen Size: {deviceMetrics["Screen_Width"]:F0}x{deviceMetrics["Screen_Height"]:F0}";
            }
        }
        catch (System.Exception e)
        {
            test.errorMessage = $"UI components validation error: {e.Message}";
            test.severity = ValidationTest.TestSeverity.Critical;
            test.passed = false;
        }
        
        test.executionTime = Time.time - startTime;
        results.tests.Add(test);
        
        LogTestResult(test);
        
        yield return null;
    }
    
    /// <summary>
    /// Validate multi-device support
    /// </summary>
    private IEnumerator ValidateMultiDeviceSupport()
    {
        var test = new ValidationTest("Multi-Device Support");
        float startTime = Time.time;
        
        LogTestStart(test.testName);
        
        try
        {
            var canvasManager = FindFirstObjectByType<BrawlStarsCanvasManager>();
            if (canvasManager == null)
            {
                test.errorMessage = "Canvas Manager not found for multi-device testing";
                test.severity = ValidationTest.TestSeverity.Warning;
                test.passed = false;
            }
            else
            {
                // Test device detection
                var deviceType = canvasManager.GetCurrentDeviceType();
                var screenSize = canvasManager.GetCurrentScreenSize();
                var aspectRatio = canvasManager.GetCurrentAspectRatio();
                
                test.details = $"Device Type: {deviceType}\n";
                test.details += $"Screen Size: {screenSize.x:F0}x{screenSize.y:F0}\n";
                test.details += $"Aspect Ratio: {aspectRatio:F2}\n";
                
                // Validate device type detection
                if (deviceType != BrawlStarsCanvasManager.DeviceType.Unknown)
                {
                    test.passed = true;
                    test.status = $"Device Detected: {deviceType}";
                }
                else
                {
                    test.errorMessage = "Unknown device type detected";
                    test.severity = ValidationTest.TestSeverity.Warning;
                    test.passed = false;
                }
                
                // Test configuration
                var config = canvasManager.GetCurrentConfiguration();
                if (config != null)
                {
                    test.details += $"Reference Resolution: {config.referenceResolution.x:F0}x{config.referenceResolution.y:F0}\n";
                    test.details += $"Match Ratio: {config.matchWidthOrHeight:F2}\n";
                }
            }
        }
        catch (System.Exception e)
        {
            test.errorMessage = $"Multi-device validation error: {e.Message}";
            test.severity = ValidationTest.TestSeverity.Warning;
            test.passed = false;
        }
        
        test.executionTime = Time.time - startTime;
        results.tests.Add(test);
        
        LogTestResult(test);
        
        yield return null;
    }
    
    /// <summary>
    /// Validate performance targets
    /// </summary>
    private IEnumerator ValidatePerformanceTargets()
    {
        var test = new ValidationTest("Performance Targets");
        float startTime = Time.time;
        List<float> frameRates = new List<float>();
        
        LogTestStart(test.testName);
        
        float monitorDuration = 2f;
        float monitorStart = Time.time;
        while (Time.time - monitorStart < monitorDuration)
        {
            float currentFPS = 1f / Time.deltaTime;
            frameRates.Add(currentFPS);
            yield return null;
        }
        
        try
        {
            float totalFPS = 0f;
            foreach (float fps in frameRates)
                totalFPS += fps;
            float averageFPS = frameRates.Count > 0 ? totalFPS / frameRates.Count : 0f;
            long totalMemory = UnityEngine.Profiling.Profiler.GetTotalAllocatedMemoryLong();
            float memoryMB = totalMemory / (1024f * 1024f);
            
            test.details = $"Average FPS: {averageFPS:F1}\nMemory Usage: {memoryMB:F1} MB\nFrame Count: {frameRates.Count}\n";
            bool fpsTargetMet = averageFPS >= 55f;
            bool memoryTargetMet = memoryMB < 100f;
            if (fpsTargetMet && memoryTargetMet) { test.passed = true; test.status = "Performance Targets Met"; }
            else
            {
                test.passed = false;
                test.status = "Performance Targets Not Met";
                if (!fpsTargetMet) test.errorMessage += $"FPS target not met (target: 60, actual: {averageFPS:F1}) ";
                if (!memoryTargetMet) test.errorMessage += $"Memory target exceeded (target: 100MB, actual: {memoryMB:F1}MB) ";
                test.severity = ValidationTest.TestSeverity.Warning;
            }
        }
        catch (System.Exception e)
        {
            test.errorMessage = $"Performance validation error: {e.Message}";
            test.severity = ValidationTest.TestSeverity.Warning;
            test.passed = false;
        }
        
        test.executionTime = Time.time - startTime;
        results.tests.Add(test);
        LogTestResult(test);
        yield return null;
    }
    
    /// <summary>
    /// Generate final validation report
    /// </summary>
    private void GenerateFinalReport()
    {
        int passedTests = 0;
        int failedTests = 0;
        int criticalErrors = 0;
        int warnings = 0;
        
        foreach (var test in results.tests)
        {
            if (test.passed)
            {
                passedTests++;
            }
            else
            {
                failedTests++;
                if (test.severity == ValidationTest.TestSeverity.Critical)
                {
                    criticalErrors++;
                }
                else if (test.severity == ValidationTest.TestSeverity.Warning)
                {
                    warnings++;
                }
            }
        }
        
        results.overallSuccess = criticalErrors == 0;
        
        StringBuilder report = new StringBuilder();
        report.AppendLine("=== BRAWL STARS UI SYSTEM VALIDATION REPORT ===");
        report.AppendLine($"Generated: {results.validationDate:yyyy-MM-dd HH:mm:ss}");
        report.AppendLine($"Overall Status: {(results.overallSuccess ? "✅ PASSED" : "❌ FAILED")}");
        report.AppendLine($"Tests Passed: {passedTests}/{results.tests.Count}");
        report.AppendLine($"Critical Errors: {criticalErrors}");
        report.AppendLine($"Warnings: {warnings}");
        report.AppendLine();
        
        // Detailed results
        report.AppendLine("DETAILED RESULTS:");
        foreach (var test in results.tests)
        {
            string statusIcon = test.passed ? "✅" : (test.severity == ValidationTest.TestSeverity.Critical ? "❌" : "⚠️");
            report.AppendLine($"{statusIcon} {test.testName}: {(test.passed ? "PASSED" : "FAILED")} ({test.executionTime:F2}s)");
            if (!string.IsNullOrEmpty(test.errorMessage))
            {
                report.AppendLine($"    Error: {test.errorMessage}");
            }
            if (!string.IsNullOrEmpty(test.details))
            {
                report.AppendLine($"    Details: {test.details}");
            }
        }
        
        // Recommendations
        if (criticalErrors > 0 || warnings > 0)
        {
            report.AppendLine();
            report.AppendLine("RECOMMENDATIONS:");
            
            if (criticalErrors > 0)
            {
                report.AppendLine("🔴 CRITICAL: Fix critical errors before proceeding");
            }
            if (warnings > 0)
            {
                report.AppendLine("🟡 WARNINGS: Address warnings for optimal performance");
            }
        }
        
        results.summaryReport = report.ToString();
        
        LogHeader("VALIDATION REPORT");
        Debug.Log(results.summaryReport);
    }
    
    #region Logging Methods
    
    private void LogHeader(string message)
    {
        if (enableDetailedLogging)
        {
            Debug.Log($"\n=== {message} ===\n");
        }
    }
    
    private void LogTestStart(string testName)
    {
        if (enableDetailedLogging)
        {
            Debug.Log($"🔄 Starting: {testName}");
        }
    }
    
    private void LogTestResult(ValidationTest test)
    {
        if (enableDetailedLogging)
        {
            string statusIcon = test.passed ? "✅" : (test.severity == ValidationTest.TestSeverity.Critical ? "❌" : "⚠️");
            string status = test.passed ? "PASSED" : "FAILED";
            Debug.Log($"{statusIcon} {test.testName}: {status} ({test.executionTime:F2}s)");
            
            if (!string.IsNullOrEmpty(test.errorMessage))
            {
                Debug.LogWarning($"   Error: {test.errorMessage}");
            }
            if (!string.IsNullOrEmpty(test.details))
            {
                Debug.Log($"   Details: {test.details}");
            }
        }
    }
    
    private void LogError(string message)
    {
        Debug.LogError($"❌ {message}");
    }
    
    #endregion
    
    #region Public Methods
    
    /// <summary>
    /// Get validation results
    /// </summary>
    public ValidationResults GetValidationResults()
    {
        return results;
    }
    
    /// <summary>
    /// Check if validation was successful
    /// </summary>
    public bool IsValidationSuccessful()
    {
        return results != null && results.overallSuccess;
    }
    
    /// <summary>
    /// Get specific test result
    /// </summary>
    public ValidationTest GetTestResult(string testName)
    {
        return results.tests.FirstOrDefault(t => t.testName == testName);
    }
    
    /// <summary>
    /// Force revalidation
    /// </summary>
    public void ForceRevalidation()
    {
        StartCoroutine(RunCompleteValidation());
    }
    
    #endregion
    
    void OnDestroy()
    {
        // Cleanup
        if (results != null)
        {
            results.tests.Clear();
            results.componentStatus.Clear();
        }
    }
}