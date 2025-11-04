using System;
using UnityEngine;

namespace SeqDirect.Core.Graph {
    [Serializable]
    public class SeqGraphNodeData {
        public string id = Guid.NewGuid().ToString();
        public SeqNodeType nodeType = SeqNodeType.SeqNode;
        public Vector2 position;
        public float duration;
        public bool waitForCompletion = true;
        public float delay;
        public float customValue;
        public string serializedParams;
    }
}
