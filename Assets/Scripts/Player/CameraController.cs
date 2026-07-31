using UnityEngine;

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

    private float _yaw;
    private float _pitch;

    private void Awake()
    {
        if (!PlayerPrefs.HasKey(_playerPrefKeys.CameraSensHor))
            PlayerPrefs.SetFloat(_playerPrefKeys.CameraSensHor, _sensHor);
        else
            _sensHor = PlayerPrefs.GetFloat(_playerPrefKeys.CameraSensHor);

        if (!PlayerPrefs.HasKey(_playerPrefKeys.CameraSensVer))
            PlayerPrefs.SetFloat(_playerPrefKeys.CameraSensVer, _sensVer);
        else
            _sensVer = PlayerPrefs.GetFloat(_playerPrefKeys.CameraSensVer);
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        FollowCar();
    }

    private void Update()
    {
        FollowCar();
        LookAround();
    }

    private void FollowCar() => transform.position = _followPosition.position;

    private void LookAround()
    {
        float mouseX = Input.GetAxis("Mouse X") * Time.deltaTime * _sensHor;

        _yaw += mouseX;

        float mouseY = -Input.GetAxis("Mouse Y") * Time.deltaTime * _sensVer;

        _pitch += mouseY;

        transform.rotation = Quaternion.Euler(_carTransform.eulerAngles.x + _pitch, _carTransform.eulerAngles.y + _yaw, 0f);
    }
}