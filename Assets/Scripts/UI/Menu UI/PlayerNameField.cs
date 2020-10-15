using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PlayerNameField : MonoBehaviour
{

    [SerializeField]
    private TMP_InputField nameInput;

    [SerializeField]
    private Button continueButton;

    Regex rx = new Regex(@"^[A-Za-z0-9]+(?:[ _-][A-Za-z0-9]+)*$");


    public static string DisplayName {get; set;}

    private const string PLAYER_PREFS_NAME_KEY = "PlayerName";
    // Start is called before the first frame update
    void Start()
    {
        if(!PlayerPrefs.HasKey(PLAYER_PREFS_NAME_KEY)) {
            return;
        }

        string defaultName = PlayerPrefs.GetString(PLAYER_PREFS_NAME_KEY);
        nameInput.text = defaultName;
        SetPlayerName(defaultName);
    }

    public void SetPlayerName(string name) {
        continueButton.interactable = ValidateName(name);
    }

    public bool ValidateName(string name) {
        if(rx.IsMatch(name))
            return true;

        return false;
    }

    public void SavePlayerName() {
        DisplayName = nameInput.text;
        PlayerPrefs.SetString(PLAYER_PREFS_NAME_KEY, DisplayName);
    }
}