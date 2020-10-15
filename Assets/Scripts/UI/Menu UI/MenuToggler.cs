using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuToggler : MonoBehaviour
{

    public List<GameObject> menuContents;
    public GameObject parentContent;

    public void EnableMenu(int index) {
        if(parentContent != null) {
            parentContent.SetActive(false);
        }
        
        foreach(GameObject menu in menuContents) {
            menu.SetActive(false);
        }
        menuContents[index].SetActive(true);


        //highlight the first available selectable in the new menu
        Selectable first = menuContents[index].GetComponentInChildren<Selectable>();
        first.Select();
        first.OnSelect(null);
    }

    public void DisableAllMenus() {
        foreach(GameObject menu in menuContents) {
            menu.SetActive(false);
        }

        if(parentContent != null) {
            parentContent.SetActive(true);

            Selectable first = parentContent.GetComponentInChildren<Selectable>();
            first.Select();
            first.OnSelect(null);

        }
    }
}
