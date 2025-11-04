using UnityEngine;
#if DOTWEEN_PRESENT
using DG.Tweening;
#endif

namespace SeqDirect.Core.Nodes {
    [SeqNode("ScaleDown", Category = "Transform", Description = "Scales down a transform to the target scale.")]
    public class SeqNode_ScaleDown : SeqNodeBase<ScaleDownParams> {
        public override float Duration => data.duration;

        protected override Tween BuildTween(Transform t) {
#if !DOTWEEN_PRESENT
            SeqLogger.DependencyMissing("DOTween", "ScaleDown");
            return null;
#endif
            if (t == null) return null;
            var start = data.fromScale <= 0f ? t.localScale.x : data.fromScale;
            var end = data.toScale <= 0f ? t.localScale.x : data.toScale;

            t.localScale = Vector3.one * start;
            return t.DOScale(end, data.duration).SetEase(data.ease);
        }
    }

    [System.Serializable]
    public class ScaleDownParams : SeqNodeParams {
        [Tooltip("Duration of the scale animation.")]
        public float duration = 0.5f;

        [Tooltip("Optional starting scale multiplier.")]
        public float fromScale = 1f;

        [Tooltip("Final scale multiplier.")]
        public float toScale = 0.8f;

        [Tooltip("Tween easing type.")]
        public Ease ease = Ease.InBack;
    }
}
