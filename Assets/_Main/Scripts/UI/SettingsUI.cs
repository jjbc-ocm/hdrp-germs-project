using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsUI : UI<SettingsUI>
{
    [SerializeField]
    private GameObject[] qualityOptions;

    [SerializeField]
    private GameObject[] resolutionOptions;

    [SerializeField]
    private GameObject[] resolutionScaleOptions;

    [SerializeField]
    private GameObject[] particleOptions;

    [SerializeField]
    private GameObject[] antiAliasingOptions;

    [SerializeField]
    private GameObject[] postProcessingOptions;

    [SerializeField]
    private Slider sliderMusic;

    [SerializeField]
    private Slider sliderSound;

    [SerializeField]
    private GameObject loadIndicator;

    public SettingsData Data { get; set; }


    public void OnQualityClick()
    {
        RefreshUI((self) =>
        {
            self.Data.QualityIndex = (self.Data.QualityIndex + 1) % qualityOptions.Length;
        });
    }

    public void OnResolutionClick()
    {
        RefreshUI((self) =>
        {
            self.Data.ResolutionIndex = (self.Data.ResolutionIndex + 1) % resolutionOptions.Length;
        });
    }

    public void OnResolutionScaleClick()
    {
        RefreshUI((self) =>
        {
            self.Data.ResolutionScaleIndex = (self.Data.ResolutionScaleIndex + 1) % resolutionScaleOptions.Length;
        });
    }

    public void OnParticleDetailClick()
    {
        RefreshUI((self) =>
        {
            self.Data.ParticleIndex = (self.Data.ParticleIndex + 1) % particleOptions.Length;
        });
    }

    public void OnAntiAliasingClick()
    {
        RefreshUI((self) =>
        {
            self.Data.AntiAliasingIndex = (self.Data.AntiAliasingIndex + 1) % antiAliasingOptions.Length;
        });
    }

    public void OnPostProcessingClick()
    {
        RefreshUI((self) =>
        {
            self.Data.PostProcessingIndex = (self.Data.PostProcessingIndex + 1) % postProcessingOptions.Length;
        });
    }

    public void OnMusicVolumeChange(float value)
    {
        RefreshUI((self) =>
        {
            self.Data.MusicVolume = value;

            AudioListener.volume = value;
        });
    }

    public void OnSoundVolumeChange(float value)
    {
        RefreshUI((self) =>
        {
            self.Data.SoundVolume = value;

            AudioListener.volume = value;
        });
    }

    public void OnResetClick()
    {
        RefreshUI((self) =>
        {
            self.Data = new SettingsData();
        });
    }

    public async void OnApplyClick()
    {
        loadIndicator.SetActive(true);

        await APIManager.Instance.PlayerData.SetSettings(Data).Put();

        loadIndicator.SetActive(false);
    }

    protected override void OnRefreshUI()
    {
        for (var i = 0; i < qualityOptions.Length; i++)
        {
            qualityOptions[i].SetActive(Data.QualityIndex == i);
        }

        for (var i = 0; i < resolutionOptions.Length; i++)
        {
            resolutionOptions[i].SetActive(Data.ResolutionIndex == i);
        }

        for (var i = 0; i < resolutionScaleOptions.Length; i++)
        {
            resolutionScaleOptions[i].SetActive(Data.ResolutionScaleIndex == i);
        }

        for (var i = 0; i < particleOptions.Length; i++)
        {
            particleOptions[i].SetActive(Data.ParticleIndex == i);
        }

        for (var i = 0; i < antiAliasingOptions.Length; i++)
        {
            antiAliasingOptions[i].SetActive(Data.AntiAliasingIndex == i);
        }

        for (var i = 0; i < postProcessingOptions.Length; i++)
        {
            postProcessingOptions[i].SetActive(Data.PostProcessingIndex == i);
        }

        sliderMusic.value = Data.MusicVolume;

        sliderSound.value = Data.SoundVolume;
    }
}
