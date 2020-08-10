using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSound : MonoBehaviour
{

    public AudioClip jumpSound;
    public AudioClip dashSound;
    GlobalPlayerController gpc;

    bool canPlaySound;


    // Start is called before the first frame update
    void Start() {
        gpc = transform.parent.GetComponent<GlobalPlayerController>();

        canPlaySound = true;
    }

    // Update is called once per frame
    void Update() { 



        switch (gpc.recentAction) {
            case RecentActionType.None:
                canPlaySound = true;
                break;
            case RecentActionType.Dash:
                if (canPlaySound) { 
                    AudioSource.PlayClipAtPoint(dashSound, transform.position);
                    canPlaySound = false;
                }
                break;
            case RecentActionType.RegularJump:
                if (canPlaySound) {
                    AudioSource.PlayClipAtPoint(jumpSound, transform.position);
                    canPlaySound = false;
                }
                break;
            case RecentActionType.SlideJump:
            case RecentActionType.WallJump:
                if (canPlaySound) {
                    AudioSource.PlayClipAtPoint(jumpSound, transform.position);
                    canPlaySound = false;
                }
                break;
        }
    }
}
