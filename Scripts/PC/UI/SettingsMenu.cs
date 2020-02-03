using UnityEngine.Audio;
using UnityEngine.UI;
using UnityEngine;

public class SettingsMenu : MonoBehaviour {

    public AudioMixer audioMixer;

    const float defaultVolume = 0f;
    const float defaultBrigthness = 0.325f;
    const int defaultQuality = 5;

    public Slider volumeSlider;
    public Slider brigthnessSlider;
    public Dropdown qualityDropdown;

    private void Awake()
    {
        float volume = PlayerPrefs.GetFloat("volume", defaultVolume);
        SetVolume(volume);

        float brigthness = PlayerPrefs.GetFloat("brigthness", defaultBrigthness);
        SetBrightness(brigthness);
        
        int quality = PlayerPrefs.GetInt("quality", defaultQuality);
        SetQuality(quality);
    }

    public void SetAllDefault()
    {
        SetVolume(defaultVolume);
        SetBrightness(defaultBrigthness);
        SetQuality(defaultQuality);
    }

    public void SetVolume(float volume)
    {
        volumeSlider.value = volume;
        audioMixer.SetFloat("volume", volume);
        PlayerPrefs.SetFloat("volume", volume);
    }

    public void SetBrightness(float value)
    {
        brigthnessSlider.value = value;
        RenderSettings.ambientIntensity = value;
        PlayerPrefs.SetFloat("brigthness", value);
    }

    public void SetQuality(int qualityIndex)
    {
        qualityDropdown.value = qualityIndex;
        QualitySettings.SetQualityLevel(qualityIndex);
        PlayerPrefs.SetInt("quality", qualityIndex);
    }

}
