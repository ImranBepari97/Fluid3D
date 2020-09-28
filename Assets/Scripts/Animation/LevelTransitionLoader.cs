using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelTransitionLoader : MonoBehaviour
{

    public Animator transition;
    public float transitionTime;

    public Slider progressBar;

    public static LevelTransitionLoader instance;

    void Awake() {
        if(instance != null && instance != this) {
            Debug.Log("Destroying animation instance, as one already exists");
            Destroy(this.gameObject);
        } else {
            DontDestroyOnLoad(this.gameObject);
            instance = this;
        }
    }

    void Start() {
        transition.Rebind();
    }

    public void LoadSceneWithTransition(string sceneName) {
        StartCoroutine(LoadSceneWithTransitionCoroutine(sceneName));
    }

    IEnumerator LoadSceneWithTransitionCoroutine(string sceneName) {
        instance.transition.SetTrigger("Start");
        Time.timeScale = 1f;

        yield return new WaitForSeconds(transitionTime);

        List<AsyncOperation> scenesToLoad = new List<AsyncOperation>();
        float totalProgress = 0;

        scenesToLoad.Add(SceneManager.LoadSceneAsync(sceneName));

        string[] sceneDefaultName = sceneName.Split('_');

        if(Application.CanStreamedLevelBeLoaded(sceneDefaultName[0] + "_Geometry")) {
            scenesToLoad.Add(SceneManager.LoadSceneAsync(sceneDefaultName[0] + "_Geometry", LoadSceneMode.Additive));
        }
        
        for(int i = 0; i < scenesToLoad.Count; ++i) {
            while(!scenesToLoad[i].isDone) {
                totalProgress += scenesToLoad[i].progress;
                progressBar.value = totalProgress / scenesToLoad.Count;
                yield return null;
            } 
        }
        
        instance.transition.SetTrigger("End");

    }
}
