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
        QualitySettings.SetQualityLevel(data.QualityIndex);

        var urpCamera = Camera.main.GetUniversalAdditionalCameraData();

        var urpAsset = (UniversalRenderPipelineAsset)GraphicsSettings.currentRenderPipeline;

        var resolution = Constants.SETTINGS_RESOLUTION[data.ResolutionIndex];

        urpAsset.renderScale = Constants.SETTINGS_RENDER_SCALE[data.ResolutionScaleIndex];

        urpAsset.msaaSampleCount = Constants.SETTINGS_MSAA[data.AntiAliasingIndex];

        urpCamera.renderPostProcessing = Constants.SETTINGS_POST_PROCESS[data.PostProcessingIndex];

        Screen.SetResolution(resolution.x, resolution.y, true);

        //Debug.Log(data.MusicVolume + " " + data.SoundVolume);

        AudioManager.Instance.SetMusicVolume(data.MusicVolume);

        AudioManager.Instance.SetSoundVolume(data.SoundVolume);

        //Debug.Log("MUST CALL");

        AudioManager.Instance.PlayMusic(Random.Range(0, AudioManager.Instance.MusicClips.Length));
    }
}
