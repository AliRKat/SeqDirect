using UnityEngine;
#if DOTWEEN_PRESENT
using DG.Tweening;
#endif

namespace SeqDirect.Core.Nodes {
    [SeqNode("Stamp", Category = "Transform", Description = "Stamps element with a rotation and scale punch.")]
    public class SeqNode_Stamp : SeqNodeBase<StampParams> {
        public override float Duration => data.duration;

        protected override Tween BuildTween(Transform t) {
#if !DOTWEEN_PRESENT
            SeqLogger.DependencyMissing("DOTween", "Stamp");
            return null;
#endif
            if (t == null) return null;

            t.localRotation = Quaternion.Euler(0, 0, data.startRotation);
            t.localScale = Vector3.one * data.startScale;

            var seq = DOTween.Sequence();
            seq.Append(t.DOScale(Vector3.one, data.duration * (1 - data.reboundFraction)).SetEase(Ease.OutBack));
            seq.Append(t.DOScale(Vector3.one * data.reboundScale, data.duration * data.reboundFraction).SetEase(Ease.InOutSine));
            seq.Join(t.DORotate(Vector3.zero, data.duration, RotateMode.FastBeyond360).SetEase(Ease.OutCubic));
            return seq;
        }
    }

    [System.Serializable]
    public class StampParams : SeqNodeParams {
        public float duration = 0.6f;
        public float startScale = 1.3f;
        public float startRotation = -90f;
        public float reboundScale = 0.95f;
        [Range(0.1f, 0.9f)] public float reboundFraction = 0.2f;
    }
}
