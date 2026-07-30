using System;
using UnityEngine;
using static UnityEditor.PlayerSettings;

// Suggestion: [Alta] - Clase con muchas responsabilidades: movimiento, sincronización de visuales de ruedas, gestión de input, manejo de combustible y reset de posición.
[RequireComponent(typeof(Rigidbody), typeof(GasSystem))]
public class CarController : MonoBehaviour
{
    public event Action<float> OnSpeedChange;

    [Header("Wheel Colliders")]
    [SerializeField] private WheelCollider _wheelFR;
    [SerializeField] private WheelCollider _wheelFL;
    [SerializeField] private WheelCollider _wheelRR;
    [SerializeField] private WheelCollider _wheelRL;

    [Header("Wheel Visuals")]
    [SerializeField] private Transform _wheelVisualFR;
    [SerializeField] private Transform _wheelVisualFL;
    [SerializeField] private Transform _wheelVisualRR;
    [SerializeField] private Transform _wheelVisualRL;

    [Header("Settings")]
    [SerializeField] private CarSettingsSO _data;
    [SerializeField] private KeyBindingsSO _keys;

    private float _accelerationForce;
    private float _brakeForce;
    private float _steeringForce;

    private Rigidbody _rb;
    private GasSystem _gasSystem;

    private bool _canMove = true;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _gasSystem = GetComponent<GasSystem>();
    }

    private void OnEnable()
    {
        _gasSystem.OnGasEmpty += OnGasEmpty_StopMoving;
    }

    private void Start()
    {
        _rb.mass = _data.Weight;
        _canMove = true;
    }

    private void Update()
    {
        if (_canMove)
            _accelerationForce = Input.GetAxis("Vertical") * _data.HorsePower;

        _brakeForce = Input.GetAxis("Brake") * _data.BrakePower;
        _steeringForce = Input.GetAxis("Horizontal") * _data.SteeringPower;

        if (Input.GetKeyDown(_keys.restartCar))
            RestartCar();

        OnSpeedChange?.Invoke(_rb.linearVelocity.magnitude * 3.6f);
    }

    private void FixedUpdate()
    {
        Steer();
        Accelerate();
        Brake();
        FixRotation();
    }

    private void LateUpdate()
    {
        SyncVisuals(_wheelFR, _wheelVisualFR);
        SyncVisuals(_wheelFL, _wheelVisualFL);
        SyncVisuals(_wheelRR, _wheelVisualRR);
        SyncVisuals(_wheelRL, _wheelVisualRL);
    }

    private void OnDisable()
    {
        _gasSystem.OnGasEmpty -= OnGasEmpty_StopMoving;
    }

    private void Accelerate()
    {
        if (_rb.linearVelocity.magnitude >= _data.MaxSpeed)
            _accelerationForce = _data.MaxSpeed;

        if (_data.DrivingType != DrivingType.RWD)
        {
            _wheelFR.motorTorque = _accelerationForce;
            _wheelFL.motorTorque = _accelerationForce;
        }

        if (_data.DrivingType != DrivingType.FWD)
        {
            _wheelRR.motorTorque = _accelerationForce;
            _wheelRL.motorTorque = _accelerationForce;
        }
    }

    private void Brake()
    {
        _wheelFR.brakeTorque = _brakeForce;
        _wheelFL.brakeTorque = _brakeForce;
        _wheelRR.brakeTorque = _brakeForce;
        _wheelRL.brakeTorque = _brakeForce;
    }

    private void Steer()
    {
        if (_steeringForce < 0f) // Steering left
        {
            _wheelFR.steerAngle = _steeringForce;
            _wheelFL.steerAngle = _steeringForce * _data.SteeringCompensation;
        }
        else // Steering right
        {
            _wheelFR.steerAngle = _steeringForce * _data.SteeringCompensation;
            _wheelFL.steerAngle = _steeringForce;
        }
    }

    private void FixRotation()
    {
        if (Mathf.Abs(_steeringForce) < 0.01f)
        {
            Quaternion currentRot = _rb.rotation;

            Quaternion targetRot = Quaternion.Euler(0f, currentRot.eulerAngles.y, 0f);

            _rb.rotation = Quaternion.Slerp(currentRot, targetRot, _data.TimeToResetRotation * Time.fixedDeltaTime);
        }
    }

    private void RestartCar()
    {
        _rb.linearVelocity = Vector3.zero;
        _rb.angularVelocity = Vector3.zero;

        _wheelFR.steerAngle = 0;
        _wheelFL.steerAngle = 0;

        transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
    }

    private void SyncVisuals(WheelCollider coll, Transform visual)
    {
        coll.GetWorldPose(out Vector3 pos, out Quaternion rot);
        visual.SetPositionAndRotation(pos, rot);
    }

    private void OnGasEmpty_StopMoving()
    {
        _canMove = false;
        _accelerationForce = 0f;
    }

    public Vector3 GetSpeed() => _rb.linearVelocity;
}