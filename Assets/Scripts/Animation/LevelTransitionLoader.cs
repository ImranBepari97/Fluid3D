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
        Debug.Log("Start called for animatio");
        transition.Rebind();
    }

    public void LoadSceneWithTransition(string sceneName) {
        StartCoroutine(LoadSceneWithTransitionCoroutine(sceneName));
    }

    IEnumerator LoadSceneWithTransitionCoroutine(string sceneName) {
        instance.transition.SetTrigger("Start");
        Time.timeScale = 1f;

        yield return new WaitForSeconds(transitionTime);

        AsyncOperation async = SceneManager.LoadSceneAsync(sceneName);
        
        while(!async.isDone) {
            progressBar.value = async.progress;

            yield return null;
        }
        instance.transition.SetTrigger("End");

    }
}
