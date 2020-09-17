using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuToggler : MonoBehaviour
{

    public List<GameObject> menuContents;

    public void EnableMenu(int index) {
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
    }
}
