using UnityEngine;
using UnityEngine.UI;

public class UiTime : MonoBehaviour
{
    [SerializeField] private Timer _timer;
    [SerializeField] private Image _imageTimer;
    [SerializeField] private LevelDataSO _levelData;

    private void OnEnable()
    {
        _timer.OnTimerUpdated += UpdateTime;
    }

    private void OnDisable()
    {
        _timer.OnTimerUpdated -= UpdateTime;
    }

    private void UpdateTime(float time) => _imageTimer.fillAmount = time / _levelData.MaxTime;
}