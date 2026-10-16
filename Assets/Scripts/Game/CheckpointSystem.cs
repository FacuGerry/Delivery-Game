using System;
using UnityEngine;

public class CheckpointSystem : MonoBehaviour
{
    public event Action OnEndOfLevelReached;

    [Header("Checkpoints array")]
    [SerializeField] private Transform _start;
    [SerializeField] private Transform[] _checkpoints;
    [SerializeField] private Transform _end;

    [Header("Player")]
    [SerializeField] private CarController _player;

    private Transform _currentCheckpoint;

    private void Start()
    {
        ChangeCurrentCheckpoint(_start);
    }

    private void OnEnable()
    {
        _player.OnCarRestarted += ResetPlayerToCurrentCheckpoint;
    }

    private void OnDisable()
    {
        _player.OnCarRestarted -= ResetPlayerToCurrentCheckpoint;
    }

    private void ChangeCurrentCheckpoint(Transform newCheckpoint)
    {
        _currentCheckpoint = newCheckpoint;
        if (newCheckpoint == _end)
            OnEndOfLevelReached?.Invoke();
    }

    private void ResetPlayerToCurrentCheckpoint(Transform player) => player.SetPositionAndRotation(_currentCheckpoint.position, _currentCheckpoint.rotation);
}