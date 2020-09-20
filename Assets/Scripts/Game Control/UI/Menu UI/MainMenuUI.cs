using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class MainMenuUI : MonoBehaviour
{

    public GameObject title;
    public GameObject mainMenuParentContent;
    public GameObject mainMenuFadedImage;

    public List<Button> mainMenuButtons;


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
}
