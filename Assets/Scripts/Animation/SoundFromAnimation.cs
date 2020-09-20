using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundFromAnimation : MonoBehaviour
{

    public List<AudioClip> footsteps;
    // Start is called before the first frame update

    public void MakeFootstepSound() {

        if(footsteps.Count == 0) {
            return;
        }

        AudioSource.PlayClipAtPoint(footsteps[Random.Range(0, footsteps.Count)], transform.position);
    }
}
