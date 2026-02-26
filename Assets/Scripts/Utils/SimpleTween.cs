using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

namespace DG.Tweening
{
    public enum AutoPlay { None, Auto, All }
    public enum LoopType { Restart, Yoyo, Incremental }
    public enum Ease { Linear, InSine, OutSine, InOutSine, InQuad, OutQuad, InOutQuad, InCubic, OutCubic, InOutCubic, InQuart, OutQuart, InOutQuart, InQuint, OutQuint, InOutQuint, InExpo, OutExpo, InOutExpo, InCirc, OutCirc, InOutCirc, InElastic, OutElastic, InOutElastic, InBack, OutBack, InOutBack, InBounce, OutBounce, InOutBounce }

    public class Tween
    {
        public Action onComplete;
        public Action onKill;
        public bool isComplete;
        public bool isPlaying;

        public Tween OnComplete(Action action)
        {
            onComplete = action;
            return this;
        }

        public Tween SetEase(Ease ease)
        {
            return this;
        }

        public bool IsActive()
        {
            return isPlaying;
        }

        public CustomYieldInstruction WaitForCompletion()
        {
            return new WaitForTweenComplete(this);
        }

        public virtual void Kill()
        {
            onKill?.Invoke();
            isPlaying = false;
        }
    }

    internal class WaitForTweenComplete : CustomYieldInstruction
    {
        private readonly Tween tween;
        public override bool keepWaiting => tween != null && tween.isPlaying;
        public WaitForTweenComplete(Tween t) { tween = t; }
    }

    public class Sequence : Tween
    {
        public List<Tween> tweens = new List<Tween>();

        public Sequence()
        {
            isPlaying = true;
        }

        public Sequence Append(Tween tween)
        {
            tweens.Add(tween);
            return this;
        }

        public Sequence Join(Tween tween)
        {
            tweens.Add(tween);
            return this;
        }

        public new Sequence SetEase(Ease ease)
        {
            return this;
        }

        public override void Kill()
        {
            foreach (var t in tweens)
                t?.Kill();
            base.Kill();
        }
    }

    public static class DOTween
    {
        public static AutoPlay defaultAutoPlay;
        public static bool defaultAutoKill;

        public static void SetTweensCapacity(int tweenersCapacity, int sequencesCapacity) { }
        
        public static void KillAll() 
        { 
            SimpleTweenRunner.Instance.StopAllCoroutines();
        }

        public static Sequence Sequence()
        {
            return new Sequence();
        }

        public static Tween To(Func<float> getter, Action<float> setter, float endValue, float duration)
        {
            return SimpleTweenRunner.Instance.StartTween(SimpleTweenRunner.Instance.FloatTween(getter, setter, endValue, duration));
        }
    }

    public static class DOVirtual
    {
        public static Tween DelayedCall(float delay, Action callback)
        {
            return SimpleTweenRunner.Instance.StartTween(SimpleTweenRunner.Instance.DelayedCallRoutine(delay, callback));
        }
    }

    public static class ShortcutExtensions
    {
        public static Tween DOScale(this Transform target, float endValue, float duration)
        {
            return SimpleTweenRunner.Instance.StartTween(SimpleTweenRunner.Instance.ScaleTween(target, Vector3.one * endValue, duration));
        }

        public static Tween DOScale(this Transform target, Vector3 endValue, float duration)
        {
            return SimpleTweenRunner.Instance.StartTween(SimpleTweenRunner.Instance.ScaleTween(target, endValue, duration));
        }

        public static Tween DOLocalMoveY(this Transform target, float endValue, float duration)
        {
             return SimpleTweenRunner.Instance.StartTween(SimpleTweenRunner.Instance.LocalMoveYTween(target, endValue, duration));
        }

        public static Tween DOFade(this CanvasGroup target, float endValue, float duration)
        {
            return SimpleTweenRunner.Instance.StartTween(SimpleTweenRunner.Instance.FadeTween(target, endValue, duration));
        }
        
        public static Tween DOFade(this Image target, float endValue, float duration)
        {
            return SimpleTweenRunner.Instance.StartTween(SimpleTweenRunner.Instance.FadeImageTween(target, endValue, duration));
        }
    }

    public class SimpleTweenRunner : MonoBehaviour
    {
        private static SimpleTweenRunner _instance;
        public static SimpleTweenRunner Instance
        {
            get
            {
                if (_instance == null)
                {
                    GameObject go = new GameObject("SimpleTweenRunner");
                    _instance = go.AddComponent<SimpleTweenRunner>();
                    DontDestroyOnLoad(go);
                }
                return _instance;
            }
        }

        public Tween StartTween(IEnumerator routine)
        {
            Tween tween = new Tween();
            tween.isPlaying = true;
            tween.onKill = () => { if (_instance != null) _instance.StopCoroutine(routine); };
            StartCoroutine(RunTween(routine, tween));
            return tween;
        }

        private IEnumerator RunTween(IEnumerator routine, Tween tween)
        {
            yield return routine;
            tween.isComplete = true;
            tween.isPlaying = false;
            tween.onComplete?.Invoke();
        }

        public IEnumerator ScaleTween(Transform target, Vector3 endValue, float duration)
        {
            if (target == null) yield break;
            Vector3 startValue = target.localScale;
            float elapsed = 0;
            while (elapsed < duration)
            {
                if (target == null) yield break;
                elapsed += Time.deltaTime;
                target.localScale = Vector3.Lerp(startValue, endValue, elapsed / duration);
                yield return null;
            }
            if (target != null) target.localScale = endValue;
        }

        public IEnumerator LocalMoveYTween(Transform target, float endValue, float duration)
        {
            if (target == null) yield break;
            Vector3 startValue = target.localPosition;
            float elapsed = 0;
            while (elapsed < duration)
            {
                if (target == null) yield break;
                elapsed += Time.deltaTime;
                Vector3 newPos = startValue;
                newPos.y = Mathf.Lerp(startValue.y, endValue, elapsed / duration);
                target.localPosition = newPos;
                yield return null;
            }
            if (target != null)
            {
                Vector3 finalPos = target.localPosition;
                finalPos.y = endValue;
                target.localPosition = finalPos;
            }
        }

        public IEnumerator FadeTween(CanvasGroup target, float endValue, float duration)
        {
            if (target == null) yield break;
            float startValue = target.alpha;
            float elapsed = 0;
            while (elapsed < duration)
            {
                if (target == null) yield break;
                elapsed += Time.deltaTime;
                target.alpha = Mathf.Lerp(startValue, endValue, elapsed / duration);
                yield return null;
            }
            if (target != null) target.alpha = endValue;
        }

        public IEnumerator FadeImageTween(Image target, float endValue, float duration)
        {
            if (target == null) yield break;
            Color startColor = target.color;
            Color endColor = startColor;
            endColor.a = endValue;
            float elapsed = 0;
            while (elapsed < duration)
            {
                if (target == null) yield break;
                elapsed += Time.deltaTime;
                target.color = Color.Lerp(startColor, endColor, elapsed / duration);
                yield return null;
            }
            if (target != null) target.color = endColor;
        }

        public IEnumerator FloatTween(Func<float> getter, Action<float> setter, float endValue, float duration)
        {
            float startValue = getter();
            float elapsed = 0;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                setter(Mathf.Lerp(startValue, endValue, elapsed / duration));
                yield return null;
            }
            setter(endValue);
        }

        public IEnumerator DelayedCallRoutine(float delay, Action callback)
        {
            yield return new WaitForSeconds(delay);
            callback?.Invoke();
        }
    }
}
