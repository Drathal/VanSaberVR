using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

public class AudioHandler : MonoBehaviour
{
    public AudioSource currentPlaybackSource;
    public static AudioHandler Instance;

    public static void OnLoad()
    {
        if (Instance != null)
        {
            return;
        }

        GameObject go = new GameObject("Audio Handler");
        AudioHandler newManager = go.AddComponent<AudioHandler>();
        newManager.currentPlaybackSource = go.AddComponent<AudioSource>();
    }

    private void Awake()
    {
        if (Instance != null)
        {
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public IEnumerator RecieveAudio(string path, Map _clip)
    {
        var newFileLoc = "file:///" + path;

        var downloadHandler2 = new DownloadHandlerAudioClip(newFileLoc, AudioType.OGGVORBIS);
        downloadHandler2.compressed = false;
        downloadHandler2.streamAudio = true;
        var uwr2 = new UnityWebRequest(
            newFileLoc,
            UnityWebRequest.kHttpVerbGET,
            downloadHandler2,
            null);

        var request2 = uwr2.SendWebRequest();

        while (!request2.isDone)
        {
            yield return null;
        }

        _clip._clip = DownloadHandlerAudioClip.GetContent(uwr2);
    }

    public float SongTime()
    {
        if (currentPlaybackSource != null)
            return currentPlaybackSource.time + Time.deltaTime;

        return 0;
    }

    public void stopAllAudio()
    {
        if (currentPlaybackSource != null)
            currentPlaybackSource.Stop();
    }

    public void setAllAudioTime(float time)
    {
        if (currentPlaybackSource != null)
            currentPlaybackSource.time = time;
    }

    public void playAllAudio(float delay)
    {
        if (currentPlaybackSource != null)
            currentPlaybackSource.PlayScheduled(AudioSettings.dspTime + delay);
    }

    public void PreviewSong(AudioClip _clip)
    {
        stopAllAudio();
        currentPlaybackSource.clip = _clip;
        currentPlaybackSource.time = 40f;
        playAllAudio(0);
    }
}