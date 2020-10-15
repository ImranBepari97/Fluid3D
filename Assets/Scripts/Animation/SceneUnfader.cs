using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneUnfader : MonoBehaviour
{
    //Literally only exists because unfading the scene at the right time is painful
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(WaitAndFade());
    }

    IEnumerator WaitAndFade() {
        yield return new WaitForSeconds(0.1f);
        LevelTransitionLoader.instance.PlayTransitionFadeOut();
        Destroy(this.gameObject);
    }
}
