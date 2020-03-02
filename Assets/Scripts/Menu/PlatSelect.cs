using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static HelperClass;

public class PlatSelect : MonoBehaviour
{
    public int index;
    public void GoToTarget()
    {
        PlatformManager.Instance.ChangeToPlatform(index);
        _currentSettings.LastKnownPlatform = PlatformManager.Instance.currentPlatform.platName;
        SettingsHandler.Instance.WriteSettings(_currentSettings);
    }
}
