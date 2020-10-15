using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Mirror;

public class LevelTransitionLoader : MonoBehaviour {

    public Animator transition;
    public float transitionTime;

    public Slider progressBar;

    bool isLoadingAdditiveScene;

    public static LevelTransitionLoader instance;

    void Awake() {
        if (instance != null && instance != this) {
            Debug.Log("Destroying animation instance, as one already exists");
            Destroy(this.gameObject);
        } else {
            DontDestroyOnLoad(this.gameObject);
            instance = this;
            isLoadingAdditiveScene = false;
        }
    }

    // private void OnEnable() {
    //     //SceneManager.sceneLoaded += OnSceneLoaded;
    //     SceneManager.activeSceneChanged += OnNewActiveScene;
    // }

    // private void OnDisable() {
    //     //SceneManager.sceneLoaded -= OnSceneLoaded;
    //     SceneManager.activeSceneChanged -= OnNewActiveScene;
    // }


    // void OnNewActiveScene(Scene current, Scene next) {

    //     Debug.Log("new active scene time");
    //     // string[] sceneDefaultName = next.name.Split('_');

    //     // if (Application.CanStreamedLevelBeLoaded(sceneDefaultName[0] + "_Geometry")) {
    //     //     Debug.Log("New active scene has an additive with it, so set the wait for fading.");
    //     //     isLoadingAdditiveScene = true;
    //     // } else {
    //     //     Debug.Log("No additive scene for this loaded scene, fading now.");
    //     // }
    // }

    // void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
    //     if(!isLoadingAdditiveScene) {
    //         Debug.Log("This has no additive scene, fade out");
    //         StartCoroutine(WaitBeforeFadeOut());
    //     } else if(mode == LoadSceneMode.Additive) {
    //         Debug.Log("Additive scene has loaded, fade out");
    //         StartCoroutine(WaitBeforeFadeOut());
    //         isLoadingAdditiveScene = false;
    //     } else {
    //         Debug.Log("This scene has additive waiting, dont fade");
    //     }
    // }

    // IEnumerator WaitBeforeFadeOut() {
    //     yield return new WaitForSeconds(0.2f);
    //     PlayTransitionFadeOut();
    // }

    public void LoadSceneWithTransition(string sceneName) {
        StartCoroutine(LoadSceneWithTransitionCoroutine(sceneName));
    }

    /// <summary>
    /// Fades the scene to black.
    /// </summary>
    public void PlayTransitionFadeIn() {
        Debug.Log("Fading to black");
        instance.transition.ResetTrigger("End");
        instance.transition.SetTrigger("Start");
        Time.timeScale = 1f;
    }

    /// <summary>
    /// Unfades the scene.
    /// </summary>
    public void PlayTransitionFadeOut() {
        Debug.Log("Unfading scene");
        instance.transition.ResetTrigger("Start");
        instance.transition.SetTrigger("End");
    }

    IEnumerator LoadSceneWithTransitionCoroutine(string sceneName) {
        PlayTransitionFadeIn();

        yield return new WaitForSeconds(transitionTime);

        List<AsyncOperation> scenesToLoad = new List<AsyncOperation>();
        float totalProgress = 0;

        scenesToLoad.Add(SceneManager.LoadSceneAsync(sceneName));

        string[] sceneDefaultName = sceneName.Split('_');

        if (Application.CanStreamedLevelBeLoaded(sceneDefaultName[0] + "_Geometry")) {
            scenesToLoad.Add(SceneManager.LoadSceneAsync(sceneDefaultName[0] + "_Geometry", LoadSceneMode.Additive));
        }

        for (int i = 0; i < scenesToLoad.Count; ++i) {
            while (!scenesToLoad[i].isDone) {
                totalProgress += scenesToLoad[i].progress;
                progressBar.value = totalProgress / scenesToLoad.Count;
                yield return null;
            }
        }

        //PlayTransitionFadeOut();
    }
}
