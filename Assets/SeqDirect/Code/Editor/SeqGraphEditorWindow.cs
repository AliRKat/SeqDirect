#if UNITY_EDITOR
using SeqDirect.Core.Graph;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

namespace SeqDirect.Editor {
    public class SeqGraphEditorWindow : EditorWindow {
        private SeqGraphView _graphView;
        private SeqGraphAsset _currentAsset;

        [MenuItem("Window/SeqDirect/Graph Editor")]
        public static void OpenWindow() {
            var wnd = GetWindow<SeqGraphEditorWindow>();
            wnd.titleContent = new GUIContent("SeqDirect Graph Editor");
            wnd.Show();
        }

        private void OnEnable() {
            ConstructGraphView();
            GenerateToolbar();
        }

        private void OnDisable() {
            rootVisualElement.Remove(_graphView);
        }

        private void ConstructGraphView() {
            _graphView = new SeqGraphView { name = "SeqGraphView" };
            _graphView.StretchToParentSize();
            rootVisualElement.Add(_graphView);
        }

        private void GenerateToolbar() {
            var container = new VisualElement();
            container.style.flexDirection = FlexDirection.Column;

            // --- Graph Asset Row ---
            var row1 = new Toolbar();
            var objField = new ObjectField("Graph Asset") {
                objectType = typeof(SeqGraphAsset),
                allowSceneObjects = false
            };
            objField.RegisterValueChangedCallback(evt => _currentAsset = evt.newValue as SeqGraphAsset);
            row1.Add(objField);

            // --- Save / Load / Clear / Create ---
            var row2 = new Toolbar();

            var saveButton = new Button(() => {
                if (_currentAsset != null) _graphView.SaveToAsset(_currentAsset);
            }) { text = "Save" };

            var loadButton = new Button(() => {
                if (_currentAsset != null) _graphView.LoadFromAsset(_currentAsset);
            }) { text = "Load" };

            var clearButton = new Button(() => _graphView.DeleteElements(_graphView.graphElements.ToList())) { text = "Clear" };

            var createButton = new Button(() => {
                var asset = CreateInstance<SeqGraphAsset>();
                string path = AssetDatabase.GenerateUniqueAssetPath("Assets/Resources/SeqGraphs/NewSeqGraph.asset");
                AssetDatabase.CreateAsset(asset, path);
                AssetDatabase.SaveAssets();
                _currentAsset = asset;
                Selection.activeObject = asset;
                EditorGUIUtility.PingObject(asset);
            }) { text = "Create Graph" };

            row2.Add(saveButton);
            row2.Add(loadButton);
            row2.Add(clearButton);
            row2.Add(createButton);

            // --- Node Creation Row ---
            var row3 = new Toolbar();
            var addSeqButton = new Button(() => _graphView.CreateNode("SeqNode", new Vector2(250, 250))) {
                text = "Add Seq Node"
            };
            var addAwaitButton = new Button(() => _graphView.CreateNode("AwaitNode", new Vector2(500, 250))) {
                text = "Add Await Node"
            };

            row3.Add(addSeqButton);
            row3.Add(addAwaitButton);

            // --- Layout Assembly ---
            container.Add(row1);
            container.Add(row2);
            container.Add(row3);
            rootVisualElement.Add(container);
        }
    }
}
#endif
