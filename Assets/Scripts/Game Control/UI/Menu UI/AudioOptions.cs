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
        mixer.GetFloat("MasterVolume", out masterVol);
        master.SetTrueValue(masterVol);
        master.RefreshPercievedUI();
        
        float musicVol;
        mixer.GetFloat("MusicVolume", out musicVol);
        music.SetTrueValue(musicVol);
        music.RefreshPercievedUI();

        float effectsVol;
        mixer.GetFloat("EffectsVolume", out effectsVol);
        effects.SetTrueValue(effectsVol);
        effects.RefreshPercievedUI();

        Debug.Log("Sliders set to correct volumes");
    }

    public void SetMasterVolume() {
        float trueVol = master.GetTrueValue();
        mixer.SetFloat("MasterVolume", trueVol);
    }  

    public void SetMusicVolume() {
        float trueVol = music.GetTrueValue();
        mixer.SetFloat("MusicVolume", trueVol);
    }  

    public void SetEffectsVolume() {
        float trueVol = effects.GetTrueValue();
        mixer.SetFloat("EffectsVolume", trueVol);
    } 
}
