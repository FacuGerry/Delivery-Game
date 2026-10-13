using UnityEngine;

[CreateAssetMenu(fileName = "LevelData", menuName = "GameSettings/LevelData")]

public class LevelDataSO : ScriptableObject
{
    // SETTERS
    [SerializeField] private float _maxTime;


    // GETTERS
    public float MaxTime => _maxTime;
}