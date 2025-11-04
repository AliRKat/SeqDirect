using System;
using System.Collections.Generic;
using UnityEngine;
#if DOTWEEN_PRESENT
using DG.Tweening;
#endif

namespace SeqDirect.Core {
    /// <summary>
    /// Central runtime controller for SeqDirect.
    /// Handles node creation, sequencing, and lifecycle control.
    /// </summary>
    [AddComponentMenu("SeqDirect/SeqDirector")]
    public class SeqDirector : MonoBehaviour {
        private readonly List<ISeqNode> _activeNodes = new();
        private static SeqDirector _instance;

        public static SeqDirector Instance {
            get {
                if (_instance == null) {
                    var go = new GameObject("[SeqDirector]");
                    _instance = go.AddComponent<SeqDirector>();
                    DontDestroyOnLoad(go);
                }
                return _instance;
            }
        }

        /// <summary>
        /// Plays a single node immediately with custom setup and targets.
        /// </summary>
        public T PlayNode<T>(Action<T> setup, List<Transform> targets) where T : ISeqNode {
            var id = typeof(T).Name.Replace("SeqNode_", "");
            if (SeqNodeRegistry.Create(id) is not T node) {
                SeqLogger.Error($"Failed to create node of type {typeof(T).Name}");
                return default;
            }

            setup?.Invoke(node);
            node.Execute(targets);
            _activeNodes.Add(node);
            return node;
        }

        /// <summary>
        /// Plays a node by string id (e.g. "FadeOut") with optional setup.
        /// </summary>
        public ISeqNode PlayNode(string id, Action<ISeqNode> setup, List<Transform> targets) {
            var node = SeqNodeRegistry.Create(id);
            if (node == null) {
                SeqLogger.Error($"No node found for id: {id}");
                return null;
            }

            setup?.Invoke(node);
            node.Execute(targets);
            _activeNodes.Add(node);
            return node;
        }

        /// <summary>
        /// Plays a series of nodes in sequence (timeline).
        /// Each node executes after the previous one finishes if waitForCompletion is true.
        /// </summary>
        public void PlaySequence(List<ISeqNode> nodes, List<Transform> targets) {
#if !DOTWEEN_PRESENT
            SeqLogger.DependencyMissing("DOTween", "SeqDirector Sequence");
            return;
#endif
            if (nodes == null || nodes.Count == 0) {
                SeqLogger.Warn("Tried to play an empty sequence.");
                return;
            }

            var seq = DOTween.Sequence();
            foreach (var node in nodes) {
                if (node == null) continue;

                seq.AppendCallback(() => node.Execute(targets));

                // ✅ Artık Params doğrudan ISeqNode üstünden erişiliyor
                if (node.Params.waitForCompletion)
                    seq.AppendInterval(node.Duration + node.Params.delay);
            }

            seq.OnComplete(() => SeqLogger.Info("Sequence complete."));
        }

        public void StopAll() {
            foreach (var n in _activeNodes)
                n.Kill();
            _activeNodes.Clear();
            SeqLogger.Info("All active nodes stopped.");
        }

        public void PauseAll() {
            foreach (var n in _activeNodes)
                n.Pause();
            SeqLogger.Info("All active nodes paused.");
        }

        public void ResumeAll() {
            foreach (var n in _activeNodes)
                n.Resume();
            SeqLogger.Info("All active nodes resumed.");
        }
    }
}
