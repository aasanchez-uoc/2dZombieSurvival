using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionsManager : MonoBehaviour
{
    public Slider MusicSlider;

    public Slider SoundSlider;

    void OnEnable()
    {   

        int music = PlayerPrefs.GetInt("VolumeMusic", 100);
        int sound = PlayerPrefs.GetInt("VolumeEffect", 100);

        MusicSlider.value = music;
        SoundSlider.value = sound;
    }

    public void setMusicText()
    {
        MusicSlider.GetComponentInChildren<Text>().text = MusicSlider.value.ToString();
    }
    public void setSoundText()
    {
        SoundSlider.GetComponentInChildren<Text>().text = SoundSlider.value.ToString();
    }

    public void SaveValues()
    {
        PlayerPrefs.SetInt("VolumeMusic", (int)MusicSlider.value);
        PlayerPrefs.SetInt("VolumeEffect", (int)SoundSlider.value);
        HideOptions();
    }

    public void HideOptions()
    {
        gameObject.SetActive(false);
    }

    public void ShowOptions()
    {
        gameObject.SetActive(false);
    }
}
