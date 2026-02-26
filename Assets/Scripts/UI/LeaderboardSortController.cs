using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using LakbayTala.Leaderboard;

/// <summary>
/// Provides sortable column behaviour for leaderboard: score, time, level, rank, name, etc.
/// Wire to header buttons or dropdown; apply sort to a list of LeaderboardEntry before display.
/// </summary>
public class LeaderboardSortController : MonoBehaviour
{
    public enum SortField
    {
        Score,
        Rank,
        Name,
        CompletionTime,
        LevelName,
        CulturalScore,
        LastUpdated
    }

    [Header("Sort state")]
    public SortField currentSort = SortField.Score;
    public bool ascending = false;

    [Header("Optional UI (assign for visual feedback)")]
    public Button sortByScoreButton;
    public Button sortByRankButton;
    public Button sortByNameButton;
    public Button sortByTimeButton;
    public Button sortByLevelButton;
    public TMPro.TMP_Text sortIndicatorText;

    public event Action<SortField, bool> OnSortChanged;

    private void Start()
    {
        if (sortByScoreButton) sortByScoreButton.onClick.AddListener(() => SetSort(SortField.Score));
        if (sortByRankButton) sortByRankButton.onClick.AddListener(() => SetSort(SortField.Rank));
        if (sortByNameButton) sortByNameButton.onClick.AddListener(() => SetSort(SortField.Name));
        if (sortByTimeButton) sortByTimeButton.onClick.AddListener(() => SetSort(SortField.CompletionTime));
        if (sortByLevelButton) sortByLevelButton.onClick.AddListener(() => SetSort(SortField.LevelName));
        UpdateSortIndicator();
    }

    public void SetSort(SortField field)
    {
        if (currentSort == field)
            ascending = !ascending;
        else
        {
            currentSort = field;
            ascending = field == SortField.Score || field == SortField.Rank || field == SortField.CulturalScore ? false : true;
        }
        OnSortChanged?.Invoke(currentSort, ascending);
        UpdateSortIndicator();
    }

    private void UpdateSortIndicator()
    {
        if (sortIndicatorText != null)
            sortIndicatorText.text = $"{currentSort} ({(ascending ? "A↑" : "D↓")})";
    }

    /// <summary>Sort a list of entries in place by current sort field and direction.</summary>
    public void SortEntries(List<LeaderboardEntry> entries)
    {
        if (entries == null) return;
        switch (currentSort)
        {
            case SortField.Score:
                entries.Sort((a, b) => (ascending ? 1 : -1) * (a.score.CompareTo(b.score)));
                break;
            case SortField.Rank:
                entries.Sort((a, b) => (ascending ? 1 : -1) * (a.rank.CompareTo(b.rank)));
                break;
            case SortField.Name:
                entries.Sort((a, b) => (ascending ? 1 : -1) * string.Compare(GetName(a), GetName(b), StringComparison.OrdinalIgnoreCase));
                break;
            case SortField.CompletionTime:
                entries.Sort((a, b) => (ascending ? 1 : -1) * (GetCompletionTime(a).CompareTo(GetCompletionTime(b))));
                break;
            case SortField.LevelName:
                entries.Sort((a, b) => (ascending ? 1 : -1) * string.Compare(GetLevelName(a), GetLevelName(b), StringComparison.OrdinalIgnoreCase));
                break;
            case SortField.LastUpdated:
                entries.Sort((a, b) => (ascending ? 1 : -1) * (a.lastUpdated.CompareTo(b.lastUpdated)));
                break;
            default:
                entries.Sort((a, b) => b.score.CompareTo(a.score));
                break;
        }
    }

    private static string GetName(LeaderboardEntry e) => e?.user?.displayName ?? e?.user?.username ?? "";
    private static float GetCompletionTime(LeaderboardEntry e)
    {
        if (e?.additionalData == null || !e.additionalData.TryGetValue("completionTime", out var t)) return 0f;
        try { return Convert.ToSingle(t); } catch { return 0f; }
    }
    private static string GetLevelName(LeaderboardEntry e)
    {
        if (e?.additionalData != null && e.additionalData.TryGetValue("levelName", out var l)) return l?.ToString() ?? "";
        return e?.user?.culturalLevel ?? "";
    }
}
