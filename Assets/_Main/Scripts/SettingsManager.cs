using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class SettingsManager : MonoBehaviour
{
    // Start is called before the first frame update
    /*void Start()
    {
        QualitySettings.SetQualityLevel(0, true);

        GraphicsSettings.defaultRenderPipeline.scale
    }

    // Update is called once per frame
    void Update()
    {
        
    }*/

    /*void OnGUI()
    {
        string[] names = QualitySettings.names;
        GUILayout.BeginVertical();
        for (int i = 0; i < names.Length; i++)
        {
            if (GUILayout.Button(names[i]))
            {
                QualitySettings.SetQualityLevel(i, true);
            }
        }
        GUILayout.EndVertical();
    }*/

    public static SettingsManager Instance;

    void Awake()
    {
        Instance = this;

        ApplySettings(APIManager.Instance.PlayerData.Settings);
    }

    public void ApplySettings(SettingsData data)
    {
        QualitySettings.SetQualityLevel(data.QualityIndex);

        var urpCamera = Camera.main.GetUniversalAdditionalCameraData();

        var urpAsset = (UniversalRenderPipelineAsset)GraphicsSettings.currentRenderPipeline;

        urpAsset.renderScale = Constants.SETTINGS_RENDER_SCALE[data.ResolutionScaleIndex];

        urpAsset.msaaSampleCount = Constants.SETTINGS_MSAA[data.AntiAliasingIndex];

        urpCamera.renderPostProcessing = Constants.SETTINGS_POST_PROCESS[data.PostProcessingIndex];
    }
}
