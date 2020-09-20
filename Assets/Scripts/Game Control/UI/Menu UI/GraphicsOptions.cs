using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GraphicsOptions : MonoBehaviour
{

    public TMP_Dropdown resolutionDropdown;
    public Toggle fullscreenToggle;
    Resolution[] resolutions;

    // Start is called before the first frame update
    void Start()
    {
        SetResolutionOptions();

        fullscreenToggle.isOn = Screen.fullScreen;
    }


    void SetResolutionOptions() {
        Resolution currentResolution = Screen.currentResolution;
        resolutions = Screen.resolutions;
        resolutionDropdown.ClearOptions();

        List<string> resOptions = new List<string>();
        for (int i = 0; i < resolutions.Length; i++) {
            resOptions.Add(resolutions[i].width + " x " + resolutions[i].height + "@" + resolutions[i].refreshRate);
            if(resolutions[i].height == currentResolution.height &&
             resolutions[i].width == currentResolution.width) {
                 resolutionDropdown.value = i;
            }
        }

        resolutionDropdown.AddOptions(resOptions);
        resolutionDropdown.RefreshShownValue();
    }

    public void ChangeResolution(int value) {
        Screen.SetResolution(resolutions[value].width, resolutions[value].height, Screen.fullScreen, resolutions[value].refreshRate);
    }

    public void ToggleFullscreen(bool value) {
        Screen.fullScreen = value;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
