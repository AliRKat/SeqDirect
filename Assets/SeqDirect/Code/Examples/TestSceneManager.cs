using System.Collections.Generic;
using SeqDirect.Core;
using SeqDirect.Core.Nodes;
using UnityEngine;

/// <summary>
/// Manual test harness for SeqDirect runtime API.
/// Press keys to trigger nodes and lifecycle operations.
/// </summary>
public class TestSceneManager : MonoBehaviour {
    [Header("Targets")]
    public Transform uiImage;
    private List<Transform> singleTarget;

    private void Start() {
        singleTarget = new() { uiImage };
    }

    private void Update() {
        // --- Node execution tests ---
        if (Input.GetKeyDown(KeyCode.A)) {
            SeqDirector.Instance.PlayNode<SeqNode_FadeIn>(node => {
                node.Params.duration = 1f;
                node.Params.targetAlpha = 1f;
                node.Params.ease = DG.Tweening.Ease.OutCubic;
            }, singleTarget);
        }

        if (Input.GetKeyDown(KeyCode.S)) {
            SeqDirector.Instance.PlayNode<SeqNode_FadeOut>(node => {
                node.Params.duration = 1f;
                node.Params.targetAlpha = 0f;
                node.Params.ease = DG.Tweening.Ease.InCubic;
            }, singleTarget);
        }

        if (Input.GetKeyDown(KeyCode.D)) {
            SeqDirector.Instance.PlayNode<SeqNode_Show>(node => {
                node.Params.duration = 0.6f;
                node.Params.startScale = 0.5f;
            }, singleTarget);
        }

        if (Input.GetKeyDown(KeyCode.F)) {
            SeqDirector.Instance.PlayNode<SeqNode_Hide>(node => {
                node.Params.duration = 0.6f;
                node.Params.targetScale = 0.5f;
            }, singleTarget);
        }

        if (Input.GetKeyDown(KeyCode.G)) {
            SeqDirector.Instance.PlayNode<SeqNode_Bounce>(node => {
                node.Params.duration = 0.8f;
                node.Params.bounceScale = 1.3f;
            }, singleTarget);
        }

        if (Input.GetKeyDown(KeyCode.H)) {
            SeqDirector.Instance.PlayNode<SeqNode_Stamp>(node => {
                node.Params.duration = 0.8f;
                node.Params.startRotation = -60f;
            }, singleTarget);
        }

        // --- Lifecycle tests ---
        if (Input.GetKeyDown(KeyCode.Z)) {
            SeqDirector.Instance.PauseAll();
        }

        if (Input.GetKeyDown(KeyCode.X)) {
            SeqDirector.Instance.ResumeAll();
        }

        if (Input.GetKeyDown(KeyCode.C)) {
            SeqDirector.Instance.StopAll();
        }
    }
}
