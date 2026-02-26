using UnityEngine;
using UnityEngine.UI;

namespace LakbayTala.UI.Core
{
    /// <summary>
    /// Haptic and visual feedback for UI interactions (button press, hover, selection).
    /// Use from button OnClick/OnPointerDown or from custom selectables.
    /// </summary>
    public static class UIFeedbackHelper
    {
        public static bool hapticsEnabled = true;
        public static bool visualFeedbackEnabled = true;

        /// <summary>Light impact for button press (mobile).</summary>
        public static void TriggerButtonPress()
        {
            if (!hapticsEnabled) return;
#if UNITY_IOS || UNITY_ANDROID
            try
            {
                Handheld.Vibrate(); // Consider using lighter feedback on iOS via native; Unity's Vibrate() is heavy on Android
                // Alternative: if using new Input system, use Gamepad.current?.SetMotorSpeeds(0.2f, 0.2f) for gamepad
            }
            catch { /* ignore */ }
#endif
        }

        /// <summary>Optional light haptic for hover (if supported).</summary>
        public static void TriggerHover()
        {
            // Many platforms don't support hover haptic; leave empty or wire to Input System.
        }

        /// <summary>Apply a quick scale punch for visual feedback on the given RectTransform.</summary>
        public static void ApplyPressVisual(RectTransform rect, float scaleDown = 0.95f, float duration = 0.08f)
        {
            if (!visualFeedbackEnabled || rect == null) return;
            var runner = Object.FindFirstObjectByType<UIFeedbackRunner>();
            if (runner != null)
                runner.PunchScale(rect, scaleDown, duration);
        }

        /// <summary>Apply highlight color flash on a Graphic (e.g. Image/Button target).</summary>
        public static void ApplyHighlightFlash(Graphic graphic, Color highlightColor, float duration = 0.15f)
        {
            if (!visualFeedbackEnabled || graphic == null) return;
            var runner = Object.FindFirstObjectByType<UIFeedbackRunner>();
            if (runner != null)
                runner.FlashColor(graphic, highlightColor, duration);
        }
    }

    /// <summary>MonoBehaviour runner for coroutine-based visual feedback. Add one to scene (or UIManager).</summary>
    public class UIFeedbackRunner : MonoBehaviour
    {
        public void PunchScale(RectTransform rect, float scaleDown, float duration)
        {
            if (rect == null) return;
            StopAllCoroutines();
            StartCoroutine(PunchScaleRoutine(rect, scaleDown, duration));
        }

        private System.Collections.IEnumerator PunchScaleRoutine(RectTransform rect, float scaleDown, float duration)
        {
            Vector3 original = rect.localScale;
            float half = duration * 0.5f;
            float t = 0;
            while (t < half)
            {
                t += Time.deltaTime;
                float k = t / half;
                rect.localScale = Vector3.Lerp(original, original * scaleDown, k);
                yield return null;
            }
            t = 0;
            while (t < half)
            {
                t += Time.deltaTime;
                float k = t / half;
                rect.localScale = Vector3.Lerp(original * scaleDown, original, k);
                yield return null;
            }
            rect.localScale = original;
        }

        public void FlashColor(Graphic graphic, Color highlight, float duration)
        {
            if (graphic == null) return;
            StartCoroutine(FlashColorRoutine(graphic, highlight, duration));
        }

        private System.Collections.IEnumerator FlashColorRoutine(Graphic graphic, Color highlight, float duration)
        {
            Color original = graphic.color;
            graphic.color = highlight;
            yield return new WaitForSeconds(duration);
            if (graphic != null)
                graphic.color = original;
        }
    }
}
