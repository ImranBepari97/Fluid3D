using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TabGroup : MonoBehaviour
{
    public List<TabButton> tabButtons;
    public Color tabIdle;
    public Color tabHovered;
    public Color tabActive;

    TabButton selectedTab;

    TabButton hoveredButton;

    void Update() {
        if(hoveredButton != null) {
            hoveredButton.background.color = Color.Lerp(hoveredButton.background.color, tabHovered, 0.1f);
        }
    }

    public void Subscribe(TabButton button) {
        if(tabButtons == null) {
            tabButtons = new List<TabButton>();
            tabButtons.Add(button);
            OnTabSelected(tabButtons[0]);
        } else {
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
