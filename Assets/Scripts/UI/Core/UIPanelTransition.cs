using UnityEngine;
using System;
using System.Collections;

namespace LakbayTala.UI.Core
{
    /// <summary>
    /// Reusable panel transition helpers: fade, scale, slide with easing.
    /// Uses AnimationCurve for easing (no external tween dependency required).
    /// </summary>
    public static class UIPanelTransition
    {
        public static IEnumerator Fade(CanvasGroup group, float targetAlpha, float duration, AnimationCurve curve, Action onComplete = null)
        {
            if (group == null) { onComplete?.Invoke(); yield break; }
            float start = group.alpha;
            float t = 0;
            while (t < duration)
            {
                t += Time.deltaTime;
                float k = duration > 0 ? Mathf.Clamp01(t / duration) : 1f;
                group.alpha = Mathf.Lerp(start, targetAlpha, curve != null ? curve.Evaluate(k) : k);
                yield return null;
            }
            group.alpha = targetAlpha;
            onComplete?.Invoke();
        }

        public static IEnumerator Scale(RectTransform rect, Vector3 targetScale, float duration, AnimationCurve curve, Action onComplete = null)
        {
            if (rect == null) { onComplete?.Invoke(); yield break; }
            Vector3 start = rect.localScale;
            float t = 0;
            while (t < duration)
            {
                t += Time.deltaTime;
                float k = duration > 0 ? Mathf.Clamp01(t / duration) : 1f;
                float s = curve != null ? curve.Evaluate(k) : k;
                rect.localScale = Vector3.Lerp(start, targetScale, s);
                yield return null;
            }
            rect.localScale = targetScale;
            onComplete?.Invoke();
        }

        public static IEnumerator Slide(RectTransform rect, Vector2 targetAnchoredPos, float duration, AnimationCurve curve, Action onComplete = null)
        {
            if (rect == null) { onComplete?.Invoke(); yield break; }
            Vector2 start = rect.anchoredPosition;
            float t = 0;
            while (t < duration)
            {
                t += Time.deltaTime;
                float k = duration > 0 ? Mathf.Clamp01(t / duration) : 1f;
                float s = curve != null ? curve.Evaluate(k) : k;
                rect.anchoredPosition = Vector2.Lerp(start, targetAnchoredPos, s);
                yield return null;
            }
            rect.anchoredPosition = targetAnchoredPos;
            onComplete?.Invoke();
        }

        /// <summary>Standard ease-in-out curve for panel open/close.</summary>
        public static AnimationCurve DefaultCurve => AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
    }
}
