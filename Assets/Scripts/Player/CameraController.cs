using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    [Header("Player Pref Keys")]
    [SerializeField] private PlayerPrefKeysSO _playerPrefKeys;

    [Header("Car")]
    [SerializeField] private Transform _carTransform;
    [SerializeField] private Transform _followPosition;

    [Header("Base sensitivity")]
    [SerializeField][Min(1.0f)] private float _sensHor;
    [SerializeField][Min(1.0f)] private float _sensVer;

    [Header("InputAction")]
    [SerializeField] private InputActionReference _look;

    private float _yaw;
    private float _pitch;

    private Vector2 _mouseInput;

    private void Awake()
    {
        if (!PlayerPrefs.HasKey(_playerPrefKeys.CameraSensHor) || _sensHor != PlayerPrefs.GetFloat(_playerPrefKeys.CameraSensHor))
            PlayerPrefs.SetFloat(_playerPrefKeys.CameraSensHor, _sensHor);
        else
            _sensHor = PlayerPrefs.GetFloat(_playerPrefKeys.CameraSensHor);

        if (!PlayerPrefs.HasKey(_playerPrefKeys.CameraSensVer) || _sensVer != PlayerPrefs.GetFloat(_playerPrefKeys.CameraSensVer))
            PlayerPrefs.SetFloat(_playerPrefKeys.CameraSensVer, _sensVer);
        else
            _sensVer = PlayerPrefs.GetFloat(_playerPrefKeys.CameraSensVer);
    }

    private void OnEnable()
    {
        _look.action.Enable();
        _look.action.started += ViewCaptured;
        _look.action.performed += ViewCaptured;
        _look.action.canceled += ViewCaptured;
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        FollowCar();
    }

    private void Update()
    {
        FollowCar();
        //LookAround();
    }

    private void OnDisable()
    {
        _look.action.started -= ViewCaptured;
        _look.action.performed -= ViewCaptured;
        _look.action.canceled -= ViewCaptured;
        _look.action.Disable();
    }

    private void FollowCar()
    {
        transform.position = _followPosition.position;
        transform.LookAt(_carTransform);
    }

    private void ViewCaptured(InputAction.CallbackContext obj) => _mouseInput = obj.ReadValue<Vector2>();

    private void LookAround()
    {
        float mouseX = _mouseInput.x * Time.deltaTime * _sensHor;

        _yaw += mouseX;

        float mouseY = -_mouseInput.y * Time.deltaTime * _sensVer;

        _pitch += mouseY;

        transform.rotation = Quaternion.Euler(_carTransform.eulerAngles.x + _pitch, _carTransform.eulerAngles.y + _yaw, 0f);
    }
}