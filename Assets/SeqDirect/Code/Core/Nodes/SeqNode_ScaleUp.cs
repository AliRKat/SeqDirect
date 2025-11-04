using UnityEngine;
#if DOTWEEN_PRESENT
using DG.Tweening;
#endif

namespace SeqDirect.Core.Nodes {
    [SeqNode("ScaleUp", Category = "Transform", Description = "Scales up a transform to the target scale.")]
    public class SeqNode_ScaleUp : SeqNodeBase<ScaleUpParams> {
        public override float Duration => data.duration;

        protected override Tween BuildTween(Transform t) {
#if !DOTWEEN_PRESENT
            SeqLogger.DependencyMissing("DOTween", "ScaleUp");
            return null;
#endif
            if (t == null) return null;
            var start = data.fromScale <= 0f ? t.localScale.x : data.fromScale;
            var end = data.toScale <= 0f ? t.localScale.x : data.toScale;

            // uniform scale tween
            t.localScale = Vector3.one * start;
            return t.DOScale(end, data.duration).SetEase(data.ease);
        }
    }

    [System.Serializable]
    public class ScaleUpParams : SeqNodeParams {
        [Tooltip("Duration of the scale animation.")]
        public float duration = 0.5f;

        [Tooltip("Optional starting scale multiplier.")]
        public float fromScale = 1f;

        [Tooltip("Final scale multiplier.")]
        public float toScale = 1.2f;

        [Tooltip("Tween easing type.")]
        public Ease ease = Ease.OutBack;
    }
}
