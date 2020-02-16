using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatSelect : MonoBehaviour
{    public void GoToTarget(int noteIndex)
    {
        PlatformManager.Instance.ChangeToPlatform(noteIndex);
    }
}
