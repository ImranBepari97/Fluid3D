using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Audio;

public class AudioOptions : MonoBehaviour
{
    public AudioMixer mixer;

    public InputOptionCombo master;
    public InputOptionCombo music;
    public InputOptionCombo effects;


    // Start is called before the first frame update

    void Awake() {
        SetSliders();
    }


    private void OnEnable() {
        master.inputField.Select();     
        master.inputField.OnSelect(null);  
    }
    

    void SetSliders() {
        float masterVol;
        float musicVol;
        float effectsVol;

        if(PlayerPrefs.HasKey("MasterVolume")) {
            masterVol = PlayerPrefs.GetFloat("MasterVolume");
        } else {
            masterVol = 0.5f;
        }

        if(PlayerPrefs.HasKey("MusicVolume")) {
            musicVol = PlayerPrefs.GetFloat("MusicVolume");
        } else {
            musicVol = 0.5f;
        }

        if(PlayerPrefs.HasKey("EffectsVolume")) {
            effectsVol = PlayerPrefs.GetFloat("EffectsVolume");
        } else {
            effectsVol = 0.5f;
        }

        mixer.SetFloat("MasterVolume",  Mathf.Log10(masterVol) * 20f);
        master.SetTrueValue(masterVol);
        master.RefreshPercievedUI();
        
        mixer.SetFloat("MusicVolume",  Mathf.Log10(musicVol) * 20f);
        music.SetTrueValue(musicVol);
        music.RefreshPercievedUI();

        mixer.SetFloat("EffectsVolume", Mathf.Log10(effectsVol) * 20f);
        effects.SetTrueValue(effectsVol);
        effects.RefreshPercievedUI();

        Debug.Log("Sliders set to correct volumes");
    }

    public void SetMasterVolume() {
        float trueVol = master.GetTrueValue();
        mixer.SetFloat("MasterVolume", Mathf.Log10(trueVol) * 20f);
        PlayerPrefs.SetFloat("MasterVolume", trueVol);
        PlayerPrefs.Save();
    }  

    public void SetMusicVolume() {
        float trueVol = music.GetTrueValue();
        mixer.SetFloat("MusicVolume", Mathf.Log10(trueVol) * 20f);
        PlayerPrefs.SetFloat("MusicVolume", trueVol);
        PlayerPrefs.Save();
    }  

    public void SetEffectsVolume() {
        float trueVol = effects.GetTrueValue();
        mixer.SetFloat("EffectsVolume", Mathf.Log10(trueVol) * 20f);
        PlayerPrefs.SetFloat("EffectsVolume", trueVol);
        PlayerPrefs.Save();
    } 
}
