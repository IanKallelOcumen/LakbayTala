using UnityEngine;
using System;

/// <summary>
/// Quick verification script to test the Figma integration with your real credentials
/// </summary>
public class TestFigmaIntegration : MonoBehaviour
{
    [Header("Your Real Figma Credentials")]
    public string figmaApiToken = ""; // Set in Inspector: your Figma PAT from figma.com/settings
    public string figmaFileUrl = "https://www.figma.com/design/BOMLvyJ570CbsohHwPtEGB/Brawl-Stars-Game-UI-Kit--Community-?node-id=1-892&t=q4MCZssNkwucf3LB-1";
    
    void Start()
    {
        TestCredentials();
    }
    
    void TestCredentials()
    {
        Debug.Log("🚀 TESTING YOUR REAL FIGMA CREDENTIALS");
        Debug.Log("=".PadRight(50, '='));
        
        // Test 1: Token Format
        bool tokenValid = figmaApiToken.StartsWith("figd_") && figmaApiToken.Length >= 40;
        Debug.Log($"Token Format: {(tokenValid ? "✅ VALID" : "❌ INVALID")}");
        Debug.Log($"  Token: {figmaApiToken.Substring(0, 20)}...");
        Debug.Log($"  Length: {figmaApiToken.Length} characters");
        
        // Test 2: URL Format
        bool urlValid = figmaFileUrl.Contains("figma.com") && figmaFileUrl.Contains("/design/");
        Debug.Log($"File URL Format: {(urlValid ? "✅ VALID" : "❌ INVALID")}");
        Debug.Log($"  URL: {figmaFileUrl}");
        
        // Test 3: Extract File Key
        string fileKey = ExtractFileKey(figmaFileUrl);
        bool keyValid = !string.IsNullOrEmpty(fileKey) && fileKey.Length > 10;
        Debug.Log($"File Key Extraction: {(keyValid ? "✅ SUCCESS" : "❌ FAILED")}");
        Debug.Log($"  Key: {fileKey}");
        
        // Test 4: Overall Status
        bool allValid = tokenValid && urlValid && keyValid;
        Debug.Log($"Overall Status: {(allValid ? "🎉 READY FOR API CONNECTION" : "⚠️ ISSUES FOUND")}");
        
        if (allValid)
        {
            Debug.Log("✅ Your Figma integration is properly configured!");
            Debug.Log("✅ Ready to connect to Brawl Stars Game UI Kit community file");
        }
        else
        {
            Debug.LogError("❌ Please check the issues above and fix them");
        }
        
        Debug.Log("=".PadRight(50, '='));
    }
    
    string ExtractFileKey(string url)
    {
        try
        {
            string[] segments = url.Split('/');
            for (int i = 0; i < segments.Length; i++)
            {
                if (segments[i] == "design" && i + 1 < segments.Length)
                {
                    return segments[i + 1].Split('?')[0]; // Remove query parameters
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Error extracting file key: {e.Message}");
        }
        return "";
    }
}