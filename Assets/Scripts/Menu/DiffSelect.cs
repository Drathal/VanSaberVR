using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static HelperClass;

public class DiffSelect : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GoToTarget(DifficultyBeatmapSets set, DifficultyBeatmap difficulty)
    {
        CustomMenuManager.Instance.CurrentMap.TargetBeatMapSet = set;
        CustomMenuManager.Instance.CurrentMap.TargetDifficulty = difficulty;

        _currentMenuObjects._PlayButton.GetComponent<Button>().interactable = true;
        _currentMenuObjects._EditorButton.GetComponent<Button>().interactable = true;
    }
}
