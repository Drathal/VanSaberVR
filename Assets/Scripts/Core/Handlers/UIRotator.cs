using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static HelperClass;

public class UIRotator : MonoBehaviour
{
    public static UIRotator Instance;
    public TextMeshPro _Score;
    public TextMeshPro _Combo;
    public TextMeshPro test;
    public int score = 0;
    public int combo = 0;
    public int multiplier = 0;
    //public GameObject _spawnBar;
    public float _smooth = 2f;
    public float targetRotation;
    public float currentRotation;
    public float lastRotation;
    public float newRotation;
    protected void Awake()
    {
        if (Instance != null)
        {
            return;
        }
        test = _currentMenuObjects._TimeRemain.GetComponent<TextMeshPro>();
        _Combo = _currentMenuObjects._Combo.GetComponent<TextMeshPro>();
        _Score = _currentMenuObjects._Score.GetComponent<TextMeshPro>();
        Instance = this;
    }
    
    public void Move(float angle)
    {
        lastRotation = newRotation;
        newRotation = angle;
        targetRotation = Mathf.Lerp(lastRotation, newRotation, 0.5f);
    }

    public float newAngle = 0;

    // Update is called once per frame
    public void Update()
    {
        currentRotation = Mathf.Lerp(currentRotation, targetRotation, Time.smoothDeltaTime * _smooth);
        transform.localRotation = Quaternion.Euler(0.0f, currentRotation, 0.0f);
    }

    public void UpdateScore(int amount, bool miss = false)
    {
        if (!miss)
            combo++;
        else
            combo = 0;

        if (combo >= 3 && combo <= 7)
            multiplier = 2;

        if (combo >= 8 && combo <= 11)
            multiplier = 4;

        if (combo >= 12 && combo <= 15)
            multiplier = 6;

        if (combo >= 16)
            multiplier = 8;

        if (combo <= 2)
            multiplier = 1;

        score += amount * multiplier;

        _Score.text = score.ToString();
        _Combo.text = combo.ToString();
    }
}