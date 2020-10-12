using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GraphicsOptions : MonoBehaviour
{

    public TMP_Dropdown resolutionDropdown;
    public TMP_Dropdown qualityDropdown;
    public Toggle fullscreenToggle;

    public Toggle vsyncToggle;
    Resolution[] resolutions;

    // Start is called before the first frame update
    void Start()
    {
        SetResolutionOptions();

        vsyncToggle.isOn = QualitySettings.vSyncCount != 0;
        fullscreenToggle.isOn = Screen.fullScreen;

        qualityDropdown.value = QualitySettings.GetQualityLevel();
    }


    void SetResolutionOptions() {
        Resolution currentResolution = Screen.currentResolution;
        resolutions = Screen.resolutions;
        resolutionDropdown.ClearOptions();

        int currentResIndex = 0;

        List<string> resOptions = new List<string>();
        for (int i = 0; i < resolutions.Length; i++) {
            resOptions.Add(resolutions[i].width + " x " + resolutions[i].height + "@" + resolutions[i].refreshRate);
            
            if(resolutions[i].height == currentResolution.height &&
             resolutions[i].width == currentResolution.width && currentResolution.refreshRate == resolutions[i].refreshRate) {
                currentResIndex = i;
            }
        }

        resolutionDropdown.AddOptions(resOptions);
        resolutionDropdown.value = currentResIndex;
        resolutionDropdown.RefreshShownValue();
    }

    public void ChangeResolution(int value) {
        Screen.SetResolution(resolutions[value].width, resolutions[value].height, Screen.fullScreen, resolutions[value].refreshRate);
    }

    public void ToggleFullscreen(bool value) {
        Screen.fullScreen = value;
    }

    public void ToggleVSync(bool value) {
        if(value) {
            QualitySettings.vSyncCount = 1;
        } else {
            QualitySettings.vSyncCount = 0;
        }
        Screen.fullScreen = fullscreenToggle.isOn;
        
    }

    // Update is called once per frame
    public void SetQualitySettings(int qualityIndex) {
        QualitySettings.SetQualityLevel(qualityIndex);
    }
}
