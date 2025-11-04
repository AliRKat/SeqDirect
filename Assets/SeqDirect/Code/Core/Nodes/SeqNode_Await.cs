using UnityEngine;
#if DOTWEEN_PRESENT
using DG.Tweening;
#endif

namespace SeqDirect.Core.Nodes {
    [SeqNode("AwaitNode", Category = "Control", Description = "Waits for a specific amount of time before continuing the sequence.")]
    public class SeqNode_Await : SeqNodeBase<AwaitParams> {
        public override float Duration => data.waitTime;

        protected override Tween BuildTween(Transform t) {
#if !DOTWEEN_PRESENT
            SeqLogger.DependencyMissing("DOTween", "Await");
            return null;
#endif
            // DOTween interval tween (dummy target)
            return DOVirtual.DelayedCall(data.waitTime, () => { }).SetEase(Ease.Linear);
        }
    }

    [System.Serializable]
    public class AwaitParams : SeqNodeParams {
        [Tooltip("Time (in seconds) to wait before continuing the sequence.")]
        public float waitTime = 1f;
    }
}
