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

    public GameObject optionsMenu;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if((Input.anyKeyDown || Input.GetButtonDown("Submit")) && !Input.GetButton("Cancel") && title.activeSelf && !mainMenuParentContent.activeSelf) {
            ConfirmToMain();
        }
    }

    public void ConfirmToMain() {
        title.SetActive(false);
        mainMenuParentContent.SetActive(true);
        mainMenuFadedImage.SetActive(false);

        mainMenuButtons[0].Select();
        mainMenuButtons[0].OnSelect(null);

        Debug.Log("set main");
    }

    public void ReturnToTitle() {
        title.SetActive(true);
        mainMenuParentContent.SetActive(false);
        Debug.Log("set title");
    }

    public void QuitGame() {
        Application.Quit();
    }

    public void ToggleMainMenuButtons(bool enabled) {
        foreach (Button b in mainMenuButtons) {
            b.interactable = enabled;
        }
    }

    public void DisableAllMenus() {
        mainMenuFadedImage.SetActive(false);
        optionsMenu.SetActive(false);

        mainMenuButtons[0].Select();
        mainMenuButtons[0].OnSelect(null);
    }
    public void EnableOptionsMenu() {
        optionsMenu.SetActive(true);
        mainMenuFadedImage.SetActive(true);
    }
}
