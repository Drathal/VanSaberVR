using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static HelperClass;

public class SaberSelect : MonoBehaviour
{
    public int index;
    public void GoToTarget()
    {
        CustomSaberManager.Instance.ChangeToSaber(index);
        _currentSettings.LastKnownSaberSet = CustomSaberManager.Instance.currentSaber.SaberName;
        SettingsHandler.Instance.WriteSettings(_currentSettings);
    }
}
