using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class SettingsManager : MonoBehaviour
{
    public static SettingsManager Instance;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        ApplySettings(APIManager.Instance.PlayerData.Settings);
    }

    public void ApplySettings(SettingsData data)
    {
        var constants = SOManager.Instance.Constants;

        QualitySettings.SetQualityLevel(data.QualityIndex);

        var urpCamera = Camera.main.GetUniversalAdditionalCameraData();

        var urpAsset = (UniversalRenderPipelineAsset)GraphicsSettings.currentRenderPipeline;

        var resolution = constants.SettingResolutions[data.ResolutionIndex % constants.SettingResolutions.Length];

        urpAsset.renderScale = constants.SettingRenderScales[data.ResolutionScaleIndex % constants.SettingRenderScales.Length];

        urpAsset.msaaSampleCount = constants.SettingMSAA[data.AntiAliasingIndex % constants.SettingMSAA.Length];

        urpCamera.renderPostProcessing = constants.SettingPostProcessing[data.PostProcessingIndex % constants.SettingPostProcessing.Length];

        Screen.SetResolution(resolution.x, resolution.y, data.ModeIndex == 0);

        AudioManager.Instance.SetMusicVolume(data.MusicVolume);

        AudioManager.Instance.SetSoundVolume(data.SoundVolume);

        AudioManager.Instance.PlayMusic(Random.Range(0, AudioManager.Instance.MusicClips.Length));
    }
}
