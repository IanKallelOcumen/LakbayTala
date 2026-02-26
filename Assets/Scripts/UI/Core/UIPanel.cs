using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

namespace LakbayTala.UI.Core
{
    public enum UIPanelType
    {
        None,
        HUD,
        Menu,
        Lore,
        Popup,
        Overlay,
        Leaderboard,
        Stats,
        Achievements,
        Settings
    }

    [RequireComponent(typeof(CanvasGroup))]
    public class UIPanel : MonoBehaviour
    {
        [Header("Panel Configuration")]
        public UIPanelType panelType;
        public bool showOnStart = false;
        public bool hideOnStart = true;
        
        [Header("Animation")]
        public float transitionDuration = 0.3f;
        public AnimationCurve showCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
        public AnimationCurve hideCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

        protected CanvasGroup canvasGroup;
        protected RectTransform rectTransform;

        public event Action OnShowComplete;
        public event Action OnHideComplete;

        protected virtual void Awake()
        {
            canvasGroup = GetComponent<CanvasGroup>();
            rectTransform = GetComponent<RectTransform>();

            if (showOnStart) Show(true);
            else if (hideOnStart) Hide(true);
        }

        public virtual void Show(bool instant = false)
        {
            gameObject.SetActive(true);
            StopAllCoroutines();
            NotifyStateShown();

            if (instant)
            {
                canvasGroup.alpha = 1;
                canvasGroup.interactable = true;
                canvasGroup.blocksRaycasts = true;
                OnShowComplete?.Invoke();
            }
            else
            {
                StartCoroutine(FadeRoutine(1, transitionDuration, showCurve, () =>
                {
                    canvasGroup.interactable = true;
                    canvasGroup.blocksRaycasts = true;
                    OnShowComplete?.Invoke();
                }));
            }
        }

        public virtual void Hide(bool instant = false)
        {
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
            StopAllCoroutines();

            if (instant)
            {
                canvasGroup.alpha = 0;
                gameObject.SetActive(false);
                OnHideComplete?.Invoke();
                NotifyStateHidden();
            }
            else
            {
                StartCoroutine(FadeRoutine(0, transitionDuration, hideCurve, () =>
                {
                    gameObject.SetActive(false);
                    OnHideComplete?.Invoke();
                    NotifyStateHidden();
                }));
            }
        }

        private void NotifyStateShown()
        {
            if (UIStateManager.Instance != null)
                UIStateManager.Instance.NotifyPanelShown(panelType);
        }

        private void NotifyStateHidden()
        {
            if (UIStateManager.Instance != null)
                UIStateManager.Instance.NotifyPanelHidden(panelType);
        }

        private IEnumerator FadeRoutine(float targetAlpha, float duration, AnimationCurve curve, Action onComplete)
        {
            float startAlpha = canvasGroup.alpha;
            float time = 0;

            while (time < duration)
            {
                time += Time.deltaTime;
                float t = time / duration;
                float curveValue = curve.Evaluate(t);
                canvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, curveValue);
                yield return null;
            }

            canvasGroup.alpha = targetAlpha;
            onComplete?.Invoke();
        }
    }
}
