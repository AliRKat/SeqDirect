using UnityEngine;
#if DOTWEEN_PRESENT
using DG.Tweening;
#endif

namespace SeqDirect.Core.Nodes {
    [SeqNode("Hide", Category = "UI", Description = "Hides an element by scaling down and fading out.")]
    public class SeqNode_Hide : SeqNodeBase<HideParams> {
        public override float Duration => data.duration;

        protected override Tween BuildTween(Transform t) {
#if !DOTWEEN_PRESENT
            SeqLogger.DependencyMissing("DOTween", "Hide");
            return null;
#endif
            if (t == null) return null;

            var seq = DOTween.Sequence();
            seq.Append(t.DOScale(Vector3.one * data.targetScale, data.duration).SetEase(data.ease));

            var cg = t.GetComponent<CanvasGroup>();
            if (cg != null)
                seq.Join(cg.DOFade(0f, data.duration));

            return seq;
        }
    }

    [System.Serializable]
    public class HideParams : SeqNodeParams {
        public float duration = 0.5f;
        public float targetScale = 0.8f;
        public Ease ease = Ease.InBack;
    }
}
