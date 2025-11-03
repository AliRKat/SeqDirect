using System;
using System.Collections.Generic;
using UnityEngine;

namespace SeqDirect.Core {
    /// <summary>
    /// Base class for all nodes in SeqDirect. Nodes are atomic animation or logic actions.
    /// Generic TData stores serializable parameters unique to that node type.
    /// </summary>
    [Serializable]
    public abstract class SeqNodeBase<TData> : ISeqNode where TData : SeqNodeParams, new() {
        [SerializeField] protected string nodeLabel;
        [SerializeField] protected TData data = new();

        public string NodeLabel => nodeLabel;
        public TData Params => data;
        public abstract float Duration { get; }

        /// <summary>
        /// Execute the node for the given targets.
        /// </summary>
        public abstract void Execute(List<Transform> targets);

        /// <summary>
        /// Optional cleanup after execution.
        /// </summary>
        public virtual void Stop() { }

        /// <summary>
        /// Creates a new instance with cloned parameter data.
        /// </summary>
        public virtual ISeqNode Clone() {
            var copy = (SeqNodeBase<TData>)Activator.CreateInstance(GetType());
            copy.nodeLabel = nodeLabel;
            copy.data = JsonUtility.FromJson<TData>(JsonUtility.ToJson(data));
            return copy;
        }
    }

    /// <summary>
    /// Serializable parameter container for nodes. Extend for each node type.
    /// </summary>
    [Serializable]
    public class SeqNodeParams {
        [Tooltip("Optional delay before node executes (in seconds).")]
        public float delay;

        [Tooltip("If true, the next node will wait for this to finish.")]
        public bool waitForCompletion = true;

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
        void Execute(List<Transform> targets);
        void Stop();
        ISeqNode Clone();
    }
}
