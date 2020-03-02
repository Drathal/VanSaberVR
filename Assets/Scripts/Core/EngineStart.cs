using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static HelperClass;

public class EngineStart : MonoBehaviour
{
    public static EngineStart Instance;
    public GameObject SelectorPanelPrefab;
    public GameObject CameraRig;
    public GameObject LeftController;
    public GameObject RightController;
    public GameObject PlayerHead;
    public GameObject LeftModel;
    public GameObject RightModel;
    void Start()
    {
        if (Instance != null)
        {
            return;
        }

        Instance = this;

        Application.targetFrameRate = 300;

        var mainCamera = Camera.main;
        if (mainCamera != null)
        {
            CameraPlusManager.OnLoad(mainCamera);
        }

        RightModel.SetActive(false);
        LeftModel.SetActive(false);

        AudioHandler.OnLoad();
        SettingsHandler.OnLoad();
        CustomMenuManager.OnLoad(GetChildByName(gameObject, "MainMenuSelector"), SelectorPanelPrefab);
        PlatformManager.OnLoad();
        NoteManager.OnLoad();
        CustomSaberManager.OnLoad();
    }

    public void ApplyClick()
    {
        _currentSettings.LastKnownMenu = CustomMenuManager.Instance.currentMenu.MenuName;
        SettingsHandler.Instance.WriteSettings(_currentSettings);
        CustomMenuManager.Instance.BuildMenu(GetChildByName(gameObject, "MainMenuSelector"));
    }
}
