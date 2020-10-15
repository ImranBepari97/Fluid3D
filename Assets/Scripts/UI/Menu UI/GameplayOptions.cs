using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameplayOptions : MonoBehaviour
{
    public TMP_InputField mouseXInput;
    public TMP_InputField mouseYInput;

    public TMP_InputField stickXInput;

    public TMP_InputField stickYInput;

    public Toggle stickInverted;
    public Toggle mouseInverted;

    // Start is called before the first frame update
    void Awake()
    {
        LoadPlayerPrefs();
    }

    void LoadPlayerPrefs() {
        if(PlayerPrefs.HasKey("mouseXInput")) {
            mouseXInput.text = PlayerPrefs.GetFloat("mouseXInput").ToString("0.00");;
        } else {
            mouseXInput.text = "1.00";
        }

        if(PlayerPrefs.HasKey("mouseYInput")) {
            mouseYInput.text = PlayerPrefs.GetFloat("mouseYInput").ToString("0.00");;
        } else {
            mouseYInput.text = "1.00";
        }

        if(PlayerPrefs.HasKey("stickXInput")) {
            stickXInput.text = PlayerPrefs.GetFloat("stickXInput").ToString("0.00");;
        } else {
            stickXInput.text = "1.00";
        }

        if(PlayerPrefs.HasKey("stickYInput")) {
            stickYInput.text = PlayerPrefs.GetFloat("stickYInput").ToString("0.00");;
        } else {
            stickYInput.text = "1.00";
        }

        if(PlayerPrefs.HasKey("stickInverted") && PlayerPrefs.GetInt("stickInverted") != 0) {
            stickInverted.isOn = true;
        } else {
            stickInverted.isOn = false;
        }

        if(PlayerPrefs.HasKey("mouseInverted") && PlayerPrefs.GetInt("mouseInverted") != 0) {
            mouseInverted.isOn = true;
        } else {
            mouseInverted.isOn = false;
        }
    }

    public void SavePlayerPrefs() {
        PlayerPrefs.SetFloat("mouseXInput", float.Parse(mouseXInput.text));
        PlayerPrefs.SetFloat("mouseYInput", float.Parse(mouseYInput.text));
        PlayerPrefs.SetFloat("stickXInput", float.Parse(stickXInput.text));
        PlayerPrefs.SetFloat("stickYInput", float.Parse(stickYInput.text));

        PlayerPrefs.SetInt("mouseInverted", mouseInverted.isOn ? 1 : 0);
        PlayerPrefs.SetInt("stickInverted", stickInverted.isOn ? 1 : 0);
    }
}
