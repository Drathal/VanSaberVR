using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvironmentSpinHandler : MonoBehaviour
{
    public static EnvironmentSpinHandler Instance;
    public GameHandler _objectHandler;
    public GameObject _spawnBar;
    float _smooth = 20;
    public float targetRotation;
    public float currentRotation;

    protected void Awake()
    {
        if (Instance != null)
        {
            return;
        }

        Instance = this;
    }

    public void StartUp(GameHandler objectHandler)
    {
        _objectHandler = objectHandler;
        currentRotation = _objectHandler._Angle;
    }

    public void Move(float angle)
    {
        targetRotation = angle;
    }

    // Update is called once per frame
    public void Update()
    {
        currentRotation = Mathf.Lerp(currentRotation, targetRotation, Time.deltaTime * _smooth);
        transform.localRotation = Quaternion.Euler(0.0f, currentRotation, 0.0f);
    }
}