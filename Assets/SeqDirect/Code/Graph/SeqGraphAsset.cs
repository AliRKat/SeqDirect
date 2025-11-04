using System;
using System.Collections.Generic;
using UnityEngine;

namespace SeqDirect.Core.Graph {
    [CreateAssetMenu(menuName = "SeqDirect/Graph Asset", fileName = "New_SeqGraph")]
    public class SeqGraphAsset : ScriptableObject {
        [SerializeField] public string graphId = "NewGraph";
        [SerializeField] public List<SeqGraphNodeData> nodes = new();
        [SerializeField] public List<SeqGraphConnection> connections = new();

        public void Clear() {
            nodes.Clear();
            connections.Clear();
        }
    }

    [Serializable]
    public class SeqGraphConnection {
        public string fromNodeId;
        public string toNodeId;
    }
}
