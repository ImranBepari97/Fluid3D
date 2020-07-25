using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallPlayerController : MonoBehaviour
{

    GlobalPlayerController globalPlayerController;
    Vector3 initialCollisionNormal;

    // Start is called before the first frame update
    void Start()
    {
        globalPlayerController = GetComponent<GlobalPlayerController>();
    }

    void OnEnable() {
        Debug.Log("hello");
    }

    // Update is called once per frame
    void Update()
    {
         if(Input.GetButtonDown("Jump")) {
             globalPlayerController.EnableDefaultControls();
         }
    }
}
