using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SongSelect : MonoBehaviour
{
    public int arrayID;
    // Start is called before the first frame update
    void Start()
    {
        gameObject.GetComponent<Button>().onClick.AddListener(GoToTarget);
    }

    void GoToTarget()
    {
        /*PacketWriter RequestScores = new PacketWriter(Opcodes.REQUEST_SCORES);
        RequestScores.WriteString(_currentSettings.UniqueID);
        RequestScores.WriteString(SongList[arrayID].initialData.SongName);
        RequestScores.WriteString(SongList[arrayID].initialData.Artist);
        mSocket.SendPacket(RequestScores);*/

        CustomMenuManager.Instance.SetMap(arrayID);
        AudioHandler.Instance.PreviewSong(CustomMenuManager.Instance.CurrentMap._clip);
        CustomMenuManager.Instance.UpdateDiffs();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
