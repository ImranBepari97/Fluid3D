using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Audio;

public class AudioOptions : MonoBehaviour
{
    public AudioMixer mixer;

    public Slider masterSlider;
    public Slider musicSlider;
    public Slider effectsSlider;


    public TMP_InputField masterInput;
    public TMP_InputField musicInput;

    public TMP_InputField effectsInput;

    // Start is called before the first frame update

    void Awake() {
        SetSliders();
    }


    private void OnEnable() {
        masterSlider.Select();     
        masterSlider.OnSelect(null);  
    }
    

    void SetSliders() {
        float masterVol;
        mixer.GetFloat("MasterVolume", out masterVol);
        masterSlider.value = masterVol;
        
        float musicVol;
        mixer.GetFloat("MusicVolume", out musicVol);
        musicSlider.value = musicVol;

        float effectsVol;
        mixer.GetFloat("EffectsVolume", out effectsVol);
        effectsSlider.value = effectsVol;


        Debug.Log("Sliders set to correct volumes");
    }

    public void SetMasterVolumeFromInput(string volString) {
        float vol = float.Parse(volString);
        float trueVol = PlayerAnimator.RangeRemap(vol, 0, 100, -80, 0);
        mixer.SetFloat("MasterVolume", trueVol);
        masterSlider.value = trueVol;
        masterInput.text = vol.ToString("0.00");
    }  

    public void SetMusicVolumeFromInput(string volString) {
        float vol = float.Parse(volString);
        float trueVol = PlayerAnimator.RangeRemap(vol, 0, 100, -80, 0);
        mixer.SetFloat("MusicVolume", trueVol);
        musicSlider.value = trueVol;
        musicInput.text = vol.ToString("0.00");
    }     

    public void SetEffectsVolumeFromInput(string volString) {
        float vol = float.Parse(volString);
        float trueVol = PlayerAnimator.RangeRemap(vol, 0, 100, -80, 0);
        mixer.SetFloat("EffectsVolume", trueVol);
        effectsSlider.value = trueVol;
        effectsInput.text = vol.ToString("0.00");
    } 


    public void SetMasterVolume(float vol) {
        mixer.SetFloat("MasterVolume", vol);
        masterSlider.value = vol;
        masterInput.text = PlayerAnimator.RangeRemap(vol, -80, 0, 0, 100).ToString("0.00");
    }  

    public void SetMusicVolume(float vol) {
        mixer.SetFloat("MusicVolume", vol);
        musicSlider.value = vol;
        musicInput.text = PlayerAnimator.RangeRemap(vol, -80, 0, 0, 100).ToString("0.00");
    }     

    public void SetEffectsVolume(float vol) {
        mixer.SetFloat("EffectsVolume", vol);
        effectsSlider.value = vol;
        effectsInput.text =  PlayerAnimator.RangeRemap(vol, -80, 0, 0, 100).ToString("0.00");
    }        
   
}
