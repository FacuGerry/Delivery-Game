using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

// Suggestion: [Alta] - Clase con muchas responsabilidades: movimiento, sincronización de visuales de ruedas, gestión de input, manejo de combustible y reset de posición.
[RequireComponent(typeof(Rigidbody), typeof(GasSystem))]
public class CarController : MonoBehaviour
{
    public event Action<float> OnSpeedChange;

    [Header("Input actions")]
    [SerializeField] private InputActionReference _accelerate;
    [SerializeField] private InputActionReference _brake;
    [SerializeField] private InputActionReference _reverse;
    [SerializeField] private InputActionReference _steer;
    [SerializeField] private InputActionReference _drift;
    [SerializeField] private InputActionReference _reset;

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

    private float _accelerationForce = 0f;
    private float _brakeForce = 0f;
    private float _steeringForce;

    private float _endAcceleration;
    private float _endBrake;
    private float _endReverse;
    private float _endSteer;

    private bool _isDrifting;

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

        EnableAction(_accelerate, CaptureAccelerate);
        EnableAction(_brake, CaptureBrake);
        EnableAction(_reverse, CaptureReverse);
        EnableAction(_steer, CaptureSteer);
        EnableAction(_drift, CaptureDrift);
        EnableAction(_reset, RestartCar);
    }

    private void Start()
    {
        _rb.mass = _data.Weight;
        _canMove = true;
    }

    private void Update()
    {
        _accelerationForce = Mathf.Lerp(_accelerationForce, (_endAcceleration - _endReverse), _data.AccelerationSmoothness * Time.deltaTime);

        float finalSteerAngle = _isDrifting ? _endSteer * _data.DriftingPower : _endSteer;
        _steeringForce = Mathf.Lerp(_steeringForce, finalSteerAngle, _data.SteerSmoothness * Time.deltaTime);

        if (_endBrake == 0f)
            _brakeForce = 0f;
        else
            _brakeForce = Mathf.Lerp(_brakeForce, _endBrake, _data.BrakeSmoothness * Time.deltaTime);

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

        DisableAction(_accelerate, CaptureAccelerate);
        DisableAction(_brake, CaptureBrake);
        DisableAction(_reverse, CaptureReverse);
        DisableAction(_steer, CaptureSteer);
        DisableAction(_drift, CaptureDrift);
        DisableAction(_reset, RestartCar);
    }

    private void EnableAction(InputActionReference actionRef, Action<InputAction.CallbackContext> callback)
    {
        if (actionRef != null && actionRef.action != null)
        {
            actionRef.action.Enable();
            actionRef.action.started += callback;
            actionRef.action.performed += callback;
            actionRef.action.canceled += callback;
        }
    }

    private void DisableAction(InputActionReference actionRef, Action<InputAction.CallbackContext> callback)
    {
        if (actionRef != null && actionRef.action != null)
        {
            actionRef.action.started -= callback;
            actionRef.action.performed -= callback;
            actionRef.action.canceled -= callback;
            actionRef.action.Disable();
        }
    }

    private void CaptureAccelerate(InputAction.CallbackContext obj)
    {
        float rawAcceleration = obj.ReadValue<float>();
        _endAcceleration = rawAcceleration * _data.HorsePower;
    }

    private void CaptureBrake(InputAction.CallbackContext obj)
    {
        float rawBrake = _brake.action.ReadValue<float>();
        _endBrake = rawBrake * _data.BrakePower;
    }

    private void CaptureReverse(InputAction.CallbackContext obj)
    {
        float rawReverse = _reverse.action.ReadValue<float>();
        _endReverse = rawReverse * _data.HorsePower;
    }

    private void CaptureSteer(InputAction.CallbackContext obj)
    {
        float rawSteer = _steer.action.ReadValue<Vector2>().x;
        _endSteer = rawSteer * _data.SteeringPower;
    }

    private void CaptureDrift(InputAction.CallbackContext obj)
    {
        float rawDrift = _drift.action.ReadValue<float>();
        _isDrifting = !(rawDrift < 0.5f && rawDrift > -0.5f);
    }


    private void Accelerate()
    {
        if (_rb.linearVelocity.magnitude >= _data.MaxSpeed || _brakeForce > 0.01f)
            _accelerationForce = 0f;

        if (_isDrifting)
        {
            _wheelFR.motorTorque = 0.0f;
            _wheelFL.motorTorque = 0.0f;
        }

        if (_data.DrivingType != DrivingType.RWD || !_isDrifting)
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
        if (_brakeForce < 0.01f)
            _brakeForce = 0f;

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
        if (Mathf.Abs(_endSteer) < 0.01f)
        {
            Quaternion currentRot = _rb.rotation;

            Quaternion targetRot = Quaternion.Euler(0f, currentRot.eulerAngles.y, 0f);

            _rb.rotation = Quaternion.Slerp(currentRot, targetRot, _data.TimeToResetRotation * Time.fixedDeltaTime);
        }
    }

    private void RestartCar(InputAction.CallbackContext context)
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