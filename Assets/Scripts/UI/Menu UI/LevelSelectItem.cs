using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelSelectItem : MonoBehaviour
{

    public string levelSceneName;
    public string levelTitle;

    [TextArea(15,20)]
    public string levelDescription;

    public Sprite levelThumbnail;

    public LevelContentPane pane; 

    public void SetValuesInContentPane() {
        pane.SetSceneDescription(levelDescription);
        pane.SetSceneTitle(levelTitle);
        pane.SetSceneImage(levelThumbnail);
        pane.SetSceneName(levelSceneName);
    }
}
