using UnityEngine;
#if DOTWEEN_PRESENT
using DG.Tweening;
#endif

namespace SeqDirect.Core.Nodes {
    [SeqNode("FadeIn", Category = "UI", Description = "Fades in UI elements or CanvasGroups.")]
    public class SeqNode_FadeIn : SeqNodeBase<FadeInParams> {
        public override float Duration => data.duration;

        protected override Tween BuildTween(Transform t) {
#if !DOTWEEN_PRESENT
            SeqLogger.DependencyMissing("DOTween", "FadeIn");
            return null;
#endif
            if (t == null) return null;

            if (t.TryGetComponent(out CanvasGroup cg))
                return cg.DOFade(data.targetAlpha, data.duration).SetEase(data.ease);

            var img = t.GetComponent<UnityEngine.UI.Graphic>();
            if (img != null)
                return img.DOFade(data.targetAlpha, data.duration).SetEase(data.ease);

            SeqLogger.Warn($"No CanvasGroup or UI Graphic found on {t.name}.", t);
            return null;
        }
    }

    [System.Serializable]
    public class FadeInParams : SeqNodeParams {
        public float duration = 0.5f;
        [Range(0, 1)] public float targetAlpha = 1f;
        public Ease ease = Ease.OutQuad;
    }
}
