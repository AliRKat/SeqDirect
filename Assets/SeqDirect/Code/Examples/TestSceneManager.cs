using System.Collections.Generic;
using SeqDirect.Core;
using UnityEngine;

public class TestSceneManager : MonoBehaviour {
    public Transform myImage;

    void Update() {
        if (Input.GetKeyDown(KeyCode.Alpha1))
            SeqDirector.Instance.Play("IntroSequence", new List<Transform> { myImage });

        if (Input.GetKeyDown(KeyCode.Space))
            SeqDirector.Instance.StopAll();
    }
}
