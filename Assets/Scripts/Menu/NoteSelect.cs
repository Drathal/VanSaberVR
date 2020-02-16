using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteSelect : MonoBehaviour
{
    public void GoToTarget(int noteIndex)
    {
        NoteManager.Instance.ChangeToNote(noteIndex);
    }
}
