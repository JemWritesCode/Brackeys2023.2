using DG.Tweening;

using UnityEngine;
using UnityEngine.EventSystems;

public class HoverButtonEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {
  [field: SerializeField]
  public Vector3 OnHoverScaleValue { get; private set; } = Vector3.one;

  [field: SerializeField, Min(0f)]
  public float OnHoverScaleDuration { get; private set; } = 0.5f;

  private Tweener _onHoverTweener;

  private void Start() {
    _onHoverTweener =
        transform
            .DOScale(OnHoverScaleValue, OnHoverScaleDuration)
            .SetLink(gameObject)
            .SetAutoKill(false)
            .Pause();
  }

  public void OnPointerEnter(PointerEventData eventData) {
    if (_onHoverTweener.IsPlaying()) {
      _onHoverTweener.PlayForward();
    } else {
      _onHoverTweener.Restart();
    }
  }

  public void OnPointerExit(PointerEventData eventData) {
    _onHoverTweener.SmoothRewind();
  }
}
