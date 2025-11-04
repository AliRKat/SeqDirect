using System.Collections.Generic;
using UnityEngine;
using SeqDirect.Core.Graph;
#if DOTWEEN_PRESENT
using DG.Tweening;
#endif

namespace SeqDirect.Core {
    [AddComponentMenu("SeqDirect/Seq Director")]
    public class SeqDirector : MonoBehaviour {
        public static SeqDirector Instance { get; private set; }
        private readonly Dictionary<string, SeqGraphAsset> _graphLibrary = new();
        private readonly List<ISeqNode> _activeNodes = new();

        private void Awake() {
            if (Instance != null && Instance != this) {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            LoadLibrary();
        }

        private void LoadLibrary() {
            _graphLibrary.Clear();
            var graphs = Resources.LoadAll<SeqGraphAsset>("SeqGraphs");
            foreach (var g in graphs) {
                if (string.IsNullOrEmpty(g.graphId)) continue;
                if (!_graphLibrary.ContainsKey(g.graphId))
                    _graphLibrary.Add(g.graphId, g);
            }
            SeqLogger.Info($"[SeqDirector] Loaded {_graphLibrary.Count} graphs from Resources/SeqGraphs");
        }

#if DOTWEEN_PRESENT
        public void Play(string graphId, List<Transform> targets) {
            if (!_graphLibrary.TryGetValue(graphId, out var asset)) {
                SeqLogger.Warn($"[SeqDirector] Graph '{graphId}' not found in library.");
                return;
            }
            PlayGraph(asset, targets);
        }

        public Sequence PlayGraph(SeqGraphAsset asset, List<Transform> targets) {
            if (asset == null) {
                SeqLogger.Error("[SeqDirector] Null graph asset.");
                return null;
            }

            var seq = DOTween.Sequence();
            seq.SetAutoKill(false);
            seq.Pause();

            foreach (var nodeData in asset.nodes) {
                var node = SeqNodeRegistry.Create(nodeData.nodeType.ToString());
                if (node == null) {
                    SeqLogger.Error($"[SeqDirector] Unknown node type: {nodeData.nodeType}");
                    continue;
                }

                if (node is SeqNodeBase<SeqNodeParams> baseNode)
                    baseNode.ApplyGraphOverrides(nodeData);

                var nodeSeq = DOTween.Sequence();
                foreach (var target in targets) {
                    if (target == null) continue;
                    var method = node.GetType().GetMethod("BuildTween",
                        System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                    if (method == null) continue;

                    var tw = method.Invoke(node, new object[] { target }) as Tween;
                    if (tw != null)
                        nodeSeq.Join(tw);
                }

                if (nodeData.waitForCompletion) {
                    if (nodeData.delay > 0)
                        seq.AppendInterval(nodeData.delay);
                    seq.Append(nodeSeq);
                } else {
                    nodeSeq.SetDelay(nodeData.delay);
                    seq.Join(nodeSeq);
                }

                _activeNodes.Add(node);
                SeqLogger.NodeExecution($"{asset.graphId}/{nodeData.nodeType}", nodeData.duration);
            }

            seq.Play();
            SeqLogger.Info($"[SeqDirector] Playing Graph '{asset.graphId}' with {asset.nodes.Count} nodes.");
            return seq;
        }
#else
        public void Play(string graphId, List<Transform> targets) {
            SeqLogger.DependencyMissing("DOTween", "SeqDirector");
        }
        public void PlayGraph(SeqGraphAsset asset, List<Transform> targets) {
            SeqLogger.DependencyMissing("DOTween", "SeqDirector");
        }
#endif

        public void StopAll() {
#if DOTWEEN_PRESENT
            foreach (var n in _activeNodes)
                n.Kill();
            _activeNodes.Clear();
#endif
        }
    }
}
