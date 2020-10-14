using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerScoreEntry : MonoBehaviour
{
    [SerializeField] TMP_Text score;
    [SerializeField] TMP_Text displayName;
    [SerializeField] TMP_Text rank;

    public void SetScore(string score) {
        this.score.text = score;
    }

    public void SetDisplayName(string name) {
        this.displayName.text = name;
    }

    public void SetRank(string rank) {
        this.rank.text = rank;
    }
}
