using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static HelperClass;

public class EngineStart : MonoBehaviour
{
    public GameObject SelectorPanelPrefab;
    void Start()
    {
        var mainCamera = Camera.main;
        if (mainCamera != null)
        {
            CameraPlusManager.OnLoad(mainCamera);
        }

        AudioHandler.OnLoad();
        SettingsHanler.OnLoad();
        CustomMenuManager.OnLoad(GetChildByName(gameObject, "MainMenuSelector"), SelectorPanelPrefab);
        PlatformManager.OnLoad();
        NoteManager.OnLoad();
    }

    public void ApplyClick()
    {
        _currentSettings.LastKnownMenu = CustomMenuManager.Instance.currentMenu.MenuName;
        SettingsHanler.Instance.WriteSettings(_currentSettings);
        CustomMenuManager.Instance.BuildMenu(GetChildByName(gameObject, "MainMenuSelector"));
    }
}
