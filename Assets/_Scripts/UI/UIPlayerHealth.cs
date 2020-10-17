using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class UIPlayerHealth : MonoBehaviour
{
    [SerializeField] Health _health;
    Slider _slider;

    Tween _tween;

    private void Awake()
    {
        _slider = GetComponent<Slider>();

        _health.OnHit += OnUpdateHealth;
        _health.OnDeath += OnUpdateHealth;
    }

    void OnUpdateHealth(float percentage)
    {
        if (_tween != null)
            _tween.Kill();

        _tween = _slider.DOValue(percentage, 0.5f).Play();
    }
}
