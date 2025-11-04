using System;
using System.Collections.Generic;
using UnityEngine;
#if DOTWEEN_PRESENT
using DG.Tweening;
#endif

namespace SeqDirect.Core {
    /// <summary>
    /// Base class for all nodes in SeqDirect. Nodes create and manage their own tweens.
    /// Provides lifecycle control (Play, Pause, Kill, Loop).
    /// </summary>
    [Serializable]
    public abstract class SeqNodeBase<TData> : ISeqNode where TData : SeqNodeParams, new() {
        [SerializeField] protected string nodeLabel;
        [SerializeField] protected TData data = new();

#if DOTWEEN_PRESENT
        protected readonly List<Tween> activeTweens = new();
#endif

        public string NodeLabel => nodeLabel;
        public TData Params => data;
        SeqNodeParams ISeqNode.Params => data;
        public abstract float Duration { get; }

        /// <summary>
        /// Builds a DOTween tween for the given transform without playing it.
        /// </summary>
        protected abstract Tween BuildTween(Transform target);

        /// <summary>
        /// Executes the node by building and playing tweens for all targets.
        /// </summary>
        public virtual void Execute(List<Transform> targets) {
#if !DOTWEEN_PRESENT
            SeqLogger.DependencyMissing("DOTween", nodeLabel);
            return;
#endif
            SeqLogger.NodeExecution(nodeLabel, data.delay + Duration);
            Kill(); // clear previous tweens

            foreach (var t in targets) {
                if (t == null) continue;
                var tw = BuildTween(t);
#if DOTWEEN_PRESENT
                if (tw == null) continue;
                tw.SetDelay(data.delay);
                if (data.loopCount > 0)
                    tw.SetLoops(data.loopCount, data.loopType);
                activeTweens.Add(tw.Play());
#endif
            }
        }

        public virtual void Pause() {
#if DOTWEEN_PRESENT
            foreach (var tw in activeTweens)
                tw.Pause();
#endif
        }

        public virtual void Resume() {
#if DOTWEEN_PRESENT
            foreach (var tw in activeTweens)
                tw.Play();
#endif
        }

        public virtual void Kill() {
#if DOTWEEN_PRESENT
            foreach (var tw in activeTweens)
                tw.Kill();
            activeTweens.Clear();
#endif
        }

        public virtual ISeqNode Clone() {
            var copy = (SeqNodeBase<TData>)Activator.CreateInstance(GetType());
            copy.nodeLabel = nodeLabel;
            copy.data = JsonUtility.FromJson<TData>(JsonUtility.ToJson(data));
            return copy;
        }
    }

    /// <summary>
    /// Serializable parameter container for nodes. Includes delay, wait, and loop settings.
    /// </summary>
    [Serializable]
    public class SeqNodeParams {
        [Tooltip("Optional delay before node executes (in seconds)." )]
        public float delay;

        [Tooltip("If true, the next node will wait for this to finish.")]
        public bool waitForCompletion = true;

        [Tooltip("Number of times to loop this node. 0 = no loop.")]
        public int loopCount = 0;

        [Tooltip("DOTween loop type.")]
        public LoopType loopType = LoopType.Restart;

        /// <summary>
        /// Utility clone using JSON serialization.
        /// </summary>
        public virtual SeqNodeParams Clone() {
            var json = JsonUtility.ToJson(this);
            return JsonUtility.FromJson<SeqNodeParams>(json);
        }
    }

    /// <summary>
    /// Common surface for all nodes (runtime interface).
    /// </summary>
    public interface ISeqNode {
        string NodeLabel { get; }
        float Duration { get; }
        SeqNodeParams Params { get; }
        void Execute(List<Transform> targets);
        void Pause();
        void Resume();
        void Kill();
        ISeqNode Clone();
    }
}
