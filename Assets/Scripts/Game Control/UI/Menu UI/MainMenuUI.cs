using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
public class MainMenuUI : MonoBehaviour
{

    public GameObject title;
    public GameObject mainMenuParentContent;
    public GameObject mainMenuFadedImage;

    public List<Button> mainMenuButtons;

    public AudioMixer mainMixer;


    void Awake() {
        LoadAudioPrefs();
    }

    // Update is called once per frame
    void Update()
    {
        if((Input.anyKeyDown || Input.GetButtonDown("Submit")) && !Input.GetButton("Cancel") && title.activeSelf && !mainMenuParentContent.activeSelf) {
            ConfirmToMain();
        }

        //keyboard and mouse escape
        if(Input.GetKeyDown("escape") || Input.GetKeyDown("mouse 0")) {
            Cursor.lockState = CursorLockMode.None;
        }
    }

    public void ConfirmToMain() {
        title.SetActive(false);
        mainMenuParentContent.SetActive(true);
        mainMenuFadedImage.SetActive(false);

        mainMenuButtons[0].Select();
        mainMenuButtons[0].OnSelect(null);
    }

    public void ReturnToTitle() {
        title.SetActive(true);
        mainMenuParentContent.SetActive(false);
    }

    public void QuitGame() {
        Application.Quit();
    }

    public void ToggleMainMenuButtons(bool enabled) {
        foreach (Button b in mainMenuButtons) {
            b.interactable = enabled;
        }
    }

    public void EnableFade() {
        mainMenuFadedImage.SetActive(true);
    }

    public void HighlightMainMenu() {
        mainMenuFadedImage.SetActive(false);

        mainMenuButtons[0].Select();
        mainMenuButtons[0].OnSelect(null);
    }

    void LoadAudioPrefs() {
        if(PlayerPrefs.HasKey("MasterVolume")) {
            float vol = PlayerPrefs.GetFloat("MasterVolume");
            mainMixer.SetFloat("MasterVolume",  Mathf.Log10(vol) * 20f);
        } else {
            mainMixer.SetFloat("MasterVolume",  Mathf.Log10(0.5f) * 20f);
        }

        if(PlayerPrefs.HasKey("MusicVolume")) {
           float vol = PlayerPrefs.GetFloat("MusicVolume");
            mainMixer.SetFloat("MusicVolume",  Mathf.Log10(vol) * 20f);
        } else {
            mainMixer.SetFloat("MusicVolume",  Mathf.Log10(0.5f) * 20f);
        }

        if(PlayerPrefs.HasKey("EffectsVolume")) {
            float vol = PlayerPrefs.GetFloat("EffectsVolume");
            mainMixer.SetFloat("EffectsVolume",  Mathf.Log10(vol) * 20f);
        } else {
            mainMixer.SetFloat("EffectsVolume",  Mathf.Log10(0.5f) * 20f);
        }
    }
}
