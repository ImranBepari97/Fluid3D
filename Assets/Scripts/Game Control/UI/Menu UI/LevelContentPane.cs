using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class LevelContentPane : MonoBehaviour
{

    [SerializeField]
    TMP_Text levelTitle;

    [SerializeField]
    Image levelThumbnail;

    [SerializeField]
    TMP_Text levelDesc;

    public string sceneName;

    public void SetSceneTitle(string title) {
        levelTitle.text = title;
    }

    public void SetSceneDescription(string desc) {
        levelDesc.text = desc;
    }

    public void SetSceneImage(Sprite desc) {
        levelThumbnail.sprite = desc;
    }

     public void SetSceneName(string desc) {
        sceneName = desc;
    }


    public void StartLevel() {
        SceneManager.LoadScene(sceneName);
    }
}
