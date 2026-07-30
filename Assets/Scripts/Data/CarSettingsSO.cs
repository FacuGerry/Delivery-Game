using UnityEngine;

[CreateAssetMenu(fileName = "CarSettings", menuName = "Car/CarSettings")]
public class CarSettingsSO : ScriptableObject
{
    [Header("Car GameObject")]
    [SerializeField] private GameObject _prefab;
    [SerializeField] private GameObject _visual;

    [Header("Stats")]
    [Min(0.01f)][SerializeField] private float _maxSpeed;
    [Min(0.01f)][SerializeField] private float _weight;
    [Min(0.01f)][SerializeField] private float _horsePower;
    [Min(0.01f)][SerializeField] private float _brakePower;
    [SerializeField] private float _timeToResetRotation;

    [Header("Steering")]
    [Min(0.01f)][SerializeField] private float _steeringPower;
    [Min(1.01f)][SerializeField] private float _steeringCompensation;

    [Header("Driving type")]
    [SerializeField] private DrivingType _drivingType;

    [Header("Durability settings")]
    [SerializeField] private float _maxDurability;
    [SerializeField] private float _durabilityToCharge;

    [Header("Gas settings")]
    [SerializeField] private float _maxGas;
    [SerializeField] private float _gasUsage;
    [SerializeField] private float _gasToCharge;

    // GETTERS
    public GameObject Prefab => _prefab;
    public GameObject Visual => _visual;
    public float MaxSpeed => _maxSpeed;
    public float Weight => _weight;
    public float HorsePower => _horsePower;
    public float BrakePower => _brakePower;
    public float TimeToResetRotation => _timeToResetRotation;
    public float SteeringPower => _steeringPower;
    public float SteeringCompensation => _steeringCompensation;
    public DrivingType DrivingType => _drivingType;
    public float MaxDurability => _maxDurability;
    public float DurabilityToCharge => _durabilityToCharge;
    public float MaxGas => _maxGas;
    public float GasUsage => _gasUsage;
    public float GasToCharge => _gasToCharge;
}