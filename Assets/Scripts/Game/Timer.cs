using System;
using UnityEngine;

public class Timer : MonoBehaviour
{
    public event Action<float> OnTimerUpdated;
    public event Action OnTimerFinished;

    [SerializeField] private LevelDataSO _levelData;

    private float _time;
    private bool _hasAlreadyFinished = false;

    private void Start()
    {
        _time = _levelData.MaxTime;
    }

    private void Update()
    {
        if (_time > 0)
        {
            _time -= Time.deltaTime;
            OnTimerUpdated?.Invoke(_time);
        }
        else
        {
            if (!_hasAlreadyFinished)
            {
                _time = 0.0f;
                OnTimerUpdated?.Invoke(_time);
                OnTimerFinished?.Invoke();
                _hasAlreadyFinished = true;
            }
        }
    }
}