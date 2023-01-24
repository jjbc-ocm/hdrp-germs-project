using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsUI : WindowListViewUI<SettingCategoryUI, SettingsUI>
{
    [SerializeField]
    private SettingsSO data;

    /*[Header("DELETE THESE PROPERTIES LATER")]

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
    private Slider sliderSound;*/

    [SerializeField]
    private GameObject loadIndicator;

    public SettingsData Data { get; set; }

    private bool isInitialized;


    public void OnQualityClick()
    {
        RefreshUI((self) =>
        {
            self.Data.QualityIndex = self.Data.QualityIndex + 1;
        });
    }

    public void OnResolutionClick()
    {
        RefreshUI((self) =>
        {
            self.Data.ResolutionIndex = self.Data.ResolutionIndex + 1;
        });
    }

    public void OnResolutionScaleClick()
    {
        RefreshUI((self) =>
        {
            self.Data.ResolutionScaleIndex = self.Data.ResolutionScaleIndex + 1;
        });
    }

    public void OnParticleDetailClick()
    {
        RefreshUI((self) =>
        {
            self.Data.ParticleIndex = self.Data.ParticleIndex + 1;
        });
    }

    public void OnAntiAliasingClick()
    {
        RefreshUI((self) =>
        {
            self.Data.AntiAliasingIndex = self.Data.AntiAliasingIndex + 1;
        });
    }

    public void OnPostProcessingClick()
    {
        RefreshUI((self) =>
        {
            self.Data.PostProcessingIndex = self.Data.PostProcessingIndex + 1;
        });
    }

    public void OnMusicVolumeChange(float value)
    {
        RefreshUI((self) =>
        {
            self.Data.MusicVolume = value;

            AudioManager.Instance.SetMusicVolume(value);
        });
    }

    public void OnSoundVolumeChange(float value)
    {
        RefreshUI((self) =>
        {
            self.Data.SoundVolume = value;

            AudioManager.Instance.SetSoundVolume(value);
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

        SettingsManager.Instance.ApplySettings(Data);

        loadIndicator.SetActive(false);

        RefreshUI((self) =>
        {
            self.Data = new SettingsData(APIManager.Instance.PlayerData.Settings);
        });
    }

    protected override void OnRefreshUI()
    {
        /*for (var i = 0; i < qualityOptions.Length; i++)
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

        sliderSound.value = Data.SoundVolume;*/

        var index = 0;

        RefreshItems(data.Categories, (item, data) =>
        {
            item.Data = data;

            item.GetOption =
                index == 0 ?
                    new List<Func<float>>
                    {
                        () => Data.QualityIndex,
                        () => Data.ResolutionIndex,
                        () => Data.ResolutionScaleIndex,
                        () => Data.ParticleIndex,
                        () => Data.AntiAliasingIndex,
                        () => Data.PostProcessingIndex
                    }
                : index == 1 ?
                    new List<Func<float>>
                    {
                        () => Data.MusicVolume,
                        () => Data.SoundVolume
                    }
                : // TODO: this part is about controls and not implemented yet
                    new List<Func<float>>
                    {
                        () => 0,
                        () => 0,
                        () => 0,
                        () => 0,
                        () => 0,
                        () => 0,
                        () => 0,
                        () => 0,
                        () => 0,
                    };

            item.SetOption =
                index == 0 ?
                    new List<Action<float>>
                    {
                        (value) => OnQualityClick(),
                        (value) => OnResolutionClick(),
                        (value) => OnResolutionScaleClick(),
                        (value) => OnParticleDetailClick(),
                        (value) => OnAntiAliasingClick(),
                        (value) => OnPostProcessingClick()
                    }
                : index == 1 ?
                    new List<Action<float>>
                    {
                        (value) => OnMusicVolumeChange(value),
                        (value) => OnSoundVolumeChange(value)
                    }
                : // TODO: this part is about controls and not implemented yet
                    new List<Action<float>>
                    {
                        (value) => { },
                        (value) => { },
                        (value) => { },
                        (value) => { },
                        (value) => { },
                        (value) => { },
                        (value) => { },
                        (value) => { },
                        (value) => { },
                    };

            index++;
        }, 
        isInitialized);

        isInitialized = true;
    }
}
