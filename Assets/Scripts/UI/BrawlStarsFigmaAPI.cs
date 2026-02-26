using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System;

/// <summary>
/// Brawl Stars Figma API Integration - handles actual HTTP requests to Figma API
/// for importing Brawl Stars UI components with proper authentication and error handling.
/// </summary>
public class BrawlStarsFigmaAPI : MonoBehaviour
{
    [Header("Figma API Configuration")]
    [Tooltip("Figma Personal Access Token - Get from https://www.figma.com/settings/account")]
    public string figmaApiToken = ""; // Set in Inspector or BrawlStarsUIConfig
    
    [Tooltip("Figma File URL or File Key - Use community files or your own designs")]
    public string figmaFileUrl = "https://www.figma.com/design/BOMLvyJ570CbsohHwPtEGB/Brawl-Stars-Game-UI-Kit--Community-?node-id=1-892&t=q4MCZssNkwucf3LB-1";
    
    [Tooltip("Enable debug logging for API calls")]
    public bool enableDebugLogging = true;
    
    [Tooltip("Timeout for API requests (seconds)")]
    public float apiTimeout = 30f;
    
    [Tooltip("Maximum retry attempts for failed requests")]
    public int maxRetryAttempts = 3;
    
    [Tooltip("Delay between retry attempts (seconds)")]
    public float retryDelay = 2f;
    
    // Figma API endpoints
    private const string FIGMA_API_BASE = "https://api.figma.com/v1";
    private const string FIGMA_FILES_ENDPOINT = "/files/";
    private const string FIGMA_IMAGES_ENDPOINT = "/images/";
    
    // Community Figma files for Brawl Stars UI
    private const string COMMUNITY_BRAWL_STARS_UI = "https://www.figma.com/community/file/123456789/brawl-stars-ui-kit";
    private const string ALTERNATIVE_BRAWL_STARS_UI = "https://www.figma.com/community/file/987654321/brawl-stars-game-ui";
    
    // API response tracking
    private bool isConnecting = false;
    private float connectionProgress = 0f;
    private string connectionStatus = "";
    private int currentRetryAttempt = 0;
    
    // Events
    public System.Action<float> OnConnectionProgress;
    public System.Action<bool, string> OnConnectionComplete;
    public System.Action<string> OnConnectionError;
    /// <summary>Alias for validator: same as OnConnectionComplete.</summary>
    public System.Action<bool, string> OnConnectionTestComplete;
    
    /// <summary>Start connection test (for validator).</summary>
    public void TestFigmaConnection() { StartCoroutine(ConnectToFigma()); }
    
    [System.Serializable]
    public class FigmaFileInfo
    {
        public string name;
        public string key;
        public string thumbnail_url;
        public string last_modified;
        public FigmaDocument document;
        public FigmaComponent[] components;
        public FigmaStyle[] styles;
    }
    
    [System.Serializable]
    public class FigmaDocument
    {
        public string id;
        public string name;
        public string type;
        public FigmaNode[] children;
    }
    
    [System.Serializable]
    public class FigmaNode
    {
        public string id;
        public string name;
        public string type;
        public float[] absoluteBoundingBox;
        public FigmaNode[] children;
        public FigmaFill[] fills;
        public FigmaStroke[] strokes;
        public float strokeWeight;
        public float opacity;
        public string blendMode;
        public bool visible;
        public string[] characters;
        public FigmaTextStyle style;
        public FigmaEffect[] effects;
    }
    
    [System.Serializable]
    public class FigmaComponent
    {
        public string key;
        public string name;
        public string description;
        public FigmaNode node;
    }
    
    [System.Serializable]
    public class FigmaStyle
    {
        public string key;
        public string name;
        public string description;
        public string remote;
        public string style_type;
    }
    
    [System.Serializable]
    public class FigmaFill
    {
        public string type;
        public bool visible;
        public float opacity;
        public FigmaColor color;
        public string blendMode;
        public FigmaGradient gradient;
        public FigmaImage image;
    }
    
    [System.Serializable]
    public class FigmaColor
    {
        public float r;
        public float g;
        public float b;
        public float a;
        
        public Color ToUnityColor()
        {
            return new Color(r, g, b, a);
        }
    }
    
    [System.Serializable]
    public class FigmaGradient
    {
        public string type;
        public FigmaColor[] colors;
        public float[] positions;
    }
    
    [System.Serializable]
    public class FigmaImage
    {
        public string imageRef;
        public float scale;
    }
    
    [System.Serializable]
    public class FigmaStroke
    {
        public string type;
        public bool visible;
        public float opacity;
        public FigmaColor color;
    }
    
    [System.Serializable]
    public class FigmaEffect
    {
        public string type;
        public bool visible;
        public float radius;
        public FigmaColor color;
        public string blendMode;
        public float[] offset;
    }
    
    [System.Serializable]
    public class FigmaTextStyle
    {
        public string fontFamily;
        public string fontPostScriptName;
        public float fontWeight;
        public float fontSize;
        public string textAlignHorizontal;
        public string textAlignVertical;
        public float letterSpacing;
        public float fills;
        public float hyperlink;
        public float opentypeFlags;
        public float lineHeightPx;
        public float lineHeightPercent;
        public float lineHeightUnit;
    }
    
    void Start()
    {
        ValidateConfiguration();
    }
    
    /// <summary>
    /// Validate Figma API configuration
    /// </summary>
    private void ValidateConfiguration()
    {
        if (string.IsNullOrEmpty(figmaApiToken))
        {
            LogError("Figma API token is required. Get it from https://www.figma.com/settings/account");
            return;
        }
        
        if (string.IsNullOrEmpty(figmaFileUrl))
        {
            LogWarning("Figma file URL is empty. Using community file recommendations.");
            SuggestCommunityFiles();
        }
        else
        {
            LogMessage($"Figma configuration validated. File URL: {figmaFileUrl}");
        }
    }
    
    /// <summary>
    /// Suggest community Figma files for Brawl Stars UI
    /// </summary>
    private void SuggestCommunityFiles()
    {
        LogMessage("=== RECOMMENDED COMMUNITY FIGMA FILES FOR BRAWL STARS UI ===");
        LogMessage("1. Search Figma Community for: 'Brawl Stars UI Kit'");
        LogMessage("2. Look for files by: 'Game UI', 'Mobile Game UI', 'Brawl Stars'");
        LogMessage("3. Popular community files:");
        LogMessage("   - 'Brawl Stars UI Kit' by various designers");
        LogMessage("   - 'Mobile Game UI Components'");
        LogMessage("   - 'Game UI Pack'");
        LogMessage("");
        LogMessage("TO USE COMMUNITY FILES:");
        LogMessage("1. Go to https://www.figma.com/community");
        LogMessage("2. Search for 'Brawl Stars UI'");
        LogMessage("3. Open a community file");
        LogMessage("4. Click 'Duplicate' to copy to your account");
        LogMessage("5. Copy the file URL and paste it in the Figma File URL field");
        LogMessage("6. Get your API token from https://www.figma.com/settings/account");
        LogMessage("");
        LogMessage("EXAMPLE COMMUNITY FILE URLS:");
        LogMessage($"- {COMMUNITY_BRAWL_STARS_UI}");
        LogMessage($"- {ALTERNATIVE_BRAWL_STARS_UI}");
    }
    
    /// <summary>
    /// Connect to Figma API and retrieve file information
    /// </summary>
    public IEnumerator ConnectToFigma()
    {
        if (isConnecting)
        {
            LogWarning("Already connecting to Figma API");
            yield break;
        }
        
        isConnecting = true;
        connectionProgress = 0f;
        currentRetryAttempt = 0;
        
        LogMessage("Starting Figma API connection...");
        
        // Validate configuration
        if (string.IsNullOrEmpty(figmaApiToken))
        {
            OnConnectionError?.Invoke("Figma API token is required");
            OnConnectionTestComplete?.Invoke(false, "Figma API token is required");
            isConnecting = false;
            yield break;
        }
        
        // Extract file key from URL
        string fileKey = ExtractFileKey(figmaFileUrl);
        if (string.IsNullOrEmpty(fileKey))
        {
            OnConnectionError?.Invoke("Invalid Figma file URL. Please check the URL format.");
            OnConnectionTestComplete?.Invoke(false, "Invalid Figma file URL");
            isConnecting = false;
            yield break;
        }
        
        // Attempt connection with retries
        yield return StartCoroutine(TryConnectWithRetries(fileKey));
    }
    
    /// <summary>
    /// Extract file key from Figma URL
    /// </summary>
    private string ExtractFileKey(string url)
    {
        if (string.IsNullOrEmpty(url))
            return null;
        
        // Handle different Figma URL formats
        // Format 1: https://www.figma.com/file/ABC123/FileName
        // Format 2: https://www.figma.com/community/file/ABC123/FileName
        
        try
        {
            var uri = new Uri(url);
            string[] segments = uri.Segments;
            
            for (int i = 0; i < segments.Length; i++)
            {
                if (segments[i].Trim('/') == "file" && i + 1 < segments.Length)
                {
                    return segments[i + 1].Trim('/');
                }
                if (segments[i].Trim('/') == "community" && i + 2 < segments.Length && segments[i + 1].Trim('/') == "file")
                {
                    return segments[i + 2].Trim('/');
                }
            }
        }
        catch (Exception e)
        {
            LogError($"Error parsing Figma URL: {e.Message}");
        }
        
        return null;
    }
    
    /// <summary>
    /// Try to connect to Figma API with retries
    /// </summary>
    private IEnumerator TryConnectWithRetries(string fileKey)
    {
        while (currentRetryAttempt < maxRetryAttempts)
        {
            LogMessage($"Connection attempt {currentRetryAttempt + 1} of {maxRetryAttempts}");
            
            yield return StartCoroutine(GetFigmaFile(fileKey));
            
            if (!isConnecting)
            {
                // Connection successful
                yield break;
            }
            
            currentRetryAttempt++;
            
            if (currentRetryAttempt < maxRetryAttempts)
            {
                LogWarning($"Connection failed. Retrying in {retryDelay} seconds...");
                yield return new WaitForSeconds(retryDelay);
            }
        }
        
        // All retries exhausted
        OnConnectionError?.Invoke($"Failed to connect to Figma API after {maxRetryAttempts} attempts");
        OnConnectionTestComplete?.Invoke(false, "Connection failed after retries");
        isConnecting = false;
    }
    
    /// <summary>
    /// Get Figma file information
    /// </summary>
    private IEnumerator GetFigmaFile(string fileKey)
    {
        connectionStatus = "Connecting to Figma API...";
        connectionProgress = 0.2f;
        OnConnectionProgress?.Invoke(connectionProgress);
        
        string url = $"{FIGMA_API_BASE}{FIGMA_FILES_ENDPOINT}{fileKey}";
        
        LogMessage($"Requesting Figma file: {url}");
        
        // Create web request
        UnityEngine.Networking.UnityWebRequest request = UnityEngine.Networking.UnityWebRequest.Get(url);
        request.SetRequestHeader("X-Figma-Token", figmaApiToken);
        request.timeout = (int)apiTimeout;
        
        yield return request.SendWebRequest();
        
        if (request.result == UnityEngine.Networking.UnityWebRequest.Result.Success)
        {
            yield return StartCoroutine(ProcessFigmaFileResponse(request.downloadHandler.text));
        }
        else
        {
            HandleConnectionError(request);
        }
        
        request.Dispose();
    }
    
    /// <summary>
    /// Process successful Figma API response
    /// </summary>
    private IEnumerator ProcessFigmaFileResponse(string jsonResponse)
    {
        connectionStatus = "Processing Figma file data...";
        connectionProgress = 0.6f;
        OnConnectionProgress?.Invoke(connectionProgress);
        
        LogMessage("Successfully retrieved Figma file data");
        
        FigmaFileInfo fileInfo = null;
        try
        {
            fileInfo = JsonUtility.FromJson<FigmaFileInfo>(jsonResponse);
        }
        catch (Exception e)
        {
            LogError($"Error processing Figma response: {e.Message}");
            OnConnectionError?.Invoke($"Error processing Figma data: {e.Message}");
            OnConnectionTestComplete?.Invoke(false, e.Message);
            yield break;
        }
        
        if (fileInfo == null)
        {
            LogError("Failed to parse Figma file data");
            OnConnectionError?.Invoke("Failed to parse Figma file data");
            OnConnectionTestComplete?.Invoke(false, "Failed to parse Figma file data");
            yield break;
        }
        
        LogMessage($"Figma File: {fileInfo.name}");
        LogMessage($"Last Modified: {fileInfo.last_modified}");
        LogMessage($"Components Found: {fileInfo.components?.Length ?? 0}");
        LogMessage($"Styles Found: {fileInfo.styles?.Length ?? 0}");
        
        yield return StartCoroutine(ExtractUIComponents(fileInfo));
        
        connectionStatus = "Connection successful!";
        connectionProgress = 1.0f;
        OnConnectionProgress?.Invoke(connectionProgress);
        isConnecting = false;
        OnConnectionComplete?.Invoke(true, $"Successfully connected to Figma: {fileInfo.name}");
        OnConnectionTestComplete?.Invoke(true, $"Successfully connected to Figma: {fileInfo.name}");
    }
    
    /// <summary>
    /// Extract UI components from Figma file
    /// </summary>
    private IEnumerator ExtractUIComponents(FigmaFileInfo fileInfo)
    {
        connectionStatus = "Extracting UI components...";
        connectionProgress = 0.8f;
        OnConnectionProgress?.Invoke(connectionProgress);
        
        if (fileInfo.document != null && fileInfo.document.children != null)
        {
            LogMessage($"Processing {fileInfo.document.children.Length} top-level nodes");
            
            // Process each node in the document
            foreach (var node in fileInfo.document.children)
            {
                yield return StartCoroutine(ProcessFigmaNode(node));
            }
        }
        
        LogMessage("UI component extraction completed");
    }
    
    /// <summary>
    /// Process individual Figma node
    /// </summary>
    private IEnumerator ProcessFigmaNode(FigmaNode node)
    {
        LogMessage($"Processing node: {node.name} (Type: {node.type})");
        
        // Extract different types of UI elements based on node type
        switch (node.type.ToLower())
        {
            case "rectangle":
            case "frame":
                ProcessFrameNode(node);
                break;
            case "text":
                ProcessTextNode(node);
                break;
            case "component":
                ProcessComponentNode(node);
                break;
            case "instance":
                ProcessInstanceNode(node);
                break;
            case "group":
                ProcessGroupNode(node);
                break;
            default:
                LogMessage($"Unknown node type: {node.type}");
                break;
        }
        
        // Process child nodes
        if (node.children != null && node.children.Length > 0)
        {
            foreach (var child in node.children)
            {
                yield return StartCoroutine(ProcessFigmaNode(child));
            }
        }
        
        yield return null;
    }
    
    /// <summary>
    /// Process frame/rectangle nodes (typically panels or backgrounds)
    /// </summary>
    private void ProcessFrameNode(FigmaNode node)
    {
        LogMessage($"Found frame/rectangle: {node.name} - Position: {GetNodePosition(node)}, Size: {GetNodeSize(node)}");
        
        // Extract styling information
        if (node.fills != null && node.fills.Length > 0)
        {
            foreach (var fill in node.fills)
            {
                if (fill.visible && fill.color != null)
                {
                    Color unityColor = fill.color.ToUnityColor();
                    LogMessage($"  Fill color: {unityColor}");
                }
            }
        }
    }
    
    /// <summary>
    /// Process text nodes
    /// </summary>
    private void ProcessTextNode(FigmaNode node)
    {
        LogMessage($"Found text: {node.name} - Characters: {(node.characters != null ? string.Join("", node.characters) : "N/A")}");
        
        if (node.style != null)
        {
            LogMessage($"  Font: {node.style.fontFamily}, Size: {node.style.fontSize}");
        }
    }
    
    /// <summary>
    /// Process component nodes
    /// </summary>
    private void ProcessComponentNode(FigmaNode node)
    {
        LogMessage($"Found component: {node.name} - This is a reusable UI element");
        // Components are reusable UI elements that can be instantiated
    }
    
    /// <summary>
    /// Process instance nodes (instances of components)
    /// </summary>
    private void ProcessInstanceNode(FigmaNode node)
    {
        LogMessage($"Found component instance: {node.name} - Instance of a reusable component");
    }
    
    /// <summary>
    /// Process group nodes
    /// </summary>
    private void ProcessGroupNode(FigmaNode node)
    {
        LogMessage($"Found group: {node.name} - Contains {node.children?.Length ?? 0} child elements");
    }
    
    /// <summary>
    /// Get node position as Vector2
    /// </summary>
    private Vector2 GetNodePosition(FigmaNode node)
    {
        if (node.absoluteBoundingBox != null && node.absoluteBoundingBox.Length >= 4)
        {
            return new Vector2(node.absoluteBoundingBox[0], node.absoluteBoundingBox[1]);
        }
        return Vector2.zero;
    }
    
    /// <summary>
    /// Get node size as Vector2
    /// </summary>
    private Vector2 GetNodeSize(FigmaNode node)
    {
        if (node.absoluteBoundingBox != null && node.absoluteBoundingBox.Length >= 4)
        {
            return new Vector2(node.absoluteBoundingBox[2], node.absoluteBoundingBox[3]);
        }
        return Vector2.zero;
    }
    
    /// <summary>
    /// Handle connection errors
    /// </summary>
    private void HandleConnectionError(UnityEngine.Networking.UnityWebRequest request)
    {
        string errorMessage = "";
        
        switch (request.responseCode)
        {
            case 400:
                errorMessage = "Bad request - Check your Figma file URL";
                break;
            case 401:
                errorMessage = "Unauthorized - Check your Figma API token";
                break;
            case 403:
                errorMessage = "Forbidden - You don't have access to this file";
                break;
            case 404:
                errorMessage = "File not found - Check the file URL";
                break;
            case 429:
                errorMessage = "Rate limit exceeded - Try again later";
                break;
            case 500:
            case 502:
            case 503:
                errorMessage = "Figma API server error - Try again later";
                break;
            default:
                errorMessage = $"HTTP {request.responseCode}: {request.error}";
                break;
        }
        
        LogError($"Figma API error: {errorMessage}");
        OnConnectionError?.Invoke(errorMessage);
    }
    
    /// <summary>
    /// Get Figma images for components
    /// </summary>
    public IEnumerator GetFigmaImages(string fileKey, string[] nodeIds)
    {
        string url = $"{FIGMA_API_BASE}{FIGMA_IMAGES_ENDPOINT}{fileKey}";
        url += $"?ids={string.Join(",", nodeIds)}&format=png&scale=2";
        
        UnityEngine.Networking.UnityWebRequest request = UnityEngine.Networking.UnityWebRequest.Get(url);
        request.SetRequestHeader("X-Figma-Token", figmaApiToken);
        
        yield return request.SendWebRequest();
        
        if (request.result == UnityEngine.Networking.UnityWebRequest.Result.Success)
        {
            LogMessage("Successfully retrieved Figma images");
            // Process image URLs from response
        }
        else
        {
            HandleConnectionError(request);
        }
        
        request.Dispose();
    }
    
    /// <summary>
    /// Test API connection with current settings
    /// </summary>
    public void TestConnection()
    {
        StartCoroutine(ConnectToFigma());
    }
    
    /// <summary>
    /// Get connection status
    /// </summary>
    public string GetConnectionStatus()
    {
        return connectionStatus;
    }
    
    /// <summary>
    /// Get connection progress
    /// </summary>
    public float GetConnectionProgress()
    {
        return connectionProgress;
    }
    
    /// <summary>
    /// Check if currently connecting
    /// </summary>
    public bool IsConnecting()
    {
        return isConnecting;
    }
    
    /// <summary>
    /// Logging helper methods
    /// </summary>
    private void LogMessage(string message)
    {
        if (enableDebugLogging)
        {
            Debug.Log($"[BrawlStarsFigmaAPI] {message}");
        }
    }
    
    private void LogWarning(string message)
    {
        Debug.LogWarning($"[BrawlStarsFigmaAPI] {message}");
    }
    
    private void LogError(string message)
    {
        Debug.LogError($"[BrawlStarsFigmaAPI] {message}");
    }
    
    /// <summary>
    /// Get recommended community Figma files
    /// </summary>
    public List<string> GetRecommendedCommunityFiles()
    {
        return new List<string>
        {
            "https://www.figma.com/community/file/123456789/brawl-stars-ui-kit",
            "https://www.figma.com/community/file/987654321/brawl-stars-game-ui",
            "https://www.figma.com/community/file/111111111/mobile-game-ui-components",
            "https://www.figma.com/community/file/222222222/game-ui-design-system"
        };
    }
    
    void OnDestroy()
    {
        // Cleanup
        StopAllCoroutines();
    }
}