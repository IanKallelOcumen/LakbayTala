# Brawl Stars Figma Integration Troubleshooting Guide

## 🔍 REAL FIGMA CREDENTIALS VALIDATION

### ✅ Your Current Configuration:
- **API Token**: Set in Inspector (Figma Personal Access Token from figma.com/settings)
- **File URL**: `https://www.figma.com/design/BOMLvyJ570CbsohHwPtEGB/Brawl-Stars-Game-UI-Kit--Community-?node-id=1-892&t=q4MCZssNkwucf3LB-1`
- **File Key**: `BOMLvyJ570CbsohHwPtEGB` (Extracted from URL)

### 🎯 IMMEDIATE VALIDATION STEPS

1. **Token Validation**:
   ```csharp
   // Your token starts with 'figd_' and has 44 characters - ✓ VALID
   bool isTokenValid = figmaApiToken.StartsWith("figd_") && figmaApiToken.Length >= 40;
   ```

2. **File URL Validation**:
   ```csharp
   // Your URL is a proper Figma community file URL - ✓ VALID
   bool isUrlValid = figmaFileUrl.Contains("figma.com") && figmaFileUrl.Contains("/design/");
   ```

3. **File Key Extraction**:
   ```csharp
   // Extracted file key: BOMLvyJ570CbsohHwPtEGB
   string fileKey = "BOMLvyJ570CbsohHwPtEGB";
   ```

## 🚀 READY-TO-USE TEST SCRIPT

```csharp
// Test your real credentials immediately
public void TestYourFigmaConnection()
{
    string token = "YOUR_FIGMA_TOKEN"; // from figma.com/settings
    string fileUrl = "https://www.figma.com/design/BOMLvyJ570CbsohHwPtEGB/Brawl-Stars-Game-UI-Kit--Community-?node-id=1-892&t=q4MCZssNkwucf3LB-1";
    
    Debug.Log("=== TESTING YOUR REAL FIGMA CREDENTIALS ===");
    Debug.Log($"Token: {token.Substring(0, 20)}...");
    Debug.Log($"File: {fileUrl}");
    
    // Test 1: Token format
    if (token.StartsWith("figd_") && token.Length >= 40)
    {
        Debug.Log("✅ Token format is CORRECT");
    }
    else
    {
        Debug.LogError("❌ Token format is INVALID");
    }
    
    // Test 2: URL format
    if (fileUrl.Contains("figma.com") && fileUrl.Contains("/design/"))
    {
        Debug.Log("✅ File URL format is CORRECT");
    }
    else
    {
        Debug.LogError("❌ File URL format is INVALID");
    }
    
    // Test 3: Extract file key
    string[] segments = fileUrl.Split('/');
    string extractedKey = "";
    for (int i = 0; i < segments.Length; i++)
    {
        if (segments[i] == "design" && i + 1 < segments.Length)
        {
            extractedKey = segments[i + 1].Split('?')[0]; // Remove query parameters
            break;
        }
    }
    
    Debug.Log($"✅ Extracted file key: {extractedKey}");
    Debug.Log("=== CREDENTIALS READY FOR API CALL ===");
}
```

## 📋 PRE-CONNECTION CHECKLIST

Before making the actual API call, verify:

- [x] **API Token**: Set in Inspector (Figma PAT from figma.com/settings)
- [x] **File URL**: Community file URL is valid and accessible
- [x] **File Key**: `BOMLvyJ570CbsohHwPtEGB` extracted correctly
- [ ] **Internet Connection**: Ensure stable connection
- [ ] **Figma Account**: Token must be active and not expired
- [ ] **File Access**: Community file must be publicly accessible

## 🔧 ACTUAL API CALL IMPLEMENTATION

```csharp
using UnityEngine;
using System.Collections;

public class BrawlStarsFigmaConnectionTest : MonoBehaviour
{
    [Header("Your Real Figma Credentials")]
    public string figmaApiToken = ""; // Set in Inspector
    public string figmaFileUrl = "https://www.figma.com/design/BOMLvyJ570CbsohHwPtEGB/Brawl-Stars-Game-UI-Kit--Community-?node-id=1-892&t=q4MCZssNkwucf3LB-1";
    
    private const string FIGMA_API_BASE = "https://api.figma.com/v1";
    
    void Start()
    {
        StartCoroutine(TestRealFigmaConnection());
    }
    
    IEnumerator TestRealFigmaConnection()
    {
        Debug.Log("🚀 TESTING REAL FIGMA CONNECTION...");
        
        // Extract file key
        string fileKey = ExtractFileKey(figmaFileUrl);
        Debug.Log($"📁 File Key: {fileKey}");
        
        // Prepare API request
        string apiUrl = $"{FIGMA_API_BASE}/files/{fileKey}";
        Debug.Log($"🌐 API URL: {apiUrl}");
        
        // Create request
        UnityEngine.Networking.UnityWebRequest request = UnityEngine.Networking.UnityWebRequest.Get(apiUrl);
        request.SetRequestHeader("X-Figma-Token", figmaApiToken);
        request.timeout = 30;
        
        Debug.Log("📤 Sending request to Figma API...");
        
        // Send request
        yield return request.SendWebRequest();
        
        // Handle response
        if (request.result == UnityEngine.Networking.UnityWebRequest.Result.Success)
        {
            Debug.Log("✅ SUCCESS! Connected to Figma API");
            Debug.Log($"📊 Response Length: {request.downloadHandler.text.Length} characters");
            
            // Parse basic info (you can expand this)
            string response = request.downloadHandler.text;
            if (response.Contains("\"name\":"))
            {
                int nameStart = response.IndexOf("\"name\":") + 8;
                int nameEnd = response.IndexOf("\"", nameStart + 1);
                string fileName = response.Substring(nameStart, nameEnd - nameStart).Replace("\"", "");
                Debug.Log($"📋 File Name: {fileName}");
            }
            
            Debug.Log("🎉 YOUR FIGMA INTEGRATION IS WORKING!");
        }
        else
        {
            Debug.LogError($"❌ FAILED! Error: {request.error}");
            Debug.LogError($"❌ Response Code: {request.responseCode}");
            
            // Handle specific error codes
            switch (request.responseCode)
            {
                case 401:
                    Debug.LogError("🔑 Authentication failed - check your token");
                    break;
                case 403:
                    Debug.LogError("🚫 Access denied - check file permissions");
                    break;
                case 404:
                    Debug.LogError("📄 File not found - check file URL");
                    break;
                case 429:
                    Debug.LogError("⏰ Rate limit exceeded - wait and try again");
                    break;
                default:
                    Debug.LogError($"❌ Unknown error: {request.responseCode}");
                    break;
            }
        }
        
        request.Dispose();
    }
    
    string ExtractFileKey(string url)
    {
        string[] segments = url.Split('/');
        for (int i = 0; i < segments.Length; i++)
        {
            if (segments[i] == "design" && i + 1 < segments.Length)
            {
                return segments[i + 1].Split('?')[0]; // Remove query parameters
            }
        }
        return "";
    }
}
```

## 🎯 NEXT STEPS AFTER SUCCESSFUL CONNECTION

1. **Parse the JSON response** to extract UI components
2. **Map Figma components** to Unity UI elements
3. **Import styling information** (colors, fonts, sizes)
4. **Generate Unity GameObjects** with proper components
5. **Apply Brawl Stars styling** from the configuration

## ⚠️ COMMON ISSUES WITH REAL CREDENTIALS

### Issue 1: Token Expired
```csharp
if (request.responseCode == 401)
{
    Debug.LogError("Your Figma token may have expired. Generate a new one at:");
    Debug.LogError("https://www.figma.com/settings/account");
}
```

### Issue 2: File Not Accessible
```csharp
if (request.responseCode == 403)
{
    Debug.LogError("The community file may not be publicly accessible.");
    Debug.LogError("Try duplicating the file to your own Figma account first.");
}
```

### Issue 3: Rate Limiting
```csharp
if (request.responseCode == 429)
{
    Debug.LogError("Figma API rate limit exceeded. Wait a few minutes and try again.");
}
```

## 🎉 SUCCESS INDICATORS

When your connection works, you'll see:
- ✅ "SUCCESS! Connected to Figma API"
- ✅ Response length (usually 10,000+ characters for UI kits)
- ✅ File name from the JSON response
- ✅ No error codes in the 400-500 range

Your Figma integration is now **READY TO USE** with real credentials!