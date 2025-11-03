using System.Collections.Generic;
using SeqDirect.Core;
using SeqDirect.Core.Nodes;
using UnityEngine;

public class TestSceneManager : MonoBehaviour {
    public Transform myImage;
    void Start() {
        var node = (SeqNode_FadeOut)SeqNodeRegistry.Create("FadeOut");
        node.Params.duration = 1.5f;
        node.Params.targetAlpha = 0f;
        node.Execute(new List<Transform> { myImage });
    }
}
