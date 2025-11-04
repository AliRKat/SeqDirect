using UnityEngine;
#if DOTWEEN_PRESENT
using DG.Tweening;
#endif

namespace SeqDirect.Core.Nodes {
    [SeqNode("Bounce", Category = "Transform", Description = "Scales an object up and back down like a bounce.")]
    public class SeqNode_Bounce : SeqNodeBase<BounceParams> {
        public override float Duration => data.duration;

        protected override Tween BuildTween(Transform t) {
#if !DOTWEEN_PRESENT
            SeqLogger.DependencyMissing("DOTween", "Bounce");
            return null;
#endif
            if (t == null) return null;

            var seq = DOTween.Sequence();
            seq.Append(t.DOScale(Vector3.one * data.bounceScale, data.duration * data.upFraction).SetEase(data.easeOut));
            seq.Append(t.DOScale(Vector3.one, data.duration * (1 - data.upFraction)).SetEase(data.easeIn));
            return seq;
        }
    }

    [System.Serializable]
    public class BounceParams : SeqNodeParams {
        public float duration = 0.5f;
        public float bounceScale = 1.2f;
        [Range(0.1f, 0.9f)] public float upFraction = 0.5f;
        public Ease easeOut = Ease.OutQuad;
        public Ease easeIn = Ease.InQuad;
    }
}
