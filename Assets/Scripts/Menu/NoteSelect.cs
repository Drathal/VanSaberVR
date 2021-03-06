﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static HelperClass;

public class NoteSelect : MonoBehaviour
{
    public int index;
    public void GoToTarget()
    {
        NoteManager.Instance.ChangeToNote(index);
        _currentSettings.LastKnownNoteSet = NoteManager.Instance.currentNote.NoteName;
        SettingsHandler.Instance.WriteSettings(_currentSettings);
    }
}
