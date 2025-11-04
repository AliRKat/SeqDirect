using UnityEngine;
#if DOTWEEN_PRESENT
using DG.Tweening;
#endif

namespace SeqDirect.Core.Nodes {
    [SeqNode("Show", Category = "UI", Description = "Shows an element by scaling and fading it in.")]
    public class SeqNode_Show : SeqNodeBase<ShowParams> {
        public override float Duration => data.duration;

        protected override Tween BuildTween(Transform t) {
#if !DOTWEEN_PRESENT
            SeqLogger.DependencyMissing("DOTween", "Show");
            return null;
#endif
            if (t == null) return null;

            t.localScale = Vector3.one * data.startScale;

            var seq = DOTween.Sequence();
            seq.Append(t.DOScale(Vector3.one, data.duration).SetEase(data.ease));

            var cg = t.GetComponent<CanvasGroup>();
            if (cg != null)
                seq.Join(cg.DOFade(1f, data.duration));

            return seq;
        }
    }

    [System.Serializable]
    public class ShowParams : SeqNodeParams {
        public float duration = 0.5f;
        public float startScale = 0.8f;
        public Ease ease = Ease.OutBack;
    }
}
