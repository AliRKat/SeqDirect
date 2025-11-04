#if UNITY_EDITOR
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using SeqDirect.Core;
using SeqDirect.Core.Graph;

namespace SeqDirect.Editor {
    public class SeqGraphNodeView : Node {
        public string Guid = System.Guid.NewGuid().ToString();
        public SeqNodeType NodeType;
        public bool waitForCompletion = true;
        public float delay = 0f;
        public float duration = 0.5f;
        public float customValue = 0f;
        public string SerializedParams = "{}";

        public Port input;
        public Port output;

        public SeqGraphNodeView(SeqNodeType type) {
            NodeType = type;
            title = type.ToString();

            input = InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Multi, typeof(bool));
            input.portName = "In";
            output = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Multi, typeof(bool));
            output.portName = "Out";

            inputContainer.Add(input);
            outputContainer.Add(output);

            var typeField = new EnumField("Node Type", NodeType);
            typeField.RegisterValueChangedCallback(evt => {
                NodeType = (SeqNodeType)evt.newValue;
                title = NodeType.ToString();
                RefreshParamFields();
            });
            mainContainer.Add(typeField);

            var delayField = new FloatField("Delay (s)") { value = delay };
            delayField.RegisterValueChangedCallback(evt => delay = evt.newValue);
            mainContainer.Add(delayField);

            var durationField = new FloatField("Duration (s)") { value = duration };
            durationField.RegisterValueChangedCallback(evt => duration = evt.newValue);
            mainContainer.Add(durationField);

            var waitToggle = new Toggle("Wait") { value = waitForCompletion };
            waitToggle.RegisterValueChangedCallback(evt => waitForCompletion = evt.newValue);
            mainContainer.Add(waitToggle);

            RefreshParamFields();
        }

        public void ApplyData(SeqGraphNodeData data) {
            Guid = data.id;
            NodeType = data.nodeType;
            waitForCompletion = data.waitForCompletion;
            delay = data.delay;
            duration = data.duration;
            customValue = data.customValue;
            SerializedParams = data.serializedParams;

            title = NodeType.ToString();
            RefreshParamFields();
        }

        public void RefreshParamFields() {
            var old = mainContainer.Query<FloatField>("CustomParam").ToList();
            foreach (var f in old)
                mainContainer.Remove(f);

            FloatField cf;
            switch (NodeType) {
                case SeqNodeType.FadeIn:
                case SeqNodeType.FadeOut:
                    cf = new FloatField("Target Alpha") { name = "CustomParam", value = customValue };
                    cf.RegisterValueChangedCallback(evt => customValue = evt.newValue);
                    mainContainer.Add(cf);
                    break;
                case SeqNodeType.AwaitNode:
                    cf = new FloatField("Wait (s)") { name = "CustomParam", value = customValue };
                    cf.RegisterValueChangedCallback(evt => customValue = evt.newValue);
                    mainContainer.Add(cf);
                    break;
                case SeqNodeType.ScaleUp:
                case SeqNodeType.ScaleDown:
                    cf = new FloatField("Scale") { name = "CustomParam", value = customValue == 0f ? 1f : customValue };
                    cf.RegisterValueChangedCallback(evt => customValue = evt.newValue);
                    mainContainer.Add(cf);
                    break;
                default:
                    cf = new FloatField("Value") { name = "CustomParam", value = customValue };
                    cf.RegisterValueChangedCallback(evt => customValue = evt.newValue);
                    mainContainer.Add(cf);
                    break;
            }

            RefreshExpandedState();
        }
    }
}
#endif
