using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GasSystem : MonoBehaviour
{
    public event Action<float> OnGasChange;
    public event Action OnGasEmpty;

    [Header("Data")]
    [SerializeField] private CarSettingsSO _data;
    [SerializeField] private float _consumeInterval = 1f;

    [Header("Wheels")]
    [SerializeField] private List<WheelCollider> _wheels = new();

    private float _gas = 0f;
    private float _timer = 0f;

    private bool _isPlaying = true;

    private IEnumerator _coroutineTakeGas = null;
    private IEnumerator _coroutineAddGas = null;

    private void Start()
    {
        _gas = _data.MaxGas;
        OnGasChange?.Invoke(_gas);

        if (_coroutineTakeGas == null)
            _coroutineTakeGas = Defueling();

        StartCoroutine(_coroutineTakeGas);
    }

    private void OnDestroy()
    {
        _isPlaying = false;
        _coroutineTakeGas = null;
        _coroutineAddGas = null;
        StopAllCoroutines();
    }

    private IEnumerator Defueling()
    {
        while (_isPlaying)
        {
            _timer += Time.deltaTime;

            if (_timer < _consumeInterval)
                yield return null;

            _timer = 0f;

            _gas -= _data.GasUsage * CalculateRPM() / 1000f * Time.deltaTime;
            if (_gas < 0)
            {
                _gas = 0;
                OnGasEmpty?.Invoke();
            }

            OnGasChange?.Invoke(_gas);
        }
    }

    private IEnumerator FuelingUp()
    {
        while (_gas < _data.MaxGas)
        {
            _gas += _data.GasToCharge * Time.deltaTime;

            if (_gas >= _data.MaxGas)
                _gas = _data.MaxGas;

            OnGasChange?.Invoke(_gas);

            yield return null;
        }
        yield return null;
    }

    private float CalculateRPM()
    {
        float rpm = 0;
        foreach (WheelCollider wheel in _wheels)
            rpm += wheel.rpm;

        rpm /= _wheels.Count;

        return Mathf.Abs(rpm);
    }

    public void FuelUp()
    {
        if (_coroutineAddGas != null)
            StopCoroutine(_coroutineAddGas);

        _coroutineAddGas = FuelingUp();
        StartCoroutine(_coroutineAddGas);
    }

    public void StopFuelUp()
    {
        if (_coroutineAddGas != null)
            StopCoroutine(_coroutineAddGas);
    }
}