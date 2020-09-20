using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class TabGroup : MonoBehaviour
{
    public List<TabButton> tabButtons;
    public Color tabIdle;
    public Color tabHovered;
    public Color tabActive;

    TabButton selectedTab;

    TabButton hoveredButton;

    void Awake() {
        if(tabButtons.Count > 0) {
            OnTabSelected(tabButtons[0]);
        }   
    }

    void Update() {
        if(hoveredButton != null) {
            hoveredButton.background.color = Color.Lerp(hoveredButton.background.color, tabHovered, 0.1f);
        }

        if(Input.GetButtonDown("TabSwitchR")) {
            int sel = tabButtons.IndexOf(selectedTab);
            OnTabSelected(tabButtons[(sel + 1) % tabButtons.Count]);
        }
        
        if(Input.GetButtonDown("TabSwitchL")) {
            int sel = tabButtons.IndexOf(selectedTab);
            OnTabSelected(tabButtons[(sel - 1) % tabButtons.Count]);
        }
    }

    public void Subscribe(TabButton button) {
        if(tabButtons == null) {
            tabButtons = new List<TabButton>();
            tabButtons.Add(button);
            OnTabSelected(tabButtons[0]);
        } else {
            if(tabButtons.Contains(button)) {
                return;
            }
            tabButtons.Add(button);
        }

        
    }

    public void OnTabEnter(TabButton button) {
        ResetTabs();

        if(selectedTab == null|| button != selectedTab) {
            hoveredButton = button;
        }
    }

    public void OnTabExit(TabButton button) {
        ResetTabs();
    }

    public void OnTabSelected(TabButton button) {
        if(selectedTab != null) {
            selectedTab.Deselect();
        }
        
        selectedTab = button;
        selectedTab.Select();
        button.background.color = tabActive;
        ResetTabs();

        foreach(TabButton tb in tabButtons) {
            if(tb == selectedTab) {
                tb.toggleObject.SetActive(true);
            } else {
                tb.toggleObject.SetActive(false);
            }
        }

        Selectable first = selectedTab.toggleObject.gameObject.GetComponentInChildren<Button>();
        if(first != null) {
            first.Select();
            first.OnSelect(null);
        }
        
    }

    public void ResetTabs() {
        foreach(TabButton tb in tabButtons) {
            if(tb == selectedTab) {
                continue;
            }
            tb.background.color = tabIdle;
            hoveredButton = null;
        }
    }

    
}
