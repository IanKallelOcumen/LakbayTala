using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using LakbayTala.UI.Core;
using LakbayTala.Lore.Data;

namespace LakbayTala.UI.Lore
{
    public enum LoreCardRevealMode
    {
        Scale,
        FadeAndScale,
        SlideFromBottom,
        Staggered
    }

    public class LoreCardUI : UIPanel
    {
        [Header("Content References")]
        public Image loreImage;
        public TextMeshProUGUI titleText;
        public TextMeshProUGUI categoryText;
        public TextMeshProUGUI descriptionText;
        public Button closeButton;

        [Header("Reveal Animation")]
        public LoreCardRevealMode revealMode = LoreCardRevealMode.FadeAndScale;
        public ParticleSystem revealParticles;
        public AnimationCurve revealScaleCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
        [Tooltip("Stagger delay per element when using Staggered mode.")]
        public float staggerDelay = 0.06f;

        private LoreData currentData;

        protected override void Awake()
        {
            base.Awake();
            if (closeButton != null)
                closeButton.onClick.AddListener(OnCloseClicked);
        }

        private void OnCloseClicked()
        {
            UIFeedbackHelper.TriggerButtonPress();
            UIFeedbackHelper.ApplyPressVisual(closeButton != null ? closeButton.GetComponent<RectTransform>() : null);
            Hide();
        }

        public void DisplayLore(LoreData data)
        {
            if (data == null) return;
            currentData = data;
            UpdateUI();
            Show();
        }

        private void UpdateUI()
        {
            if (titleText) titleText.text = currentData.title;
            if (categoryText) categoryText.text = currentData.category.ToString();
            if (descriptionText) descriptionText.text = currentData.fullDescription;

            if (loreImage)
            {
                if (currentData.illustration != null)
                {
                    loreImage.sprite = currentData.illustration;
                    loreImage.gameObject.SetActive(true);
                }
                else
                    loreImage.gameObject.SetActive(false);
            }
        }

        public override void Show(bool instant = false)
        {
            base.Show(instant);

            if (!instant)
                StartCoroutine(RevealRoutine());
            else if (revealParticles != null)
                revealParticles.Play();
        }

        private IEnumerator RevealRoutine()
        {
            switch (revealMode)
            {
                case LoreCardRevealMode.Scale:
                    yield return RevealScaleOnly();
                    break;
                case LoreCardRevealMode.FadeAndScale:
                    yield return RevealFadeAndScale();
                    break;
                case LoreCardRevealMode.SlideFromBottom:
                    yield return RevealSlideFromBottom();
                    break;
                case LoreCardRevealMode.Staggered:
                    yield return RevealStaggered();
                    break;
                default:
                    yield return RevealFadeAndScale();
                    break;
            }
            if (revealParticles != null)
                revealParticles.Play();
        }

        private IEnumerator RevealScaleOnly()
        {
            rectTransform.localScale = Vector3.zero;
            float time = 0;
            while (time < transitionDuration)
            {
                time += Time.deltaTime;
                float t = revealScaleCurve.Evaluate(time / transitionDuration);
                rectTransform.localScale = Vector3.one * t;
                yield return null;
            }
            rectTransform.localScale = Vector3.one;
        }

        private IEnumerator RevealFadeAndScale()
        {
            rectTransform.localScale = Vector3.zero;
            if (canvasGroup != null) canvasGroup.alpha = 0f;
            float time = 0;
            while (time < transitionDuration)
            {
                time += Time.deltaTime;
                float t = revealScaleCurve.Evaluate(time / transitionDuration);
                rectTransform.localScale = Vector3.one * t;
                if (canvasGroup != null) canvasGroup.alpha = t;
                yield return null;
            }
            rectTransform.localScale = Vector3.one;
            if (canvasGroup != null) canvasGroup.alpha = 1f;
        }

        private IEnumerator RevealSlideFromBottom()
        {
            float height = rectTransform.rect.height;
            Vector2 startPos = rectTransform.anchoredPosition;
            Vector2 endPos = startPos + new Vector2(0, height);
            rectTransform.anchoredPosition = endPos;
            if (canvasGroup != null) canvasGroup.alpha = 0f;
            float time = 0;
            while (time < transitionDuration)
            {
                time += Time.deltaTime;
                float t = revealScaleCurve.Evaluate(time / transitionDuration);
                rectTransform.anchoredPosition = Vector2.Lerp(endPos, startPos, t);
                if (canvasGroup != null) canvasGroup.alpha = t;
                yield return null;
            }
            rectTransform.anchoredPosition = startPos;
            if (canvasGroup != null) canvasGroup.alpha = 1f;
        }

        private IEnumerator RevealStaggered()
        {
            rectTransform.localScale = Vector3.one;
            var elements = new CanvasGroup[] { };
            if (loreImage != null && loreImage.gameObject.activeSelf)
            {
                var cg = loreImage.GetComponent<CanvasGroup>();
                if (cg == null) cg = loreImage.gameObject.AddComponent<CanvasGroup>();
                cg.alpha = 0f;
                yield return FadeInElement(cg, staggerDelay * 0f);
            }
            if (titleText != null) { var cg = EnsureCanvasGroup(titleText.gameObject); cg.alpha = 0f; yield return FadeInElement(cg, staggerDelay * 1f); }
            if (categoryText != null) { var cg = EnsureCanvasGroup(categoryText.gameObject); cg.alpha = 0f; yield return FadeInElement(cg, staggerDelay * 2f); }
            if (descriptionText != null) { var cg = EnsureCanvasGroup(descriptionText.gameObject); cg.alpha = 0f; yield return FadeInElement(cg, staggerDelay * 3f); }
        }

        private CanvasGroup EnsureCanvasGroup(GameObject go)
        {
            var cg = go.GetComponent<CanvasGroup>();
            if (cg == null) cg = go.AddComponent<CanvasGroup>();
            return cg;
        }

        private IEnumerator FadeInElement(CanvasGroup cg, float delay)
        {
            if (delay > 0) yield return new WaitForSeconds(delay);
            float time = 0;
            float dur = transitionDuration * 0.5f;
            while (time < dur && cg != null)
            {
                time += Time.deltaTime;
                cg.alpha = revealScaleCurve.Evaluate(time / dur);
                yield return null;
            }
            if (cg != null) cg.alpha = 1f;
        }
    }
}
