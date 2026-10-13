using UnityEngine;

[CreateAssetMenu(fileName = "CarSettings", menuName = "Car/CarSettings")]

public class LevelDataSO : ScriptableObject
{
    // SETTERS
    [SerializeField] private float _maxTime;


    // GETTERS
    public float MaxTime => _maxTime;
}