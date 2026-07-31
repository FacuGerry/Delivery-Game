using UnityEngine;

[CreateAssetMenu(fileName = "PlayerPrefKeys", menuName = "GameSettings/PlayerPrefKeys")]

public class PlayerPrefKeysSO : ScriptableObject
{
    [Header("Camera")]
    [SerializeField] private string _cameraSensHor;
    [SerializeField] private string _cameraSensVer;

    [Header("Audio")]
    [SerializeField] private string _volumeMaster;
    [SerializeField] private string _volumeMusic;
    [SerializeField] private string _volumeSfx;
    [SerializeField] private string _volumeUi;

    // GETTERS
    // Camera
    public string CameraSensHor => _cameraSensHor;
    public string CameraSensVer => _cameraSensVer;

    // Audio
    public string VolumeMaster => _volumeMaster;
    public string VolumeMusic => _volumeMusic;
    public string VolumeSfx => _volumeSfx;
    public string VolumeUi => _volumeUi;
}