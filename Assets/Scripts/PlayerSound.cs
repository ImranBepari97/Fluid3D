using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSound : MonoBehaviour
{

    AudioSource au;
    public AudioClip jumpSound;
    public AudioClip dashSound;
    GlobalPlayerController gpc;


    // Start is called before the first frame update
    void Start() {
        gpc = transform.parent.GetComponent<GlobalPlayerController>();
        au = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update() {

        switch(gpc.recentAction) {
            case RecentActionType.Dash:
                au.clip = dashSound;
                au.Play();
                break;
            case RecentActionType.RegularJump:
            case RecentActionType.WallJump:
                au.clip = jumpSound;
                au.Play();
                break;
        }
    }
}
