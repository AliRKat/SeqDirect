#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using SeqDirect.Core;
using SeqDirect.Core.Graph;

namespace SeqDirect.Editor {
    public class SeqGraphView : GraphView {
        public SeqGraphView() {
            var grid = new GridBackground();
            Insert(0, grid);
            grid.StretchToParentSize();

            this.AddManipulator(new ContentZoomer());
            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());

            style.flexGrow = 1;
        }

        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter) {
            var compatible = new List<Port>();
            ports.ForEach(port => {
                if (startPort == port) return;
                if (startPort.node == port.node) return;
                if (startPort.direction == port.direction) return;
                if (startPort.orientation != port.orientation) return;
                compatible.Add(port);
            });
            return compatible;
        }

        public SeqGraphNodeView CreateNode(string typeName, Vector2 position) {
            if (!Enum.TryParse(typeName, out SeqNodeType type))
                type = SeqNodeType.SeqNode;
            return CreateNode(type, position);
        }

        public SeqGraphNodeView CreateNode(SeqNodeType type, Vector2 position) {
            var node = new SeqGraphNodeView(type);
            node.SetPosition(new Rect(position, new Vector2(200, 180)));
            AddElement(node);
            return node;
        }

        public void SaveToAsset(SeqGraphAsset asset) {
            asset.nodes.Clear();
            asset.connections.Clear();

            foreach (var n in nodes.ToList()) {
                if (n is not SeqGraphNodeView nodeView) continue;

                asset.nodes.Add(new SeqGraphNodeData {
                    id = nodeView.Guid,
                    nodeType = nodeView.NodeType,
                    position = nodeView.GetPosition().position,
                    waitForCompletion = nodeView.waitForCompletion,
                    delay = nodeView.delay,
                    duration = nodeView.duration,
                    customValue = nodeView.customValue,
                    serializedParams = nodeView.SerializedParams
                });
            }

            foreach (var e in edges.ToList()) {
                if (e.output?.node is not SeqGraphNodeView outNode) continue;
                if (e.input?.node is not SeqGraphNodeView inNode) continue;

                asset.connections.Add(new SeqGraphConnection {
                    fromNodeId = outNode.Guid,
                    toNodeId = inNode.Guid
                });
            }

            UnityEditor.EditorUtility.SetDirty(asset);
            UnityEditor.AssetDatabase.SaveAssets();
        }

        public void LoadFromAsset(SeqGraphAsset asset) {
            DeleteElements(graphElements.ToList());

            foreach (var n in asset.nodes) {
                var node = new SeqGraphNodeView(n.nodeType);
                node.ApplyData(n);
                node.SetPosition(new Rect(n.position, new Vector2(200, 180)));
                AddElement(node);
            }

            foreach (var e in asset.connections) {
                var outNode = nodes.ToList().FirstOrDefault(x => (x as SeqGraphNodeView)?.Guid == e.fromNodeId);
                var inNode  = nodes.ToList().FirstOrDefault(x => (x as SeqGraphNodeView)?.Guid == e.toNodeId);
                if (outNode == null || inNode == null) continue;

                var outPort = (outNode as SeqGraphNodeView).output;
                var inPort  = (inNode as SeqGraphNodeView).input;
                AddElement(outPort.ConnectTo(inPort));
            }
        }
    }
}
#endif
