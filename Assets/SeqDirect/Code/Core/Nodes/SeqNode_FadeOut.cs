using System.Collections.Generic;
using UnityEngine;
#if DOTWEEN_PRESENT
using DG.Tweening;
#endif

namespace SeqDirect.Core.Nodes {
    [SeqNode("FadeOut", Category = "UI", Description = "Fades out UI elements or CanvasGroups.")]
    public class SeqNode_FadeOut : SeqNodeBase<FadeOutParams> {
        public override float Duration => data.duration;

        public override void Execute(List<Transform> targets) {
            foreach (var t in targets) {
                if (t == null) continue;

                // Try CanvasGroup first
                if (t.TryGetComponent(out CanvasGroup cg)) {
#if DOTWEEN_PRESENT
                    cg.DOFade(data.targetAlpha, data.duration).SetEase(data.ease);
#else
                    t.gameObject.AddComponent<FadeOutRuntime>().Init(cg, data);
#endif
                    continue;
                }

                // Try Image or Graphic fallback
                var img = t.GetComponent<UnityEngine.UI.Graphic>();
                if (img != null) {
#if DOTWEEN_PRESENT
                    img.DOFade(data.targetAlpha, data.duration).SetEase(data.ease);
#else
                    t.gameObject.AddComponent<FadeOutRuntime>().Init(img, data);
#endif
                }
            }
        }
    }

    [System.Serializable]
    public class FadeOutParams : SeqNodeParams {
        [Range(0, 1)] public float targetAlpha = 0f;
        [Range(0.05f, 5f)] public float duration = 0.5f;
        public Ease ease = Ease.OutQuad;
    }

    /// <summary>
    /// Lightweight fallback for when DOTween is unavailable.
    /// </summary>
    internal class FadeOutRuntime : MonoBehaviour {
        private CanvasGroup _cg;
        private UnityEngine.UI.Graphic _graphic;
        private FadeOutParams _data;
        private float _elapsed;
        private float _startAlpha;

        public void Init(CanvasGroup cg, FadeOutParams data) {
            _cg = cg;
            _data = data;
            _startAlpha = cg.alpha;
        }

        public void Init(UnityEngine.UI.Graphic g, FadeOutParams data) {
            _graphic = g;
            _data = data;
            _startAlpha = g.color.a;
        }

        private void Update() {
            if (_data == null) { Destroy(this); return; }
            _elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(_elapsed / _data.duration);
            float newAlpha = Mathf.Lerp(_startAlpha, _data.targetAlpha, t);

            if (_cg != null) _cg.alpha = newAlpha;
            if (_graphic != null) {
                var c = _graphic.color;
                c.a = newAlpha;
                _graphic.color = c;
            }

            if (t >= 1f) Destroy(this);
        }
    }
}
